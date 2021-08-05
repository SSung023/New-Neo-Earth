using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SoundManager
{
    /// <summary>
    /// NEXT : 다음 마디로 이동한다.
    /// LOOP : 스스로 루프한다.             transitionOn일 경우 다음 마디로 이동한다.
    /// MOVE : 특정 마디로 이동시킨다.      transitionOn일 경우 다음 마디로 이동한다.
    /// 이 모든 성질은 1순위 강제 중지 > 2순위 강제 다음 마디 설정 > 3순위로 적용된다.
    /// </summary>
    public enum MeasureType : int { NEXT, LOOP, MOVE, END };

    public class MusicConductor : MonoBehaviour
    {
        public static MusicConductor _music;

        private AudioSource[] bgmSources;

        public AdaptiveTrack[] adaptiveTrack = new AdaptiveTrack[1];

        [Serializable]
        public class AdaptiveTrack
        {
            public AudioClip[] clips;
            public MeasureType[] measures;
            public int[] allocates;

            public MeasureType[] MeasureTypes { get; }
            public int[] MeasureAllocation { get; }
        }

        private void Start()
        {
            _music = this;
            bgmSources = SoundManager._sndInstance.MusicSources;
        }

        // --------------------------------- 적응형 사운드트랙 구현 요소 선언 ---------------------------------

        // 현재 적응형 사운드 트랙이 재생 중인지 확인하는 필드
        private bool adptTrackIsPlaying = false;

        // 현재 재생 중인 사운드 트랙에서 다음 파트로 전환할 것인지를 결정하는 필드
        private bool transitionOn = false;

        // 현재 재생 중인 사운드 트랙 내 현재 재생 중인 마디의 재생이 끝나면, 다음에 재생할 마디를 강제로 지정하는 필드
        private int adptMeasureForceSet = -1; // -1일 경우 비활성화, 0부터는 마디 번호 지정

        // 현재 선택한 적응형 트랙의 클래스
        private AdaptiveTrack currentAdaptiveTrack;

        private int toggle;                     // 적응형 사운드트랙을 재생할 때 어떤 오디오소스를 사용할지 결정하는 필드
        private double measureStartTime;        // 마디가 시작되는 시간
        private double measureDuration;         // 현재 마디가 지속되는 시간
        private double measureCheckTime;        // 다음 마디를 시작하는 시간 - offset의 을 말함, transition 상태를 점검하는 시간

        [SerializeField]
        private double adaptiveTrackStartOffset = 0.5d; // 오디오클립 재생 시 발생하는 로딩시간을 보완하는 값

        // --------------------------------- 적응형 사운드트랙 관련 메서드 ---------------------------------

        /// <summary>
        /// 적응형 사운드트랙을 시작합니다.
        /// </summary>
        /// <param name="trackNo">재생할 트랙 넘버</param>
        public void AdptTrackStart(int trackNo)
        {
            adptTrackIsPlaying = true;
            toggle = 0;

            currentAdaptiveTrack = adaptiveTrack[trackNo];

            measureStartTime = AudioSettings.dspTime + adaptiveTrackStartOffset;

            StartCoroutine(AdptTrackClipStarter(0));
        }

        /// <summary>
        /// preDelay 초만큼 기다린 후 적응형 사운드트랙 재생을 시작합니다.
        /// </summary>
        /// <param name="trackNo">재생할 트랙 넘버</param>
        /// <param name="preDelay">(double) 기다리는 시간</param>
        public void AdptTrackStart(int trackNo, double preDelay)
        {
            adptTrackIsPlaying = true;
            toggle = 0;

            currentAdaptiveTrack = adaptiveTrack[trackNo];

            measureStartTime = AudioSettings.dspTime + adaptiveTrackStartOffset + preDelay;

            StartCoroutine(AdptTrackClipStarter(0));
        }

        // 클립 할당, 재생, 시간 계산을 담당하는 코루틴
        // 1. 클립을 할당하고, 
        // 2. measureStartTime에 클립을 시작한다. 
        // 3. measureDuration을 계산, 다음 measureStartTime을 산출
        // 4. checkTime에 QueueMeasure를 실행
        IEnumerator AdptTrackClipStarter(int startClip)
        {
            if (adptTrackIsPlaying)
            {
                bgmSources[toggle].clip = currentAdaptiveTrack.clips[startClip];

                bgmSources[toggle].PlayScheduled(measureStartTime);

                measureDuration = (double)bgmSources[toggle].clip.samples / bgmSources[toggle].clip.frequency;
                measureStartTime = measureStartTime + measureDuration;
                measureCheckTime = measureStartTime - adaptiveTrackStartOffset;

                yield return new WaitUntil(() => AudioSettings.dspTime > measureCheckTime);

                QueueMeasure(startClip);
            }
        }

        // 다음에 재생할 클립을 결정한 뒤 해당 클립을 코루틴으로 호출하는 메서드
        private void QueueMeasure(int currentClip)
        {
            toggle = 1 - toggle;

            MeasureType type = currentAdaptiveTrack.MeasureTypes[currentClip];
            int allocation = currentAdaptiveTrack.MeasureAllocation[currentClip];

            // 에러 체크용
            if (type == MeasureType.MOVE)
            {
                if (allocation < 0)
                {
                    string message = "오류 : " + currentClip.ToString() + " 번 클립은 MOVE 이지만 다음 마디가 할당되어있지 않습니다.";
                    print(message);
                }
            }

            int nextClip = 0;

            if (adptMeasureForceSet < 0)
            {
                switch (type)
                {
                    case MeasureType.NEXT:
                        nextClip = currentClip + 1;
                        break;
                    case MeasureType.LOOP:
                        nextClip = transitionOn == true ? currentClip + 1 : currentClip;
                        transitionOn = false;
                        break;
                    case MeasureType.MOVE:
                        nextClip = transitionOn == true ? currentClip + 1 : nextClip = allocation;
                        transitionOn = false;
                        break;
                    case MeasureType.END:
                        StopAdaptiveSoundtrack(false, false);
                        break;
                }
            }
            else
            {
                int adjust = Mathf.Clamp(adptMeasureForceSet, 0, currentAdaptiveTrack.clips.Length - 1); // 강제로 마디를 지정했을 때 범위를 벗어나지 않도록 제한을 둠
                nextClip = adjust;

                transitionOn = false;
                adptMeasureForceSet = -1;
            }

            StartCoroutine(AdptTrackClipStarter(nextClip));
        }

        /// <summary>
        /// 현재 진행중인 적응형 사운드트랙을 종료합니다.
        /// </summary>
        /// <param name="now">현재 재생중인 클립이 종료되기를 기다리지 않고 즉시 트랙을 종료합니다.</param>
        /// <param name="fadeout">현재 재생중인 클립을 페이드아웃해서 종료시킬지 결정합니다. now == false 일 경우 적용되지 않습니다.</param>
        private void StopAdaptiveSoundtrack(bool now, bool fadeout)
        {
            if (now)
            {
                if (fadeout)
                {

                }
                bgmSources[0].Stop();
                bgmSources[1].Stop();

                bgmSources[0].clip = null;
                bgmSources[1].clip = null;
            }

            adptTrackIsPlaying = false;
            toggle = 0;
            AdptTransition = false;
            AdptNextMeasureForceSet = -1;
        }

        // ------------------------------ 적응형 사운드트랙 관련 외부에서 호출가능한 프로퍼티/메서드 ----------------------

        /// <summary>
        /// 현재 transitionOn 변수의 상태를 반환하거나, transitionOn의 상태를 스크립트 외부에서 변경하는 프로퍼티
        /// </summary>
        public bool AdptTransition
        {
            get { return transitionOn; }
            set { transitionOn = AdptTransition; }
        }

        /// <summary>
        /// 현재 적응형 사운드트랙 재생 상태를 반환하는 프로퍼티
        /// </summary>
        public bool AdaptiveTrackPlayStatus
        {
            get { return adptTrackIsPlaying; }
        }

        /// <summary>
        /// 현재 재생중인 적응형 사운드트랙의 다음 마디(클립)을 강제로 설정한다. transitionOn의 값과는 상관 없이 진행된다.
        /// </summary>
        public int AdptNextMeasureForceSet
        {
            get { return adptMeasureForceSet; }
            set { if (adptTrackIsPlaying) { adptMeasureForceSet = AdptNextMeasureForceSet; } }
        }
    }
}

