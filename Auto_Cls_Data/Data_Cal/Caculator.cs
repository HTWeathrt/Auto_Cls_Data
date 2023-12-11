using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using MySqlConnector;

namespace Auto_Cls_Data.Data_Cal
{
    public class Caculator
    {
        
        public Caculator() 
        {
            
        }
        NewLoading newloading;
        public DataTable LDTable(string Machine, string TimerST, string TimerEN, MySqlConnection connection)
        {
            DataTable sqlbaseTable = new DataTable();
            newloading = new NewLoading();
            string SQLLD = "";
            if (Machine == "Assy_AMI")
            {
                SQLLD = newloading.LoadAssyAMI(TimerST, TimerEN);
            }
            if (Machine == "CP_AOI")
            {
                SQLLD = newloading.LoadCPAOI(TimerST, TimerEN);
            }
            if (Machine == "LT_AMI")
            {
                SQLLD = newloading.LoadLTAMI(TimerST, TimerEN);
            }
            //
            MySqlCommand SQLCommandloading = new MySqlCommand(SQLLD, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(SQLCommandloading);
           
            adapter.Fill(sqlbaseTable);
            sqlbaseTable.Columns.Add("STT");
            sqlbaseTable.Columns["STT"].SetOrdinal(0);
            int ixb = 1;
            foreach (DataRow rowxa in sqlbaseTable.Rows)
            {
                rowxa["STT"] = ixb++;
            }
            return sqlbaseTable;
            


        }
        public DataTable TableBase (string TimerST, string TimerEN, int DataLimit, string Judge,string Defection,MySqlConnection connection)
        {
            DataTable sqlbaseTable = new DataTable();
            newloading = new NewLoading();
            string sqlselection = newloading.TableDatabaseShow(TimerST, TimerEN, DataLimit, Judge, Defection);
            MySqlCommand cmd = new MySqlCommand(sqlselection, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(sqlbaseTable);
            sqlbaseTable.Columns.Add("STT");
            sqlbaseTable.Columns["STT"].SetOrdinal(0);
            int ixb = 1;
            foreach (DataRow rowxa in sqlbaseTable.Rows)
            {
                rowxa["STT"] = ixb++;
            }
            return sqlbaseTable;
        }
        public DataTable YRTTable(string Machine,string TimerST,string TimerEN,int Limited,MySqlConnection connection)
        {
            newloading = new NewLoading();
            //Table dữ liệu tính toán cho CG_AOI
            string sql = newloading.YRTtable(Machine, TimerST, TimerEN, Limited);
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
            return sqltbrateTable;

            
        }
        public DataTable SeachID (string Query, MySqlConnection connection)
        {   DataTable dt = new DataTable();
            MySqlCommand cmd = new MySqlCommand(Query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(dt);
            dt.Columns.Add("STT");
            dt.Columns["STT"].SetOrdinal(0);
            int ixb = 1;
            foreach (DataRow rowxa in dt.Rows)
            {
                rowxa["STT"] = ixb++;
            }
            return dt;
        }
        public DataTable CGBaseProcess(string Query, MySqlConnection connection)
        {
            DataTable sqlbaseTable = new DataTable();
            MySqlCommand cmd = new MySqlCommand(Query, connection);
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            adapter.Fill(sqlbaseTable);
            sqlbaseTable.Columns.Add("STT");
            sqlbaseTable.Columns["STT"].SetOrdinal(0);
            int ixb = 1;
            foreach (DataRow rowxa in sqlbaseTable.Rows)
            {
                rowxa["STT"] = ixb++;
            }
            return sqlbaseTable;
        }
        public DataTable CGYRTProcess(string Query, MySqlConnection connection)
        {
            int ixb = 1;
            MySqlCommand Conx = new MySqlCommand(Query, connection);
            MySqlDataAdapter Adater2 = new MySqlDataAdapter(Conx);
            DataTable SQLDefect = new DataTable();
            Adater2.Fill(SQLDefect);
            SQLDefect.Columns.Add("STT");
            SQLDefect.Columns["STT"].SetOrdinal(0);
            foreach (DataRow rowxa in SQLDefect.Rows)
            {
                rowxa["STT"] = ixb++;
            }
            return SQLDefect;

        }
        
    }
}
