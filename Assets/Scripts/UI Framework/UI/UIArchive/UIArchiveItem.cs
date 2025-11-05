using Archive;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Framework.UI.UIArchive
{
    public class UIArchiveItem : MonoBehaviour
    {
        public Image image;
        public TextMeshProUGUI text;
        public Button choseButton;
        public Button resetButton;

        private SinglePlayerArchiveData m_Player;
        
        public void Init(SinglePlayerArchiveData player)
        {
            m_Player = player;
            UpdateView();
            
            choseButton.onClick.AddListener(()=>
            {
                ArchiveManager.Instance.SetCurrentPlayer(m_Player);
            });
            
            resetButton.onClick.AddListener(() =>
            {
                ArchiveManager.Instance.ResetCurPlayer();
                UpdateView();
            });

            ArchiveManager.Instance.OnArchiveSelected += UpdateView;
        }

        private void UpdateView()
        {
            image.color =
                ArchiveManager.Instance.data.players[ArchiveManager.Instance.data.curPlayerDataIndex] == m_Player
                    ? Color.green
                    : Color.white;

            text.text = $"Player:\nMax Wave : {m_Player.maxWave}";
        }

        private void OnDestroy()
        {
            ArchiveManager.Instance.OnArchiveSelected -= UpdateView;
        }
    }
}