using System;
using System.Collections.Generic;
using UI_Framework.UI;

namespace UI_Framework.Scripts
{
    public static class UIFormRegistry
    {
        private static readonly Dictionary<Type, string> pathMap = new()
        {
            { typeof(PlayerInfoUI), "Prefabs/UI/Player UI" },
        };

        public static string GetPath<T>() where T : UIFormBase
        {
            return pathMap[typeof(T)];
        }
    }
}