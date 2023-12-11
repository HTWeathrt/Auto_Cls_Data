using Auto_Cls_Data.Data_Cal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySqlConnector;
using static Auto_Cls_Data.Gplus.VH009213;
using Label = System.Windows.Controls.Label;
using Auto_Cls_Data.windownld;
using System.ComponentModel;
using System.Web.Security;
using System.Windows.Threading;
using Newtonsoft.Json;
using System.IO;
using System.Windows.Media.Animation;
using System.Diagnostics;

namespace Auto_Cls_Data.Gplus
{
    /// <summary>
    /// Interaction logic for CGAOIProfeat.xaml
    /// </summary>
    public partial class CGAOIProfeat : Window
    {
        public CGAOIProfeat()
        {
            InitializeComponent();
            this.Loaded += Grouding;
        }
        private void Grouding(object sender, RoutedEventArgs e)
        {
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true; // ho tro bao cao tien do
            bw.WorkerSupportsCancellation = true; // cho phep dung tien trinh             
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;

            string Filex = "config\\Prefence.json";
            if (!File.Exists(Filex))
            {
                MessageBox.Show("No RecipeFile");
                return;
            }
            string ADB = File.ReadAllText(Filex);
            ALGOTech = JsonConvert.DeserializeObject<AlgorithmDLL>(ADB);
        }
        BackgroundWorker bw;
        AlgorithmDLL ALGOTech;
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {

            var Timerxxx = new Stopwatch();
            Timerxxx.Start();
            SQLLoading sqload = new SQLLoading();
            tbledata = new List<tbdata>();
            Caculator caculator = new Caculator();
            List<string> selc = new List<string>();
            string timerST = string.Empty, TimerEN = string.Empty;
            Dispatcher.Invoke(() =>
            {
                timerST = timertoday;
                TimerEN = EndTime;
                selc = selectedItems;

            });
            //timerautomanual();
            VH009213 vh009213 = new VH009213();
            Dictionary<string, int> labels = new Dictionary<string, int>();
            string[] doublelane = { "A", "B" };
            string Database = string.Empty;

            foreach (var item in selc)
            {
                DataTable databaseA = new DataTable();
                
                DataTable databaseB = new DataTable();
                foreach (var databaseorgx in ALGOTech.Databasex)
                {
                    if (databaseorgx.Machine == "CG_AOI")
                    {
                        Database = databaseorgx.Database;
                    }
                }
                string query = "SELECT * FROM product WHERE pt_datetime >= '" + EndTime + "' AND pt_datetime <= '" + timertoday + "'";

                string sql = "SELECT judge, priority_defect_name, COUNT(*) AS count_defects, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" + EndTime + "' AND pt_datetime <= '" + timertoday + "'), 2) AS percent_defects FROM product WHERE pt_datetime >= '" + EndTime + "' AND pt_datetime <= '" + timertoday + "' GROUP BY judge, priority_defect_name ORDER BY priority_defect_name LIMIT 20000 ";
                foreach (string doub in doublelane)
                {
                    string Line = $"{item}_{doub}";
                    try
                    {
                        string IP = string.Empty;
                        foreach (var ipseachxx in ALGOTech.DataAlgorithm)
                        {
                            if (ipseachxx.Machine == "CG_AOI")
                            {
                                foreach (var IPD in ipseachxx.DataORG)
                                {
                                    if (IPD.Name == Line)
                                    {
                                        IP = IPD.IP;
                                    }
                                }
                            }
                        }
                        if(IP != string.Empty)
                        {
                            string connectionx = $"Server={IP}; Database={Database};User = ami; Password = protnc";
                            // string Connec = sqload.DBShow("CG_AOI", Line);
                            MySqlConnection connection = new MySqlConnection(connectionx);
                            connection.Open();
                            if (connection.State == ConnectionState.Open)
                            {
                                DataTable Basedata = caculator.CGBaseProcess(query, connection);
                                DataTable YRTtbdata = caculator.CGYRTProcess(sql, connection);
                                tbledata.Add(new tbdata(Basedata, YRTtbdata, Line));
                                string abv= LoaDataQueryALLstep(YRTtbdata, Line);
                                if (doub == "A")
                                {
                                    databaseA = Basedata;
                                }
                                else
                                {
                                    databaseB = Basedata;
                                }
                                Dispatcher.Invoke(() =>
                                {
                                    outputdefec.Text = abv;
                                    lineorgdata.Items.Add(Line);
                                    linedatacopyselect.Items.Add(Line);
                                });
                                Basedata.Dispose();
                                YRTtbdata.Dispose();
                            }
                            connection.Close();
                        }

                    }
                    catch
                    {
                        
                    }
                }
                int total, ttok, ttng, ttna = 0;
                vh009213.CaculatorCGAOI(databaseA, databaseB, SpecSEQ1, SpecSEQ2, SpecSEQ3, item);

                total = vh009213.Total;
                ttok = vh009213.ttOK;
                ttng = vh009213.ttNG;
                ttna = vh009213.ttNA;
                labels[$"ALL{item}"] = total;
                labels[$"OK{item}"] = ttok;
                labels[$"NG{item}"] = ttng;
                labels[$"NA{item}"] = ttna;

                string ADBxx = vh009213.XABV;
                Dispatcher.Invoke(() =>
                {
                    alarm_messenger.Text = ADBxx;
                    foreach (KeyValuePair<string, int> pair in labels)
                    {
                        Label label = FindLabel(pair.Key);
                        label.Content = pair.Value.ToString();
                        if (pair.Key.ToString() == "ALL" + item + "" && pair.Value != 0)
                        {//green
                            label.Background = new SolidColorBrush(Color.FromRgb(162, 185, 162));
                        }
                        else if (pair.Key.ToString() == "ALL" + item + "")
                        {
                            // red
                            label.Background = new SolidColorBrush(Color.FromRgb(200, 103, 103));
                        }
                    }
                    LineData_Defect.Items.Add(item);

                });
            }
            Timerxxx.Stop();
            TimeSpan timecopy = Timerxxx.Elapsed;
            string XACL = "Tact time : " + timecopy.ToString(@"m\:ss\.fff");
            Dispatcher.Invoke(() =>
            {
                labeltactime.Content = XACL;
            });
            Timerxxx.Restart();
            selc.Clear();
        }
        private void Bw_RunWorkerCompleted(object sender,RunWorkerCompletedEventArgs e)
        {
            loadingwindown.Visibility = Visibility.Collapsed;
        }
        private void clearallselect(object sender, RoutedEventArgs e)
        {
           
        }
        private void selectall_Checked(object sender, RoutedEventArgs e)
        {
            cg303_checkbox.IsChecked = true;
            cg304_checkbox.IsChecked = true;
            cg305_checkbox.IsChecked = true;
            cg306_checkbox.IsChecked = true;
            cg401_checkbox.IsChecked = true;
            cg402_checkbox.IsChecked = true;
            cg403_checkbox.IsChecked = true;
            cg404_checkbox.IsChecked = true;
            cg405_checkbox.IsChecked = true;
            cg406_checkbox.IsChecked = true;
            cg501_checkbox.IsChecked = true;
            cg502_checkbox.IsChecked = true;
            cg503_checkbox.IsChecked = true;
            cg504_checkbox.IsChecked = true;
           
        }
        private void Start1step(object sender, RoutedEventArgs e)
        {
            checkboxadd();
        }
        private void checkboxadd()
        {
            Cleardata();
            if (cg303_checkbox.IsChecked == true)
            {
                selectedItems.Add("303");
            }
            if (cg304_checkbox.IsChecked == true)
            {
                selectedItems.Add("304");
            }
            if (cg305_checkbox.IsChecked == true)
            {
                selectedItems.Add("305");
            }
            if (cg306_checkbox.IsChecked == true)
            {
                selectedItems.Add("306");
            }
            if (cg401_checkbox.IsChecked == true)
            {
                selectedItems.Add("401");
            }
            if (cg402_checkbox.IsChecked == true)
            {
                selectedItems.Add("402");
            }
            if (cg403_checkbox.IsChecked == true)
            {
                selectedItems.Add("403");
            }
            if (cg404_checkbox.IsChecked == true)
            {
                selectedItems.Add("404");
            }
            if (cg405_checkbox.IsChecked == true)
            {
                selectedItems.Add("405");
            }
            if (cg406_checkbox.IsChecked == true)
            {
                selectedItems.Add("406");
            }
            if (cg501_checkbox.IsChecked == true)
            {
                selectedItems.Add("501");
            }
            if (cg502_checkbox.IsChecked == true)
            {
                selectedItems.Add("502");
            }
            if (cg503_checkbox.IsChecked == true)
            {
                selectedItems.Add("503");
            }
            if (cg504_checkbox.IsChecked == true)
            {
                selectedItems.Add("504");
            }
            loadingwindown.Visibility = Visibility.Visible;
            timerautomanual();
            SocicalDefect.Clear();
            LDSpec();
            bw.RunWorkerAsync();
        }
        Window1 adb;
        float SpecSEQ1;
        float SpecSEQ2;
        float SpecSEQ3;
        private void LDSpec()
        {
            string SEQ1 = DRSet.Text;
            string SEQ2 = DRABSet.Text;
            string SEQ3 = DRStageSet.Text;
            SpecSEQ1 = float.Parse(SEQ1);
            SpecSEQ2 = float.Parse(SEQ2);
            SpecSEQ3 = float.Parse(SEQ3);
        }
        private void WindownOpen()
        {
            adb = new Window1();
            adb.Show();
        }
        List<string> selectedItems = new List<string>();
        List<string> ErrorADD = new List<string>();
        List<string> LineADD  = new List<string>();
        private string timertoday;
        private string EndTime;
        bool Timercheckmanual = false;
        private List<tbdata> tbledata;
       /* private void RunFor()
        {   SQLLoading sqload  = new SQLLoading();
            tbledata = new List<tbdata>();
            Caculator caculator = new Caculator();
            timerautomanual();
            VH009213 vh009213 = new VH009213();
            Dictionary<string, int> labels = new Dictionary<string, int>();
            string HelloWWW = "Dear Sir\nAbnormal report Algorithm_Tech \nTimer : " + EndTime + " __to__ " + timertoday + "";
            ErrorADD.Add(HelloWWW);
            ErrorADD.Add("\n");
            string[] doublelane = { "A", "B" };
            foreach (var item in selectedItems)
            {
                LineData_Defect.Items.Add(item);
                string query = "SELECT * FROM product WHERE pt_datetime >= '" + EndTime + "' AND pt_datetime <= '" + timertoday + "'";
                string sql = "SELECT judge, priority_defect_name, COUNT(*) AS count_defects, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" + EndTime + "' AND pt_datetime <= '" + timertoday + "'), 2) AS percent_defects FROM product WHERE pt_datetime >= '" + EndTime + "' AND pt_datetime <= '" + timertoday + "' GROUP BY judge, priority_defect_name ORDER BY priority_defect_name LIMIT 20000 ";
                foreach (string doub in  doublelane)
                {
                    string Line = $"{item}_{doub}";
                    linedatacopyselect.Items.Add(Line);
                    lineorgdata.Items.Add(Line);
                    try
                    {
                        string Connec = sqload.DBShow("CG_AOI", Line);
                        MySqlConnection connection = new MySqlConnection(Connec);
                        connection.Open();
                        if (connection.State == ConnectionState.Open)
                        {
                            DataTable Basedata = caculator.CGBaseProcess(query ,connection);
                            DataTable YRTtbdata = caculator.CGYRTProcess(sql ,connection);
                            tbledata.Add(new tbdata(Basedata, YRTtbdata, Line));
                            if(doub == "A")
                            {
                                databaseA = Basedata;
                            }
                            else
                            {
                                databaseB = Basedata;
                            }
                            Basedata.Dispose();
                            YRTtbdata.Dispose();
                        }
                        connection.Close();
                    }
                    catch
                    {
                        MessageBox.Show($"Error Connected Line{item}");
                    }
                }
                vh009213.CaculatorCGAOI(databaseA, databaseB);
                int total = vh009213.Total;
                int ttok = vh009213.ttOK;
                int ttng = vh009213.ttNG;
                int ttna= vh009213.ttNA;
                labels[$"ALL{item}"] = total;
                labels[$"OK{item}"] = ttok;
                labels[$"NG{item}"] = ttng;
                labels[$"NA{item}"] = ttna;
                foreach (KeyValuePair<string, int> pair in labels)
                {
                    Label label = FindLabel(pair.Key);
                    label.Content = pair.Value.ToString();
                }
            }
            adb.Close();
        }*/

        private Label FindLabel(string name)
        {
            return (Label)FindName(name);
        }


        private void Cleardata()
        {
            selectedItems.Clear();
            lineorgdata.Items.Clear();
            LineData_Defect.Items.Clear();
        }
        private void timerautomanual()
        {
            if (Timercheckmanual == true)
            {
                timertoday = Time_EN.Text;
                EndTime = Time_ST.Text;
            }
            else
            {
                timertoday = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                int ACKTimer = Convert.ToInt32(Timeback.Text);
                DateTime now = DateTime.Now;
                TimeSpan hihi = TimeSpan.FromMinutes(ACKTimer);
                DateTime Result = now.Subtract(hihi);
                EndTime = Result.ToString("yyyy-MM-dd HH:mm:ss");
            }
        }
        private void ACKTimerL(int Timer)
        {
            TimerAX = new DispatcherTimer();
            TimerAX.Interval = TimeSpan.FromSeconds(Timer);
            TimerAX.Tick += Timer_Tick;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            // WindownOpen();
            if (bw.IsBusy != true)
            {
                checkboxadd();
            }
        }
        private void startauto(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = System.Windows.MessageBox.Show("You want to use AUTO MODE\n Bạn muốn sử dụng chế độ AUTO MODE ", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                int Second = Convert.ToInt32(ReloadTimer.Text);
                ACKTimerL(Second);
                TimerAX.Start();
                buttonstart.Visibility = Visibility.Collapsed;
                buttonstopxa.Visibility = Visibility.Visible;
            }
            else
            {
                return;
            }
        }
       
        private void checkbox_manualtimer(object sender, RoutedEventArgs e)
        {
            Timercheckmanual = true;
            Time_EN.Opacity = 1;
            Time_ST.Opacity = 1;
            Timeback.Opacity = 0.5;
            Timeback.IsEnabled = false;

            Time_EN.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime now = DateTime.Now;
            TimeSpan hihi = TimeSpan.FromMinutes(30);
            DateTime Result = now.Subtract(hihi);
            Time_ST.Text = Result.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private string LoaDataQueryALLstep(DataTable table, string item)
        {
            List<string> Name_Defect = new List<string>();
            List<string> Name_DefectNG = new List<string>();
            List<int> TotalOK = new List<int>();
            List<int> TotalNG = new List<int>();
            List<int> TotalNA = new List<int>();
            Name_DefectNG.Clear();
            Name_Defect.Clear();
            int totalALL = table.Rows.Count;
            if (totalALL > 0)
            {
                foreach (DataRow row in table.Rows)
                {
                    string Name;
                    string Total;
                    string YRT;
                    Name = row["priority_defect_name"].ToString();
                    string jugde = row["judge"].ToString();
                    if (jugde == "G" || jugde == "OK")
                    {
                        Total = row["count_defects"].ToString();
                        YRT = row["percent_defects"].ToString();
                        if (Name != string.Empty && Name != "N/A")
                        {
                            Name_Defect.Add("-" + Name + " ⥎ " + Total + "ea ⥎ " + YRT + "%");
                        }
                        TotalOK.Add(Convert.ToInt32(Total));
                    }
                    else if (jugde == "N" || jugde == "NG")
                    {
                        //Name = row["priority_defect_name"].ToString();
                        Total = row["count_defects"].ToString();
                        YRT = row["percent_defects"].ToString();
                        Name_DefectNG.Add(" -" + Name + " ⥎ " + Total + "ea ⥎ " + YRT + "%");
                        TotalNG.Add(Convert.ToInt32(Total));
                    }
                    else
                    {
                        Total = row["count_defects"].ToString();
                        TotalNA.Add(Convert.ToInt32(Total));
                    }
                }
                int Sum = TotalOK.Sum();
                int SumNG = TotalNG.Sum();
                int Total_NA = TotalNA.Sum();
                int SumALL = Sum + SumNG;
                //
                float SumX = float.Parse(Sum.ToString());
                float SumNGX = float.Parse(SumNG.ToString());
                float SumALLX = float.Parse(SumALL.ToString());
                float SUMNA = float.Parse(Total_NA.ToString());
                //
                SocicalDefect.Add("•Line: " + item);
                SocicalDefect.Add("Total: " + SumALL + "ea");
                //
                if (Sum > 0)
                {
                    float SUMALYRT = (SumX / SumALLX) * 100;
                    float SUMOK = float.Parse(SUMALYRT.ToString("#.#"));
                    SocicalDefect.Add("OK : " + Sum + "ea ⥎ " + SUMOK + "%");
                }
                else
                {
                    SocicalDefect.Add("OK : " + Sum + "ea ⥎ 0%");
                }
                //
                foreach (string name in Name_Defect)
                {
                    SocicalDefect.Add(name);
                }
                if (SumNG > 0)
                {
                    float SUMNGYR = (SumNGX / SumALLX) * 100;
                    float SUMNGX = float.Parse(SUMNGYR.ToString("#.#"));
                    SocicalDefect.Add("NG : " + SumNG + "ea ⥎ " + SUMNGX + "% ");
                }
                else
                {
                    SocicalDefect.Add("NG : " + SumNG + "ea ⥎ 0% ");
                }
                //
                foreach (string name in Name_DefectNG)
                {
                    SocicalDefect.Add(name);

                }
                if (Total_NA > 0)
                {

                }
                SocicalDefect.Add(" ");

            }
            string XABV = string.Join(Environment.NewLine, SocicalDefect);
            return XABV;
        }
        List<string> SocicalDefect = new List<string>();
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Timercheckmanual = false;
            Time_EN.Opacity = 0.5;
            Time_ST.Opacity = 0.5;
            Timeback.Opacity = 1;
            Timeback.IsEnabled = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string Line = linedatacopyselect.Text;
            DataObject dataObject = new DataObject();
            StringBuilder clipboardText = new StringBuilder();
            foreach (var Item in tbledata)
            {
                DataTable dt = Item.Dt;
                if (Line == Item.Xbline)
                {
                    clipboardText.Append(GetClipboardText(dt));
                }
                dataObject.SetData(DataFormats.UnicodeText, clipboardText.ToString());
                Clipboard.SetDataObject(dataObject, true);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private string GetClipboardText(DataTable sqlbaseTable)
        {
            StringBuilder sb = new StringBuilder();
            // Lấy tên các cột
            IEnumerable<string> columnNames = sqlbaseTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
            sb.AppendLine(string.Join("\t", columnNames));
            // Lấy giá trị các dòng
            foreach (DataRow row in sqlbaseTable.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join("\t", fields));
            }
            return sb.ToString();
        }
        private DispatcherTimer TimerAX;
        private void buttonstop(object sender, RoutedEventArgs e)
        {
            TimerAX.Stop();
            buttonstart.Visibility = Visibility.Visible;
            buttonstopxa.Visibility = Visibility.Collapsed;
        }
        private void checkimport(object sender, RoutedEventArgs e)
        {
            string line = LineData_Defect.Text;
            string A = $"{line}_A";
            string B = $"{line}_B";
            foreach (var Item in tbledata)
            {
                DataTable dt = Item.Tbyrt;
                if (A == Item.Xbline)
                {
                    DatatbdefectA.ItemsSource = dt.DefaultView;
                }
                if(B == Item.Xbline)
                {
                    DatatbdefectB.ItemsSource = dt.DefaultView;
                }
            }
        }
        private void checklinetabledataorg(object sender, RoutedEventArgs e)
        {
            string line = linedatacopyselect.Text;
            foreach (var Item in tbledata)
            {
                DataTable dt = Item.Dt;
                if (line == Item.Xbline)
                {
                    datatableorg.ItemsSource = dt.DefaultView;
                }
            }
        }
        private void copyyalldatatoclipboard(object sender, RoutedEventArgs e)
        {
            DataObject dataObject = new DataObject();
            StringBuilder clipboardText = new StringBuilder();
            foreach (var Item in tbledata)
            {
                DataTable dt = Item.Dt;
                int rowtt = dt.Rows.Count;
                if (rowtt != 0)
                {
                    clipboardText.Append(GetClipboardText(dt));
                }
            }
            dataObject.SetData(DataFormats.UnicodeText, clipboardText.ToString());
            Clipboard.SetDataObject(dataObject, true);
        }
        string[] itemline = { "303", "304", "305", "306", "401", "402", "403", "404", "405", "501", "502", "503", "504" };

        private void selectall_Unchecked(object sender, RoutedEventArgs e)
        {
            cg303_checkbox.IsChecked = false;
            cg304_checkbox.IsChecked = false;
            cg305_checkbox.IsChecked = false;
            cg306_checkbox.IsChecked = false;
            cg401_checkbox.IsChecked = false;
            cg402_checkbox.IsChecked = false;
            cg403_checkbox.IsChecked = false;
            cg404_checkbox.IsChecked = false;
            cg405_checkbox.IsChecked = false;
            cg406_checkbox.IsChecked = false;
            cg501_checkbox.IsChecked = false;
            cg502_checkbox.IsChecked = false;
            cg503_checkbox.IsChecked = false;
            cg504_checkbox.IsChecked = false;

            Dictionary<string, int> labels = new Dictionary<string, int>();
            foreach (string itemx in itemline)
            {
                labels[$"ALL{itemx}"] = 0;
                foreach (KeyValuePair<string, int> pair in labels)
                {
                    Label label = FindLabel(pair.Key);
                    if (pair.Key.ToString() == "ALL" + itemx + "")
                    {
                        label.Background = null;
                    }
                }
            }
        }
    }
    public class tbdata
    {
        private DataTable dt;
        private string xbline;
        private DataTable tbyrt;
        public tbdata(DataTable sqldata,DataTable sqlyrttb,string Line)
        {
            this.Dt = sqldata;
            this.Xbline = Line;
            this.Tbyrt = sqlyrttb;
        }
        public DataTable Dt { get => dt; set => dt = value; }
        public string Xbline { get => xbline; set => xbline = value; }
        public DataTable Tbyrt { get => tbyrt; set => tbyrt = value; }
    }
}
