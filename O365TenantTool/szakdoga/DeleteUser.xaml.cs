using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace O365TenantTool
{
    /// <summary>
    /// Interaction logic for DeleteUser.xaml
    /// </summary>
    public partial class DeleteUser : Window
    {
        public DeleteUser()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.powershell = PowerShell.Create();
            MainWindow.command = new PSCommand();
            MainWindow.command.AddCommand("Invoke-Command");
            MainWindow.command.AddParameter("ScriptBlock", System.Management.Automation.ScriptBlock.Create(@"Remove-Mailbox" + " -Identity " + identity.Text + " -Confirm:$false"));
            MainWindow.command.AddParameter("Session", MainWindow.result[0]);
            MainWindow.powershell.Commands = MainWindow.command;
            MainWindow.powershell.Runspace = MainWindow.runspace;
            Collection<PSObject> result = MainWindow.powershell.Invoke();

            if (MainWindow.powershell.HadErrors)
            {
                result_label.Items.Clear();
                result_label.Items.Add("Sikertelen Végrehajtás");
                try
                {
                    var errors = MainWindow.powershell.Streams.Error.ToList();

                    foreach (var item in errors)
                    {
                        result_label.Items.Add(item.ErrorDetails.ToString());
                    }
                }
                catch (Exception ex)
                {

                    result_label.Items.Add(ex.Message);
                }

            }
            else
            {
                result_label.Items.Clear();
                result_label.Items.Add("Sikeres Végrehajtás!");
            }
        }
    }
}
