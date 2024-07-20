using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace alphappy.TAMacro
{
    internal static class Keyboard
    {
        private static List<Binding> bindings = new();

        public static Binding NewBind(string modid, string name, KeyCode key)
        {
            if (GetBinding(modid, name) is not null)
                throw new Exceptions.TAMacroException("A binding with that name already exists.");

            if (GetBinding(key) is Binding binding)
                throw new Exceptions.TAMacroException($"`{key}` is already used by another binding:  `{binding}`");

            var n = new Binding(modid, name, key);
            bindings.Add(n);
            return n;
        }
        public static void RemoveBind(string modid, string name) => RemoveBind(GetBinding(modid, name));
        public static void RemoveBind(Binding binding)
        {
            if (!bindings.Remove(binding)) throw new Exceptions.TAMacroException($"`{binding}` is already unbound.");
        }
        public static void Update()
        {
            bindings.ForEach(x => x.Update());
        }

        public static Binding GetBinding(string modid, string name) => bindings.FirstOrDefault(b => b.name == name && b.modid == modid)
            ?? throw new Exceptions.TAMacroException($"No active binding `{modid} / {name}` exists.");

        public static Binding GetBinding(KeyCode code) => bindings.FirstOrDefault(b => b.key == code)
            ?? throw new Exceptions.TAMacroException($"No active binding to `{code}` exists.");

        public class Binding
        {
            public string modid;
            public string name;
            public KeyCode key;
            public event Action Action;
            public bool pressed;

            public Binding(string modid, string name, KeyCode key)
            {

            }

            public void Update()
            {
                bool nowPressed = Input.GetKey(key);
                if (nowPressed && !pressed) Action?.Invoke();
                pressed = nowPressed;
            }

            public void Remove() => bindings.Remove(this);

            public override string ToString()
            {
                return $"{modid} / {name} [{key}]";
            }
        }
    }
}
