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

    private bool increaseFlag = false;
    private int idx; // lineRenderer의 index

    private List<Vector3> laserIndex = new List<Vector3>();
    
    

    private void Awake()
    {
        m_transform = GetComponent<Transform>();
        lineRenderer = GetComponent<LineRenderer>();
        
        this.lineRenderer.startWidth = 0.1f;
        this.lineRenderer.endWidth = 0.1f;
        this.lineRenderer.startColor = Color.green;
        this.lineRenderer.endColor = Color.green;

        idx = 0;
        laserIndex[0] = transform.position;
    }

    private void Update()
    {
        ShootLaser(transform.position, transform.up * -1);
    }

    private void ShootLaser(Vector3 pos, Vector3 dir)
    {
        RaycastHit2D hit = Physics2D.Raycast(pos, dir, int.MaxValue,layerMask);

        if (hit.collider != null)
        {
            CheckHit(hit, dir);
            DrawRay2D(pos, hit.point);
        }
        else
        {
            DrawRay2D(pos, dir  * rayDistance);
        }
    }

    private void DrawRay2D(Vector2 start, Vector2 end)
    {
        //lineRenderer.SetPosition(0, start);
        //lineRenderer.SetPosition(1, end);
        lineRenderer.SetPosition(idx, start);
        lineRenderer.SetPosition(idx + 1, end);

        //lineRenderer.positionCount++; // 한 번만 늘어나야 하는데..
        //idx++;
    }

    private void ApplyList()
    {
        int count = 0;
        foreach (var item in laserIndex)
        {
            lineRenderer.SetPosition(count, item);
            count++;
        }
    }

    private void AddPoint(Vector2 point)
    {
        laserIndex.Add(point);
    }

    private void CheckHit(RaycastHit2D hit, Vector2 direction)
    {
        // 해당 Mirror 오브젝트를 처음 만났을 때 그 때에 한 번만 이루어져야 한다. 지금은 Update문을 통해서 idx가 급작스럽게 늘어난다
        // idx 컨트롤 할 방법이 필요하다
        if (hit.collider.gameObject.tag == "Mirror" && !increaseFlag)
        {
            
            Vector2 pos = hit.point;
            Vector2 dir = Vector2.Reflect(direction, hit.normal);

            increaseFlag = true;
            ShootLaser(pos, dir);
        }
        else if(hit.collider.gameObject.tag == "Player")
        {
            GameManager.player.GetComponent<PlayerController>().Die();
        }
    }
}