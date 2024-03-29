﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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

        public static void ChangePage(int delta)
        {
            currentContainer.ViewedPage += delta;
            DisplayPanel.UpdateSelectMenu();
        }

        public static void SelectOnPage(int offset, RainWorldGame game)
        {
            if (currentContainer == null) return;
            if (currentContainer.IsCookbook)
            {
                if (currentContainer.SelectMacroOnViewedPage(offset) is Macro macro 
                    && game.Players.Count > 0 && game.Players[0]?.realizedCreature is Player player)
                {
                    macro.Initialize(player);
                    stack.Push(macro);
                    instructionsWithoutTick = 0;
                }
            }
            else
            {
                currentContainer = currentContainer.SelectContainerOnViewedPage(offset) ?? currentContainer;
                ChangePage(0);
            }
        }

        public static void UpOne()
        {
            currentContainer = currentContainer.parent ?? currentContainer;
            ChangePage(0);
        }

        public static void ReloadFromTopLevel()
        {
            topContainer = new MacroContainer(Const.COOKBOOK_ROOT_PATH, null);
            currentContainer = topContainer;
            stack.Clear();
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
            if (nowRecording)
            {
                recorded[0].Add(self.input[0]);
            }
            if (instructionsWithoutTick > Const.MAXIMUM_INSTRUCTIONS_WITHOUT_TICK)
            {
                Mod.Log($"WARNING: {Const.MAXIMUM_INSTRUCTIONS_WITHOUT_TICK} instructions ran without ticking!");
                TerminateMacro();
                return;
            }
            if (activeMacro is Macro macro)
            {
                if (macro.GetPackage(self) is Player.InputPackage package)
                {
                    if (Const.SUPER_DEBUG_MODE) Mod.Log($"Received {package.AsString()}");
                    self.input[0] = package.WithDownDiagonals();
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
                    DisplayPanel.TrackMe(activeMacro);
                    Update(self);
                }
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

        public static void PushNewMacro(string path, Player player)
        {
            stack.Push(GetMacroByAbsolutePath(path));
            activeMacro.Initialize(player);
        }
        private static bool nowRecording;
        private static List<List<Player.InputPackage>> recorded;
        public static void ToggleRecording()
        {
            if (!nowRecording)
            {
                nowRecording = true;
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
                    File.AppendAllText(Const.COOKBOOK_RECORDED_FILE, $"{Macro.RepFromInputList(recorded)}\n");
                }
            }
        }
    }
}
