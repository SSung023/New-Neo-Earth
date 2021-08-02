using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class MoveController : MonoBehaviour
{
    private PlayerData playerData;

    private PlayerParam playerParam; // 플레이어 공통 변수
    private BasicMove basicMove; // 기본 움직임 (+점프)
    private DashMove dashMove; // 대쉬
    private WallMove wallMove; // 벽 움직임

    private MoveType moveType;
    public MoveController(PlayerData _playerData, Rigidbody2D rigidbody, Transform transform, Transform[] wallCheck)
    {
        playerData = _playerData;
        
        playerParam = new PlayerParam(playerData, rigidbody, transform, wallCheck);
        basicMove = new BasicMove(playerData, playerParam);
        
        dashMove = new DashMove(playerData, playerParam);
        wallMove = new WallMove(playerData, playerParam);
    }



    public void UpdateMovement()
    {
        playerParam.UpdateParam();
        

        basicMove.MovePlayer();
        basicMove.Jump();
        
        CheckJumping();
        
        Debug.Log("isjumping: " + basicMove.IsJumping);
        
        // dashMove.UpdateDashVector();
        //
        // if (wallMove.IsWallJumping && playerParam.CanBasicMove)
        // {
        //     basicMove.MovePlayer();
        //     basicMove.Jump();
        //     dashMove.Dash();
        // }
        //
        // if (playerParam.IsWall)
        // {
        //     wallMove.WallMoving();
        // }

        // if (isLanded)
        // {
        //     Land();
        // }
    }

    public int passCoroutineParam()
    {
        if (basicMove.JumpCoroutineStart)
        {
            return 0;
        }
        if (basicMove.LandCoroutineStart)
        {
            return 1;
        }
        if (wallMove.WallCoroutineStart)
        {
            return 2;
        }
        if (dashMove.DashCoroutineStart)
        {
            return 3;
        }
        else
        {
            return 4;
        }
    }

    private void CheckJumping()
    {
        if (playerParam.CanChangeJumpValue && basicMove.IsJumping && playerParam.IsGround)
        {
            basicMove.IsJumping = false;
            basicMove.IsLanded = true;
            basicMove.LandCoroutineStart = true;
        }

        if (basicMove.IsJumping && playerParam.IsWall)
        {
            basicMove.IsJumping = false;
        }
    }
    
    // GETTERS && SETTERS

}
