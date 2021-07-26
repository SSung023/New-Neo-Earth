using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundManager
{
    public class AudioclipBank : MonoBehaviour
    {
        // 오디오클립을 받는 클래스
        [SerializeField]
        AudioClip[] musClips, sfxClips, ambClips, uixClips;

        /// <summary>
        /// AudioclipBank에서 클립을 가져옵니다.
        /// </summary>
        /// <param name="type">AudioType 형식으로 입력</param>
        /// <returns></returns>
        public AudioClip[] GetClips(AudioType type)
        {
            switch (type)
            {
                case AudioType.mus:
                    return musClips;
                case AudioType.sfx:
                    return sfxClips;
                case AudioType.amb:
                    return ambClips;
                case AudioType.uix:
                    return uixClips;
                default:
                    Debug.Log("AudioclipBank.GetClips 메서드의 인자 입력에 오류가 있습니다.");
                    return null;
            }
        }

        /// <summary>
        /// AudioclipBank에서 클립을 가져옵니다. (int 형)
        /// </summary>
        /// <param name="type"> mus, sfx, amb, uix 순서</param>
        /// <returns></returns>
        public AudioClip[] GetClips(int type)
        {
            switch (type)
            {
                case 0:
                    return musClips;
                case 1:
                    return sfxClips;
                case 2:
                    return ambClips;
                case 3:
                    return uixClips;
                default:
                    Debug.Log("AudioclipBank.GetClips 메서드의 인자 입력에 오류가 있습니다.");
                    return null;
            }
        }
    }
}
