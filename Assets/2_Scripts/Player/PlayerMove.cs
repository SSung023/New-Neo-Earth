using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private PlayerData playerData;

    private readonly LayerMask layerMask_ground;
    private readonly LayerMask layerMask_wall;
    private Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider2D;
    private Transform transform;
    private readonly Transform wallCheckTransform_r;
    private readonly Transform wallCheckTransform_l;
    private Vector2 dashVector;

    private readonly float speed;
    private readonly float slidingSpeed;
    private readonly float jumpForce;
    private readonly float dashForce;
    private readonly float dashCoolTime; // const
    private float curDashTime; // 쿨타임에 사용될 변수
    
    private float horizontalMove;
    private float verticalMove;
    private float isSightRight;

    private const float groundCheckDist = 0.9f;
    private const float wallCheckDist = 0.1f;
    
    
    private bool isWalking;
    private bool isWall; // 벽타기 유무
    private bool isGround;
    [HideInInspector] public bool isWallJumping; // 벽타는 동안에 점프했는가의 유무
    [HideInInspector] public bool isSpaceOn = false;
    [HideInInspector] public bool coroutineStart = false; // 해당 변수가 true가 되면 코루틴을 실행
    
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
        UpdateValue();

        if (!isSpaceOn)
        {
            CheckWall();
        }

        if (!isWallJumping && !isSpaceOn)
        {
            Move();
        }

        if (isGround)
        {
            Jump();
        }

        if (isWall)
        {
            WallMove();
        }

    }

    private void Move()
    {
        if (horizontalMove != 0)
        {
            isWalking = true;
            rigidbody.velocity = new Vector2(horizontalMove * speed, rigidbody.velocity.y);
        }
        else
        {
            isWalking = false;
        }
    }

    private void WallMove()
    {
        isWallJumping = false;
        if (!isSpaceOn)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * slidingSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isSpaceOn)
        {
            if (!isWallJumping)
            {
                isWallJumping = true;
                coroutineStart = true;
                rigidbody.velocity = new Vector2(-1f * isSightRight, 1.75f) * (jumpForce * 0.5f);
            }
        }
    }
    
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // AddForce나 velocity 중 하나 선택
            //rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            rigidbody.velocity = Vector2.up * jumpForce;
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
    
    private void UpdateValue()
    {
        // horizontalMove, VerticalMove, isGround 값 업데이트
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        isGround = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDist, layerMask_ground);
        
        CheckSight();
    }

    private void CheckWall()
    {
        if (isSightRight == 1)
        {
            isWall = Physics2D.Raycast(wallCheckTransform_r.position, Vector2.right, wallCheckDist, layerMask_wall);
            Debug.DrawRay(wallCheckTransform_r.position, Vector3.right * wallCheckDist, Color.cyan);
        }
        else if(isSightRight == -1)
        {
            isWall = Physics2D.Raycast(wallCheckTransform_l.position, Vector2.left, wallCheckDist, layerMask_wall);
            Debug.DrawRay(wallCheckTransform_l.position, Vector3.left * wallCheckDist, Color.cyan);
        }
    }
    
    private void CheckSight()
    {
        // 어느 방향을 바라보고 있는지 체크하는 구문
        if (horizontalMove == 1)
        {
            isSightRight = 1;
        }
        else if (horizontalMove == -1)
        {
            isSightRight = -1;
        }
    }
    

    // Getters & Setters
    public Rigidbody2D Rigidbody
    {
        get => rigidbody;
        set => rigidbody = value;
    }
}
