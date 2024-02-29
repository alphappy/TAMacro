using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace alphappy.TAMacro
{
    internal class DisplayPanel
    {
        public static FLabel label;
        public static FLabel labelInfo;
        public static FSprite pointer;
        public static FSprite box1;
        public static Macro macro;
        public static string header = $"TAMacro v{Const.PLUGIN_VERSION}\n\n[F1]  Previous page\n[F2]  Interrupt\n[F3]  Next page\n[F4]  Up one level\n[F5]  Reload\n[F7]  Start/stop recording\n[\\]  Move display panel\n\n";
        public static StringBuilder loadedMacros = new StringBuilder();
        public static float lineHeight;
        public static FContainer container;
        public static void Initialize()
        {
            container = new FContainer();
            Futile.stage.AddChild(container);
            container.SetPosition(Vector2.zero - Vector2.one);

            label = new FLabel(Const.FONT_NAME, "");
            label.isVisible = true;
            label.alpha = 0.5f;
            label.color = new Color(0.75f, 0.55f, 0.87f);
            label.alignment = FLabelAlignment.Left;
            container.AddChild(label);
            lineHeight = label.FontLineHeight * label.scale;

            labelInfo = new FLabel(Const.FONT_NAME, header);
            labelInfo.isVisible = true;
            labelInfo.alpha = 0.5f;
            labelInfo.color = new Color(0.55f, 0.55f, 0.87f);
            labelInfo.alignment = FLabelAlignment.Left;
            container.AddChild(labelInfo);

            UpdateLabelPosition();

            pointer = new FSprite("Futile_White")
            {
                width = 200,
                height = lineHeight,
                isVisible = false,
                alpha = 0.15f,
                color = new Color(0.7f, 0.7f, 0.1f),
            };
            container.AddChild(pointer);

            //box1 = new FSprite("Futile_White")
            //{
            //    width = 520,
            //    height = lineHeight,
            //    isVisible = false,
            //    alpha = 0.15f,
            //    color = new Color(0.7f, 0.7f, 0.1f),
            //};
            //container.AddChild(box1);
        }
        public static void Remove()
        {
            container.RemoveFromContainer();
            container.RemoveAllChildren();
            container = null;
        }
        public static void Update()
        {
            if (macro == null || macro.currentIndex >= macro.instructions.Count)
            {
                pointer.isVisible = false;
                return;
            }
            pointer.SetPosition(anchor + new Vector2(
                pointer.width / 2f, 
                LabelHeight - (lineHeight * (macro.currentLine + 2f))
                ));
            pointer.isVisible = true;
        }
        public static void TrackMe(Macro m)
        {
            if (m == null) return;
            macro = m;
            label.text = $"{macro.name}\n\n{macro.text}";
            UpdateLabelPosition();
        }
        public static void AnchorToCursor()
        {
            anchor = (Vector2)Input.mousePosition;
            anchor.x = Mathf.Floor(anchor.x) + 0.5f;
            anchor.y = Mathf.Floor(anchor.y) + 0.5f;
            UpdateLabelPosition();
        }
        public static void UpdateSelectMenu()
        {
            loadedMacros.Clear();
            loadedMacros.Append(header);
            loadedMacros.AppendLine($"{MacroLibrary.currentContainer?.name} ({MacroLibrary.currentContainer?.ViewedPage + 1} / {MacroLibrary.currentContainer?.PageCount})");
            if (MacroLibrary.currentContainer.IsCookbook)
            {
                for (int i = 0; i < 10; i++)
                {
                    Macro macro = MacroLibrary.currentContainer?.SelectMacroOnViewedPage(i);
                    loadedMacros.AppendLine($"[{(i + 1) % 10}]  {macro?.name ?? ""}");
                }
            }
            else
            {
                for (int i = 0; i < 10; i++)
                {
                    MacroContainer cont = MacroLibrary.currentContainer?.SelectContainerOnViewedPage(i);
                    loadedMacros.AppendLine($"[{(i + 1) % 10}]  {cont?.name ?? ""}");
                }
            }

            labelInfo.text = loadedMacros.ToString();
            UpdateLabelPosition();
        }
        public static void UpdateLabelPosition()
        { 
            label.SetPosition(anchor + new Vector2(0f, LabelHeight / 2f));
            labelInfo.SetPosition(anchor + new Vector2(-300f, LabelInfoHeight / 2f));
        }
        public static float LabelHeight => label.text.Split('\n').Length * lineHeight;  // (macro != null ? 2 + macro.lines : 1) * lineHeight;
        public static float LabelInfoHeight => labelInfo.text.Split('\n').Length * lineHeight;
        public static Vector2 anchor = new Vector2(700f, 100f);
    }
}
