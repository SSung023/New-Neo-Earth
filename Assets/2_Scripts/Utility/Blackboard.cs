using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour
{
    // 공통으로 사용되는 변수들을 저장하는 클래스
    
    // horizontalMove = Input.GetAxisRaw("Horizontal");
    // isGround = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDist, layerMask_ground);
    // isWall = Physics2D.Raycast(wallCheckTransform_r.position, Vector2.right, wallCheckDist, layerMask_wall);
    //
    // if (!isWallJumping)
    // rigidbody.velocity = new Vector2(horizontalMove * speed, rigidbody.velocity.y);
    //     
    //     if (isGround)
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         rigidbody.velocity = Vector2.up * jumpForce;
    //     }
    // }
    //
    // if (isWall)
    // {
    //     isWallJumping = false;
    //     rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.y * slidingSpeed);
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         // isWallJumping = true;
    //         // coroutineStart = true;
    //         // rigidbody.velocity = new Vector2(-1 * (jumpForce * 0.5f), jumpForce * 0.9f);
    //             
    //         if (!isWallJumping)
    //         {
    //             isWallJumping = true;
    //             coroutineStart = true;
    //             //rigidbody.velocity = new Vector2(-1, 1) * (jumpForce * 0.5f);
    //             rigidbody.AddForce(new Vector2(-1, 1) * jumpForce, ForceMode2D.Impulse);
    //             Debug.Log("벽점프 실행");
    //         }
    //     }
    // }
    //     
    // Debug.DrawRay(wallCheckTransform_r.position, Vector3.right * wallCheckDist, Color.cyan);
}
