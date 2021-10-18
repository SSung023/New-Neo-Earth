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

        CastRay(pos, dir, laserRenderer);
    }
    

    void CastRay(Vector3 pos, Vector3 dir, LineRenderer lineRenderer)
    {
        laserIndex.Add(pos);

        //Ray2D ray = new Ray2D(pos, dir);
        RaycastHit2D hit2D = Physics2D.Raycast(pos,dir, 50, layerMask);

        if (hit2D.collider != null)
        {
            laserIndex.Add(hit2D.point);
            UpdateLaser();
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
}
