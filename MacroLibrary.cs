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
        public static Macro activeMacro;
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
                activeMacro = currentContainer.SelectMacroOnViewedPage(offset);
                if (activeMacro != null && game.Players.Count > 0 && game.Players[0]?.realizedCreature is Player player) 
                {
                    activeMacro.Initialize(player);
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
            activeMacro = null;
            ChangePage(0);
        }

        public static void TerminateMacro()
        {
            Mod.Log("Macro terminated manually!");
            activeMacro = null;
        }
    }
}
