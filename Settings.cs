﻿using System;
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
        public static Configurable<KeyCode> kbInterrupt = instance.config.Bind("kbInterrupt", KeyCode.F2, new ConfigurableInfo("Interrupt macro"));
        public static Configurable<KeyCode> kbNextPage = instance.config.Bind("kbNextPage", KeyCode.F3, new ConfigurableInfo("Next page"));
        public static Configurable<KeyCode> kbUpOne = instance.config.Bind("kbUpOne", KeyCode.F4, new ConfigurableInfo("Up one level"));
        public static Configurable<KeyCode> kbReloadLibrary = instance.config.Bind("kbReloadLibrary", KeyCode.F5, new ConfigurableInfo("Reload library"));
        public static Configurable<KeyCode> kbToggleRecording = instance.config.Bind("kbToggleRecording", KeyCode.F7, new ConfigurableInfo("Toggle recording"));

        public static Configurable<KeyCode>[] allKeyCodes = { kbPrevPage, kbInterrupt, kbNextPage, kbUpOne, kbReloadLibrary, kbToggleRecording };

        public static Configurable<bool> useDevconsolas = instance.config.Bind("useDevconsolas", true, new ConfigurableInfo("Use devconsolas font instead of default\n(requires Dev Console to be enabled)"));
        public static Configurable<bool> showFullPath = instance.config.Bind("showFullPath", false, new ConfigurableInfo("Show full macro paths"));
        public static Configurable<bool> blinkRecordingButton = instance.config.Bind("blinkRecordingButton", true, new ConfigurableInfo("Blink recording button while recording"));
        public static Configurable<bool> recordScugState = instance.config.Bind("recordScugState", true, new ConfigurableInfo("Save scug state when recording"));
        public static Configurable<bool> discardFinalNeutral = instance.config.Bind("discardFinalNeutral", true, new ConfigurableInfo("Discard final input of recording if neutral"));
        public static Configurable<bool> allowMacroGlobalHotkeys = instance.config.Bind("allowMacroGlobalHotkeys", true, new ConfigurableInfo("Allow macros to set global hotkeys"));

        public override void Initialize()
        {
            base.Initialize();
            Tabs = new OpTab[] { new OpTab(this, "Bindings"), new OpTab(this, "Toggles") };
            Vector2 pos = new(20f, 550f);
            List<UIelement> list = new();

            foreach (var c in new List<Configurable<KeyCode>> { kbPrevPage, kbInterrupt, kbNextPage, kbUpOne, kbReloadLibrary, kbToggleRecording })
            {
                list.Add(new OpKeyBinder(c, pos, new(150f, 35f)));
                list.Add(new OpLabel(pos + new Vector2(160f, 3f), new(150f, 35f), c.info.description, FLabelAlignment.Left));
                pos += new Vector2(0f, -45f);
            }

            Tabs[0].AddItems(list.ToArray());
            list.Clear();
            pos = new(50f, 550f);

            foreach (var c in new List<Configurable<bool>> { useDevconsolas, showFullPath, blinkRecordingButton, recordScugState, discardFinalNeutral, allowMacroGlobalHotkeys })
            {
                list.Add(new OpCheckBox(c, pos));
                list.Add(new OpLabel(pos.x + 35f, pos.y + 3f, c.info.description));
                pos += new Vector2(0f, -35f);
            }

            Tabs[1].AddItems(list.ToArray());
        }
    }
}
