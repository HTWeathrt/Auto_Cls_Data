using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Windows;
using System.Threading;
using System.Runtime.ExceptionServices;
using System.Windows.Forms;

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
        }
        int countpwerr =0;
        private void ld_windown_auto(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("You want to use Program Monitoring\n Bạn muốn sử dụng phần mềm Monitoring", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                string pw = pw_box.Password;
                //pw_box.Text = string.Empty;
                if (pw == "algotech001")
                {
                    Data_LD data_LD = new Data_LD();
                    data_LD.Show();
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
            }
             
            
        }

        private void ld_windown_seachdata(object sender, RoutedEventArgs e)
        {
            Seachdata seachdata = new Seachdata();
            seachdata.Show();
            Thread.Sleep(100);
            this.Close();
        }
        //private bool Change;


    }
}
