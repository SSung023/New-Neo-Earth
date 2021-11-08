using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class LaserShooter : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float rayDistance = 100;
    //[SerializeField] private Transform laserFirePos;
    private LineRenderer lineRenderer;

    //private Vector3 pos;
    [SerializeField] private int reflections; // 반사의 최대 횟수
    

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        this.lineRenderer.startWidth = 0.1f;
        this.lineRenderer.endWidth = 0.1f;
        this.lineRenderer.startColor = Color.green;
        this.lineRenderer.endColor = Color.green;

        //pos = transform.position;
    }

    private void Update()
    {
        ShootLaser(transform.position, transform.up * -1);
    }

    private void ShootLaser(Vector3 pos, Vector3 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, rayDistance,layerMask);

        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);

        float remainingLength = rayDistance;

        for (int i = 0; i < reflections; i++)
        {
            if (hit.collider)
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);
                
                remainingLength -= Vector3.Distance(pos, hit.point);

                // pos = hit.point;
                // hit = Physics2D.Raycast(pos, Vector3.Reflect(dir, hit.normal), rayDistance,layerMask);
                
                if (hit.collider.gameObject.tag == "Player")
                {
                    GameManager.player.GetComponent<PlayerController>().Die();
                }
                if (hit.collider.gameObject.tag == "Mirror")
                {
                    pos = hit.point;
                    dir = Vector3.Reflect(dir, hit.normal);
                    hit = Physics2D.Raycast(pos, dir, rayDistance,layerMask);
                    
                    lineRenderer.positionCount++;
                    lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point + ((Vector2)dir * remainingLength));
                    break;
                }
                else
                {
                    // lineRenderer.positionCount++;
                    // lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos + dir * remainingLength);
                }
            }
        }
    }

    private void CheckHit(RaycastHit2D hit, Vector2 direction)
    {
        if (hit.collider.gameObject.tag == "Mirror")
        {
            Vector2 pos = hit.point;
            Vector2 dir = Vector2.Reflect(direction, hit.normal);
            
        }
        else if(hit.collider.gameObject.tag == "Player")
        {
            GameManager.player.GetComponent<PlayerController>().Die();
        }
    }
}