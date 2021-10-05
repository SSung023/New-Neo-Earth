using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShooter : MonoBehaviour
{
    [SerializeField] private Material material;
    private LaserBeam laserBeam;
    

    // private void Update()
    // {
    //     Destroy(GameObject.Find("Laser"));
    //     laserBeam = new LaserBeam(gameObject.transform.position, gameObject.transform.right, material);
    // }

    private void Start()
    {
        laserBeam = new LaserBeam(gameObject.transform.position, gameObject.transform.up * -1, material);
    }
}
