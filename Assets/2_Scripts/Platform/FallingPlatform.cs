using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class FallingPlatform : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Transform landingPos;
    private Vector2 checkVector;
    private Vector3 startPos; // tmp code
    private LayerMask playerLayer;
    private Transform checkPos;

    private bool isPlayerOn = false; // 플레이어가 플랫폼에 닿아있는가의 여부
    private bool canExecute; // 플랫폼의 기능이 실행될 수 있는가의 여부

    private int cnt = 0;
    private const float standbyTime = 1f; // 플랫폼 대기 시간


    private void Awake()
    {
        playerLayer = LayerMask.GetMask("Player");
        checkPos = transform.GetChild(0);

        checkVector.x = transform.localScale.x * 1.1f;
        checkVector.y = transform.localScale.y * 0.95f;
    }
    private void Start()
    {
        startPos = transform.position;
        canExecute = true;
    }
    
    private void Update()
    {
        CheckPlayer();
        ReturnToStartPos();
    }

    private void CheckPlayer()
    {
        if (canExecute)
        {
            isPlayerOn = Physics2D.OverlapBox(checkPos.position, checkVector, 0, playerLayer);
            if (isPlayerOn)
            {
                StartCoroutine(StandbyAndDrop());
                canExecute = false;
            }
        }
    }
    
    //tmp code
    private void ReturnToStartPos()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            transform.position = startPos;
            canExecute = true;
        }
    }

    private IEnumerator StandbyAndDrop()
    {
        yield return new WaitForSeconds(standbyTime);
        transform.DOMove(landingPos.position, 0.2f).SetEase(Ease.InQuart);
    }
    
    
    // Debug
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(checkPos.position, checkVector);
    }
}
