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
    {    IP_Class ip_class = new IP_Class();
        public string DBShow(string Machine , string Line)
        {
            
            ip_class.IP_Selx(Machine, Line);
            string ip_selc = ip_class.Ip_in;
            string dataXL = ip_class.Data_Basexx;
            string Loading = "Server=" + ip_selc + "; Database=" + dataXL + "; Port=3306; User = ami; Password = protnc";
            return Loading;

        }
    }
}
