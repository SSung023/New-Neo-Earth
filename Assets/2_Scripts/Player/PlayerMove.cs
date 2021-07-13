using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private PlayerData playerData;

    private LayerMask layerMask_ground = LayerMask.GetMask("Ground");
    private LayerMask layerMask_wall = LayerMask.GetMask("Wall");
    private Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider2D;
    private Transform transform;
    
    private float speed;
    private float slidingSpeed;
    private float jumpForce;
    private float dashForce;
    private float dashCoolTime; // const
    private float curDashTime; // 쿨타임에 사용될 변수
    
    private float horizontalMove;
    private float verticalMove;

    private const float groundCheckDist = 0.9f;
    private const float wallCheckDist = 0.5f;

    //private Vector3 vector;
    private Vector2 dashVector;
    
    private bool isWalking;
    private bool isJumping;
    
    public PlayerMove(PlayerData _playerData, Transform _transform)
    {
        this.playerData = _playerData;
        
        this.speed = playerData.getSpeed;
        this.slidingSpeed = playerData.getSlidingSpeed;
        this.jumpForce = playerData.getJumpForce;
        this.dashForce = playerData.getDashForce;
        this.dashCoolTime = playerData.getDashCoolTime;
        curDashTime = dashCoolTime;

        this.transform = _transform;
    }
    
    
    
    public void UpdateMovement()
    {
        CheckGround();
        
        Move();
        Jump();
        Dash();

        Debug.DrawRay(transform.position, Vector3.down * groundCheckDist, Color.cyan);
        Debug.DrawRay(transform.position, Vector3.right * wallCheckDist * horizontalMove, Color.cyan);
    }

    private void Move()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        
        if (horizontalMove != 0)
        {
            isWalking = true;
            
            //vector.Set(horizontalMove, 0.0f, 0.0f);
            //transform.Translate(vector.x * speed * Time.deltaTime, 0,0);
            rigidbody.velocity = new Vector2(horizontalMove * speed, rigidbody.velocity.y);
        }
        else
        {
            isWalking = false;
        }
    }

    private void Dash() // 미완성
    {
        dashVector = new Vector2(horizontalMove, verticalMove);
        Debug.Log(dashVector);

        if (curDashTime <= 0)
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                Debug.Log("dash");
                // AddForce나 velocity 중 하나 선택
                //rigidbody.AddForce(dashVector * dashForce, ForceMode2D.Impulse);
                rigidbody.velocity = dashVector * dashForce;
                
                curDashTime = dashCoolTime;
            }
        }
        else
        {
            curDashTime -= Time.deltaTime;
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isJumping)
            {
                isJumping = true;
                
                // AddForce나 velocity 중 하나 선택
                //rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                rigidbody.velocity = Vector2.up * jumpForce;
            }
        }
    }

    private void CheckWall()
    {
        var raycastHit2D = Physics2D.Raycast(transform.position, Vector2.right, wallCheckDist, layerMask_wall);
    }

    private void CheckGround()
    {
        // 플레이어의 아래 방향을 계속 확인 -> 땅이 확인되면 점프 초기화
        var raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDist, layerMask_ground);
        if (raycastHit2D.collider != null)
        {
            isJumping = false;
        }
        else
        {
            isJumping = true;
        }
    }
    
    
    // Getters & Setters
    public Rigidbody2D Rigidbody
    {
        get => rigidbody;
        set => rigidbody = value;
    }
}
