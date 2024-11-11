using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BepInEx;
using UnityEngine;
using System.Resources;
using BepInEx.Logging;

namespace alphappy.TAMacro
{
    [BepInDependency("slime-cubed.devconsole", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(Const.PLUGIN_GUID, Const.PLUGIN_NAME, Const.PLUGIN_VERSION)]
    public class Mod : BaseUnityPlugin
    {
        private static bool initialized = false;

        private static ManualLogSource logger;

        public static void Log(object obj) { logger.LogDebug(obj); }
        public static void Log(Exception exc) { logger.LogError(exc); }
        public static void LogDebug(object obj) { if (Const.SUPER_DEBUG_MODE) Log(obj); }

        public void OnEnable()
        {
            logger = Logger;
            On.RainWorld.OnModsInit += RainWorldOnModsInitHook;
            Serialization.Populate();
        }

        private void RainWorldOnModsInitHook(On.RainWorld.orig_OnModsInit orig, RainWorld self)
        {
            orig(self);
            if (initialized) return;
            try
            {
                On.Player.checkInput += Player_checkInput;
                On.Player.Update += Player_Update;
                On.RainWorldGame.Update += RainWorldGame_Update;
                On.RoomCamera.ClearAllSprites += RoomCamera_ClearAllSprites;
                On.RainWorldGame.GrafUpdate += RainWorldGame_GrafUpdate;
                On.RainWorld.PostModsInit += RainWorld_PostModsInit;
                On.RainWorldGame.ShutDownProcess += RainWorldGame_ShutDownProcess;
                On.RainWorldGame.ctor += RainWorldGame_ctor;
                Log("Hooking complete");
                initialized = true;
                MachineConnector.SetRegisteredOI(Const.PLUGIN_GUID, Settings.instance);

                Log($"Ensuring main cookbook ready:  {Const.COOKBOOK_MAIN_FILE}");
                if (!Directory.Exists(Const.COOKBOOK_ROOT_PATH))
                {
                    Directory.CreateDirectory(Const.COOKBOOK_ROOT_PATH);
                    File.WriteAllText(Const.COOKBOOK_MAIN_FILE, "");
                }

            }
            catch (Exception e) { Log(e); }
        }

        private void Player_Update(On.Player.orig_Update orig, Player self, bool eu)
        {
            orig(self, eu);
            try
            {
                MacroLibrary.UpdateDisplacement(self);
            }
            catch (Exception e) { Log(e); }
        }

        private void RainWorldGame_ctor(On.RainWorldGame.orig_ctor orig, RainWorldGame self, ProcessManager manager)
        {
            orig(self, manager);
            try
            {
                PanelManager.Initialize(self);
                MacroLibrary.ReloadFromTopLevel();
            }
            catch (Exception e) { Log(e); }
        }

        private void RainWorldGame_ShutDownProcess(On.RainWorldGame.orig_ShutDownProcess orig, RainWorldGame self)
        {
            orig(self);
            try
            {
                if (MacroLibrary.activeMacro != null) MacroLibrary.TerminateMacro();
            }
            catch (Exception e) { Log(e); }
        }

        private void RainWorld_PostModsInit(On.RainWorld.orig_PostModsInit orig, RainWorld self)
        {
            orig(self);
            Const.WARP_MENU_ENABLED = ModManager.ActiveMods.Any(mod => mod.name == "Warp Menu" && mod.id == "warp");
            Log($"Found Warp Menu: {Const.WARP_MENU_ENABLED}");
            Const.DEVCONSOLAS_AVAILABLE = Futile.atlasManager.DoesContainFontWithName("devconsolas");
            Log($"Found devconsolas: {Const.DEVCONSOLAS_AVAILABLE}");
        }

        private void RoomCamera_ClearAllSprites(On.RoomCamera.orig_ClearAllSprites orig, RoomCamera self)
        {
            PanelManager.Shutdown();
            orig(self);
        }

        private static void RainWorldGame_GrafUpdate(On.RainWorldGame.orig_GrafUpdate orig, RainWorldGame self, float timeStacker)
        {
            orig(self, timeStacker);
            PanelManager.Frame();
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

                if (Input.GetKey(Settings.kbInterrupt.Value))
                {
                    keyDown = Settings.kbInterrupt.Value;
                    MacroLibrary.TerminateMacro(); Log("User requested macro termination");
                    return;
                }
                if (Input.GetKey(Settings.kbReloadLibrary.Value))
                {
                    keyDown = Settings.kbReloadLibrary.Value;
                    MacroLibrary.ReloadFromTopLevel();
                    return;
                }
                if (Input.GetKey(Settings.kbPrevPage.Value))
                {
                    keyDown = Settings.kbPrevPage.Value;
                    MacroLibrary.ChangePage(-1);
                    return;
                }
                if (Input.GetKey(Settings.kbNextPage.Value))
                {
                    keyDown = Settings.kbNextPage.Value;
                    MacroLibrary.ChangePage(1);
                    return;
                }
                if (Input.GetKey(Settings.kbUpOne.Value))
                {
                    keyDown = Settings.kbUpOne.Value;
                    MacroLibrary.UpOne();
                    return;
                }
                if (Input.GetKey(Settings.kbToggleRecording.Value))
                {
                    keyDown = Settings.kbToggleRecording.Value;
                    MacroLibrary.ToggleRecording();
                    return;
                }
                if (Input.GetKey(Settings.kbToggleVisible.Value))
                {
                    keyDown = Settings.kbToggleVisible.Value;
                    PanelManager.ToggleVisible();
                    return;
                }

                for (int i = 0; i < Settings.allSelectorKeys.Length; i++)
                {
                    if (Input.GetKey(Settings.allSelectorKeys[i].Value))
                    {
                        MacroLibrary.SelectOnPage(i, self);
                        keyDown = Settings.allSelectorKeys[i].Value;
                        return;
                    }
                }

                foreach (var pair in MacroLibrary.globalHotkeys)
                {
                    if (Input.GetKey(pair.Key))
                    {
                        MacroLibrary.StartMacro(pair.Value, self);
                        keyDown = pair.Key;
                        return;
                    }
                }

                keyDown = KeyCode.None;
            }
            catch (Exception e) { Log(e); }
        }
    }
}
