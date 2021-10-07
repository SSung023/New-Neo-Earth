using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnChanger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            // 해당 섹터에 들어갔을 때 respawnManager의 respawnPoint 갱신
            GameManager.instance.respawnManager.RespawnPoint = gameObject;
        }
    }
}
