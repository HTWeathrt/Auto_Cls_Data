using Auto_Cls_Data.Data_Cal;
using Auto_Cls_Data.windownld;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MySqlConnector;
using System.Timers;
using Timer = System.Timers.Timer;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.Runtime.InteropServices;
using System.Diagnostics.Metrics;
using System.IO;
using System.Windows.Media.Media3D;
using System.Windows.Threading;
using System.ComponentModel;
using System.Windows.Forms;
using DataObject = System.Windows.DataObject;
using Clipboard = System.Windows.Forms.Clipboard;
using DataFormats = System.Windows.DataFormats;
using MessageBox = System.Windows.Forms.MessageBox;
using System.Windows.Shapes;
//using Label = System.Windows.Forms.Label;
using Label = System.Windows.Controls.Label;
using System.Web.UI;
using Auto_Cls_Data.Gplus;
using Serilog;
using Newtonsoft.Json;
using System.Diagnostics;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;

namespace Auto_Cls_Data
{
    /// <summary>
    /// Interaction logic for Data_LD.xaml
    /// </summary>
    public partial class Data_LD : Window
    {
        public Data_LD()
        {
            InitializeComponent();


            this.Loaded += Grouding;
            
        }
        private void Grouding(object sender , RoutedEventArgs e)
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd");
            if (!File.Exists(timestamp))
            {
                File.Create(timestamp + ".txt");
            }
            Log.Logger = new LoggerConfiguration().WriteTo.File($"Log\\{timestamp}.txt").CreateLogger();

            string Filex = "config\\Prefence.json";
            if (!File.Exists(Filex))
            {
                MessageBox.Show("No RecipeFile");
                return;
            }
            string ADB = File.ReadAllText(Filex);
            ALGOTech = JsonConvert.DeserializeObject<AlgorithmDLL>(ADB);
            bw = new BackgroundWorker();
            bw.WorkerReportsProgress = true; // ho tro bao cao tien do
            bw.WorkerSupportsCancellation = true; // cho phep dung tien trinh             
            bw.DoWork += bw_DoWork;
            bw.RunWorkerCompleted += Bw_RunWorkerCompleted;
        }
        AlgorithmDLL ALGOTech;
        BackgroundWorker bw;
        bool manualtimer = false;
        private DispatcherTimer TimerAX;
        private string MachineName;
        private bool XAML;
        private void bw_DoWork(object sender , DoWorkEventArgs e)
        {

            var timerx = new Stopwatch();
            timerx.Start();
            SocicalDefect.Clear();
            AlarmMessenger.Clear();
            tabledataquex = new List<DataTableQueyy>();
            string timerST = string.Empty, TimerEN = string.Empty,machineten = string.Empty;
            string database = string.Empty;
            float Specseq1 = 0, Specseq2 = 0, Specseq3 = 0;
            List<string> selc = new List<string>();
            Dictionary<string, int> labels = new Dictionary<string, int>();
            Dispatcher.Invoke(() =>
            {
                 timerST = timertoday;
                 TimerEN = EndTime;
                 selc = selectedItems;
                 machineten = MachineName;
                 Specseq1 = SpecSEQ1;
                 Specseq2=SpecSEQ2 ;
                 Specseq3=SpecSEQ3 ;
            });
            IP_Class iP_Class = new IP_Class();
            SQLLoading sqlloading = new SQLLoading();
            Caculator caculator = new Caculator();
            ADBData adbdata = new ADBData();
            ///
            foreach (var Data in ALGOTech.Databasex)
            {
                if (machineten == Data.Machine)
                {
                    database = Data.Database;
                }
            }
            foreach (string item in selc)
            {
                string IP = string.Empty;

                foreach(var Data in ALGOTech.DataAlgorithm )
                {
                    if(Data.Machine == machineten)
                    {
                        foreach (var xxx in Data.DataORG)
                        {
                            if(xxx.Name == item)
                            {
                                IP = xxx.IP;
                            }    
                        }

                    }    
                }
                string Loading = $"Server={IP}; Database={database}; User = ami; Password = protnc";
              //  string xxxxx = sqlloading.DBShow(machineten, item);
                if (IP != string.Empty)
                {
                    try
                    {
                        MySqlConnection connection = new MySqlConnection(Loading);
                        connection.Open();
                        if (connection.State == ConnectionState.Open)
                        {
                            string query = "SELECT * FROM product WHERE pt_datetime >= '" + TimerEN + "' AND pt_datetime <= '" + timerST + "'";
                            string sql = "SELECT judge, priority_defect_name, COUNT(*) AS count_defects, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" + TimerEN + "' AND pt_datetime <= '" + timerST + "'), 2) AS percent_defects FROM product WHERE pt_datetime >= '" + TimerEN + "' AND pt_datetime <= '" + timerST + "' GROUP BY judge, priority_defect_name ORDER BY priority_defect_name LIMIT 40000 ";

                            DataTable SQLDefect = caculator.CGBaseProcess(sql, connection);
                            DataTable sqlbaseTable = caculator.CGBaseProcess(query, connection);
                            tabledataquex.Add(new DataTableQueyy(sqlbaseTable, SQLDefect, item));
                            //DatabaseinTable(sqlbaseTable);
                            //SQLDefect_Dow(SQLDefect);
                            // adapter.Fill(sqlbaseTable);
                            adbdata.MNT_CP_IS(sqlbaseTable, machineten, item, Specseq1, Specseq2, Specseq3);
                            int total = adbdata.totalALL;
                            int ttok = adbdata.totalOK;
                            int ttng = adbdata.totalNG;
                            int ttna = adbdata.totalNA;





                            string Locat = adbdata.XABV;

                            //AlarmMessenger.Add(Locat);
                            labels[$"ALL{item}"] = total;
                            labels[$"OK{item}"] = ttok;
                            labels[$"NG{item}"] = ttng;
                            labels[$"NA{item}"] = ttna;

                            string XABV = LoaDataQueryALLstep(SQLDefect,item);
                            //List<string> Listxx = adbdata.;
                            Dispatcher.Invoke(() =>
                            {
                                // Tải dữ liệu vào Heap
                                Alarm_messenger.Text = Locat;
                                    outputdefec.Text = XABV;
                                    LineData_Defect.Items.Add(item);
                                    lineorgdata.Items.Add(item);
                                    linedatacopyselect.Items.Add(item);
                                    foreach (KeyValuePair<string, int> pair in labels)
                                    {
                                        Label label = FindLabel(pair.Key);
                                        label.Content = pair.Value.ToString();
                                        if (pair.Key.ToString() == "ALL" + item + "" && pair.Value != 0)
                                        {//green
                                                label.Background = new SolidColorBrush(Color.FromRgb(162, 185, 162));
                                        }
                                        else if(pair.Key.ToString() == "ALL" + item + "")
                                        {
                                          // red
                                           label.Background = new SolidColorBrush(Color.FromRgb(200, 103, 103));
                                        }
                                    }
                            });
                            SQLDefect.Dispose();
                            sqlbaseTable.Dispose();
                        }
                        connection.Close();
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show($"Error Connected Line{item}");
                        Log.Error("Err"+ex);
                    }
                }
                else
                {
                    Dispatcher.Invoke(() =>
                    {
                        labels[$"ALL{item}"] = 0;
                        foreach (KeyValuePair<string, int> pair in labels)
                        {
                            Label label = FindLabel(pair.Key);
                            if (pair.Key.ToString() == "ALL" + item + "")
                            {
                                label.Background = new SolidColorBrush(Color.FromRgb(200, 103, 103));
                                
                            }
                            // label.Background = new SolidColorBrush(Color.FromRgb(162,185,162));
                        }
                    });
                }
            }
            timerx.Stop();
            
            TimeSpan timecopy = timerx.Elapsed;
            string XACL = "Tact time : " + timecopy.ToString(@"m\:ss\.fff");
            Dispatcher.Invoke(() =>
            {
                labeltactime.Content = XACL;
            });
            timerx.Restart();
            selc.Clear();
        }
        
        private void Bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //SQLDefect_Dow();
            loadingwindown.Visibility = Visibility.Collapsed;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
           // WindownOpen();
           if(bw.IsBusy != true)
            {
                Stepbystop();
            }   
        }

        private void ACKTimerL(int Timer)
        {
            TimerAX = new DispatcherTimer();
            TimerAX.Interval = TimeSpan.FromSeconds(Timer);
            TimerAX.Tick += Timer_Tick;
        }
        private void Start_Auto(object sender, RoutedEventArgs e)
        {

        }
        private void Setting_windown(object sender, RoutedEventArgs e)
        {
           
        }
        List<string> SocicalDefect = new List<string>();
        List<string> AlarmMessenger = new List<string>();
        private string LoaDataQueryALLstep(DataTable table , string item)
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
        private void StartQuery1step(object sender, RoutedEventArgs e)
        {
            //MessageBox.Show("Are your Start Mode Auto");
            MessageBoxResult result = System.Windows.MessageBox.Show("You want to use AUTO MODE\n Bạn muốn sử dụng chế độ AUTO MODE ", "Confirm", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if(result == MessageBoxResult.Yes)
            {
                int Second = Convert.ToInt32(ReloadTimer.Text);
                ACKTimerL(Second);
                TimerAX.Start();
               //s LoaDataQueryALLstep();
                buttonstart.Visibility = Visibility.Collapsed;
                buttonstopxa.Visibility = Visibility.Visible;
            }
            else
            {
                return;
            }
        }
        //private string iplist;
        private void querysmartAOI()
        {
            Cleardata();
            if (cp301_checkbox.IsChecked == true)
            {
                selectedItems.Add("301");
            }
            if (cp302_checkbox.IsChecked == true)
            {
                selectedItems.Add("302");
            }
            if (cp303_checkbox.IsChecked == true)
            {
                selectedItems.Add("303");
            }
            if (cp304_checkbox.IsChecked == true)
            {
                selectedItems.Add("304");
            }
            if (cp305_checkbox.IsChecked == true)
            {
                selectedItems.Add("305");
            }
            if (cp306_checkbox.IsChecked == true)
            {
                selectedItems.Add("306");
            }
            if (cp401_checkbox.IsChecked == true)
            {
                selectedItems.Add("401");
            }
            if (cp402_checkbox.IsChecked == true)
            {
                selectedItems.Add("402");
            }
            if (cp403_checkbox.IsChecked == true)
            {
                selectedItems.Add("403");
            }
            if (cp404_checkbox.IsChecked == true)
            {
                selectedItems.Add("404");
            }
            if (cp405_checkbox.IsChecked == true)
            {
                selectedItems.Add("405");
            }
            if (cp406_checkbox.IsChecked == true)
            {
                selectedItems.Add("406");
            }
            if (cp501_checkbox.IsChecked == true)
            {
                selectedItems.Add("501");
            }
            if (cp502_checkbox.IsChecked == true)
            {
                selectedItems.Add("502");
            }
            if (cp503_checkbox.IsChecked == true)
            {
                selectedItems.Add("503");
            }
            if (cp504_checkbox.IsChecked == true)
            {
                selectedItems.Add("504");
            }
            if (cp505_checkbox.IsChecked == true)
            {
                selectedItems.Add("505");
            }
            if (cp506_checkbox.IsChecked == true)
            {
                selectedItems.Add("506");
            }
            TimerStart();
            bw.RunWorkerAsync();
        }
        private void StartQuery1st_1query(object sender, RoutedEventArgs e)
        {

        }
        private void LDSpec()
        {
            string SEQ1 = DRSet.Text;
            string SEQ2 = DRABSet.Text;
            string SEQ3 = DRStageSet.Text;
            SpecSEQ1 = float.Parse(SEQ1);
            SpecSEQ2 = float.Parse(SEQ2);
            SpecSEQ3 = float.Parse(SEQ3);
        }

        //private string Linemachine;
        public void Stepbystop()
        {
            LDSpec();
            LineData_Defect.Items.Clear();
            linedatacopyselect.Items.Clear();
            lineorgdata.Items.Clear();
            ErrorADD.Clear();
            if (selectline.Text != string.Empty)
            {
                MachineName = selectline.Text;
                querysmartAOI();
                loadingwindown.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("BUG");
            }    
        }
        private void Start1step(object sender, RoutedEventArgs e)
        {
            if (selectline.Text != string.Empty) 
            {
               // WindownOpen();
                Stepbystop();
                
            }
            else
            {
                MessageBox.Show("Select Machine ");
            }    
        }

        private void DatabaseinTable(DataTable table)
        {
            // Tổng số của Cả bảng;
            int totalALL = table.Rows.Count;
            int totalOK = 0;
            int totalNG = 0;
            int totalNA = 0;
            // Tổng số của bảng A
            int totalA_A = 0;
            int totalOK_A = 0;
            int totalNG_A = 0;
            int totalNA_A = 0;
            // Tổng số của bảng B
            int totalB_B = 0;
            int totalOK_B = 0;
            int totalNG_B = 0;
            int totalNA_B = 0;
            // Tổng số NG của Bàn 1 và 2
            int NGA1 = 0;
            int NGA2 = 0;
            int NGB1 = 0;
            int NGB2 = 0;
             // Tổng số Total của 2 bàn 
            int A1TT = 0;
            int A2TT = 0;
            int B1TT = 0;
            int B2TT = 0;
            switch (MachineName)
            {
                case "CP_AOI":
                    foreach (DataRow row in table.Rows)
                    {
                        string rowpid = "";
                        rowpid = row["localid"].ToString();
                        /// total
                        if (row["localid"].ToString() != null)
                        {
                            string[] splitValues = rowpid.Split('_');
                            string valueA = splitValues[1].Substring(0, 1);
                            string value12 = splitValues[2].Substring(0, 1);
                            if (valueA == "A")
                            {
                                totalA_A++;
                                if (value12 == "1")
                                {
                                    A1TT++;
                                }
                                if (value12 == "2")
                                {
                                    A2TT++;
                                }
                            }
                            if (valueA == "B")
                            {
                                totalB_B++;
                                if (value12 == "1")
                                {
                                    B1TT++;
                                }
                                if (value12 == "2")
                                {
                                    B2TT++;
                                }
                            }
                        }
                        if (row["judge"].ToString() == "N")
                        {
                            if (row["localid"].ToString() != null)
                            {
                                string[] splitValues = rowpid.Split('_');
                                string valueA = splitValues[1].Substring(0, 1);
                                string value12 = splitValues[2].Substring(0, 1);
                                if (valueA == "A")
                                {
                                    totalNG_A++;
                                    if (value12 == "1")
                                    {
                                        NGA1++;
                                    }
                                    if (value12 == "2")
                                    {
                                        NGA2++;
                                    }
                                }
                                if (valueA == "B")
                                {
                                    totalNG_B++;
                                    if (value12 == "1")
                                    {
                                        NGB1++;
                                    }
                                    if (value12 == "2")
                                    {
                                        NGB2++;
                                    }
                                }
                            }
                            totalNG++;
                        }
                        if (row["judge"].ToString() == "G")
                        {
                            if (row["localid"].ToString() != null)
                            {
                                string[] splitValues = rowpid.Split('_');
                                string valueA = splitValues[1].Substring(0, 1);
                                if (valueA == "A")
                                {
                                    totalOK_A++;
                                }
                                if (valueA == "B")
                                {
                                    totalOK_B++;
                                }
                            }
                            totalOK++;
                        }
                        if (row["judge"].ToString() != "N" && row["judge"].ToString() != "G")
                        {
                            if (row["localid"].ToString() != null)
                            {
                                string[] splitValues = rowpid.Split('_');
                                string valueA = splitValues[1].Substring(0, 1);
                                if (valueA == "A")
                                {
                                    totalNA_A++;
                                }
                                if (valueA == "B")
                                {
                                    totalNA_B++;
                                }

                            }
                            totalNA++;
                        }
                    };
                    break;
                case "IS_AOI":
                    foreach (DataRow row in table.Rows)
                    {
                        string rowpid = "";
                        rowpid = row["inspection_zone"].ToString();
                        string valueA = rowpid[0].ToString();
                        string value12 = rowpid[1].ToString();

                        if (row["localid"].ToString() != null)
                        {
                            if (valueA == "A")
                            {
                                totalA_A++;
                                if (value12 == "1")
                                {
                                    A1TT++;
                                }
                                if (value12 == "2")
                                {
                                    A2TT++;
                                }
                            }
                            if (valueA == "B")
                            {
                                totalB_B++;
                                if (value12 == "1")
                                {
                                    B1TT++;
                                }
                                if (value12 == "2")
                                {
                                    B2TT++;
                                }
                            }
                        }
                        if (row["judge"].ToString() == "NG")
                        {
                            if (row["localid"].ToString() != null)
                            {
                                if (valueA == "A")
                                {
                                    totalNG_A++;
                                    if (value12 == "1")
                                    {
                                        NGA1++;
                                    }
                                    if (value12 == "2")
                                    {
                                        NGA2++;
                                    }
                                }
                                if (valueA == "B")
                                {
                                    totalNG_B++;
                                    if (value12 == "1")
                                    {
                                        NGB1++;
                                    }
                                    if (value12 == "2")
                                    {
                                        NGB2++;
                                    }
                                }
                            }
                            totalNG++;
                        }
                        if (row["judge"].ToString() == "OK")
                        {
                            if (row["localid"].ToString() != null)
                            {

                                if (valueA == "A")
                                {
                                    totalOK_A++;
                                }
                                if (valueA == "B")
                                {
                                    totalOK_B++;
                                }
                            }
                            totalOK++;
                        }
                        if (row["judge"].ToString() != "NG" && row["judge"].ToString() != "OK")
                        {
                            if (row["localid"].ToString() != null)
                            {

                                if (valueA == "A")
                                {
                                    totalNA_A++;
                                }
                                if (valueA == "B")
                                {
                                    totalNA_B++;
                                }

                            }
                            totalNA++;
                        }
                    };
                    break;
            }

            float AA_A = float.Parse(totalA_A.ToString());
            float A_NG = float.Parse(totalNG_A.ToString());
            float BB_B = float.Parse(totalB_B.ToString());
            float B_NG = float.Parse(totalNG_B.ToString());
            //
            float TotalALL_AB = float.Parse(totalALL.ToString());
            float NGALLAB = float.Parse(totalNG.ToString());
            //
            float NGA1_X = float.Parse(NGA1.ToString());
            float NGA2_X = float.Parse(NGA2.ToString());
            float NGB1_X = float.Parse(NGB1.ToString());
            float NGB2_X = float.Parse(NGB2.ToString());
            // YRT Table
            float YRTTBA1 = (NGA1_X / A_NG) * 100;
            float YRTTBA2 = (NGA2_X / A_NG) * 100;
            float YRTTBB1 = (NGB1_X / B_NG) * 100;
            float YRTTBB2 = (NGB2_X / B_NG) * 100;

            float YRTNA_AB = (totalNA / TotalALL_AB) * 100;
            float YRTOK_AB = (totalOK / TotalALL_AB) * 100;
            float YRTNG_AB = (NGALLAB / TotalALL_AB) * 100;

            float YRTNG_A = (A_NG / AA_A) * 100;
            float YRTNG_B = (B_NG / BB_B) * 100;
            
            switch (numberlinexab)
            {
                case "301":
                    ALL301.Content = totalALL.ToString();
                    OK301.Content = totalOK.ToString();
                    NG301.Content = totalNG.ToString();
                    NA301.Content = totalNA.ToString();
                    break;
                case "302":
                    ALL302.Content = totalALL.ToString();
                    OK302.Content = totalOK.ToString();
                    NG302.Content = totalNG.ToString();
                    NA302.Content = totalNA.ToString();
                    break;
                case "303":
                    ALL303.Content = totalALL.ToString();
                    OK303.Content = totalOK.ToString();
                    NG303.Content = totalNG.ToString();
                    NA303.Content = totalNA.ToString();
                    break;
                case "304":
                    ALL304.Content = totalALL.ToString();
                    OK304.Content = totalOK.ToString();
                    NG304.Content = totalNG.ToString();
                    NA304.Content = totalNA.ToString();
                    break;
                case "305":
                    ALL305.Content = totalALL.ToString();
                    OK305.Content = totalOK.ToString();
                    NG305.Content = totalNG.ToString();
                    NA305.Content = totalNA.ToString();
                    break;
                case "306":
                    ALL306.Content = totalALL.ToString();
                    OK306.Content = totalOK.ToString();
                    NG306.Content = totalNG.ToString();
                    NA306.Content = totalNA.ToString();
                    break;
                case "401":
                    ALL401.Content = totalALL.ToString();
                    OK401.Content = totalOK.ToString();
                    NG401.Content = totalNG.ToString();
                    NA401.Content = totalNA.ToString();
                    break;
                case "402":
                    ALL402.Content = totalALL.ToString();
                    OK402.Content = totalOK.ToString();
                    NG402.Content = totalNG.ToString();
                    NA402.Content = totalNA.ToString();
                    break;
                case "403":
                    ALL403.Content = totalALL.ToString();
                    OK403.Content = totalOK.ToString();
                    NG403.Content = totalNG.ToString();
                    NA403.Content = totalNA.ToString();
                    break;
                case "404":
                    ALL404.Content = totalALL.ToString();
                    OK404.Content = totalOK.ToString();
                    NG404.Content = totalNG.ToString();
                    NA404.Content = totalNA.ToString();
                    break;
                case "405":
                    ALL405.Content = totalALL.ToString();
                    OK405.Content = totalOK.ToString();
                    NG405.Content = totalNG.ToString();
                    NA405.Content = totalNA.ToString();
                    break;
                case "406":
                    ALL406.Content = totalALL.ToString();
                    OK406.Content = totalOK.ToString();
                    NG406.Content = totalNG.ToString();
                    NA406.Content = totalNA.ToString();
                    break;
                case "501":
                    ALL501.Content = totalALL.ToString();
                    OK501.Content = totalOK.ToString();
                    NG501.Content = totalNG.ToString();
                    NA501.Content = totalNA.ToString();
                    break;
                case "502":
                    ALL502.Content = totalALL.ToString();
                    OK502.Content = totalOK.ToString();
                    NG502.Content = totalNG.ToString();
                    NA502.Content = totalNA.ToString();
                    break;
                case "503":
                    ALL503.Content = totalALL.ToString();
                    OK503.Content = totalOK.ToString();
                    NG503.Content = totalNG.ToString();
                    NA503.Content = totalNA.ToString();
                    break;
                case "504":
                    ALL504.Content = totalALL.ToString();
                    OK504.Content = totalOK.ToString();
                    NG504.Content = totalNG.ToString();
                    NA504.Content = totalNA.ToString();
                    break;
                case "505":
                    ALL505.Content = totalALL.ToString();
                    OK505.Content = totalOK.ToString();
                    NG505.Content = totalNG.ToString();
                    NA505.Content = totalNA.ToString();
                    break;
                case "506":
                    ALL506.Content = totalALL.ToString();
                    OK506.Content = totalOK.ToString();
                    NG506.Content = totalNG.ToString();
                    NA506.Content = totalNA.ToString();
                    break;
            }
            bool ON_ALARM = false;
            ///
            if (YRTNG_AB > SpecSEQ1)
            {
                /// Xử lý data rồi xuất ra 
                ErrorADD.Add("Line: " + numberlinexab + "\nSpec Defect Rate Line Over ");
                ON_ALARM = true;
            }
            else
            {
                if (YRTNG_A > YRTNG_B + SpecSEQ2 || YRTNG_B > +SpecSEQ2)
                {
                    ON_ALARM = true;
                    ErrorADD.Add("Line: " + numberlinexab + "\nSpec Lane A & B Over ");
                }
                else
                {
                    if (YRTTBA1 > YRTTBA2 + SpecSEQ3 || YRTTBA2 > YRTTBA1 + SpecSEQ3)
                    {
                        ON_ALARM = true;
                        ErrorADD.Add("Line: " + numberlinexab + "\nSpec Stage A1 & A2 Over ");
                    }
                    else if (YRTTBB1 > YRTTBB2 + SpecSEQ3 || YRTTBB2 > YRTTBB1 + SpecSEQ3)
                    {
                        ON_ALARM = true;
                        ErrorADD.Add("Line: " + numberlinexab + "\nSpec Stage B1 & B2 Over ");
                    }
                }
            }
            if (ON_ALARM == true)
            {
                XAML = true;
                string YRT = " ➤YRT \n⇢ Total: " + totalALL + " ea\n⇢ OK: " + totalOK + " ea ↔ " + YRTOK_AB + "%\n⇢ NG: " + totalNG + " ea ↔ " + YRTNG_AB + "%\n⇢ NA: " + totalNA + " ea ↔ " + YRTNA_AB + "% ";
                string LANE = "⟹ Lane A\n⇢ Total: " + totalA_A + " ea\n⇢ NG: " + totalNG_A + " ea ↔ " + YRTNG_A + "%\n⟹ Lane B: \n⇢ Total: " + totalB_B + " ea\n⇢ NG: " + totalNG_B + " ea ↔ " + YRTNG_B + "% ";
                ErrorADD.Add(YRT);
                ErrorADD.Add(LANE);
                ErrorADD.Add("\n");
            }
            ////
        }
        private Label FindLabel(string name)
        {
            return (Label)FindName(name);
        }
        private void SQLDefect_Dow()
        {
            List<string> SocicalDefect = new List<string>();
            foreach(var Item in tabledataquex)
            {
                List<string> Name_Defect = new List<string>();
                List<string> Name_DefectNG = new List<string>();
                List<int> TotalOK = new List<int>();
                List<int> TotalNG = new List<int>();
                List<int> TotalNA = new List<int>();
                Name_DefectNG.Clear();
                Name_Defect.Clear();
                int totalALL = Item.Tableyrtdt.Rows.Count;
                if (totalALL > 0)
                {
                    foreach (DataRow row in Item.Tableyrtdt.Rows)
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
                    SocicalDefect.Add("•Line: " + numberlinexab);
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
            }
            string XABV = string.Join(Environment.NewLine, SocicalDefect);
            outputdefec.Text = XABV;
        }
        private void TimerStart()
        {   
            if(manualtimer== true)
            {
                timertoday = Time_EN.Text;
                EndTime = Time_ST.Text;
            }
            else
            {
                timertoday = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                ACKTimer = Convert.ToInt32(Timeback.Text);
                DateTime now = DateTime.Now;
                TimeSpan hihi = TimeSpan.FromMinutes(ACKTimer);
                DateTime Result = now.Subtract(hihi);
                EndTime = Result.ToString("yyyy-MM-dd HH:mm:ss");
            }
            
        }

        private List<DataTableQueyy> tabledataquex;
        private static List<string> ErrorADD = new List<string>();
        private int ACKTimer;
        private string EndTime;
        private string timertoday;
        private string IP_Selection;
        public static List<string> selectedItems = new List<string>();
        public int Timerdelayloading;
        private string Databasexx;
        float SpecSEQ1;
        float SpecSEQ2;
        float SpecSEQ3;
        private string numberlinexab;
        private void SelectlineDropDown(object sender, EventArgs e)
        {
            if (selectline.Text == "IS_AOI")
            {
                cp301_checkbox.Content = "IS__301";
                cp302_checkbox.Content = "IS__302";
                cp303_checkbox.Content = "IS__303";
                cp304_checkbox.Content = "IS__304";
                cp305_checkbox.Content = "IS__305";
                cp306_checkbox.Content = "IS__306";
                cp401_checkbox.Content = "IS__401";
                cp402_checkbox.Content = "IS__402";
                cp403_checkbox.Content = "IS__403";
                cp404_checkbox.Content = "IS__404";
                cp405_checkbox.Content = "IS__405";
                cp406_checkbox.Content = "IS__406";
                cp501_checkbox.Content = "IS__501";
                cp502_checkbox.Content = "IS__502";
                cp503_checkbox.Content = "IS__503";
                cp504_checkbox.Content = "IS__504";
                cp505_checkbox.Content = "IS__505";
                cp506_checkbox.Content = "IS__506";
            }
            if (selectline.Text == "CP_AOI")
            {
                cp301_checkbox.Content = "CP__301";
                cp302_checkbox.Content = "CP__302";
                cp303_checkbox.Content = "CP__303";
                cp304_checkbox.Content = "CP__304";
                cp305_checkbox.Content = "CP__305";
                cp306_checkbox.Content = "CP__306";
                cp401_checkbox.Content = "CP__401";
                cp402_checkbox.Content = "CP__402";
                cp403_checkbox.Content = "CP__403";
                cp404_checkbox.Content = "CP__404";
                cp405_checkbox.Content = "CP__405";
                cp406_checkbox.Content = "CP__406";
                cp501_checkbox.Content = "CP__501";
                cp502_checkbox.Content = "CP__502";
                cp503_checkbox.Content = "CP__503";
                cp504_checkbox.Content = "CP__504";
                cp505_checkbox.Content = "CP__505";
                cp506_checkbox.Content = "CP__506";

            }
        }
        private void AddData_Defect_Drop(object sender, EventArgs e)
        {
            Griddata.Visibility = Visibility.Visible;
        }
        private void Cleardata()
        {
            string zero = "...";
            ALL301.Content = zero.ToString();
            OK301.Content = zero.ToString();
            NG301.Content = zero.ToString();
            NA301.Content = zero.ToString();
            ALL302.Content = zero.ToString();
            OK302.Content = zero.ToString();
            NG302.Content = zero.ToString();
            NA302.Content = zero.ToString();
            ALL303.Content = zero.ToString();
            OK303.Content = zero.ToString();
            NG303.Content = zero.ToString();
            NA303.Content = zero.ToString();
            ALL304.Content = zero.ToString();
            OK304.Content = zero.ToString();
            NG304.Content = zero.ToString();
            NA304.Content = zero.ToString();
            ALL305.Content = zero.ToString();
            OK305.Content = zero.ToString();
            NG305.Content = zero.ToString();
            NA305.Content = zero.ToString();
            ALL306.Content = zero.ToString();
            OK306.Content = zero.ToString();
            NG306.Content = zero.ToString();
            NA306.Content = zero.ToString();
            ALL401.Content = zero.ToString();
            OK401.Content = zero.ToString();
            NG401.Content = zero.ToString();
            NA401.Content = zero.ToString();
            ALL402.Content = zero.ToString();
            OK402.Content = zero.ToString();
            NG402.Content = zero.ToString();
            NA402.Content = zero.ToString();
            ALL403.Content = zero.ToString();
            OK403.Content = zero.ToString();
            NG403.Content = zero.ToString();
            NA403.Content = zero.ToString();
            ALL404.Content = zero.ToString();
            OK404.Content = zero.ToString();
            NG404.Content = zero.ToString();
            NA404.Content = zero.ToString();
            ALL405.Content = zero.ToString();
            OK405.Content = zero.ToString();
            NG405.Content = zero.ToString();
            NA405.Content = zero.ToString();
            ALL406.Content = zero.ToString();
            OK406.Content = zero.ToString();
            NG406.Content = zero.ToString();
            NA406.Content = zero.ToString();
            ALL501.Content = zero.ToString();
            OK501.Content = zero.ToString();
            NG501.Content = zero.ToString();
            NA501.Content = zero.ToString();
            ALL502.Content = zero.ToString();
            OK502.Content = zero.ToString();
            NG502.Content = zero.ToString();
            NA502.Content = zero.ToString();
            ALL503.Content = zero.ToString();
            OK503.Content = zero.ToString();
            NG503.Content = zero.ToString();
            NA503.Content = zero.ToString();
            ALL504.Content = zero.ToString();
            OK504.Content = zero.ToString();
            NG504.Content = zero.ToString();
            NA504.Content = zero.ToString();
            ALL505.Content = zero.ToString();
            OK505.Content = zero.ToString();
            NG505.Content = zero.ToString();
            NA505.Content = zero.ToString();
            ALL506.Content = zero.ToString();
            OK506.Content = zero.ToString();
            NG506.Content = zero.ToString();
            NA506.Content = zero.ToString();




        }
        private void checkimport(object sender, RoutedEventArgs e)
        {
            string line = LineData_Defect.Text;
            if (line == string.Empty)
            {
                MessageBox.Show("Không có chọn line");
                return;
            }
            foreach (var Item in tabledataquex)
            {
                if (line == Item.Line1)
                {
                    DataTable dt = Item.Tableyrtdt;
                    Datatbdefect.ItemsSource = dt.DefaultView;
                }
            }
        }
      
        private void buttonstop(object sender, RoutedEventArgs e)
        {
            TimerAX.Stop();
            buttonstart.Visibility = Visibility.Visible;
            buttonstopxa.Visibility = Visibility.Collapsed;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DataObject dataObject = new DataObject();
            StringBuilder clipboardText = new StringBuilder();
            foreach (var Item in tabledataquex)
            {
                DataTable dt = Item.Tabledatax;
                clipboardText.Append(GetClipboardText(dt));
            }
            dataObject.SetData(DataFormats.UnicodeText, clipboardText.ToString());
            Clipboard.SetDataObject(dataObject, true);
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

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {  string line = linedatacopyselect.Text;
            if (line != string.Empty)
            {
                DataObject dataObject = new DataObject();
                StringBuilder clipboardText = new StringBuilder();
                foreach (var Item in tabledataquex)
                {
                   if(line == Item.Line1)
                    {
                        DataTable dt = Item.Tabledatax;
                        int rowdt = dt.Rows.Count;
                        if(rowdt !=1)
                        {
                            clipboardText.Append(GetClipboardText(dt));
                        }    
                    }    
                }
                dataObject.SetData(DataFormats.UnicodeText, clipboardText.ToString());
                Clipboard.SetDataObject(dataObject, true);
            }    
        }

        private void checklinetabledataorg(object sender, RoutedEventArgs e)
        {
            string line = lineorgdata.Text;
            if (line != string.Empty)
            {
                foreach (var Item in tabledataquex)
                {
                    if (line == Item.Line1)
                    {
                        DataTable dt = Item.Tabledatax;
                        datatableorg.ItemsSource = dt.DefaultView;
                    }
                }
            }

        }
        private string[] listline;
        private void ListLine()
        {



        }
        string[] itemline = { "301", "302", "303", "304", "305", "306", "401", "402", "403", "404", "405", "406", "501", "502", "503", "504", "505", "506" };

        #region
        private void Unchecker_all(object sender, RoutedEventArgs e)
        {
            cp301_checkbox.IsChecked = false;
            cp302_checkbox.IsChecked = false;
            cp303_checkbox.IsChecked = false;
            cp304_checkbox.IsChecked = false;
            cp305_checkbox.IsChecked = false;
            cp306_checkbox.IsChecked = false;
            cp401_checkbox.IsChecked = false;
            cp402_checkbox.IsChecked = false;
            cp403_checkbox.IsChecked = false;
            cp404_checkbox.IsChecked = false;
            cp405_checkbox.IsChecked = false;
            cp406_checkbox.IsChecked = false;
            cp501_checkbox.IsChecked = false;
            cp502_checkbox.IsChecked = false;
            cp503_checkbox.IsChecked = false;
            cp504_checkbox.IsChecked = false;
            cp505_checkbox.IsChecked = false;
            cp506_checkbox.IsChecked = false;

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
        private void selectall_Checked(object sender, RoutedEventArgs e)
        {
            cp301_checkbox.IsChecked = true;
            cp302_checkbox.IsChecked = true;
            cp303_checkbox.IsChecked = true;
            cp304_checkbox.IsChecked = true;
            cp305_checkbox.IsChecked = true;
            cp306_checkbox.IsChecked = true;
            cp401_checkbox.IsChecked = true;
            cp402_checkbox.IsChecked = true;
            cp403_checkbox.IsChecked = true;
            cp404_checkbox.IsChecked = true;
            cp405_checkbox.IsChecked = true;
            cp406_checkbox.IsChecked = true;
            cp501_checkbox.IsChecked = true;
            cp502_checkbox.IsChecked = true;
            cp503_checkbox.IsChecked = true;
            cp504_checkbox.IsChecked = true;
            cp505_checkbox.IsChecked = true;
            cp506_checkbox.IsChecked = true;

        }
        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            Time_ST.Opacity = 0.5;
            Time_EN.Opacity = 0.5;
            Timeback.Opacity = 1;
            manualtimer = false;
        }
        private void checkbox_manualtimer(object sender, RoutedEventArgs e)
        {
            Time_ST.Opacity = 1;
            Time_EN.Opacity = 1;
            Timeback.Opacity = 0.5;
            manualtimer = true;
            //
            Time_EN.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            DateTime now = DateTime.Now;
            TimeSpan hihi = TimeSpan.FromMinutes(30);
            DateTime Result = now.Subtract(hihi);
            Time_ST.Text = Result.ToString("yyyy-MM-dd HH:mm:ss");
        }

        private void RunFor()
        {
            tabledataquex = new List<DataTableQueyy>();
            XAML = false;
            TimerStart();
            string HelloWWW = "Dear Sir\nAbnormal report Algorithm_Tech \nTimer : " + EndTime + " __to__ " + timertoday + ""; ErrorADD.Add(HelloWWW); ErrorADD.Add("\n");
            IP_Class iP_Class = new IP_Class();
            SQLLoading sqlloading = new SQLLoading();
            foreach (string item in selectedItems)
            {
                string connectionxx = sqlloading.DBShow(MachineName, item);
                iP_Class.IP_Selx(MachineName, item);
                IP_Selection = iP_Class.Ip_in;
                Databasexx = iP_Class.Data_Basexx;
                //
                if (IP_Selection != "1")
                {
                    LineData_Defect.Items.Add(item);
                    linedatacopyselect.Items.Add(item);
                    lineorgdata.Items.Add(item);
                    numberlinexab = item;

                    string connectionadb = "Server=" + IP_Selection + "; Database = " + Databasexx + "; Port=3306; User = ami; Password = protnc; charSet = utf8";
                    try
                    {
                        MySqlConnection connection = new MySqlConnection(connectionadb);
                        connection.Open();
                        if (connection.State == ConnectionState.Open)
                        {
                            string query = "SELECT * FROM product WHERE pt_datetime >= '" + EndTime + "' AND pt_datetime <= '" + timertoday + "'";
                            string sql = "SELECT judge, priority_defect_name, COUNT(*) AS count_defects, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" + EndTime + "' AND pt_datetime <= '" + timertoday + "'), 2) AS percent_defects FROM product WHERE pt_datetime >= '" + EndTime + "' AND pt_datetime <= '" + timertoday + "' GROUP BY judge, priority_defect_name ORDER BY priority_defect_name LIMIT 20000 ";
                            MySqlCommand Conx = new MySqlCommand(sql, connection);
                            MySqlCommand cmd = new MySqlCommand(query, connection);
                            MySqlDataAdapter Adater2 = new MySqlDataAdapter(Conx);
                            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                            DataTable sqlbaseTable = new DataTable();
                            DataTable SQLDefect = new DataTable();
                            Adater2.Fill(SQLDefect);
                            adapter.Fill(sqlbaseTable);
                            sqlbaseTable.Columns.Add("STT");
                            sqlbaseTable.Columns["STT"].SetOrdinal(0);
                            int ixb = 1, ibc = 1;
                            foreach (DataRow rowxa in sqlbaseTable.Rows)
                            {
                                rowxa["STT"] = ixb++;
                            }

                            SQLDefect.Columns.Add("STT");
                            SQLDefect.Columns["STT"].SetOrdinal(0);
                            foreach (DataRow rowxa in SQLDefect.Rows)
                            {
                                rowxa["STT"] = ibc++;
                            }
                            tabledataquex.Add(new DataTableQueyy(sqlbaseTable, SQLDefect, item));
                            DatabaseinTable(sqlbaseTable);
                            // SQLDefect_Dow(SQLDefect);
                            SQLDefect.Dispose();
                            sqlbaseTable.Dispose();
                        }
                    }
                    catch
                    {
                        MessageBox.Show($"Error Connected Line{item}");
                    }


                }
                //selectedItems.Clear();
                Thread.Sleep(10);
            }
            if (XAML == true)
            {
                string AVGX = string.Join(Environment.NewLine, ErrorADD);
                //AlarmTxbox.Text = AVGX;
            }
            else
            {
                string AVGX = "Nodata";
               // AlarmTxbox.Text = AVGX;
            }
            //adb.Close();
            selectedItems.Clear();
            /*string XABV = string.Join(Environment.NewLine, SocicalDefect);
            outputdefec.Text = XABV;
            SocicalDefect.Clear();
            adb.Close();*/
        }

        #endregion
    }

    public class DataTableQueyy
    {
        private DataTable tabledatax;
        private string  Line;
        private DataTable tableyrtdt;
        public DataTableQueyy(DataTable tabledata,DataTable tableyrrt , string linenumber)
        {
            this.tabledatax = tabledata;
            this.Line = linenumber;
            this.Tableyrtdt = tableyrrt;

        }
        public DataTable Tabledatax { get => tabledatax; set => tabledatax = value; }
        public string Line1 { get => Line; set => Line = value; }
        public DataTable Tableyrtdt { get => tableyrtdt; set => tableyrtdt = value; }
    }
}




    
