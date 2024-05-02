using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Security.Policy;
using IL.MoreSlugcats;
using System.ComponentModel;
using static alphappy.TAMacro.Pane;
using static alphappy.TAMacro.PaneButton;

namespace alphappy.TAMacro
{
    internal class DisplayPanel
    {
        public static FLabel label;
        public static FLabel labelInfo;
        public static FSprite pointer;
        public static FSprite box1;
        public static Macro macro;
        public static StringBuilder loadedMacros = new StringBuilder();
        public static float lineHeight;
        public static FContainer container;
        public static string Font => Const.DEVCONSOLAS_AVAILABLE && Settings.useDevconsolas.Value ? "devconsolas" : RWCustom.Custom.GetFont();
        public static string Header => $"TAMacro v{Const.PLUGIN_VERSION}\n\n{HeaderControls}";
        public static string HeaderControls => Settings.showControls.Value ? $"[{Settings.kbPrevPage.Value}]  Previous page\n[{Settings.kbInterrupt.Value}]  Interrupt\n[{Settings.kbNextPage.Value}]  Next page\n[{Settings.kbUpOne.Value}]  Up one level\n[{Settings.kbReloadLibrary.Value}]  Reload\n[{Settings.kbToggleRecording.Value}]  Start/stop recording\n[{Settings.kbMoveDisplayPanelToCursor.Value}]  Move display panel\n\n" : "";
        public static void Initialize()
        {
            container = new FContainer();
            Futile.stage.AddChild(container);
            container.SetPosition(Vector2.zero - Vector2.one);

            label = new FLabel(Font, "");
            label.isVisible = true;
            label.alpha = 0.5f;
            label.color = new Color(0.75f, 0.55f, 0.87f);
            label.alignment = FLabelAlignment.Left;
            container.AddChild(label);
            lineHeight = label.FontLineHeight * label.scale;

            labelInfo = new FLabel(Font, Header);
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
            loadedMacros.Append(Header);
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

    public class PaneManager
    {
        public Pane infoPane;
        public PaneManager()
        {
            infoPane = new Pane();
            infoPane.InitiateSprites();
            infoPane.AddButton("<-", 30f, 20f, () => { MacroLibrary.ChangePage(-1); });
        }
        public void Update()
        {
            infoPane.Update();
        }
        public void Remove()
        {
            infoPane.Remove();
            infoPane = null;
        }
    }

    public class Pane
    {
        public FContainer container;
        public FSprite backdrop;
        public FSprite titlebarbg;
        public List<PaneButton> buttons;
        public Vector2 dragRelPos;
        public Vector2 anchor;
        public DragState dragState = DragState.Neutral;
        public enum DragState { Neutral, ClickedOutside, HoveringInside, ClickedInside }

        public void InitiateSprites()
        {
            container = new FContainer();
            Futile.stage.AddChild(container);
            anchor = Vector2.zero - Vector2.one;
            container.SetPosition(anchor);

            backdrop = new FSprite("pixel")
            {
                alpha = 0.4f, color = Color.black, x = 500f, y = 500f, width = 200f, height = 600f
            };

            titlebarbg = new FSprite("pixel")
            {
                alpha = 0.0f, color = Color.white, x = 500f, y = 540f, width = 200f, height = 20f
            };

            container.AddChild(backdrop);
            container.AddChild(titlebarbg);
        }
        public void Update()
        {
            DragUpdate();
            foreach (PaneButton button in buttons) { button.Update(); }
        }
        public void DragUpdate()
        {
            bool nowClicking = Input.GetMouseButton(0);
            Vector2 mousePos = Input.mousePosition;
            bool nowHovering = titlebarbg.AmIBeingHovered();

            switch (dragState)
            {
                case DragState.ClickedInside:
                    if (nowClicking) { anchor = mousePos - dragRelPos; container.SetPosition(anchor); }
                    else { dragState = DragState.HoveringInside; }
                    break;

                case DragState.HoveringInside:
                    if (!nowHovering) { dragState = DragState.Neutral; }
                    else if (nowClicking) { dragState = DragState.ClickedInside; dragRelPos = mousePos - anchor; }
                    break;

                case DragState.ClickedOutside:
                    if (!nowClicking) { dragState = DragState.Neutral; }
                    break;

                case DragState.Neutral:
                    if (nowHovering) { dragState = DragState.HoveringInside; }
                    else if (nowClicking) { dragState = DragState.ClickedOutside; }
                    break;
            }

            titlebarbg.alpha = dragState == DragState.HoveringInside ? 0.6f : 0f;
        }
        public void Remove()
        {
            container.RemoveFromContainer();
            container.RemoveAllChildren();
            container = null;
        }

        private float potentialX;
        public void AddButton(string text, float width, float height, OnClickHandler onClick)
        {
            buttons.Add(new PaneButton(container, text, width, height, onClick));
        }
    }

    public class PaneButton
    {
        public FSprite bgsprite;
        public FLabel label;
        public delegate void OnClickHandler();
        public OnClickHandler onClick;
        public enum ClickState { Neutral, ClickedOutside, HoveringInside, ClickedInside, DraggedOutside }
        public ClickState clickState = ClickState.Neutral;

        public PaneButton(FContainer container, string text, float width, float height, OnClickHandler onClick)
        {
            bgsprite = new FSprite("pixel")
            {
                alpha = 0.4f,
                color = Const.BUTTON_COLOR,
                width = width,
                height = height
            };
            label = new FLabel(DisplayPanel.Font, text)
            {
                alpha = 0.6f,
                color = Color.white
            };
            container.AddChild(label);
            container.AddChild(bgsprite);
            this.onClick = onClick;
        }
        public void Remove()
        {
            label.RemoveFromContainer();
            bgsprite.RemoveFromContainer();
        }
        public void Update()
        {
            ClickUpdate();
        }
        public void ClickUpdate()
        {
            bool nowClicking = Input.GetMouseButton(0);
            bool nowHovering = bgsprite.AmIBeingHovered();

            switch (clickState)
            {
                case ClickState.ClickedOutside:
                    if (!nowClicking) { clickState = ClickState.Neutral; }
                    break;

                case ClickState.Neutral:
                    if (nowHovering) { clickState = ClickState.HoveringInside; }
                    else if (nowClicking) { clickState = ClickState.ClickedOutside; }
                    break;

                case ClickState.HoveringInside:
                    if (!nowHovering) { clickState = ClickState.Neutral; }
                    else if (nowClicking) { clickState = ClickState.ClickedInside; }
                    break;

                case ClickState.ClickedInside:
                    if (!nowHovering) { clickState = ClickState.DraggedOutside; }
                    else if (!nowClicking) { onClick.Invoke(); clickState = ClickState.HoveringInside; }
                    break;

                case ClickState.DraggedOutside:
                    if (nowHovering) { clickState = ClickState.ClickedInside; }
                    else if (!nowClicking) { clickState = ClickState.Neutral; }
                    break;
            }

            bgsprite.alpha = (clickState == ClickState.HoveringInside || clickState == ClickState.DraggedOutside) ? 0.8f : 0.4f;
        }
    }
}
