using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class LaserBeam
{
    private readonly LayerMask layerMask;
    private Vector3 pos, dir;
    private GameObject laserObj;
    private LineRenderer laserRenderer;
    private List<Vector3> laserIndex = new List<Vector3>();
    
    public LaserBeam(Vector3 pos, Vector3 dir, Material material, LayerMask _layerMask)
    {
        this.layerMask = _layerMask;
        this.laserRenderer = new LineRenderer();
        this.laserObj = new GameObject();
        this.laserObj.name = "Laser";
        this.pos = pos;
        this.dir = dir;
        
        this.laserRenderer = this.laserObj.AddComponent(typeof(LineRenderer)) as LineRenderer;
        this.laserRenderer.startWidth = 0.1f;
        this.laserRenderer.endWidth = 0.1f;
        this.laserRenderer.material = material;
        this.laserRenderer.startColor = Color.green;
        this.laserRenderer.endColor = Color.green;

        CastRay(pos, dir);
    }
    

    void CastRay(Vector3 pos, Vector3 dir)
    {
        laserIndex.Add(pos);

        //Ray2D ray = new Ray2D(pos, dir);
        RaycastHit2D hit2D = Physics2D.Raycast(pos,dir, 50, layerMask);

        if (hit2D.collider != null)
        {
            // laserIndex.Add(hit2D.point);
            // UpdateLaser();
            CheckHit(hit2D, dir);
        }
        else
        {
            laserIndex.Add(new Vector3(pos.x, pos.y + 50));
            UpdateLaser();
        }
    }

    void UpdateLaser()
    {
        int count = 0;
        laserRenderer.positionCount = laserIndex.Count;

        foreach (Vector3 idx in laserIndex)
        {
            laserRenderer.SetPosition(count, idx);
            count++;
        }
    }

    void CheckHit(RaycastHit2D hit, Vector3 direction)
    {
        if (hit.collider.tag == "Mirror")
        {
            Vector3 pos = hit.point;
            Vector3 dir = Vector3.Reflect(direction, hit.normal);
            
            CastRay(pos, dir);
        }
        else
        {
            laserIndex.Add(hit.point);
            UpdateLaser();
        }
    }
}
