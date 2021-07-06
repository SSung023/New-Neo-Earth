using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Objects/PlayerData", order = int.MinValue)]
public class PlayerData : ScriptableObject
{
    [SerializeField] private float speed;
    public float getSpeed { get { return speed; } }

    [SerializeField] private int maxHp;
    public int getMaxHp { get { return maxHp; } }

    [SerializeField] private float jumpForce;
    public float getJumpForce { get { return jumpForce; } }

    [SerializeField] private float dashForce;
    public float getDashForce { get { return dashForce; } }
}
