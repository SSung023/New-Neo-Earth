using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{

    [SerializeField] private Transform target;
    [SerializeField] private float smoothSpeed = 1.5f;

    private Vector3 desiredPos;
    private Vector3 offset = new Vector3(0,0, -2f);

    [SerializeField] private LayerMask cameraBoundLayerMask;
    private bool bound_x, bound_y;
    [SerializeField] private Transform[] boundTransform;
    
    private int dir; //0:left, 1:up, 2:right, 3:down
    private float horizontalMove, verticalMove;
    

    private void Start()
    {

    }
    
    private void LateUpdate()
    {
        // CheckDir();
        // CheckBounds();
        // ControlCamera();
        //
        // Debug.Log("x: " + bound_x);
        // Debug.Log("y: " + bound_y);
        
        desiredPos = target.position + offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.fixedDeltaTime);
        transform.position = smoothedPos;
    }
    
    
    private void CheckDir()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal");
        verticalMove = Input.GetAxisRaw("Vertical");

        if (horizontalMove == -1 && verticalMove == 0)
        {
            dir = 0;
        }
        else if (horizontalMove == 0 && verticalMove == 1)
        {
            dir = 1;
        }
        else if (horizontalMove == 1 && verticalMove == 0)
        {
            dir = 2;
        }
        else if (horizontalMove == 0 && verticalMove == -1)
        {
            dir = 3;
        }
    }
    private void ControlCamera()
    {
        // // 왼쪽 혹은 오른쪽으로 더 이상 진행을 못할 때
        // if (isBound[0] || isBound[2])
        // {
        //     desiredPos = new Vector3(transform.position.x, target.position.y) + offset;
        // }
        // // 위 혹은 아래로 더 이상 진행을 못할 때
        // else if (isBound[1] || isBound[3])
        // {
        //     desiredPos = new Vector3(target.position.x, transform.position.y) + offset;
        // }
        // // 부딫히는 방향이 없을 때
        // else if(!isBound[0] && !isBound[1] && !isBound[2] && !isBound[3])
        // {
        //     desiredPos = target.position + offset;
        // }

        if (bound_x)
        {
            desiredPos = new Vector3(transform.position.x, target.position.y) + offset;
        }
        else if (bound_y)
        {
            desiredPos = new Vector3(target.position.x, transform.position.y) + offset;
        }
        else if (!bound_x && !bound_y)
        {
            desiredPos = target.position + offset;
        }
        
    }
    private void CheckBounds()
    {
        if (dir == 0 || dir == 2) // x축 이동
        {
            bound_x = Physics2D.OverlapBox(boundTransform[dir].position, new Vector2(1, 14), 0,cameraBoundLayerMask);
            bound_y = false;
        }

        else if (dir == 1 || dir == 3) // y축 이동
        {
            bound_y = Physics2D.OverlapBox(boundTransform[dir].position, new Vector2(25, 1), 0,cameraBoundLayerMask);
            bound_x = false;
        }
    }


    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        
        if (dir == 0 || dir == 2) // x축 이동
        {
            Gizmos.DrawWireCube(boundTransform[dir].position, new Vector2(1, 14));
        }

        if (dir == 1 || dir == 3)
        {
            Gizmos.DrawWireCube(boundTransform[dir].position, new Vector2(25, 1));
        }
    }
}
