using System;
using System.Collections.Generic;
using System.Linq;
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
using JCode.Model;

namespace JCode.OtherWindow
{
    /// <summary>
    /// BuildSettingsWindow.xaml 的交互逻辑
    /// </summary>
    public partial class BuildSettingsWindow : Window
    {
        public BuildSettingsWindow()
        {
            InitializeComponent();
            if (SettingsManager.GetSetting("BuildSettings_StackSize") != "SettingNotFound") StackSize.Text = SettingsManager.GetSetting("BuildSettings_StackSize");
            if (SettingsManager.GetSetting("BuildSettings_MallocSize") != "SettingNotFound") MallocSize.Text = SettingsManager.GetSetting("BuildSettings_MallocSize");
        }

        private void SaveSettings_Click(object sender, RoutedEventArgs e)
        {
            
            SettingsManager.ChangeSetting("BuildSettings_StackSize",StackSize.Text) ;
            SettingsManager.ChangeSetting("BuildSettings_MallocSize",MallocSize.Text) ;
            this.Close();
        }
    }
}
