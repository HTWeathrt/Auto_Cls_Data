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

namespace Auto_Cls_Data
{
    public partial class Seachdata : Window
    {
        public Seachdata()
        {
            InitializeComponent();
            lddatime();
        }
        private void lddatime()
        {
            // Tải thời gian đầu tiên
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
        public void Connection()
        {
            try
            {
                IP_Class ipcls = new IP_Class();
                ipcls.IP_Selx(MachineSelection.Text, LineSelection.Text);
                ip_selc = ipcls.Ip_in;
                dataXL = ipcls.Data_Basexx;
                connection = new MySqlConnection("Server=" + ip_selc + "; Database=" + dataXL + "; Port=3306; User = ami; Password = protnc"); //charSet = utf8"


                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    TableALL();
                    tabletileld();

                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable sqlbaseTable = new DataTable();
                    adapter.Fill(sqlbaseTable);
                    Caculator(sqlbaseTable);

                    if (checkerdataintable.IsChecked == true)
                    {
                        sqlbaseTable.Columns.Add("STT");
                        sqlbaseTable.Columns["STT"].SetOrdinal(0);
                        int ixb = 1;
                        foreach (DataRow rowxa in sqlbaseTable.Rows)
                        {
                            rowxa["STT"] = ixb++;
                        }
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
        private void tabletileld()
        {
            //Table dữ liệu tính toán cho CG_AOI
            if (MachineSelection.Text == "CG_AOI" || MachineSelection.Text == "IS_AOI")
            {
                //LoadingData Window1 = new LoadingData();
                string sql = "SELECT judge, priority_defect_name, COUNT(*) AS count_defects, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "'), 2) AS percent_defects FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' GROUP BY judge, priority_defect_name ORDER BY priority_defect_name LIMIT " + data_limit_seach.Text + "";
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
            else if (MachineSelection.Text == "CP_AOI" || MachineSelection.Text == "LT_AMI")
            {
                string sql = "SELECT judge, priority_defect_name, COUNT(*) AS count_defects, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "'), 2) AS percent_defects FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' GROUP BY judge, priority_defect_name ORDER BY priority_defect_name LIMIT " + data_limit_seach.Text + "";
                //LoadingData Window1 = new LoadingData();
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
        }
        public void TableALL()
        {

            if (Judgeslection.Text == "ALL")
            {
                string query = "SELECT * FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' LIMIT " + data_limit_seach.Text + "";
                cmd = new MySqlCommand(query, connection);
            }
            if (Judgeslection.Text != "ALL")
            {
                if (Judgeslection.Text == "OK" || Judgeslection.Text == "G")
                {
                    string query = "SELECT * FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' AND judge = '" + Judgeslection.Text + "' LIMIT " + data_limit_seach.Text + "";
                    cmd = new MySqlCommand(query, connection);
                }
                if (Judgeslection.Text == "NG" || Judgeslection.Text == "N")
                {
                    string query = "SELECT * FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' AND priority_defect_name = '" + defectselection.Text + "' LIMIT " + data_limit_seach.Text + "";
                    cmd = new MySqlCommand(query, connection);
                }
            }
            //setting.LimitSeach = dataasetting.LimitSeach;



        }
        public void StartSeach(object sender, RoutedEventArgs e)
        {

            CleardatatableGrid();
            Showwindown();
            if (MachineSelection.Text != "CG_AOI_Plus")
            {
                Connection();
                 adb.Close();
            }
            else
            {
                Machinex = MachineSelection.Text;
                IP_Class iP_Class = new IP_Class();
                foreach (string Item in IPMachine_CGPlus)
                {
                    //ConnectionCGPlus();
                    iP_Class.IP_Selx(Machinex, Item);
                    string Lane = Item.Substring(4, 1);
                    ip_selc = iP_Class.Ip_in;
                    dataXL = iP_Class.Data_Basexx;
                    ConnectionCGPlus(Lane);
                }
                adb.Close();
            }
        }
        private void CGplustableA(string Lane)
        {
            string sql = "SELECT judge, priority_defect_name, COUNT(*) AS count_defects, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "'), 2) AS percent_defects FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' GROUP BY judge, priority_defect_name ORDER BY priority_defect_name LIMIT " + data_limit_seach.Text + "";
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
            if (Lane == "A")
            {
                table2.ItemsSource = sqltbrateTable.DefaultView;
            }
            else if (Lane == "B")
            {
                TableB.ItemsSource = sqltbrateTable.DefaultView;
            }
            sqltbrateTable.Dispose();
        }

        private void ConnectionCGPlus(string Lane)
        {
            try
            {
                connection = new MySqlConnection("Server=" + ip_selc + "; Database=" + dataXL + "; Port=3306; User = ami; Password = protnc"); //charSet = utf8"
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    TableALL();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable sqlbaseTable = new DataTable();
                    adapter.Fill(sqlbaseTable);
                    sqlbaseTable.Columns.Add("STT");
                    sqlbaseTable.Columns["STT"].SetOrdinal(0);
                    int ixb = 1;
                    foreach (DataRow rowxa in sqlbaseTable.Rows)
                    {
                        rowxa["STT"] = ixb++;
                    }
                    if (Lane == "A")
                    {
                        CaculatorCGPlusA(sqlbaseTable);
                        Lane_A_CG.ItemsSource = sqlbaseTable.DefaultView;
                    }
                    else if (Lane == "B")
                    {
                        CaculatorCGPlusB(sqlbaseTable);
                        Lane_B_CG.ItemsSource = sqlbaseTable.DefaultView;
                    }
                    CGplustableA(Lane);
                    Catital();
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
        private void SeachID(object sender, RoutedEventArgs e)
        {
            
            int length = BoxID.Text.Length;
            if (BoxID.Text != string.Empty || length >=10)
            {
                CleardatatableGrid();
                Showwindown();
                if (MachineSelection.Text != "CG_AOI_Plus")
                {
                    try
                    {
                        string IDSeachcheck = BoxID.Text;
                        ConnectionSQL();
                        connection.Open();
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            if (IDSeachcheck.IndexOf(' ') > 1 || IDSeachcheck.IndexOf('\n') < 2)
                            {
                                // Seach panel ID thiếu hụt
                                string IDSeach1 = "'" + BoxID.Text + "%'";
                                string query = "SELECT * FROM product WHERE panelid LIKE " + IDSeach1;
                                MySqlCommand cmd = new MySqlCommand(query, connection);
                                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                                DataTable dt = new DataTable();
                                adapter.Fill(dt);
                                tablebase.ItemsSource = dt.DefaultView;
                                adb.Close();
                                connection.Close();
                            }
                            else
                            {
                                /// Seach Nhiều panel ID
                                string output = BoxID.Text;
                                if (output.Contains("\r") || output.Contains("\n"))
                                {
                                    output = output.Replace("\r", "").Replace("\n", "','");
                                }
                                if (output.EndsWith("','"))
                                {
                                    output = output.Substring(0, output.Length - 3);
                                }
                                string query = "SELECT * FROM product WHERE panelid IN ('" + output + "')";
                                MySqlCommand cmd = new MySqlCommand(query, connection);
                                MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                                DataTable dt = new DataTable();
                                adapter.Fill(dt);
                                tablebase.ItemsSource = dt.DefaultView;
                                adb.Close();
                                connection.Close();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        //  Bẫy lỗi và đưa thông báo lỗi vào messenger
                        MessageBox.Show("Error: " + ex.ToString());
                        adb.Close();
                        connection.Close();
                        return;
                    }

                }

            }
            else
            {
                MessageBox.Show("Input Data >13 length");
                return;
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
                        if (row["pid"].ToString() != null)
                        {
                            string valueA = rowpid[0].ToString();
                            if (valueA == "1" || valueA == "2" || valueA == "3" || valueA == "4")
                            {
                                totalNG_A++;
                            }
                            if (valueA == "5" || valueA == "6" || valueA == "7" || valueA == "8")
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
                            if (valueA == "1" || valueA == "2" || valueA == "3" || valueA == "4")
                            {
                                totalOK_A++;
                            }
                            if (valueA == "5" || valueA == "6" || valueA == "7" || valueA == "8")
                            {
                                totalOK_B++;
                            }
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
        private void ShowSubForm()
        {
            thread5 = new Thread(ShowSubForm);
            thread5.SetApartmentState(ApartmentState.STA);
            thread5.IsBackground = true;
            thread5.Start();

            adb = new Window1();
            adb.Left = 850;
            adb.Top = 300;
            adb.Show();


            //Dispatcher.Run();
            //Nói rằng đây là một Window độc lập/*
            System.Windows.Threading.Dispatcher.Run();
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
            
            
        }
        
        private void ConnectionSQL()
        {
            IP_Class ipcls = new IP_Class();
            ipcls.IP_Selx(MachineSelection.Text,LineSelection.Text);
            ip_selc = ipcls.Ip_in;
            dataXL = ipcls.Data_Basexx;
            connection = new MySqlConnection("Server=" + ip_selc + "; Database=" + dataXL + "; Port=3306; User = ami; Password = protnc"); //charSet = utf8"
        }
        private void copydatatoclipboard(object sender, RoutedEventArgs e)
        {
            Showwindown();
            try
            {
                ConnectionSQL();
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    TableALL();
                    MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                    DataTable sqlbaseTable = new DataTable();
                    adapter.Fill(sqlbaseTable);
                    DataObject dataObject = new DataObject();
                    dataObject.SetData(DataFormats.UnicodeText, GetClipboardText(sqlbaseTable));
                    Clipboard.SetDataObject(dataObject, true);



                }
                adb.Close();


            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                MessageBox.Show("Error: " + ex.Message);
                connection.Close();
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
        private void Testconnectbt(object sender, RoutedEventArgs e)
        {
            try
            {
                Connection();
                connection.Open();
                string message = "Đã kết nối tới Server: " + ip_in + " Database: " + dataXL;
                // Hiển thị thông báo trong MessageBox
                MessageBox.Show(message);

            }
            catch (Exception ex)
            {
                string message = "NOTCONNECT Server: " + ip_in + " Database: " + dataXL + ex.Message;
                // Hiển thị thông báo trong MessageBox
                MessageBox.Show(message);
            }
            finally
            {
                connection.Close();
            }
        }
        private void tabledatainpt(object sender, SelectedCellsChangedEventArgs e)
        {

        }
        private void tablebase_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            // if (Line_Sel.Text == "IS_AOI" || Line_Sel.Text == "CP_AOI")
            // {
           

            /*if (dataselect == true)
            {
                DateTime pt_datetime = new DateTime(dataptdatetime);
               // string pt_datetime = dataptdatetime;
                MessageBox.Show(pt_datetime);
                *//*try
                {
                    Connection();
                    string query = "SELECT * FROM defect WHERE pt_datetime = @pt_datetime";
                    DataTable datastring = new DataTable();
                    using (MySqlCommand command = new MySqlCommand(query, connection))
                    {
                        //command.Parameters.AddWithValue("@iddefect", iddefect);
                        command.Parameters.AddWithValue("@pt_datetime", pt_datetime);

                        using (MySqlDataAdapter adapter = new MySqlDataAdapter(command))
                        {
                            adapter.Fill(datastring);
                            byte[] raw = (byte[])datastring.Rows[0]["feature"];
                            string text = Encoding.UTF8.GetString(raw);
                            MessageBox.Show(text);

                            tableyrt.ItemsSource = datastring.DefaultView;

                            connection.Close();
                            dataselect = false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                    Window1.Close();
                    connection.Close();
                }*//*
            }*/

        }
        //private bool dataselect;
        private void tablebase_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            if (tablebase.SelectedCells != null && tablebase.SelectedCells.Count > 0)
            {
                // dataselect = true;
                DataGridCellInfo cellInfo = tablebase.SelectedCells[1];
                object data = cellInfo.Item;
                DataRowView rowXY = data as DataRowView;
                dataptdatetime = rowXY["pt_datetime"].ToString();
            }

        }
        //int sleep_time;
       /* private void Loadingsetting()
        {
            string Filesetting = "Setting/setting_Main.txt";
            using (StreamReader sw = new StreamReader(Filesetting))
            {
                string Readtosetting;
                Readtosetting = sw.ReadLine();
                limitdata = (Readtosetting);

            }
        }*/
        private string ip_selc;
        private string Machinex;
        private string Line;
        private string dataptdatetime;
        private Window1 adb;
        public DataTable sqlbaseTable;
        public MySqlConnection connection;
        private MySqlCommand cmd;
        string ip_in;
        string dataXL;
        private Thread thread5;
        private List<string> IPMachine_CGPlus = new List<string>();
        private void TableB_SelectedCellsChanged_1(object sender, SelectedCellsChangedEventArgs e)
        {
            string A = "B";
            string Line = LineSelection.Text;
            string LineSelec = $"{Line}_{A}";
            
            if (TableB.SelectedCells != null && TableB.SelectedCells.Count > 0)
            {
                // Lấy giá trị ô được chọn
                DataGridCellInfo cellInfo = TableB.SelectedCells[1];
                object data = cellInfo.Item;
                DataRowView rowXY = data as DataRowView; // Ép kiểu sang DataRowView nếu cần thiết
                string name_seach = rowXY["priority_defect_name"].ToString();
                Machinex = MachineSelection.Text;
                Line = LineSelection.Text;
                IP_Class iP_Class = new IP_Class();
                iP_Class.IP_Selx(Machinex, LineSelec);
                ip_selc = iP_Class.Ip_in;
                dataXL = iP_Class.Data_Basexx;
                try
                {
                    connection = new MySqlConnection("Server=" + ip_selc + "; Database=" + dataXL + "; Port=3306; User = ami; Password = protnc"); //charSet = utf8"
                    connection.Open();
                    if (connection.State == System.Data.ConnectionState.Open)
                    {
                        string query = "SELECT priority_defect_name, priority_grid_pos, COUNT(*) AS CountPos FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' AND priority_defect_name = '" + name_seach + "'GROUP BY priority_defect_name,priority_grid_pos ";
                        string IDpanel = "SELECT priority_defect_name,priority_grid_pos,panelid FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' AND priority_defect_name = '" + name_seach + "' ";
                        MySqlCommand command = new MySqlCommand(query, connection);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                        DataTable loadingmapping = new DataTable();
                        DataTable loadingidpanel = new DataTable();
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
                        MySqlCommand command2 = new MySqlCommand(IDpanel, connection);
                        MySqlDataAdapter adapter2 = new MySqlDataAdapter(command2);
                        adapter2.Fill(loadingidpanel);
                        loadingidpanel.Columns.Add("STT");
                        loadingidpanel.Columns["STT"].SetOrdinal(0);
                        int ixa = 1;
                        foreach (DataRow row in loadingidpanel.Rows)
                        {
                            row["STT"] = ixa++;
                        }
                        tableidpanel.ItemsSource = loadingidpanel.DefaultView;
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
            if (MachineSelection.Text == "CG_AOI_Plus")
            {
                string A = "A";
                string Line = LineSelection.Text;
                string LineSelec = $"{Line}_{A}";
                if (table2.SelectedCells != null && table2.SelectedCells.Count > 0)
                {
                    // Lấy giá trị ô được chọn
                    DataGridCellInfo cellInfo = table2.SelectedCells[1];
                    object data = cellInfo.Item;
                    DataRowView rowXY = data as DataRowView; // Ép kiểu sang DataRowView nếu cần thiết
                    string name_seach = rowXY["priority_defect_name"].ToString();
                    Machinex = MachineSelection.Text;
                    //Line = LineSelection.Text;
                    IP_Class iP_Class = new IP_Class();
                    iP_Class.IP_Selx(Machinex, LineSelec);
                    ip_selc = iP_Class.Ip_in;
                    dataXL = iP_Class.Data_Basexx;
                    try
                    {
                        connection = new MySqlConnection("Server=" + ip_selc + "; Database=" + dataXL + "; Port=3306; User = ami; Password = protnc"); //charSet = utf8"

                        connection.Open();
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            string query = "SELECT priority_defect_name, priority_grid_pos, COUNT(*) AS CountPos FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' AND priority_defect_name = '" + name_seach + "'GROUP BY priority_defect_name,priority_grid_pos ";
                            string IDpanel = "SELECT priority_defect_name,priority_grid_pos,pt_datetime,panelid FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' AND priority_defect_name = '" + name_seach + "' ";
                            MySqlCommand command = new MySqlCommand(query, connection);
                            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                            DataTable loadingmapping = new DataTable();
                            DataTable loadingidpanel = new DataTable();
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
                            MySqlCommand command2 = new MySqlCommand(IDpanel, connection);
                            MySqlDataAdapter adapter2 = new MySqlDataAdapter(command2);
                            adapter2.Fill(loadingidpanel);
                            loadingidpanel.Columns.Add("STT");
                            loadingidpanel.Columns["STT"].SetOrdinal(0);
                            int ixa = 1;
                            foreach (DataRow row in loadingidpanel.Rows)
                            {
                                row["STT"] = ixa++;
                            }
                            tableidpanel.ItemsSource = loadingidpanel.DefaultView;
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
            else if (MachineSelection.Text == "LT_AMI")
            {
                if (table2.SelectedCells != null && table2.SelectedCells.Count > 0)
                {
                    // Lấy giá trị ô được chọn
                    DataGridCellInfo cellInfo = table2.SelectedCells[1];
                    object data = cellInfo.Item;
                    DataRowView rowXY = data as DataRowView; // Ép kiểu sang DataRowView nếu cần thiết
                    string name_seach = rowXY["priority_defect_name"].ToString();

                    try
                    {
                        ConnectionSQL();
                        connection.Open();
                        if (connection.State == System.Data.ConnectionState.Open)
                        {
                            string query = "1";

                            if (MachineSelection.Text == "LT_AMI")
                            {
                                query = "SELECT priority_defect_name, display_insp, COUNT(*) AS CountPos FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' AND priority_defect_name = '" + name_seach + "'GROUP BY priority_defect_name,display_insp ";

                            }
                            string IDpanel = "SELECT priority_defect_name,display_insp,pt_datetime,panelid FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' AND priority_defect_name = '" + name_seach + "' ";
                            MySqlCommand command = new MySqlCommand(query, connection);
                            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                            DataTable loadingmapping = new DataTable();
                            DataTable loadingidpanel = new DataTable();
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
                            MySqlCommand command2 = new MySqlCommand(IDpanel, connection);
                            MySqlDataAdapter adapter2 = new MySqlDataAdapter(command2);
                            adapter2.Fill(loadingidpanel);
                            loadingidpanel.Columns.Add("STT");
                            loadingidpanel.Columns["STT"].SetOrdinal(0);
                            int ixa = 1;
                            foreach (DataRow row in loadingidpanel.Rows)
                            {
                                row["STT"] = ixa++;
                            }
                            tableidpanel.ItemsSource = loadingidpanel.DefaultView;
                            //
                        }
                        connection.Close();
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: ");
                        connection.Close();
                        return;
                    }
                }
            }
            else
            {
                if (table2.SelectedCells != null && table2.SelectedCells.Count > 0)
                {
                    // Lấy giá trị ô được chọn
                    DataGridCellInfo cellInfo = table2.SelectedCells[1];
                    object data = cellInfo.Item;
                    DataRowView rowXY = data as DataRowView; // Ép kiểu sang DataRowView nếu cần thiết
                    string name_seach = rowXY["priority_defect_name"].ToString();

                    try
                    {
                        ConnectionSQL();
                        connection.Open();
                        if (connection.State == System.Data.ConnectionState.Open)
                        {

                            string IDpanel = "SELECT priority_defect_name,pt_datetime,panelid FROM product WHERE pt_datetime >= '" + Time_ST.Text + "' AND pt_datetime <= '" + Time_EN.Text + "' AND priority_defect_name = '" + name_seach + "' ";
                            DataTable loadingidpanel = new DataTable();
                            //
                            MySqlCommand command2 = new MySqlCommand(IDpanel, connection);
                            MySqlDataAdapter adapter2 = new MySqlDataAdapter(command2);
                            adapter2.Fill(loadingidpanel);
                            loadingidpanel.Columns.Add("STT");
                            loadingidpanel.Columns["STT"].SetOrdinal(0);
                            int ixa = 1;
                            foreach (DataRow row in loadingidpanel.Rows)
                            {
                                row["STT"] = ixa++;
                            }
                            tableidpanel.ItemsSource = loadingidpanel.DefaultView;
                            //
                        }
                        connection.Close();
                        return;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error: ");
                        connection.Close();
                        return;
                    }
                }
            }
        }
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
            Lane_A_CG.Visibility = Visibility.Collapsed;
            Lane_B_CG.Visibility = Visibility.Collapsed;
            Mapping_CGAOI.Visibility = Visibility.Collapsed;
            TableB.Visibility = Visibility.Collapsed;
            table2.Visibility = Visibility.Collapsed;
            Name_laneB.Visibility = Visibility.Collapsed;
            Name_laneA.Visibility = Visibility.Collapsed;
            Buttoncopyclipboard.Visibility = Visibility.Visible;
            tableIDpanel.Visibility = Visibility.Visible;
            tableIDpanel.Margin = new Thickness(463, 59, 684, 10);
            contentableidpanel.Content = "Table__Defect__ID__Mapping";
            Mapping_CGAOI.Margin = new Thickness(854, 64, 0, 0);
            checkerdataintable.IsEnabled = true;
            //Table Loading
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
                        table2.Visibility = Visibility.Visible;
                        //Mapping_CGAOI.Margin = new Thickness(854, 64, 0, 0);
                        string[] AddItemLineName = { "303_A", "303_B", "304_A", "304_B", "305_A", "305_B", "306_A", "306_B", "401_A", "401_B", "401_B", "402_A", "402_B", "403_A", "403_B", "404_A", "404_B", "405_A", "405_B", "501_A", "501_B", "502_A", "502_B", "503_A", "503_B", "504_A", "504_B" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }

                    }
                    break;
                case "CP_AOI":
                    {
                        tablebase.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Collapsed;
                        table2.Visibility = Visibility.Visible;
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
                        Mapping_CGAOI.Visibility = Visibility.Collapsed;
                        table2.Visibility = Visibility.Visible;
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
                        string[] AddItemLineName = { "406", "501", "502", "503", "504", "505", "506" };
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
                        Lane_A_CG.Visibility = Visibility.Visible;
                        Lane_B_CG.Visibility = Visibility.Visible;
                        table2.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Visibility = Visibility.Visible;
                        TableB.Visibility = Visibility.Visible;
                        Mapping_CGAOI.Margin = new Thickness(1181, 64, 0, 0);
                        tableIDpanel.Margin = new Thickness(844, 59, 336, 10);
                        Name_laneB.Visibility = Visibility.Visible;
                        Name_laneA.Visibility = Visibility.Visible;
                        Buttoncopyclipboard.Visibility = Visibility.Collapsed;
                        checkerdataintable.IsEnabled = false;
                        string[] AddItemLineName = { "303", "304", "305", "306", "401", "402", "403", "404", "405", "501", "502", "503", "504" };
                        foreach (string Item in AddItemLineName.ToArray())
                        {
                            LineSelection.Items.Add($"{Item}");
                        }
                    }
                    break;
            }
            
            /*if (MachineSelection.Text == "CG_AOI" )
            {
                tablebase.Visibility = Visibility.Visible;
                Mapping_CGAOI.Visibility = Visibility.Visible;
                Connten.Content = "CG_AOI Mapping ";
                LaneALTAMI.Content = "Stage 1";
                LaneBLTAMI.Content = "Stage 2";
                Mapping_CGAOI.Margin = new Thickness(788, 64, 0, 0);
                Lane_A_CG.Visibility = Visibility.Collapsed;
                Lane_B_CG.Visibility = Visibility.Collapsed;

            }
            else if (MachineSelection.Text == "LT_AMI")
            {
                tablebase.Visibility = Visibility.Visible;
                Mapping_CGAOI.Visibility = Visibility.Visible;
                Connten.Content = "LT_AMI Inspection Stage";
                LaneALTAMI.Content = "Stage 1,2,3,4";
                LaneBLTAMI.Content = "Stage 5,6,7,8";
                Mapping_CGAOI.Margin = new Thickness(788, 64, 0, 0);
                Lane_A_CG.Visibility = Visibility.Collapsed;
                Lane_B_CG.Visibility = Visibility.Collapsed;

            }
            else if (MachineSelection.Text == "CG_AOI_Plus")
            {
                
                Connten.Content = "CG_AOI Mapping ";
                LaneALTAMI.Content = "Stage 1";
                LaneBLTAMI.Content = "Stage 2";
                tablebase.Visibility = Visibility.Collapsed;
                Lane_A_CG.Visibility = Visibility.Visible;
                Lane_B_CG.Visibility = Visibility.Visible;
                Mapping_CGAOI.Visibility = Visibility.Visible;
                TableB.Visibility = Visibility.Visible;
                Mapping_CGAOI.Margin = new Thickness(1052, 64, 0, 0);
               // Mapping_CGAOI.Margin = "788,64,0,0";
                  //  Margin = "788,64,0,0"

            }
            else
            {
                tablebase.Visibility = Visibility.Visible;
                Mapping_CGAOI.Visibility = Visibility.Collapsed;
                LaneALTAMI.Content = "Lane A";
                LaneBLTAMI.Content = "Lane B";
                Lane_A_CG.Visibility = Visibility.Collapsed;
                Lane_B_CG.Visibility = Visibility.Collapsed;

            }*/
            
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
    }
}
