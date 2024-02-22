using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace alphappy.TAMacro
{
    public class MacroLibrary
    {
        public static Dictionary<string, Macro> macros = new Dictionary<string, Macro>();
        public static Macro activeMacro;
        public static Macro newestMacro;
        public static int page = 0;
        public static int macrosPerPage = 10;
        public static void LoadCookbook(string filename)
        {
            macros.Clear();
            try
            {
                if (File.Exists(filename))
                {
                    Mod.Log($"Reading cookbook at {filename}");
                    string[] lines = File.ReadAllLines(filename);
                    Dictionary<string, string> bookMeta = new Dictionary<string, string>();
                    for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
                    {
                        string rawline = lines[lineNumber];
                        string line = rawline.Trim(' ');
                        if (line.Length == 0) { continue; }
                        if (line[0] == '#') { continue; }
                        if (line.Length > 2 && line.Substring(0, 2) == "//") { ParseAsBookMetadata(line, bookMeta); continue; }
                        if (line[0] == '/') { ParseAsMacroMetadata(line); continue; }
                        if (ParseAsInstruction(line, rawline)) { continue; }
                        Mod.Log($"  Unrecognized instruction ignored at line {lineNumber + 1}: {line}");
                    }
                    page = 0;
                    ChangePage(0);
                }
                else { Mod.Log($"Tried to load non-existent cookbook: {filename}"); }
            }
            catch (Exception e) { Mod.Log(e); }
        }

        public static Macro SelectOnCurrentPage(int offset)
        {
            return macros.Values.ElementAt(offset + page * macrosPerPage);
        }

        public static void ChangePage(int delta)
        {
            Mod.Log("change page");
            page += delta;
            if (page < 0) { page = 0; }
            if (page > macros.Count / 10) { page = macros.Count / 10; }
            DisplayPanel.UpdateSelectableMacros();
        }

        public static bool ParseAsBookMetadata(string line, Dictionary<string, string> bookMeta)
        {
            if (Regex.Match(line, "^\\/\\/(\\w+): (.+)$") is Match match && match.Success)
            {
                bookMeta.Add(match.Groups[1].Value, match.Groups[2].Value);
                return true;
            }
            return false;
        }

        public static bool ParseAsMacroMetadata(string line)
        {
            if (Regex.Match(line, "^\\/(\\w+): (.+)$") is Match match && match.Success)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                if (key == "NAME")
                {
                    newestMacro = new Macro();
                    macros.Add(value, newestMacro);
                }
                newestMacro.metadata.Add(key, value);
                return true;
            }
            return false;
        }

        public static bool ParseAsInstruction(string line, string rawline)
        {
            if (newestMacro == null)
            {
                Mod.Log("A macro must be defined with /NAME: first");
                return false;
            }
            foreach (Parsers.Parser parser in Parsers.parsers)
            {
                if (parser.Invoke(line) is List<Instruction> list && list.Count > 0)
                {
                    newestMacro.text.AppendLine(rawline);
                    newestMacro.lines += 1;
                    foreach (Instruction instruction in list)
                    {
                        newestMacro.AddInstruction(instruction);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
