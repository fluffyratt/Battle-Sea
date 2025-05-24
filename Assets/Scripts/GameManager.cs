﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    [Header("Ships")]
    public GameObject[] ships;
    public EnemyScript enemyScript;
    private ShipScript shipScript;
    private List<int[]> enemyShips;
    private int shipIndex = 0;
    public List<TileScript> allTileScripts;


    [Header("HUD")]
    public Button nextBtn;
    public Button rotateBtn;
    public Button replayBtn;
    public TMP_Text topText;
    public TMP_Text playerShipText;
    public TMP_Text enemyShipText;

    [Header("Objects")]
    public GameObject missilePrefab;
    public GameObject enemyMissilePrefab;
    public GameObject firePrefab;
    public GameObject woodDock;


    private bool setupComplete = false;
    private bool playerTurn = true;


    private List<GameObject> playerFires = new List<GameObject>();
    private List<GameObject> enemyFires =  new List<GameObject>();
   

    private int enemyShipCount = 5;
    private int playerShipCount = 5;

    private List<Vector3> occupiedPositions = new List<Vector3>();
    private Renderer shipRenderer;
    private Color defaultColor;

    void Start()
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipRenderer = ships[shipIndex].GetComponent<Renderer>();
        defaultColor = shipRenderer.material.color;

        nextBtn.onClick.AddListener(() => NextShipClicked());
        rotateBtn.onClick.AddListener(() => RotateClicked());
        replayBtn.onClick.AddListener(() => ReplayClicked());
        enemyShips = enemyScript.PlaceEnemyShips();


        TileScript[] foundTiles = FindObjectsOfType<TileScript>();
        allTileScripts = new List<TileScript>();
        for (int i = 0; i < foundTiles.Length; i++)
        {
            foundTiles[i].tileIndex = i;
            allTileScripts.Add(foundTiles[i]);
        }
    }

    private void NextShipClicked()
    {
        if (!shipScript.OnGameBoard())
        {
            shipScript.FlashColor(Color.red);
            shipRenderer = ships[shipIndex].GetComponent<Renderer>();
            defaultColor = shipRenderer.material.color;
        }
        else
        {
            if (shipIndex <= ships.Length - 2)
            {
                shipIndex++;
                shipScript = ships[shipIndex].GetComponent<ShipScript>();
                shipScript.FlashColor(Color.yellow);
            }
            else
            {
                rotateBtn.gameObject.SetActive(false);
                nextBtn.gameObject.SetActive(false);
                woodDock.SetActive(false);
                topText.text = "Try to guess an enemy tile.";
                setupComplete = true;
                for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
            }
        }
        
    }


    public void TileClicked(GameObject tile)
    {
        if (setupComplete && playerTurn)
        {
            Vector3 tilePos = tile.transform.position;
            tilePos.y += 15;
            playerTurn = false;
            Instantiate(missilePrefab, tilePos, missilePrefab.transform.rotation);
        }
        else if (!setupComplete)
        {
            PlaceShip(tile);
            shipScript.SetClickedTile(tile);
        }

    }

    private void PlaceShip(GameObject tile)
    {
        shipScript = ships[shipIndex].GetComponent<ShipScript>();
        shipScript.ClearTileList();
        Vector3 newVec = shipScript.GetOffsetVec(tile.transform.position);

        bool isHorizontal = Mathf.Approximately(ships[shipIndex].transform.eulerAngles.y, 90f);
        int length = Mathf.RoundToInt(ships[shipIndex].transform.localScale.x);

        List<Vector3> proposedPositions = new List<Vector3>();

        for (int i = 0; i < length; i++)
        {
            float x = newVec.x + (isHorizontal ? i : 0);
            float z = newVec.z + (isHorizontal ? 0 : i);
            proposedPositions.Add(new Vector3(x, newVec.y, z));

            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dz = -1; dz <= 1; dz++)
                {
                    Vector3 checkPos = new Vector3(x + dx, newVec.y, z + dz);
                    if (occupiedPositions.Contains(checkPos))
                    {
                        shipRenderer.material.color = Color.blue; 
                        Invoke("ResetShipColor", 0.5f); 
                        return;
                    }
                }
            }
        }

        ships[shipIndex].transform.localPosition = newVec;

        foreach (var pos in proposedPositions)
        {
            occupiedPositions.Add(pos);
        }
    }

    private void ResetShipColor()
    {
        shipRenderer.material.color = defaultColor;
    }

    void RotateClicked()
    {
        shipScript.RotateShip();
    }

    public void CheckHit(GameObject tile)
    {
        int tileNum = Int32.Parse(Regex.Match(tile.name, @"\d+").Value);
        int hitCount = 0;
        foreach (int[] tileNumArray in enemyShips)
        {
            if(tileNumArray.Contains(tileNum))
            {
                for (int i = 0; i < tileNumArray.Length; i++)
                {
                    if (tileNumArray[i] == tileNum)
                    {
                        tileNumArray[i] = -5;
                        hitCount++;
                    }
                    else if (tileNumArray[i] == -5)
                    {
                        hitCount++;
                    }
                }
                if (hitCount == tileNumArray.Length)
                {
                    enemyShipCount--;
                    topText.text = "The ship sank!";
                    enemyFires.Add(Instantiate(firePrefab, tile.transform.position, Quaternion.identity));
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(68, 0, 0, 255)); // темно червоний
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                else
                {
                    topText.text = "Hit!!";
                    tile.GetComponent<TileScript>().SetTileColor(1, new Color32(255, 0, 0, 255)); // червоний
                    tile.GetComponent<TileScript>().SwitchColors(1);
                }
                break;
            }
           
        }
        if (hitCount == 0)
        {
           tile.GetComponent<TileScript>().SetTileColor(1, new Color32(38, 57, 76, 255)); // сірий
           tile.GetComponent<TileScript>().SwitchColors(1);
            topText.text = "Missed, there is no ship there.";
        }
        Invoke("EndPlayerTurn", 1.0f);
    }

    public void EnemyHitPlayer(Vector3 tile, int tileNum, GameObject hitObj)
    {  enemyScript.MissileHit(tileNum);
              tile.y += 0.2f;
              playerFires.Add(Instantiate(firePrefab, tile, Quaternion.identity));
             if (hitObj.GetComponent<ShipScript>().HitCheckSank())
             {
                playerShipCount--;
                playerShipText.text = playerShipCount.ToString();
                enemyScript.SunkPlayer();
             }
             Invoke("EndEnemyTurn", 2.0f);
    }

    private void EndPlayerTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(true);
        foreach (GameObject fire in playerFires) fire.SetActive(true);
        foreach (GameObject fire in enemyFires) fire.SetActive(false);
        enemyShipText.text = enemyShipCount.ToString();
        topText.text = "            Enemy's turn";
        enemyScript.NPCTurn();
        ColorAllTiles(0);
        if (playerShipCount < 1) GameOver("ENEMY WINs !! ");
    }

    public void EndEnemyTurn()
    {
        for (int i = 0; i < ships.Length; i++) ships[i].SetActive(false);
        foreach (GameObject fire in playerFires) fire.SetActive(false);
        foreach (GameObject fire in enemyFires) fire.SetActive(true);
        playerShipText.text = playerShipCount.ToString();
        topText.text = "Select a tile";
        playerTurn = true;
        ColorAllTiles(1);
        if (enemyShipCount < 1) GameOver("YOU WIN!! ");
    }

    private void ColorAllTiles(int colorIndex)
    {
        foreach (TileScript tileScript in allTileScripts)
        {
            tileScript.SwitchColors(colorIndex);
        }
    }

    void GameOver(string winner)
    {
        topText.text = "Game Over: " + winner;
        replayBtn.gameObject.SetActive(true);
        playerTurn = false;
    }
    void ReplayClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
