using Auto_Cls_Data.windownld;
//using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.ComponentModel;
using MySqlConnector;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using System.Windows.Shapes;
using System.Collections;
using Auto_Cls_Data.Data_Cal;
using System.Diagnostics.Eventing.Reader;
using static System.Net.Mime.MediaTypeNames;
using Auto_Cls_Data.Gplus;
using System.Windows.Forms.VisualStyles;
using static System.Windows.Forms.LinkLabel;
using System.Windows.Forms;
using System.Drawing;
using DataObject = System.Windows.DataObject;
using DataFormats = System.Windows.DataFormats;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.Forms.MessageBox;
using Newtonsoft.Json;
using Xceed.Wpf.Toolkit;
using Application = System.Windows.Application;
using Serilog;
using System.Web.Hosting;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace Auto_Cls_Data
{
 
    public partial class Seachdata : Window
    {
        NewLoading newloading;
        SQLLoading sqload;
        IP_Class iP_Class;
        CGAOIPlus cgaoiplus;
        DataTable CG_PlusA;
        DataTable CG_PlusB;
        BackgroundWorker loading;
        BackgroundWorker seachdd;
        SeachDataVol2 seachexport;
        public Seachdata()
        {
            InitializeComponent();
            
            this.Loaded += lddatime;
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            Log.Logger = new LoggerConfiguration()
                    .WriteTo.File($"Log\\log{timestamp}.txt")
                    .CreateLogger();
            this.Loaded += MainWindown_Loadi;
            Seachpanelid.IsChecked = true;
        }
        
        private void MainWindown_Loadi(object sender, RoutedEventArgs e)
        {
            loading = new BackgroundWorker();
            loading.WorkerSupportsCancellation = true;
            loading.DoWork += loading_DoWork;
            loading.RunWorkerCompleted += loading_RunWorkerCompleted;
            seachdd = new BackgroundWorker();
            seachdd.WorkerSupportsCancellation = true;
            seachdd.DoWork += seachdd_DoWork;
            seachdd.RunWorkerCompleted += seachdd_RunWorkerCompleted;
            string Filex = "config\\Prefence.json";
            if (!File.Exists(Filex))
            {
                MessageBox.Show("No RecipeFile");
                return;
            }
            string ADB = File.ReadAllText(Filex);
            ALGOTech = JsonConvert.DeserializeObject<AlgorithmDLL>(ADB);



            //string Fileprefence = "config\\Prefence.json";
            /* string ADB = File.ReadAllText(Fileprefence);
             Prefence prefence = JsonConvert.DeserializeObject< Prefence>( ADB );
             this.Width = prefence.Monitor.Width;
             this.Height = prefence.Monitor.Height;
             this.MaxHeight = prefence.Monitor.Height;
             this.MaxWidth = prefence.Monitor.Width;*/
        }
        AlgorithmDLL ALGOTech;
        private void seachdd_DoWork(object sender, DoWorkEventArgs e)
        {
            sqload = new SQLLoading();
            SeachDataCls seachDataCls = new SeachDataCls();

            string Machine = string.Empty;
            string Line = string.Empty;
            string IDList = string.Empty;
            int Length = 0;
            bool seachpanelxx = false;
            Dispatcher.Invoke(() =>
            {
                 Length = BoxID.Text.Length;
                 Machine = MachineSelection.Text;
                 Line = LineSelection.Text;
                 IDList = BoxID.Text;
                 seachpanelxx = seachpanel;
            });
            try
            {
                DataTable seachdata = seachexport.SeachData(Machine, Line, Length, IDList,seachpanelxx);
                datatable = seachdata;
                Dispatcher.Invoke(() =>
                {
                    if (seachdata != null && checkerdataintable.IsChecked == true)
                    {
                        tablebase.ItemsSource = seachdata.DefaultView;
                    }
                });
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                Log.Error("Err: " + ex);
                return;
            }
            
        }
        private void seachdd_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (MachineSelection.Text != "CG_AOI_Plus")
            {
                if (datatable != null) { Caculator(datatable); }
            }
            loadingwindown.Visibility = Visibility.Collapsed;
        }
        private void loading_DoWork(object sender , DoWorkEventArgs e)
        {
            string Machine = string.Empty;
            string Line = string.Empty;
            int DataLimit = 0;
            string TimerST = string.Empty;
            string TimerEN = string.Empty;
            string Judge = string.Empty;
            string Defection = string.Empty;
            Dispatcher.Invoke(() =>
            {
                 Machine = MachineSelection.Text;
                 Line = LineSelection.Text;
                 DataLimit = Convert.ToInt32(data_limit_seach.Text);
                 TimerST = Time_ST.Text;
                 TimerEN = Time_EN.Text;
                 Judge = Judgeslection.Text;
                 Defection = defectselection.Text;
                
            });
            if (Machine == string.Empty || Line == string.Empty || Judge == string.Empty)
            {
                MessageBox.Show("Pls Selection Data");
                return;
            }
            /// UI Guid 
            /// 
            string Database = string.Empty;
            string IP = string.Empty;
            foreach(var item in ALGOTech.Databasex)
            {
                if(item.Machine == Machine)
                {
                    Database = item.Database;
                }
            }
            foreach(var item in ALGOTech.DataAlgorithm)
            {
                if(item.Machine == Machine)
                {
                    foreach (var IPD in item.DataORG)
                    {
                        if(IPD.Name == Line)
                        {
                            IP = IPD.IP;
                        }    
                    }
                }
            }
            string connectionx = $"Server={IP}; Database={Database};User = ami; Password = protnc";
            
            if (Machine != "CG_AOI_Plus" && IP != string.Empty)
            {
                sqload = new SQLLoading();
                try
                {
                    DataTable base_table = seachexport.DataBaseQuery(connectionx, TimerST, TimerEN, DataLimit, Judge, Defection);
                    DataTable yrt_table = seachexport.DataYRTQuery(connectionx,Machine, TimerST, TimerEN, DataLimit);
                    Dispatcher.Invoke(() =>
                    { 
                        if(base_table != null && checkerdataintable.IsChecked == true) { tablebase.ItemsSource = base_table.DefaultView;}
                        if (yrt_table != null) { table2.ItemsSource = yrt_table.DefaultView; }
                    });
                    datatable = base_table;
                    var rsdatable = new List<DataTable> { base_table, yrt_table };
                    if (Machine == "Assy_AMI" || Machine == "CP_AOI" || Machine == "LT_AMI")
                    {
                        DataTable datatb = seachexport.LDTable(Machine, Line, TimerST, TimerEN);
                        Dispatcher.Invoke(() =>
                        {
                            if (datatb != null ) { tablechanel.ItemsSource = datatb.DefaultView; }
                        });
                        datatb.Dispose();
                    }
                    base_table.Dispose();
                    yrt_table.Dispose();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex);
                    Log.Error("Err: "+ex);
                    return;
                }
            }
            else if(Machine == "CG_AOI_Plus")
            {
                Database = string.Empty;
                foreach (var item in ALGOTech.Databasex)
                {
                    if (item.Machine == "CG_AOI")
                    {
                        Database = item.Database;
                    }
                }
                foreach (string Item in IPMachine_CGPlus)
                {
                    string LaneX = Item.Substring(4, 1);
                    string LineCGPlus = $"{Line}_{LaneX}";
                    try
                    {
                        
                        IP = string.Empty;
                        foreach (var item in ALGOTech.DataAlgorithm)
                        {
                            if (item.Machine == "CG_AOI")
                            {
                                foreach (var IPD in item.DataORG)
                                {
                                    if (IPD.Name == LineCGPlus)
                                    {
                                        IP = IPD.IP;
                                    }
                                }
                            }
                        }
                        connectionx = $"Server={IP}; Database={Database};User = ami; Password = protnc";
                        if (LaneX == "A")
                        {

                            DataTable DatatableA = seachexport.CGAOIPLS(connectionx, LineCGPlus, TimerST, TimerEN, DataLimit);
                            DataTable BaseTb = seachexport.CGAOIYRTPLUS(connectionx, LineCGPlus, TimerST, TimerEN, DataLimit, Judge, Defection);
                            Dispatcher.Invoke(() =>
                            {
                                table2.ItemsSource = DatatableA.DefaultView;
                                if (checkerdataintable.IsChecked == true)
                                {
                                    tablebase.ItemsSource = BaseTb.DefaultView;
                                }
                                // 
                            });
                            CG_PlusA = BaseTb;
                        }
                        else if (LaneX == "B")
                        {
                            DataTable DatatableA = seachexport.CGAOIPLS(connectionx, LineCGPlus, TimerST, TimerEN, DataLimit);
                            DataTable BaseTb = seachexport.CGAOIYRTPLUS(connectionx, LineCGPlus, TimerST, TimerEN, DataLimit, Judge, Defection);
                            Dispatcher.Invoke(() =>
                            {
                                TableB.ItemsSource = DatatableA.DefaultView;
                                if (checkerdataintable.IsChecked == true)
                                {
                                    Lane_B_CG.ItemsSource = BaseTb.DefaultView;
                                }
                            });
                            CG_PlusB = BaseTb;
                        }
                    }
                    catch(Exception ex)
                    {
                        MessageBox.Show("Error: " + ex);
                        Log.Error("Err: " + ex);
                        return;
                    }
                    
                }
            }
        }
        
        private void loading_RunWorkerCompleted(object sender , RunWorkerCompletedEventArgs e)
        {
            if(MachineSelection.Text != "CG_AOI_Plus")
            {
                if(datatable != null) { Caculator(datatable); }
            }
            else
            { if(CG_PlusA != null && CG_PlusB != null) 
                {
                    CaculatorCGPlusA(CG_PlusA);
                    CaculatorCGPlusB(CG_PlusB);
                    Catital();
                }
            }
            loadingwindown.Visibility = Visibility.Collapsed;
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        private void lddatime(object sender ,RoutedEventArgs e)
        {
            Seachpanelid.IsChecked = false;
            Time_EN.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            DateTime now = DateTime.Now;
            TimeSpan hihi = TimeSpan.FromMinutes(120);
            DateTime Result = now.Subtract(hihi);
            Time_ST.Text = Result.ToString("yyyy-MM-dd HH:mm");
        }
        private void Timersetnow(object sender, RoutedEventArgs e)
        {
            Time_EN.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
        public void StartSeach(object sender, RoutedEventArgs e)
        {
            if(!loading.IsBusy && !seachdd.IsBusy)
            {
                seachexport = new SeachDataVol2();
                CleardatatableGrid();
                loadingwindown.Visibility = Visibility.Visible;
                loading.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Is Running");
            }
        }
        private void SeachID(object sender, RoutedEventArgs e)
        {
            if (!loading.IsBusy && !seachdd.IsBusy && MachineSelection.Text != "CG_AOI_Plus")
            {
                seachexport = new SeachDataVol2();
                CleardatatableGrid();
                loadingwindown.Visibility = Visibility.Visible;
                seachdd.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("Is Running");
            }
            
        }
        public void Caculator(DataTable table)
        {
            ClearData();
            // Tính toán giá trị rồi đưa ra bảng mini bên trái
            int countNG = 0;
            int countOK = 0;
            int countNA = 0;
            int totalRows = table.Rows.Count;
            /// CP AOI Improvement
            int totalOK_A = 0;
            int totalNG_A = 0;
            int totalNA_A = 0;
            //
            int totalOK_B = 0;
            int totalNG_B = 0;
            int totalNA_B = 0;
            //
            int totalA_A = 0;
            int totalB_B = 0;
            /// 
            string rowpid = "";
            foreach (DataRow row in table.Rows)
            {
                if (MachineSelection.Text == "IS_AOI")
                {
                    rowpid = row["inspection_zone"].ToString();
                    /// total
                    if (row["inspection_zone"].ToString() != null)
                    {

                        string valueA = rowpid[0].ToString();

                        if (valueA == "A")
                        {
                            totalA_A++;
                        }
                        if (valueA == "B")
                        {
                            totalB_B++;
                        }
                    }
                    if (row["judge"].ToString() == "NG")
                    {
                        rowpid = row["inspection_zone"].ToString();
                        if (row["inspection_zone"].ToString() != null)
                        {
                            string valueA = rowpid[0].ToString();
                            if (valueA == "A")
                            {
                                totalNG_A++;
                            }
                            if (valueA == "B")
                            {
                                totalNG_B++;
                            }
                        }
                        countNG++;
                    }
                    if (row["judge"].ToString() == "OK")
                    {
                        rowpid = row["inspection_zone"].ToString();
                        if (row["inspection_zone"].ToString() != null)
                        {
                            string valueA = rowpid[0].ToString();
                            if (valueA == "A")
                            {
                                totalOK_A++;
                            }
                            if (valueA == "B")
                            {
                                totalOK_B++;
                            }
                        }
                        countOK++;
                    }
                    if (row["judge"].ToString() != "NG" && row["judge"].ToString() != "OK")
                    {
                        rowpid = row["inspection_zone"].ToString();
                        if (row["inspection_zone"].ToString() != null)
                        {
                            string valueA = rowpid[0].ToString();
                            if (valueA == "A")
                            {
                                totalNA_A++;
                            }
                            if (valueA == "B")
                            {
                                totalNA_B++;
                            }
                        }
                        countNA++;
                    }
                }
                else if (MachineSelection.Text == "CP_AOI")
                {
                    rowpid = row["localid"].ToString();
                    string[] splitValues = rowpid.Split('_');
                    string valueA = splitValues[1].Substring(0, 1);
                    /// total
                    if (row["localid"].ToString() != null)
                    {
                        
                        if (valueA == "A")
                        {
                            totalA_A++;
                        }
                        if (valueA == "B")
                        {
                            totalB_B++;
                        }
                    }
                    if (row["judge"].ToString() == "N")
                    {
                        if (row["localid"].ToString() != null)
                        {
                           
                            if (valueA == "A")
                            {
                                totalNG_A++;
                            }
                            if (valueA == "B")
                            {
                                totalNG_B++;
                            }
                        }
                        countNG++;
                    }
                    if (row["judge"].ToString() == "G")
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
                        countOK++;
                    }
                    if (row["judge"].ToString() != "N" && row["judge"].ToString() != "G")
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
                        countNA++;
                    }
                }
                else if (MachineSelection.Text == "CG_AOI")
                {
                    rowpid = row["pid"].ToString();
                    /// total
                    if (row["pid"].ToString() != null)
                    {
                        string valueA = rowpid[0].ToString();
                        if (valueA == "1")
                        {
                            totalA_A++;
                        }
                        if (valueA == "2")
                        {
                            totalB_B++;
                        }
                    }
                    if (row["judge"].ToString() == "NG")
                    {
                        //rowpid = row["inspection_zone"].ToString();
                        if (row["pid"].ToString() != null)
                        {
                            string valueA = rowpid[0].ToString();
                            if (valueA == "1")
                            {
                                totalNG_A++;
                            }
                            if (valueA == "2")
                            {
                                totalNG_B++;
                            }
                        }
                        countNG++;
                    }
                    if (row["judge"].ToString() == "OK")
                    {
                        //rowpid = row["inspection_zone"].ToString();
                        if (row["pid"].ToString() != null)
                        {
                            string valueA = rowpid[0].ToString();
                            if (valueA == "1")
                            {
                                totalOK_A++;
                            }
                            if (valueA == "2")
                            {
                                totalOK_B++;
                            }
                        }
                        countOK++;
                    }
                    if (row["judge"].ToString() != "NG" && row["judge"].ToString() != "OK")
                    {
                        //rowpid = row["inspection_zone"].ToString();
                        if (row["pid"].ToString() != null)
                        {
                            string valueA = rowpid[0].ToString();
                            if (valueA == "1")
                            {
                                totalNA_A++;
                            }
                            if (valueA == "2")
                            {
                                totalNA_B++;
                            }
                        }
                        countNA++;
                    }
                    

                    /// Total cọc đầu tiên

                }
                else if (MachineSelection.Text == "LT_AMI")
                {
                    rowpid = row["display_insp"].ToString();
                    /// total
                    if (row["judge"].ToString() != null)
                    {
                        string valueA = rowpid[0].ToString();
                        if (valueA == "1" || valueA == "2" ||valueA == "3" || valueA == "4")
                        {
                            totalA_A++;
                        }
                        if (valueA == "5" || valueA == "6" || valueA == "7" || valueA == "8")
                        {
                            totalB_B++;
                        }
                    }
                    if (row["judge"].ToString() == "NG")
                    {
                        //rowpid = row["inspection_zone"].ToString();
                        string valueA = rowpid[0].ToString();
                        if (valueA == "1" || valueA == "2" || valueA == "3" || valueA == "4")
                        {
                            totalNG_A++;
                        }
                        if (valueA == "5" || valueA == "6" || valueA == "7" || valueA == "8")
                        {
                            totalNG_B++;
                        }


                        countNG++;
                    }
                    if (row["judge"].ToString() == "OK")
                    {
                        //rowpid = row["inspection_zone"].ToString();
                        string valueA = rowpid[0].ToString();
                        if (valueA == "1" || valueA == "2" || valueA == "3" || valueA == "4")
                        {
                            totalOK_A++;
                        }
                        if (valueA == "5" || valueA == "6" || valueA == "7" || valueA == "8")
                        {
                            totalOK_B++;
                        }


                        countOK++;
                    }
                    if (row["judge"].ToString() != "NG" && row["judge"].ToString() != "OK")
                    {
                        //NA Comple
                        if (row["judge"].ToString() != null)
                        {
                            string valueA = rowpid[0].ToString();
                            if (valueA == "1" || valueA == "2" || valueA == "3" || valueA == "4")
                            {
                                totalNA_A++;
                            }
                            if (valueA == "5" || valueA == "6" || valueA == "7" || valueA == "8")
                            {
                                totalNA_B++;
                            }
                        }
                        countNA++;
                    }


                    /// Total cọc đầu tiên
                    

                }
                else if (MachineSelection.Text == "Assy_AMI")
                {

                    if (row["judge"].ToString() == "N")
                    {
                        //rowpid = row["inspection_zone"].ToString();
                        
                        countNG++;
                    }
                    if (row["judge"].ToString() == "G")
                    {
                        //rowpid = row["inspection_zone"].ToString();
                        
                        countOK++;
                    }
                    if (row["judge"].ToString() != "G" && row["judge"].ToString() != "N")
                    {
                        //NA Comple
                        
                        countNA++;
                    }
                }
                TotalOKA.Content = totalOK_A.ToString();
                TotalNGA.Content = totalNG_A.ToString();
                TotalNAA.Content = totalNA_A.ToString();
                //
                TotalOKB.Content = totalOK_B.ToString();
                TotalNGB.Content = totalNG_B.ToString();
                TotalNAB.Content = totalNA_B.ToString();
                //
                TotalAAA.Content = totalA_A.ToString();
                TotalBBB.Content = totalB_B.ToString();
                /// total cọc ngoài cùng
                TotalNGAB.Content = countNG.ToString();
                TotalOKAB.Content = countOK.ToString();
                TotalNAAB.Content = countNA.ToString();
                TotalALLAB.Content = totalRows.ToString();
                //
                float AA_A = float.Parse(TotalAAA.Content.ToString());
                float A_OK = float.Parse(TotalOKA.Content.ToString());
                float A_NG = float.Parse(TotalNGA.Content.ToString());
                float A_NA = float.Parse(TotalNAA.Content.ToString());
                //
                float BB_B = float.Parse(TotalBBB.Content.ToString());
                float B_OK = float.Parse(TotalOKB.Content.ToString());
                float B_NG = float.Parse(TotalNGB.Content.ToString());
                float B_NA = float.Parse(TotalNAB.Content.ToString());

                float TotalALL_AB = float.Parse(TotalALLAB.Content.ToString());
                float NGALLAB = float.Parse(TotalNGAB.Content.ToString());
                float NAALLAB = float.Parse(TotalNAAB.Content.ToString());
                float OKALLAB = float.Parse(TotalOKAB.Content.ToString());
                //
                float YRTOK_AB = (OKALLAB / TotalALL_AB) * 100;
                YRTALLOKAB.Content = YRTOK_AB.ToString("#.#") + "%";
                float YRTNG_AB = (NGALLAB / TotalALL_AB) * 100;
                YRTALLNGAB.Content = YRTNG_AB.ToString("#.#") + "%";
                float YRTNA_AB = (NAALLAB / TotalALL_AB) * 100;
                YRTALLNAAB.Content = YRTNA_AB.ToString("#.#") + "%";


                float YRTOK_A = (A_OK / AA_A) * 100;
                YRTOKA.Content = YRTOK_A.ToString("#.#") + "%";
                float YRTNG_A = (A_NG / AA_A) * 100;
                YRTNGA.Content = YRTNG_A.ToString("#.#") + "%";
                float YRTNA_A = (A_NA / AA_A) * 100;
                YRTNAA.Content = YRTNA_A.ToString("#.#") + "%";
                //
                float YRTOK_B = (B_OK / BB_B) * 100;
                YRTOKB.Content = YRTOK_B.ToString("#.#") + "%";
                float YRTNG_B = (B_NG / BB_B) * 100;
                YRTNGB.Content = YRTNG_B.ToString("#.#") + "%";
                float YRTNA_B = (B_NA / BB_B) * 100;
                YRTNAB.Content = YRTNA_B.ToString("#.#") + "%";
            }
        }
        private void copydatatoclipboard(object sender, RoutedEventArgs e)
         {
            DataObject dataObject = new DataObject();
            if (MachineSelection.Text != "CG_AOI_Plus")
            {
                if (datatable == null)
                {
                    return;
                }
                dataObject.SetData(DataFormats.UnicodeText, GetClipboardText(datatable));
                Clipboard.SetDataObject(dataObject, true);
            }
            else
            {
                if(CG_PlusA == null || CG_PlusB == null)
                { return; }
                
                StringBuilder clipboardText = new StringBuilder();
                clipboardText.Append(GetClipboardText(CG_PlusA));
                clipboardText.Append(GetClipboardText(CG_PlusB));
                dataObject.SetData(DataFormats.UnicodeText, clipboardText.ToString());
                Clipboard.SetDataObject(dataObject, true);


            }
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
        private void tablebase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
        }
        private void tablebase_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
        }
        #region CGByLoading
        private void CaculatorCGPlusA(DataTable table)
        {
            ClearData();
            int countNG = 0;
            int countOK = 0;
            int countNA = 0;
            int totalRows = table.Rows.Count;
            /// CP AOI Improvement
            foreach (DataRow row in table.Rows)
            {
                if (row["judge"].ToString() == "NG")
                {
                    countNG++;
                }
                else if (row["judge"].ToString() == "OK")
                {
                    countOK++;
                }
                else if (row["judge"].ToString() != "NG" && row["judge"].ToString() != "OK")
                {
                    countNA++;
                }
            }
            TotalOKA.Content = countOK.ToString();
            TotalNGA.Content = countNG.ToString();
            TotalNAA.Content = countNA.ToString();
            TotalAAA.Content = totalRows.ToString();
            AA_A = float.Parse(TotalAAA.Content.ToString());
            A_OK = float.Parse(TotalOKA.Content.ToString());
            A_NG = float.Parse(TotalNGA.Content.ToString());
            A_NA = float.Parse(TotalNAA.Content.ToString());

            float YRTOK_A = (A_OK / AA_A) * 100;
            YRTOKA.Content = YRTOK_A.ToString("#.#") + "%";
            float YRTNG_A = (A_NG / AA_A) * 100;
            YRTNGA.Content = YRTNG_A.ToString("#.#") + "%";
            float YRTNA_A = (A_NA / AA_A) * 100;
            YRTNAA.Content = YRTNA_A.ToString("#.#") + "%";
        }
        float BB_B;
        float B_OK;
        float B_NG;
        float B_NA;
        private void CaculatorCGPlusB(DataTable table)
        {
            int countNG = 0;
            int countOK = 0;
            int countNA = 0;
            int totalRows = table.Rows.Count;
            /// CP AOI Improvement
            foreach (DataRow row in table.Rows)
            {
                if (row["judge"].ToString() == "NG")
                {
                    countNG++;
                }
                else if (row["judge"].ToString() == "OK")
                {
                    countOK++;
                }
                else if (row["judge"].ToString() != "NG" && row["judge"].ToString() != "OK")
                {
                    countNA++;
                }
            }

            TotalOKB.Content = countOK.ToString();
            TotalNGB.Content = countNG.ToString();
            TotalNAB.Content = countNA.ToString();
            TotalBBB.Content = totalRows.ToString();
            BB_B = float.Parse(TotalBBB.Content.ToString());
            B_OK = float.Parse(TotalOKB.Content.ToString());
            B_NG = float.Parse(TotalNGB.Content.ToString());
            B_NA = float.Parse(TotalNAB.Content.ToString());

            float YRTOK_B = (B_OK / BB_B) * 100;
            YRTOKB.Content = YRTOK_B.ToString("#.#") + "%";
            float YRTNG_B = (B_NG / BB_B) * 100;
            YRTNGB.Content = YRTNG_B.ToString("#.#") + "%";
            float YRTNA_B = (B_NA / BB_B) * 100;
            YRTNAB.Content = YRTNA_B.ToString("#.#") + "%";
        }
        float AA_A;
        float A_OK;
        float A_NG;
        float A_NA;
        private void Catital()
        {

            float TotalALL_AB = AA_A + BB_B;
            float NGALLAB = A_NG + B_NG;
            float NAALLAB = A_NA + B_NA;
            float OKALLAB = A_OK + B_OK;

            TotalALLAB.Content = TotalALL_AB;
            TotalOKAB.Content = OKALLAB;
            TotalNGAB.Content = NGALLAB;
            TotalNAAB.Content = NAALLAB;

            float YRTOK_AB = (OKALLAB / TotalALL_AB) * 100;
            YRTALLOKAB.Content = YRTOK_AB.ToString("#.#") + "%";
            float YRTNG_AB = (NGALLAB / TotalALL_AB) * 100;
            YRTALLNGAB.Content = YRTNG_AB.ToString("#.#") + "%";
            float YRTNA_AB = (NAALLAB / TotalALL_AB) * 100;
            YRTALLNAAB.Content = YRTNA_AB.ToString("#.#") + "%";
        }
        #endregion
        private DataTable datatable;
        public DataTable sqlbaseTable;
        public MySqlConnection connection;
        private List<string> IPMachine_CGPlus = new List<string>();
        #region TableShow and Hide
        private void TableB_SelectedCellsChanged_1(object sender, SelectedCellsChangedEventArgs e)
        {
            string A = "B";
            string Line = LineSelection.Text;
            string LineSelec = $"{Line}_{A}";
            string TimerST = Time_ST.Text;
            string TimerEN = Time_EN.Text;
            if (TableB.SelectedCells != null && TableB.SelectedCells.Count > 0)
            {
                DataGridCellInfo cellInfo = TableB.SelectedCells[1];
                object data = cellInfo.Item;
                DataRowView rowXY = data as DataRowView; // Ép kiểu sang DataRowView nếu cần thiết
                string name_seach = rowXY["priority_defect_name"].ToString();
                string Machinex = MachineSelection.Text;
                newloading = new NewLoading();
                sqload = new SQLLoading();
                string Connect = sqload.DBShow(Machinex, LineSelec);
                try
                {
                    connection = new MySqlConnection(Connect); //charSet = utf8"
                    connection.Open();
                    if (connection.State == System.Data.ConnectionState.Open)
                    {

                        string query = newloading.TableBselection(TimerST, TimerEN,name_seach);
                        MySqlCommand command = new MySqlCommand(query, connection);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataTable loadingmapping = new DataTable();
                        adapter.Fill(loadingmapping);

                        loadingmapping.Columns.Add("STT");
                        loadingmapping.Columns["STT"].SetOrdinal(0);
                        int ixb = 1;
                        foreach (DataRow rowxa in loadingmapping.Rows)
                        {
                            rowxa["STT"] = ixb++;
                        }
                        tabledt.ItemsSource = loadingmapping.DefaultView;
                        //
                    }
                    connection.Close();
                    return;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Step BY1 CONNECTION FAIL " + ex);
                    connection.Close();
                    return;
                }
            }
        }
        private void table2_SelectedCellsChanged_1(object sender, SelectedCellsChangedEventArgs e)
        {
            string Machine = MachineSelection.Text;
            string Line = LineSelection.Text;
            string CG_Plus_LineSelec = $"{Line}_A";
            string TimerST = Time_ST.Text;
            string TimerEN = Time_EN.Text;
            newloading = new NewLoading();
            sqload = new SQLLoading();
            if (table2.SelectedCells != null && table2.SelectedCells.Count > 0)
            {
                DataGridCellInfo cellInfo = table2.SelectedCells[1];
                object data = cellInfo.Item;
                DataRowView rowXY = data as DataRowView; // Ép kiểu sang DataRowView nếu cần thiết
                string name_seach;
                if (Machine == "Assy_AMI")
                {
                    name_seach = rowXY["final_defect_name"].ToString();
                }
                else
                {
                    name_seach = rowXY["priority_defect_name"].ToString();
                }
                try
                {
                    string Connect;
                    if (Machine == "CG_AOI_Plus")
                    {
                        Connect = sqload.DBShow(Machine, CG_Plus_LineSelec);
                    }
                    else
                    {
                        Connect = sqload.DBShow(Machine, Line);
                    }
                    connection = new MySqlConnection(Connect);
                    connection.Open();
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        string query = newloading.SelectionTable(TimerST, TimerEN, name_seach,Machine);
                        MySqlCommand command = new MySqlCommand(query, connection);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataTable loadingmapping = new DataTable();
                        adapter.Fill(loadingmapping);
                        loadingmapping.Columns.Add("STT");
                        loadingmapping.Columns["STT"].SetOrdinal(0);
                        int ixb = 1;
                        foreach (DataRow rowxa in loadingmapping.Rows)
                        {
                            rowxa["STT"] = ixb++;
                        }
                        tabledt.ItemsSource = loadingmapping.DefaultView;
                    }
                    connection.Close();
                    return;
                }
                catch 
                {
                    MessageBox.Show("Error: Step BY1 CONNECTION FAIL ");
                    connection.Close();
                    return;
                }
            }
           
        }
        bool seachpanel = false;
        #region
        private void Judgeslection_DropDownClosed(object sender, EventArgs e)
        {
            defectselection.IsEnabled = false;
            buttonstart.IsEnabled = false;
            defectselection.Opacity = 0.5;
            buttonstart.Opacity = 0.5;

            if(Judgeslection.Text != string.Empty)
            {
                buttonstart.IsEnabled = true;
                buttonstart.Opacity = 1;
            }    

            switch (Judgeslection.Text)
            {
                case "OK":
                case "G":
                case "ALL":
                    {
                        defectselection.Opacity = 0.5;
                        defectselection.IsEnabled = false;
                    }
                    break;
                case "NG":
                case "N":
                    {
                        defectselection.Opacity = 1;
                        defectselection.IsEnabled = true;
                        buttonstart.IsEnabled = false;
                        buttonstart.Opacity = 0.5;

                    }
                    break;
            }

        }
        private void Defectloading()
        {
            ///Defect được yêu cầu loading và tên line
            ///
            string Machineselec = MachineSelection.Text;
            LineSelection.Items.Clear();
            defectselection.Items.Clear();
            Judgeslection.Items.Add("ALL");
            defectselection.Items.Add("ALL");
            switch (Machineselec)
            {

               case "IS_AOI":
               case "LT_AMI":
                case "CG_AOI_Plus":
                case "CG_AOI":
                    {
                        Judgeslection.Items.Add("OK");
                        Judgeslection.Items.Add("NG");


                        foreach (var item in ALGOTech.DataAlgorithm)
                        {
                            if (Machineselec == item.Machine)
                            {
                                foreach (string add in item.Defect)
                                {
                                    defectselection.Items.Add(add);
                                }
                            }
                        }

                        /* string file_CGAOI = "config/CGAOI_Defect.txt";
                         using (StreamReader readerCG = new StreamReader(file_CGAOI))
                         {
                             string lineCG;
                             while ((lineCG = readerCG.ReadLine()) != null)
                             {
                                 defectselection.Items.Add(lineCG);
                             }
                         }*/
                    }
                    break;
                case "Assy_AMI":
                case "CP_AOI":
                    {

                        Judgeslection.Items.Add("N");
                        Judgeslection.Items.Add("G");


                        foreach (var item in ALGOTech.DataAlgorithm)
                        {
                            if (Machineselec == item.Machine)
                            {
                                foreach (string add in item.Defect)
                                {
                                    defectselection.Items.Add(add);
                                }
                            }
                        }

                        /*string file_CPAOI = "config/CPAOI_Defect.txt";
                        using (StreamReader readerCP = new StreamReader(file_CPAOI))
                        {
                            string lineCP;
                            while ((lineCP = readerCP.ReadLine()) != null)
                            {
                                defectselection.Items.Add(lineCP);
                            }
                        }*/
                    }
                    break;

            };

        }
        private void Line_Sel_DropDownClosed(object sender, EventArgs e)
        {
            Judgeslection.Items.Clear();
            defectselection.Items.Clear();
            LineSelection.IsEnabled = false;
            Time_ST.IsEnabled = false;
            Time_EN.IsEnabled = false;
            Judgeslection.IsEnabled = false;
            defectselection.IsEnabled = false;
            buttonstart.IsEnabled = false;
            LineSelection.Opacity = 0.5;
            Time_ST.Opacity = 0.5;
            Time_EN.Opacity = 0.5;
            Judgeslection.Opacity = 0.5;
            defectselection.Opacity = 0.5;
            buttonstart.Opacity = 0.5;
            if (MachineSelection.Text != string.Empty)
            {
                LineSelection.Opacity = 1;
                LineSelection.IsEnabled = true;
            } 
            Defectloading();
            LaneALTAMI.Content = "Lane A";
            LaneBLTAMI.Content = "Lane B";
            tablebase.Visibility = Visibility.Collapsed;
            Mapping_CGAOI.Visibility = Visibility.Collapsed;
            TableB.Visibility = Visibility.Collapsed;
            table2.Visibility = Visibility.Collapsed;
            Name_laneA.Content = "Table Defect";
            Name_laneB.Visibility = Visibility.Collapsed;
            Buttoncopyclipboard.Visibility = Visibility.Visible;
            gridchaneltable.Visibility = Visibility.Collapsed;

            //tableIDpanel.Margin = new Thickness(463, 59, 684, 10);
            contentableidpanel.Content = "Table__Defect__ID__Mapping";
            checkerdataintable.IsEnabled = true;
            // 0,10,10,53
            tablebase.Margin = new Thickness(0, 10, 10, 53);
            Lane_B_CG.Visibility = Visibility.Collapsed;
            Mapping_CGAOI.Margin = new Thickness(1103, 64, 0, 0);
            Mapping_CGAOI.Width = 344;
            string Machineselec = MachineSelection.Text;
            //Table Loading
            CleardatatableGrid();
            foreach (var item in ALGOTech.DataAlgorithm)
            {
                if (item.Machine == Machineselec)
                {
                    foreach (var itemadd in item.DataORG)
                    {
                        LineSelection.Items.Add(itemadd.Name);
                    }
                }
            }
            switch (Machineselec)
            {
                case "CG_AOI":
                    {//Height="886" Margin="1103,64,0,0" VerticalAlignment="Top" Visibility="Visible" Width="344"
                        table2.Visibility = Visibility.Visible;
                        LaneALTAMI.Content = "Stage 1";
                        LaneBLTAMI.Content = "Stage 2";
                        Connten.Content = "CG__AOI Mapping ";
                        tablebase.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        gridchaneltable.Margin = new Thickness (463, 59, 550, 10);
                        Mapping_CGAOI.Margin = new Thickness(1103, 64, 0, 0);
                        Mapping_CGAOI.Width = 344;
                        Seachserialno.Content = "pid";
                        //Mapping_CGAOI.Margin = new Thickness(854, 64, 0, 0);


                        /*string[] AddItemLineName = { "303_A", "303_B", "304_A", "304_B", "305_A", "305_B", "306_A", "306_B", "401_A", "401_B", "402_A", "402_B", "403_A", "403_B", "404_A", "404_B", "405_A", "405_B", "501_A", "501_B", "502_A", "502_B", "503_A", "503_B", "504_A", "504_B" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }*/

                    }
                    break;
                case "CP_AOI":
                    {
                        tablebase.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        table2.Visibility = Visibility.Visible;
                        gridchaneltable.Visibility = Visibility.Visible;
                        gridchaneltable.Margin = new Thickness(440, 59, 500, 10);
                        Seachserialno.Content = "pid";

                        //
                        //
                        /*string[] AddItemLineName = { "301", "302", "303", "304", "305", "306", "404", "406", "501", "502", "503", "504", "505", "506" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }*/
                    }
                    break;
                case "IS_AOI":
                    {
                        tablebase.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        table2.Visibility = Visibility.Visible;
                       
                        tabledt.Visibility = Visibility.Visible;
                        gridchaneltable.Visibility = Visibility.Visible;
                        Seachserialno.Content = "short_serial_no";
                        gridchaneltable.Margin = new Thickness(440, 59, 500, 10);


                        //
 
                        //
                        /*string[] AddItemLineName = { "301", "302", "303", "304", "305", "306", "402", "404", "405", "406", "501", "502", "503", "504", "505", "506" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }*/
                    }
                    break;
                case "LT_AMI":
                    {
                        Connten.Content = "LT__AMI Stage ";
                        tablebase.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        table2.Visibility = Visibility.Visible;
                        tabledt.Visibility = Visibility.Visible;
                        gridchaneltable.Visibility = Visibility.Visible;
                        Seachserialno.Content = "idproduct";
                        gridchaneltable.Margin = new Thickness(440, 59, 500, 10);
/*

                        string[] AddItemLineName = { "406", "501", "502", "503", "504", "505", "506" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }*/

                    }
                    break;
                case "Assy_AMI":
                    {
                        Connten.Content = "Assy__AMI Stage ";
                        tablebase.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        /*Tabletotalpanel.Visibility = Visibility.Visible;
                        Tabletotalpanel.Margin = new Thickness(440, 59, 500, 10);*/
                        
                        Mapping_CGAOI.Margin = new Thickness(1103, 64, 0, 0);
                        Mapping_CGAOI.Width = 344;
                        gridchaneltable.Visibility = Visibility.Visible;
                        table2.Visibility = Visibility.Visible;
                        table2.Width = 410;
                        Seachserialno.Content = "short_serial_no";
                        tabledt.Visibility = Visibility.Visible;

                        /*string[] AddItemLineName = { "301" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }*/
                    }
                    break;
                case "CG_AOI_Plus":
                    {

                        Connten.Content = "CG__AOI__Plus Mapping ";
                        contentableidpanel.Content = "CG__AOI__Plus__Defect__ID__Mapping";
                        //tablebase.Visibility = Visibility.Visible;
                        tablebase.Margin = new Thickness(0, 0, 770, 53);
                        Lane_B_CG.Visibility = Visibility.Visible;
                        table2.Visibility = Visibility.Visible;
                        table2.Width = 350;
                        TableB.Visibility = Visibility.Visible;
                        TableB.Width = 350;
                        TableB.Margin = new Thickness(300, 20, 654, 10);

                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Width = 500;
                        Mapping_CGAOI.Margin = new Thickness(800, 64, 0, 0);
                        Name_laneA.Content = "Lane A";
                        Name_laneB.Visibility = Visibility.Visible;
                        Name_laneA.Visibility = Visibility.Visible;
                        //Buttoncopyclipboard.Visibility = Visibility.Collapsed;
                       // checkerdataintable.IsEnabled = false;
                        tablebase.Visibility = Visibility.Visible;
                        tablebase.Margin = new Thickness(0, 10, 743, 53);

                        /*string[] AddItemLineName = { "303", "304", "305", "306", "401", "402", "403", "404", "405", "501", "502", "503", "504" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }*/
                    }
                    break;
            }
        }
        private void LineSelection_DropDownClosed(object sender, EventArgs e)
        {
            IPMachine_CGPlus.Clear();
            
            Time_ST.IsEnabled = false;
            Time_EN.IsEnabled = false;
            Judgeslection.IsEnabled = false;
            defectselection.IsEnabled = false;
            buttonstart.IsEnabled = true;

            Time_ST.Opacity = 0.5;
            Time_EN.Opacity = 0.5;
            Judgeslection.Opacity = 0.5;
            defectselection.Opacity = 0.5;
            buttonstart.Opacity = 1;

            if (LineSelection.Text != string.Empty)
            {
                Time_ST.IsEnabled = true;
                Time_EN.IsEnabled = true;
                Judgeslection.IsEnabled = true;
                Time_ST.Opacity = 1;
                Time_EN.Opacity = 1;
                Judgeslection.Opacity= 1;
            }
            if(Judgeslection.Text != string.Empty || Judgeslection.Text != "N" || Judgeslection.Text != "NG")
            {
                //buttonstart.IsEnabled = true;
                //buttonstart.Opacity = 1;
            }
            if (MachineSelection.Text == "CG_AOI_Plus")
            {
                switch (LineSelection.Text)
                {
                    case "303":
                        {
                            IPMachine_CGPlus.Add("303_A");
                            IPMachine_CGPlus.Add("303_B");

                        }
                        return;
                    case "304":
                        {
                            IPMachine_CGPlus.Add("304_A");
                            IPMachine_CGPlus.Add("304_B");
                        }
                        return;
                    case "305":
                        {
                            IPMachine_CGPlus.Add("305_A");
                            IPMachine_CGPlus.Add("305_B");
                        }
                        return;
                    case "306":
                        {

                            IPMachine_CGPlus.Add("306_A");
                            IPMachine_CGPlus.Add("306_B");
                        }
                        return;
                    case "401":
                        {
                            IPMachine_CGPlus.Add("401_A");
                            IPMachine_CGPlus.Add("401_B");
                        }
                        return;
                    case "402":
                        {
                            IPMachine_CGPlus.Add("402_A");
                            IPMachine_CGPlus.Add("402_B");
                        }
                        return;
                    case "403":
                        {
                            IPMachine_CGPlus.Add("403_A");
                            IPMachine_CGPlus.Add("403_B");
                        }
                        return;
                    case "404":
                        {
                            IPMachine_CGPlus.Add("404_A");
                            IPMachine_CGPlus.Add("404_B");
                        }
                        return;
                    case "405":
                        {
                            IPMachine_CGPlus.Add("405_A");
                            IPMachine_CGPlus.Add("405_B");
                        }
                        return;
                    case "501":
                        {
                            IPMachine_CGPlus.Add("501_A");
                            IPMachine_CGPlus.Add("501_B");

                        }
                        return;
                    case "502":
                        {
                            IPMachine_CGPlus.Add("502_A");
                            IPMachine_CGPlus.Add("502_B");
                        }
                        return;
                    case "503":
                        {
                            IPMachine_CGPlus.Add("503_A");
                            IPMachine_CGPlus.Add("503_B");
                        }
                        return;
                    case "504":
                        {
                            IPMachine_CGPlus.Add("504_A");
                            IPMachine_CGPlus.Add("504_B");
                        }
                        return;
                }
            }
        }
        private void defectselection_DropDownClosed(object sender, EventArgs e)
        {
            if (defectselection.Text != string.Empty)
            {
                buttonstart.IsEnabled = true;
                buttonstart.Opacity = 1;
            }
        }
        
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            Seachserialno.IsChecked = false;
            seachpanel = true;
        }
        private void CheckBox_Checked_1(object sender, RoutedEventArgs e)
        {
            Seachpanelid.IsChecked = false;
            seachpanel = false;

        }
        private void checkerdataintable_Checked(object sender, RoutedEventArgs e)
        {
        }
        private void CleardatatableGrid()
        {
            tablebase.ItemsSource = null;
            tablebase.Items.Clear();
            table2.ItemsSource = null;
            table2.Items.Clear();
            tabledt.ItemsSource = null;
            tabledt.Items.Clear();
            TableB.ItemsSource = null;
            TableB.Items.Clear();
            tablechanel.ItemsSource = null;
            tablechanel.Items.Clear();
            Lane_B_CG.ItemsSource = null;
            Lane_B_CG.Items.Clear();


        }
        private void ClearData()
        {
            // Xóa dữ liệu trước khi chạy
            int zero = 0;
            TotalOKA.Content = zero.ToString();
            TotalNGA.Content = zero.ToString();
            TotalNAA.Content = zero.ToString();
            TotalOKB.Content = zero.ToString();
            TotalNGB.Content = zero.ToString();
            TotalNAB.Content = zero.ToString();
            TotalAAA.Content = zero.ToString();
            TotalBBB.Content = zero.ToString();
            TotalNGAB.Content = zero.ToString();
            TotalOKAB.Content = zero.ToString();
            TotalNAAB.Content = zero.ToString();
            TotalALLAB.Content = zero.ToString();
            YRTALLOKAB.Content = zero.ToString();
            YRTALLNGAB.Content = zero.ToString();
            YRTALLNAAB.Content = zero.ToString();
            YRTOKA.Content = zero.ToString();
            YRTNGA.Content = zero.ToString();
            YRTNAA.Content = zero.ToString();
            YRTOKB.Content = zero.ToString();
            YRTNGB.Content = zero.ToString();
            YRTNAB.Content = zero.ToString();

        }
        #endregion

        #endregion

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            


        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            
        }
    }
}
