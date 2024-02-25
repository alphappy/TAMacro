using UnityEngine;

namespace alphappy.TAMacro
{
    public class Const
    {
        public const string PLUGIN_GUID = "alphappy.tamacro";
        public const string PLUGIN_NAME = "Debug - TAMacro";
        public const string PLUGIN_VERSION = "0.5.0.11";
        public const int MAXIMUM_INSTRUCTIONS_WITHOUT_TICK = 100;

        public static bool SUPER_DEBUG_MODE = false;
        public static string COOKBOOK_ROOT_PATH = Application.persistentDataPath + "\\ModConfigs\\TAMacro";
        public static string COOKBOOK_MAIN_FILE = Application.persistentDataPath + "\\ModConfigs\\TAMacro\\main.tmc";
        public static KeyCode[] SELECT_KEYS = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };
    }
}
