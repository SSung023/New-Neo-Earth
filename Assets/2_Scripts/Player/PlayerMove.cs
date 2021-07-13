using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private PlayerData playerData;
    private Blackboard blackboard;

    private LayerMask layerMask_ground;
    private LayerMask layerMask_wall;
    private Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider2D;
    private Transform transform;
    private Transform wallCheckTransform_r;
    private Transform wallCheckTransform_l;

    private float speed;
    private float slidingSpeed;
    private float jumpForce;
    private float dashForce;
    private float dashCoolTime; // const
    private float curDashTime; // 쿨타임에 사용될 변수
    
    private float horizontalMove;
    private float verticalMove;

    private const float groundCheckDist = 0.9f;
    private const float wallCheckDist = 0.1f;
    
    private Vector2 dashVector;
    
    private bool isWalking;
    private bool isJumping;
    private bool isWall; // 벽타기 유무
    [HideInInspector] public bool isWallJumping; // 벽타는 동안에 점프했는가의 유무
    private bool isSightRight;
    private bool isGround;

    [HideInInspector] public bool coroutineStart = false;
    
    public PlayerMove(PlayerData _playerData, Transform _transform, Transform[] _wallCheckTransform)
    {
        this.playerData = _playerData;
        
        this.speed = playerData.getSpeed;
        this.slidingSpeed = playerData.getSlidingSpeed;
        this.jumpForce = playerData.getJumpForce;
        this.dashForce = playerData.getDashForce;
        this.dashCoolTime = playerData.getDashCoolTime;
        curDashTime = dashCoolTime;

        this.transform = _transform;
        wallCheckTransform_r = _wallCheckTransform[0];
        wallCheckTransform_l = _wallCheckTransform[1];

        layerMask_ground = playerData.getLayermask_ground;
        layerMask_wall = playerData.getLayermask_wall;
    }
    
    
    
    public void UpdateMovement()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        isGround = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDist, layerMask_ground);
        isWall = Physics2D.Raycast(wallCheckTransform_r.position, Vector2.right, wallCheckDist, layerMask_wall);
        
        if (!isWallJumping)
            rigidbody.velocity = new Vector2(horizontalMove * speed, rigidbody.velocity.y);
            
        if (isGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                rigidbody.velocity = Vector2.up * jumpForce;
            }
        }
        
        if (isWall)
        {
            isWallJumping = false;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * slidingSpeed);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // isWallJumping = true;
                // coroutineStart = true;
                // rigidbody.velocity = new Vector2(-1 * (jumpForce * 0.5f), jumpForce * 0.9f);
                    
                if (!isWallJumping)
                {
                    isWallJumping = true;
                    coroutineStart = true;
                    //rigidbody.velocity = new Vector2(-1, 1) * (jumpForce * 0.5f);
                    rigidbody.AddForce(new Vector2(-1, 1) * jumpForce, ForceMode2D.Impulse);
                    Debug.Log("벽점프 실행");
                }
            }
        }
            
        Debug.DrawRay(wallCheckTransform_r.position, Vector3.right * wallCheckDist, Color.cyan);
    }

    private void Move()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        CheckSight(); // 어느 방향을 바라보고 있는지 체크

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

    private void WallMove()
    {
        if (isWall)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // 이 부분에 코루틴을 실행해서 일정 시간 후에 isWallJumping = false로 바꿔줘야 한다.
                if (!isWallJumping)
                {
                    isWallJumping = true;
                    coroutineStart = true;
                    rigidbody.velocity = new Vector2(-1, 1) * (jumpForce * 0.5f);
                    Debug.Log("벽점프 실행");
                }
            }
            else
            {
                isWallJumping = false;
                // 벽에 닿아있는 상태라면 느리게 벽에서 미끄러진다.
                Debug.Log("wallmove의 else문 실행");
                rigidbody.velocity = new Vector2(horizontalMove * speed, rigidbody.velocity.y * slidingSpeed);
            }
            
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

    private void Dash() // 미완성
    {
        dashVector = new Vector2(horizontalMove, verticalMove);
        //Debug.Log(dashVector);

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

    private void CheckWall()
    {
        if (isSightRight)
        {
            isWall = Physics2D.Raycast(wallCheckTransform_r.position, Vector2.right, wallCheckDist, layerMask_wall);
            Debug.DrawRay(wallCheckTransform_r.position, Vector3.right * wallCheckDist, Color.cyan);
        }
        else
        {
            isWall = Physics2D.Raycast(wallCheckTransform_l.position, Vector2.left, wallCheckDist, layerMask_wall);
            Debug.DrawRay(wallCheckTransform_l.position, Vector3.left * wallCheckDist, Color.cyan);
        }
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

    private void CheckSight()
    {
        // 어느 방향을 바라보고 있는지 체크하는 구문
        if (horizontalMove == 1)
        {
            isSightRight = true;
        }
        else if (horizontalMove == -1)
        {
            isSightRight = false;
        }
    }


    // Getters & Setters
    public Rigidbody2D Rigidbody
    {
        get => rigidbody;
        set => rigidbody = value;
    }
}
