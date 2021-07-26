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
    private readonly float jumpForce;
    private readonly float dashForce;
    private readonly float dashCoolTime; // const
    private float curDashTime; // 쿨타임에 사용될 변수
    private int dashCnt;
    

    private float horizontalMove;
    private float verticalMove;
    [HideInInspector] public float isSightRight;

    private const float GroundCheckDist = 0.9f;
    private const float WallCheckWidth = 0.1f;
    private const float WallCheckHeight = 0.8f;
    
    
    private bool isWalking;
    private bool isWall; // 벽타기 유무
    private bool isGround;
    private bool isWallJumping; // 벽타는 동안에 점프했는가의 유무
    private bool canBasicMove = true; // 동작이 가능한가의 여부
    [HideInInspector] public bool wallCoroutineStart = false; // 해당 변수가 true가 되면 코루틴을 실행
    [HideInInspector] public bool dashCoroutineStart = false;
    
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
        CheckWall();

        if (!isWallJumping && canBasicMove)
        {
            Move();
            Dash();
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
            if (isGround)
            {
                PlayerFoley.playerFoley.StartCoroutine("FootstepSound"); // 땅에서 이동할 때만 발소리 재생
            }
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
                rigidbody.velocity = new Vector2(-1f * isSightRight, 2.5f) * (jumpForce * 0.5f);
            }
        }
    }
    
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerFoley.playerFoley.PlayJump(); // 점프 소리 재생

            // AddForce나 velocity 중 하나 선택
            //rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            rigidbody.velocity = Vector2.up * jumpForce;
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
        if (isGround) dashCnt = 1;
        
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
}
