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
        public static Configurable<KeyCode> kbInterrupt = instance.config.Bind("kbInterrupt", KeyCode.F2, new ConfigurableInfo("Interrupt macro"));
        public static Configurable<KeyCode> kbNextPage = instance.config.Bind("kbNextPage", KeyCode.F3, new ConfigurableInfo("Next page"));
        public static Configurable<KeyCode> kbUpOne = instance.config.Bind("kbUpOne", KeyCode.F4, new ConfigurableInfo("Up one level"));
        public static Configurable<KeyCode> kbReloadLibrary = instance.config.Bind("kbReloadLibrary", KeyCode.F5, new ConfigurableInfo("Reload library"));
        public static Configurable<KeyCode> kbToggleRecording = instance.config.Bind("kbToggleRecording", KeyCode.F7, new ConfigurableInfo("Toggle recording"));

        public static Configurable<KeyCode> kbSelect0 = instance.config.Bind("kbSelect0", KeyCode.Alpha1, new ConfigurableInfo("Select #1"));
        public static Configurable<KeyCode> kbSelect1 = instance.config.Bind("kbSelect1", KeyCode.Alpha2, new ConfigurableInfo("Select #2"));
        public static Configurable<KeyCode> kbSelect2 = instance.config.Bind("kbSelect2", KeyCode.Alpha3, new ConfigurableInfo("Select #3"));
        public static Configurable<KeyCode> kbSelect3 = instance.config.Bind("kbSelect3", KeyCode.Alpha4, new ConfigurableInfo("Select #4"));
        public static Configurable<KeyCode> kbSelect4 = instance.config.Bind("kbSelect4", KeyCode.Alpha5, new ConfigurableInfo("Select #5"));
        public static Configurable<KeyCode> kbSelect5 = instance.config.Bind("kbSelect5", KeyCode.Alpha6, new ConfigurableInfo("Select #6"));
        public static Configurable<KeyCode> kbSelect6 = instance.config.Bind("kbSelect6", KeyCode.Alpha7, new ConfigurableInfo("Select #7"));
        public static Configurable<KeyCode> kbSelect7 = instance.config.Bind("kbSelect7", KeyCode.Alpha8, new ConfigurableInfo("Select #8"));
        public static Configurable<KeyCode> kbSelect8 = instance.config.Bind("kbSelect8", KeyCode.Alpha9, new ConfigurableInfo("Select #9"));
        public static Configurable<KeyCode> kbSelect9 = instance.config.Bind("kbSelect9", KeyCode.Alpha0, new ConfigurableInfo("Select #10"));

        public static Configurable<KeyCode>[] allSelectorKeys = { kbSelect0, kbSelect1, kbSelect2, kbSelect3, kbSelect4, kbSelect5, kbSelect6, kbSelect7, kbSelect8, kbSelect9 };
        public static Configurable<KeyCode>[] allNonselectorKeys = { kbPrevPage, kbInterrupt, kbNextPage, kbUpOne, kbReloadLibrary, kbToggleRecording };
        public static Configurable<KeyCode>[] AllKeys => allSelectorKeys.Concat(allNonselectorKeys).ToArray();

        public static Configurable<bool> useDevconsolas = instance.config.Bind("useDevconsolas", true, new ConfigurableInfo("Use devconsolas font instead of default\n(requires Dev Console to be enabled)"));
        public static Configurable<bool> showFullPath = instance.config.Bind("showFullPath", false, new ConfigurableInfo("Show full macro paths"));
        public static Configurable<bool> blinkRecordingButton = instance.config.Bind("blinkRecordingButton", true, new ConfigurableInfo("Blink recording button while recording"));
        public static Configurable<bool> recordScugState = instance.config.Bind("recordScugState", true, new ConfigurableInfo("Save scug state when recording"));
        public static Configurable<bool> discardFinalNeutral = instance.config.Bind("discardFinalNeutral", true, new ConfigurableInfo("Discard final input of recording if neutral"));
        public static Configurable<bool> allowMacroGlobalHotkeys = instance.config.Bind("allowMacroGlobalHotkeys", true, new ConfigurableInfo("Allow macros to set global hotkeys"));
        public static Configurable<bool> autoAddDisplacementRefPoint = instance.config.Bind("autoAddDisplacementRefPoint", true, new ConfigurableInfo("Automatically set refpoint at start of every macro"));

        public override void Initialize()
        {
            base.Initialize();
            Tabs = new OpTab[] { new OpTab(this, "Bindings"), new OpTab(this, "Toggles") };
            Vector2 pos = new(20f, 550f);
            List<UIelement> list = new();

            foreach (var c in allNonselectorKeys)
            {
                list.Add(new OpKeyBinder(c, pos, new(150f, 35f)));
                list.Add(new OpLabel(pos + new Vector2(160f, 3f), new(150f, 35f), c.info.description, FLabelAlignment.Left));
                pos += new Vector2(0f, -45f);
            }

            list.Add(new OpLabel(new(320f, 550f), new(150f, 35f), "Select from current folder:", FLabelAlignment.Left));
            pos = new(320f, 520f);
            foreach (var c in allSelectorKeys)
            {
                list.Add(new OpKeyBinder(c, pos, new(100f, 35f)));
                list.Add(new OpLabel(pos + new Vector2(110f, 3f), new(150f, 35f), c.info.description, FLabelAlignment.Left));
                pos += new Vector2(0f, -45f);
            }

            Tabs[0].AddItems(list.ToArray());
            list.Clear();
            pos = new(50f, 550f);

            foreach (var c in new List<Configurable<bool>> { useDevconsolas, showFullPath, blinkRecordingButton, recordScugState, discardFinalNeutral, allowMacroGlobalHotkeys, autoAddDisplacementRefPoint })
            {
                list.Add(new OpCheckBox(c, pos));
                list.Add(new OpLabel(pos.x + 35f, pos.y + 3f, c.info.description));
                pos += new Vector2(0f, -35f);
            }

            Tabs[1].AddItems(list.ToArray());
        }
    }
}
