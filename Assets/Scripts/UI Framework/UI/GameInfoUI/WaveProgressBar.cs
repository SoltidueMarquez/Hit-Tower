using System.Collections.Generic;
using System.Linq;
using Enemy;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Framework.UI.GameInfoUI
{
    public class WaveProgressBar : MonoBehaviour
    {
        [Header("UI组件")]
        [SerializeField] private Slider progressSlider;
        [SerializeField] private Text waveText;
        [SerializeField] private Text timeText;
        [SerializeField] private RectTransform timelineMarkerContainer;
        
        [Header("预制体")]
        [SerializeField] private GameObject timelineMarkerPrefab;
        
        private EnemyWaveDatas enemyWaveDatas;
        private EnemyWaveDatas.WaveTimeline currentTimeline;
        private float levelStartTime;
        private bool isLevelActive;
        
        // 时间轴标记缓存
        private List<GameObject> timelineMarkers = new List<GameObject>();
        
        public void Init(EnemySpawner spawner)
        {
            enemyWaveDatas = spawner.EnemyWaveDataList;
            currentTimeline = spawner.EnemyWaveDataList.GenerateTimeline();
            ClearTimelineMarkers();
            CreateTimelineMarkers();

            spawner.OnSpawnStart += StartLevel;
            spawner.OnSpawnEnd += EndLevel;
        }
        
        private void Update()
        {
            if (isLevelActive && currentTimeline != null)
            {
                UpdateProgress();
            }
        }

        private void StartLevel()
        {
            levelStartTime = Time.time;
            isLevelActive = true;
            progressSlider.value = 0f;
            timeText.text = "0.0s";
        }

        private void EndLevel()
        {
            isLevelActive = false;
            progressSlider.value = 1f;
            timeText.text = "关卡完成";
        }
        
        public void PauseLevel()
        {
            isLevelActive = false;
        }
        
        public void ResumeLevel()
        {
            isLevelActive = true;
            levelStartTime = Time.time - (progressSlider.value * currentTimeline.totalDuration);
        }
        
        private void UpdateProgress()
        {
            float elapsedTime = Time.time - levelStartTime;
            float progress = Mathf.Clamp01(elapsedTime / currentTimeline.totalDuration);
            
            progressSlider.value = progress;
            timeText.text = $"{elapsedTime:F1}s";
            
            // 更新当前波次显示
            UpdateCurrentWaveText(elapsedTime);
            
            // 更新时间轴标记状态
            UpdateTimelineMarkers(elapsedTime);
        }
        
        private void UpdateCurrentWaveText(float currentTime)
        {
            var currentWave = GetCurrentWaveIndex(currentTime);
            waveText.text = currentWave >= 0 ? $"第 {currentWave + 1} 波" : "准备中...";
        }
        
        private int GetCurrentWaveIndex(float currentTime)
        {
            for (int i = 0; i < enemyWaveDatas.waveDataList.Count; i++)
            {
                float waveStartTime = GetWaveStartTime(i);
                float waveEndTime = GetWaveEndTime(i);
                
                if (currentTime >= waveStartTime && currentTime <= waveEndTime)
                {
                    return i;
                }
            }
            return -1;
        }
        
        private float GetWaveStartTime(int waveIndex)
        {
            // 查找该波次的真正开始事件（跳过等待事件）
            foreach (var evt in currentTimeline.events)
            {
                if (evt.waveIndex == waveIndex && evt.enemyName == "WAVE_START" && 
                    evt.description.Contains("开始生成"))
                {
                    return evt.absoluteTime;
                }
            }
            return 0f;
        }
        
        private float GetWaveEndTime(int waveIndex)
        {
            // 查找该波次的结束事件
            foreach (var evt in currentTimeline.events)
            {
                if (evt.waveIndex == waveIndex && evt.enemyName == "WAVE_END")
                {
                    return evt.absoluteTime;
                }
            }
            return currentTimeline.totalDuration;
        }
        
        private void CreateTimelineMarkers()
        {
            ClearTimelineMarkers();
            
            foreach (var timelineEvent in currentTimeline.events)
            {
                CreateTimelineMarker(timelineEvent);
            }
        }
        
        private void CreateTimelineMarker(EnemyWaveDatas.WaveTimeline.WaveEvent timelineEvent)
        {
            if (timelineMarkerPrefab == null || timelineMarkerContainer == null) return;
            
            var markerObj = Instantiate(timelineMarkerPrefab, timelineMarkerContainer);
            var marker = markerObj.GetComponent<TimelineMarker>();
            
            if (marker != null)
            {
                float normalizedTime = timelineEvent.absoluteTime / currentTimeline.totalDuration;
                marker.Init(timelineEvent, normalizedTime);
                timelineMarkers.Add(markerObj);
            }
        }
        
        private void UpdateTimelineMarkers(float currentTime)
        {
            foreach (var marker in timelineMarkers
                         .Select(markerObj => markerObj.GetComponent<TimelineMarker>())
                         .Where(marker => marker != null))
            {
                marker.UpdateState(currentTime);
            }
        }
        
        private void ClearTimelineMarkers()
        {
            foreach (var marker in timelineMarkers.Where(marker => marker != null))
            {
                Destroy(marker);
            }

            timelineMarkers.Clear();
        }
    }
}