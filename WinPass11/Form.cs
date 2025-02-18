﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using WinPass11.Helpers;

namespace WinPass11
{
    public partial class Form : System.Windows.Forms.Form
    {
        public static string mTempWorkingDir = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "WinPass11");

        public Form()
        {
            InitializeComponent();
            InitializeWorkingDir();

            MaximizeBox = false;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void InitializeWorkingDir()
        {
            try
            {
                if (Directory.Exists(mTempWorkingDir))
                    Directory.Delete(mTempWorkingDir, true);
                Directory.CreateDirectory(mTempWorkingDir);
            }
            catch { }
        }

        private void InstallButtonClick(object sender, EventArgs e)
        {
            string regTweaksDownloadPath = $@"{mTempWorkingDir}\regtweaks.reg";

            try
            {
                Utils.DownloadFile(Constants.Url.RegTweaks, regTweaksDownloadPath);
                Utils.ShowMessageBox(string.Format(Strings.Body.DownloadSuccess, "registry tweaks"), MessageBoxType.Information);
            }
            // Create an error box if download fails
            catch
            {
                Utils.ShowMessageBox(string.Format(Strings.Body.DownloadFailed, "registry tweaks"), MessageBoxType.Error);
            }

            if (selectionBox.Text == "Dev")
            {
                DialogResult result = MessageBox.Show(Strings.Body.InstallButtonDialog, "WinPass11 Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result.Equals(DialogResult.Yes))
                {
                    if (File.Exists(regTweaksDownloadPath))
                    {
                        try
                        {
                            string text = File.ReadAllText(regTweaksDownloadPath);
                            text = text.Replace("Beta", "Dev");
                            File.WriteAllText(regTweaksDownloadPath, text);
                        }
                        catch
                        {
                            Utils.ShowMessageBox($"{Strings.Body.RegApplyFailed}\nStage 1 Failed", MessageBoxType.Error);
                        }

                        try
                        {
                            int ret = Utils.StartProcess("regedit.exe", $"/s {regTweaksDownloadPath}", true);
                            Console.WriteLine("regedit exited with exit code of {0}", ret);
                            Utils.ShowMessageBox(Strings.Body.RegApplySuccess, MessageBoxType.Information);
                        }
                        // Create an error box if registry applicaation fails
                        catch
                        {
                            Utils.ShowMessageBox($"{Strings.Body.RegApplyFailed}\nStage 2 Failed", MessageBoxType.Error);
                        }
                    }
                    else
                    {
                        Utils.ShowMessageBox(Strings.Body.RegFileNotDownloaded, MessageBoxType.Error);
                    }
                    string usoClient = "UsoClient";
                    int ust = Utils.StartProcess(usoClient, "StartInteractiveScan", true);
                    Console.WriteLine($"{usoClient} exited with exit code of {0}", ust);

                    // debug: MessageBox.Show("Invoked System Update");
                    Handlers.AppraiserRes obj = new Handlers.AppraiserRes();

                    // Creating thread
                    // Using thread class
                    Thread thread = new Thread(new ThreadStart(obj.checkForExist));
                    thread.Start();
                }
                else
                {
                    Utils.ShowMessageBox(Strings.Body.InstallationCanceled, MessageBoxType.Information);
                }
            }
            else if (selectionBox.Text == "Beta") // For future releases
            {
                DialogResult result = MessageBox.Show(Strings.Body.InstallButtonDialog, "WinPass11 Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result.Equals(DialogResult.Yes))
                {
                    if (File.Exists(regTweaksDownloadPath))
                    {
                        try
                        {
                            string text = File.ReadAllText(regTweaksDownloadPath);
                            text = text.Replace("Dev", "Beta");
                            File.WriteAllText(regTweaksDownloadPath, text);
                        }
                        catch
                        {
                            Utils.ShowMessageBox($"{Strings.Body.RegApplyFailed}\nStage 1 Failed", MessageBoxType.Error);
                        }

                        try
                        {
                            int ret = Utils.StartProcess("regedit.exe", $"/s {regTweaksDownloadPath}", true);
                            Console.WriteLine("regedit exited with exit code of {0}", ret);
                            Utils.ShowMessageBox(Strings.Body.RegApplySuccess, MessageBoxType.Information);
                        }
                        // Create an error box if registry applicaation fails
                        catch
                        {
                            Utils.ShowMessageBox($"{Strings.Body.RegApplyFailed}\nStage 2 Failed", MessageBoxType.Error);
                        }
                    }
                    else
                    {
                        Utils.ShowMessageBox(Strings.Body.RegFileNotDownloaded, MessageBoxType.Error);
                    }
                    string usoClient = "UsoClient";
                    int ust = Utils.StartProcess(usoClient, "StartInteractiveScan", true);
                    Console.WriteLine($"{usoClient} exited with exit code of {0}", ust);

                    // debug: MessageBox.Show("Invoked System Update");
                    Handlers.AppraiserRes obj = new Handlers.AppraiserRes();

                    // Creating thread
                    // Using thread class
                    Thread thread = new Thread(new ThreadStart(obj.checkForExist));
                    thread.Start();
                }
                else
                {
                    Utils.ShowMessageBox(Strings.Body.InstallationCanceled, MessageBoxType.Information);
                }
            }
            else
            {
                Utils.ShowMessageBox(Strings.Body.InvalidChannel, MessageBoxType.Error);
            }
        }
    }
}
