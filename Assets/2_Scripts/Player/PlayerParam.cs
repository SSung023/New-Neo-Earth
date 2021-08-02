using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerParam : MonoBehaviour
{
    private readonly LayerMask layerMask_ground;
    private readonly LayerMask layerMask_wall;

    private readonly Transform wallCheckTransform_r;
    private readonly Transform wallCheckTransform_l;
    private Transform transform;
    private Rigidbody2D rigidbody;
    
    private float horizontalMove;
    private float verticalMove;
    private float isSightRight;
    
    private bool isGround;
    private bool isWall;
    private bool canBasicMove;
    private bool canChangeJumpValue;
    

    // CONST VALUE
    private const float GroundCheckDist = 0.9f;
    private const float WallCheckWidth = 0.1f;
    private const float WallCheckHeight = 0.8f;

    public PlayerParam(PlayerData playerData, Rigidbody2D _rigidbody, Transform _transform, Transform[] _wallCheckTransform)
    {
        layerMask_ground = playerData.getLayermask_ground;
        layerMask_wall = playerData.getLayermask_wall;

        rigidbody = _rigidbody;
        transform = _transform;
        
        wallCheckTransform_r = _wallCheckTransform[0];
        wallCheckTransform_l = _wallCheckTransform[1];
    }
    

    
    public void UpdateParam()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");
        
        isGround = Physics2D.Raycast(transform.position, Vector2.down, GroundCheckDist, layerMask_ground);

        CheckSight();
        CheckWall();
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



    // GETTERS && SETTERS
    public LayerMask LayerMaskGround => layerMask_ground;
    public LayerMask LayerMaskWall => layerMask_wall;
    public Rigidbody2D Rigidbody
    {
        get => rigidbody;
        set => rigidbody = value;
    }
    public Transform Transform
    {
        get => transform;
        set => transform = value;
    }
    public float HorizontalMove
    {
        get => horizontalMove;
        set => horizontalMove = value;
    }
    public float VerticalMove
    {
        get => verticalMove;
        set => verticalMove = value;
    }
    public float IsSightRight
    {
        get => isSightRight;
        set => isSightRight = value;
    }
    public bool CanBasicMove
    {
        get => canBasicMove;
        set => canBasicMove = value;
    }
    public bool CanChangeJumpValue
    {
        get => canChangeJumpValue;
        set => canChangeJumpValue = value;
    }
    public bool IsGround
    {
        get => isGround;
        set => isGround = value;
    }
    public bool IsWall
    {
        get => isWall;
        set => isWall = value;
    }
   
}
