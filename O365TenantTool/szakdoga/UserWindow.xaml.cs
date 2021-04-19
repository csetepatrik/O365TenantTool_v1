using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace O365TenantTool
{
    /// <summary>
    /// Interaction logic for UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {

        public static string txt_user = "";
        public static List<string> users = new List<string>();
        public static Dictionary<string,string> customCommands = new Dictionary<string,string>();
        public UserWindow()
        {
            
            InitializeComponent();
            felhasznalo.Content = MainWindow.logged_username;          
            var vmi = MainWindow.logged_username.Split('@');
            txt_user = vmi[0];
            if (File.Exists(@"C:\" + txt_user + ".txt"))
            {
                StreamReader sr = new StreamReader(@"C:\" + txt_user + ".txt",Encoding.UTF8);

                while (!sr.EndOfStream)
                {
                    var splits = sr.ReadLine().Split(';');
                    myCommands.Items.Add(splits[1]);
                    customCommands.Add(splits[1], splits[2]);
                }
                sr.Close();
            }


        }

        private void GetAllUser(object sender, RoutedEventArgs e)
        {
            label.Items.Clear();
            
            MainWindow.powershell = PowerShell.Create();
            MainWindow.command = new PSCommand();
            MainWindow.command.AddCommand("Invoke-Command");
            MainWindow.command.AddParameter("ScriptBlock", System.Management.Automation.ScriptBlock.Create("Get-User"));
            MainWindow.command.AddParameter("Session", MainWindow.result[0]);
            MainWindow.powershell.Commands = MainWindow.command;
            MainWindow.powershell.Runspace = MainWindow.runspace;
            MainWindow.powershell.Invoke();
            Collection<PSObject> result = MainWindow.powershell.Invoke();

            if (MainWindow.powershell.HadErrors)
            {
                result_label.Content = "";
                result_label.Content = "Sikertelen Végrehajtás";
                try
                {
                    var errors = MainWindow.powershell.Streams.Error.ToList();

                    foreach (var item in errors)
                    {
                        result_label.Content += item.ErrorDetails.ToString();
                    }
                }
                catch (Exception ex)
                {

                    result_label.Content += ex.Message;
                }

            }
            else
            {
                result_label.Content = "";
                result_label.Content += "Sikeres Végrehajtás!";
            }
            foreach (var item in result)
            {
                label.Items.Add(item.ToString());
            }
            MainWindow.powershell = PowerShell.Create();
            MainWindow.command = new PSCommand();
            MainWindow.command.AddCommand("Invoke-Command");
            MainWindow.command.AddParameter("ScriptBlock", System.Management.Automation.ScriptBlock.Create(@"$ExecutionContext.SessionState.LanguageMode = ConstrainedLanguage"));
            MainWindow.command.AddParameter("Session", MainWindow.result[0]);
            MainWindow.powershell.Commands = MainWindow.command;
            MainWindow.powershell.Runspace = MainWindow.runspace;         
            MainWindow.powershell.Invoke();
            var result1 = MainWindow.powershell.Invoke();
            

        }

        private void RemoveActualSession(object sender, RoutedEventArgs e)
        {
            MainWindow.powershell = PowerShell.Create();
            MainWindow.command = new PSCommand();
            MainWindow.command.AddCommand("Invoke-Command");
            MainWindow.command.AddParameter("ScriptBlock", System.Management.Automation.ScriptBlock.Create("Remove-PSSession -Session (Get-PSSession)"));
            MainWindow.powershell.Commands = MainWindow.command;
            MainWindow.powershell.Runspace = MainWindow.runspace;
            MainWindow.powershell.Invoke();

            
        }

        private void GetAllGroup(object sender, RoutedEventArgs e)
        {
            label.Items.Clear();
            MainWindow.powershell = PowerShell.Create();
            MainWindow.command = new PSCommand();
            MainWindow.command.AddCommand("Invoke-Command");
            MainWindow.command.AddParameter("ScriptBlock", System.Management.Automation.ScriptBlock.Create("get-group"));
            MainWindow.command.AddParameter("Session", MainWindow.result[0]);
            MainWindow.powershell.Commands = MainWindow.command;
            MainWindow.powershell.Runspace = MainWindow.runspace;
            var result = MainWindow.powershell.Invoke();
            if (result.Count > 0)
            {
                result_label.Content = "Sikeres végrehajtás!";
            }
            else
            {
                result_label.Content = "Sajnos valamelyik adat nem megfelelő!";
            }
            foreach (var item in result)
            {
                label.Items.Add(item.ToString());
            }

        }

        private void OpenAddUserUnifGroupWindow(object sender, RoutedEventArgs e)
        {
            AddUsertoUnifiedGroup autg = new AddUsertoUnifiedGroup();
            autg.Show();
            
        }

        private void OpenRemoveUserfromGroupWindow(object sender, RoutedEventArgs e)
        {
            RemoveUserFromGroup autg = new RemoveUserFromGroup();
            autg.Show();
        }

        private void GetDnsDetails(object sender, RoutedEventArgs e)
        {
            label.Items.Clear();
            MainWindow.powershell = PowerShell.Create();
            MainWindow.command = new PSCommand();
            MainWindow.command.AddCommand("Invoke-Command");
            MainWindow.command.AddParameter("ScriptBlock", System.Management.Automation.ScriptBlock.Create("Get-DnsClientServerAddress"));
            MainWindow.command.AddParameter("Session", MainWindow.result[0]);
            MainWindow.powershell.Commands = MainWindow.command;
            MainWindow.powershell.Runspace = MainWindow.runspace;
            Collection<PSObject> result = MainWindow.powershell.Invoke();

            if (MainWindow.powershell.HadErrors)
            {
                label.Items.Clear();
                label.Items.Add("Sikertelen Végrehajtás");
                try
                {
                    var errors = MainWindow.powershell.Streams.Error.ToList();

                    foreach (var item in errors)
                    {
                        label.Items.Add(item.ErrorDetails.ToString());
                    }
                }
                catch (Exception ex)
                {

                    label.Items.Add(ex.Message);
                }

            }
            else
            {
                label.Items.Clear();
               label.Items.Add("Sikeres Végrehajtás!");
            }
        }

        private void OpenGetGroupMembersWindow(object sender, RoutedEventArgs e)
        {
            GetGroupMembers gmg = new GetGroupMembers();
            gmg.Show();
        }

        
        private void OpenCreateGroupWindow(object sender, RoutedEventArgs e)
        {
            CreateGroup cg = new CreateGroup();
            cg.Show();

        }

        private void OpenEnableUserWindow(object sender, RoutedEventArgs e)
        {
            EnableUser eu = new EnableUser();
            eu.Show();
        }

        private void OpenDisableUserWindow(object sender, RoutedEventArgs e)
        {
            DisableUser du = new DisableUser();
            du.Show();
        }

        private void OpenResetPasswordWindow(object sender, RoutedEventArgs e)
        {
            ResetPassword rp = new ResetPassword();
            rp.Show();
        }

        private void OpenCreateUserWindow(object sender, RoutedEventArgs e)
        {
            CreateUser cu = new CreateUser();
            cu.Show();
        }

        private void OpenAddCalenderPermissionWindow(object sender, RoutedEventArgs e)
        {
            AddCalendarPermission acp = new AddCalendarPermission();
            acp.Show();
        }

        private void OpenRemoveCalendarPermissionWindow(object sender, RoutedEventArgs e)
        {
            RemoveCalendarPermission rcp = new RemoveCalendarPermission();
            rcp.Show();
        }

        private void OpenGetCalendarPermissionWindow(object sender, RoutedEventArgs e)
        {
            GetCalendarPermission gcp = new GetCalendarPermission();
            gcp.Show();
        }

        private void OpenCreateDistrGroupWindow(object sender, RoutedEventArgs e)
        {
            CreateDistributionGroup cdg = new CreateDistributionGroup();
            cdg.Show();
        }

        private void OpenAddToDistrGroupWindow(object sender, RoutedEventArgs e)
        {
            AddToDistributionGroup adg = new AddToDistributionGroup();
            adg.Show();
        }

        private void OpenPowerShellWindow(object sender, RoutedEventArgs e)
        {
            OpenPowerShell(@"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe");
        }

        static void OpenPowerShell(string path)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(path);
            startInfo.UseShellExecute = false;
            startInfo.EnvironmentVariables.Add("RedirectStandardOutput", "true");
            startInfo.EnvironmentVariables.Add("RedirectStandardError", "true");
            startInfo.EnvironmentVariables.Add("UseShellExecute", "false");
            startInfo.EnvironmentVariables.Add("CreateNoWindow", "true");
            Process.Start(startInfo);
        }

        private void LoadMyCommands(object sender, MouseButtonEventArgs e)
        {
            var vmi = myCommands.SelectedItem.ToString() ;
            var first = customCommands.First(x => x.Key == vmi).Value;

            MainWindow.powershell = PowerShell.Create();
            MainWindow.command = new PSCommand();
            MainWindow.command.AddCommand("Invoke-Command");
            MainWindow.command.AddParameter("ScriptBlock", System.Management.Automation.ScriptBlock.Create(""+first+""));
            MainWindow.command.AddParameter("Session", MainWindow.result[0]);
            MainWindow.powershell.Commands = MainWindow.command;
            MainWindow.powershell.Runspace = MainWindow.runspace;
            var result = MainWindow.powershell.Invoke();
            label.Items.Clear();
            foreach (var item in result)
            {
                label.Items.Add(item.ToString());
            }
        }

        private void SaveMyCustomCommand(object sender, RoutedEventArgs e)
        {
            if (File.Exists(@"C:\" + txt_user + ".txt"))
            {
                foreach (var item in customCommands.Keys)
                {
                    if (item == command_name.Text)
                    {
                        MessageBox.Show("Már létezik ilyen parancs");
                            break;
                    }
                    else
                    {
                        try
                        {
                            myCommands.Items.Add(command_name.Text);
                            customCommands.Add(command_name.Text, new_command.Text);
                            StreamWriter sw = new StreamWriter(@"C:\" + txt_user + ".txt", true);
                            sw.WriteLine(txt_user + ";" + command_name.Text + ";" + new_command.Text);
                            sw.Close();
                            break;
                        }
                        catch (Exception)
                        {

                            MessageBox.Show("Sajnos valamit hibásan adtál meg!");
                            break;
                        }
                    }
                }
            }
            else
            {
                myCommands.Items.Add(command_name.Text);
                customCommands.Add(command_name.Text, new_command.Text);
                StreamWriter sw = new StreamWriter(@"C:\" + txt_user + ".txt", true);
                sw.WriteLine(txt_user + ";" + command_name.Text + ";" + new_command.Text);
                sw.Close();
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            MainWindow.powershell = PowerShell.Create();
            MainWindow.command = new PSCommand();
            MainWindow.command.AddCommand("Invoke-Command");
            MainWindow.command.AddParameter("ScriptBlock", System.Management.Automation.ScriptBlock.Create("Remove-PSSession -Session (Get-PSSession)"));
            MainWindow.powershell.Commands = MainWindow.command;
            MainWindow.powershell.Runspace = MainWindow.runspace;
            MainWindow.powershell.Invoke();
        }

        private void OpenGetDistrGroupMemberWindow(object sender, RoutedEventArgs e)
        {
            GetDistGroupMember gd = new GetDistGroupMember();
            gd.Show();
        }

        private void OpenRemoveUserFromDistrWindow(object sender, RoutedEventArgs e)
        {
            RemoveUserFromDistr ru = new RemoveUserFromDistr();
            ru.Show();
        }

        private void GetAllDistrGroup(object sender, RoutedEventArgs e)
        {
            label.Items.Clear();
            MainWindow.powershell = PowerShell.Create();
            MainWindow.command = new PSCommand();
            MainWindow.command.AddCommand("Invoke-Command");
            MainWindow.command.AddParameter("ScriptBlock", System.Management.Automation.ScriptBlock.Create("Get-DistributionGroup"));
            MainWindow.command.AddParameter("Session", MainWindow.result[0]);
            MainWindow.powershell.Commands = MainWindow.command;
            MainWindow.powershell.Runspace = MainWindow.runspace;
            MainWindow.powershell.Invoke();
            var result = MainWindow.powershell.Invoke();

            foreach (var item in result)
            {
                label.Items.Add(item.ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            CreateUser cu = new CreateUser();
            cu.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DeleteUser du = new DeleteUser();
            du.Show();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            foreach (var user in label.SelectedItems)
            {
                users.Add(user.ToString());
            }
            ModifyProp mp = new ModifyProp();
            mp.Show();
        }

        private void UserSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                label.Items.Clear();

                MainWindow.powershell = PowerShell.Create();
                MainWindow.command = new PSCommand();
                MainWindow.command.AddCommand("Invoke-Command");
                MainWindow.command.AddParameter("ScriptBlock", System.Management.Automation.ScriptBlock.Create(@"Get-User -Identity " + userSearch.Text +"*"));
                MainWindow.command.AddParameter("Session", MainWindow.result[0]);
                MainWindow.powershell.Commands = MainWindow.command;
                MainWindow.powershell.Runspace = MainWindow.runspace;
                var result = MainWindow.powershell.Invoke();

                foreach (var item in result)
                {
                    label.Items.Add(item.ToString());
                }
            }
        }
    }
}
