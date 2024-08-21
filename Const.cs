using UnityEngine;

namespace alphappy.TAMacro
{
    public class Const
    {
        public const string PLUGIN_GUID = "alphappy.tamacro";
        public const string PLUGIN_NAME = "Debug - TAMacro";
        public const string PLUGIN_VERSION = "0.9.4.7";
        public const int MAXIMUM_INSTRUCTIONS_WITHOUT_TICK = 100;

        public static bool SUPER_DEBUG_MODE = false;
        public static string COOKBOOK_ROOT_PATH = Application.persistentDataPath + "\\ModConfigs\\TAMacro";
        public static string COOKBOOK_MAIN_FILE = Application.persistentDataPath + "\\ModConfigs\\TAMacro\\main.tmc";
        public static string COOKBOOK_RECORDED_FILE = Application.persistentDataPath + "\\ModConfigs\\TAMacro\\recorded.tmc";

        public static bool WARP_MENU_ENABLED = false;
        public static bool DEVCONSOLAS_AVAILABLE = false;

        public static string Font => DEVCONSOLAS_AVAILABLE && Settings.useDevconsolas.Value ? "devconsolas" : RWCustom.Custom.GetFont();
    }
}
