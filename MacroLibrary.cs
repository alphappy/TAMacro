using System;
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
            Mod.Log("Macro terminated by user!");
            stack.Clear();
        }

        public static void Update(Player self)
        {
            if (self.AI != null || !self.Consious) return;
            if (activeMacro is Macro macro)
            {
                if (macro.GetPackage(self) is Player.InputPackage package)
                {
                    if (macro.terminated)
                    {
                        Mod.Log("WARNING: Terminating macro which ran too long without ticking!");
                        stack.Clear();
                        return;
                    }
                    if (Const.SUPER_DEBUG_MODE) Mod.Log($"Received {package.AsString()}");
                    self.input[0] = package.WithDownDiagonals();
                }
                else
                {
                    Mod.Log("Macro finished");
                    stack.Pop();
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
    }
}
