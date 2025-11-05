using System;
using System.Collections.Generic;
using UI_Framework.UI;
using UI_Framework.UI.UIArchive;
using UI_Framework.UI.UIBuildings;
using UI_Framework.UI.UIDamageText;
using UI_Framework.UI.UIDebug;
using UI_Framework.UI.UIGameInfo;
using UI_Framework.UI.UIGameOver;
using UI_Framework.UI.UIGameSettings;
using UI_Framework.UI.UIShop;
using UI_Framework.UI.UIStart;

namespace UI_Framework.Scripts
{
    public static class UIFormRegistry
    {
        private static readonly Dictionary<Type, string> pathMap = new()
        {
            { typeof(UIStart), "Prefabs/UI/Start Scene/StartUI" },
            { typeof(UIPlayerInfo), "Prefabs/UI/Player UI" },
            { typeof(UIGameSettings), "Prefabs/UI/GameSettingsUI" },
            { typeof(UIGameInfo), "Prefabs/UI/UIGameInfo/Game Info UI" },
            { typeof(UIBuildings), "Prefabs/UI/UIBuildings/BuildingAndUpgrade UI" },
            { typeof(UIDebug), "Prefabs/UI/UIDebug/Debug UI" },
            { typeof(UIGameOver), "Prefabs/UI/UIGameOver/GamOver UI" },
            { typeof(UIDamageInfo), "Prefabs/UI/UIDamageInfo/Damage Info UI" },
            { typeof(UIShop), "Prefabs/UI/UIShop/Shop UI" },
            { typeof(UIArchive), "Prefabs/UI/UIArchive/Archieve UI" },
        };

        public static string GetPath<T>() where T : UIFormBase
        {
            return pathMap[typeof(T)];
        }
    }
}