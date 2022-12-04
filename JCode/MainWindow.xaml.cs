using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using JCode.Model;
using JCode.OtherWindow;

namespace JCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly string JCodeFolderLocation = "C:\\Users\\" + System.Environment.UserName + "\\Documents\\JCode";
        public readonly string JCodeFolderLocation_Left = "C:/Users/" + System.Environment.UserName + "/Documents/JCode";
        readonly string tempFolderLocation = "C:\\Users\\" + System.Environment.UserName + "\\Documents\\JCode\\temp";
        readonly string tempFolderLocation_Left = "C:/Users/" + System.Environment.UserName + "/Documents/JCode/temp";
        public readonly string SettingsFolderLocation = "C:\\Users\\" + System.Environment.UserName + "\\Documents\\JCode\\Settings\\";
        public readonly string SettingsFolderLocation_Left = "C:/Users/" + System.Environment.UserName + "/Documents/JCode/Settings/";


        //public List<LocalFile> localFiles = new List<LocalFile>();
        int NewFileNameIndex = 0;//未命名0，未命名1……
        bool changingTab = false;

        public MainWindow()
        {
            InitializeComponent();
            
            Directory.CreateDirectory(JCodeFolderLocation);
            Directory.CreateDirectory(JCodeFolderLocation+"\\Compiler");
            Directory.CreateDirectory(JCodeFolderLocation+"\\Jigsaw");
            Directory.CreateDirectory(JCodeFolderLocation+"\\Settings");

            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!ExitJCode())
            {
                e.Cancel = true;
            }
            
        }

        void OpenFile()
        {
            LocalFile file = new LocalFile();
            Debug.WriteLine(LocalFileManager.OpenFile(file));
            if (string.IsNullOrEmpty(file.FileLocation))
            {
                return;
            }
            file.ischanged = false;
            //localFiles.Add(file);
            MainTabControl.Items.Add(file);
            MainTabControl.SelectedItem = file;
            EditControl.Visibility = Visibility.Visible;
        }

        void CreatFile()
        {
            LocalFile file = new LocalFile { Name="未命名"+(++NewFileNameIndex),FileContent="\0",ischanged=false};
            //localFiles.Add(file);
            MainTabControl.Items.Add(file);
            MainTabControl.SelectedItem = file;
            EditControl.Visibility = Visibility.Visible;
        }

        async Task BuildProgram()
        {
            int StackSize = 1024, MallocSize = 64;
            Debug.WriteLine(SettingsManager.GetSetting("BuildSettings_StackSize"));
            if (SettingsManager.GetSetting("BuildSettings_StackSize") != "SettingNotFound") StackSize = Convert.ToInt32(SettingsManager.GetSetting("BuildSettings_StackSize"));
            if (SettingsManager.GetSetting("BuildSettings_MallocSize") != "SettingNotFound") MallocSize = Convert.ToInt32(SettingsManager.GetSetting("BuildSettings_MallocSize"));
            



            Directory.CreateDirectory(tempFolderLocation);
            if (!LocalFileManager.SaveFile((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex])) return;

            FileInfo file = new FileInfo(((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]).FileLocation);
            if (file==null)return;

            file.CopyTo(tempFolderLocation+"\\temp.c", true);
            
            string command = "cd " + tempFolderLocation+'\n';
            command += JCodeFolderLocation + "\\Compiler" + "\\cc1.exe -I../Jigsaw/ -Os -Wall -quiet -o " + tempFolderLocation_Left + "/temp.gas " + tempFolderLocation_Left + "/temp.c\n";
            command += JCodeFolderLocation + "\\Compiler" + "\\gas2nask.exe -a " + tempFolderLocation_Left + "/temp.gas " + tempFolderLocation_Left + "/temp.nas\n";
            command += JCodeFolderLocation + "\\Compiler" + "\\nask.exe " + tempFolderLocation_Left + "/temp.nas " + tempFolderLocation_Left + "/temp.obj " + tempFolderLocation_Left + "/temp.lst\n";
            command += JCodeFolderLocation + "\\Compiler\\obj2bim.exe @" +JCodeFolderLocation_Left + "/Jigsaw/haribote.rul " + "out:" + tempFolderLocation_Left + "/temp.bim " + "map:" + tempFolderLocation_Left + "/temp.map " + "stack:"+StackSize+"k         " + tempFolderLocation_Left + "/temp.obj " + JCodeFolderLocation_Left + "/Jigsaw/apilib/apilib.lib\n";
            command += JCodeFolderLocation + "\\Compiler\\bim2hrb.exe "+ tempFolderLocation_Left+ "/temp.bim " + tempFolderLocation_Left + "/temp.hrb "+MallocSize+"k\n";

            
            OutPut_TextBox.Text = await Cmd.RunCmd(command);
            Debug.WriteLine("？");

            FileStream myStream = new FileStream(tempFolderLocation + "\\temp.hrb", FileMode.Open, FileAccess.Read);
            BinaryReader myReader = new BinaryReader(myStream);
            FileInfo f = new FileInfo(tempFolderLocation + "\\temp.hrb");

            Debug.WriteLine("！");
            if (myReader.PeekChar() != -1)
            {
                byte[] data;

                data = myReader.ReadBytes((int)f.Length);
                
                //string data_Dec="";
                for (int i = 0; i < f.Length; i++)
                {
                    Debug.Write(data[i] + " ");
                }
                //开始更改数据
                //data[0] = 0;
                //data[1] = 48;

                data[4] = 72;//H
                data[5] = 97;//a
                data[6] = 114;//r
                data[7] = 105;//i
                

                string cFileLocation = ((LocalFile)MainTabControl.SelectedItem).FileLocation;
                cFileLocation = cFileLocation.Substring(0, cFileLocation.LastIndexOf("\\"));


                string hrbFileName = ((LocalFile)MainTabControl.SelectedItem).Name;
                hrbFileName = hrbFileName.Substring(0,hrbFileName.LastIndexOf("."));

                Debug.WriteLine(cFileLocation +"\\" +hrbFileName + ".hrb");


                //Directory.Delete(cFileLocation + "\\" + hrbFileName + ".hrb", true);

                FileStream fileStream = new FileStream(cFileLocation + "\\" + hrbFileName + ".hrb", FileMode.OpenOrCreate, FileAccess.Write);
                fileStream.Write(data, 0, data.Length);
                fileStream.Close();
                if (false)
                {
                    //FileStream myStream = new FileStream(tempFolderLocation+"\\temp.bim", FileMode.Open, FileAccess.Read);
                    //BinaryReader myReader = new BinaryReader(myStream);
                    //FileInfo f = new FileInfo(tempFolderLocation + "\\temp.bim");

                    //Debug.WriteLine("！");
                    //if (myReader.PeekChar() != -1)
                    //{
                    //    byte[] data;

                    //    data = myReader.ReadBytes((int)f.Length);
                    //    byte[] finaldata = new byte[data.Length];
                    //    //string data_Dec="";
                    //    for (int i = 0; i < f.Length; i++)
                    //    {
                    //        Debug.Write(data[i]+" ");
                    //    }

                    //    ////开始更改数据
                    //    //data[0] = 0;
                    //    //data[1] = 48;

                    //    //data[4] = 72;//H
                    //    //data[5] = 97;//a
                    //    //data[6] = 114;//r
                    //    //data[7] = 105;//i
                    //    //data[8] = 0;//

                    //    //for (int i=0;i<12;i++)
                    //    //{
                    //    //    finaldata[i] = data[i];
                    //    //}
                    //    //finaldata[12] = 0;
                    //    //finaldata[13] = 32;
                    //    //finaldata[14] = 0;
                    //    //finaldata[15] = 0;
                    //    //for (int i = 16; i < 19; i++)
                    //    //{
                    //    //    finaldata[i] = data[i-4];
                    //    //}
                    //    //finaldata[20] = 40;
                    //    //finaldata[21] = 1;
                    //    //for (int i = 22; i < 27; i++)
                    //    //{
                    //    //    finaldata[i] = 0;
                    //    //}

                    //    //finaldata[27] = 233;
                    //    //finaldata[28] = 254;
                    //    //finaldata[29] = 0;

                    //    //finaldata[30] = 0;
                    //    //finaldata[31] = 0;
                    //    //finaldata[32] = 32;
                    //    //finaldata[33] = 32;
                    //    //finaldata[34] = 0;
                    //    //finaldata[35] = 0;
                    //    //for (int i = 36; i<finaldata.Length; i++)
                    //    //{
                    //    //    finaldata[i] = data[i];
                    //    //}

                    //    //数据更改完毕

                    //    Debug.Write("\n");
                    //    for (int i = 0; i < f.Length; i++)
                    //    {
                    //        Debug.Write(data[i] + " ");
                    //    }

                    //    string hrbFileLocation = ((LocalFile)MainTabControl.SelectedItem).FileLocation;
                    //    hrbFileLocation = hrbFileLocation.Substring(0, hrbFileLocation.LastIndexOf("\\"));
                    //    string hrbFileName = ((LocalFile)MainTabControl.SelectedItem).Name;
                    //    hrbFileName = hrbFileName.Substring(0,hrbFileName.LastIndexOf("."));
                    //    Debug.WriteLine(hrbFileLocation + "\\" + hrbFileName + ".hrb");
                    //    using (FileStream fs = new FileStream(hrbFileLocation + "\\" + hrbFileName + ".hrb"/*tempFolderLocation+"\\temp.org"*/, FileMode.OpenOrCreate, FileAccess.Write))
                    //    {
                    //        fs.Write(finaldata, 0, finaldata.Length);


                    //        if (string.IsNullOrEmpty(hrbFileLocation))
                    //        {
                    //            OutPut_TextBox.Text += "\n生成失败";
                    //            return;
                    //        }

                    //        //hrbFileLocation = hrbFileLocation.Substring(0,hrbFileLocation.LastIndexOf("\\"));
                    //        //Debug.WriteLine(hrbFileLocation);
                    //        //FileStream fs1 = new FileStream(hrbFileLocation+"\\"+hrbFileName, FileMode.OpenOrCreate, FileAccess.Write);


                    //        //fs.CopyTo(fs1);
                    //        fs.Close();
                    //        //fs1.Close();
                    //    }

                    //}
                    //else
                    //{
                    //    Debug.WriteLine("temp.bim 不存在");
                    //}
                    //myReader.Close();
                    //myStream.Close();
                }



            }
            Directory.Delete(tempFolderLocation, true);
        }
        
        bool CloseFile(int FileIndexInTab)
        {
            LocalFile file = ((LocalFile)MainTabControl.Items[FileIndexInTab]);
            Debug.WriteLine(file.FileContent+"\n"+file.FileLocation);
            if (string.IsNullOrEmpty(file.FileLocation))
            {
                LocalFileManager.SaveFileAs(file);
                MainTabControl.Items.RemoveAt(FileIndexInTab);
                if (MainTabControl.Items.Count==0)
                {
                    EditControl.Visibility = Visibility.Collapsed;
                }
                return true;
            }
            if (file.ischanged)
            {
                switch (MessageBox.Show("是否保存对“"+file.Name+"”所作的更改？", "提示", MessageBoxButton.YesNoCancel,MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        //写入文件
                        LocalFileManager.SaveFile(file);
                        
                        break;
                    case MessageBoxResult.No:
                        //不写入文件
                        
                        break;
                    case MessageBoxResult.Cancel:
                        //不关闭文件
                        return false;
                }
            }
            MainTabControl.Items.RemoveAt(FileIndexInTab);
            if (MainTabControl.Items.Count == 0)
            {
                EditControl.Visibility = Visibility.Collapsed;
            }
            return true;
        }

        private bool ExitJCode()
        {
            while (MainTabControl.Items.Count!=0)
            {
                if (!CloseFile(MainTabControl.Items.Count - 1))
                {
                    
                    return false;
                }
            }
            Application.Current.Shutdown();
            return true;
        }

        private void Menu_File_Creat_Click(object sender, RoutedEventArgs e)
        {
            CreatFile();
        }

        private void Menu_File_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void Menu_File_Save_Click(object sender, RoutedEventArgs e)
        {
            LocalFileManager.SaveFile((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]);
        }

        private void Menu_File_SaveAs_Click(object sender, RoutedEventArgs e)
        {
            LocalFileManager.SaveFileAs((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]);
        }

        private void Menu_File_Exit_Click(object sender, RoutedEventArgs e)
        {
            ExitJCode();
        }

        private void Menu_Setting_BuildSettings_Click(object sender, RoutedEventArgs e)
        {
            BuildSettingsWindow buildSettingsWindow = new BuildSettingsWindow();
            buildSettingsWindow.Owner = this;
            buildSettingsWindow.ShowDialog();
        }


        private void Menu_Build_BuildProgram_Click(object sender, RoutedEventArgs e)
        {
            BuildProgram();
        }

        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            changingTab = true;
            Debug.WriteLine(MainTabControl.SelectedIndex);
            Content_RichTextBox.Document = new FlowDocument();

            if (MainTabControl.Items.Count<1|| MainTabControl.SelectedIndex<0)
            {
                changingTab = false;
                return;
            }

            Paragraph paragraph = new Paragraph();
            Run run = new Run() { Text = ((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]).FileContent /*, Background = new SolidColorBrush(Color.FromRgb(255, 0, 0)) */};
            
            paragraph.Inlines.Add(run);


            
            
            Debug.WriteLine(run.Text);
            //Content_RichTextBox.Document.Blocks.Clear();
            
            Content_RichTextBox.Document.Blocks.Add(paragraph);

            TextRange MyText = new TextRange(
                    Content_RichTextBox.Document.ContentStart,
                    Content_RichTextBox.Document.ContentEnd
            );

            string[] splittedLines = MyText.Text.Split(new[] { Environment.NewLine }
                                          , StringSplitOptions.None);
            //MessageBox.Show(splittedLines.Length.ToString());


            
            int max = Convert.ToInt32(splittedLines.Length);
            string linetext="";
            for (int i = 1; i < max; i++)
            {
                linetext += i.ToString() + '\n';
            }

            Paragraph paragraph1 = new Paragraph();
            Run run1 = new Run() { Text = linetext /*, Background = new SolidColorBrush(Color.FromRgb(255, 0, 0)) */};

           
            paragraph1.Inlines.Add(run1);
            Content_Line.Document.Blocks.Clear();
            Content_Line.Document.Blocks.Add(paragraph1);
            changingTab = false;
        }


        
        private void Content_RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            

            if (changingTab==false)
            {
                ((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]).ischanged=true;
                
                TextRange textRange = new TextRange(Content_RichTextBox.Document.ContentStart, Content_RichTextBox.Document.ContentEnd);
                ((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]).FileContent = textRange.Text;
            }

            if (false)
            {
                //    localFiles[MainTabControl.SelectedIndex].FileContent = new TextRange(Content_RichTextBox.Document.ContentStart, Content_RichTextBox.Document.ContentEnd).Text;

                //    TextRange MyText = new TextRange(
                //            ((RichTextBox)sender).Document.ContentStart,
                //            ((RichTextBox)sender).Document.ContentEnd
                //            );

                //    string[] splittedLines = MyText.Text.Split(new[] { Environment.NewLine }
                //                                  , StringSplitOptions.None);
                //    //MessageBox.Show(splittedLines.Length.ToString());
                //    localFiles[MainTabControl.SelectedIndex].Contentline = splittedLines.Length;
                //    Content_Line.Text = "";
                //    int max = Convert.ToInt32(localFiles[MainTabControl.SelectedIndex].Contentline);
                //    for (int i = 1; i <= max; i++)
                //    {
                //        Content_Line.Text += i.ToString() + '\n';
                //    }
            }
        }

        private void Menu_Help_AboutJCode_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }

        private void MainTabControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            
        }

        private void MainTabControl_RightButtonUpMenu_CopyFilePath_Click(object sender, RoutedEventArgs e)
        {
            LocalFile file = (LocalFile)MainTabControl.SelectedItem;
            if (string.IsNullOrEmpty(file.FileLocation))
            {
                return;
            }
            Clipboard.SetDataObject(file.FileLocation, true);
        }

        private void MainTabControl_RightButtonUpMenu_CloseFile_Click(object sender, RoutedEventArgs e)
        {
            CloseFile(MainTabControl.SelectedIndex);
        }

        private void Menu_Help_DevDocument_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://kvzmstudios.github.io/JigsawOS/DeveloperCenter/";
            //url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        
    }
}
