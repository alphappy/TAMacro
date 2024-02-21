using System.Text.RegularExpressions;

namespace alphappy.TAMacro
{
    //public class InstructionSimple : InstructionBase
    //{
    //    public bool flippable;
    //    public InstructionSimple(int x = 0, int y = 0, bool jmp = false, bool thrw = false, bool pckp = false, bool mp = false)
    //    {
    //        package = new Player.InputPackage(false, Options.ControlSetup.Preset.None, x, y, jmp, thrw, pckp, mp, false);
    //    }
    //    public override Player.InputPackage? GetPackage(Player player)
    //    {
    //        if (!flippable) return package;
    //        if (player.Data().initialDirection == 1) return package;
    //        if (package is Player.InputPackage packageDefinitelyNotNull)
    //        {
    //            packageDefinitelyNotNull.x *= -1; 
    //            packageDefinitelyNotNull.analogueDir.x *= -1; 
    //            return packageDefinitelyNotNull; 
    //        }
    //        return package;
    //    }
    //    public static InstructionSimple FromString(string s)
    //    {
    //        if (Regex.Match(s, "^([LRFBUDJGTM]+)(?:~(\\d+))?$") is Match match && match.Success)
    //        {
    //            InstructionSimple ret = new InstructionSimple();
    //            foreach (char c in match.Groups[1].Value)
    //            {
    //                switch (c)
    //                {
    //                    case 'L': ret.package.analogueDir.x = -1; ret.package.x = -1; break;
    //                    case 'R': ret.package.analogueDir.x = 1; ret.package.x = 1; break;
    //                    case 'B': ret.package.analogueDir.x = -1; ret.package.x = -1; ret.flippable = true; break;
    //                    case 'F': ret.package.analogueDir.x = 1; ret.package.x = 1; ret.flippable = true; break;
    //                    case 'D': ret.package.y = -1; break;
    //                    case 'U': ret.package.y = 1; break;
    //                    case 'J': ret.package.jmp = true; break;
    //                    case 'G': ret.package.pckp = true; break;
    //                    case 'T': ret.package.thrw = true; break;
    //                    case 'M': ret.package.mp = true; break;
    //                }
    //            }
    //            if (int.TryParse(match.Groups[2].Value, out int i) && i > 0)
    //            {
    //                ret.remain = i; ret.lifetime = i;
    //            }
    //            return ret;
    //        }
    //        else if (Regex.Match(s, "^~(\\d+)$") is Match match2 && match2.Success)
    //        {
    //            InstructionSimple ret = new InstructionSimple();
    //            if (int.TryParse(match2.Groups[1].Value, out int i) && i > 0)
    //            {
    //                ret.remain = i; ret.lifetime = i;
    //            }
    //            return ret;
    //        }
    //        return null;
    //    }
    //}
}
