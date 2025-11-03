using UI_Framework.Scripts;

namespace UI_Framework.UI.GameInfoUI
{
    public class UIGameInfo : UIFormBase
    {
        // 波次时间轴
        public WaveProgressBar waveProgressBar;
        protected override void OnInit()
        {
            Open();

            waveProgressBar.Init(GameManager.Instance.enemyManager.enemySpawner);
        }
    }
}