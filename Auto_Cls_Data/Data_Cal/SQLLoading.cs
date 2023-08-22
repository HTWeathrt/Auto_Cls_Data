using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;
using MySqlConnector;
using System.Xml.Linq;
using System.Threading;
using System.Data;

namespace Auto_Cls_Data.Data_Cal
{
    internal class SQLLoading
    {
        public void ClsConnect(string Name,string Linemachine)
        {

            IP_Class iP_Class = new IP_Class();
            iP_Class.IP_Selx(Name, Linemachine);
            string ip_selc = iP_Class.Ip_in;
            string dataXL = iP_Class.Data_Basexx;

            connection = new MySqlConnection("Server=" + ip_selc + "; Database=" + dataXL + "; Port=3306; User = ami; Password = protnc"); //charSet = utf8"
            connection.Open();
            if (connection.State == System.Data.ConnectionState.Open)
            {
                
            }
        }
        private string Defect;
        private string Judge;
        private MySqlConnection connection;
        private MySqlCommand cmd;
        private string TimeST;
        private string timeEN;
        private int Data_Limit;

        public MySqlConnection Connection { get => connection; set => connection = value; }

        public void ClsQueryALL()
        {
            if (Defect == "ALL")
            {
                if (Judge == "ALL")
                {
                    string query = "SELECT * FROM product WHERE pt_datetime >= '" + TimeST + "' AND pt_datetime <= '" + timeEN + "' LIMIT " + Data_Limit + "";
                    cmd = new MySqlCommand(query, connection);
                }
                if (Judge != "ALL")
                {
                    string query = "SELECT * FROM product WHERE pt_datetime >= '" + TimeST + "' AND pt_datetime <= '" + timeEN + "' AND judge = '" + Judge + "' LIMIT " + Data_Limit + "";
                    cmd = new MySqlCommand(query, connection);
                }
            }
            else if (Defect != "ALL")
            {
                string query = "SELECT * FROM product WHERE pt_datetime >= '" + TimeST + "' AND pt_datetime <= '" + timeEN + "' AND priority_defect_name = '" + Defect + "' LIMIT " + Data_Limit + "";
                cmd = new MySqlCommand(query, connection);
            }
            MySqlDataAdapter adapter = new MySqlDataAdapter(cmd);
            DataTable sqlbaseTable = new DataTable();
            adapter.Fill(sqlbaseTable);
            
        }
    }
}
