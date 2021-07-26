using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundManager
{
    enum Type { mus, sfx, amb, uix };

    // 오디오클립을 받는 클래스
    [SerializeField]
    class AudioclipBank
    {
        AudioClip[] musClips, sfxClips, ambClips, uixClips;

        public AudioClip[] GetClips(Type type)
        {
            return musClips;
        }
    }

    public class SoundManager : MonoBehaviour
    {
        // 싱글톤 디자인을 위한 인스턴스
        static public SoundManager SoundInstance;

        AudioListener audioListener;
        AudioSource mus, sfx, amb, uix; // 음악, 다이렉트 효과음, 환경음, 인터페이스음 오디오 소스

        AudioMixer audioMixer; // 마스터 믹서

        AudioclipBank bank = new AudioclipBank();

        private void Awake()
        {
            // 사운드매니저 게임오브젝트 내 4개의 오디오소스를 얻어냄
            AudioSource[] audioSources = GetComponents<AudioSource>();

            // 오디오소스에 연결된 믹서 그룹 명에 따라 역할 배분
            var count = 0;
            while (count < audioSources.Length)
            {
                switch (audioSources[count].outputAudioMixerGroup.name)
                {
                    case "Music":
                        mus = audioSources[count];
                        break;
                    case "Direct":
                        sfx = audioSources[count];
                        break;
                    case "Ambient":
                        amb = audioSources[count];
                        break;
                    case "Interface":
                        uix = audioSources[count];
                        break;
                }
                count++;
            }
        }
        
    }


}


