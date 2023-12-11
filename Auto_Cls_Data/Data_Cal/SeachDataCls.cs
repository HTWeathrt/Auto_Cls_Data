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
        public string LoadingData(string Machine,int lengh, string line,bool SeachMode,string IDSeachcheck)
        {
            string query = string.Empty;
            string seach = string.Empty;
            
            try
            {
                if (SeachMode)
                {
                    seach = "panelid";
                }
                else
                {
                    switch (Machine)
                    {
                        case "CP_AOI":
                            {
                                seach = "pid";
                            }
                            break;
                        case "IS_AOI":
                            {
                                seach = "short_serial_no";
                            }
                            break;
                        case "CG_AOI":
                            {
                                seach = "pid";
                            }
                            break;
                        case "LT_AMI":
                            {
                                seach = "idproduct";
                            }
                            break;
                        case "Assy_AMI":
                            {
                                seach = "short_serial_no";
                            }
                            break;
                    }
                }
                string output = IDSeachcheck;
                List<string> listIDSeach = new List<string>(output.Split('\n'));
                int ACx = Convert.ToInt32(listIDSeach[0].ToString().Length);
                string Result = string.Join("','", listIDSeach);
                Result = Result.Replace("\r", "").Replace("\n", "");
                Result = Result.Substring(0, Result.Length - 3);
                
                query = $"SELECT * FROM product WHERE {seach} IN ('{Result}')";
            }
            catch
            {
                return null;
            }
            return query;

        }


    }
}
