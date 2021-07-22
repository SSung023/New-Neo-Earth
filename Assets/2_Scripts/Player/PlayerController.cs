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
            
            if (playerMove.wallCoroutineStart)
            {
                StartCoroutine(changeValue());
            }
            if (playerMove.dashCoroutineStart)
            {
                StartCoroutine(controlRigid());
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

    IEnumerator changeValue()
    {
        playerMove.wallCoroutineStart = false;
        playerMove.isSpaceOn = true;
        yield return new WaitForSeconds(0.3f);
        playerMove.isWallJumping = false;
        playerMove.isSpaceOn = false;
    }

    IEnumerator controlRigid()
    {
        playerMove.dashCoroutineStart = false;
        playerMove.Rigidbody.gravityScale = 0f;
        playerMove.isSpaceOn = true;
        
        yield return new WaitForSeconds(playerData.getDashDuration);
        
        // 여기서 velocity 변경
        playerMove.Rigidbody.velocity = new Vector2(playerMove.dashVector.x * 5f, playerMove.dashVector.y);
        // yield return new WaitForSeconds(playerData.getDashDuration);
        
        StartCoroutine(controlGravity());
        
        // playerMove.isWallJumping = false;
        // playerMove.isSpaceOn = false;
    }

    IEnumerator controlGravity()
    {
        // playerMove.Rigidbody.gravityScale = 0.8f;
        for (float i = 1.7f; i < 2f; i++)
        {
            playerMove.Rigidbody.gravityScale = i;
            yield return new WaitForSeconds(0.07f);
        }
        
        yield return new WaitForSeconds(0.75f);

        playerMove.isWallJumping = false;
        playerMove.isSpaceOn = false;
    }
    
    // Debug
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        if (playerMove.isSightRight == 1)
        {
            Gizmos.DrawWireCube(wallCheckTransform[0].position, new Vector2(0.1f, 0.8f));
        }
        else if(playerMove.isSightRight == -1)
        {
            Gizmos.DrawWireCube(wallCheckTransform[1].position, new Vector2(0.1f, 0.8f));
        }
    }
}
