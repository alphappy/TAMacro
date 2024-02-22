using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace alphappy.TAMacro
{
    internal class Parsers
    {
        public delegate List<Instruction> Parser(string line);
        public static List<Parser> parsers = new List<Parser> { Simple, DefineLabel, GotoLabelUnlessOrIfScugTouch };

        public static List<Instruction> Simple(string line)
        {
            var ret = new List<Instruction>();

            if (Regex.Match(line, "^([LRFBUDJGTM]+)(?:~(\\d+))?$") is Match match && match.Success)
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
                        case 'D': package.y = -1; break;
                        case 'U': package.y = 1; break;
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
            else if (Regex.Match(line, "^~(\\d+)$") is Match match2 && match2.Success)
            {
                ret.Add(new Instruction(InstructionType.SetPackageFromPackage, default(Player.InputPackage)));
                if (int.TryParse(match2.Groups[1].Value, out int i) && i > 0)
                {
                    ret.Add(new Instruction(InstructionType.SetHoldFromNumber, i));
                }
                ret.Add(new Instruction(InstructionType.Tick));
            }

            return ret;
        }

        public static List<Instruction> DefineLabel(string line)
        {
            if (Regex.Match(line, "^>label ([\\w\\d]+)$") is Match match && match.Success)
            {
                return new List<Instruction> { new Instruction(InstructionType.DefineLabelFromString, match.Groups[1].Value) };
            }
            return new List<Instruction>();
        }
        public static List<Instruction> GotoLabelUnlessOrIfScugTouch(string line)
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
            return new List<Instruction>();
        }
    }
}
