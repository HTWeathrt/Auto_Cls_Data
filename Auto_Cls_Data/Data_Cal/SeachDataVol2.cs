using Microsoft.Xaml.Behaviors.Media;
using MySqlConnector;
using Serilog.Filters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Controls;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;

namespace Auto_Cls_Data.Data_Cal
{
    public  class SeachDataVol2
    {
        NewLoading FormulaQuery = new NewLoading();
        SQLLoading sqload = new SQLLoading();
        public DataTable DataBaseQuery(string Connec, string TimerST, string TimerEN, int Limited,string Judge, string Defection)
        {//Machine, Line, TimerST, TimerEN, Limited, Judge, Defection
            DataTable dt = new DataTable();
            string Query = FormulaQuery.TableDatabaseShow(TimerST, TimerEN, Limited, Judge, Defection);
            //string Connec = sqload.DBShow(Machine, Line);
            //string TimerST , string TimerEN , int Limited, string Judge,string Defection
            MySqlConnection connection = new MySqlConnection(Connec);
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
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
            else
            {
                return null;
            }
            
        }
        public DataTable DataYRTQuery(string Connec,string Machine, string TimerST, string TimerEN, int Limited)
        {//Machine, Line, TimerST, TimerEN, Limited, Judge, Defection
            DataTable dt = new DataTable();
            string Query = FormulaQuery.YRTtable(Machine, TimerST, TimerEN, Limited);
            //YRTtable(string Machine,string TimerST,string TimerEN,int Limited)
            //string Connec = sqload.DBShow(Machine, Line);
            //string TimerST , string TimerEN , int Limited, string Judge,string Defection
            MySqlConnection connection = new MySqlConnection(Connec);
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
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
            else
            {
                return null;
            }
            
        }
        public DataTable LDTable(string Machine,string Line, string TimerST, string TimerEN)
        {
            DataTable sqlbaseTable = new DataTable();

            string Connec = sqload.DBShow(Machine, Line);
            //string TimerST , string TimerEN , int Limited, string Judge,string Defection
            MySqlConnection connection = new MySqlConnection(Connec);
            connection.Open();
            if(connection.State == System.Data.ConnectionState.Open)
            {
                string SQLLD = "";
                if (Machine == "Assy_AMI")
                {
                    SQLLD = FormulaQuery.LoadAssyAMI(TimerST, TimerEN);
                }
                if (Machine == "CP_AOI")
                {
                    SQLLD = FormulaQuery.LoadCPAOI(TimerST, TimerEN);
                }
                if (Machine == "LT_AMI")
                {
                    SQLLD = FormulaQuery.LoadLTAMI(TimerST, TimerEN);
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
            else
            {
                return null;
            }
        }
        public DataTable CGAOIPLS(string connect, string Line , string TimerST, string TimerEN, int Limit)
        {
            DataTable sqlbaseTable = new DataTable();
           // sqload = new SQLLoading();
            //string connect = sqload.DBShow(Machine, Line);
            MySqlConnection connection = new MySqlConnection(connect);
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                string query = FormulaQuery.LoadCG_AOI_Plus(TimerST, TimerEN);
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
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
            else
            {
                return null;
            }
        }
        public DataTable CGAOIYRTPLUS(string connect, string Line, string TimerST, string TimerEN, int Limit, string Judge , string Defection)
        {
            DataTable sqlbaseTable = new DataTable();
           // sqload = new SQLLoading();
           // string connect = sqload.DBShow(Machine, Line);
            MySqlConnection connection = new MySqlConnection(connect);
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                string query = FormulaQuery.TableDatabaseShow(TimerST, TimerEN, Limit, Judge, Defection);
               // string query = FormulaQuery.LoadCG_AOI_Plus(TimerST, TimerEN);
                MySqlCommand command = new MySqlCommand(query, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
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
            else
            {
                return null;
            }


            // DataTable sqlbaseTable = new DataTable();
           
        }
        public DataTable SeachData (string Machine , string Line,int Length, string IDList,bool seachpanel)
        {
            DataTable xxxx = new DataTable();
            SeachDataCls seachquery = new SeachDataCls();
            sqload = new SQLLoading();

            string connect = sqload.DBShow(Machine, Line);
            MySqlConnection connection = new MySqlConnection(connect);
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                string loadingdata = seachquery.LoadingData(Machine, Length, Line, seachpanel, IDList);
                MySqlCommand command = new MySqlCommand(loadingdata, connection);
                MySqlDataAdapter adapter = new MySqlDataAdapter(command);
                adapter.Fill(xxxx);
                xxxx.Columns.Add("STT");
                xxxx.Columns["STT"].SetOrdinal(0);
                int ixb = 1;
                foreach (DataRow rowxa in xxxx.Rows)
                {
                    rowxa["STT"] = ixb++;
                }
                return xxxx;
            }
            else
            {
                return null;
            }
            
        }
    }
}
