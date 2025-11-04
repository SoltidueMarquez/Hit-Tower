using TMPro;
using UI_Framework.Scripts;

namespace UI_Framework.UI.UIGameOver
{
    public class UIGameOver : UIFormBase
    {
        public TextMeshProUGUI titleText;
        public TextMeshProUGUI waveText;
        protected override void OnInit()
        {
        }

        public void InitWin()
        {
            titleText.text = "You Win";
            CommonInit();
            Open();
        }

        public void InitLose()
        {
            titleText.text = "You Lose";
            CommonInit();
            Open();
        }

        private void CommonInit()
        {
            GameManager.Instance.Pause();
            waveText.text = $"You survived {GameManager.Instance.enemyManager.enemySpawner.GetCurrentWaveIndex() + 1} waves this time.";
        }
        
        public void Restart() => GameManager.Instance.Restart();

        public void Exit() => GameManager.Instance.Exit();
    }
}