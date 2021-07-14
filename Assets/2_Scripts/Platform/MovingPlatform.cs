using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private float goSpeed = 4f;
    private float backSpeed = 1.5f; //(구현x)갔다가 올 때 속도 다르면 좋겠다고 할 수도 있어서 변수 2개 만듦
    private float delayTime = 1f; //(구현x)발판 밟을 때 살짝 기다리는 시간
    private bool canMove = false;

    [Header("Positions")]
    [SerializeField] protected Transform startPos;
    [SerializeField] protected Transform goalPos;

    private Vector3 _nextPos;

    private void FixedUpdate()
    {
        SetNextPosition();
        
        if (canMove)
        {
            StartCoroutine("MovePlatform");
        }
        else
        {
            StopCoroutine("MovePlatform");
        }
    }

    IEnumerator MovePlatform()
    {
        yield return new WaitForSeconds(delayTime);
        
        //목표지점까지 current 지점을 직선이동
        transform.position = Vector3.MoveTowards(transform.position, _nextPos, goSpeed * Time.deltaTime);
        
        if (gameObject.transform.position == startPos.position)
        {
            canMove = false;
        }
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            canMove = true;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Player") && gameObject.transform.position == startPos.position)
        {
            canMove = true;
        }
    }

    //플레이어가 moving platform 위에 올라가있으면 발판과 같이 움직임
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.transform.SetParent(transform);
        }
    }
    
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            col.transform.SetParent(null);
        }
    }
}
