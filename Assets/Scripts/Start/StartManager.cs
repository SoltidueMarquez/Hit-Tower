using UI_Framework.Scripts;
using UI_Framework.Scripts.Tools;
using UI_Framework.UI.UIStart;
using UnityEditor;
using UnityEngine;

namespace Start
{
    public class StartManager : Singleton<StartManager>
    {
        private void Start()
        {
            Time.timeScale = 1;
            UIMgr.Instance.CreateUI<UIStart>();
        }

        public void StartGame()
        {
            SceneChangeHelper.Instance.LoadScene(ConstManager.GameSceneName);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            // 在编辑器中停止播放模式
            EditorApplication.isPlaying = false;
#else
            // 在实际运行时退出游戏
            Application.Quit();
#endif
        }
    }
}