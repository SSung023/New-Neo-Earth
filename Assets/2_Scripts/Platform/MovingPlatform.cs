using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private float goSpeed = 2f;
    private float backSpeed = 1.5f;
    private float delayTime;
    private bool isMoving = false;

    [Header("Positions")]
    [SerializeField] protected Transform startPos;
    [SerializeField] protected Transform pos1;
    [SerializeField] protected Transform pos2;
    
    private Vector3 nextPos;

    private void FixedUpdate()
    {
        SetNextPosition();
        
        if (isMoving)
        {
            MovePlatform();
        }
        
        Debug.Log("isMoving : " + isMoving);
    }

    void MovePlatform()
    {
        //목표지점까지 current 지점을 직선이동
        transform.position = Vector3.MoveTowards(transform.position, nextPos, goSpeed * Time.deltaTime);
        
        if (gameObject.transform.position == startPos.position)
        {
            isMoving = false;
        }
    }
    
    void SetNextPosition()
    {
        if (gameObject.transform.position == pos1.position)
        {
            nextPos = pos2.position;
        }
        else if (gameObject.transform.position == pos2.position)
        {
            nextPos = pos1.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            isMoving = true;
        }
    }
}
