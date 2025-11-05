using Archive;
using UI_Framework.Scripts;
using UI_Framework.Scripts.Tools;
using UI_Framework.UI.UIArchive;
using UI_Framework.UI.UIStart;
using UnityEditor;
using UnityEngine;

namespace Start
{
    public class StartManager : Singleton<StartManager>
    {
        public ArchiveManager archiveManager;
        private void Start()
        {
            Time.timeScale = 1;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;

            if (ArchiveManager.Instance == null)
                archiveManager.Init();// 这也算是一种生命周期管理吧,大概
            
            UIMgr.Instance.CreateUI<UIStart>();
            UIMgr.Instance.CreateUI<UIArchive>().Close();
        }

        public void StartGame()
        {
            SceneChangeHelper.Instance.LoadScene(ConstManager.k_GameSceneName);
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