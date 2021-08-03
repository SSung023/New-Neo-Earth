using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFoley : MonoBehaviour
{
    public static PlayerFoley playerFoley;

    AudioSource aud;
    PlayerMove playerMove;

    [SerializeField]
    AudioClip[] footstepsConcrete;
    [SerializeField]
    AudioClip[] footstepsConcreteLand;

    [SerializeField]
    AudioClip[] footstepsWater;
    [SerializeField]
    AudioClip[] footstepsWaterLand;

    [SerializeField]
    AudioClip[] jumpClip;

    public float walkSpan = 0.4f; // 한 발 딛고 다음 발 딛을 때 까지의 시간
    public float walkAcctime = 1.0f; // 발걸음을 시작해서 완전히 가속될 때까지 걸리는 시간
    public float walkDectime = 1.0f; // 완전 가속 상태에서 발걸음을 완전히 멈출 때까지 걸리는 시간

    private float pitch = 1.0f;

    private bool walkSoundPlay = false;

    private void Start()
    {
        playerFoley = this;
        aud = GetComponent<AudioSource>();
        playerMove = GetComponent<PlayerMove>();
    }

    // 현재 땅에 착지한 상태인지, 바닥의 재질은 어떠한지에 따라 다른 발소리를 재생함
    public void PlayFootstep(string material, bool land)
    {
        AudioClip[] clips;
        switch (material)
        {
            case "Concrete":
                if (land)
                { clips = footstepsConcreteLand; }
                else
                { clips = footstepsConcrete; }
                break;
            case "Water":
                if (land)
                { clips = footstepsWaterLand; }
                else
                { clips = footstepsWater; }
                break;
            default:
                Debug.Log("잘못된 footstep 메터리얼입니다.");
                clips = null;
                break;
        }

        if(clips != null)
        {
            int i = Random.Range(0, clips.Length);
            pitch = Random.Range(0.8f, 1.2f);

            aud.clip = clips[i];
            aud.pitch = pitch;

            aud.Play();
        }
    }

    // 현재 바닥의 재질은 어떠한지에 따라 다른 착지소리를 재생함
    public void PlayJump()
    {
        if (jumpClip != null)
        {
            int i = Random.Range(0, jumpClip.Length);
            pitch = Random.Range(0.9f, 1.1f);

            aud.clip = jumpClip[i];
            aud.pitch = pitch;

            aud.Play();
        }
    }

    IEnumerator FootstepSound()
    {
        if (!walkSoundPlay)
        {
            walkSoundPlay = true;
            PlayFootstep("Concrete", false);
            yield return new WaitForSeconds(walkSpan);
            walkSoundPlay = false;
        }
    }
}
