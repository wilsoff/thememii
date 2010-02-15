/* This file is part of ThemeMii
 * Copyright (C) 2009 Leathl
 * 
 * ThemeMii is free software: you can redistribute it and/or
 * modify it under the terms of the GNU General Public License as published
 * by the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * ThemeMii is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied warranty
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */
 
using System.IO;
using System.Collections.Generic;
using System.Text;
using System;
using System.ComponentModel;

namespace ThemeMii
{
    public struct iniEntry
    {
        public enum ContainerType
        {
            ASH = 0,
            U8 = 1,
        }

        public enum TplFormat
        {
            I4 = 0,
            I8 = 1,
            IA4 = 2,
            IA8 = 3,
            RGB5A3 = 5,
            RGB565 = 4,
            RGBA8 = 6
        }

        public enum EntryType
        {
            Container = 0,
            CustomImage = 1,
            StaticImage = 2,
            CustomData = 3,
            StaticData = 4
        }

        public EntryType entryType;
        public string entry;
        public string file;
        public string source;
        public string name;
        public ContainerType type;
        public int width;
        public int height;
        public TplFormat format;
        public string filepath;
    }

    class mymini
    {
        public event EventHandler<ProgressChangedEventArgs> ProgressChanged;
        private string iniFile;
        private List<iniEntry> entries = new List<iniEntry>();



        public iniEntry[] Entries { get { return entries.ToArray(); } set { entries = new List<iniEntry>(value); } }
        public List<iniEntry> EntryList { get { return entries; } set { entries = value; } }



        public static mymini LoadIni(string iniPath)
        {
            mymini m = new mymini();
            m.iniFile = iniPath;
            m.ParseIni();
            return m;
        }

        public static mymini CreateIni(iniEntry[] iniEntries)
        {
            mymini m = new mymini();
            m.entries = new List<iniEntry>(iniEntries);
            return m;
        }

        public void Load(string iniPath)
        {
            iniFile = iniPath;
            ParseIni();
        }

        public void Save(string savePath)
        {
            StringBuilder sb = new StringBuilder();
            int[] counters = new int[5];

            foreach (iniEntry tempEntry in entries)
            {
                if (tempEntry.entryType == iniEntry.EntryType.Container)
                {
                    sb.AppendLine(string.Format("[cont{0}]", ++counters[0]));
                    sb.AppendLine(string.Format("file={0}", tempEntry.file));
                    sb.AppendLine(string.Format("type={0}", (tempEntry.type == iniEntry.ContainerType.ASH) ? "ASH|U8" : "U8"));
                }
                else if (tempEntry.entryType == iniEntry.EntryType.CustomImage)
                {
                    sb.AppendLine(string.Format("[cimg{0}]", ++counters[1]));
                    sb.AppendLine(string.Format("file={0}", tempEntry.file));
                    sb.AppendLine(string.Format("name={0}", tempEntry.name));
                    sb.AppendLine(string.Format("width={0}", tempEntry.width));
                    sb.AppendLine(string.Format("height={0}", tempEntry.height));
                    if (tempEntry.format != iniEntry.TplFormat.RGB5A3)
                        sb.AppendLine(string.Format("format={0}", tempEntry.format.ToString()));
                }
                else if (tempEntry.entryType == iniEntry.EntryType.StaticImage)
                {
                    sb.AppendLine(string.Format("[simg{0}]", ++counters[2]));
                    sb.AppendLine(string.Format("file={0}", tempEntry.file));
                    sb.AppendLine(string.Format("source={0}", tempEntry.source));
                    sb.AppendLine(string.Format("width={0}", tempEntry.width));
                    sb.AppendLine(string.Format("height={0}", tempEntry.height));
                    if (tempEntry.format != iniEntry.TplFormat.RGB5A3)
                        sb.AppendLine(string.Format("format={0}", tempEntry.format.ToString()));
                }
                else if (tempEntry.entryType == iniEntry.EntryType.CustomData)
                {
                    sb.AppendLine(string.Format("[cdta{0}]", ++counters[3]));
                    sb.AppendLine(string.Format("file={0}", tempEntry.file));
                    sb.AppendLine(string.Format("name={0}", tempEntry.name));
                }
                else if (tempEntry.entryType == iniEntry.EntryType.StaticData)
                {
                    sb.AppendLine(string.Format("[sdta{0}]", ++counters[4]));
                    sb.AppendLine(string.Format("file={0}", tempEntry.file));
                    sb.AppendLine(string.Format("source={0}", tempEntry.source));
                }

                if (tempEntry.entry != entries[entries.Count - 1].entry)
                    sb.AppendLine();
            }

            using (FileStream fs = new FileStream(savePath, FileMode.Create))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(sb.ToString());
                }
            }
        }

        public iniEntry GetEntry(string entry)
        {
            foreach (iniEntry tempEntry in entries)
                if (entry == tempEntry.entry) return tempEntry;

            throw new Exception();
        }

        public void EditEntry(iniEntry editedEntry)
        {
            for (int i = 0; i < entries.Count; i++)
                if (entries[i].entry == editedEntry.entry) entries[i] = editedEntry;
        }



        private void ReportProgress(int progressPercentage, string statusText)
        {
            EventHandler<ProgressChangedEventArgs> _progressChanged = ProgressChanged;
            if (_progressChanged != null) ProgressChanged(this, new ProgressChangedEventArgs(progressPercentage, statusText));
        }
        
        private string[] CutComments(string[] iniLines)
        {
            for (int i = 0; i < iniLines.Length; i++)
            {
                if (iniLines[i].Contains(";"))
                {
                    iniLines[i] = iniLines[i].Remove(iniLines[i].IndexOf(";"));
                }

                while (iniLines[i].EndsWith(" "))
                    iniLines[i] = iniLines[i].Remove(iniLines[i].Length - 1);
            }

            return iniLines;
        }

        private void ParseIni()
        {
            string[] iniLines = File.ReadAllLines(iniFile);
            List<iniEntry> tempEntries = new List<iniEntry>();

            iniLines = CutComments(iniLines);

            for (int i = 0; i < iniLines.Length; i++)
            {
                ReportProgress((i + 1) * 100 / iniLines.Length, "Pasring mym.ini...");

                if (iniLines[i].ToLower().StartsWith("[cont")) //Container
                {
                    iniEntry tempEntry = new iniEntry();
                    tempEntry.entryType = iniEntry.EntryType.Container;
                    tempEntry.entry = iniLines[i];
                    bool[] trues = new bool[] { false, false };
                    int j = i + 1;

                    while (!iniLines[j].StartsWith("["))
                    {
                        if (iniLines[j].ToLower().StartsWith("file="))
                        {
                            tempEntry.file = iniLines[j].Substring(5);
                            trues[0] = true;
                        }
                        else if (iniLines[j].ToLower().StartsWith("type="))
                        {
                            if (iniLines[j].ToUpper().Contains("ASH"))
                                tempEntry.type = iniEntry.ContainerType.ASH;
                            else
                                tempEntry.type = iniEntry.ContainerType.U8;

                            trues[1] = true;
                        }

                        j++;
                        if (j == iniLines.Length) break;
                    }

                    if (trues[0] && trues[1]) tempEntries.Add(tempEntry);
                }
                else if (iniLines[i].ToLower().StartsWith("[cimg")) //Custom Image
                {
                    iniEntry tempEntry = new iniEntry();
                    tempEntry.entryType = iniEntry.EntryType.CustomImage;
                    tempEntry.entry = iniLines[i];
                    bool[] trues = new bool[] { false, false, false, false };
                    int j = i + 1;

                    tempEntry.format = iniEntry.TplFormat.RGB5A3;

                    while (!iniLines[j].StartsWith("["))
                    {
                        if (iniLines[j].ToLower().StartsWith("file="))
                        {
                            tempEntry.file = iniLines[j].Substring(5);
                            trues[0] = true;
                        }
                        else if (iniLines[j].ToLower().StartsWith("name="))
                        {
                            tempEntry.name = iniLines[j].Substring(5);
                            trues[1] = true;
                        }
                        else if (iniLines[j].ToLower().StartsWith("width="))
                        {
                            try
                            {
                                tempEntry.width = int.Parse(iniLines[j].Substring(6));
                                trues[2] = true;
                            }
                            catch { }
                        }
                        else if (iniLines[j].ToLower().StartsWith("height="))
                        {
                            try
                            {
                                tempEntry.height = int.Parse(iniLines[j].Substring(7));
                                trues[3] = true;
                            }
                            catch { }
                        }
                        else if (iniLines[j].ToLower().StartsWith("format="))
                        {
                            if (iniLines[j].Substring(7).ToUpper().StartsWith("RGBA8"))
                                tempEntry.format = iniEntry.TplFormat.RGBA8;
                            else if (iniLines[j].Substring(7).ToUpper().StartsWith("RGB565"))
                                tempEntry.format = iniEntry.TplFormat.RGB565;
                            else if (iniLines[j].Substring(7).ToUpper() == "I4")
                                tempEntry.format = iniEntry.TplFormat.I4;
                            else if (iniLines[j].Substring(7).ToUpper() == "I8")
                                tempEntry.format = iniEntry.TplFormat.I8;
                            else if (iniLines[j].Substring(7).ToUpper() == "IA4")
                                tempEntry.format = iniEntry.TplFormat.IA4;
                            else if (iniLines[j].Substring(7).ToUpper() == "IA8")
                                tempEntry.format = iniEntry.TplFormat.IA8;
                        }

                        j++;
                        if (j == iniLines.Length) break;
                    }

                    if (trues[0] && trues[1] && trues[2] && trues[3]) tempEntries.Add(tempEntry);
                }
                else if (iniLines[i].ToLower().StartsWith("[simg")) //Static Image
                {
                    iniEntry tempEntry = new iniEntry();
                    tempEntry.entryType = iniEntry.EntryType.StaticImage;
                    tempEntry.entry = iniLines[i];
                    bool[] trues = new bool[] { false, false, false, false };
                    int j = i + 1;

                    tempEntry.format = iniEntry.TplFormat.RGB5A3;

                    while (!iniLines[j].StartsWith("["))
                    {
                        if (iniLines[j].ToLower().StartsWith("file="))
                        {
                            tempEntry.file = iniLines[j].Substring(5);
                            trues[0] = true;
                        }
                        else if (iniLines[j].ToLower().StartsWith("source="))
                        {
                            tempEntry.source = iniLines[j].Substring(7);
                            trues[1] = true;
                        }
                        else if (iniLines[j].ToLower().StartsWith("width="))
                        {
                            try
                            {
                                tempEntry.width = int.Parse(iniLines[j].Substring(6));
                                trues[2] = true;
                            }
                            catch { }
                        }
                        else if (iniLines[j].ToLower().StartsWith("height="))
                        {
                            try
                            {
                                tempEntry.height = int.Parse(iniLines[j].Substring(7));
                                trues[3] = true;
                            }
                            catch { }
                        }
                        else if (iniLines[j].ToLower().StartsWith("format="))
                        {
                            if (iniLines[j].Substring(7).ToUpper() == "RGBA8")
                                tempEntry.format = iniEntry.TplFormat.RGBA8;
                            else if (iniLines[j].Substring(7).ToUpper() == "RGB565")
                                tempEntry.format = iniEntry.TplFormat.RGB565;
                            else if (iniLines[j].Substring(7).ToUpper() == "I4")
                                tempEntry.format = iniEntry.TplFormat.I4;
                            else if (iniLines[j].Substring(7).ToUpper() == "I8")
                                tempEntry.format = iniEntry.TplFormat.I8;
                            else if (iniLines[j].Substring(7).ToUpper() == "IA4")
                                tempEntry.format = iniEntry.TplFormat.IA4;
                            else if (iniLines[j].Substring(7).ToUpper() == "IA8")
                                tempEntry.format = iniEntry.TplFormat.IA8;
                        }

                        tempEntry.filepath = Path.GetDirectoryName(iniFile) + tempEntry.source;

                        j++;
                        if (j == iniLines.Length) break;
                    }

                    if (trues[0] && trues[1] && trues[2] && trues[3]) tempEntries.Add(tempEntry);
                }
                else if (iniLines[i].ToLower().StartsWith("[cdta")) //Custom Data
                {
                    iniEntry tempEntry = new iniEntry();
                    tempEntry.entryType = iniEntry.EntryType.CustomData;
                    tempEntry.entry = iniLines[i];
                    bool[] trues = new bool[] { false, false };
                    int j = i + 1;

                    while (!iniLines[j].StartsWith("["))
                    {
                        if (iniLines[j].ToLower().StartsWith("file="))
                        {
                            tempEntry.file = iniLines[j].Substring(5);
                            trues[0] = true;
                        }
                        else if (iniLines[j].ToLower().StartsWith("name="))
                        {
                            tempEntry.name = iniLines[j].Substring(5);
                            trues[1] = true;
                        }

                        j++;
                        if (j == iniLines.Length) break;
                    }

                    if (trues[0] && trues[1]) tempEntries.Add(tempEntry);
                }
                else if (iniLines[i].ToLower().StartsWith("[sdta")) //Static Data
                {
                    iniEntry tempEntry = new iniEntry();
                    tempEntry.entryType = iniEntry.EntryType.StaticData;
                    tempEntry.entry = iniLines[i];
                    bool[] trues = new bool[] { false, false };
                    int j = i + 1;

                    while (!iniLines[j].StartsWith("["))
                    {
                        if (iniLines[j].ToLower().StartsWith("file="))
                        {
                            tempEntry.file = iniLines[j].Substring(5);
                            trues[0] = true;
                        }
                        else if (iniLines[j].ToLower().StartsWith("source="))
                        {
                            tempEntry.source = iniLines[j].Substring(7);
                            trues[1] = true;
                        }

                        j++;
                        if (j == iniLines.Length) break;
                    }

                    tempEntry.filepath = Path.GetDirectoryName(iniFile) + tempEntry.source;

                    if (trues[0] && trues[1]) tempEntries.Add(tempEntry);
                }
            }

            entries = tempEntries;
        }

    }
}
