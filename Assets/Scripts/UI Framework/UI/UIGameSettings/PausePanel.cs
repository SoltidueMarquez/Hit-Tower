using UnityEngine;
using UnityEngine.UI;

namespace UI_Framework.UI.UIGameSettings
{
    public class PausePanel : MonoBehaviour
    {
        public Button continueButton;
        public Button restartButton;
        public Button exitButton;
        
        public void Open()
        {
            SetAllBtnsInteractable(true);
            gameObject.SetActive(true);
        }

        public void Close()
        {
            gameObject.SetActive(false);
        }

        public void SetAllBtnsInteractable(bool interactable)
        {
            continueButton.interactable = interactable;
            restartButton.interactable = interactable;
            exitButton.interactable = interactable;
        }
    }
}