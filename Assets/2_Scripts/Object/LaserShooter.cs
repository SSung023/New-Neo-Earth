using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float rayDistance = 100;
    [SerializeField] private Transform laserFirePos;
    private Transform m_transform;
    private LineRenderer lineRenderer;
    
    

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();
        
        this.lineRenderer.startWidth = 0.1f;
        this.lineRenderer.endWidth = 0.1f;
        this.lineRenderer.startColor = Color.green;
        this.lineRenderer.endColor = Color.green;
    }

    private void Update()
    {
        ShootLaser();
    }

    private void ShootLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(m_transform.position, transform.up * -1, int.MaxValue,layerMask);

        if (hit.collider != null)
        {
            DrawRay2D(m_transform.position, hit.point);
        }
        else
        {
            DrawRay2D(m_transform.position, m_transform.up * (-1  * rayDistance));
        }
    }

    private void DrawRay2D(Vector2 start, Vector2 end)
    {
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }
}
