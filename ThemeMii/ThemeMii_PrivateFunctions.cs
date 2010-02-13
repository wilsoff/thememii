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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;

namespace ThemeMii
{
    partial class ThemeMii_Main
    {
        private void ShowDisclaimer()
        {
            MessageBox.Show("Only install themes if you have a proper brickprotection or you might get a brick beyond repair!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private bool CheckInet()
        {
            try
            {
                System.Net.IPHostEntry ipHost = System.Net.Dns.GetHostEntry("www.google.com");
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void Initialize()
        {
            lastSelected = -1;
            lbxIniEntries.Items.Clear();
            ClearControls(this);
            SetControls(false);
            HidePanels();
            if (!string.IsNullOrEmpty(tempDir)) ClearTempDir();
            GetTempDir();
        }

        private void ClearControls(Control parentControl)
        {
            foreach (Control thisControl in parentControl.Controls)
            {
                if (thisControl is Panel) ClearControls(thisControl);
                else if (thisControl is TextBox) ((TextBox)thisControl).Text = string.Empty;
                else if (thisControl is ComboBox) ((ComboBox)thisControl).SelectedIndex = 0;
            }
        }

        private void GetTempDir()
        {
            tempDir = Path.GetTempPath() + Guid.NewGuid().ToString() + "\\";
            appOut = tempDir + "appOut\\";
            mymOut = tempDir + "mymOut\\";

            Directory.CreateDirectory(appOut);
            Directory.CreateDirectory(mymOut);
        }

        private void ClearTempDir()
        {
            try { Directory.Delete(tempDir, true); }
            catch { }
        }

        private void SetControls(bool enable)
        {
            MethodInvoker m;

            if (enable)
                m = new MethodInvoker(this._setControlsTrue);
            else
                m = new MethodInvoker(this._setControlsFalse);

            this.Invoke(m);
        }

        private void HidePanels()
        {
            panContainer.Visible = false;
            panCustomImage.Visible = false;
            panStaticImage.Visible = false;
            panCustomData.Visible = false;
            panStaticData.Visible = false;
        }

        private void LoadSettings()
        {
            containerManage = Properties.Settings.Default.containerManage;
            msContainerManage.Checked = containerManage;
            sourceManage = Properties.Settings.Default.sourceManage;
            msSourceManage.Checked = sourceManage;
            autoImageSize = Properties.Settings.Default.autoImageSize;
            msImageSizeFromPng.Checked = autoImageSize;
            ignoreMissing = Properties.Settings.Default.ignoreMissing;
            msIgnoreMissingEntries.Checked = ignoreMissing;

            BaseApp bApp = (BaseApp)Properties.Settings.Default.standardMenu;
            UncheckSysMenus();
            if (bApp == BaseApp.E32) ms32E.Checked = true;
            else if (bApp == BaseApp.E40) ms40E.Checked = true;
            else if (bApp == BaseApp.E41) ms41E.Checked = true;
            else if (bApp == BaseApp.E42) ms42E.Checked = true;
            else if (bApp == BaseApp.J32) ms32J.Checked = true;
            else if (bApp == BaseApp.J40) ms40J.Checked = true;
            else if (bApp == BaseApp.J41) ms41J.Checked = true;
            else if (bApp == BaseApp.J42) ms42J.Checked = true;
            else if (bApp == BaseApp.U32) ms32U.Checked = true;
            else if (bApp == BaseApp.U40) ms40U.Checked = true;
            else if (bApp == BaseApp.U41) ms41U.Checked = true;
            else if (bApp == BaseApp.U42) ms42U.Checked = true;

            if (sourceManage)
            {
                tbStaticDataSource.Enabled = false;
                tbStaticImageSource.Enabled = false;
            }
            if (autoImageSize)
            {
                tbStaticImageWidth.Enabled = false;
                tbStaticImageHeight.Enabled = false;
                tbCustomImageWidth.Enabled = false;
                tbCustomImageHeight.Enabled = false;
            }
        }

        private void SaveSettings()
        {
            Properties.Settings.Default.firstRun = false;
            Properties.Settings.Default.containerManage = containerManage;
            Properties.Settings.Default.sourceManage = sourceManage;
            Properties.Settings.Default.autoImageSize = autoImageSize;
            Properties.Settings.Default.ignoreMissing = ignoreMissing;

            Properties.Settings.Default.standardMenu = (int)GetBaseApp();

            Properties.Settings.Default.Save();
        }

        private BaseApp GetBaseApp()
        {
            if (ms32E.Checked) return BaseApp.E32;
            else if (ms32J.Checked) return BaseApp.J32;
            else if (ms32U.Checked) return BaseApp.U32;
            else if (ms40E.Checked) return BaseApp.E40;
            else if (ms40J.Checked) return BaseApp.J40;
            else if (ms40U.Checked) return BaseApp.U40;
            else if (ms41E.Checked) return BaseApp.E41;
            else if (ms41J.Checked) return BaseApp.J41;
            else if (ms41U.Checked) return BaseApp.U41;
            else if (ms42E.Checked) return BaseApp.E42;
            else if (ms42J.Checked) return BaseApp.J42;
            else if (ms42U.Checked) return BaseApp.U42;
            else return (BaseApp)0;
        }

        private void UncheckSysMenus()
        {
            ms32E.Checked = false;
            ms32J.Checked = false;
            ms32U.Checked = false;

            ms40E.Checked = false;
            ms40J.Checked = false;
            ms40U.Checked = false;

            ms41E.Checked = false;
            ms41J.Checked = false;
            ms41U.Checked = false;

            ms42E.Checked = false;
            ms42J.Checked = false;
            ms42U.Checked = false;
        }

        private void ReportProgress(int progressPercentage, string statusText)
        {
            _progressReporter p = new _progressReporter(this._reportProgress);
            this.Invoke(p, new ProgressChangedEventArgs(progressPercentage, (object)statusText));
        }

        private void ErrorBox(string message)
        {
            boxInvoker b = new boxInvoker(this._errorBox);
            this.Invoke(b, message);
        }

        private void InfoBox(string message)
        {
            boxInvoker b = new boxInvoker(this._infoBox);
            this.Invoke(b, message);
        }

        private void AddEntries()
        {
            try
            {

                for (int i=0;i<ini.Entries.Length;i++)
                {
                    ReportProgress((i + 1) * 100 / ini.Entries.Length, "Loading entries...");
                    lbxIniEntries.Items.Add(ini.Entries[i].entry);
                }
                ReportProgress(100, " ");
                SetControls(true);
            }
            catch (Exception ex) { ErrorBox(ex.Message); }
        }

        private void SwapEntries(int selectedIndex, bool up)
        {
            if (selectedIndex > -1)
            {
                int bIndex = (up) ? selectedIndex - 1 : selectedIndex + 1;

                if (bIndex > -1 && bIndex < lbxIniEntries.Items.Count)
                {
                    SaveSelected();
                    object selectedObject = lbxIniEntries.Items[selectedIndex];
                    lbxIniEntries.Items[selectedIndex] = lbxIniEntries.Items[bIndex];
                    lbxIniEntries.Items[bIndex] = selectedObject;

                    lbxIniEntries.SelectedIndex = bIndex;
                }
            }
        }

        private void SaveSelected()
        {
            if (lbxIniEntries.SelectedIndex == -1) return;

            iniEntry tempEntry = ini.GetEntry(lbxIniEntries.Items[lbxIniEntries.SelectedIndex].ToString());

            if (tempEntry.entryType == iniEntry.EntryType.Container)
            {
                tempEntry.file = (tbContainerFile.Text.StartsWith("\\")) ? tbContainerFile.Text : tbContainerFile.Text.Insert(0, "\\");
                tempEntry.type = (cmbContainerFormat.SelectedIndex == 0) ? iniEntry.ContainerType.ASH : iniEntry.ContainerType.U8;
            }
            else if (tempEntry.entryType == iniEntry.EntryType.CustomImage)
            {
                tempEntry.file = (tbCustomImageFile.Text.StartsWith("\\")) ? tbCustomImageFile.Text : tbCustomImageFile.Text.Insert(0, "\\");
                tempEntry.name = tbCustomImageName.Text;
                if (!autoImageSize) tempEntry.width = int.Parse(tbCustomImageWidth.Text);
                if (!autoImageSize) tempEntry.height = int.Parse(tbCustomImageHeight.Text);
                tempEntry.format = (cmbCustomImageFormat.SelectedIndex == 0) ? iniEntry.TplFormat.RGB5A3 :
                    (cmbCustomImageFormat.SelectedIndex == 1) ? iniEntry.TplFormat.RGBA8 : iniEntry.TplFormat.RGB565;
            }
            else if (tempEntry.entryType == iniEntry.EntryType.StaticImage)
            {
                tempEntry.file = (tbStaticImageFile.Text.StartsWith("\\")) ? tbStaticImageFile.Text : tbStaticImageFile.Text.Insert(0, "\\");
                if (!sourceManage) tempEntry.source = (tbStaticImageSource.Text.StartsWith("\\")) ? tbStaticImageSource.Text : tbStaticImageSource.Text.Insert(0, "\\");
                if (!autoImageSize) tempEntry.width = int.Parse(tbStaticImageWidth.Text);
                if (!autoImageSize) tempEntry.height = int.Parse(tbStaticImageHeight.Text);
                tempEntry.format = (cmbStaticImageFormat.SelectedIndex == 0) ? iniEntry.TplFormat.RGB5A3 :
                    (cmbStaticImageFormat.SelectedIndex == 1) ? iniEntry.TplFormat.RGBA8 : iniEntry.TplFormat.RGB565;

                tempEntry.filepath = tbStaticImageFilepath.Text;
            }
            else if (tempEntry.entryType == iniEntry.EntryType.CustomData)
            {
                tempEntry.file = (tbCustomDataFile.Text.StartsWith("\\")) ? tbCustomDataFile.Text : tbCustomDataFile.Text.Insert(0, "\\");
                tempEntry.name = tbCustomDataName.Text;
            }
            else if (tempEntry.entryType == iniEntry.EntryType.StaticData)
            {
                tempEntry.file = (tbStaticDataFile.Text.StartsWith("\\")) ? tbStaticDataFile.Text : tbStaticDataFile.Text.Insert(0, "\\");
                if (!sourceManage) tempEntry.source = (tbStaticDataSource.Text.StartsWith("\\")) ? tbStaticDataSource.Text : tbStaticDataSource.Text.Insert(0, "\\");

                tempEntry.filepath = tbStaticDataFilepath.Text;
            }

            ini.EditEntry(tempEntry);
        }

        private void SaveLastSelected()
        {
            if (lastSelected > -1 && lastSelected < lbxIniEntries.Items.Count && lbxIniEntries.Items[lastSelected].ToString() == lastSelectedEntry)
            {
                iniEntry tempEntry = ini.GetEntry(lbxIniEntries.Items[lastSelected].ToString());

                if (tempEntry.entryType == iniEntry.EntryType.Container)
                {
                    tempEntry.file = (tbContainerFile.Text.StartsWith("\\")) ? tbContainerFile.Text : tbContainerFile.Text.Insert(0, "\\");
                    tempEntry.type = (cmbContainerFormat.SelectedIndex == 0) ? iniEntry.ContainerType.ASH : iniEntry.ContainerType.U8;
                }
                else if (tempEntry.entryType == iniEntry.EntryType.CustomImage)
                {
                    tempEntry.file = (tbCustomImageFile.Text.StartsWith("\\")) ? tbCustomImageFile.Text : tbCustomImageFile.Text.Insert(0, "\\");
                    tempEntry.name = tbCustomImageName.Text;
                    if (!autoImageSize) tempEntry.width = int.Parse(tbCustomImageWidth.Text);
                    if (!autoImageSize) tempEntry.height = int.Parse(tbCustomImageHeight.Text);
                    tempEntry.format = (cmbCustomImageFormat.SelectedIndex == 0) ? iniEntry.TplFormat.RGB5A3 :
                        (cmbCustomImageFormat.SelectedIndex == 1) ? iniEntry.TplFormat.RGBA8 : iniEntry.TplFormat.RGB565;
                }
                else if (tempEntry.entryType == iniEntry.EntryType.StaticImage)
                {
                    tempEntry.file = (tbStaticImageFile.Text.StartsWith("\\")) ? tbStaticImageFile.Text : tbStaticImageFile.Text.Insert(0, "\\");
                    if (!sourceManage) tempEntry.source = (tbStaticImageSource.Text.StartsWith("\\")) ? tbStaticImageSource.Text : tbStaticImageSource.Text.Insert(0, "\\");
                    if (!autoImageSize) tempEntry.width = int.Parse(tbStaticImageWidth.Text);
                    if (!autoImageSize) tempEntry.height = int.Parse(tbStaticImageHeight.Text);
                    tempEntry.format = (cmbStaticImageFormat.SelectedIndex == 0) ? iniEntry.TplFormat.RGB5A3 :
                        (cmbStaticImageFormat.SelectedIndex == 1) ? iniEntry.TplFormat.RGBA8 : iniEntry.TplFormat.RGB565;

                    tempEntry.filepath = tbStaticImageFilepath.Text;
                }
                else if (tempEntry.entryType == iniEntry.EntryType.CustomData)
                {
                    tempEntry.file = (tbCustomDataFile.Text.StartsWith("\\")) ? tbCustomDataFile.Text : tbCustomDataFile.Text.Insert(0, "\\");
                    tempEntry.name = tbCustomDataName.Text;
                }
                else if (tempEntry.entryType == iniEntry.EntryType.StaticData)
                {
                    tempEntry.file = (tbStaticDataFile.Text.StartsWith("\\")) ? tbStaticDataFile.Text : tbStaticDataFile.Text.Insert(0, "\\");
                    if (!sourceManage) tempEntry.source = (tbStaticDataSource.Text.StartsWith("\\")) ? tbStaticDataSource.Text : tbStaticDataSource.Text.Insert(0, "\\");

                    tempEntry.filepath = tbStaticDataFilepath.Text;
                }

                ini.EditEntry(tempEntry);
            }
        }

        private void AddEntry(iniEntry.EntryType entryType)
        {
            int newIndex = GetLastEntryNum(entryType) + 1;

            string type = "[cont";
            if (entryType == iniEntry.EntryType.CustomImage) type = "[cimg";
            else if (entryType == iniEntry.EntryType.StaticImage) type = "[simg";
            else if (entryType == iniEntry.EntryType.CustomData) type = "[cdta";
            else if (entryType == iniEntry.EntryType.StaticData) type = "[sdta";

            iniEntry newEntry = new iniEntry();
            newEntry.entryType = entryType;
            newEntry.entry = type + newIndex.ToString() + "]";

            ini.EntryList.Add(newEntry);
            lbxIniEntries.Items.Add(newEntry.entry);
            lbxIniEntries.SelectedIndex = lbxIniEntries.Items.Count - 1;
        }

        private int GetLastEntryNum(iniEntry.EntryType entryType)
        {
            int highestIndex = 0;

            string type = "[cont";
            if (entryType == iniEntry.EntryType.CustomImage) type = "[cimg";
            else if (entryType == iniEntry.EntryType.StaticImage) type = "[simg";
            else if (entryType == iniEntry.EntryType.CustomData) type = "[cdta";
            else if (entryType == iniEntry.EntryType.StaticData) type = "[sdta";

            foreach (iniEntry entry in ini.EntryList)
            {
                if (entry.entryType == entryType)
                {
                    int newIndex = int.Parse(entry.entry.Replace(type, "").Replace("]", ""));
                    if (newIndex > highestIndex) highestIndex = newIndex;
                }
            }

            return highestIndex;
        }

        private void CreateCsm(string appFile)
        {
            SaveSelected();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "csm|*.csm";

            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                ReportProgress(0, "Collecting data...");

                List<object> lbEntries = new List<object>();

                foreach (object entry in lbxIniEntries.Items)
                    lbEntries.Add(entry);

                CreationInfo cInfo = new CreationInfo();
                cInfo.savePath = sfd.FileName;
                cInfo.lbEntries = lbEntries.ToArray();
                cInfo.createCsm = true;
                cInfo.appFile = appFile;

                Thread workerThread = new Thread(new ParameterizedThreadStart(this._saveMym));
                workerThread.Start(cInfo);
            }
        }

        private void SaveMym()
        {
            if (lbxIniEntries.Items.Count > 0)
            {
                SaveSelected();

                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "mym|*.mym";

                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    ReportProgress(0, "Collecting data...");

                    List<object> lbEntries = new List<object>();

                    foreach (object entry in lbxIniEntries.Items)
                        lbEntries.Add(entry);

                    CreationInfo cInfo = new CreationInfo();
                    cInfo.savePath = sfd.FileName;
                    cInfo.lbEntries = lbEntries.ToArray();
                    cInfo.createCsm = false;

                    Thread workerThread = new Thread(new ParameterizedThreadStart(this._saveMym));
                    workerThread.Start(cInfo);
                }
            }
        }

        private bool CheckEntry(iniEntry entry)
        {
            if (entry.entryType == iniEntry.EntryType.Container)
            {
                if (string.IsNullOrEmpty(entry.file) || entry.file.Length < 2)
                { if (!ignoreMissing) ErrorBox(string.Format("Entry: {0}\nInvalid argument \"file\"...", entry.entry)); return false; }
            }
            else if (entry.entryType == iniEntry.EntryType.CustomImage)
            {
                if (string.IsNullOrEmpty(entry.file) || entry.file.Length < 2)
                { if (!ignoreMissing) ErrorBox(string.Format("Entry: {0}\nInvalid argument \"file\"...", entry.entry)); return false; }
                if (string.IsNullOrEmpty(entry.name))
                { if (!ignoreMissing) ErrorBox(string.Format("Entry: {0}\nInvalid argument \"name\"...", entry.entry)); return false; }
            }
            else if (entry.entryType == iniEntry.EntryType.StaticImage)
            {
                if (string.IsNullOrEmpty(entry.file) || entry.file.Length < 2)
                { if (!ignoreMissing) ErrorBox(string.Format("Entry: {0}\nInvalid argument \"file\"...", entry.entry)); return false; }
                if (string.IsNullOrEmpty(entry.source))
                { if (!ignoreMissing) ErrorBox(string.Format("Entry: {0}\nInvalid argument \"source\"...", entry.entry)); return false; }

                if (!File.Exists(entry.filepath))
                { if (!ignoreMissing) ErrorBox(string.Format("Entry: {0}\nFile not found...\n\n{1}", entry.entry, entry.filepath)); return false; }
            }
            else if (entry.entryType == iniEntry.EntryType.CustomData)
            {
                if (string.IsNullOrEmpty(entry.file) || entry.file.Length < 2)
                { if (!ignoreMissing) ErrorBox(string.Format("Entry: {0}\nInvalid argument \"file\"...", entry.entry)); return false; }
                if (string.IsNullOrEmpty(entry.name))
                { if (!ignoreMissing) ErrorBox(string.Format("Entry: {0}\nInvalid argument \"name\"...", entry.entry)); return false; }
            }
            else if (entry.entryType == iniEntry.EntryType.StaticData)
            {
                if (string.IsNullOrEmpty(entry.file) || entry.file.Length < 2)
                { if (!ignoreMissing) ErrorBox(string.Format("Entry: {0}\nInvalid argument \"file\"...", entry.entry)); return false; }
                if (string.IsNullOrEmpty(entry.source))
                { if (!ignoreMissing) ErrorBox(string.Format("Entry: {0}\nInvalid argument \"source\"...", entry.entry)); return false; }

                if (!File.Exists(entry.filepath))
                { if (!ignoreMissing) ErrorBox(string.Format("Entry: {0}\nFile not found...\n\n{1}", entry.entry, entry.filepath)); return false; }
            }

            return true;
        }

        private void RemoveEntry(int index)
        {
            try
            {
                string entry = lbxIniEntries.Items[index].ToString();
                lbxIniEntries.Items.RemoveAt(index);

                ini.EntryList.Remove(ini.GetEntry(entry));

                if (lbxIniEntries.Items.Count > index)
                    lbxIniEntries.SelectedIndex = index;
                else
                    lbxIniEntries.SelectedIndex = index - 1;
            }
            catch { }
        }

        private void DeASH(iniEntry mymC, string appOut)
        {
            ProcessStartInfo pInfo = new ProcessStartInfo(Application.StartupPath + "\\ASH.exe", string.Format("\"{0}\"", appOut + mymC.file));
            pInfo.UseShellExecute = false;
            //pInfo.RedirectStandardOutput = true;
            pInfo.CreateNoWindow = true;

            Process p = Process.Start(pInfo);
            p.WaitForExit();

            //ErrorBox(p.StandardOutput.ReadToEnd() + "\n\n" + mymC.file);
        }

        private void DeASH(string path)
        {
            ProcessStartInfo pInfo = new ProcessStartInfo(Application.StartupPath + "\\ASH.exe", string.Format("\"{0}\"", path));
            pInfo.UseShellExecute = false;
            //pInfo.RedirectStandardOutput = true;
            pInfo.CreateNoWindow = true;

            Process p = Process.Start(pInfo);
            p.WaitForExit();

            //ErrorBox(p.StandardOutput.ReadToEnd() + "\n\n" + mymC.file);
        }

        private Image ResizeImage(Image img, int x, int y)
        {
            Image newimage = new Bitmap(x, y);
            using (Graphics gfx = Graphics.FromImage(newimage))
            {
                gfx.DrawImage(img, 0, 0, x, y);
            }
            return newimage;
        }

        private bool HashCheck(byte[] newFile, byte[] tmdHash)
        {
            System.Security.Cryptography.SHA1 sha = System.Security.Cryptography.SHA1.Create();
            byte[] fileHash = sha.ComputeHash(newFile);

            return Wii.Tools.CompareByteArrays(fileHash, tmdHash);
        }

        private bool CommonKeyCheck()
        {
            if (!File.Exists(Application.StartupPath + "\\common-key.bin"))
            {
                ThemeMii_ckInput ib = new ThemeMii_ckInput();

                if (ib.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Wii.Tools.CreateCommonKey(ib.Input, Application.StartupPath);
                    return true;
                }
                else return false;
            }
            else return true;
        }

        private BaseApp GetStandardBaseApp()
        {
            if (ms32E.Checked) return BaseApp.E32;
            if (ms32U.Checked) return BaseApp.U32;
            if (ms32J.Checked) return BaseApp.J32;
            if (ms40E.Checked) return BaseApp.E40;
            if (ms40U.Checked) return BaseApp.U40;
            if (ms40J.Checked) return BaseApp.J40;
            if (ms41E.Checked) return BaseApp.E41;
            if (ms41U.Checked) return BaseApp.U41;
            if (ms41J.Checked) return BaseApp.J41;
            if (ms42E.Checked) return BaseApp.E42;
            if (ms42U.Checked) return BaseApp.U42;
            if (ms42J.Checked) return BaseApp.J42;
            return (BaseApp)0;
        }

        private bool EntryExists(iniEntry entry, List<string[]> list)
        {
            foreach (string[] array in list)
            {
                if (array[0] == entry.source)
                {
                    FileInfo fi = new FileInfo(entry.filepath);
                    if (fi.Length != int.Parse(array[1]))
                        return true;
                }
            }

            return false;
        }

        private void AppBrowse()
        {
            BaseApp standardApp = GetStandardBaseApp();
            if (standardApp == (BaseApp)0) { ErrorBox("You have to choose a Standard System Menu!"); return; }

            string browsePath = tempDir + "appBrowse\\";

            if (standardApp != lastExtracted || !Directory.Exists(browsePath))
            {
                //Extract app
                if (!File.Exists(Application.StartupPath + "\\" + ((int)standardApp).ToString("x8") + ".app"))
                { ErrorBox("app file wasn't found!"); return; }

                Thread workerThread = new Thread(new ParameterizedThreadStart(this._extractAppForBrowsing));
                workerThread.Start(new object[] { standardApp, browsePath });
            }
            else OpenAppBrowser(browsePath);
        }

        private void OpenAppBrowser(string browsePath)
        {
            if (!Directory.Exists(browsePath)) { ErrorBox("An error occured!"); return; }

            ThemeMii_AppBrowse appBrowser = new ThemeMii_AppBrowse();
            appBrowser.RootPath = browsePath;
            appBrowser.ViewOnly = viewOnly;
            appBrowser.ContainerBrowse = containerBrowse;
            appBrowser.SelectedPath = selectedNode;

            if (appBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (panStaticData.Visible) tbStaticDataFile.Text = appBrowser.SelectedPath;
                else if (panStaticImage.Visible) tbStaticImageFile.Text = appBrowser.SelectedPath;
                else if (panCustomData.Visible) tbCustomDataFile.Text = appBrowser.SelectedPath;
                else if (panCustomImage.Visible) tbCustomImageFile.Text = appBrowser.SelectedPath;
                else if (panContainer.Visible) tbContainerFile.Text = appBrowser.SelectedPath;
            }
        }

        private bool StringExistsInStringArray(string theString, string[] theStringArray)
        {
            return Array.Exists(theStringArray, thisString => thisString.ToLower() == theString.ToLower());
        }
    }
}
