using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SoundManager
{
    public enum AudioType { mus, sfx, amb, uix };

    public class SoundManager : MonoBehaviour
    {
        // 싱글톤 디자인을 위한 인스턴스
        static public SoundManager _sndInstance;

        private AudioListener audioListener;
        private AudioSource mus,adp, sfx, amb, uix; // 음악, 적응형 트랙 추가 오디오소스, 다이렉트 효과음, 환경음, 인터페이스음 오디오 소스
        private AudioSource[] adpSources = new AudioSource[2];

        [SerializeField]
        AudioMixer topMixer; // 마스터 믹서
        AudioMixer topMus, topSfx, topAmb, topUix; // 플레이어가 옵션에서 조절하는 최상위 믹서 옵션

        private bool masterMute, musMute, sfxMute, ambMute, uixMute;
        private float masterVol, musVol, sfxVol, ambVol, uixVol;

        private enum MixerType { MASTER, MUSIC, DIRECT, AMBIENT, INTERFACE};

        AudioclipBank bank;
        

        private void Awake()
        {
            // 싱글톤 패턴을 위한 인스턴스 할당
            if (_sndInstance == null)
            {
                _sndInstance = this;
            }
            else
            {
                Destroy(this);
                return;
            }

            AllocateAudioSources();
            AllocateAudioMixers();
            SaveMixerStatusAll();

            bank = GetComponent<AudioclipBank>();
        }

        private void Update()
        {
            if (Input.GetKey(KeyCode.BackQuote))
            {
                if(Input.GetKeyDown(KeyCode.Alpha1)) { MuteMixer(MixerType.MASTER); }
                if (Input.GetKeyDown(KeyCode.Alpha2)) { MuteMixer(MixerType.MUSIC); }
                if (Input.GetKeyDown(KeyCode.Alpha3)) { MuteMixer(MixerType.DIRECT); }
                if (Input.GetKeyDown(KeyCode.Alpha4)) { MuteMixer(MixerType.AMBIENT); }
                if (Input.GetKeyDown(KeyCode.Alpha5)) { MuteMixer(MixerType.INTERFACE); }

            }
        }

        // 오디오소스를 할당하는 메서드
        private void AllocateAudioSources()
        {
            // 사운드매니저 게임오브젝트 내 5개의 오디오소스를 얻어냄
            AudioSource[] audioSources = GetComponents<AudioSource>();

            // 오디오소스에 연결된 믹서 그룹 명에 따라 역할 배분
            var count = 0;
            while (count < audioSources.Length)
            {
                switch (audioSources[count].outputAudioMixerGroup.name)
                {
                    case "Music":
                        if (mus == null && adp == null) mus = audioSources[count];
                        else adp = audioSources[count];
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

            adpSources[0] = mus;
            adpSources[1] = adp;
        }

        // 최상위 믹서들을 할당하고 음량 값을 가져오는 메서드
        private void AllocateAudioMixers()
        {
            if(topMixer != null)
            {
                topMixer.GetFloat("TopMasterVol", out masterVol);

                AudioMixerGroup[] groups = topMixer.FindMatchingGroups("Top");

                for (int i = 0; i < groups.Length; i++)
                {
                    switch (groups[i].name)
                    {
                        case "TopMusic":
                            topMus = groups[i].audioMixer;
                            break;
                        case "TopDirect":
                            topSfx = groups[i].audioMixer;
                            break;
                        case "TopAmbient":
                            topAmb = groups[i].audioMixer;
                            break;
                        case "TopInterface":
                            topUix = groups[i].audioMixer;
                            break;
                        default:
                            Debug.Log("AllocatedAudioMixers() 메서드 오류, 최상위 믹서가 너무 많습니다.");
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// 음소거되지 않은 모든 믹서의 볼륨 상태를 저장하는 메서드
        /// </summary>
        private void SaveMixerStatusAll()
        {
            if (!masterMute) topMixer.GetFloat("MasterMixerVol", out masterVol);
            if (!musMute)    topMus.GetFloat("TopMusicVol", out musVol);
            if (!sfxMute)    topSfx.GetFloat("TopDirectVol", out sfxVol);
            if (!ambMute)    topAmb.GetFloat("TopAmbientVol", out ambVol);
            if (!uixMute)    topUix.GetFloat("TopInterfaceVol", out uixVol);
        }

        /// <summary>
        /// 음소거되지 않은 특정 믹서의 볼륨 상태만 저장하는 메서드
        /// </summary>
        /// <param name="type">선택할 믹서</param>
        private void SaveMixerStatus(MixerType type)
        {
            switch (type)
            {
                case MixerType.MASTER:
                    if (!masterMute) topMixer.GetFloat("MasterMixerVol", out masterVol);
                    break;
                case MixerType.MUSIC:
                    if (!musMute)    topMus.GetFloat("TopMusicVol", out musVol);
                    break;
                case MixerType.DIRECT:
                    if (!sfxMute)    topSfx.GetFloat("TopDirectVol", out sfxVol);
                    break;
                case MixerType.AMBIENT:
                    if (!ambMute)    topAmb.GetFloat("TopAmbientVol", out ambVol);
                    break;
                case MixerType.INTERFACE:
                    if (!uixMute)    topUix.GetFloat("TopInterfaceVol", out uixVol);
                    break;
            }
        }

        // 외부 파일로부터 옵션값을 가져올 때 사용할 메서드, PlayerPrefs를 사용하게 될 듯, 현재 미사용
        private void GetSoundPreferences()
        {
            masterMute = false;
            musMute = false;
            sfxMute = false;
            ambMute = false;
            uixMute = false;

            // masterVol 등도 가져와야 한다.
        }

        // 사운드 옵션을 초기화 했을 때 사용하는 메서드
        private void GetDefaultSoundPrefences()
        {
            masterMute = false;
            musMute = false;
            sfxMute = false;
            ambMute = false;
            uixMute = false;
        }

        // 특정 믹서를 음소거하는 메서드
        private void MuteMixer(MixerType type)
        {
            switch (type)
            {
                case MixerType.MASTER:
                    if (!masterMute)
                    {
                        SaveMixerStatus(MixerType.MASTER);
                        topMixer.SetFloat("TopMasterVol", -80.0f);
                        masterMute = true;
                    }
                    else
                    {
                        topMixer.SetFloat("TopMasterVol", masterVol);
                        masterMute = false;
                    }
                    break;
                case MixerType.MUSIC:
                    if (!musMute)
                    {
                        SaveMixerStatus(MixerType.MUSIC);
                        topMus.SetFloat("TopMusicVol", -80.0f);
                        musMute = true;
                    }
                    else
                    {
                        topMus.SetFloat("TopMusicVol", musVol);
                        musMute = false;
                    }
                    break;
                case MixerType.DIRECT:
                    if (!sfxMute)
                    {
                        SaveMixerStatus(MixerType.DIRECT);
                        topSfx.SetFloat("TopDirectVol", -80.0f);
                        sfxMute = true;
                    }
                    else
                    {
                        topSfx.SetFloat("TopDirectVol", sfxVol);
                        sfxMute = false;
                    }
                    break;
                case MixerType.AMBIENT:
                    if (!ambMute)
                    {
                        SaveMixerStatus(MixerType.AMBIENT);
                        topAmb.SetFloat("TopAmbientVol", -80.0f);
                        ambMute = true;
                    }
                    else
                    {
                        topAmb.SetFloat("TopAmbientVol", ambVol);
                        ambMute = false;
                    }
                    break;
                case MixerType.INTERFACE:
                    if (!uixMute)
                    {
                        SaveMixerStatus(MixerType.INTERFACE);
                        topUix.SetFloat("TopInterfaceVol", -80.0f);
                        uixMute = true;
                    }
                    else
                    {
                        topUix.SetFloat("TopInterfaceVol", uixVol);
                        uixMute = false;
                    }
                    break;
            }
        }


        // --------------------------- 스크립트 외부로 값을 반환하는 프로퍼티 ----------------------------
        
        /// <summary>
        /// 음악에 사용할 오디오소스를 반환합니다.
        /// </summary>
        public AudioSource[] MusicSources
        {
            get { return adpSources; }
        }
        
    }
}


