using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData", order = int.MinValue)]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float speed;
    public float getSpeed => speed;

    [SerializeField] private float slidingSpeed;
    public float getSlidingSpeed => slidingSpeed;

    [SerializeField] private int maxHp;
    public int getMaxHp => maxHp;

    [SerializeField] private float jumpForce;
    public float getJumpForce => jumpForce;

    [SerializeField] private float dashForce;
    public float getDashForce => dashForce;

    [SerializeField] private float dashCoolTime;
    public float getDashCoolTime => dashCoolTime;

    [SerializeField] private LayerMask layerMask_ground;
    public LayerMask getLayermask_ground => layerMask_ground;

    [SerializeField] private LayerMask layerMask_wall;
    public LayerMask getLayermask_wall => layerMask_wall;
}
