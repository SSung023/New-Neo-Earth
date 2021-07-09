using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class FadeOutPlatform : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    
    private float fadeOutSpeed = 0.1f; //투명도가 0.1깎일 때까지 걸리는 시간, 클수록 느려짐
    private float delayTime = 0; //layer가 바뀌기 전까지 걸리는 시간
    private float coolTime = 2f; //없어진 플랫폼이 다시 나타날 때 까지의 쿨타임

    private int cnt = 0;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            cnt++;
            
            if(cnt == 1)
                StartCoroutine(FadeOut());
        }
    }

    IEnumerator FadeOut()
    {
        for (float i = 10; i >= 0; i--)
        {
            float alpha = i / 10.0f;
            _spriteRenderer.color = new Color(1,1,1,alpha);
            
            //alpha가 0.7이 되는 순간에 플랫폼 콜라이더 통과로 바꾸기
            if (alpha == 0.7f)
            {
                GetComponent<BoxCollider2D>().isTrigger = true;
            }
            yield return new WaitForSeconds(fadeOutSpeed);
        }
        
        yield return new WaitForSeconds(coolTime);
        
        _spriteRenderer.color = new Color(1,1,1,1);
        GetComponent<BoxCollider2D>().isTrigger = false;
        
        cnt = 0;
    }
}
