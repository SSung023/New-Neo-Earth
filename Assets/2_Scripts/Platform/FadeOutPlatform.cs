using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class FadeOutPlatform : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;
    private GameObject wallCheck; // Wall layer가 있는 오브젝트(자식 오브젝트)
    
    private float fadeOutDegree = 0.2f; //투명도가 0.1깎일 때까지 걸리는 시간, 클수록 느려짐
    private float delayTime = 0; //layer가 바뀌기 전까지 걸리는 시간
    private float coolTime = 2f; //없어진 플랫폼이 다시 나타날 때 까지의 쿨타임
    private float disableTiming = 0.7f;

    private int cnt = 0;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        wallCheck = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            cnt++;
            
            if(cnt == 1)
                StartCoroutine(FadeOut());
            else
                StopCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        for (float i = 10; i >= 0; i--)
        {
            float alpha = i / 10.0f;
            spriteRenderer.color = new Color(1,1,1,alpha);
            
            //alpha가 0.7이 되는 순간에 플랫폼 콜라이더 통과로 바꾸기
            if (alpha == disableTiming)
            {
                boxCollider2D.isTrigger = true;
                wallCheck.SetActive(false);
            }
            yield return new WaitForSeconds(fadeOutDegree);
        }
        
        
        yield return new WaitForSeconds(coolTime);
        
        spriteRenderer.color = new Color(1,1,1,1);
        boxCollider2D.isTrigger = false;
        wallCheck.SetActive(true);
        
        cnt = 0;
        
    }
}
