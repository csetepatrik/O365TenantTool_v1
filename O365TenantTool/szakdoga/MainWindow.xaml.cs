using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Reflection;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace O365TenantTool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static InitialSessionState initialState = InitialSessionState.CreateDefault();
        public static PSCredential credential;
        public static Runspace runspace; 
        public static PowerShell powershell = PowerShell.Create();
        public static PSCommand command = new PSCommand();
        public static Collection<System.Management.Automation.PSObject> result = new Collection<PSObject>();
        public static string logged_username = "";
        public static int vmi = 1;
        public MainWindow()
        {
            InitializeComponent();
        }       
        private void LoginClick(object sender, RoutedEventArgs e)
        {
            result = LogIn(result, powershell);
        }
        public Collection<PSObject> LogIn(Collection<PSObject> result, PowerShell powershell) {           
            logged_username = username.Text;
            runspace = System.Management.Automation.Runspaces.RunspaceFactory.CreateRunspace(initialState);
            string connectionUri = "https://outlook.office365.com/powershell-liveid/";
            var loginPassword = password.Password;
            SecureString secpassword = new SecureString();
            foreach (char c in loginPassword)
            {
                secpassword.AppendChar(c);
            }

            credential = new PSCredential(username.Text, secpassword);

            command.AddCommand("New-PSSession");
            command.AddParameter("ConfigurationName", "Microsoft.Exchange");
            command.AddParameter("ConnectionUri", new Uri(connectionUri));
            command.AddParameter("Credential", credential);
            command.AddParameter("Authentication", "Basic");
            powershell.Commands = command;


            runspace.Open();
            powershell.Runspace = runspace;
            result = powershell.Invoke();
            if (powershell.Streams.Error.Count > 0 || result.Count != 1)
            {
                label.Content = "Sikertelen bejelentkezés : (";
                var errors = powershell.Streams.Error.ToList();
                foreach (var item in errors)
                {
                    label_hiba.Content += item.ErrorDetails.ToString();
                }
            }
            else
            {
                UserWindow uw = new UserWindow();
                uw.Show();
                this.Close();
            }
            return result;
        }
    }
}
