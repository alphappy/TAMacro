using RWCustom;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using UnityEngine;

namespace alphappy.TAMacro
{
    internal class ScugState
    {
        public static BindingFlags bf = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static Type[] types = {
            typeof(bool), typeof(int), typeof(float), typeof(string),
            typeof(float[]),
            typeof(Vector2), typeof(Vector2?), typeof(IntVector2), typeof(IntVector2?),
            typeof(Player.InputPackage), typeof(Player.InputPackage[]),
            typeof(Player.AnimationIndex), typeof(Player.BodyModeIndex),
            typeof(BodyChunk[]), typeof(PhysicalObject.BodyChunkConnection[]), typeof(PhysicalObject.BodyChunkConnection.Type),
        };

        public static IEnumerable<FieldInfo> playerFields = typeof(Player).GetFields(bf).Where(f => types.Contains(f.FieldType));
        public static IEnumerable<FieldInfo> bodychunkFields = typeof(BodyChunk).GetFields(bf).Where(f => types.Contains(f.FieldType));
        public static IEnumerable<FieldInfo> bccFields = typeof(PhysicalObject.BodyChunkConnection).GetFields(bf).Where(f => types.Contains(f.FieldType));

        public ScugState(Player player)
        {
            var dict = playerFields.ToDictionary(f => f.Name, f => Serialize(f.GetValue(player)));
            Mod.Log(string.Join("\n", dict.Select(pair => $"{pair.Key}: {pair.Value}"))); // TODO destroy
        }

        public string Serialize(object obj)
        {
            return obj switch
            {
                Player.InputPackage p => $"{p.x}|{p.y}|{p.jmp}|{p.thrw}|{p.pckp}|{p.mp}|{p.gamePad}|{p.crouchToggle}|{p.analogueDir.x}|{p.analogueDir.y}|{p.downDiagonal}",
                Player.InputPackage[] ps => string.Join("/", ps.Select(p => Serialize(p))),
                Vector2 v => $"{v.x}|{v.y}",
                IntVector2 v => $"{v.x}|{v.y}",
                BodyChunk b => string.Join("|", bodychunkFields.Select(f => Serialize(f.GetValue(b)))),
                BodyChunk[] bs => string.Join("/", bs.Select(b => Serialize(b))),
                PhysicalObject.BodyChunkConnection c => string.Join("|", bccFields.Select(f => Serialize(f.GetValue(c)))),
                PhysicalObject.BodyChunkConnection[] cs => string.Join("/", cs.Select(c => Serialize(c))),
                null => "<NULL>",
                float[] f => string.Join(";", f),
                _ => obj.ToString()
            };
        }

        public void Deserialize(string full, Player p)
        {
            var pairs = playerFields.Zip(full.Trim().Split(';'), (k, v) => new {k,v}).ToDictionary(x => x.k, x => x.v);
            foreach (var pair in pairs)
            {
                var field = pair.Key;
                var s = pair.Value;
                object value = null;

                if (s == "<NULL>") value = null;
                else if (field.FieldType == typeof(string)) value = s;
                else if (field.FieldType == typeof(int)) value = int.Parse(s);
                else if (field.FieldType == typeof(float)) value = float.Parse(s);
                else if (field.FieldType == typeof(bool)) value = bool.Parse(s);
                else if (field.FieldType == typeof(float[])) value = s.Split(';').Select(e => float.Parse(e)).ToArray();

                else if (field.FieldType == typeof(Player.InputPackage)) value = ConstructInputPackage(s);
                else if (field.FieldType == typeof(Player.InputPackage[])) value = s.Split('/').Select(e => ConstructInputPackage(e));

                else if (field.FieldType == typeof(Player.AnimationIndex)) value = new Player.AnimationIndex(s, false);
                else if (field.FieldType == typeof(Player.BodyModeIndex)) value = new Player.BodyModeIndex(s, false);

                else if (field.FieldType == typeof(Vector2) || field.FieldType == typeof(Vector2?))
                {
                    var vals = s.Split('|');
                    value = new Vector2(float.Parse(vals[0]), float.Parse(vals[1]));
                }

                else if (field.FieldType == typeof(IntVector2) || field.FieldType == typeof(IntVector2?))
                {
                    var vals = s.Split('|');
                    value = new IntVector2(int.Parse(vals[0]), int.Parse(vals[1]));
                }

                else if (field.FieldType == typeof(BodyChunk[]))
                {
                    int chunkNum = 0;
                    foreach (var chunkSettings in s.Split('/'))
                    {
                        var chunk = p.bodyChunks[chunkNum];
                        foreach (var pair2 in bodychunkFields.Zip(chunkSettings.Split('|'), (field, value) => new { field, value }))
                        {
                            pair2.field.SetValue(chunk, pair2.value);
                        }
                        chunkNum++;
                    }
                    continue;
                }

                field.SetValue(p, value);
            }
        }

        public static Player.InputPackage ConstructInputPackage(string s)
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
                analogueDir = new(int.Parse(vals[8]), int.Parse(vals[9])),
                downDiagonal = int.Parse(vals[10])
            };
        }
    }
}
