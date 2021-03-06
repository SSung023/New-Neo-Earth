using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

enum MoveType
{
    JUMP = 0, LAND = 1, WALL = 2, DASH = 3
}
public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    
    //private MoveController moveController;
    private MoveType moveType;
    
    private PlayerMove playerMove;

    private Rigidbody2D rigidbody;
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
        
        rigidbody = GetComponent<Rigidbody2D>();
        //moveController = new MoveController(playerData, rigidbody, transform, wallCheckTransform);
        
        
        playerMove = new PlayerMove(playerData, transform, wallCheckTransform);
        playerMove.Rigidbody = GetComponent<Rigidbody2D>();
        GameManager.player = this.gameObject;
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
            // moveController.UpdateMovement();
            // ControlCoroutine(moveController.passCoroutineParam());
            
            playerMove.UpdateMovement();
            
            if (playerMove.WallCoroutineStart)
            {
                StartCoroutine(wallJump_changeValue());
            }
            if (playerMove.DashCoroutineStart)
            {
                StartCoroutine(dash_controlRigid());
            }
            if (playerMove.JumpCoroutineStart)
            {
                StartCoroutine(Jump_Coroutine());
            }
            if (playerMove.LandCoroutineStart)
            {
                StartCoroutine(Land_Coroutine());
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
        /*curScene = SceneManager.GetActiveScene().buildIndex;*/
        //curSpawnPoint = GetComponent<LevelChanger>().spawnPointObj.position;
        
        if (Input.GetKeyDown(KeyCode.R))
        {
            //Debug.Log("Press R Key");
            // 임시 코드
            isAlive = true;
            var spriteRenderer = GetComponent<SpriteRenderer>();
            var _color = spriteRenderer.color;
            _color.a = 1f;
            spriteRenderer.color = _color;
            
            //transform.Translate(transform.position - curSpawnPoint);
            /*SceneManager.LoadScene(curScene);*/
        }
    }


    private void ControlCoroutine(int param)
    {
        switch (param)
        {
            case 0:
                StartCoroutine(Jump_Coroutine());
                break;
            case 1:
                StartCoroutine(Land_Coroutine());
                break;
            case 2:
                StartCoroutine(wallJump_changeValue());
                break;
            case 3:
                StartCoroutine(dash_controlRigid());
                break;
        }
    }
    IEnumerator Land_Coroutine()
    {
        playerMove.LandCoroutineStart = false;
        playerMove.IsLanded = true;
        yield return new WaitForSeconds(0.01f);
        playerMove.IsLanded = false;
    }
    IEnumerator Jump_Coroutine()
    {
        
        playerMove.JumpCoroutineStart = false;
        playerMove.CanChangeJumpValue = false;
        yield return new WaitForSeconds(0.8f);
        playerMove.CanChangeJumpValue = true;
    }

    IEnumerator wallJump_changeValue()
    {
        // 벽에서 점프했을 때 실행되는 코루틴
        // 일정 시간동안 플레이어의 조작을 막아서 벽 점프가 정상적으로 실행되도록 한다
        playerMove.WallCoroutineStart = false;
        playerMove.CanBasicMove = false;

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
        playerMove.IsJumping = false;

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
    
    // GETTERS && SETTERS
    public PlayerMove PlayerMove
    {
        get => playerMove;
        set => playerMove = value;
    }
}
