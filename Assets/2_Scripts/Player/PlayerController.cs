using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    private PlayerMove playerMove;

    
    
    private void Awake()
    {
        playerMove = new PlayerMove(playerData, transform);
        playerMove.Rigidbody = GetComponent<Rigidbody2D>();
    }
    private void Start()
    {
        
        
    }
    private void Update()
    {
        playerMove.UpdateMovement();
    }
}
