using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    private readonly float floatingSpeed;
    private readonly float jumpForce;
    private readonly float wallJumpForce;
    private readonly float dashForce;
    private float jumpTimeCounter;
    private readonly float maxJumpTime;
    private int dashCnt;
    

    private float horizontalMove;
    private float verticalMove;
    private float isSightRight;

    private const float GroundCheckDist = 0.9f;
    private const float WallCheckWidth = 0.1f;
    private const float WallCheckHeight = 0.8f;
    
    
    private bool isWalking;
    private bool isWall; // 벽타기 유무
    private bool isGround;
    private bool isJumping;
    private bool isWallJumping; // 벽타는 동안에 점프했는가의 유무
    private bool isParkourDoing; // 특수 동작? 대쉬, 벽점프 중인가의 여부
    private bool canBasicMove = true; // 동작이 가능한가의 여부
    private bool wallCoroutineStart = false; // 해당 변수가 true가 되면 코루틴을 실행
    private bool dashCoroutineStart = false;
    
    public PlayerMove(PlayerData _playerData, Transform _transform, Transform[] _wallCheckTransform)
    {
        this.playerData = _playerData;
        
        this.speed = playerData.getSpeed;
        this.slidingSpeed = playerData.getSlidingSpeed;
        this.floatingSpeed = playerData.getFloatingSpeed;
        this.jumpForce = playerData.getJumpForce;
        this.wallJumpForce = playerData.getWallJumpForce;
        this.maxJumpTime = playerData.getMaxJumpTime;
        this.dashForce = playerData.getDashForce;

        this.transform = _transform;
        wallCheckTransform_r = _wallCheckTransform[0];
        wallCheckTransform_l = _wallCheckTransform[1];

        layerMask_ground = playerData.getLayermask_ground;
        layerMask_wall = playerData.getLayermask_wall;
    }
    
    
    
    public void UpdateMovement()
    {
        UpdateValue();
        CheckWall();

        if (!isWallJumping && canBasicMove)
        {
            Move();
            Dash();
        }
        
        Jump();
        
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
            if (isGround)
            {
                // 땅에 닿아 있을 때
                PlayerFoley.playerFoley.StartCoroutine("FootstepSound"); // 땅에서 이동할 때만 발소리 재생
                // rigidbody.velocity = new Vector2(horizontalMove * speed, rigidbody.velocity.y);
            }
            rigidbody.velocity = new Vector2(horizontalMove * speed, rigidbody.velocity.y);
            // else
            // {
            //     rigidbody.velocity = new Vector2(horizontalMove * floatingSpeed, rigidbody.velocity.y);
            // }
            // else if (isParkourDoing)
            // {
            //     // dash, wall jump 후 체공 시
            //     rigidbody.velocity = new Vector2(horizontalMove * floatingSpeed, rigidbody.velocity.y);
            // }
            
        }
        else
        {
            isWalking = false;
        }
    }

    private void WallMove()
    {
        isWallJumping = false;
        if (canBasicMove)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * slidingSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space) && canBasicMove)
        {
            if (!isWallJumping)
            {
                isWallJumping = true;
                wallCoroutineStart = true;
                rigidbody.velocity = new Vector2(-1f * isSightRight, 2.5f) * wallJumpForce;
            }
        }
    }
    
    private void Jump()
    {
        if (isGround && Input.GetKeyDown(KeyCode.Space))
        {
            PlayerFoley.playerFoley.PlayJump(); // 점프 소리 재생
            
            isJumping = true;
            jumpTimeCounter = 0;
            rigidbody.velocity = Vector2.up * jumpForce;
            //rigidbody.AddForce(Vector2.up * (jumpForce), ForceMode2D.Impulse);
        }
        
        // 이 부분에서 움직임 어색
        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter <= maxJumpTime)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
                //rigidbody.AddForce(Vector2.up * (jumpForce), ForceMode2D.Force);
                jumpTimeCounter += Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }

    private void Dash()
    {
        // 땅에 닿아있을 때 or 아직 대쉬 기회가 남아있을 때
        if (isGround || (!isGround && dashCnt == 1))
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                rigidbody.velocity = Vector2.zero;
                rigidbody.velocity = dashVector * dashForce;
                dashCoroutineStart = true;
                dashCnt--;
            }
        }
    }

    private void UpdateValue()
    {
        // horizontalMove, VerticalMove, isGround 값 업데이트
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        isGround = Physics2D.Raycast(transform.position, Vector2.down, GroundCheckDist, layerMask_ground);
        if (isGround)
        {
            dashCnt = 1;
            isParkourDoing = false; // 특수 동작 체공 상태 종료
        }
        
        SetDashVector();
        CheckSight();
    }
    private void SetDashVector()
    {
        dashVector = new Vector2(horizontalMove, verticalMove);

        if (horizontalMove == 0 && verticalMove == 0)
        {
            dashVector = new Vector2(isSightRight, verticalMove);
        }
    }

    private void CheckWall()
    {
        if (isSightRight == 1)
        {
            isWall = Physics2D.OverlapBox(wallCheckTransform_r.position, new Vector2(WallCheckWidth, WallCheckHeight), 0, layerMask_wall);
        }
        else if(isSightRight == -1)
        {
            isWall = Physics2D.OverlapBox(wallCheckTransform_l.position, new Vector2(WallCheckWidth, WallCheckHeight), 0, layerMask_wall);
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
    public Vector2 DashVector
    {
        get => dashVector;
        set => dashVector = value;
    }
    public float IsSightRight
    {
        get => isSightRight;
        set => isSightRight = value;
    }
    public bool DashCoroutineStart
    {
        get => dashCoroutineStart;
        set => dashCoroutineStart = value;
    }
    public bool WallCoroutineStart
    {
        get => wallCoroutineStart;
        set => wallCoroutineStart = value;
    }
    public bool CanBasicMove
    {
        get => canBasicMove;
        set => canBasicMove = value;
    }
    public bool IsWallJumping
    {
        get => isWallJumping;
        set => isWallJumping = value;
    }
    public bool IsParkourDoing
    {
        get => isParkourDoing;
        set => isParkourDoing = value;
    }
}
