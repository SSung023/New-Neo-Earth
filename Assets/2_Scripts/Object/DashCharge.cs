using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCharge : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private PlayerController playerController;
    private Collider2D collider2D;

    
    
    private void Start()
    {
        if (playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        ChargeDash();
    }

    private void ChargeDash()
    {
        collider2D = Physics2D.OverlapCircle(transform.position, 0.3f, layerMask);
        if (collider2D != null)
        {
            Debug.Log("Player Dash 충전");
            
            playerController.PlayerMove.DashCnt = 1;
            Destroy(this.gameObject);
        }
    }
}
