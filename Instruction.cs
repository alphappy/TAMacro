using System.Collections.Generic;
using System.Linq;

namespace alphappy.TAMacro
{
    public struct Instruction
    {
        public InstructionType type;
        public object value;

        public override string ToString() => $"{type} {value}";

        public Instruction(InstructionType type, object value = null) { this.type = type; this.value = value; }

        public delegate void EnterHandler(Instruction self, Macro macro, Player player);
        public static Dictionary<InstructionType, EnterHandler> enterHandlers = new Dictionary<InstructionType, EnterHandler>
        {
            { InstructionType.SetHoldFromNumber, EnterSetHoldFromNumber },
            { InstructionType.Tick, EnterTick },
            { InstructionType.SetPackageFromString, EnterSetPackageFromString },
            { InstructionType.SetPackageFromPackage, EnterSetPackageFromPackage },
            { InstructionType.SetFlippablePackageFromPackage, EnterSetFlippablePackageFromPackage },
            { InstructionType.DefineLabelFromString, EnterDefineLabelFromString },
            { InstructionType.GotoLabelFromStringIfTrue, EnterGotoLabelFromStringIfTrue },
            { InstructionType.GotoLabelFromStringUnlessTrue, EnterGotoLabelFromStringUnlessTrue },
            { InstructionType.TestScugTouch, EnterTestScugTouch },
            { InstructionType.PushHeldPhysicalObject, EnterPushHeldPhysicalObject },
            { InstructionType.TestPhysicalObjectIs, EnterTestPhysicalObjectIs },
        };
        public void Enter(Macro macro, Player player)
        {
            enterHandlers[type].Invoke(this, macro, player);
        }

        public static void EnterSetHoldFromNumber(Instruction self, Macro macro, Player player) { macro.hold = (int)self.value - 1; }
        public static void EnterTick(Instruction self, Macro macro, Player player) { macro.readyToTick = true; }
        public static void EnterSetPackageFromString(Instruction self, Macro macro, Player player)
        {
            foreach (char c in (string)self.value)
            {
                switch (c)
                {
                    case 'L': macro.package.analogueDir.x = -1; macro.package.x = -1; break;
                    case 'R': macro.package.analogueDir.x = 1; macro.package.x = 1; break;
                    //case 'B': macro.package.analogueDir.x = -1; macro.package.x = -1; macro.flippable = true; break;
                    //case 'F': macro.package.analogueDir.x = 1; macro.package.x = 1; macro.flippable = true; break;
                    case 'D': macro.package.y = -1; break;
                    case 'U': macro.package.y = 1; break;
                    case 'J': macro.package.jmp = true; break;
                    case 'G': macro.package.pckp = true; break;
                    case 'T': macro.package.thrw = true; break;
                    case 'M': macro.package.mp = true; break;
                }
            }
        }
        public static void EnterSetPackageFromPackage(Instruction self, Macro macro, Player player) { macro.package = (Player.InputPackage)self.value; }
        public static void EnterSetFlippablePackageFromPackage(Instruction self, Macro macro, Player player) 
        {
            macro.package = (Player.InputPackage)self.value; 
            if (macro.throwDirection < 0)
            {
                macro.package.x *= -1;
                macro.package.analogueDir.x *= -1;
            }
        }
        public static void EnterDefineLabelFromString(Instruction self, Macro macro, Player player) { }
        public static void EnterGotoLabelFromStringIfTrue(Instruction self, Macro macro, Player player)
        {
            if ((bool)macro.stack.Pop())
            {
                macro.currentIndex = macro.labels[(string)self.value] - 1;
            }
        }
        public static void EnterGotoLabelFromStringUnlessTrue(Instruction self, Macro macro, Player player)
        {
            if (!(bool)macro.stack.Pop())
            {
                macro.currentIndex = macro.labels[(string)self.value] - 1;
            }
        }
        public static void EnterTestScugTouch(Instruction self, Macro macro, Player player)
        {
            switch ((string)self.value)
            {
                case "floor": macro.stack.Push(player.bodyChunks.Any(chunk => chunk.ContactPoint.y < 0)); break;
                case "wall": macro.stack.Push(player.bodyChunks.Any(chunk => chunk.ContactPoint.x != 0)); break;
                case "ceiling": macro.stack.Push(player.bodyChunks.Any(chunk => chunk.ContactPoint.y > 0)); break;
                default: macro.stack.Push(false); break;
            }
        }
        public static void EnterTestPhysicalObjectIs(Instruction self, Macro macro, Player player)
        {
            object obj0 = macro.stack.Pop();
            if (obj0 == null) macro.stack.Push(false);
            PhysicalObject obj = (PhysicalObject)obj0;
            switch ((string)self.value)
            {
                case "ANY": macro.stack.Push(obj != null); break;
                case "WEAPON": macro.stack.Push(obj is Weapon); break;
                default: macro.stack.Push(obj?.abstractPhysicalObject.type.value == (string)self.value); break;
            }
        }
        public static void EnterPushHeldPhysicalObject(Instruction self, Macro macro, Player player)
        {
            macro.stack.Push(player.grasps[(int)self.value]?.grabbed);
        }
    }

    public enum InstructionType
    {
        NoOp, SetPackageFromNumber, Tick, SetHoldFromNumber, SetPackageFromString, SetPackageFromPackage,
        DefineLabelFromString, GotoLabelFromStringIfTrue, GotoLabelFromStringUnlessTrue, SetFlippablePackageFromPackage,
        TestScugTouch,
        PushHeldPhysicalObject, TestPhysicalObjectIs
    }
}
