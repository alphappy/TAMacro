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
        public int SelectableCount => (IsCookbook ? macros.Count : children.Count) - ViewedPage * MacroLibrary.macrosPerPage;
        public string SelectableName(int offset)
        {
            int idx = viewedPage * MacroLibrary.macrosPerPage + offset;
            return IsCookbook
                ? (idx >= macros.Count ? null : macros.Values.ElementAt(idx).name)
                : (idx >= children.Count ? null : children.Values.ElementAt(idx).name);
        }

        public string sysPath;
        public string DisplayName => parent == null ? "." : (Settings.showFullPath.Value ? $"{parent.DisplayName}/{name}" : name);
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
                }
                else { Mod.Log($"Tried to load non-existent cookbook: {filename}"); }
            }
            catch (Exception e) { Mod.Log(e); }
        }

        public bool ParseAsBookMetadata(string line, Dictionary<string, string> bookMeta)
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
                    loading = new Macro();
                    macros.Add(value, loading);
                }
                loading.metadata.Add(key, value);
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

                loading.text.AppendLine(rawline);
                loading.newlinePositions.Add(loading.text.Length);
                loading.lines += 1;
                foreach (Instruction instruction in list)
                {
                    loading.AddInstruction(instruction);
                }
                return true;
            }
            return false;
        }
    }
}
