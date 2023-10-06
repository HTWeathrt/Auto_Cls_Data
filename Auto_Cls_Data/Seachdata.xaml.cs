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

namespace Auto_Cls_Data
{
    public partial class Seachdata : Window
    {
        NewLoading newloading;
        SQLLoading sqload;
        IP_Class iP_Class;
        CGAOIPlus cgaoiplus;
        public Seachdata()
        {
            InitializeComponent();
            lddatime();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }
        private void lddatime()
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
        private void TableAssyAMI(string TimerST,string TimerEN,string Machine,string Line)
        {
            newloading = new NewLoading();
            string SQLLD = "";
            if (Machine == "Assy_AMI")
            {
                SQLLD = newloading.LoadAssyAMI(TimerST, TimerEN);
            }
            if(Machine =="CP_AOI")
            {
                SQLLD = newloading.LoadCPAOI(TimerST, TimerEN);
            }
            if(Machine == "LT_AMI")
            {
                SQLLD = newloading.LoadLTAMI(TimerST, TimerEN);
            }
            //
            MySqlCommand SQLCommandloading = new MySqlCommand(SQLLD, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(SQLCommandloading);
            DataTable sqlbaseTable = new DataTable();
            adapter.Fill(sqlbaseTable);
            sqlbaseTable.Columns.Add("STT");
            sqlbaseTable.Columns["STT"].SetOrdinal(0);
            int ixb = 1;
            foreach (DataRow rowxa in sqlbaseTable.Rows)
            {
                rowxa["STT"] = ixb++;
            }
            tablepanel.ItemsSource = sqlbaseTable.DefaultView;
            sqlbaseTable.Dispose();
        }
        public void Connection(string TimerST,string TimerEN,string Machine,string Line,string Judge,string Defection,int DataLimit)
        {
            try
            {   
                string Connec = sqload.DBShow(Machine, Line);
                connection = new MySqlConnection(Connec); 
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    string sqlselection = newloading.TableDatabaseShow(TimerST, TimerEN, DataLimit, Judge, Defection);
                    cmd = new MySqlCommand(sqlselection, connection);
                    //(string Machine , string Limited, string TimerST, string TimerEN)
                    if (Machine =="Assy_AMI" || Machine == "CP_AOI" || Machine =="LT_AMI")
                    {
                        TableAssyAMI(TimerST, TimerEN, Machine,Line);
                    }
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable sqlbaseTable = new DataTable();
                    adapter.Fill(sqlbaseTable);
                    sqlbaseTable.Columns.Add("STT");
                    sqlbaseTable.Columns["STT"].SetOrdinal(0);
                    int ixb = 1;   
                    tabletileld(Machine, DataLimit, TimerST, TimerEN);
                    foreach (DataRow rowxa in sqlbaseTable.Rows)
                    {
                        rowxa["STT"] = ixb++;
                    }
                    Caculator(sqlbaseTable);
                    if (DatashowTable == true)
                    {
                        tablebase.ItemsSource = sqlbaseTable.DefaultView;
                    }
                    sqlbaseTable.Dispose();
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
        private void tabletileld(string Machine ,  int Limited, string TimerST, string TimerEN)
        {
            newloading = new NewLoading();
            //Table dữ liệu tính toán cho CG_AOI
            string sql = newloading.YRTtable(Machine,TimerST,TimerEN,Limited);
            MySqlCommand act = new MySqlCommand(sql, connection);
            MySqlDataAdapter adapter2 = new MySqlDataAdapter(act);
            DataTable sqltbrateTable = new DataTable();
            adapter2.Fill(sqltbrateTable);
            sqltbrateTable.Columns.Add("STT");
            sqltbrateTable.Columns["STT"].SetOrdinal(0);
            int ixb = 1;
            foreach (DataRow rowxa in sqltbrateTable.Rows)
            {
                rowxa["STT"] = ixb++;
            }
            table2.ItemsSource = sqltbrateTable.DefaultView;
            sqltbrateTable.Dispose();
        }
        public void StartSeach(object sender, RoutedEventArgs e)
        {
            sqload = new SQLLoading();
            newloading = new NewLoading();
            iP_Class = new IP_Class();
            cgaoiplus = new CGAOIPlus();

            string Machine = MachineSelection.Text;
            string Line = LineSelection.Text;
            int DataLimit = Convert.ToInt32(data_limit_seach.Text);
            string TimerST = Time_ST.Text;
            string TimerEN = Time_EN.Text;
            string Judge = Judgeslection.Text;
            string Defection = defectselection.Text;
            if (Machine == string.Empty || Line == string.Empty)
            {
                MessageBox.Show("No Data LD");
                return;
            }
            CleardatatableGrid();
            Showwindown();
            if (Machine != "CG_AOI_Plus")
            {// public void Connection(string TimerST,string TimerEN,string Machine,string Line,string Judge,string Defection,int DataLimit)
                Connection(TimerST, TimerEN, Machine,Line,Judge,Defection,DataLimit);
            }
            else
            {
                foreach (string Item in IPMachine_CGPlus)
                {
                   
                    string LaneX = Item.Substring(4, 1);
                    string LineCGPlus = $"{Line}_{LaneX}";
                    if (LaneX == "A")
                    {
                        DataTable DatataleA = cgaoiplus.Plus_cgaoi(Machine, LineCGPlus, TimerST, TimerEN);
                        table2.ItemsSource = DatataleA.DefaultView;
                        DataTable DataORGA = cgaoiplus.Plus_CGA(Machine, LineCGPlus, DataLimit, TimerST,TimerEN,Judge,Defection);
                        tablebase.ItemsSource = DataORGA.DefaultView;
                        CaculatorCGPlusA(DataORGA);
                    }
                    else if (LaneX == "B")
                    {
                        DataTable DatataleB = cgaoiplus.Plus_cgaoiB(Machine, LineCGPlus, TimerST, TimerEN);
                        TableB.ItemsSource = DatataleB.DefaultView;
                        DataTable DataORGB = cgaoiplus.Plus_CGB(Machine, LineCGPlus, DataLimit, TimerST, TimerEN, Judge, Defection);
                        Lane_B_CG.ItemsSource = DataORGB.DefaultView;
                        CaculatorCGPlusB(DataORGB);
                        Catital();
                    }
                }
            }
            adb.Close();
        }
        private void SeachID(object sender, RoutedEventArgs e)
        {
            sqload = new SQLLoading();
            SeachDataCls seachDataCls = new SeachDataCls();
            DataTable dt = new DataTable();
            int length = BoxID.Text.Length;
            string Machine = MachineSelection.Text;
            string Line = LineSelection.Text;
            string IDList = BoxID.Text;
            string connector = sqload.DBShow(Machine, Line);
            MySqlConnection connection = new MySqlConnection(connector);
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                CleardatatableGrid();
                string loadingdata = seachDataCls.LoadingData(Machine, length, Line, seachpanel, IDList);
                MySqlCommand cmd = new MySqlCommand(loadingdata, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                adapter.Fill(dt);
                dt.Columns.Add("STT");
                dt.Columns["STT"].SetOrdinal(0);
                int ixb = 1;
                foreach (DataRow rowxa in dt.Rows)
                {
                    rowxa["STT"] = ixb++;
                }
                datatable = dt;
                tablebase.ItemsSource = dt.DefaultView;
            }
           

        }
        DataTable datatable;
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
        private void Showwindown()
        {
            adb = new Window1();
            adb.Left = 850;
            adb.Top = 300;
            adb.Show();
        }
        private void copydatatoclipboard(object sender, RoutedEventArgs e)
         {
            if (datatable == null)
            {
                return;
            }
            DataObject dataObject = new DataObject();
            dataObject.SetData(DataFormats.UnicodeText, GetClipboardText(datatable));
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
        private Window1 adb;
        public DataTable sqlbaseTable;
        public MySqlConnection connection;
        private MySqlCommand cmd;
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
            if (MachineSelection.Text == "IS_AOI")
            {
                LineSelection.Items.Clear();
                defectselection.Items.Clear();
                Judgeslection.Items.Add("ALL");
                Judgeslection.Items.Add("OK");
                Judgeslection.Items.Add("NG");

                string file_ISAOI_X = "config/ISAOI_Defect.txt";
                using (StreamReader readerIS = new StreamReader(file_ISAOI_X))
                {
                    string lineIS;
                    while ((lineIS = readerIS.ReadLine()) != null)
                    {
                        defectselection.Items.Add(lineIS);
                    }
                }
            }
            else if (MachineSelection.Text == "CG_AOI")
            {
                LineSelection.Items.Clear();
                defectselection.Items.Clear();
                Judgeslection.Items.Add("ALL");
                Judgeslection.Items.Add("OK");
                Judgeslection.Items.Add("NG");
                
                string file_CGAOI = "config/CGAOI_Defect.txt";
                using (StreamReader readerCG = new StreamReader(file_CGAOI))
                {
                    string lineCG;
                    while ((lineCG = readerCG.ReadLine()) != null)
                    {
                        defectselection.Items.Add(lineCG);
                    }
                }
                // Defect CGAOI
            }
            else if (MachineSelection.Text == "CP_AOI")
            {
                LineSelection.Items.Clear();
                defectselection.Items.Clear();
                Judgeslection.Items.Add("ALL");
                Judgeslection.Items.Add("N");
                Judgeslection.Items.Add("G");
                
                string file_CPAOI = "config/CPAOI_Defect.txt";
                using (StreamReader readerCP = new StreamReader(file_CPAOI))
                {
                    string lineCP;
                    while ((lineCP = readerCP.ReadLine()) != null)
                    {
                        defectselection.Items.Add(lineCP);
                    }
                }
            }
            else if (MachineSelection.Text == "LT_AMI")
            {
                LineSelection.Items.Clear();
                defectselection.Items.Clear();
                Judgeslection.Items.Clear();
                Judgeslection.Items.Add("ALL");
                Judgeslection.Items.Add("OK");
                Judgeslection.Items.Add("NG");
                
                string file_LT_AMI = "config/LTAMI_Defect.txt";
                using (StreamReader reader = new StreamReader(file_LT_AMI))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        defectselection.Items.Add(line);
                    }
                }
            }
            else if (MachineSelection.Text == "Assy_AMI")
            {
                LineSelection.Items.Clear();
                defectselection.Items.Clear();
                Judgeslection.Items.Clear();
                Judgeslection.Items.Add("ALL");
                Judgeslection.Items.Add("G");
                Judgeslection.Items.Add("N");

                string file_LT_AMI = "config/AssyAMI_Defect.txt";
                using (StreamReader reader = new StreamReader(file_LT_AMI))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        defectselection.Items.Add(line);
                    }
                }
            }
            else if (MachineSelection.Text == "CG_AOI_Plus")
            {
                LineSelection.Items.Clear();
                defectselection.Items.Clear();
                Judgeslection.Items.Add("ALL");
                Judgeslection.Items.Add("OK");
                Judgeslection.Items.Add("NG");
                
                string file_CPAOI = "config/CGAOI_Defect.txt";
                using (StreamReader readerCP = new StreamReader(file_CPAOI))
                {
                    string lineCP;
                    while ((lineCP = readerCP.ReadLine()) != null)
                    {
                        defectselection.Items.Add(lineCP);
                    }
                }
            }
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
            Name_laneB.Visibility = Visibility.Collapsed;
            Name_laneA.Visibility = Visibility.Collapsed;
            Buttoncopyclipboard.Visibility = Visibility.Visible;
            Tabletotalpanel.Visibility = Visibility.Collapsed;

            //tableIDpanel.Margin = new Thickness(463, 59, 684, 10);
            contentableidpanel.Content = "Table__Defect__ID__Mapping";
            checkerdataintable.IsEnabled = true;
            // 0,10,10,53
            tablebase.Margin = new Thickness(0, 10, 10, 53);
            Lane_B_CG.Visibility = Visibility.Collapsed;
            //Table Loading
            CleardatatableGrid();
            switch (MachineSelection.Text)
            {
                case "CG_AOI":
                    {
                        table2.Visibility = Visibility.Visible;
                        LaneALTAMI.Content = "Stage 1";
                        LaneBLTAMI.Content = "Stage 2";
                        Connten.Content = "CG__AOI Mapping ";
                        tablebase.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        Tabletotalpanel.Margin = new Thickness (463, 59, 550, 10);
                        Mapping_CGAOI.Margin = new Thickness(1000, 64, 0, 0);
                        Mapping_CGAOI.Width = 400;
                        //Mapping_CGAOI.Margin = new Thickness(854, 64, 0, 0);
                        string[] AddItemLineName = { "303_A", "303_B", "304_A", "304_B", "305_A", "305_B", "306_A", "306_B", "401_A", "401_B", "402_A", "402_B", "403_A", "403_B", "404_A", "404_B", "405_A", "405_B", "501_A", "501_B", "502_A", "502_B", "503_A", "503_B", "504_A", "504_B" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }

                    }
                    break;
                case "CP_AOI":
                    {
                        tablebase.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        table2.Visibility = Visibility.Visible;
                        Tabletotalpanel.Visibility = Visibility.Visible;
                        Tabletotalpanel.Margin = new Thickness(440, 59, 500, 10);
                        
                        string[] AddItemLineName = { "301", "302", "303", "304", "305", "306", "404", "406", "501", "502", "503", "504", "505", "506" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }
                    }
                    break;
                case "IS_AOI":
                    {
                        tablebase.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        table2.Visibility = Visibility.Visible;
                       
                        tabledt.Visibility = Visibility.Visible;
                        Tabletotalpanel.Visibility = Visibility.Visible;
                        Tabletotalpanel.Margin = new Thickness(440, 59, 500, 10);
                        string[] AddItemLineName = { "301", "302", "303", "304", "305", "306", "402", "404", "405", "406", "501", "502", "503", "504", "505", "506" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }
                    }
                    break;
                case "LT_AMI":
                    {
                        Connten.Content = "LT__AMI Stage ";
                        tablebase.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        table2.Visibility = Visibility.Visible;
                        tabledt.Visibility = Visibility.Visible;
                        Tabletotalpanel.Visibility = Visibility.Visible;
                        Tabletotalpanel.Margin = new Thickness(440, 59, 500, 10);
                        string[] AddItemLineName = { "406", "501", "502", "503", "504", "505", "506" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }

                    }
                    break;
                case "Assy_AMI":
                    {
                        Connten.Content = "Assy__AMI Stage ";
                        tablebase.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        Tabletotalpanel.Visibility = Visibility.Visible;
                        Tabletotalpanel.Margin = new Thickness(440, 59, 500, 10);
                        Mapping_CGAOI.Margin = new Thickness(1000, 64, 0, 0);
                        Mapping_CGAOI.Width = 465;
                        table2.Visibility = Visibility.Visible;
                        table2.Width = 410;
                        tabledt.Visibility = Visibility.Visible;
                        string[] AddItemLineName = { "301" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }
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
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        TableB.Visibility = Visibility.Visible;
                        TableB.Width = 350;
                        TableB.Margin = new Thickness(300, 20, 654, 10);
                        Mapping_CGAOI.Width = 500;
                        Mapping_CGAOI.Margin = new Thickness(800, 64, 0, 0);
                        Tabletotalpanel.Visibility = Visibility.Collapsed;
                        Buttoncopyclipboard.Visibility = Visibility.Collapsed;
                        Name_laneB.Visibility = Visibility.Visible;
                        Name_laneA.Visibility = Visibility.Visible;
                        Buttoncopyclipboard.Visibility = Visibility.Collapsed;
                        checkerdataintable.IsEnabled = false;
                        tablebase.Visibility = Visibility.Visible;
                        tablebase.Margin = new Thickness(0, 10, 743, 53);
                        string[] AddItemLineName = { "303", "304", "305", "306", "401", "402", "403", "404", "405", "501", "502", "503", "504" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }
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
        bool DatashowTable = false;
        private void checkerdataintable_Checked(object sender, RoutedEventArgs e)
        {
           
            if(checkerdataintable.IsChecked == true)
            {
                DatashowTable = true;
            }
            else
            {
                DatashowTable = false;
            }
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
            tablepanel.ItemsSource = null;
            tablepanel.Items.Clear();

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


    }
}
