using Enemy;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI_Framework.UI.UIGameInfo
{
    public class TimelineMarker : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private Image markerImage;
        [SerializeField] private GameObject tooltipPanel;
        [SerializeField] private Text tooltipText;
        
        private EnemyWaveDatas.WaveTimeline.WaveEvent timelineEvent;
        private float normalizedTime;
        private bool isTriggered;
        
        public void Init(EnemyWaveDatas.WaveTimeline.WaveEvent @event, float normalizedTime)
        {
            timelineEvent = @event;
            this.normalizedTime = normalizedTime;
            
            // 设置位置
            var rectTransform = GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(normalizedTime, 0);
            rectTransform.anchorMax = new Vector2(normalizedTime, 1);
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = new Vector2(5, 0);
            
            // 设置颜色基于事件类型
            markerImage.color = GetColorForEventType(@event.enemyName);
            
            // 设置提示文本
            tooltipText.text = @event.description;
            tooltipPanel.SetActive(false);
        }
        
        public void UpdateState(float currentTime)
        {
            if (!isTriggered && currentTime >= timelineEvent.absoluteTime)
            {
                // 触发事件效果
                markerImage.color = Color.gray;
                isTriggered = true;
            }
        }
        
        private Color GetColorForEventType(string eventType)
        {
            switch (eventType)
            {
                case "WAVE_START":
                    return Color.yellow;
                case "WAVE_END":
                    return Color.green;
                case "GROUP_INTERVAL":
                    return Color.blue;
                default:
                    return Color.red; // 敌人生成事件
            }
        }
        
        public void OnPointerEnter(PointerEventData eventData)
        {
            tooltipPanel.SetActive(true);
        }
        
        public void OnPointerExit(PointerEventData eventData)
        {
            tooltipPanel.SetActive(false);
        }
    }
}