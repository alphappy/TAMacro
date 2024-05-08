using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace alphappy.TAMacro
{
    internal class PanelReactions
    {
        public static void Simple(Panel panel, Panel.MouseEventType type, Vector2 _)
        {
            FSprite backdrop = (FSprite)panel["backdrop"];
            switch (type)
            {
                case Panel.MouseEventType.StartHover:
                    backdrop.alpha += 0.4f;
                    break;
                case Panel.MouseEventType.StopHover:
                    backdrop.alpha -= 0.4f;
                    break;
                case Panel.MouseEventType.StartClick:
                    backdrop.color = Color.gray;
                    break;
                case Panel.MouseEventType.StopClick:
                    backdrop.color = Color.black;
                    break;
            }
        }

        public static void DragParent(Panel panel, Panel.MouseEventType type, Vector2 dragVector)
        {
            if (type == Panel.MouseEventType.Drag)
            {
                panel.container.x += dragVector.x;
                panel.container.y += dragVector.y;
            }
            else if (type == Panel.MouseEventType.StopClick)
            {
                var bounds = panel.BoundsAbsolute;
                var screen = RWCustom.Custom.rainWorld.options.ScreenSize;

                if (bounds.xMin < 0) panel.container.x += -bounds.xMin;
                if (bounds.xMax > screen.x) panel.container.x -= bounds.xMax - screen.x;
                if (bounds.yMin < 0) panel.container.y += -bounds.yMin;
                if (bounds.yMax > screen.y) panel.container.y -= bounds.yMax - screen.y;
            }
        }

        public static void ResizeParent(Panel panel, Panel.MouseEventType type, Vector2 dragVector)
        {
            if (type != Panel.MouseEventType.Drag) return;
            panel.container.x += dragVector.x;
            panel.container.y += dragVector.y;
        }

        public static Panel.MouseEvent DisplayTooltip(FNode tooltip)
        {
            return (Panel panel, Panel.MouseEventType type, Vector2 dragVector) =>
            {
                switch (type)
                {
                    case Panel.MouseEventType.StartHover: tooltip.isVisible = true; break;
                    case Panel.MouseEventType.StopHover: tooltip.isVisible = false; break;
                }
            };
        }

        public static Panel.MouseEvent WhenClicked(Action action)
        {
            return (Panel panel, Panel.MouseEventType type, Vector2 dragVector) =>
            {
                if (type == Panel.MouseEventType.Fire) { action(); }
            };
        }
    }
}
