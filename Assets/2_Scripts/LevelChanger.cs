using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    private GameObject virtualCam;
    private Transform spawnPointObj;


    private void Start()
    {
        virtualCam = transform.GetChild(0).gameObject;
        spawnPointObj = transform.GetChild(1).gameObject.transform;
    }
    private void Update()
    {

    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            //카메라, 스폰 포인트 오브젝트 관리
            virtualCam.SetActive(true);
            spawnPointObj.gameObject.SetActive(true);

            // 해당 섹터에 들어갔을 때 respawnManager의 respawnPoint 갱신
            GameManager.instance.respawnManager.RespawnPoint = spawnPointObj.gameObject;
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            //카메라, 스폰 포인트 오브젝트 관리
            virtualCam.SetActive(false);
            spawnPointObj.gameObject.SetActive(false);
        }
    }
}
