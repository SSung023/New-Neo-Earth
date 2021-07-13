using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    private PlayerMove playerMove;
    
    private Transform[] wallCheckTransform;

    private bool isAlive;
    private int curHealth;

    
    
    
    private void Awake()
    {
        wallCheckTransform = new Transform[2];
        wallCheckTransform[0] = transform.GetChild(0);
        wallCheckTransform[1] = transform.GetChild(1);
        
        playerMove = new PlayerMove(playerData, transform, wallCheckTransform);
        playerMove.Rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        isAlive = true;

    }
    private void Update()
    {
        if (isAlive)
        {
            playerMove.UpdateMovement();
        }
        RestartGame();
    }
    

    public void TakeHit(int damage)
    {
        var finalHealth = curHealth - damage;
        
        if (finalHealth >= 0)
        {
            Die();
        }
        else
        {
            curHealth = finalHealth;
        }
    }
    
    //플레이어 사망시 실행되는 메서드
    public void Die()
    {
        Debug.Log("플레이어 사망");
        
        // 임시 코드
        isAlive = false;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        var _color = spriteRenderer.color;
        _color.a = 0f;
        spriteRenderer.color = _color;
        
        //gameObject.SetActive(false);
    }
    
    // 임시로 넣은 기능. R키를 누르면 재시작
    private void RestartGame()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(0);
        }
    }
}
