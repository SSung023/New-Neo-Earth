using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private float speed;
    private float jumpForce;
    private float dashForce;
    private float horizontalMove;
    
    private Vector3 vector;
    
    private bool isWalking;
    
    public PlayerMove(float _speed, float _jumpForce, float _dashForce)
    {
        this.speed = _speed;
        this.jumpForce = _jumpForce;
        this.dashForce = _dashForce;
    }

    
    private void UpdateMovement()
    {
        
    }

    public void Move(Transform transform)
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        
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
}
