using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;

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
            { InstructionType.PushPickupCandidate, EnterPushPickupCandidate },
            { InstructionType.PushMyX, EnterPushMyX },
            { InstructionType.PushMyY, EnterPushMyY },
            { InstructionType.TestGreaterThan, EnterTestGreaterThan },
            { InstructionType.TestLessThan, EnterTestLessThan },
            { InstructionType.ExecuteMacroByString, EnterExecuteMacroByString },
            { InstructionType.ReturnTempNull, EnterReturnTempNull },
            { InstructionType.RequestWarp, EnterRequestWarp },
            { InstructionType.PushWarpWactive, EnterPushWarpActive },
            { InstructionType.SuperHardSetPosition, EnterSuperHardSetPosition },
            { InstructionType.GetGenericItem, EnterGetGenericItem },
            { InstructionType.GetSpear, EnterGetSpear },
            { InstructionType.SetCompleteScugStateFromString, EnterSetCompleteScugStateFromString },
            { InstructionType.SetDisplacementRefPoint, EnterSetDisplacementRefPoint },
            { InstructionType.PushMyAnimationIndexValue, EnterPushMyAnimationIndexValue },
            { InstructionType.PushConstant, EnterPushConstant },
            { InstructionType.TestEqualStrings, EnterTestEqualStrings },
            { InstructionType.SetPartialScugState, EnterSetPartialScugState }
        };
        public void Enter(Macro macro, Player player)
        {
            MacroLibrary.instructionsWithoutTick++;
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
        public static void EnterPushPickupCandidate(Instruction self, Macro macro, Player player)
        {
            macro.stack.Push(player.pickUpCandidate);
        }
        public static void EnterPushMyX(Instruction self, Macro macro, Player player) => macro.stack.Push(player.DangerPos.x);
        public static void EnterPushMyY(Instruction self, Macro macro, Player player) => macro.stack.Push(player.DangerPos.y);
        public static void EnterPushMyAnimationIndexValue(Instruction self, Macro macro, Player player) => macro.stack.Push(player.animation.value);
        public static void EnterTestGreaterThan(Instruction self, Macro macro, Player player) => macro.stack.Push((float)macro.stack.Pop() > (float)self.value);
        public static void EnterTestLessThan(Instruction self, Macro macro, Player player) => macro.stack.Push((float)macro.stack.Pop() < (float)self.value);
        public static void EnterExecuteMacroByString(Instruction self, Macro macro, Player player) => MacroLibrary.PushNewMacro((string)self.value, player);
        public static void EnterReturnTempNull(Instruction self, Macro macro, Player player) => macro.returnNull = true;
        public static void EnterRequestWarp(Instruction self, Macro macro, Player player)
        {
            if (!Const.WARP_MENU_ENABLED) return;
            var arg = (WarpTarget)self.value;
            WarpModMenu.newRoom = arg.room;
            WarpModMenu.newRegion = arg.room.Split('_')[0];
            WarpModMenu.coords = new RWCustom.IntVector2(arg.x, arg.y);
            WarpModMenu.warpActive = true;
        }
        public static void EnterPushWarpActive(Instruction self, Macro macro, Player player) => macro.stack.Push(Const.WARP_MENU_ENABLED && WarpModMenu.warpActive);
        public static void EnterSuperHardSetPosition(Instruction self, Macro macro, Player player) => player.SuperHardSetPosition((Vector2)self.value);
        public static void EnterGetGenericItem(Instruction self, Macro macro, Player player)
        {
            var type = new AbstractPhysicalObject.AbstractObjectType((string)self.value);
            var obj = new AbstractPhysicalObject(player.room.world, type, null, player.abstractCreature.pos, player.room.game.GetNewID());
            player.room.abstractRoom.AddEntity(obj);
            obj.RealizeInRoom();
            if (player.FreeHand() is int f && f > -1) player.SlugcatGrab(obj.realizedObject, f);
        }
        public static void EnterGetSpear(Instruction self, Macro macro, Player player)
        {
            var obj = new AbstractSpear(player.room.world, null, player.abstractCreature.pos, player.room.game.GetNewID(), (bool)self.value);
            player.room.abstractRoom.AddEntity(obj);
            obj.RealizeInRoom();
            if (player.FreeHand() is int f && f > -1) player.SlugcatGrab(obj.realizedObject, f);
        }
        public static void EnterSetCompleteScugStateFromString(Instruction self, Macro macro, Player player) => player.Deserialize((string)self.value);
        public static void EnterSetDisplacementRefPoint(Instruction self, Macro macro, Player player) => MacroLibrary.refPoint = player.bodyChunks[1].pos;
        public static void EnterPushConstant(Instruction self, Macro macro, Player player) => macro.stack.Push(self.value);
        public static void EnterTestEqualStrings(Instruction self, Macro macro, Player player) => macro.stack.Push((string)macro.stack.Pop() == (string)macro.stack.Pop());
        public static void EnterSetPartialScugState(Instruction self, Macro macro, Player player)
        {
            string value = (string)macro.stack.Pop();
            string fieldName = (string)macro.stack.Pop();
            string objName = (string)macro.stack.Pop();
            System.Reflection.FieldInfo field;
            object obj;

            switch (objName)
            {
                case "player":
                    field = typeof(Player).GetField(fieldName, Mod.bfAll);
                    obj = player;
                    break;

                case "bc0":
                    field = typeof(BodyChunk).GetField(fieldName, Mod.bfAll);
                    obj = player.bodyChunks[0];
                    break;

                case "bc1":
                    field = typeof(BodyChunk).GetField(fieldName, Mod.bfAll);
                    obj = player.bodyChunks[1];
                    break;

                case "bcc":
                    field = typeof(PhysicalObject.BodyChunkConnection).GetField(fieldName, Mod.bfAll);
                    obj = player.bodyChunkConnections[0];
                    break;

                default:
                    throw new Exceptions.IllegalCommandException($"'{objName}' is not a valid object identifier");
            }

            if (field is null) throw new Exceptions.IllegalCommandException($"'{fieldName}' is not a valid field for {obj.GetType()}");

            Func<string, object> parser = null;
            if (field.FieldType == typeof(bool)) parser = Serialization.Parse.Bool;
            else if (field.FieldType == typeof(int)) parser = Serialization.Parse.Int;
            else if (field.FieldType == typeof(float)) parser = Serialization.Parse.Float;
            else if (field.FieldType == typeof(float[])) parser = Serialization.Parse.FloatArray;
            else if (field.FieldType == typeof(string)) parser = Serialization.Parse.String;
            else if (field.FieldType == typeof(Vector2)) parser = Serialization.Parse.Vector2;
            else if (field.FieldType == typeof(Vector2?)) parser = Serialization.Parse.Vector2;
            else if (field.FieldType == typeof(RWCustom.IntVector2)) parser = Serialization.Parse.IntVector2;
            else if (field.FieldType == typeof(RWCustom.IntVector2?)) parser = Serialization.Parse.IntVector2;
            else if (field.FieldType == typeof(Player.InputPackage)) parser = Serialization.Parse.InputPackage;
            else if (field.FieldType == typeof(Player.InputPackage[])) parser = Serialization.Parse.InputPackageArray;
            else if (field.FieldType == typeof(Player.AnimationIndex)) parser = Serialization.Parse.AnimationIndex;
            else if (field.FieldType == typeof(Player.BodyModeIndex)) parser = Serialization.Parse.BodyModeIndex;
            else if (field.FieldType == typeof(PhysicalObject.BodyChunkConnection.Type)) parser = Serialization.Parse.ConnectionType;
            else throw new Exceptions.IllegalCommandException($"'{fieldName}' is not a supported field type ({field.FieldType.FullName})");

            try
            {
                field.SetValue(obj, parser(value));
            }
            catch (ArgumentException)
            {
                throw new Exceptions.IllegalCommandException($"'{value}' is not a valid value for field {field.Name}");
            }
        }
    }

    public enum InstructionType
    {
        NoOp, SetPackageFromNumber, Tick, SetHoldFromNumber, SetPackageFromString, SetPackageFromPackage,
        DefineLabelFromString, GotoLabelFromStringIfTrue, GotoLabelFromStringUnlessTrue, SetFlippablePackageFromPackage,
        TestScugTouch, TestGreaterThan, TestLessThan, TestEqualStrings,
        PushHeldPhysicalObject, TestPhysicalObjectIs,
        PushPickupCandidate, PushMyX, PushMyY, PushMyAnimationIndexValue,
        PushConstant,
        ExecuteMacroByString, ReturnTempNull,
        RequestWarp, PushWarpWactive, SuperHardSetPosition,
        GetGenericItem, GetSpear,
        SetCompleteScugStateFromString, SetPartialScugState,
        SetDisplacementRefPoint,
    }

    public struct WarpTarget
    {
        public string room; public int x; public int y;

        public WarpTarget(string room, int x, int y)
        {
            this.room = room;
            this.x = x;
            this.y = y;
        }
    }
}
