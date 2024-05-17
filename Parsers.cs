using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace alphappy.TAMacro
{
    internal class Parsers
    {
        public delegate List<Instruction> Parser(string line);
        public static List<Parser> parsers = new List<Parser> { Simple, DefineLabel, ConditionScugTouch, ConditionScugHold, ConditionScugWant, ConditionScugPosition, ExecuteMacro, CheatWarp, CheatGetItem, CheatScugState };

        public static List<Instruction> Simple(string line)
        {
            var ret = new List<Instruction>();

            if (Regex.Match(line, "^([LRFBUDdJGTM]+)(?:[~`\\-](\\d+))?$") is Match match && match.Success)
            {
                Player.InputPackage package = new Player.InputPackage();
                bool flippable = false;
                foreach (char c in match.Groups[1].Value)
                {
                    switch (c)
                    {
                        case 'L': package.analogueDir.x = -1; package.x = -1; break;
                        case 'R': package.analogueDir.x = 1; package.x = 1; break;
                        case 'B': package.analogueDir.x = -1; package.x = -1; flippable = true; break;
                        case 'F': package.analogueDir.x = 1; package.x = 1; flippable = true; break;
                        case 'D': package.analogueDir.y = -1; package.y = -1; break;
                        case 'd': package.analogueDir.y = -0.06f; package.y = 0; break;
                        case 'U': package.analogueDir.y = 1; package.y = 1; break;
                        case 'J': package.jmp = true; break;
                        case 'G': package.pckp = true; break;
                        case 'T': package.thrw = true; break;
                        case 'M': package.mp = true; break;
                    }
                }
                ret.Add(new Instruction(flippable ? InstructionType.SetFlippablePackageFromPackage : InstructionType.SetPackageFromPackage, package));
                if (int.TryParse(match.Groups[2].Value, out int i) && i > 0)
                {
                    ret.Add(new Instruction(InstructionType.SetHoldFromNumber, i));
                }
                ret.Add(new Instruction(InstructionType.Tick));
                return ret;
            }
            else if (Regex.Match(line, "^[~`\\-](\\d+)$") is Match match2 && match2.Success)
            {
                ret.Add(new Instruction(InstructionType.SetPackageFromPackage, default(Player.InputPackage)));
                if (int.TryParse(match2.Groups[1].Value, out int i) && i > 0)
                {
                    ret.Add(new Instruction(InstructionType.SetHoldFromNumber, i));
                }
                ret.Add(new Instruction(InstructionType.Tick));
            }
            else { return null; }

            return ret;
        }

        public static List<Instruction> DefineLabel(string line)
        {
            if (Regex.Match(line, "^>label ([\\w\\d]+)$") is Match match && match.Success)
            {
                return new List<Instruction> { new Instruction(InstructionType.DefineLabelFromString, match.Groups[1].Value) };
            }
            return null;
        }
        public static List<Instruction> ConditionScugTouch(string line)
        {
            if (Regex.Match(line, "^>goto ([\\w\\d]+) (unless|if) scug touch (\\w+)$") is Match match && match.Success)
            {
                return new List<Instruction> 
                {
                    new Instruction(InstructionType.TestScugTouch, match.Groups[3].Value),
                    new Instruction(
                        match.Groups[2].Value == "if" ? InstructionType.GotoLabelFromStringIfTrue : InstructionType.GotoLabelFromStringUnlessTrue, 
                        match.Groups[1].Value
                        )
                };
            }
            return null;
        }
        public static List<Instruction> ConditionScugHold(string line)
        {
            if (Regex.Match(line, "^>goto ([\\w\\d]+) (unless|if) scug hold (\\w+)(?: in (first|second))?$") is Match match && match.Success)
            {
                return new List<Instruction>
                {
                    new Instruction(InstructionType.PushHeldPhysicalObject, match.Groups[4].Value == "second" ? 1 : 0),
                    new Instruction(InstructionType.TestPhysicalObjectIs, match.Groups[3].Value),
                    new Instruction(
                        match.Groups[2].Value == "if" ? InstructionType.GotoLabelFromStringIfTrue : InstructionType.GotoLabelFromStringUnlessTrue,
                        match.Groups[1].Value
                        )
                };
            }
            return null;
        }
        public static List<Instruction> ConditionScugWant(string line)
        {
            if (Regex.Match(line, "^>goto ([\\w\\d]+) (unless|if) scug want (\\w+)$") is Match match && match.Success)
            {
                return new List<Instruction>
                {
                    new Instruction(InstructionType.PushPickupCandidate),
                    new Instruction(InstructionType.TestPhysicalObjectIs, match.Groups[3].Value),
                    new Instruction(
                        match.Groups[2].Value == "if" ? InstructionType.GotoLabelFromStringIfTrue : InstructionType.GotoLabelFromStringUnlessTrue,
                        match.Groups[1].Value
                        )
                };
            }
            return null;
        }
        public static List<Instruction> ConditionScugPosition(string line)
        {
            if (Regex.Match(line, "^>goto ([\\w\\d]+) (unless|if) scug located (left of|right of|above|below) (\\d+\\.?\\d*)$") is Match match && match.Success)
            {
                return new List<Instruction>
                {
                    new Instruction(match.Groups[3].Value.Contains("of") ? InstructionType.PushMyX : InstructionType.PushMyY),
                    new Instruction(match.Groups[3].Value == "right of" || match.Groups[3].Value == "above" ? InstructionType.TestGreaterThan : InstructionType.TestLessThan, float.Parse(match.Groups[4].Value)),
                    new Instruction(
                        match.Groups[2].Value == "if" ? InstructionType.GotoLabelFromStringIfTrue : InstructionType.GotoLabelFromStringUnlessTrue,
                        match.Groups[1].Value
                        )
                };
            }
            return null;
        }
        public static List<Instruction> ExecuteMacro(string line)
        {
            if (Regex.Match(line, "^>execute ([\\w\\d\\-+\\. /]+)$") is Match match && match.Success)
            {
                return new List<Instruction>
                {
                    new Instruction(InstructionType.ExecuteMacroByString, match.Groups[1].Value),
                    new Instruction(InstructionType.ReturnTempNull)
                };
            }
            return null;
        }
        public static List<Instruction> CheatWarp(string line)
        {
            if (Regex.Match(line, "!warp (?:([\\w_]+) )?(-?\\d+(?:\\.\\d+)?) (-?\\d+(?:\\.\\d+)?)") is Match match && match.Success)
            {
                float.TryParse(match.Groups[2].Value, out float x);
                float.TryParse(match.Groups[3].Value, out float y);

                if (match.Groups[1].Value == "") return new List<Instruction> { new(InstructionType.SuperHardSetPosition, new Vector2(x, y)) };

                if (!Const.WARP_MENU_ENABLED)
                {
                    Mod.Log("Warp Menu must be enabled to warp to another room.  Warp command will be ignored.");
                    return new();
                }

                return new List<Instruction>
                {
                    new Instruction(InstructionType.RequestWarp, new WarpTarget(match.Groups[1].Value, (int)(x / 20), (int)(y / 20))),
                    new Instruction(InstructionType.DefineLabelFromString, line),
                    new Instruction(InstructionType.Tick),
                    new Instruction(InstructionType.PushWarpWactive),
                    new Instruction(InstructionType.GotoLabelFromStringIfTrue, line),
                    new Instruction(InstructionType.SuperHardSetPosition, new Vector2(x, y)),
                };
            }
            return null;
        }
        public static List<Instruction> CheatGetItem(string line)
        {
            if (Regex.Match(line, "!get (\\w+)") is Match match && match.Success)
            {
                string item = match.Groups[1].Value;
                if (item == "Spear") return new List<Instruction> { new Instruction(InstructionType.GetSpear, false), };
                if (item == "ExplosiveSpear") return new List<Instruction> { new Instruction(InstructionType.GetSpear, true), };

                return new List<Instruction> { new Instruction(InstructionType.GetGenericItem, item), };
            }
            return null;
        }

        public static List<Instruction> CheatScugState(string line)
        {
            if (Regex.Match(line, "!scugstate (.+)") is Match match && match.Success)
            {
                return new List<Instruction> { new Instruction(InstructionType.SetCompleteScugStateFromString, match.Groups[1].Value), };
            }
            return null;
        }
    }
}
