using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Diagnostics;
using Microsoft.Win32;
using System.Xml;
using System.Text.RegularExpressions;
using System.Reflection;

namespace UT3ServerManager
{
    public partial class Form1 : Form
    {
        // TODO: Add few more options for:
        // configsubdir=
        // ServerDescription=
        // -log=server.log
        // GamePassword=
        // TODO: Add configuration save on exit to a file in the same folder.
        // TODO: Optimize state changes, no need for so much code.
        // TODO: Proper naming of contorls.
        // TODO: Implement regions and better formatting.
        // TODO: Apply nice looking theme.
        // TODO: Refactor to implement better coding standards and naming conventions.
        // TODO: Make form scale well with virtual resolution.
        // TODO: Move all functions to seprate classes (one for general system calls and one for game specific).

        string GamePath;
        string MapName = "";
        string GameSpyLogin = "";
        string GameSpyPassword = "";
        string AdminPassword = "th3reisn0time";
        string NoHomeDir = " -nohomedir";
        string Unattended = "";
        string GameOverride = "UTGame.UTDeathmatch";
        string Mutators = "";
        string ServerParameters;
        int VoteDuration = 45;
        int GameMode = 0;
        int NumPlay = 10;
        int BotSkill = 4;
        int ServerPort = 40777;
        int MaxPlayers = 32;
        int MinNetPlayers = 1;
        int TimeLimit = 50;
        int GoalScore = 100;
        bool AllowMapVoting = true;
        bool AllowInvites = true;
        bool ShouldAdvertise = false;
        bool LanMatch = true;
        bool AllowJoinInProgress = true;
        bool UsesArbitration = false;
        bool UsesStats = false;
        bool UsesPresence = false;
        bool AllowJoinViaPresence = true;
        bool ForceRespawn = true;
        bool PureServer = false;
        bool Dedicated = true;
        double VsBots = 1.0;
 

        public Form1()
        {
            InitializeComponent();
        }

        private void log(string message, string type)
        {
            DateTime now = DateTime.Now;
            switch (type)
            {
                case "INFO":
                    richTextBox1.AppendText("[INFO][" + now.ToString("HH:mm:ss-dd.mm.yyyy") + "]: " + message + Environment.NewLine);
                    break;
                case "ERR":
                    richTextBox1.AppendText("[ERROR][" + now.ToString("HH:mm:ss-dd.mm.yyyy") + "]: " + message + Environment.NewLine);
                    break;
                case "WARN":
                    richTextBox1.AppendText("[WARNING][" + now.ToString("HH:mm:ss-dd.mm.yyyy") + "]: " + message + Environment.NewLine);
                    break;
            }
            richTextBox1.ScrollToCaret();
        }

        public string search_Steam()
        {
            List<string> steamGameDirs = new List<string>();
            string steam32 = "SOFTWARE\\VALVE\\";
            string steam64 = "SOFTWARE\\Wow6432Node\\Valve\\";
            string steam32path;
            string steam64path;
            string config32path;
            string config64path;
            RegistryKey key32 = Registry.LocalMachine.OpenSubKey(steam32);
            RegistryKey key64 = Registry.LocalMachine.OpenSubKey(steam64);
            if (key64.ToString() == null || key64.ToString() == "")
            {
                foreach (string k32subKey in key32.GetSubKeyNames())
                {
                    using (RegistryKey subKey = key32.OpenSubKey(k32subKey))
                    {
                        steam32path = subKey.GetValue("InstallPath").ToString();
                        config32path = steam32path + "/steamapps/libraryfolders.vdf";
                        string driveRegex = @"[A-Z]:\\";
                        if (File.Exists(config32path))
                        {
                            string[] configLines = File.ReadAllLines(config32path);
                            foreach (var item in configLines)
                            {
                                Match match = Regex.Match(item, driveRegex);
                                if (item != string.Empty && match.Success)
                                {
                                    string matched = match.ToString();
                                    string item2 = item.Substring(item.IndexOf(matched));
                                    item2 = item2.Replace("\\\\", "\\");
                                    item2 = item2.Replace("\"", @"\\steamapps\\common\\Unreal Tournament 3");
                                    if (game_Executable_Exists(item2)) return item2;
                                }
                            }
                            string buffer = steam32path + @"\steamapps\common\Unreal Tournament 3";
                            if (game_Executable_Exists(buffer)) return buffer;
                        }
                    }
                }
            }
            foreach (string k64subKey in key64.GetSubKeyNames())
            {
                using (RegistryKey subKey = key64.OpenSubKey(k64subKey))
                {
                    steam64path = subKey.GetValue("InstallPath").ToString();
                    config64path = steam64path + "/steamapps/libraryfolders.vdf";
                    string driveRegex = @"[A-Z]:\\";
                    if (File.Exists(config64path))
                    {
                        string[] configLines = File.ReadAllLines(config64path);
                        foreach (var item in configLines)
                        {
                            //Console.WriteLine("64:  " + item);
                            Match match = Regex.Match(item, driveRegex);
                            if (item != string.Empty && match.Success)
                            {
                                string matched = match.ToString();
                                string item2 = item.Substring(item.IndexOf(matched));
                                item2 = item2.Replace("\\\\", "\\");
                                item2 = item2.Replace("\"", "\\steamapps\\common\\Unreal Tournament 3");
                                if (game_Executable_Exists(item2)) return item2;
                            }
                        }
                        string buffer = steam64path + @"\steamapps\common\Unreal Tournament 3";
                        if (game_Executable_Exists(buffer)) return buffer;
                    }
                }
            }
            return "";
        }
        private static string get_IP()
        {
            using (WebClient client = new WebClient())
            {
                List<String> hosts = new List<String>();
                hosts.Add("https://ip.rso.bg");
                hosts.Add("https://icanhazip.com");
                hosts.Add("https://api.ipify.org");
                hosts.Add("https://ipinfo.io/ip");
                hosts.Add("https://wtfismyip.com/text");
                hosts.Add("https://checkip.amazonaws.com/");
                hosts.Add("https://bot.whatismyipaddress.com/");
                hosts.Add("https://ipecho.net/plain");
                foreach (String host in hosts)
                {
                    try
                    {
                        String ipAdressString = client.DownloadString(host);
                        ipAdressString = ipAdressString.Replace("\n", "");
                        return ipAdressString;
                    }
                    catch
                    {
                        // TODO:
                    }
                }
            }
            return "ERRORGETTINGIP";
        }
        private void button1_Click(object sender, EventArgs e)
        {
            start_Server();
        }

        private void kill_Server()
        {
            log("Stopping all UT3 servers...", "INFO");
            try
            {
                foreach (var process in Process.GetProcessesByName("UT3"))
                {
                    if (process.MainWindowTitle.Contains(" players)")) process.Kill();
                }
            } 
            catch (Exception ex)
            {
                log("Killing server: " + ex.Message, "ERR");
            }
        }

        private void start_Server()
        {
            log("Starting UT3 server...", "INFO");
            ServerParameters = "server " + MapName + "?" + "numplay=" + NumPlay + "?maxplayers=" + MaxPlayers + "?timelimit=" + TimeLimit + "?goalscore=" + GoalScore +
                "?vsbots=" + VsBots + "?forcerespawn=" + ForceRespawn + "?GameMode=" + GameMode + "?PureServer=" + PureServer + "?minnetplayers=" + MinNetPlayers +
                "?bIsDedicated=" + Dedicated + "?bUsesArbitration=" + UsesArbitration + "?bAllowJoinViaPresence=" + AllowJoinViaPresence + "?bUsesPresence=" + UsesPresence +
                "?bAllowInvites=" + AllowInvites + "?bAllowJoinInProgress=" + AllowJoinInProgress + "?bUsesStats=" + UsesStats + "?bIsLanMatch=" + LanMatch +
                "?bShouldAdvertise=" + ShouldAdvertise + "?bAllowJoinInProgress=" + AllowJoinInProgress + "?bUsesStats=" + UsesStats + "?bIsLanMatch=" + LanMatch +
                "?AdminPassword=" + AdminPassword + "?game=" + GameOverride + Mutators +
                "?botskill=" + BotSkill + GameSpyLogin + GameSpyPassword + " -port=" + ServerPort + NoHomeDir + Unattended;
            textBox5.Text = ServerParameters;
            textBox12.Text = "open 127.0.0.1:" + ServerPort + "?password=" + AdminPassword;
            textBox13.Text = "open " + get_IP() + ":" + ServerPort;
            ProcessStartInfo startInfo = new ProcessStartInfo(GamePath + @"\Binaries\UT3.exe");
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = ServerParameters;
            Process.Start(startInfo);
        }

        public string GetInstallPath(string applicationName)
        {
            var installPath = FindApplicationPath(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall", applicationName);
            installPath = FindApplicationPath(@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall", applicationName);
            return installPath;
        }

        private string FindApplicationPath(string keyPath, string applicationName)
        {
            var hklm = Registry.LocalMachine;
            var uninstall = hklm.OpenSubKey(keyPath);
            Console.WriteLine("Checking in: " + keyPath);
            foreach (var productSubKey in uninstall.GetSubKeyNames())
            {
                var product = uninstall.OpenSubKey(productSubKey);

                var displayName = product.GetValue("DisplayName");
                
                if ((string)displayName != "")
                {
                    Console.WriteLine(displayName);
                    return product.GetValue("InstallLocation").ToString();
                }

            }

            return null;
        }

        private string search_SteamRegistry()
        {
            string buffer = "";
            using (RegistryKey hklm = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
            {
                using (RegistryKey registryKey = hklm.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 13210"))
                {
                    if (registryKey != null) buffer = (string)registryKey.GetValue("InstallLocation");
                }
            }
            if (game_Executable_Exists(buffer)) return buffer;
            return "";
        }

        private string search_StandaloneFolders()
        {
            List<string> locations = new List<string>(new string[] { @"C:\Program Files (x86)\Unreal Tournament 3", @"C:\Program Files (x86)\Epic Games\Unreal Tournament 3", @"C:\Unreal Tournament 3", @"C:\Games\Unreal Tournament 3", @"C:\Games\UT3", @"D:\Games\Unreal Tournament 3", @"D:\Games\UT3" });
            for (var i = 0; i < locations.Count; i++)
            {
                if (game_Executable_Exists(locations[i])) return locations[i];
            }
            return "";
        }

        private void get_Install_Directory()
        {
            log("Searching for game directory...", "INFO");

            string UT3Directory = string.Empty;

            // Search standard Steam registry
            UT3Directory = search_SteamRegistry();
            if (UT3Directory.Length > 0)
            {
                log("Found game folder in registry " + UT3Directory, "INFO");
                GamePath = UT3Directory;
                textBox15.Text = GamePath;
                enable_Controls();
                load_Maps();
                return;
            }

            // Search all Steam libraries
            UT3Directory = search_Steam();
            if (UT3Directory.Length > 0)
            {
                log("Found game folder in Steam " + UT3Directory, "INFO");
                GamePath = UT3Directory;
                textBox15.Text = GamePath;
                enable_Controls();
                load_Maps();
                return;
            }

            // Search standard standalone folders
            UT3Directory = search_StandaloneFolders();
            if (UT3Directory.Length > 0)
            {
                log("Found game folder in standard folders " + UT3Directory, "INFO");
                GamePath = UT3Directory;
                textBox15.Text = GamePath;
                enable_Controls();
                load_Maps();
                return;
            }

            // Installed programs
            /* string registry_key = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall";
             using (Microsoft.Win32.RegistryKey key = Registry.LocalMachine.OpenSubKey(registry_key))
             {
                 foreach (string subkey_name in key.GetSubKeyNames())
                 {
                     using (RegistryKey subkey = key.OpenSubKey(subkey_name))
                     {
                         Console.WriteLine(subkey.GetValue("DisplayName"));
                     }
                 }
             }*/



            // Check steam
            // ask user 
            log("Could not find game folder in standard locations, please select it manually.", "ERR");
        }

        private bool game_Executable_Exists(string GameFolder)
        {
            if (File.Exists(GameFolder + @"\Binaries\UT3.exe")) return true;
            return false;
        }
        private void enable_Controls()
        {
            groupBox1.Enabled = true;
            groupBox2.Enabled = true;
            groupBox3.Enabled = true;
            button6.Enabled = true;
            button1.Enabled = true;
            button3.Enabled = true;
            button5.Enabled = true;
            button4.Enabled = true;
            button7.Enabled = true;
            button13.Enabled = true;
            button14.Enabled = true;
            button15.Enabled = true;
            textBox5.Enabled = true;
            textBox12.Enabled = true;
            textBox13.Enabled = true;
            textBox15.BackColor = SystemColors.Control;
            button11.BackColor = SystemColors.Control;
            textBox15.Select(0, 0);
            this.Focus();
        }

        private void disable_Controls()
        {
            groupBox1.Enabled = false;
            groupBox2.Enabled = false;
            groupBox3.Enabled = false;
            button6.Enabled = false;
            button1.Enabled = false;
            button3.Enabled = false;
            button5.Enabled = false;
            button4.Enabled = false;
            button7.Enabled = false;
            button13.Enabled = false;
            button14.Enabled = false;
            button15.Enabled = false;
            textBox5.Enabled = false;
            textBox12.Enabled = false;
            textBox13.Enabled = false;
            textBox15.BackColor = Color.LightGreen;
            button11.BackColor = Color.LightGreen;
            textBox15.Select(0, 0);
            this.Focus();
        }

        private void load_Maps()
        {
            try
            {
                comboBox3.Items.Clear();
                // Original maps
                string[] allMaps = Directory.GetFiles(GamePath + @"\UTGame\CookedPC\Maps", "*.ut3", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < allMaps.Length; i++)
                {
                    comboBox3.Items.Add(Path.GetFileNameWithoutExtension(allMaps[i]));
                }
                // Private maps
                allMaps = Directory.GetFiles(GamePath + @"\UTGame\CookedPC\Private\Maps", "*.ut3", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < allMaps.Length; i++)
                {
                    comboBox3.Items.Add(Path.GetFileNameWithoutExtension(allMaps[i]));
                }
                // Private UT3G
                allMaps = Directory.GetFiles(GamePath + @"\UTGame\CookedPC\UT3G\Maps", "*.ut3", SearchOption.TopDirectoryOnly);
                for (int i = 0; i < allMaps.Length; i++)
                {
                    comboBox3.Items.Add(Path.GetFileNameWithoutExtension(allMaps[i]));
                }
                // Custom maps
                var myDocs = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents");
                myDocs += @"\My Games\Unreal Tournament 3\UTGame\Unpublished\CookedPC\CustomMaps";
                if (Directory.Exists(myDocs))
                {
                    allMaps = Directory.GetFiles(myDocs, "*.ut3", SearchOption.TopDirectoryOnly);
                    for (int i = 0; i < allMaps.Length; i++)
                    {
                        comboBox3.Items.Add(Path.GetFileNameWithoutExtension(allMaps[i]));
                    }
                }
                comboBox3.SelectedIndex = 0;
                MapName = comboBox3.Text;
            } 
            catch (Exception ex)
            {
                log("Problem loading maps: " + ex.Message, "ERR");
            }

        }
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            AllowMapVoting = checkBox1.Checked;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            disable_Controls();
            log("UT3SM loaded successfully.", "INFO");
            get_Install_Directory();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            VoteDuration = int.Parse(textBox1.Text);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            GameMode = comboBox1.SelectedIndex;
            switch (GameMode)
            {
                case 0:
                    GameOverride = "UTGame.UTDeathmatch";
                    break;
                case 1:
                    GameOverride = "UTGameContent.UTCTFGame_Content";
                    break;
                case 2:
                    GameOverride = "UTGameContent.UTOnslaughtGame_Content";
                    break;
                case 3:
                    GameOverride = "UTGameContent.UTVehicleCTFGame_Content";
                    break;
                case 4:
                    GameOverride = "UTGame.UTTeamGame";
                    break;
                case 5:
                    GameOverride = "UTGame.UTDuelGame";
                    break;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ForceRespawn = checkBox2.Checked;
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            PureServer = checkBox3.Checked;
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            TimeLimit = int.Parse(textBox3.Text);
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            GoalScore = int.Parse(textBox4.Text);
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            Dedicated = checkBox4.Checked;
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox6.Text.Length > 0)
            {
                GameSpyLogin = " -login=" + textBox6.Text;
            }
            else
            {
                GameSpyLogin = "";
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (textBox7.Text.Length > 0)
            {
                GameSpyPassword = " -password=" + textBox7.Text;
            }
            else
            {
                GameSpyPassword = "";
            }
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            AllowInvites = checkBox5.Checked;
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            LanMatch = checkBox6.Checked;
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            UsesArbitration = checkBox7.Checked;
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            AllowJoinInProgress = checkBox8.Checked;
        }

        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            ShouldAdvertise = checkBox9.Checked;
        }

        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            UsesStats = checkBox10.Checked;
        }

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            MinNetPlayers = int.Parse(textBox8.Text);
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            VsBots = double.Parse(textBox9.Text);
        }

        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
              (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            ServerPort = int.Parse(textBox10.Text);
        }

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            load_Maps();
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            MapName = comboBox3.Text;
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            MaxPlayers = int.Parse(textBox11.Text);
        }

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox5.Text);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox12.Text);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox13.Text);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            kill_Server();
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            AdminPassword = textBox14.Text;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            show_Message("AdminCommands.txt");
        }
        private void show_Message(string fileName)
        {
            try
            {
                Form info = new Message();
                RichTextBox message = (RichTextBox)info.Controls.Find("richTextBox1", true)[0];
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = assembly.GetManifestResourceNames().Single(str => str.EndsWith(fileName));
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (StreamReader reader = new StreamReader(stream))
                {
                    string result = reader.ReadToEnd();
                    message.Text = result;
                    info.Show();
                }
            }
            catch (Exception ex)
            {
                log("When opening a message windows: " + ex.Message, "WARN");
            }
        }
        private void button8_Click(object sender, EventArgs e)
        {
            show_Message("PortInfo.txt");
        }

        private void checkBox11_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox11.Checked)
            {
                NoHomeDir = " -nohomedir";
            }
            else
            {
                NoHomeDir = "";
            }
        }

        private void checkBox12_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox12.Checked)
            {
                Unattended = " -unattended";
            }
            else
            {
                Unattended = "";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            MessageBox.Show(@"Gamemode will also by default override the game parameter and force itself. This will give you the abiltiy to play DM on a WAR map for example.", "Gamemodes");
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            notifyIcon1.Visible = false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            hide_GUI();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            show_GUI();
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void hide_GUI()
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Hide();
            }
        }

        private void show_GUI()
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
        }

        private void showGUIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            show_GUI();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void killServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            kill_Server();
        }

        private void startServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            start_Server();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
        }

        private void notifyIcon1_MouseClick(object sender, EventArgs e)
        {
 
        }

        private void notifyIcon1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show();
            }
            else
            { 
            }
        }

        private void update_Mutator()
        {
            string buffer = "";
            if (checkBox13.Checked) buffer += "UTGame.UTMutator_Instagib,";
            if (checkBox14.Checked) buffer += "UTGame.UTMutator_BigHead,";
            if (checkBox15.Checked) buffer += "UTGame.UTMutator_LowGrav,";
            if (checkBox16.Checked) buffer += "UTGame.UTMutator_SuperBerserk,";
            if (checkBox17.Checked) buffer += "UTGame.UTMutator_FriendlyFire,";
            if (checkBox18.Checked) buffer += "UTGame.UTMutator_NoTranslocator,";
            if (checkBox19.Checked) buffer += "UTGame.UTMutator_SpeedFreak,";
            if (checkBox20.Checked) buffer += "UTGame.UTMutator_Handicap,";
            if (checkBox21.Checked) buffer += "UTGame.UTMutator_NoPowerups,";
            if (checkBox22.Checked) buffer += "UTGame.UTMutator_Slomo,";
            if (checkBox23.Checked) buffer += "UTGame.UTMutator_WeaponReplacement,";
            if (checkBox24.Checked) buffer += "UTGame.UTMutator_WeaponsRespawn";
            buffer = buffer.TrimEnd(',');
            buffer = "?mutator=" + buffer;
            Mutators = buffer;
        }
        private void checkBox13_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void checkBox14_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void checkBox15_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void checkBox16_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void checkBox17_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void checkBox18_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void checkBox19_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void checkBox20_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void checkBox21_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void checkBox22_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void checkBox23_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void checkBox24_CheckedChanged(object sender, EventArgs e)
        {
            update_Mutator();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            show_Message("LANPlay.txt");
        }

        private void button11_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    if (game_Executable_Exists(fbd.SelectedPath))
                    {
                        GamePath = fbd.SelectedPath;
                        textBox15.Text = GamePath;
                        enable_Controls();
                        load_Maps();
                    }
                    else
                    {
                        log(@"Can't find Binaries\UT3.exe in your folder. Please try again.", "ERR");
                        MessageBox.Show(@"Can't find Binaries\UT3.exe in your folder. Please try again.", "Error!");
                    }

                }
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(richTextBox1.Text);
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            BotSkill = trackBar1.Value;
            label2.Text = "Bot skill (" + BotSkill + "/7)";
        }

        private void start_Game()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(GamePath + @"\Binaries\UT3.exe");
            startInfo.WindowStyle = ProcessWindowStyle.Normal;
            startInfo.Arguments = "-nostartupmovies";
            Process.Start(startInfo);
        }
        private void button14_Click(object sender, EventArgs e)
        {
            start_Game();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            Process.Start(GamePath);
        }

        private void button15_Click(object sender, EventArgs e)
        {
            show_Message("UsefulResources.txt");
        }

        private void button16_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/thereisnotime/UT3ServerManager");
        }

        private void startGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            start_Game();
        }

        private void button17_Click(object sender, EventArgs e)
        {
            Process.Start(GamePath + @"\UTGame\CookedPC\Maps");
        }

        private void button18_Click(object sender, EventArgs e)
        {
            Process.Start(GamePath + @"\UTGame\CookedPC\Private\Maps");
        }

        private void button19_Click(object sender, EventArgs e)
        {
            Process.Start(GamePath + @"\UTGame\CookedPC\UT3G\Maps");
        }

        private void button20_Click(object sender, EventArgs e)
        {
            var myDocs = Path.Combine(Environment.ExpandEnvironmentVariables("%userprofile%"), "Documents");
            myDocs += @"\My Games\Unreal Tournament 3\UTGame\Unpublished\CookedPC\CustomMaps";
            Process.Start(myDocs);
        }
    }
}
