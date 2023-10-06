using System;
using System.Collections;
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
        public string LoadingData(string Machine,int lengh, string line,bool SeachMode,string IDList)
        {
            
            SQLLoading sqload = new SQLLoading();
            string query;
            string seach = string.Empty;
            try
            {
                if (SeachMode)
                {
                    seach = "panelid";
                }
                else
                {
                    seach = "short_serial_no";
                }
                if (IDList.IndexOf(' ') > 1 || IDList.IndexOf('\n') < 2)
                {
                    // Seach panel ID thiếu hụt
                    string IDSeach1 = "'" + IDList + "%'";
                    query = "SELECT * FROM product WHERE " + seach + " LIKE " + IDSeach1;

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
                    query = "SELECT * FROM product WHERE " + seach + " IN ('" + output + "')";
                }
            }
            catch
            {
                return null;
            }
            return query;

        }


    }
}
