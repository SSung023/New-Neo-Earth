using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingPlatform : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Jumping Platform"))
        {
            transform.SetParent(col.transform);
        }
    }
}
