using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private PlayerData playerData;

    private LayerMask layerMask = LayerMask.GetMask("Ground");
    private Rigidbody2D rigidbody;
    private BoxCollider2D boxCollider2D;
    private Transform transform;
    
    private float speed;
    private float jumpForce;
    private float dashForce;
    
    private float horizontalMove;
    private float verticalMove;

    private float groundCheckDist = 0.9f;

    private Vector3 vector;
    
    //
    private Vector2 dashVector;
    
    private bool isWalking;
    private bool isJumping;
    
    public PlayerMove(PlayerData _playerData, Transform _transform)
    {
        this.playerData = _playerData;
        
        this.speed = playerData.getSpeed;
        this.jumpForce = playerData.getJumpForce;
        this.dashForce = playerData.getDashForce;

        this.transform = _transform;
    }
    
    
    
    public void UpdateMovement()
    {
        CheckGround();
        
        Move();
        Jump();
        Dash();

        Debug.DrawRay(transform.position, Vector3.down * groundCheckDist, Color.cyan);
    }

    private void Move()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        
        if (horizontalMove != 0)
        {
            isWalking = true;
            
            vector.Set(horizontalMove, 0.0f, 0.0f);
            transform.Translate(vector.x * speed * Time.deltaTime, 0,0);
        }
        else
        {
            isWalking = false;
        }
    }

    private void Dash() // 수정 중. 보류
    {
        dashVector = new Vector2(horizontalMove, verticalMove);

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            // AddForce로는 원하는 만큼 뿅하고 이동이 안됨 가다가 흐느적거림
            //rigidbody.AddForce(dashVector * dashForce, ForceMode2D.Impulse);
            //transform.
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!isJumping)
            {
                isJumping = true;
                rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            }
        }
    }

    private void CheckGround()
    {
        // 플레이어의 아래 방향을 계속 확인 -> 땅이 확인되면 점프 초기화
        var raycastHit2D = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDist, layerMask);
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
