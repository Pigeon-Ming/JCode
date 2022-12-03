using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JCode.Model
{
    public class LocalFile
    {
        public string? FileLocation { get; set; }
        public string? FileContent { get; set; }
        public string? Name { get; set; }
        public bool ischanged { get; set; }
    }

    public class LocalFileManager
    {
        public static bool OpenFile(LocalFile file)
        {
            
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            
            openFileDialog.Filter = "c语言文件(*.c)|*.c|All files (*.*)|*.*";
            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                file.Name = openFileDialog.FileName.Substring(openFileDialog.FileName.LastIndexOf("\\")+1, openFileDialog.FileName.Length- openFileDialog.FileName.LastIndexOf("\\")-1);
                file.FileLocation = openFileDialog.FileName;
                file.FileContent = File.ReadAllText(openFileDialog.FileName); ;
                file.ischanged = false;
                return true;
            }
            return false;

        }

        public static bool SaveFile(LocalFile file)
        {
            


            if (string.IsNullOrEmpty(file.FileLocation))
            {
                SaveFileAs(file);
            }
            else
            {
                File.WriteAllText(file.FileLocation, file.FileContent);
                return true;
            }
            return false;
        }

        public static bool SaveFileAs(LocalFile file)
        {
            Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
            saveFileDialog.Filter = "c语言文件(*.c)|*.c|All files (*.*)|*.*";
            if (file.Name!=null)
            {
                saveFileDialog.FileName = file.Name;
            }
            if (saveFileDialog.ShowDialog() == true)
            {

                File.WriteAllText(saveFileDialog.FileName, file.FileContent);

                return true;
            }
            return false;
        }
    }
}
