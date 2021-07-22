using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingTrap : MonoBehaviour
{
    private float speed = 6f;
    private float delayTime = 0.25f; //(구현x)발판 밟을 때 살짝 기다리는 시간
    private bool canMove = true;

    private GameObject playerObj;
    
    [Header("Positions")]
    [SerializeField] protected Transform startPos;
    [SerializeField] protected Transform goalPos;

    private Vector3 _nextPos;

    private void Start()
    {
        playerObj = GameObject.FindWithTag("Player");
    }

    private void Update()
    {
        SetNextPosition();
        
        if(canMove)
            StartCoroutine("MovePlatform");
    }

    IEnumerator MovePlatform()
    {
        if (gameObject.transform.position == startPos.position || gameObject.transform.position == goalPos.position)
        {
            yield return new WaitForSeconds(delayTime);
        }
        
        //목표지점까지 current 지점을 직선이동
        transform.position = Vector3.MoveTowards(transform.position, _nextPos, speed * Time.deltaTime);
    }

    void SetNextPosition()
    {
        if (gameObject.transform.position == startPos.position)
        {
            _nextPos = goalPos.position;
        }
        else if (gameObject.transform.position == goalPos.position)
        {
            _nextPos = startPos.position;
        }
    }

    //플레이어가 moving platform 위에 올라가있으면 발판과 같이 움직임
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            playerObj.GetComponent<PlayerController>().Die();
        }
    }
}
