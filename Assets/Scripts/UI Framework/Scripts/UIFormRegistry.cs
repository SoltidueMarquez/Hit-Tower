using System;
using System.Collections.Generic;
using UI_Framework.UI;
using UI_Framework.UI.UIGameSettings;
using UI_Framework.UI.UIStart;

namespace UI_Framework.Scripts
{
    public static class UIFormRegistry
    {
        private static readonly Dictionary<Type, string> pathMap = new()
        {
            { typeof(StartUI), "Prefabs/UI/Start Scene/StartUI" },
            { typeof(PlayerInfoUI), "Prefabs/UI/Player UI" },
            { typeof(GameSettingsUI), "Prefabs/UI/GameSettingsUI" },
        };

        public static string GetPath<T>() where T : UIFormBase
        {
            return pathMap[typeof(T)];
        }
    }
}