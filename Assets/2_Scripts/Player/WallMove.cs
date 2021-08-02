using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallMove : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerParam playerParam;

    private readonly float slidingSpeed;
    private readonly float wallJumpForce;
    
    private bool isWallJumping;
    private bool wallCoroutineStart;

    public WallMove(PlayerData _playerData, PlayerParam _playerParam)
    {
        this.playerData = _playerData;
        this.slidingSpeed = playerData.getSlidingSpeed;
        this.wallJumpForce = playerData.getWallJumpForce;
    }
    

    public void WallMoving()
    {
        isWallJumping = false;
        if (playerParam.CanBasicMove)
        {
            playerParam.Rigidbody.velocity = new Vector2(playerParam.Rigidbody.velocity.x, playerParam.Rigidbody.velocity.y * slidingSpeed);
        }

        if (Input.GetKeyDown(KeyCode.Space) && playerParam.CanBasicMove)
        {
            if (!isWallJumping)
            {
                isWallJumping = true;
                wallCoroutineStart = true;
                playerParam.Rigidbody.velocity = new Vector2(-1f * playerParam.IsSightRight, 2.5f) * wallJumpForce;
            }
        }
    }
    
    
    // GETTERS && SETTERS
    public bool IsWallJumping
    {
        get => isWallJumping;
        set => isWallJumping = value;
    }
    public bool WallCoroutineStart
    {
        get => wallCoroutineStart;
        set => wallCoroutineStart = value;
    }
}
