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
        public static Settings instance = new();
        public static Configurable<KeyCode> kbPrevPage = instance.config.Bind("kbPrevPage", KeyCode.F1, new ConfigurableInfo("Previous page"));
        public static Configurable<KeyCode> kbInterrupt = instance.config.Bind("kbInterrupt", KeyCode.F2, new ConfigurableInfo("Interrupt Mmacro"));
        public static Configurable<KeyCode> kbNextPage = instance.config.Bind("kbNextPage", KeyCode.F3, new ConfigurableInfo("Next page"));
        public static Configurable<KeyCode> kbUpOne = instance.config.Bind("kbUpOne", KeyCode.F4, new ConfigurableInfo("Up one level"));
        public static Configurable<KeyCode> kbReloadLibrary = instance.config.Bind("kbReloadLibrary", KeyCode.F5, new ConfigurableInfo("Reload"));
        public static Configurable<KeyCode> kbToggleRecording = instance.config.Bind("kbToggleRecording", KeyCode.F7, new ConfigurableInfo("Record"));
        public static Configurable<KeyCode> kbMoveDisplayPanelToCursor = instance.config.Bind("kbMoveDisplayPanelToCursor", KeyCode.Backslash, new ConfigurableInfo("Move display panel"));
        public static Configurable<bool> showControls = instance.config.Bind("showControls", true, new ConfigurableInfo("Show controls on display panel"));
        public override void Initialize()
        {
            base.Initialize();
            Tabs = new OpTab[] { new OpTab(this, "Settings") };
            Vector2 pos = new(50f, 550f);
            List<UIelement> list = new();

            foreach (var c in new List<Configurable<KeyCode>> { kbPrevPage, kbInterrupt, kbNextPage, kbUpOne, kbReloadLibrary, kbToggleRecording, kbMoveDisplayPanelToCursor })
            {
                list.Add(new OpKeyBinder(c, pos, new(150f, 35f)));
                list.Add(new OpLabel(pos + new Vector2(160f, 3f), new(150f, 35f), c.info.description, FLabelAlignment.Left));
                pos += new Vector2(0f, -45f);
            }

            list.Add(new OpCheckBox(showControls, pos));
            list.Add(new OpLabel(pos.x + 35f, pos.y + 3f, showControls.info.description));
            //pos += new Vector2(0f, -45f);

            Tabs[0].AddItems(list.ToArray());
        }
    }
}
