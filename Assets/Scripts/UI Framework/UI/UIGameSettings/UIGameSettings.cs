using TMPro;
using UI_Framework.Scripts;
using UnityEngine;

namespace UI_Framework.UI.UIGameSettings
{
    public class UIGameSettings : UIFormBase
    {
        public TextMeshProUGUI timeScaleText;
        public TextMeshProUGUI pauseText;
        public PausePanel pausePanel;
        
        protected override void OnInit()
        {
            UpdateTimeScaleUI();
            UpdatePauseUI();
            pausePanel.Close();
            
            Open();
        }

        #region 倍速控制
        public void SwitchTimeScale()
        {
            GameManager.Instance.SwitchGameSpeed();
            UpdateTimeScaleUI();
        }

        private void UpdateTimeScaleUI()
        {
            timeScaleText.text = $"{Time.timeScale:F0}X";
        }
        #endregion
        
        #region 暂停/设置
        private void UpdatePauseUI()
        {
            pauseText.text = Time.timeScale == 0 ? "D" : "ll";
        }

        public void Pause()
        {
            GameManager.Instance.Pause();
            pausePanel.Open();
            UpdatePauseUI();
        }

        public void Continue()
        {
            GameManager.Instance.Continue();
            pausePanel.Close();
            UpdatePauseUI();
        }
        #endregion

        public void Restart() => GameManager.Instance.Restart();

        public void Exit() => GameManager.Instance.Exit();
    }
}