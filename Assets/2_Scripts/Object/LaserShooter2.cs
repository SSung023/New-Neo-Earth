using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShooter2 : MonoBehaviour
{
    [SerializeField] private Material material;
    [SerializeField] private LayerMask layerMask;
    private LaserBeam laserBeam;

    private void Update()
    {
        Destroy(GameObject.Find("Laser"));
        laserBeam = new LaserBeam(gameObject.transform.position, gameObject.transform.up * -1, material, layerMask);
    }
}
