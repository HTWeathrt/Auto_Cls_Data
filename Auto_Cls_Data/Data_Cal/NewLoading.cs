using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Xceed.Wpf.AvalonDock.Themes;
using static System.Net.Mime.MediaTypeNames;

namespace Auto_Cls_Data.Data_Cal
{
    public class NewLoading
    {
        Seachdata seachdata = new Seachdata();
        public string LoadAssyAMI(string TimerST , string TimerEN)
        {
            string Zone_A = "SUM(CASE WHEN eqp_zone = 'A' THEN 1 ELSE 0 END) AS Zone_A";
            string Zone_B = "SUM(CASE WHEN eqp_zone = 'B' THEN 1 ELSE 0 END) AS Zone_B";
            string Zone_C = "SUM(CASE WHEN eqp_zone = 'C' THEN 1 ELSE 0 END) AS Zone_C";
            string Zone_D = "SUM(CASE WHEN eqp_zone = 'D' THEN 1 ELSE 0 END) AS Zone_D";
            string sqlX = "SELECT judge,final_defect_name," + Zone_A + "," + Zone_B + "," + Zone_C + "," + Zone_D + " FROM product WHERE pt_datetime >= '" + TimerST+ "' AND pt_datetime <= '" + TimerEN + "' GROUP BY final_defect_name LIMIT 50000";
            return sqlX;
        }
        public string TableDatabaseShow(string TimerST , string TimerEN , int Limited, string Judge,string Defection)
        {
            string query = "";
            if (Judge == "ALL")
            {
                query = "SELECT * FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' LIMIT " + Limited + "";
            }
            if (Judge != "ALL")
            {
                if (Judge == "OK" || Judge == "G")
                {
                    query = "SELECT * FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' AND judge = '" + Judge + "' LIMIT " + Limited + "";
                    
                }
                if (Judge == "NG" || Judge == "N")
                {
                    if (Defection == "ALL")
                    {
                        query = "SELECT * FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' AND judge =  'NG' LIMIT " + Limited + "";
                        
                    }
                    else
                    {
                        query = "SELECT * FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' AND priority_defect_name = '" + Defection + "' LIMIT " + Limited + "";
                        
                    }
                }
            }
            return query;
        }
        public string YRTtable(string Machine,string TimerST,string TimerEN,int Limited)
        {
            string sql = "";
            if (Machine == "CG_AOI")
            {
                //LoadingData Window1 = new LoadingData();
                sql = "SELECT judge, priority_defect_name, COUNT(*) AS count_defects, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "'), 2) AS percent_defects FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' GROUP BY judge, priority_defect_name ORDER BY priority_defect_name LIMIT " + Limited + "";

            }
            else if (Machine == "CP_AOI" || Machine == "LT_AMI")
            {
                sql = "SELECT judge, priority_defect_name, COUNT(*) AS count_defects, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "'), 2) AS percent_defects FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' GROUP BY judge, priority_defect_name ORDER BY priority_defect_name LIMIT " + Limited + "";
            }
            else if (Machine == "Assy_AMI")
            {
                //LoadingData Window1 = new LoadingData();
                sql = "SELECT judge, final_defect_name, COUNT(*) AS Count__Defect, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "'), 2) AS Percent FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' GROUP BY judge, final_defect_name ORDER BY final_defect_name LIMIT " + Limited + "";
            }
            else if (Machine == "IS_AOI")
            {
                //LoadingData Window1 = new LoadingData();
                sql = "SELECT judge, priority_defect_name, COUNT(*) AS count_defects, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "'), 2) AS percent_defects FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' GROUP BY judge, priority_defect_name ORDER BY priority_defect_name LIMIT " + Limited + "";
            }
            return sql;
        }
        public string TableBselection(string TimerST,string TimerEN,string name_seach)
        {
            string query = "SELECT priority_defect_name, priority_grid_pos, COUNT(*) AS CountPos FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' AND priority_defect_name = '" + name_seach + "'GROUP BY priority_defect_name,priority_grid_pos ";
            return query;
        }
        public string LoadISAOI(string TimerST, string TimerEN)
        {
            string Zone_A = "SUM(CASE WHEN prev_insp_chuck_no IN('B1','B2') THEN 1 ELSE 0 END) AS Head_1";
            string Zone_B = "SUM(CASE WHEN prev_insp_chuck_no IN('B3','B4') THEN 1 ELSE 0 END) AS Head_2";
            string Zone_C = "SUM(CASE WHEN prev_insp_chuck_no IN('B5','B6') THEN 1 ELSE 0 END) AS Head_3";
            string Manual = "SUM(CASE WHEN prev_insp_chuck_no IN('MA','MB') THEN 1 ELSE 0 END) AS Input_M";
            string Else = "SUM(CASE WHEN prev_insp_chuck_no NOT IN ('B1','B2','B3','B4','B5','B6','MA','MB')  THEN 1 ELSE 0 END) AS Other";
            string sqlX = "SELECT judge,priority_defect_name," + Zone_A + "," + Zone_B + "," + Zone_C + ","+ Manual + ","+Else+" FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' GROUP BY priority_defect_name LIMIT 50000";
            return sqlX;
        }
        public string SelectionTable(string TimerST, string TimerEN,string Name_Seach , string machine)
        {
            string query ="";
            string Where = $"WHERE pt_datetime >= '{TimerST}' AND pt_datetime <= '{TimerEN}'";
            switch (machine)
            {
                case "CG_AOI_Plus":
                    {
                         query = $"SELECT priority_defect_name, priority_grid_pos, COUNT(*) AS CountPos FROM product {Where} AND priority_defect_name = '" + Name_Seach + "'GROUP BY priority_defect_name,priority_grid_pos ";
                    }
                    break;
                case "IS_AOI":
                    {
                        query = $"SELECT priority_defect_name,panelid,pt_datetime FROM product {Where} AND priority_defect_name = '{Name_Seach}' ";
                    }
                    break;
                case "LT_AMI":
                    {
                        query = $"SELECT priority_defect_name, display_insp, panelid,pt_datetime FROM product {Where} AND priority_defect_name = '{Name_Seach}'";
                    }
                    break;
                case "CG_AOI":
                    {
                       query = $"SELECT priority_defect_name, priority_grid_pos, COUNT(*) AS CountPos FROM product {Where} AND priority_defect_name = '" + Name_Seach + "'GROUP BY priority_defect_name,priority_grid_pos ";
                    }
                    break;
                case "Assy_AMI":
                    {
                        query = $"SELECT localid,panelid,short_serial_no,final_defect_name,stage_index,eqp_zone,channel_index,pt_datetime FROM product {Where} AND final_defect_name = '" + Name_Seach + "' ";
                    }
                    break;
                case "CP_AOI":
                    {
                        query = $"SELECT priority_defect_name,panelid,bcrid,inspection_zone,pt_datetime FROM product {Where} AND priority_defect_name = '{Name_Seach}' ";
                    }
                    break;
            }
            return query;
        }
        public string LoadCPAOI(string TimerST, string TimerEN)
        {
            string Zone_A = "SUM(CASE WHEN inspection_zone IN('A') THEN 1 ELSE 0 END) AS LANE__A";
            string Zone_B = "SUM(CASE WHEN inspection_zone IN('B') THEN 1 ELSE 0 END) AS LANE__B";
            string Else = "SUM(CASE WHEN inspection_zone NOT IN ('A','B')  THEN 1 ELSE 0 END) AS Other";
            string sqlX = "SELECT judge,priority_defect_name," + Zone_A + "," + Zone_B + "," + Else + " FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' GROUP BY priority_defect_name LIMIT 50000";
            return sqlX;
        }
        public string LoadLTAMI(string TimerST, string TimerEN)
        {
            string mot = "SUM(CASE WHEN display_insp IN('1') THEN 1 ELSE 0 END) AS tb__1";
            string hai = "SUM(CASE WHEN display_insp IN('2') THEN 1 ELSE 0 END) AS tb__2";
            string ba = "SUM(CASE WHEN display_insp IN('3') THEN 1 ELSE 0 END) AS tb__3";
            string bon = "SUM(CASE WHEN display_insp IN('4') THEN 1 ELSE 0 END) AS tb__4";
            string nam = "SUM(CASE WHEN display_insp IN('5') THEN 1 ELSE 0 END) AS tb__5";
            string sau = "SUM(CASE WHEN display_insp IN('6') THEN 1 ELSE 0 END) AS tb__6";
            string bay = "SUM(CASE WHEN display_insp IN('7') THEN 1 ELSE 0 END) AS tb__7";
            string tam = "SUM(CASE WHEN display_insp IN('8') THEN 1 ELSE 0 END) AS tb__8";
            string Else = "SUM(CASE WHEN display_insp NOT IN ('1','2','3','4','5','6','7','8')  THEN 1 ELSE 0 END) AS Other";
            string sqlX = $"SELECT judge,priority_defect_name,{mot},{hai},{ba},{bon},{nam},{sau},{bay},{tam},{Else} FROM product WHERE pt_datetime >= '" + TimerST + "' AND pt_datetime <= '" + TimerEN + "' GROUP BY priority_defect_name LIMIT 50000";
            return sqlX;
        }
        public string LoadCG_AOI_Plus(string TimerST,string TimerEN)
        {
            string Limit = $"2) AS Percent FROM product WHERE pt_datetime >= '{TimerST}' AND pt_datetime <= '{TimerEN}' GROUP BY judge, priority_defect_name ORDER BY priority_defect_name ";
            string sql = $"SELECT judge, priority_defect_name, COUNT(*) AS Count_Def, ROUND(COUNT(*) * 100.0 / (SELECT COUNT(*) FROM product WHERE pt_datetime >= '" +TimerST+ "' AND pt_datetime <= '" + TimerEN + "'),"+Limit+"";
            return sql;
        }
        
    }

}
