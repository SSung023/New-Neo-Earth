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

    //private GameManager _gameManager;
    
    private bool isAlive;
    private int curHealth;
    private int curScene;
    
    
    
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
        curHealth = playerData.getMaxHp;
        
    }
    private void Update()
    {
        if (isAlive)
        {
            playerMove.UpdateMovement();
            
            if (playerMove.WallCoroutineStart)
            {
                StartCoroutine(wallJump_changeValue());
            }
            if (playerMove.DashCoroutineStart)
            {
                StartCoroutine(dash_controlRigid());
            }
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
    }
    
    // 임시로 넣은 기능. R키를 누르면 재시작
    private void RestartGame()
    {
        curScene = SceneManager.GetActiveScene().buildIndex;
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(curScene);
        }
    }

    IEnumerator wallJump_changeValue()
    {
        // 벽에서 점프했을 때 실행되는 코루틴
        // 일정 시간동안 플레이어의 조작을 막아서 벽 점프가 정상적으로 실행되도록 한다
        playerMove.WallCoroutineStart = false;
        playerMove.CanBasicMove = false;
        playerMove.IsParkourDoing = true; // 특수 동작 체공 상태 true
        
        yield return new WaitForSeconds(0.3f);
        
        playerMove.IsWallJumping = false;
        playerMove.CanBasicMove = true;
    }

    IEnumerator dash_controlRigid()
    {
        // 대쉬를 했을 때 실행되는 코루틴
        // 일정시간 동안 중력을 0으로 설정 & 대쉬가 끝나면 자연스럽게 떨어지도록 중력 설정
        playerMove.DashCoroutineStart = false;
        playerMove.Rigidbody.gravityScale = 0f;
        playerMove.CanBasicMove = false;
        playerMove.IsParkourDoing = true;
        
        yield return new WaitForSeconds(playerData.getDashDuration);
        
        playerMove.IsWallJumping = false;
        playerMove.CanBasicMove = true;
        playerMove.Rigidbody.velocity = new Vector2(playerMove.DashVector.x * 3f, 0);
        StartCoroutine(controlGravity());
    }

    IEnumerator controlGravity()
    {
        playerMove.Rigidbody.gravityScale = 1.1f;
        yield return new WaitForSeconds(0.1f);
        playerMove.Rigidbody.gravityScale = 1.2f;
        yield return new WaitForSeconds(0.2f);
        playerMove.Rigidbody.gravityScale = 2f;
    }
    
    // Debug
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (playerMove.IsSightRight == 1)
        {
            Gizmos.DrawWireCube(wallCheckTransform[0].position, new Vector2(0.1f, 0.8f));
        }
        else if(playerMove.IsSightRight == -1)
        {
            Gizmos.DrawWireCube(wallCheckTransform[1].position, new Vector2(0.1f, 0.8f));
        }
    }
}
