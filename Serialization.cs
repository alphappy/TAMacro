using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace alphappy.TAMacro
{
    internal static class Serialization
    {
        public static List<Record> records = new();
        public static void Populate()
        {
            records.Clear();

            var bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var playerFields = typeof(Player).GetFields(bf);
            var bcFields = typeof(BodyChunk).GetFields(bf);

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

            records.AddRange(playerFields.Where(f => f.FieldType == typeof(bool))
                .Select(RecordFactoryFactory(Translate.Primitive, Parse.Bool, Access.Player)));
            records.AddRange(playerFields.Where(f => f.FieldType == typeof(int))
                .Select(RecordFactoryFactory(Translate.Primitive, Parse.Int, Access.Player)));
            records.AddRange(playerFields.Where(f => f.FieldType == typeof(float))
                .Select(RecordFactoryFactory(Translate.Primitive, Parse.Float, Access.Player)));
            records.AddRange(playerFields.Where(f => f.FieldType == typeof(string))
                .Select(RecordFactoryFactory(Translate.Primitive, Parse.String, Access.Player)));

            records.AddRange(bcFields.Where(f => f.FieldType == typeof(Vector2))
                .Select(RecordFactoryFactory(Translate.Vector2, Parse.Vector2, Access.Chunk0)));

            records.AddRange(bcFields.Where(f => f.FieldType == typeof(Vector2))
                .Select(RecordFactoryFactory(Translate.Vector2, Parse.Vector2, Access.Chunk1)));
        }

        public static string Serialize(this Player p) => string.Join(";", records.Select(r => r.serializer(p)));
        public static void Deserialize(this Player p, string full)
        {
            foreach (var pair in records.Zip(full.Trim().Split(';'), (record, s) => new { record, s }))
            {
                pair.record.deserializer(p, pair.s);
            }
        }

        public static class Access
        {
            public static object Player(Player p) => p;
            public static object Chunk0(Player p) => p.bodyChunks[0];
            public static object Chunk1(Player p) => p.bodyChunks[1];
        }
        public static class Translate
        {
            public static string Primitive(object o) => o?.ToString() ?? "<NULL>";
            public static string Vector2(object o) => $"{((Vector2)o).x},{((Vector2)o).y}";
        }
        public static class Parse
        {
            public static object Bool(string s) => bool.Parse(s);
            public static object Int(string s) => int.Parse(s);
            public static object Float(string s) => float.Parse(s);
            public static object String(string s) => s;
            public static object Vector2(string s) { var a = s.Split(','); return new Vector2(float.Parse(a[0]), float.Parse(a[1])); }
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
