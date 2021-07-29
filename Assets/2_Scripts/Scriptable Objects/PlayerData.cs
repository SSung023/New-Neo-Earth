using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData", order = int.MinValue)]
public class PlayerData : ScriptableObject
{
    [Header("Move speed")]
    [SerializeField] private float speed; // 지상에서의 속도
    public float getSpeed => speed;

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
}
