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
    [BepInDependency("slime-cubed.devconsole", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(Const.PLUGIN_GUID, Const.PLUGIN_NAME, Const.PLUGIN_VERSION)]
    public class Mod : BaseUnityPlugin
    {
        private static bool initialized = false;

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

                Log($"Ensuring main cookbook ready:  {Const.COOKBOOK_MAIN_FILE}");
                if (!Directory.Exists(Const.COOKBOOK_ROOT_PATH))
                {
                    Directory.CreateDirectory(Const.COOKBOOK_ROOT_PATH);
                    File.WriteAllText(Const.COOKBOOK_MAIN_FILE, "");
                }

                if (Futile.atlasManager.DoesContainFontWithName("devconsolas")) { Const.FONT_NAME = "devconsolas"; }
            }
            catch (Exception e) { Log(e); }
        }

        private void RoomCamera_ClearAllSprites(On.RoomCamera.orig_ClearAllSprites orig, RoomCamera self)
        {
            DisplayPanel.Remove();
            //paneManager.Remove();
            orig(self);
        }

        public static PaneManager paneManager;
        private static void RainWorldGame_GrafUpdate(On.RainWorldGame.orig_GrafUpdate orig, RainWorldGame self, float timeStacker)
        {
            orig(self, timeStacker);
            if (Input.GetKey(KeyCode.Backslash) && DisplayPanel.label != null)
            {
                DisplayPanel.AnchorToCursor();
            }
            DisplayPanel.Update();
            //paneManager.Update();
        }

        private void RoomCamera_ctor(On.RoomCamera.orig_ctor orig, RoomCamera self, RainWorldGame game, int cameraNumber)
        {
            orig(self, game, cameraNumber);
            DisplayPanel.Initialize();
            //paneManager = new PaneManager();
        }

        

        private void Player_checkInput(On.Player.orig_checkInput orig, Player self)
        {
            orig(self);
            try
            {
                if (self.AI != null) return;
                if (self.playerState.playerNumber != 0) return;
                MacroLibrary.Update(self);
            }
            catch (Exception e) { Log(e); }
        }

        private KeyCode keyDown = KeyCode.None;

        private void RainWorldGame_Update(On.RainWorldGame.orig_Update orig, RainWorldGame self)
        {
            orig(self);
            try
            {
                if (keyDown != KeyCode.None && Input.GetKey(keyDown)) return;

                if (Input.GetKey(KeyCode.F2))
                {
                    keyDown = KeyCode.F2;
                    MacroLibrary.TerminateMacro(); Log("User requested macro termination");
                    return;
                }
                if (Input.GetKey(KeyCode.F5))
                {
                    keyDown = KeyCode.F5;
                    MacroLibrary.ReloadFromTopLevel();
                    return;
                }
                if (Input.GetKey(KeyCode.F1))
                {
                    keyDown = KeyCode.F1;
                    MacroLibrary.ChangePage(-1);
                    return;
                }
                if (Input.GetKey(KeyCode.F3))
                {
                    keyDown = KeyCode.F3;
                    MacroLibrary.ChangePage(1);
                    return;
                }
                if (Input.GetKey(KeyCode.F4))
                {
                    keyDown = KeyCode.F4;
                    MacroLibrary.UpOne();
                    return;
                }
                if (Input.GetKey(KeyCode.F7))
                {
                    keyDown = KeyCode.F7;
                    MacroLibrary.ToggleRecording();
                    return;
                }

                for (int i = 0; i < Const.SELECT_KEYS.Length; i++)
                {
                    if (Input.GetKey(Const.SELECT_KEYS[i]))
                    {
                        MacroLibrary.SelectOnPage(i, self);
                        keyDown = Const.SELECT_KEYS[i];
                        return;
                    }
                }

                keyDown = KeyCode.None;
            }
            catch (Exception e) { Log(e); }
        }
    }
}
