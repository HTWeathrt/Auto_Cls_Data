using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace Auto_Cls_Data.windownld
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            Screen pri = Screen.PrimaryScreen;
            int Width = pri.Bounds.Width;
            int Height = pri.Bounds.Height;
            this.Left = Width/2-70;
            this.Top = Height/2-130;
            this.Topmost = true;

        }


    }
}
