using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using MySqlConnector;

namespace Auto_Cls_Data.Data_Cal
{
    public  class SeachDataCls
    {
        public DataTable LoadingData(string Machine,int lengh, string line,bool SeachMode,string IDList)
        {
            DataTable dt = new DataTable();
            SQLLoading sqload = new SQLLoading();
            try
            {
                string seach = string.Empty;
                string connector = sqload.DBShow(Machine, line);
                MySqlConnection connection = new MySqlConnection(connector);
                if (Machine == "CG_AOI_Plus")
                {
                    return dt = null;
                }
                if (IDList == string.Empty || lengh >= 10)
                {
                    return dt = null;
                }
                if(SeachMode)
                {
                    seach = "panelid";
                }
                else
                {
                    seach = "short_serial_no";
                }
                connection.Open();
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    if (IDList.IndexOf(' ') > 1 || IDList.IndexOf('\n') < 2)
                    {
                        // Seach panel ID thiếu hụt
                        string IDSeach1 = "'" + IDList + "%'";
                        string query = "SELECT * FROM product WHERE "+seach+" LIKE " + IDSeach1;
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                    else
                    {
                        string output = IDList;
                        if (output.Contains("\r") || output.Contains("\n"))
                        {
                            output = output.Replace("\r", "").Replace("\n", "','");
                        }
                        if (output.EndsWith("','"))
                        {
                            output = output.Substring(0, output.Length - 3);
                        }
                        string query = "SELECT * FROM product WHERE "+seach+" IN ('" + output + "')";
                        MySqlCommand cmd = new MySqlCommand(query, connection);
                        MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
                        adapter.Fill(dt);
                    }
                    dt.Columns.Add("STT");
                    dt.Columns["STT"].SetOrdinal(0);
                    int ixb = 1;
                    foreach (DataRow rowxa in dt.Rows)
                    {
                        rowxa["STT"] = ixb++;
                    }
                    connection.Close();
                }
            }
            catch
            {
                return null;
            }
            return dt;
        }


    }
}
