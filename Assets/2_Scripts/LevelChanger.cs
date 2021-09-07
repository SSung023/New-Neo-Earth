using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
    public GameObject virtualCam;
    public Transform spawnPointObj;

    private void Update()
    {
        spawnPointObj = transform.GetChild(1);
    }

    private void OnTriggerStay2D(Collider2D curLevel)
    {
        if (curLevel.CompareTag("Player") && !curLevel.isTrigger)
        {
            virtualCam.SetActive(true);
            spawnPointObj.gameObject.SetActive(true);
        }
    }
    
    private void OnTriggerExit2D(Collider2D curLevel)
    {
        if (curLevel.CompareTag("Player") && !curLevel.isTrigger)
        {
            virtualCam.SetActive(false);
            spawnPointObj.gameObject.SetActive(false);
        }
    }
}
