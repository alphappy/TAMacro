using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace alphappy.TAMacro
{
    public class Panel : FContainer
    {
        public static int nextID = 0;
        public readonly int id;

        public List<Panel> children = new();
        public Dictionary<string, FNode> named = new();
        public FNode this[string s] => named[s];

        public Vector2 size;
        public Rect BoundsRelative => new(GetPosition(), size);
        public Rect BoundsAbsolute => BoundsRelative.Shifted((container as Panel)?.BoundsAbsolute.position ?? default);

        public bool? forceListenToMouse = null;
        public bool ListenToMouse => forceListenToMouse ?? isVisible;

        public void StartedHovering(Vector2 mousePos) => OnMouseEvent?.Invoke(this, MouseEventType.StartHover, mousePos);
        public void StoppedHovering(Vector2 mousePos) => OnMouseEvent?.Invoke(this, MouseEventType.StopHover, mousePos);
        public void StartedClicking(Vector2 mousePos) => OnMouseEvent?.Invoke(this, MouseEventType.StartClick, mousePos);
        public void StoppedClicking(Vector2 mousePos) 
        { 
            OnMouseEvent?.Invoke(this, MouseEventType.StopClick, mousePos); 
            if (BoundsAbsolute.Contains(mousePos)) OnMouseEvent?.Invoke(this, MouseEventType.Fire, mousePos);
        }
        public void Dragged(Vector2 dragVector) => OnMouseEvent?.Invoke(this, MouseEventType.Drag, dragVector);
        public enum MouseEventType { StartHover, StopHover, StartClick, StopClick, Drag, Fire };
        public delegate void MouseEvent(Panel panel, MouseEventType type, Vector2 mousePos);
        public event MouseEvent OnMouseEvent;

        public Panel(Rect bounds, FContainer parent)
        {
            id = nextID++;
            if (parent is Panel panel) panel.AddChild(this); else parent.AddChild(this);
            SetBounds(bounds);
        }
        public void SetBounds(Rect bounds)
        {
            size = bounds.size;
            SetPosition(bounds.position);
            Mod.LogDebug($"Panel {id} bounds set to {bounds}.  Absolute: {BoundsAbsolute}");
        }
        public void Destroy()
        {
            children.ForEach(e => e.Destroy());
            children = null;
            RemoveAllChildren();
            RemoveFromContainer();
            named.Clear();
        }
        public Panel WhoIsHovered(Vector2 cursor)
        {
            if (!ListenToMouse) return null;
            if (!BoundsAbsolute.Contains(cursor)) return null;
            foreach (var child in children)
            {
                var h = child.WhoIsHovered(cursor);
                if (h != null) return h;
            }
            return this;
        }
        public Panel CreateBackdrop() => CreateBackdrop(new(Vector2.zero, size), Color.black, 0.3f, out _);
        public Panel CreateBackdrop(Rect bounds, Color color, float alpha, out FSprite sprite)
        {
            sprite = new("Futile_White") { color = color, alpha = alpha };
            AddChild(sprite, "backdrop");
            sprite.SetBounds(bounds);
            return this;
        }
        public new void AddChild(FNode node)
        {
            base.AddChild(node);
            if (node is Panel panel) children.Add(panel);
        }
        public void AddChild(FNode node, string key)
        {
            AddChild(node);
            named[key] = node;
        }
        public Panel CreateLabel(string key, string text, Vector2 pos, out FLabel label)
        {
            label = new FLabel(Const.Font, text);
            AddChild(label, key);
            label.SetPosition(pos + new Vector2(0.05f, 0.05f));
            return this;
        }
        public Panel CreateLabelCentered(string key, string text, out FLabel label) => CreateLabel(key, text, size / 2, out label);
        public Panel CreateLabelCentered(string key, string text) => CreateLabel(key, text, size / 2, out _);
        public Panel CreateStaticTooltip(string text, out Panel panel)
        {
            panel = new Panel(default, this) { isVisible = false };
            panel.CreateLabel("text", text, new(size.x / 2, size.y + 12f), out var label);
            panel.CreateBackdrop(label.GetFixedWidthBounds().Resized(5f, 0f), Color.black, 1f, out var backdrop);
            label.MoveInFrontOfOtherNode(backdrop);
            OnMouseEvent += PanelReactions.DisplayTooltip(panel);
            return this;
        }
        public Panel CreateStaticTooltip(string text) => CreateStaticTooltip(text, out _);
        public Panel CreatePanel(Rect bounds, out Panel panel)
        {
            panel = new Panel(bounds, this);
            return this;
        }
        public Panel CreateAndGotoPanel(Rect bounds)
        {
            CreatePanel(bounds, out var panel);
            return panel;
        }
        public Panel CreateAndGotoPanel(Rect bounds, bool backdrop = true, string label = null, string tooltip = null, bool simpleReaction = false)
        {
            CreatePanel(bounds, out var panel);
            if (backdrop) panel.CreateBackdrop();
            if (label != null) panel.CreateLabelCentered("title", label);
            if (tooltip != null) panel.CreateStaticTooltip(tooltip);
            if (simpleReaction) panel.CreateMouseEvent(PanelReactions.Simple);
            return panel;
        }
        public Panel CreateMouseEvent(MouseEvent mEvent) { OnMouseEvent += mEvent; return this; }
        public Panel CreateFireEvent(Action action) { OnMouseEvent += PanelReactions.WhenClicked(action); return this; }
    }
}
