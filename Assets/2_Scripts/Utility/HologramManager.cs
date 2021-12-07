using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HologramManager : MonoBehaviour
{
    private bool dpHologram;
    private bool keepHologramOn = false;

    [SerializeField] private GameObject hologramObj;

    private void Start()
    {
        dpHologram = false;
        //hologramObj = transform.GetChild(0).gameObject;
    }
    
    private void Update()
    {
        if (hologramObj == null)
        {
            hologramObj = GameObject.FindGameObjectWithTag("Hologram Object");
        }
        
        if (Input.GetKeyDown(KeyCode.X))
        {
            //Debug.Log(dpHologram);
            dpHologram = !dpHologram;
        }
        LoadHologram();
    }

    private void SetMode()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log(dpHologram);
            dpHologram = !dpHologram;
        }
    }
    
    public void LoadHologram()
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


    // GETTERS & SETTERS
    public GameObject HologramObj
    {
        get => hologramObj;
        set => hologramObj = value;
    }
}
