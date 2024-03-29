﻿using System;
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
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using JCode.Model;
using JCode.OtherWindow;
using Microsoft.Win32;
using static System.Net.Mime.MediaTypeNames;

namespace JCode
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly string JCodeFolderLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).Replace("/","\\")+"\\JCode";/*"C:\\Users\\" + System.Environment.UserName + "\\Documents\\JCode";*/
        public readonly string JCodeFolderLocation_Left = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)+"/JCode";
        readonly string tempFolderLocation = "C:\\Users\\" + System.Environment.UserName + "\\Documents\\JCode\\temp";
        readonly string tempFolderLocation_Left = "C:/Users/" + System.Environment.UserName + "/Documents/JCode/temp";
        public readonly string SettingsFolderLocation = "C:\\Users\\" + System.Environment.UserName + "\\Documents\\JCode\\Settings\\";
        public readonly string SettingsFolderLocation_Left = "C:/Users/" + System.Environment.UserName + "/Documents/JCode/Settings/";


        //public List<LocalFile> localFiles = new List<LocalFile>();
        int NewFileNameIndex = 0;//未命名0，未命名1……
        int lastSelectTab = -1;


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
            LocalFile file = new LocalFile { Name="未命名"+(++NewFileNameIndex),FileContent="",ischanged=false};
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



            }
            Directory.Delete(tempFolderLocation, true);
        }
        
        bool CloseFile(int FileIndexInTab)
        {
            LocalFile file = ((LocalFile)MainTabControl.Items[FileIndexInTab]);
            Debug.WriteLine(file.FileContent+"\n"+file.FileLocation);
            
            if (file.ischanged)
            {
                switch (System.Windows.MessageBox.Show("是否保存对“"+file.Name+"”所作的更改？", "提示", MessageBoxButton.YesNoCancel,MessageBoxImage.Question))
                {
                    case MessageBoxResult.Yes:
                        //写入文件
                        LocalFileManager.SaveFile(file);
                        
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        //不关闭文件
                        return false;
                }
            }
            //MainTabControl.Items.RemoveAt(FileIndexInTab);
            if (MainTabControl.Items.Count == 0)
            {
                EditControl.Visibility = Visibility.Collapsed;
            }
            MainTabControl.Items.RemoveAt(FileIndexInTab);
            if (MainTabControl.SelectedIndex==-1)
            {
                Content_RichTextBox.Visibility= Visibility.Collapsed;
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
            System.Windows.Application.Current.Shutdown();
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
            //((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]).FileContent = Content_RichTextBox.Text;
            LocalFileManager.SaveFile(((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]));
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


        bool changeingTab = false;
        private void MainTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            changeingTab = true;
            if (MainTabControl.SelectedIndex == -1)
            {
                lastSelectTab = -1;
                return;
            }
            Content_RichTextBox.Visibility = Visibility.Visible;
            if (lastSelectTab != -1 && lastSelectTab < MainTabControl.Items.Count)
            {
                //Debug.WriteLine(lastSelectTab+"||"+MainTabControl.SelectedIndex);
                ((LocalFile)MainTabControl.Items[lastSelectTab]).FileContent = Content_RichTextBox.Text;
            }

            if (!String.IsNullOrEmpty(((LocalFile)MainTabControl.SelectedItem).FileContent))
            {
                Content_RichTextBox.Text = ((LocalFile)MainTabControl.SelectedItem).FileContent;
                //Debug.WriteLine(":::"+((LocalFile)MainTabControl.SelectedItem).FileContent);
            } 
            else
                Content_RichTextBox.Text = "";
            lastSelectTab = MainTabControl.SelectedIndex;
        }


        private void Menu_Help_AboutJCode_Click(object sender, RoutedEventArgs e)
        {
            AboutWindow aboutWindow = new AboutWindow();
            aboutWindow.Owner = this;
            aboutWindow.ShowDialog();
        }


        int MainTabContol_RightClickItem = -1;
        private void MainTabControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            MainTabContol_RightClickItem = MainTabControl.Items.IndexOf(((Grid)sender).DataContext as LocalFile);
        }

        private void MainTabControl_RightButtonUpMenu_CopyFilePath_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabContol_RightClickItem == -1)
                return;
            LocalFile file = (LocalFile)MainTabControl.Items[MainTabContol_RightClickItem];
            if (string.IsNullOrEmpty(file.FileLocation))
            {
                return;
            }
            System.Windows.Clipboard.SetDataObject(file.FileLocation, true);
        }

        private void MainTabControl_RightButtonUpMenu_CloseFile_Click(object sender, RoutedEventArgs e)
        {
            if (MainTabContol_RightClickItem!=-1)
            {
                CloseFile(MainTabContol_RightClickItem);
                MainTabContol_RightClickItem = -1;
            }
        }

        private void Menu_Help_DevDocument_Click(object sender, RoutedEventArgs e)
        {
            string url = "https://kvzmstudios.github.io/JigsawOS/DeveloperCenter/";
            //url = url.Replace("&", "^&");
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        private void Content_RichTextBox_TextChanged(object sender, EventArgs e)
        {
            if (changeingTab == false)
            {
                ((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]).ischanged = true;
                ((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]).FileContent = Content_RichTextBox.Text;
            }
            changeingTab = false;

            /*Debug.WriteLine(Content_RichTextBox.Text);

            if (changingTab == false)
            {
                ((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]).ischanged = true;

                *//*TextRange textRange = new TextRange(Content_RichTextBox.Document.ContentStart, Content_RichTextBox.Document.ContentEnd);*//*
                ((LocalFile)MainTabControl.Items[MainTabControl.SelectedIndex]).FileContent = Content_RichTextBox.Text;

            }*/
        }

        private void MainTabControl_CloseTabBtn_Click(object sender, RoutedEventArgs e)
        {
            LocalFile file = ((System.Windows.Controls.Button)sender).DataContext as LocalFile;
            CloseFile(MainTabControl.Items.IndexOf(file));
        }

        private void MainTabControl_RightButtonUpMenu_ReloadFile_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(MainTabContol_RightClickItem);
            LocalFile file = (LocalFile)MainTabControl.Items[MainTabContol_RightClickItem];
            if (!String.IsNullOrEmpty(file.FileLocation))
            {
                
                file.FileContent = File.ReadAllText(file.FileLocation);
                Content_RichTextBox.Text = ((LocalFile)MainTabControl.SelectedItem).FileContent;
                changeingTab = true;
            }
        }

        double scale = 1;
        private void EditControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                if (e.Delta < 0 && scale>0.2)
                {
                    scale -= 0.1;
                }
                else
                {
                    scale += 0.1;
                }
                Content_RichTextBox.FontSize = scale * 11;
            }
        }
    }
}
