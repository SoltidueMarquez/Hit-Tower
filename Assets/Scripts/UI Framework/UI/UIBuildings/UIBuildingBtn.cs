using System;
using Buildings;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI_Framework.UI.UIBuildings
{
    public class UIBuildingBtn : MonoBehaviour
    {
        public Button btn;
        public TextMeshProUGUI text;
        public float cost;

        public void Init(BuildingData buildingData, Action action)
        {
            cost = buildingData.buildCost;
            text.text = $"{buildingData.buildingName}:{cost}";
            btn.onClick.AddListener(action.Invoke);
        }

        public void UpdateInteractable(float newMoney)
        {
            btn.interactable = newMoney >= cost;
        }
    }
}