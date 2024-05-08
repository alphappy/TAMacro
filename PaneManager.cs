//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;

//namespace alphappy.TAMacro
//{
//    public class PaneManager
//    {
//        public static PaneManager instance;
//        public List<PaneElement> elements = new();
//        public FContainer container = new();
//        public PaneManager()
//        {
//            Futile.stage.AddChild(container);
//            Default();
//        }
//        public void Default()
//        {
//            PaneElement e = new(this, new(500f, 500f, 200f, 300f));
//            e.OnStartHovering += e => { e.sprites[0].alpha += 0.4f; };
//            e.OnStopHovering += e => { e.sprites[0].alpha -= 0.4f; };
//        }

//        public static void Initialize() => instance = new PaneManager();
//        public static void Kill() { instance.Destroy(); instance = null; }

//        public void Update()
//        {
//            var pos = Input.mousePosition;
//            var click = Input.GetMouseButton(0);
//            elements.ForEach(e => e.Update(pos, click));
//        }
//        public void Destroy()
//        {
//            elements.ForEach(e => e.Destroy());
//            elements.Clear();
//            container.RemoveAllChildren();
//            container.RemoveFromContainer();
//            container = null;
//        }
//    }
//}
