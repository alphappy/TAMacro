using RWCustom;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity.Profiling;
using UnityEngine;

namespace alphappy.TAMacro
{
    internal static class Serialization
    {
        public static Dictionary<int, List<Record>> recordBooks = new();
        public const int currentVersion = 1;
        public static void Populate()
        {
            recordBooks.Clear();

            var bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var playerFields = typeof(Player).GetFields(bf);
            var bcFields = typeof(BodyChunk).GetFields(bf);
            var bccFields = typeof(PhysicalObject.BodyChunkConnection).GetFields(bf);

            var pairs = new Dictionary<Func<Player, object>, FieldInfo[]>
            {
                { Access.Player, playerFields }, 
                { Access.Chunk0, bcFields }, 
                { Access.Chunk1, bcFields }, 
                { Access.Connection, bccFields }
            };

            Func<FieldInfo, Record> RecordFactoryFactory(Func<object, string> translator, Func<string, object> parser, Func<Player, object> accessor)
            {
                Record RecordFactory(FieldInfo f)
                {
                    string Serializer(Player p) => translator(f.GetValue(accessor(p)));
                    void Deserializer(Player p, string s) => f.SetValue(accessor(p), parser(s));
                    return new(Serializer, Deserializer);
                }
                return RecordFactory;
            }

            var book = new List<Record>();
            recordBooks[1] = book;
            foreach (var pair in pairs)
            {
                var accessor = pair.Key;
                var fields = pair.Value;

                book.AddRange(fields.Where(f => f.FieldType == typeof(bool))
                    .Select(RecordFactoryFactory(Translate.Primitive, Parse.Bool, accessor)));
                book.AddRange(fields.Where(f => f.FieldType == typeof(int))
                    .Select(RecordFactoryFactory(Translate.Primitive, Parse.Int, accessor)));
                book.AddRange(fields.Where(f => f.FieldType == typeof(float))
                    .Select(RecordFactoryFactory(Translate.Primitive, Parse.Float, accessor)));

                book.AddRange(fields.Where(f => f.FieldType == typeof(Player.AnimationIndex))
                    .Select(RecordFactoryFactory(Translate.Primitive, Parse.AnimationIndex, accessor)));
                book.AddRange(fields.Where(f => f.FieldType == typeof(Player.BodyModeIndex))
                    .Select(RecordFactoryFactory(Translate.Primitive, Parse.BodyModeIndex, accessor)));
                book.AddRange(fields.Where(f => f.FieldType == typeof(PhysicalObject.BodyChunkConnection.Type))
                    .Select(RecordFactoryFactory(Translate.Primitive, Parse.ConnectionType, accessor)));

                book.AddRange(fields.Where(f => f.FieldType == typeof(Player.InputPackage))
                    .Select(RecordFactoryFactory(Translate.InputPackage, Parse.InputPackage, accessor)));

                book.AddRange(fields.Where(f => f.FieldType == typeof(IntVector2))
                    .Select(RecordFactoryFactory(Translate.IntVector2, Parse.IntVector2, accessor)));
                book.AddRange(fields.Where(f => f.FieldType == typeof(IntVector2?))
                    .Select(RecordFactoryFactory(Translate.IntVector2Nullable, Parse.IntVector2, accessor)));
                book.AddRange(fields.Where(f => f.FieldType == typeof(Vector2))
                    .Select(RecordFactoryFactory(Translate.Vector2, Parse.Vector2, accessor)));
                book.AddRange(fields.Where(f => f.FieldType == typeof(Vector2?))
                    .Select(RecordFactoryFactory(Translate.Vector2Nullable, Parse.Vector2, accessor)));
            }

            for (int i = 0; i < 10; i++)
            {
                int j = i;
                string Serializer(Player p) => Translate.InputPackage(p.input[j]);
                void Deserializer(Player p, string s) => p.input[j] = (Player.InputPackage)(Parse.InputPackage(s));
                book.Add(new(Serializer, Deserializer));
            };

            for (int i = 0; i < 4; i++)
            {
                int j = i;
                string Serializer(Player p) => Translate.Primitive(p.directionBoosts[j]);
                void Deserializer(Player p, string s) => p.directionBoosts[j] = (float)(Parse.Float(s));
                book.Add(new(Serializer, Deserializer));
            };

            for (int i = 0; i < 2; i++)
            {
                int j = i;
                string Serializer(Player p) => Translate.Primitive(p.dynamicRunSpeed[j]);
                void Deserializer(Player p, string s) => p.dynamicRunSpeed[j] = (float)(Parse.Float(s));
                book.Add(new(Serializer, Deserializer));
            };
        }

        public static string Serialize(this Player p) => $"{currentVersion}_" + string.Join(";", recordBooks[currentVersion].Select(r => r.serializer(p)));
        public static void Deserialize(this Player p, string full)
        {
            var parts = full.Split('_');
            var version = int.Parse(parts[0]);
            foreach (var pair in recordBooks[version].Zip(parts[1].Trim().Split(';'), (record, s) => new { record, s }))
            {
                pair.record.deserializer(p, pair.s);
            }
        }

        public static class Access
        {
            public static object Player(Player p) => p;
            public static object Chunk0(Player p) => p.bodyChunks[0];
            public static object Chunk1(Player p) => p.bodyChunks[1];
            public static object Connection(Player p) => p.bodyChunkConnections[0];
        }
        public static class Translate
        {
            public static string Primitive(object o) => o?.ToString() ?? "<NULL>";
            public static string PrimitiveArray(object o) => o is object[] a ? string.Join(",", a.Select(Primitive)) : "<NULL>";
            public static string Vector2(object o) => $"{((Vector2)o).x}|{((Vector2)o).y}";
            public static string Vector2Nullable(object o) => o is Vector2 v ? Vector2(v) : "<NULL>";
            public static string IntVector2(object o) => $"{((IntVector2)o).x}|{((IntVector2)o).y}";
            public static string IntVector2Nullable(object o) => o is IntVector2 v ? IntVector2(v) : "<NULL>";
            public static string InputPackage(object o)
            {
                if (o is Player.InputPackage p)
                {
                    return $"{p.x}|{p.y}|{p.jmp}|{p.thrw}|{p.pckp}|{p.mp}|{p.gamePad}|{p.crouchToggle}|{p.analogueDir.x}|{p.analogueDir.y}|{p.downDiagonal}";
                }
                return "<NULL>";
            }
            public static string InputPackageArray(object o) => o is object[] a ? string.Join(",", a.Select(InputPackage)) : "<NULL>";
        }
        public static class Parse
        {
            public static object Bool(string s) => bool.Parse(s);
            public static object Int(string s) => int.Parse(s);
            public static object Float(string s) => float.Parse(s);
            public static object FloatArray(string s) => s.Split(',').Select(Float).ToArray();
            public static object String(string s) => s == "<NULL>" ? null : s;
            public static object Vector2(string s)
            { 
                if (s == "<NULL>") return null;
                var a = s.Split('|'); 
                return new Vector2(float.Parse(a[0]), float.Parse(a[1]));
            }
            public static object IntVector2(string s)
            {
                if (s == "<NULL>") return null;
                var a = s.Split('|');
                return new IntVector2(int.Parse(a[0]), int.Parse(a[1]));
            }
            public static object InputPackage(string s)
            {
                var vals = s.Split('|');
                return new Player.InputPackage()
                {
                    x = int.Parse(vals[0]),
                    y = int.Parse(vals[1]),
                    jmp = bool.Parse(vals[2]),
                    thrw = bool.Parse(vals[3]),
                    pckp = bool.Parse(vals[4]),
                    mp = bool.Parse(vals[5]),
                    gamePad = bool.Parse(vals[6]),
                    crouchToggle = bool.Parse(vals[7]),
                    analogueDir = new(float.Parse(vals[8]), float.Parse(vals[9])),
                    downDiagonal = int.Parse(vals[10])
                };
            }
            public static object InputPackageArray(string s) => s.Split(',').Select(InputPackage).ToArray();
            public static object AnimationIndex(string s) => new Player.AnimationIndex(s, false);
            public static object BodyModeIndex(string s) => new Player.BodyModeIndex(s, false);
            public static object ConnectionType(string s) => new PhysicalObject.BodyChunkConnection.Type(s, false);
        }

        public struct Record
        {
            public Func<Player, string> serializer;
            public Action<Player, string> deserializer;
            public Record(Func<Player, string> serializer, Action<Player, string> deserializer)
            {
                this.serializer = serializer; this.deserializer = deserializer;
            }
        }
    }
}
