using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMove : MonoBehaviour
{
    private PlayerData playerData;
    private readonly PlayerParam playerParam;

    // BASIC MOVE
    private readonly float maxMoveSpeed;
    private readonly float moveAcceleration;
    private readonly float groundLinearDrag;
    private bool isWalking;
    
    // JUMP
    private readonly float jumpForce;
    private readonly float maxJumpTime;
    private readonly float fallMultiplier;
    private readonly float riseMultiplier;
    private float jumpTimeCounter;
    private bool isJumpOn; // jump 컨트롤을 위한 변수
    private bool isJumping; // 점프 중인지의 여부
    private bool jumpCoroutineStart; // 해당 변수가 true가 되면 코루틴을 실행
    
    // LAND
    private bool landCoroutineStart;
    private bool isLanded;
    
    private bool changingDirection 
        => (playerParam.Rigidbody.velocity.x > 0 && playerParam.HorizontalMove < 0) || (playerParam.Rigidbody.velocity.x < 0 && playerParam.HorizontalMove >0);

    public BasicMove(PlayerData _playerData, PlayerParam _playerParam)
    {
        playerData = _playerData;
        playerParam = _playerParam;

        maxMoveSpeed = playerData.GetMaxMoveSpeed;
        moveAcceleration = playerData.GetMoveAcceleration;
        groundLinearDrag = playerData.GetGroundLinearDrag;

        jumpForce = playerData.getJumpForce;
        maxJumpTime = playerData.getMaxJumpTime;
        fallMultiplier = playerData.getFallMultiplier;
        riseMultiplier = playerData.getRiseMultiplier;
    }

    
    public void MovePlayer()
    {
        if (playerParam.HorizontalMove != 0)
        {
            isWalking = true;
            if (playerParam.IsGround)
            {
                PlayerFoley.playerFoley.StartCoroutine("FootstepSound"); // 땅에서 이동할 때만 발소리 재생
                playerParam.Rigidbody.AddForce(new Vector2(playerParam.HorizontalMove * moveAcceleration, 0f));
                ApplyGroundDrag();
            }
            else
            {
                playerParam.Rigidbody.AddForce(new Vector2(playerParam.HorizontalMove * moveAcceleration, 0f));
            }

            if (Math.Abs(playerParam.Rigidbody.velocity.x) > maxMoveSpeed)
            {
                playerParam.Rigidbody.velocity =
                    new Vector2(Mathf.Sign(playerParam.Rigidbody.velocity.x) * maxMoveSpeed, playerParam.Rigidbody.velocity.y);
            }
        }
        else
        {
            isWalking = false;
        }
    }
    private void ApplyGroundDrag()
    {
        if (Mathf.Abs(playerParam.HorizontalMove) < 0.4f || changingDirection)
        {
            playerParam.Rigidbody.drag = groundLinearDrag;
        }
        else
        {
            playerParam.Rigidbody.drag = 0f;
        }
    }

    public void Jump()
    {
        if (playerParam.IsGround && Input.GetKeyDown(KeyCode.Space))
        {
            PlayerFoley.playerFoley.PlayJump(); // 점프 소리 재생

            isJumpOn = true;
            isJumping = true;
            jumpTimeCounter = maxJumpTime;
            playerParam.Rigidbody.velocity = new Vector2(playerParam.Rigidbody.velocity.x, jumpForce);
            jumpCoroutineStart = true;
        }

        if (Input.GetKey(KeyCode.Space) && isJumpOn)
        {
            if (jumpTimeCounter > 0)
            {
                playerParam.Rigidbody.velocity = new Vector2(playerParam.Rigidbody.velocity.x, jumpForce);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumpOn = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumpOn = false;
        }
        AdjustJumpGravity();
    }

    private void AdjustJumpGravity()
    {
        if (playerParam.Rigidbody.velocity.y < 0 && isJumping)
        {
            playerParam.Rigidbody.gravityScale = fallMultiplier;
        }
        else if (playerParam.Rigidbody.velocity.y > 0 && Input.GetKey(KeyCode.Space) && isJumping)
        {
            playerParam.Rigidbody.gravityScale = riseMultiplier;
        }
        else
        {
            playerParam.Rigidbody.gravityScale = 2f;
        }
    }

    public void Land()
    {
        
    }
    
    
    
    // GETTERS && SETTERS
    public bool JumpCoroutineStart
    {
        get => jumpCoroutineStart;
        set => jumpCoroutineStart = value;
    }
    public bool IsJumping
    {
        get => isJumping;
        set => isJumping = value;
    }

    public bool LandCoroutineStart
    {
        get => landCoroutineStart;
        set => landCoroutineStart = value;
    }

    public bool IsLanded
    {
        get => isLanded;
        set => isLanded = value;
    }
}
