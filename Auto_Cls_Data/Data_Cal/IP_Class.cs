using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Auto_Cls_Data.Data_Cal
{
    internal class IP_Class
    {
        private string Name_Machix;
        private string data_Basexx;
        private string Line;
        private string ip_in;
        public string Ip_in { get => ip_in; set => ip_in = value; }
        public string Data_Basexx { get => data_Basexx; set => data_Basexx = value; }
        public void IP_Selx(string Name, string Linemachine)
        {
            Name_Machix = Name;
            Line = Linemachine;
            // Loading DataBase
            if (Name_Machix == "IS_AOI")
            {
                Data_Basexx = "inksealing_aoi";
                switch (Line)
                {
                    case "301":
                        ip_in = "10.119.161.32"; break;
                    case "302":
                        ip_in = "10.119.135.46"; break;
                    case "303":
                        ip_in = "10.119.160.220"; break;
                    case "304":
                        ip_in = "10.119.160.224"; break;
                    case "305":
                        ip_in = "10.119.160.229"; break;
                    case "306":
                        ip_in = "10.119.212.101"; break;
                    case "402":
                        ip_in = "10.121.46.169"; break;
                    case "404":
                        ip_in = "10.121.43.66"; break;
                    case "405":
                        ip_in = "10.121.41.119"; break;
                    case "406":
                        ip_in = "10.121.41.114"; break;
                    case "501":
                        ip_in = "10.121.52.12"; break;
                    case "502":
                        ip_in = "10.121.52.15"; break;
                    case "503":
                        ip_in = "10.121.56.28"; break;
                    case "504":
                        ip_in = "10.121.56.24"; break;
                    case "505":
                        ip_in = "10.121.53.123"; break;
                    case "506":
                        ip_in = "10.121.56.160"; break;
                }
            }
            if (Name_Machix == "CG_AOI" || Name_Machix == "CG_AOI_Plus")
            {
                Data_Basexx = "ami_cg";
                switch (Line)
                {
                    case "303_A":
                        ip_in = "10.119.160.107"; break;
                    case "303_B":
                        ip_in = "10.119.160.66"; break;
                    case "304_A":
                        ip_in = "10.119.160.68"; break;
                    case "304_B":
                        ip_in = "10.119.160.70"; break;
                    case "305_A":
                        ip_in = "10.119.160.72"; break;
                    case "305_B":
                        ip_in = "10.119.160.74"; break;
                    case "306_A":
                        ip_in = "10.119.160.76"; break;
                    case "306_B":
                        ip_in = "10.119.160.77"; break;
                    case "401_A":
                        ip_in = "10.121.45.251"; break;
                    case "401_B":
                        ip_in = "10.121.45.253"; break;
                    case "402_A":
                        ip_in = "10.121.46.147"; break;
                    case "402_B":
                        ip_in = "10.121.46.149"; break;
                    case "403_A":
                        ip_in = "10.121.46.155"; break;
                    case "403_B":
                        ip_in = "10.121.46.157"; break;
                    case "404_A":
                        ip_in = "10.121.47.177"; break;
                    case "404_B":
                        ip_in = "10.121.47.179"; break;
                    case "405_A":
                        ip_in = "10.121.46.142"; break;
                    case "405_B":
                        ip_in = "10.121.46.144"; break;
                    case "501_A":
                        ip_in = "10.121.54.46"; break;
                    case "501_B":
                        ip_in = "10.121.54.48"; break;
                    case "502_A":
                        ip_in = "10.121.54.50"; break;
                    case "502_B":
                        ip_in = "10.121.54.52"; break;
                    case "503_A":
                        ip_in = "10.121.51.12"; break;
                    case "503_B":
                        ip_in = "10.121.51.14"; break;
                    case "504_A":
                        ip_in = "10.121.51.18"; break;
                    case "504_B":
                        ip_in = "10.121.51.16"; break;
                }
            }
            if (Name_Machix == "CP_AOI")
            {
                Data_Basexx = "cp_aoi";
                switch (Line)
                {
                    case "301":
                        ip_in = "10.119.135.254"; break;
                    case "302":
                        ip_in = "10.119.161.15"; break;
                    case "303":
                        ip_in = "10.119.161.73"; break;
                    case "304":
                        ip_in = "10.119.135.245"; break;
                    case "305":
                        ip_in = "10.119.161.17"; break;
                    case "306":
                        ip_in = "10.119.135.119"; break;
                    case "401":
                        ip_in = "1"; break; // chưa có máy
                    case "402":
                        ip_in = "1"; break; // chưa có máy
                    case "403":
                        ip_in = "1"; break; // chưa có máy
                    case "404":
                        ip_in = "10.121.8.60"; break;
                    case "405":
                        ip_in = "1"; break; // chưa có máy
                    case "406":
                        ip_in = "10.121.8.77"; break;
                    case "501":
                        ip_in = "10.121.52.26"; break;
                    case "502":
                        ip_in = "10.121.56.29"; break;
                    case "503":
                        ip_in = "10.121.56.34"; break;
                    case "504":
                        ip_in = "10.121.56.31"; break;
                    case "505":
                        ip_in = "10.121.55.93"; break;
                    case "506":
                        ip_in = "10.121.56.158"; break;
                }
            }
            if (Name_Machix == "LT_AMI")
            {
                Data_Basexx = "ami_lt";
                switch (Line)
                {
                    case "406":
                        ip_in = "10.121.41.104"; break;
                    case "501":
                        ip_in = "10.121.52.22"; break;
                    case "502":
                        ip_in = "10.121.52.23"; break;
                    case "503":
                        ip_in = "10.121.56.62"; break;
                    case "504":
                        ip_in = "10.121.56.30"; break;
                    case "505":
                        ip_in = "10.121.55.94"; break;
                    case "506":
                        ip_in = "10.121.56.159"; break;
                }
            }
            if (Name_Machix == "Assy_AMI")
            {
                Data_Basexx = "assembly_ami";
                switch (Line)
                {
                    case "301":
                        ip_in = "10.119.128.11"; break;
                }
            }
        }

    }
}
