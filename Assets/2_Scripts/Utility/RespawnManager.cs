using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnManager : MonoBehaviour
{
    private GameObject respawnPoint;
    private GameObject playerObj;

    
    
    private void Start()
    {
        if (playerObj == null)
        {
            playerObj = GameObject.FindGameObjectWithTag("Player");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(MoveTo_respawnPoint());
        }
    }

    private IEnumerator MoveTo_respawnPoint()
    {
        yield return null;
        playerObj.transform.position = respawnPoint.transform.position;
    }
    
    
    // GETTERS && SETTERS
    public GameObject RespawnPoint
    {
        get => respawnPoint;
        set => respawnPoint = value;
    }
}
