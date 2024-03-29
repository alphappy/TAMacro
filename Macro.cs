﻿using IL.JollyCoop;
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
        public bool returnNull = false;

        public Player.InputPackage? GetPackage(Player player)
        {
            if (Const.SUPER_DEBUG_MODE) Mod.Log($"  enter GetPackage");
            readyToTick = false;
            if (hold > 0) hold--; else currentIndex++;
            while (currentIndex < instructions.Count)
            {
                if (Const.SUPER_DEBUG_MODE) Mod.Log($"  ({currentIndex:D4}) {current}");
                current.Enter(this, player);
                if (readyToTick) { return package; }
                if (returnNull) { returnNull = false; return null; }
                currentIndex++;
            }
            return null;
        }

        public void Initialize(Player player)
        {
            currentIndex = -1; hold = 0; readyToTick = false; package = default; returnNull = false;
            throwDirection = player.ThrowDirection;
            stack.Clear();
            DisplayPanel.TrackMe(this);
        }

        public void AddInstruction(Instruction inst)
        {
            instructions.Add(inst);
            lineNumbers.Add(lines);
            if (inst.type is InstructionType.DefineLabelFromString)
            {
                labels.Add((string)inst.value, instructions.Count);
            }
        }

        public static string RepFromInputList(List<List<Player.InputPackage>> lists)
        {
            StringBuilder sb = new StringBuilder($"/NAME: {UnityEngine.Random.Range(100000000, 999999999)}\n");
            Player.InputPackage previous = default;
            int consecutive = -1;
            foreach (List<Player.InputPackage> list in lists)
            {
                foreach (Player.InputPackage input in list)
                {
                    if (consecutive == -1)
                    {
                        consecutive = 1;
                        previous = input;
                    }
                    else if (input.EqualTo(previous))
                    {
                        consecutive++;
                    }
                    else
                    {
                        sb.Append($"{previous.AsString()}~{consecutive}\n");
                        consecutive = 1;
                        previous = input;
                    }
                }
            }
            return sb.ToString();
        }
    }
}
