using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramField : MonoBehaviour
{
    private bool dpHologram;

    private void Start()
    {
        dpHologram = false;
    }

    private void FixedUpdate()
    {
        if (dpHologram)
        {
            LoadHologram();
        }
    }

    private void LoadHologram()
    {
        
    }
    
    
}
