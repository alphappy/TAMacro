//using System;
//using System.Collections.Generic;
//using UnityEngine;

//namespace alphappy.TAMacro
//{
//    public class PaneElement
//    {
//        public delegate void Handler(PaneElement e);
//        public event Handler OnStartHovering;
//        public event Handler OnStopHovering;
//        public List<PaneElement> elements = new();
//        public FContainer container = new();
//        public PaneElement parent;
//        public bool beingHovered;
//        public Rect bounds;

//        private void StartHovering() => OnStartHovering?.Invoke(this);
//        private void StopHovering() => OnStopHovering?.Invoke(this);

//        public PaneElement(PaneElement parent, Rect box)
//        {
//            this.parent = parent;
//            bounds = box;
//            parent.container.AddChild(container);
//            container.AddChild(new FSprite("Futile_White")
//            {
//                x = box.center.x,
//                y = box.center.y,
//                width = box.width,
//                height = box.height,
//                color = Color.black,
//                alpha = 0.6f
//            });
//        }

//        public void Update(Vector2 cursor, bool leftClick)
//        {
//            var nowHovered = bounds.Contains(cursor);
//            if (nowHovered != beingHovered)
//            {
//                if (nowHovered) StartHovering(); else StopHovering();
//                beingHovered = nowHovered;
//            }
//        }
//        public void Destroy()
//        {
//            elements.ForEach(e => e.Destroy());
//            elements.Clear();
//            container.RemoveAllChildren();
//            container.RemoveFromContainer();
//            container = null;
//            parent = null;
//        }
//    }
//}
