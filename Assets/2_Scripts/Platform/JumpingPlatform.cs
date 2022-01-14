using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingPlatform : MonoBehaviour
{
    private RaycastHit2D rayHit;
    private float _yRay;

    private Transform parentPlatform;
    
    private void Start()
    {
        //_yRay = gameObject.transform.localScale.y * 0.7f;
        parentPlatform = transform.parent;
        Debug.Log("Jumping의 부모: " + parentPlatform.name);
    }

    private void FixedUpdate()
    {
        //DrawRay();
        
        
        
        transform.parent = parentPlatform;
    }

    private void DrawRay()
    {
        rayHit = Physics2D.Raycast(transform.position, Vector3.down, _yRay, LayerMask.GetMask("Ground"));
            
        Debug.DrawRay(transform.position, Vector3.down * _yRay, Color.green);

        /*if (rayHit.collider.gameObject.CompareTag("Moving Platform"))
        {
            Debug.Log("점핑과 무빙이 닿아있음");
            
            transform.SetParent(rayHit.collider.transform);
        }*/
        if (rayHit == true)
        {
            Debug.Log("점핑과 무빙이 닿아있음");

            parentPlatform = rayHit.collider.gameObject.transform;
        }
        else
        {
            parentPlatform = null;
        }
    }
}
