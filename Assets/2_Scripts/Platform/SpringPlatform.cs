using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringPlatform : MonoBehaviour
{
    private float jumpForce = 9f;
    
    private GameObject playerObj;
    private Rigidbody2D playerRb;
    
    private void Start()
    {
        playerObj = GameObject.FindWithTag("Player");
        playerRb = playerObj.GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            StartCoroutine("Spring");
        }
    }

    IEnumerator Spring()
    {
        playerRb.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.05f);
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
    }
}
