using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramManager : MonoBehaviour
{
    private bool dpHologram;

    private GameObject hologramObj;

    private void Start()
    {
        dpHologram = false;
        hologramObj = GameObject.FindWithTag("Hologram On");
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log(dpHologram);
            dpHologram = !dpHologram;
        }
        LoadHologram();
    }

    private void SetMode()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Debug.Log(dpHologram);
            dpHologram = !dpHologram;
        }
    }
    
    private void LoadHologram()
    {
        if (dpHologram)
        {
            hologramObj.SetActive(true);
        }
        else
        {
            hologramObj.SetActive(false);
        }
    }
}
