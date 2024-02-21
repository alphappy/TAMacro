using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace alphappy.TAMacro
{
    public class Macro
    {
        public List<Instruction> instructions = new List<Instruction>();
        public int lines;
        public List<int> lineNumbers = new List<int>();
        public Dictionary<string, string> metadata = new Dictionary<string, string>();
        public StringBuilder text = new StringBuilder();
        public Dictionary<string, int> labels = new Dictionary<string, int>();

        public Instruction current => instructions[currentIndex];
        public int currentLine => lineNumbers[currentIndex];
        public string name => metadata.TryGetValue("NAME", out string s) ? s : "";

        public int currentIndex = -1;
        public int hold;
        public bool readyToTick;
        public Player.InputPackage package;
        public int throwDirection;
        public Stack<object> stack = new Stack<object>();

        public Player.InputPackage? GetPackage(Player player)
        {
            readyToTick = false;
            if (hold > 0) hold--; else currentIndex++;
            while (currentIndex < instructions.Count)
            {
                current.Enter(this, player);
                if (readyToTick) { return package; }
                currentIndex++;
            }
            return null;
        }

        public void Initialize(Player player)
        {
            currentIndex = -1; hold = 0; readyToTick = false; package = default;
            throwDirection = player.ThrowDirection;
            labels.Clear(); stack.Clear();
            DisplayPanel.TrackMe(this);
        }
    }
}
