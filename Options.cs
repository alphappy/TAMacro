using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Menu.Remix.MixedUI;
using UnityEngine;

namespace alphappy.TAMacro
{
    internal class Settings : OptionInterface
    {
        public static Settings instance = new Settings();
        public static Configurable<KeyCode> keybindLoadMainCookbook = instance.config.Bind("keybindLoadMainCookbook", KeyCode.F1);
        public override void Initialize()
        {
            base.Initialize();
            Tabs = new OpTab[] { new OpTab(this, "Settings") };
            Tabs[0].AddItems();
        }
    }
}
