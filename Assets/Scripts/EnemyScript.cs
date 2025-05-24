using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    char[] guessGrid;
    private int guess = 0;

    public GameObject enemyMissilePrefab;
    public GameManager gameManager;

    private void Start()
    {
        guessGrid = Enumerable.Repeat('o', 100).ToArray(); // o - open, m - miss, h - hit, x - sunk
    }

    public List<int[]> PlaceEnemyShips()
    {
        List<int[]> enemyShips = new List<int[]>
        {
            new int[]{-1, -1, -1, -1, -1},
            new int[]{-1, -1, -1, -1},
            new int[]{-1, -1, -1},
            new int[]{-1, -1, -1},
            new int[]{-1, -1}
        };

        int[] gridNumbers = Enumerable.Range(1, 100).ToArray();

        foreach (int[] ship in enemyShips)
        {
            bool placed = false;
            while (!placed)
            {
                int nose = Random.Range(0, 100);
                int vertical = Random.Range(0, 2);
                int step = vertical == 1 ? 10 : 1;

                placed = true;
                for (int i = 0; i < ship.Length; i++)
                {
                    int pos = nose + step * i;
                    if (pos >= 100 || gridNumbers[pos] == -1 ||
                        (step == 1 && pos / 10 != nose / 10)) // horizontal row check
                    {
                        placed = false;
                        break;
                    }
                }

                if (placed)
                {
                    for (int i = 0; i < ship.Length; i++)
                    {
                        int pos = nose + step * i;
                        ship[i] = pos + 1; // from 0-based to 1-based
                        gridNumbers[pos] = -1;
                    }
                }
            }
        }

        return enemyShips;
    }

    public void NPCTurn()
    {
        List<int> hitIndex = new List<int>();
        for (int i = 0; i < guessGrid.Length; i++)
        {
            if (guessGrid[i] == 'h') hitIndex.Add(i);
        }

        guess = GetNextGuess(hitIndex); 

        if (guess < 0 || guess >= gameManager.allTileScripts.Count)
        {
            return;
        }

        TileScript tileScript = gameManager.allTileScripts[guess];
        GameObject tile = tileScript.gameObject;

        guessGrid[guess] = 'm';
        Vector3 vec = tile.transform.position + Vector3.up * 15;
        GameObject missile = Instantiate(enemyMissilePrefab, vec, enemyMissilePrefab.transform.rotation);
        missile.GetComponent<EnemyMissileScript>().SetTarget(guess);
        missile.GetComponent<EnemyMissileScript>().targetTileLocation = tile.transform.position;

    }


    private int GetNextGuess(List<int> hits)
    {
        if (hits.Count > 1)
        {
            int step = hits[1] - hits[0];
            int next = hits.Last() + step;
            if (IsValidGuess(next)) return next;
            next = hits.First() - step;
            if (IsValidGuess(next)) return next;
        }

        if (hits.Count == 1)
        {
            int[] directions = { 1, -1, 10, -10 };
            directions = directions.OrderBy(x => Random.value).ToArray(); // перемішати

            foreach (int dir in directions)
            {
                int next = hits[0] + dir;
                if (IsValidGuess(next)) return next;
            }
        }

        int rand;
        do { rand = Random.Range(0, 100); } while (guessGrid[rand] != 'o');
        return rand;
    }

    private bool IsValidGuess(int index)
    {
        return index >= 0 && index < 100 && guessGrid[index] == 'o';
    }

    public void MissileHit(int hit)
    {
        guessGrid[hit] = 'h';
        Debug.Log("💥 Влучання в " + hit);
        Invoke("EndTurn", 1.0f);
    }

    public void SunkPlayer()
    {
        for (int i = 0; i < guessGrid.Length; i++)
            if (guessGrid[i] == 'h') guessGrid[i] = 'x';
    }

    public void PauseAndEnd(int miss)
    {
        if (miss >= 0 && miss < 100)
        {
            guessGrid[miss] = 'm';
            Debug.Log("❌ Промах по " + miss);
        }
        Invoke("EndTurn", 1.0f);
    }

    private void EndTurn()
    {
        gameManager.EndEnemyTurn();
    }
}
