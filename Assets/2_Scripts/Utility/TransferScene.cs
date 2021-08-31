using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransferScene : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Collider2D collider2D;

    private int cur_Scene_num;
    private const float radius = 0.2f;

    
    private void Start()
    {
        cur_Scene_num = SceneManager.GetActiveScene().buildIndex;
    }
    
    private void Update()
    {
        collider2D = Physics2D.OverlapCircle(transform.position, radius, layerMask);
        if (collider2D != null)
        {
            SceneManager.LoadScene(cur_Scene_num + 1);
        }
    }
}
