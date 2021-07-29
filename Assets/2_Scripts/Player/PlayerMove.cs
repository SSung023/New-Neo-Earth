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

    
    // BASIC MOVE
    private readonly float maxMoveSpeed; // 최대 속도
    private readonly float moveAcceleration;// 가속도
    private readonly float linearDrag; // 저항 값
    private float horizontalMove;
    private float verticalMove;
    private float isSightRight;
    private bool isWalking;
    private bool canBasicMove = true; // 동작이 가능한가의 여부
    private bool changingDirection => (rigidbody.velocity.x > 0 && horizontalMove < 0) || (rigidbody.velocity.x < 0 && horizontalMove >0);
    
    // JUMP
    private readonly float jumpForce; // 점프 속도
    private readonly float maxJumpTime; // 점프키를 누를 수 있는 최대 시간
    private readonly float fallMultiplier; // 내려 갈 때의 중력
    private readonly float riseMultiplier; // 올라 갈 때의 중력
    private float jumpTimeCounter;
    private int landCount = 0;
    private bool isGround;
    private bool isJumping;
    private bool isLanded;
    
    // WALL MOVE
    private readonly float wallJumpForce;
    private readonly float slidingSpeed;
    private bool isWall; // 벽타기 유무
    private bool isWallJumping; // 벽타는 동안에 점프했는가의 유무
    private bool wallCoroutineStart = false; // 해당 변수가 true가 되면 코루틴을 실행
    
    // DASH
    private readonly float dashForce;
    private int dashCnt;
    private bool dashCoroutineStart = false; // 해당 변수가 true가 되면 코루틴을 실행
    
    // CONST VALUE
    private const float GroundCheckDist = 0.9f;
    private const float WallCheckWidth = 0.1f;
    private const float WallCheckHeight = 0.8f;
    
    //
    private bool isParkourDoing; // 특수 동작? 대쉬, 벽점프 중인가의 여부
    
    public PlayerMove(PlayerData _playerData, Transform _transform, Transform[] _wallCheckTransform)
    {
        this.playerData = _playerData;
        
        this.maxMoveSpeed = playerData.GetMaxMoveSpeed;
        this.moveAcceleration = playerData.GetMoveAcceleration;
        this.linearDrag = playerData.GetGroundLinearDrag;
        this.slidingSpeed = playerData.getSlidingSpeed;
        
        this.jumpForce = playerData.getJumpForce;
        this.fallMultiplier = playerData.getFallMultiplier;
        this.riseMultiplier = playerData.getRiseMultiplier;
        
        this.dashForce = playerData.getDashForce;
        this.wallJumpForce = playerData.getWallJumpForce;
        
        this.maxJumpTime = playerData.getMaxJumpTime;


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
            MovePlayer();
            Dash();
            Jump();
        }
        
        if (isWall)
        {
            WallMove();
        }
        Land();
    }

    private void MovePlayer()
    {
        if (horizontalMove != 0)
        {
            isWalking = true;
            if (isGround) // 땅에 닿아 있을 때
            {
                PlayerFoley.playerFoley.StartCoroutine("FootstepSound"); // 땅에서 이동할 때만 발소리 재생
                rigidbody.AddForce(new Vector2(horizontalMove * moveAcceleration, 0f));
                ApplyGroundDrag();
            }
            else // 공중에 떠 있을 때
            {
                rigidbody.AddForce(new Vector2(horizontalMove * moveAcceleration, 0f));
            }
            
            // 지정한 속도보다 더 빠를 때
            if (Mathf.Abs(rigidbody.velocity.x) > maxMoveSpeed)
            {
                rigidbody.velocity = new Vector2(Mathf.Sign(rigidbody.velocity.x) * maxMoveSpeed, rigidbody.velocity.y);
            }
        }
        else
        {
            isWalking = false;
        }
    }

    private void ApplyGroundDrag()
    {
        if (Mathf.Abs(horizontalMove) < 0.4f || changingDirection)
        {
            rigidbody.drag = linearDrag;
        }
        else
        {
            rigidbody.drag = 0f;
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
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpTimeCounter <= maxJumpTime)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
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
        AdjustJumpGravity();
    }
    private void AdjustJumpGravity()
    {
        if (rigidbody.velocity.y < 0)
        {
            rigidbody.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 2) * Time.deltaTime);
        }
        else if (rigidbody.velocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rigidbody.velocity += Vector2.up * (Physics2D.gravity.y * (riseMultiplier - 2) * Time.deltaTime);
        }
    }

    private void Land()
    {
        // land 시에 소리 재생, velocity.x = max_speed로 고정하기
        // 점프하다가 처음으로 땅에 닿는 순간 isLanded = true로 하고 바로 false로 바꾸기
        
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
