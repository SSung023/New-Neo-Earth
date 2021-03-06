using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData", order = int.MinValue)]
public class PlayerData : ScriptableObject
{
    [Header("Move speed")]
    [SerializeField] private float maxMoveSpeed; // 지상에서의 속도
    public float GetMaxMoveSpeed => maxMoveSpeed;

    [SerializeField] private float moveAcceleration; // 가속도
    public float GetMoveAcceleration => moveAcceleration;

    [SerializeField] private float groundLinearDrag; // 저항값
    public float GetGroundLinearDrag => groundLinearDrag;
    
    [SerializeField] private float floatingSpeed; // 특수 동작 후 체공 속도
    public float getFloatingSpeed => floatingSpeed;
    
    [SerializeField] private float slidingSpeed; // 벽타기 속도
    public float getSlidingSpeed => slidingSpeed;

    
    
    [Header("Player Info")]
    [SerializeField] private int maxHp;
    public int getMaxHp => maxHp;

    
    
    [Header("Jump")]
    [SerializeField] private float jumpForce;
    public float getJumpForce => jumpForce;

    [SerializeField] private float maxJumpTime;
    public float getMaxJumpTime => maxJumpTime;
    
    [SerializeField] private float fallMultiplier;
    public float getFallMultiplier => fallMultiplier;

    [SerializeField] private float riseMultiplier;
    public float getRiseMultiplier => riseMultiplier;

    
    
    [Header("Dash")]
    [SerializeField] private float dashForce;
    public float getDashForce => dashForce;

    [SerializeField] private float dashDuration;
    public float getDashDuration => dashDuration;

    
    
    [Header("Wall Move")] 
    [SerializeField] private float wallJumpForce;
    public float getWallJumpForce => wallJumpForce;

    
    
    [Header("LayerMask")]
    [SerializeField] private LayerMask layerMask_ground;
    public LayerMask getLayermask_ground => layerMask_ground;

    [SerializeField] private LayerMask layerMask_wall;
    public LayerMask getLayermask_wall => layerMask_wall;

    [SerializeField] private LayerMask layerMask_normalWall;
    public LayerMask getLayerMask_normalWall => layerMask_normalWall;
}
