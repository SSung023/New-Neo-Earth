using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.AssetImporters;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    public GameObject virtualCam;
    public GameObject playerObj;
    
    public Transform spawnPointObj;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Respawn());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            playerObj = collision.gameObject;
            
            //카메라, 스폰 포인트 오브젝트 관리
            virtualCam.SetActive(true);
            spawnPointObj.gameObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            //카메라, 스폰 포인트 오브젝트 관리
            virtualCam.SetActive(false);
            spawnPointObj.gameObject.SetActive(false);
            
            /*StopCoroutine(Respawn());*/
        }
    }

    IEnumerator Respawn()
    {
        yield return null;
        playerObj.transform.position = spawnPointObj.transform.position;
    }
}
