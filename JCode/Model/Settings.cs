using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCode.Model
{
    public class Settings
    {
        public string SettingName { get; set; }
        public string SettingContent { get; set; }
    }
    public class SettingsManager
    {
        public static string GetSetting(string SettingName)
        {
            string SettingFilePath = "C:\\Users\\" + System.Environment.UserName + "\\Documents\\JCode\\Settings\\" + SettingName + ".set";
            Debug.WriteLine(SettingFilePath);
            if (File.Exists(SettingFilePath))
            {
                
                return File.ReadAllText(SettingFilePath);

            }
            return "SettingNotFound";
        }

        public static void ChangeSetting(string SettingName,string SettingPath)
        {
            if (string.IsNullOrEmpty(SettingPath)) return;
            string SettingFilePath = "C:\\Users\\" + System.Environment.UserName + "\\Documents\\JCode\\Settings\\" + SettingName+".set";
            File.WriteAllText(SettingFilePath, SettingPath);
        }
    }

    

}
