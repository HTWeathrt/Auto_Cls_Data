using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using Xceed.Wpf.Toolkit;

namespace Auto_Cls_Data.Data_Cal
{
    internal class Tuktukdata
    {
        
        
        public void Base()
        {
            

        }
        private string aLarm;

        public string ALarm { get => aLarm; set => aLarm = value; }

        public void TotalALL(int[] TotalBangALL)
        {
            int Total_ALL_ALL = TotalBangALL[0];
            int Total_ALL_OK = TotalBangALL[1];
            int Total_ALL_NG = TotalBangALL[2];
            int Total_ALL_NA = TotalBangALL[3];
            
            if(Total_ALL_ALL > 0)
            {
                aLarm = "Hhehe";
            }    
            
           
        }
        public void TotalTableA(int[] TotalTableA)
        {
            int Total_A_ALL = TotalTableA[0];
            int Total_A_OK = TotalTableA[1];
            int Total_A_NG = TotalTableA[2];
            int Total_A_NA = TotalTableA[3];

        }
        public void TotalTableB(int[] TotalTableB)
        {
            int Total_B_ALL = TotalTableB[0];
            int Total_B_OK = TotalTableB[1];
            int Total_B_NG = TotalTableB[2];
            int Total_B_NA = TotalTableB[3];
        }
        public void TotalNGStage(int[] TotalNGStageA)
        {
            int NG_A1 = TotalNGStageA[0];
            int NG_A2 = TotalNGStageA[1];
            int NG_B1 = TotalNGStageA[2];
            int NG_B2 = TotalNGStageA[3];
        }
        public void TotalALLStage(int[] TotalALLStage)
        {
            int A1TT = TotalALLStage[0];
            int A2TT = TotalALLStage[1];
            int B1TT = TotalALLStage[2];
            int B2TT = TotalALLStage[3];

        }


    }
}
