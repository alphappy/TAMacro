using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace alphappy.TAMacro
{
    //public class InstructionBase
    //{
    //    public static Player.InputPackage neutralPackage = new Player.InputPackage(false, Options.ControlSetup.Preset.None, 0, 0, false, false, false, false, false);
    //    public delegate InstructionBase Creator(string s);
    //    public static List<Creator> Creators = new List<Creator> { InstructionSimple.FromString, InstructionDo.FromString, InstructionUntil.FromString, InstructionUntilContactWall.FromString, InstructionUntilContactFloor.FromString, InstructionUntilXY.FromString };

    //    public Player.InputPackage package;
    //    public int remain = 1;
    //    public int lifetime = 1;
    //    public virtual Player.InputPackage? GetPackage(Player player) { return package; }
    //    public virtual void Reset() { remain = lifetime; }
    //    public InstructionBase(string s) { }
    //    public InstructionBase() { }
    //}

    //public struct InstructionResult
    //{
    //    public static InstructionResult Empty = new InstructionResult();

    //    public int hold;
    //    public bool setCheckpoint;
    //    public bool useCheckpoint;
    //    public Player.InputPackage package;

    //    public InstructionResult(Player.InputPackage package = default, int hold = 1, bool setCheckpoint = false, bool useCheckpoint = false)
    //    {
    //        //if (hold < 0) throw new ArgumentOutOfRangeException("hold", hold, "Hold cannot be negative.");
    //        this.package = package; this.hold = hold; this.setCheckpoint = setCheckpoint; this.useCheckpoint = useCheckpoint;
    //    }
    //}

    //public class InstructionInstant : InstructionBase
    //{
    //    public InstructionInstant()
    //    {
    //        remain = 0;
    //        lifetime = 0;
    //    }
    //    public override Player.InputPackage? GetPackage(Player player) => null;
    //}

    //public class InstructionDo : InstructionInstant
    //{
    //    public static InstructionDo FromString(string s)
    //    {
    //        if (s == "DO") { return new InstructionDo(); }
    //        return null;
    //    }
    //}

    //public class InstructionUntil : InstructionInstant
    //{
    //    public static InstructionUntil FromString(string s)
    //    {
    //        if (s == "UNTIL") { return new InstructionUntil(); }
    //        return null;
    //    }
    //    public virtual bool ConditionMet => false;
    //    public override Player.InputPackage? GetPackage(Player player)
    //    {

    //        return null;
    //    }
    //}

    //public class InstructionUntilContactWall : InstructionUntil
    //{
    //    public static new InstructionUntilContactWall FromString(string s)
    //    {
    //        if (s == "UNTIL scug contacts wall") { return new InstructionUntilContactWall(); }
    //        return null;
    //    }
    //    public override bool ConditionMet => conditionMet;
    //    private bool conditionMet = false;
    //    public override Player.InputPackage? GetPackage(Player player)
    //    {
    //        conditionMet = (player.bodyChunks[0].ContactPoint.x != 0) || (player.bodyChunks[1].ContactPoint.x != 0);
    //        return null;
    //    }
    //}

    //public class InstructionUntilContactFloor : InstructionUntil
    //{
    //    public static new InstructionUntilContactFloor FromString(string s)
    //    {
    //        if (s == "UNTIL scug contacts floor") { return new InstructionUntilContactFloor(); }
    //        return null;
    //    }
    //    public override bool ConditionMet => conditionMet;
    //    private bool conditionMet = false;
    //    public override Player.InputPackage? GetPackage(Player player)
    //    {
    //        conditionMet = (player.bodyChunks[0].ContactPoint.y == -1) || (player.bodyChunks[1].ContactPoint.y == -1);
    //        return null;
    //    }
    //}

    //public class InstructionUntilXY : InstructionUntil
    //{
    //    public static new InstructionUntilXY FromString(string s)
    //    {
    //        if (Regex.Match(s, "^UNTIL scug (left of|right of|above|below) (\\d+\\.?\\d*)$") is Match match && match.Success)
    //        {
    //            var ret = new InstructionUntilXY();
    //            ret.xAxis = (match.Groups[1].Value == "left of") || (match.Groups[1].Value == "right of");
    //            ret.greater = (match.Groups[1].Value == "right of") || (match.Groups[1].Value == "above");
    //            float.TryParse(match.Groups[2].Value, out float f);
    //            ret.threshold = f;
    //            return ret;
    //        }
    //        return null;
    //    }
    //    public bool xAxis;
    //    public bool greater;
    //    public float threshold;
    //    public override bool ConditionMet => conditionMet;
    //    private bool conditionMet = false;
    //    public override Player.InputPackage? GetPackage(Player player)
    //    {
    //        conditionMet = (greater ? 1 : -1) * ((xAxis ? player.DangerPos.x : player.DangerPos.y) / 20f - threshold) > 0f;
    //        return null;
    //    }
    //}
}
