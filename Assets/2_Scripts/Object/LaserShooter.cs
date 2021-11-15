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
    private LineRenderer lineRenderer;

    [SerializeField] private int reflections; // 반사의 최대 횟수
    

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        this.lineRenderer.startWidth = 0.1f;
        this.lineRenderer.endWidth = 0.1f;
        this.lineRenderer.startColor = Color.green;
        this.lineRenderer.endColor = Color.green;
    }

    private void Update()
    {
        ShootLaser(transform.position, transform.up);
        //ShootRay(transform.position, transform.up, rayDistance, 0);
        //ShootRay(transform.position, transform.up, 1);
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
                
                remainingLength -= Vector2.Distance(pos, hit.point);

                //pos = hit.point;
                Debug.Log("origin dir : " + dir);
                Debug.DrawRay(hit.point, dir, Color.blue);
                dir = Vector3.Reflect(dir, hit.normal);
                Debug.Log("reflected dir : " + dir);
                Debug.DrawRay(hit.point, dir, Color.green);
                
                hit = Physics2D.Raycast(hit.point, dir, rayDistance,layerMask);
                Debug.DrawRay(pos, dir, Color.yellow);
                Debug.Log("pos : " + pos);
                Debug.Log("hit : " + hit.point);
                // if (hit.collider.gameObject.tag == "Player")
                // {
                //     GameManager.player.GetComponent<PlayerController>().Die();
                // }
                // if (hit.collider.gameObject.tag == "Mirror")
                // {
                //     pos = hit.point;
                //     dir = Vector3.Reflect(dir, hit.normal);
                //     hit = Physics2D.Raycast(pos, dir, rayDistance,layerMask);
                //     
                //     // lineRenderer.positionCount++;
                //     // lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point + ((Vector2)dir * remainingLength));
                //     //break;
                // }
            }
            else
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, pos + dir * remainingLength);
            }
        }
    }

    private void ShootRay(Vector3 startPos, Vector3 dir, float remained, int count)
    {
        // 이 함수에서는 한 번 반사가 됐을 때, 시작한 위치에서 바로 hit이 되어버리고 있다.
        RaycastHit2D hit = Physics2D.Raycast(startPos, dir, remained, layerMask);
        
        if (hit.collider)
        {
            remained -= Vector2.Distance(startPos, hit.point);
            
            Debug.DrawRay(startPos, dir * remained, Color.yellow);
            
            Debug.Log("cnt/startPos: " + count + " " + startPos);
            //Debug.Log("cnt/hit pos: " + count + " " + hit.point);

            if (count <= reflections)
            {
                count++;

                Vector2 reflectVec = Vector2.Reflect(dir, hit.normal);
                
                Debug.Log("cnt/hit pos: " + (count - 1) + " " + hit.point);
                Debug.Log("cnt/reflectVec: " + (count - 1) + " " + reflectVec);
                Debug.DrawRay(Vector3.zero, reflectVec.normalized, Color.red);
                
                ShootRay(hit.point, reflectVec, remained, count);
            }
        }
    }
    
    void ShootRay(Vector2 startPos, Vector2 dir, int count)
    {
        RaycastHit2D hit = Physics2D.Raycast(startPos, dir, rayDistance, layerMask);

        if (hit.collider)
        {
            float distance = Vector3.Magnitude(startPos - hit.point);
            Debug.DrawRay(startPos, dir * distance, Color.red);
            startPos = hit.point;
            
            if (count <= reflections)
            {
                count++;

                Vector2 reflectVec = Vector2.Reflect(dir, hit.normal);
                
                Debug.Log("cnt/hit pos: " + (count - 1) + " " + hit.point);
                Debug.Log("cnt/startpos: " + (count - 1) + " " + startPos);
                Debug.DrawRay(Vector2.zero, reflectVec.normalized, Color.red);
                
                ShootRay(startPos, reflectVec, count);
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