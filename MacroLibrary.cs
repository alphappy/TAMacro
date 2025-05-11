using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace alphappy.TAMacro
{
    public class MacroLibrary
    {
        public static MacroContainer topContainer;
        public static MacroContainer currentContainer;
        public static Macro activeMacro => stack.Count > 0 ? stack.Peek() : null;
        private static Stack<Macro> stack = new Stack<Macro>();
        public static int macrosPerPage = 10;
        public static int instructionsWithoutTick = 0;

        public static event Action<MacroContainer> OnDirectoryChange;
        public static event Action<MacroContainer> OnPageChange;
        public static event Action<Macro> OnMacroTick;
        public static event Action<string> OnMacroException;

        public static Dictionary<KeyCode, Macro> globalHotkeys = new();

        public static Vector2? refPoint = null;

        public static void ClearEvents()
        {
            OnDirectoryChange = null; OnPageChange = null; OnMacroTick = null;
        }

        public static void ChangePage(int delta)
        {
            currentContainer.ViewedPage += delta;
            OnPageChange?.Invoke(currentContainer);
        }

        public static void SelectOnPage(int offset, RainWorldGame game)
        {
            if (currentContainer == null) return;
            if (currentContainer.IsCookbook)
            {
                if (currentContainer.SelectMacroOnViewedPage(offset) is Macro macro) StartMacro(macro, game);
            }
            else
            {
                currentContainer = currentContainer.SelectContainerOnViewedPage(offset) ?? currentContainer;
                OnDirectoryChange?.Invoke(currentContainer);
                ChangePage(0);
            }
        }

        public static event Action<Macro, RainWorldGame> OnMacroStart;
        public static void StartMacro(Macro macro, RainWorldGame game)
        {
            if (game.Players.Count > 0 && game.Players[0]?.realizedCreature is Player player)
            {
                macro.Initialize(player);
                stack.Push(macro);
                instructionsWithoutTick = 0;
                refPoint = Settings.autoAddDisplacementRefPoint.Value ? player.bodyChunks[1].pos : null;
                OnMacroStart?.Invoke(macro, game);
            }
        }

        public static void UpOne()
        {
            currentContainer = currentContainer.parent ?? currentContainer;
            OnDirectoryChange?.Invoke(currentContainer);
            ChangePage(0);
        }

        public static void ReloadFromTopLevel()
        {
            globalHotkeys.Clear();
            topContainer = new MacroContainer(Const.COOKBOOK_ROOT_PATH, null);
            currentContainer = topContainer;
            stack.Clear();
            OnDirectoryChange?.Invoke(currentContainer);
            ChangePage(0);
        }

        public static void TerminateMacro()
        {
            Mod.Log("Macro forcefully terminated!");
            instructionsWithoutTick = 0;
            stack.Clear();
        }

        public static void Update(Player self)
        {
            if (self.AI != null || !self.Consious) return;
            try
            {
                if (nowRecording)
                {
                    if (recordingStartRoom == null)
                    {
                        recordingStartRoom = self.room?.game.IsArenaSession == true ? "" : self.room?.abstractRoom.name;
                        recordingStartPosition = self.mainBodyChunk.pos;
                        if (Settings.recordScugState.Value) scugState = self.Serialize();
                    }
                    recorded[0].Add(self.input[0]);
                }
                if (instructionsWithoutTick > Const.MAXIMUM_INSTRUCTIONS_WITHOUT_TICK)
                {
                    TerminateMacro();
                    throw new Exceptions.MacroHangException($"{Const.MAXIMUM_INSTRUCTIONS_WITHOUT_TICK} instructions ran without ticking, probably because of an infinite loop.");
                }
                if (activeMacro is Macro macro)
                {
                    try
                    {
                        var playerInputNeutral = self.input[0].IsNeutral();
                        if (!playerInputNeutral && macro.options.interference == Macro.Options.Interference.Pause) return;
                        if (!playerInputNeutral && macro.options.interference == Macro.Options.Interference.Kill) { TerminateMacro(); return; }

                        if (macro.GetPackage(self) is Player.InputPackage package)
                        {
                            Mod.LogDebug($"Received {package.AsString()}");
                            if (playerInputNeutral || macro.options.interference != Macro.Options.Interference.Overwrite)
                            {
                                self.input[0] = package.WithDownDiagonals();
                                if (Settings.phantomInputInterference.Value) self.PhantomInterference();
                            }
                            instructionsWithoutTick = 0;
                        }
                        else if (activeMacro != macro)  // did the macro just call another
                        {
                            Mod.Log($"Macro {macro.name} called {activeMacro.name}");
                            Update(self);
                        }
                        else
                        {
                            Mod.Log($"Macro {macro.name} finished");
                            stack.Pop();
                            Update(self);
                        }
                        OnMacroTick?.Invoke(macro);
                    }
                    catch (Exception e)
                    {
                        var s = $"An exception occurred while running a macro.\n  Macro: {macro.FullName}\n  Line number: {macro.currentLine}\n  Line: {macro.currentLineText}\n  Instruction number: {macro.currentIndex}\n  Instruction: {macro.current}\n{e}";
                        Mod.Log(s);
                        Mod.Log(e);
                        OnMacroException?.Invoke(s);
                    }
                }

            }
            catch (Exception e)
            {
                var s = $"An exception occurred while running a macro*.\n  Macro: {activeMacro?.FullName}\n  Line number: {activeMacro?.currentLine}\n  Line: {activeMacro?.currentLineText}\n  Instruction number: {activeMacro?.currentIndex}\n  Instruction: {activeMacro?.current}\n{e}";
                Mod.Log(s);
                Mod.Log(e);
                OnMacroException?.Invoke(s);
            }
            
        }

        public static Macro GetMacroByAbsolutePath(string path)
        {
            MacroContainer container = topContainer;
            foreach (string identifier in path.Split('/'))
            {
                if (container.children.TryGetValue(identifier, out MacroContainer container2))
                {
                    container = container2;
                }
                else if (container.macros.TryGetValue(identifier, out Macro macro))
                {
                    return macro;
                }
                else
                {
                    throw new ArgumentException($"Couldn't find {identifier} while searching for {path}");
                }
            }
            throw new ArgumentException($"Could not find a macro by the name {path}");
        }

        public static Macro GetMacroByRelativePath(string path, Macro basis)
        {
            MacroContainer container = basis.parent;
            foreach (string identifier in path.Split('/'))
            {
                if (identifier == "")
                {
                    container = topContainer;
                }
                else if (identifier == "..")
                {
                    container = container.parent;
                    if (container == null) throw new Exceptions.InvalidExecuteTargetException($"`{path}` points to parent of root.");
                }
                else if (identifier == ".")
                {

                }
                else if (container.children.TryGetValue(identifier, out MacroContainer container2))
                {
                    container = container2;
                }
                else if (container.macros.TryGetValue(identifier, out Macro macro))
                {
                    return macro;
                }
                else
                {
                    throw new Exceptions.InvalidExecuteTargetException($"Could not find `{container.FullName}/{identifier}`  while searching for `{path}`.");
                }
            }
            throw new Exceptions.InvalidExecuteTargetException($"`{path}` points to a cookbook, not a macro.");
        }

        public static void PushNewMacro(string path, Player player)
        {
            stack.Push(GetMacroByRelativePath(path, activeMacro));
            activeMacro.Initialize(player);
        }
        internal static bool nowRecording;
        public static event Action<bool> OnToggleRecording;
        private static List<List<Player.InputPackage>> recorded;
        private static Vector2 recordingStartPosition;
        private static string recordingStartRoom;
        private static string scugState = null;
        public static void ToggleRecording()
        {
            if (!nowRecording)
            {
                nowRecording = true;
                recordingStartPosition = default; recordingStartRoom = null;
                recorded = new List<List<Player.InputPackage>> { new List<Player.InputPackage> { } };
            }
            else
            {
                nowRecording = false;
                if (recorded.Count > 0)
                {
                    if (!File.Exists(Const.COOKBOOK_RECORDED_FILE))
                    {
                        File.AppendAllText(Const.COOKBOOK_RECORDED_FILE, "//PARSER: 1\n//AUTHOR: [AUTOMATICALLY RECORDED]\n\n");
                    }
                    string setup = recordingStartRoom == string.Empty 
                        ? $"!warp {recordingStartPosition.x} {recordingStartPosition.y}\n" 
                        : $"!warp {recordingStartRoom} {recordingStartPosition.x} {recordingStartPosition.y}\n";
                    if (scugState != null) setup = $"{setup}!scugstate {scugState}\n";
                    File.AppendAllText(Const.COOKBOOK_RECORDED_FILE, $"{Macro.RepFromInputList(recorded, setup)}\n");
                }
            }
            OnToggleRecording?.Invoke(nowRecording);
        }

        public static event Action<Vector2> OnDisplacementUpdate;
        public static void UpdateDisplacement(Player player)
        {
            if (activeMacro is not null && refPoint is Vector2 v) OnDisplacementUpdate?.Invoke(player.bodyChunks[1].pos - v);
        }

        public static DateTime blockOpenUntil;
        public static void OpenFolder()
        {
            DateTime now = DateTime.Now;
            if (now < blockOpenUntil) return;
            blockOpenUntil = now + new TimeSpan(0, 0, 3);

            Mod.Log("Open: " + currentContainer.sysPath);
            Process.Start("explorer.exe", currentContainer.sysPath.Replace('/', '\\') + "\\");
        }
    }
}
