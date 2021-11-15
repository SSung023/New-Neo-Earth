using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using SoundManager;

public class PlayerMove : MonoBehaviour
{
    private PlayerData playerData;
    //private DashMove dashMove;

    private readonly LayerMask layerMask_ground;
    private readonly LayerMask layerMask_wall;
    private readonly LayerMask layerMask_normalWall;
    private Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider2D;
    private Transform transform;
    private readonly Transform wallCheckTransform_r;
    private readonly Transform wallCheckTransform_l;
    private Vector2 dashVector;

    
    // BASIC MOVE
    private readonly float maxMoveSpeed; // 최대 속도
    private readonly float moveAcceleration;// 가속도
    private readonly float groundLinearDrag; // 저항 값
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
    private bool isGround;
    private bool isJumpOn; // jump 컨트롤을 위한 변수
    private bool isJumping; // 점프 중인지의 여부
    private bool canChangeJumpValue; //
    private bool jumpCoroutineStart = false; // 해당 변수가 true가 되면 코루틴을 실행

    // LAND
    private bool isLanded; // 
    private bool landCoroutineStart = false; // 해당 변수가 true가 되면 코루틴을 실행


    // WALL MOVE
    private readonly float wallJumpForce;
    private readonly float slidingSpeed;
    private bool isWall; // 벽 타기 가능한 벽 유무
    private bool isNormalWall; // 벽 타기 불가능한 벽 유무
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
        this.groundLinearDrag = playerData.GetGroundLinearDrag;
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
        layerMask_normalWall = playerData.getLayerMask_normalWall;
    }
    
    
    
    public void UpdateMovement()
    {
        Debug.Log(isNormalWall);
        UpdateValue();
        CheckWall();
        CheckNormalWall();

        if (!isWallJumping && canBasicMove)
        {
            MovePlayer();
            Dash();
            Jump();
        }
        
        if (isWall || isNormalWall)
        {
            WallMove();
        }

        if (isLanded)
        {
            Land();
        }
        
    }

    private void MovePlayer()
    {
        if (horizontalMove != 0)
        {
            isWalking = true;
            if (isGround) // 땅에 닿아 있을 때
            {
                PlayerFoley._foley.StartCoroutine("FootstepSound"); // 땅에서 이동할 때만 발소리 재생
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
            rigidbody.drag = groundLinearDrag;
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

        if (Input.GetKeyDown(KeyCode.Z) && canBasicMove && isWall)
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
        if (isGround && Input.GetKeyDown(KeyCode.Z))
        {
            PlayerFoley._foley.PlayJump(); // 점프 소리 재생
            
            isJumpOn = true;
            isJumping = true; // 체공 상태
            jumpTimeCounter = maxJumpTime;
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
            jumpCoroutineStart = true;
        }

        if (Input.GetKey(KeyCode.Z) && isJumpOn)
        {
            if (jumpTimeCounter > 0)
            {
                rigidbody.velocity = new Vector2(rigidbody.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else // 점프키를 누를 수 있는 시간이 다 됐으므로
            {
                isJumpOn = false;
            }
        }
        
        if (Input.GetKeyUp(KeyCode.Z))
        {
            isJumpOn = false;
        }
        
        AdjustJumpGravity();
    }
    private void AdjustJumpGravity()
    {
        if (rigidbody.velocity.y < 0 && isJumping)
        {
            //rigidbody.velocity += Vector2.up * (Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime);
            rigidbody.gravityScale = fallMultiplier;
        }
        else if (rigidbody.velocity.y > 0 && isJumping)
        {
            //rigidbody.velocity += Vector2.up * (Physics2D.gravity.y * (riseMultiplier - 1) * Time.deltaTime);
            rigidbody.gravityScale = riseMultiplier;
        }
        else
        {
            rigidbody.gravityScale = 2;
        }
    }

    private void Land()
    {
        // land 시에 소리 재생
        Debug.Log("Land 실행");
    }

    private void Dash()
    {
        // 땅에 닿아있을 때 or 아직 대쉬 기회가 남아있을 때
        if (isGround || (!isGround && dashCnt == 1))
        {
            if (Input.GetKeyDown(KeyCode.C))
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

        CheckGround();
        
        if (isGround)
        {
            dashCnt = 1;
        }

        CheckJumping();

        SetDashVector();
        CheckSight();
    }

    private void CheckGround()
    {
        //isGround = Physics2D.Raycast(transform.position, Vector2.down, GroundCheckDist, layerMask_ground);
        
        bool isGround_left = Physics2D.Raycast(new Vector2(transform.position.x - 0.1f, transform.position.y), 
            Vector2.down, GroundCheckDist, layerMask_ground);
        bool isGround_right = Physics2D.Raycast(new Vector2(transform.position.x + 0.1f, transform.position.y), 
            Vector2.down, GroundCheckDist, layerMask_ground);

        isGround = isGround_left || isGround_right;
        
        //Debug.DrawRay(transform.position, Vector3.down * GroundCheckDist, Color.cyan);
        Debug.DrawRay(new Vector2(transform.position.x + 0.17f, transform.position.y), Vector3.down * GroundCheckDist, Color.cyan);
        Debug.DrawRay(new Vector2(transform.position.x - 0.17f, transform.position.y), Vector3.down * GroundCheckDist, Color.cyan);
    }

    private void CheckJumping()
    {
        if (canChangeJumpValue && isJumping && isGround)
        {
            // 땅에 닿았을 때
            isJumping = false;
            isLanded = true;
            landCoroutineStart = true;
        }

        if (isJumping && isWall)
        {
            // 벽에 붙었을 때
            isJumping = false;
        }

    }
    
    private void SetDashVector()
    {
        dashVector = new Vector2(horizontalMove, verticalMove);

        if (horizontalMove == 0 && verticalMove == 0)
        {
            dashVector = new Vector2(isSightRight, verticalMove);
        }
    }

    private void CheckWall() // 벽 점프하는 벽 탐지
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

    private void CheckNormalWall() // 벽 점프 불가능한 벽 탐지
    {
        if (isSightRight == 1)
        {
            isNormalWall = Physics2D.OverlapBox(wallCheckTransform_r.position, new Vector2(WallCheckWidth, WallCheckHeight), 0, layerMask_normalWall);
        }
        else if(isSightRight == -1)
        {
            isNormalWall = Physics2D.OverlapBox(wallCheckTransform_l.position, new Vector2(WallCheckWidth, WallCheckHeight), 0, layerMask_normalWall);
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
    public bool JumpCoroutineStart
    {
        get => jumpCoroutineStart;
        set => jumpCoroutineStart = value;
    }
    public bool LandCoroutineStart
    {
        get => landCoroutineStart;
        set => landCoroutineStart = value;
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

    public bool IsJumping
    {
        get => isJumping;
        set => isJumping = value;
    }
    public bool IsWallJumping
    {
        get => isWallJumping;
        set => isWallJumping = value;
    }
    public bool IsLanded
    {
        get => isLanded;
        set => isLanded = value;
    }
    public bool CanChangeJumpValue
    {
        get => canChangeJumpValue;
        set => canChangeJumpValue = value;
    }

    public int DashCnt
    {
        get => dashCnt;
        set => dashCnt = value;
    }
}
