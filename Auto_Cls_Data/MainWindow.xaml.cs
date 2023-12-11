using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Threading;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using Auto_Cls_Data.Gplus;
using LicenseKey;
using System.Diagnostics;
using System.Text;

namespace Auto_Cls_Data
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Topmost = true;
            LoadingKeyptd();
           // System.Windows.MessageBox.Show(getprdtid("wmic os get serialnumber"));
        }
        private void LoadingKeyptd()
        {
            string IDProduct = getprdtid("wmic os get serialnumber");
            LicenseKeyProduct licensex = new LicenseKeyProduct();
            bool avg = licensex.Excurte(IDProduct);
            if (avg != true)
            {
                System.Windows.MessageBox.Show("License Key không có ,cần kích hoạt \rLicense not available, need activation\rActivated by Inspection Algorithm Unit", "License");
                this.Close();
            }
            
            productkey.Content = IDProduct;
        }
        private string getprdtid(string com)
        {
            var processinfo = new ProcessStartInfo("cmd.exe", "/c " + com)
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                WorkingDirectory = @"C:\Windows\System32\"
            };
            StringBuilder sb = new StringBuilder();
            Process ps = Process.Start(processinfo);
            ps.OutputDataReceived += (sender, args_) => sb.AppendLine(args_.Data);
            ps.BeginOutputReadLine();
            ps.WaitForExit();
            string[]spp = sb.ToString().Split('\n');
            string acc = spp[2];
            string[] spp1 = acc.ToString().Split(' ');
            return spp1[0].ToString();
        }
        int countpwerr =0;
        private void ld_windown_auto(object sender, RoutedEventArgs e)
        {
            if (Machineselection.Text == "CG_AOI")
            {
                CGAOIProfeat cgaoiprofest = new CGAOIProfeat();
                cgaoiprofest.Show();
            }
            else
            {
                Data_LD data_LD = new Data_LD();
                data_LD.Show();

            }
            Thread.Sleep(100);
            this.Close();
        }
        string LinkFoder = "config\\Prefence.json";
        private void ld_windown_seachdata(object sender, RoutedEventArgs e)
        {
            Seachdata seachdata = new Seachdata();
            seachdata.Show();
            Thread.Sleep(100);
            this.Close();
        }
        private void pw_box_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                Login();
            }
        }
        private void Login()
        {
            /*MessageBoxResult result = System.Windows.MessageBox.Show("You want to use Program Monitoring\n Bạn muốn sử dụng phần mềm Monitoring", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                //string pw = pw_box.Password;
                //pw_box.Text = string.Empty;
                if (pw == "algotech001")
                {  if(Machineselection.Text == "CG_AOI")
                    {
                        CGAOIProfeat cgaoiprofest = new CGAOIProfeat();
                        cgaoiprofest.Show();
                    }
                    else
                    {
                        Data_LD data_LD = new Data_LD();
                        data_LD.Show();

                    }
                    Thread.Sleep(100);
                    this.Close();
                }
                else
                {
                    countpwerr++;
                    System.Windows.MessageBox.Show("Sai mật khẩu : " + countpwerr);
                }
                if (countpwerr == 5)
                {
                    this.Close();
                }
            }
            else
            {
                return;
            }*/
            

        }

        private void pw_box_Loaded(object sender, RoutedEventArgs e)
        {
           
        }

        private void Cls_Thumnail_Click(object sender, RoutedEventArgs e)
        {
            
        }
        //private bool Change;
    }


}
