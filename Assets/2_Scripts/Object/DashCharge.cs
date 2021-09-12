using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCharge : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float disappear_time;
    private PlayerController playerController;
    private Collider2D collider2D;
    private SpriteRenderer spriteRenderer;
    private bool isActive = true;

    
    
    private void Start()
    {
        if (playerController == null)
        {
            playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        }

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (isActive)
        {
            ChargeDash();
        }

    }

    private void ChargeDash()
    {
        collider2D = Physics2D.OverlapCircle(transform.position, 0.3f, layerMask);
        if (collider2D != null)
        {
            Debug.Log("Player Dash 충전");
            
            playerController.PlayerMove.DashCnt = 1;
            //Destroy(this.gameObject);
            StartCoroutine(Disappear_second());
        }
    }

    IEnumerator Disappear_second()
    {
        isActive = false;
        Color _color = spriteRenderer.color;
        _color.a = 0f;
        spriteRenderer.color = _color;
        
        yield return new WaitForSeconds(disappear_time);
        isActive = true;
        _color.a = 1f;
        spriteRenderer.color = _color;
    }
}
