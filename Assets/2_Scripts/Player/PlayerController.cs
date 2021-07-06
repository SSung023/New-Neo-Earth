using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    private PlayerMove playerMove;
    
    
    
    private void Start()
    {
        playerMove = new PlayerMove(playerData.getSpeed, playerData.getJumpForce, playerData.getDashForce);
        
    }
    
    private void Update()
    {
        playerMove.Move(transform);
    }
}
