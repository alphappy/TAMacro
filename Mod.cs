using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx;
using UnityEngine;
using System.Resources;

namespace alphappy.TAMacro
{
    [BepInPlugin(Const.PLUGIN_GUID, Const.PLUGIN_NAME, Const.PLUGIN_VERSION)]
    public class Mod : BaseUnityPlugin
    {
        private static bool initialized = false;

        public static string configPath = Application.persistentDataPath + "\\ModConfigs\\TAMacro";

        public static string defaultCookbookFilepath = Application.persistentDataPath + "\\ModConfigs\\TAMacro\\main.tmc";

        public static void Log(object obj) { Debug.Log($"[TAMacro]  {obj}"); }
        public static void Log(Exception exc) { Log($"UNCAUGHT EXCEPTION: {exc}"); }

        private void OnEnable()
        {
            On.RainWorld.OnModsInit += RainWorldOnModsInitHook;
        }

        private void RainWorldOnModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            if (initialized) return;
            try
            {
                On.Player.checkInput += Player_checkInput;
                On.RainWorldGame.Update += RainWorldGame_Update;
                On.RoomCamera.ctor += RoomCamera_ctor;
                On.RoomCamera.ClearAllSprites += RoomCamera_ClearAllSprites;
                On.RainWorldGame.GrafUpdate += RainWorldGame_GrafUpdate;
                Log("Hooking complete");
                initialized = true;

                Log($"Ensuring main cookbook ready:  {defaultCookbookFilepath}");
                if (!Directory.Exists(configPath)) Directory.CreateDirectory(configPath);
                if (!File.Exists(defaultCookbookFilepath)) File.WriteAllText(defaultCookbookFilepath, "");
            }
            catch (Exception e) { Log(e); }
        }

        private void RoomCamera_ClearAllSprites(On.RoomCamera.orig_ClearAllSprites orig, RoomCamera self)
        {
            DisplayPanel.Remove();
            orig(self);
        }

        private void RainWorldGame_GrafUpdate(On.RainWorldGame.orig_GrafUpdate orig, RainWorldGame self, float timeStacker)
        {
            orig(self, timeStacker);
            if (Input.GetKey(KeyCode.Backslash) && DisplayPanel.label != null)
            {
                DisplayPanel.AnchorToCursor();
            }
            DisplayPanel.Update();
        }

        private void RoomCamera_ctor(On.RoomCamera.orig_ctor orig, RoomCamera self, RainWorldGame game, int cameraNumber)
        {
            orig(self, game, cameraNumber);
            DisplayPanel.Initialize();
        }

        public static KeyCode[] debugKeys = new KeyCode[] { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8, KeyCode.Alpha9, KeyCode.Alpha0 };

        private void Player_checkInput(On.Player.orig_checkInput orig, Player self)
        {
            orig(self);
            if (self.AI != null || !self.Consious) return;

            for (int i = 0; i < debugKeys.Length; i++)
            {
                if (Input.GetKey(debugKeys[i]))
                {
                    MacroLibrary.activeMacro = MacroLibrary.SelectOnCurrentPage(i);
                    MacroLibrary.activeMacro.Initialize(self);
                    Log($"Macro started: {MacroLibrary.activeMacro.name}");
                    return;
                }
            }
            if (MacroLibrary.activeMacro?.GetPackage(self) is Player.InputPackage package)
            {
                if (MacroLibrary.activeMacro.terminated) { Log("Macro terminated - ran too long without ticking"); MacroLibrary.activeMacro = null; return; }
                self.input[0] = package.WithDownDiagonals();
            }
        }

        private KeyCode keyDown;

        private void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame self)
        {
            orig(self);
            try
            {
                if (Input.GetKey(KeyCode.F2))
                {
                    if (keyDown == KeyCode.F2) return;
                    Log($"Macro terminated manually");
                    MacroLibrary.activeMacro = null;
                    keyDown = KeyCode.F2;
                    return;
                }
                if (Input.GetKey(KeyCode.F5))
                {
                    if (keyDown == KeyCode.F5) return;
                    MacroLibrary.LoadCookbook(defaultCookbookFilepath);
                    keyDown = KeyCode.F5;
                    return;
                }
                if (Input.GetKey(KeyCode.F1))
                {
                    if (keyDown == KeyCode.F1) return;
                    keyDown = KeyCode.F1;
                    MacroLibrary.ChangePage(-1);
                    return;
                }
                if (Input.GetKey(KeyCode.F3))
                {
                    if (keyDown == KeyCode.F3) return;
                    keyDown = KeyCode.F3;
                    MacroLibrary.ChangePage(1);
                    return;
                }
                keyDown = default;
            }
            catch (Exception e) { Log(e); }
        }
    }
}
