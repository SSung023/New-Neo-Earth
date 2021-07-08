using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapObject : MonoBehaviour
{
    private GameObject playerObj;
    private void Start()
    {
        playerObj = GameObject.FindWithTag("Player");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerObj.GetComponent<PlayerController>().Die();
        }
    }
}
