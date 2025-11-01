using UI_Framework.Scripts;

namespace UI_Framework.UI.GameInfoUI
{
    public class GameInfoUI : UIFormBase
    {
        // 波次时间轴
        public WaveProgressBar waveProgressBar;
        protected override void OnInit()
        {
            Open();

            waveProgressBar.Init(GameManager.Instance.EnemyManager.enemySpawner);
        }
    }
}