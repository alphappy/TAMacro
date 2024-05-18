using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text.RegularExpressions;
using UnityEngine;

namespace alphappy.TAMacro
{
    public class MacroContainer
    {
        public MacroContainer parent;
        public Dictionary<string, MacroContainer> children = new Dictionary<string, MacroContainer>();
        public Dictionary<string, Macro> macros = new Dictionary<string, Macro>();

        public Dictionary<string, string> bookMeta = new() 
        {
        };

        public int SelectableCount => (IsCookbook ? macros.Count : children.Count) - ViewedPage * MacroLibrary.macrosPerPage;
        public string SelectableName(int offset)
        {
            int idx = viewedPage * MacroLibrary.macrosPerPage + offset;
            return IsCookbook
                ? (idx >= macros.Count ? null : macros.Values.ElementAt(idx).name)
                : (idx >= children.Count ? null : children.Values.ElementAt(idx).name);
        }

        public string sysPath;
        public string DisplayName => parent == null ? "" : (Settings.showFullPath.Value ? FullName : name);
        public string FullName => parent == null ? "" : $"{parent.FullName}/{name}";
        public string name;
        private int viewedPage;
        public int ViewedPage
        {
            get { return viewedPage; }
            set { viewedPage = Mathf.Clamp(value, 0, PageCount - 1); }
        }
        public int PageCount => 1 + (children.Count + macros.Count - 1) / 10;
        public bool IsCookbook => macros.Count > 0;
        public MacroContainer SelectContainerOnViewedPage(int offset)
        {
            int idx = viewedPage * MacroLibrary.macrosPerPage + offset;
            return idx >= children.Count ? null : children.Values.ElementAt(idx);
        }
        public Macro SelectMacroOnViewedPage(int offset)
        {
            int idx = viewedPage * MacroLibrary.macrosPerPage + offset;
            return idx >= macros.Count ? null : macros.Values.ElementAt(idx);
        }

        public MacroContainer(string path, MacroContainer parent, bool recurse = true)
        {
            try
            {
                this.parent = parent;
                sysPath = path;
                if (File.Exists(path))
                {
                    if (Path.GetExtension(path) == ".tmc")
                    {
                        LoadCookbook(path);
                        name = Path.GetFileNameWithoutExtension(path);
                    }
                }
                else if (Directory.Exists(path) && recurse)
                {
                    name = new DirectoryInfo(path).Name;
                    foreach (string folderpath in Directory.GetDirectories(path))
                        children[new DirectoryInfo(folderpath).Name] = new MacroContainer(folderpath, this);
                    foreach (string filepath in Directory.GetFiles(path))
                        children[Path.GetFileNameWithoutExtension(filepath)] = new MacroContainer(filepath, this);
                }
                else
                {
                    Mod.Log($"{path} couldn't be processed");
                }
            }
            catch (Exception e) { Mod.Log(e); }
        }

        private Macro loading;
        public void LoadCookbook(string filename)
        {
            sysPath = filename;
            try
            {
                if (File.Exists(filename))
                {
                    Mod.Log($"Reading cookbook at {filename}");
                    string[] lines = File.ReadAllLines(filename);
                    for (int lineNumber = 0; lineNumber < lines.Length; lineNumber++)
                    {
                        string rawline = lines[lineNumber];
                        string line = rawline.Trim(' ');
                        if (line.Length == 0) { continue; }
                        if (line.StartsWith("# #")) { continue; }
                        if (line[0] == '#') { ParseAsComment(line); continue; }
                        if (line.Length > 2 && line.Substring(0, 2) == "//") { ParseAsBookMetadata(line); continue; }
                        if (line[0] == '/') { ParseAsMacroMetadata(line); continue; }
                        if (ParseAsInstruction(line, rawline)) { continue; }
                        Mod.Log($"  Unrecognized instruction ignored at line {lineNumber + 1}: {line}");
                    }
                }
                else { Mod.Log($"Tried to load non-existent cookbook: {filename}"); }
            }
            catch (Exception e) { Mod.Log(e); }
        }

        public bool ParseAsBookMetadata(string line)
        {
            if (Regex.Match(line, "^\\/\\/(\\w+): (.+)$") is Match match && match.Success)
            {
                bookMeta.Add(match.Groups[1].Value, match.Groups[2].Value);
                return true;
            }
            return false;
        }

        public bool ParseAsMacroMetadata(string line)
        {
            if (Regex.Match(line, "^\\/(\\w+): (.+)$") is Match match && match.Success)
            {
                string key = match.Groups[1].Value;
                string value = match.Groups[2].Value;
                if (key == "NAME")
                {
                    loading = new Macro(this);
                    loading.options.SetFromCookbookMetadata(bookMeta);
                    macros.Add(value, loading);
                }
                loading.options.Set(key, value);
                return true;
            }
            return false;
        }

        public bool ParseAsInstruction(string line, string rawline)
        {
            if (loading == null)
            {
                Mod.Log("A macro must be defined with /NAME: first");
                return false;
            }
            foreach (Parsers.Parser parser in Parsers.parsers)
            {
                var list = parser.Invoke(line);
                if (list == null) continue; // parser recognized the line (so it's not an invalid command) but chose to add no instructions

                var text = rawline.Length >= 50 ? rawline.Substring(0, 47) + "..." : rawline;
                loading.text.AppendLine(text);
                loading.lineTexts.Add(rawline.Trim());
                loading.newlinePositions.Add(loading.text.Length);
                foreach (Instruction instruction in list)
                {
                    loading.AddInstruction(instruction);
                }
                loading.lines += 1;
                return true;
            }
            return false;
        }

        public bool ParseAsComment(string line)
        {
            if (loading != null)
            {
                loading.text.AppendLine(line);
                loading.lineTexts.Add(line.Trim());
                loading.newlinePositions.Add(loading.text.Length);
                loading.lines += 1;
                return true;
            }
            return false;
        }
    }
}
