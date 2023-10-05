using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Auto_Cls_Data.Data_Cal;
using MySqlConnector;

namespace Auto_Cls_Data.Gplus
{
    public class CGAOIPlus
    {
        MySqlConnection connection;
        NewLoading newloading;
        SQLLoading sqload;


        MySqlCommand command;
        MySqlDataAdapter adapter;

        public DataTable Plus_cgaoi(string Machine , string Line , string TimerST, string TimerEN)
        {//DataTable DatataleB = cgaoiplus.Plus_cgaoiB(Machine, LineCGPlus, DataLimit, TimerST, TimerEN, Judge, Defection);
            DataTable sqlbaseTable = new DataTable();
            newloading = new NewLoading();
            sqload = new SQLLoading();
            try
            {
                string connect = sqload.DBShow(Machine, Line);
                connection = new MySqlConnection(connect);
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    //TableALL();
                    string query = newloading.LoadCG_AOI_Plus(TimerST,TimerEN);

                    command = new MySqlCommand(query, connection);
                   adapter = new MySqlDataAdapter(command);
                    adapter.Fill(sqlbaseTable);
                    sqlbaseTable.Columns.Add("STT");
                    sqlbaseTable.Columns["STT"].SetOrdinal(0);
                    int ixb = 1;
                    foreach (DataRow rowxa in sqlbaseTable.Rows)
                    {
                        rowxa["STT"] = ixb++;
                    }                  
                }
                connection.Close();
            }
            catch 
            {
                return null;  
            }
            return sqlbaseTable;
            // Cstring Machine,string Line ,string Lane,int DataLimit,string TimerST,string TimerEN,string Judge,string Defection)
        }
        public DataTable Plus_cgaoiB(string Machine, string Line,  string TimerST, string TimerEN)
        {
            DataTable sqlbaseTable = new DataTable();
            newloading = new NewLoading();
            sqload = new SQLLoading();
            try
            {
                string connect = sqload.DBShow(Machine, Line);
                connection = new MySqlConnection(connect);
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    //TableALL();
                    string query = newloading.LoadCG_AOI_Plus(TimerST, TimerEN);

                    command = new MySqlCommand(query, connection);
                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(sqlbaseTable);
                    sqlbaseTable.Columns.Add("STT");
                    sqlbaseTable.Columns["STT"].SetOrdinal(0);
                    int ixb = 1;
                    foreach (DataRow rowxa in sqlbaseTable.Rows)
                    {
                        rowxa["STT"] = ixb++;
                    }
                }
                connection.Close();
            }
            catch
            {
                return null;
            }
            return sqlbaseTable;
            // Cstring Machine,string Line ,string Lane,int DataLimit,string TimerST,string TimerEN,string Judge,string Defection)
        }

        public DataTable Plus_CGA(string Machine, string Line,int DataLimit, string TimerST, string TimerEN,string Judge,string Defection)
        {
            DataTable sqlbaseTable = new DataTable();
            newloading = new NewLoading();
            sqload = new SQLLoading();
            try
            {
                string connect = sqload.DBShow(Machine, Line);
                connection = new MySqlConnection(connect);
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    string query = newloading.TableDatabaseShow(TimerST, TimerEN,DataLimit,Judge,Defection);

                    command = new MySqlCommand(query, connection);
                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(sqlbaseTable);
                    sqlbaseTable.Columns.Add("STT");
                    sqlbaseTable.Columns["STT"].SetOrdinal(0);
                    int ixb = 1;
                    foreach (DataRow rowxa in sqlbaseTable.Rows)
                    {
                        rowxa["STT"] = ixb++;
                    }
                }
                connection.Close();
            }
            catch
            {
                return null;
            }
            return sqlbaseTable;
            // Cstring Machine,string Line ,string Lane,int DataLimit,string TimerST,string TimerEN,string Judge,string Defection)
        }
        public DataTable Plus_CGB(string Machine, string Line, int DataLimit, string TimerST, string TimerEN, string Judge, string Defection)
        {
            DataTable sqlbaseTable = new DataTable();
            newloading = new NewLoading();
            sqload = new SQLLoading();
            try
            {
                string connect = sqload.DBShow(Machine, Line);
                connection = new MySqlConnection(connect);
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    string query = newloading.TableDatabaseShow(TimerST, TimerEN, DataLimit, Judge, Defection);

                    command = new MySqlCommand(query, connection);
                    adapter = new MySqlDataAdapter(command);
                    adapter.Fill(sqlbaseTable);
                    sqlbaseTable.Columns.Add("STT");
                    sqlbaseTable.Columns["STT"].SetOrdinal(0);
                    int ixb = 1;
                    foreach (DataRow rowxa in sqlbaseTable.Rows)
                    {
                        rowxa["STT"] = ixb++;
                    }
                }
                connection.Close();
            }
            catch
            {
                return null;
            }
            return sqlbaseTable;
            // Cstring Machine,string Line ,string Lane,int DataLimit,string TimerST,string TimerEN,string Judge,string Defection)
        }
        /*private void CGplustableA(string Lane)
        {
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
        }*/

    }
}
