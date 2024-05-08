//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using static alphappy.TAMacro.PaneButton;

//namespace alphappy.TAMacro
//{
//    public class Pane
//    {
//        public FContainer container;
//        public FSprite backdrop;
//        public List<PaneElement> elements;
//        public Vector2 dragRelPos;
//        public Vector2 anchor;
//        public PaneElement hovered;
//        public PaneElement clicked;
//        public bool clicking;

//        public void InitiateSprites()
//        {
//            container = new FContainer();
//            Futile.stage.AddChild(container);
//            anchor = Vector2.zero - Vector2.one;
//            container.SetPosition(anchor);

//            backdrop = new FSprite("pixel")
//            {
//                alpha = 0.4f, color = Color.black, x = 500f, y = 500f, width = 200f, height = 600f
//            };

//            container.AddChild(backdrop);
//        }
//        public void Update()
//        {
//            MouseUpdate();
//            elements.ForEach(e => e.Update());
//        }
//        public void MouseUpdate()
//        {
//            bool nowClicking = Input.GetMouseButton(0);
//            Vector2 mousePos = Input.mousePosition;

//            if (backdrop.BoundingBox().Contains(mousePos))
//            {
//                var newHovered = elements.FirstOrDefault(e => e.BoundingBox.Contains(mousePos));
//                if (hovered != newHovered)
//                {
//                    hovered?.StopHovering();
//                    newHovered?.StartHovering();
//                    hovered = newHovered;
//                }
//            }
//        }
//        public void Remove()
//        {
//            container.RemoveFromContainer();
//            container.RemoveAllChildren();
//            container = null;
//            elements.ForEach(e => e.Remove());
//        }
//    }
//}
