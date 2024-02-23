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
            if (currentContainer.IsCookbook)
            {
                activeMacro = currentContainer.SelectMacroOnViewedPage(offset);
                activeMacro.Initialize((Player)game.Players[0].realizedCreature);
            }
            else
            {
                currentContainer = currentContainer.SelectContainerOnViewedPage(offset);
            }
        }

        public static void UpOne() => currentContainer = currentContainer.parent ?? currentContainer;

        public static void ReloadFromTopLevel()
        {
            topContainer = new MacroContainer(Const.COOKBOOK_ROOT_PATH, null);
            currentContainer = topContainer;
            activeMacro = null;
        }

        public static void TerminateMacro()
        {
            Mod.Log("Macro terminated manually!");
            activeMacro = null;
        }
    }
}
