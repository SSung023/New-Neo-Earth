using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashMove : MonoBehaviour
{
    private PlayerData playerData;
    private PlayerParam playerParam;
    
    private Vector2 dashVector;
    
    private readonly float dashForce;
    private int dashCnt;
    private bool dashCoroutineStart = false;
    
    public DashMove(PlayerData _playerData, PlayerParam playerParam)
    {
        playerData = _playerData;
        
        this.dashForce = playerData.getDashForce;
    }


    public void Dash()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            playerParam.Rigidbody.velocity = Vector2.zero;
            playerParam.Rigidbody.velocity = dashVector * dashForce;
            dashCoroutineStart = true;
            dashCnt--;
        }
    }
    public void UpdateDashVector()
    {
        dashVector = new Vector2(playerParam.HorizontalMove, playerParam.VerticalMove);

        if (playerParam.HorizontalMove == 0 && playerParam.VerticalMove == 0)
        {
            dashVector = new Vector2(playerParam.IsSightRight, playerParam.VerticalMove);
        }
    }
    
    
    // GETTERS && SETTERS
    public Vector2 DashVector
    {
        get => dashVector;
        set => dashVector = value;
    }
    public int DashCnt
    {
        get => dashCnt;
        set => dashCnt = value;
    }
    public bool DashCoroutineStart
    {
        get => dashCoroutineStart;
        set => dashCoroutineStart = value;
    }
}
