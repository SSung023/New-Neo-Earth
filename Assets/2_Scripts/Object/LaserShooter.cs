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
    [SerializeField] private Transform laserFirePos;
    private Transform m_transform;
    private LineRenderer lineRenderer;

    private Vector2 dir;
    
    private List<Vector3> laserIndex = new List<Vector3>();
    
    

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();
        
        this.lineRenderer.startWidth = 0.1f;
        this.lineRenderer.endWidth = 0.1f;
        this.lineRenderer.startColor = Color.green;
        this.lineRenderer.endColor = Color.green;

        dir = transform.up * -1;
    }

    private void Update()
    {
        ShootLaser();
    }

    private void ShootLaser()
    {
        RaycastHit2D hit = Physics2D.Raycast(m_transform.position, dir, int.MaxValue,layerMask);

        if (hit.collider != null)
        {
            CheckHit(hit, dir);
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
    
    private void CheckHit(RaycastHit2D hit, Vector2 direction)
    {
        if (hit.collider.gameObject.tag == "Mirror Laser")
        {
            Vector2 pos = hit.point;
            Vector2 dir = Vector2.Reflect(direction, hit.normal);
            
            
        }
        else
        {
            
        }
    }
}
