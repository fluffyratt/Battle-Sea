using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    GameManager gameManager;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

    }
    private void OnCollisionEnter(Collision collision)
    {
        gameManager.CheckHit(collision.gameObject);
        Destroy(gameObject);
    }



}
