using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
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

        /*private void IS_ReportAl(DataTable tabledataYrt)
        {
            List<string> SocicalDefect = new List<string>();
            List<int> TotalNG = new List<int>();
            List<int> TotalALL = new List<int>();
            List<int> TOTALNGXXX = new List<int>();
            SocicalDefect.Clear();
            TotalNG.Clear();
            TotalALL.Clear();
            TOTALNGXXX.Clear();
            foreach (DataRow abc in tabledataYrt.Rows)
            {
                string TOP = abc["priority_defect_name"].ToString();
                if (abc["Judge"].ToString() != "N/A" && TOP.Length > 5)
                {
                    string Total = abc["count_defects"].ToString();
                    if (TOP.Substring(0, 3) == "Top")
                    {
                        string Jdge = abc["Judge"].ToString();
                        if (Jdge == "NG")
                        {
                            TotalNG.Add(Convert.ToInt32(Total));
                        }
                    }
                    if (TOP.Substring(TOP.Length - 2, 2) == "GB")
                    { //Substring(somestring.Length-3, 3

                        string Jdge = abc["Judge"].ToString();
                        if (Jdge == "NG")
                        {
                            TOTALNGXXX.Add(Convert.ToInt32(Total));
                        }
                    }
                    TotalALL.Add(Convert.ToInt32(Total));

                }
            }
            int SUMALLX = TotalALL.Sum();
            float TSUMALLX = float.Parse(SUMALLX.ToString());
            int Sum = TotalNG.Sum();
            float SumX = float.Parse(Sum.ToString());
            int SUMNOGB = TOTALNGXXX.Sum();
            float SUMNOGBX = float.Parse(SUMNOGB.ToString());
            string Nonst = "";
            if (SUMALLX > 0)
            {
                float SUMALYRT = (SumX / TSUMALLX) * 100;
                float SUMALL = float.Parse(SUMALYRT.ToString("#.#"));

                if (SUMNOGB > 0)
                {
                    float SUM_GB_YR = (SUMNOGBX / TSUMALLX) * 100;
                    float SYMX = SUMALYRT - SUM_GB_YR;
                    float SYMXALL = float.Parse(SYMX.ToString("#.#"));

                    Nonst = "Total : " + SUMALL + "%(" + SYMXALL + "%)";
                }
                else
                {
                    Nonst = "Total : " + SUMALL + "%(" + SUMALL + "%)";
                }


            }
            string DattimeSW = $"※ {LineSelection.Text} Top Edge : {Time_ST.Text}--{Time_EN.Text}\nInput: {TSUMALLX.ToString()}ea \nGuard Band 적용(미적용)";

            SocicalDefect.Add(DattimeSW);
            SocicalDefect.Add(Nonst);

            string CGOPEN = "";
            int CGOPENX = 0;
            string MP_Open = "";
            int MP_OpenX = 0;
            string CG_Particle = "";
            int CG_ParticleX = 0;
            //GS MAX

            string TOP_GS_MAX = "";
            string TOP_GS_MAX_GB = "";
            int TOP_GS_MAXX = 0;
            int TOP_GS_MAX_GBX = 0;
            // GS MIN

            string TOP_GS_MIN = "";
            string TOP_GS_MIN_GB = "";
            int TOP_GS_MINX = 0;
            int TOP_GS_MIN_GBX = 0;
            //GR MAX

            string TOP_GR_MAX = "";
            string TOP_GR_MAX_GB = "";
            int TOP_GR_MAXX = 0;
            int TOP_GR_MAX_GBX = 0;
            //GR MIN

            string TOP_GR_MIN = "";
            string TOP_GR_MIN_GB = "";
            int TOP_GR_MINX = 0;
            int TOP_GR_MIN_GBX = 0;
            ///GTMAX

            string TOP_GT_MAX = "";
            string TOP_GT_MAX_GB = "";
            int TOP_GT_MAXX = 0;
            int TOP_GT_MAX_GBX = 0;
            //GT MIn

            string TOP_GT_MIN = "";
            string TOP_GT_MIN_GB = "";
            int TOP_GT_MINX = 0;
            int TOP_GT_MIN_GBX = 0;
            ///// TOP GU MAX A

            string TOP_GU_Max_A = "";
            string TOP_GU_Max_A_GB = "";
            int TOP_GU_Max_AX = 0;
            int TOP_GU_Max_A_GBX = 0;
            //TOP GU MAXB

            string TOP_GU_Max_B = "";
            string TOP_GU_Max_B_GB = "";
            int TOP_GU_Max_BX = 0;
            int TOP_GU_Max_B_GBX = 0;
            // TOP Error

            string TOPErrorAB = "";
            string TOPGT_Max_Abnormal = "";
            int TOPErrorABX = 0;
            int TOPGT_Max_AbnormalX = 0;

            foreach (DataRow row in tabledataYrt.Rows)
            {
                string Defectname;
                string YRT;

                string rowpid = row["priority_defect_name"].ToString();
                //
                /// total
                if (rowpid != string.Empty)
                {
                    if (rowpid.Substring(0, 3) == "Top")
                    {
                        //string Showmathc = rowpid.Substring(0, 3);
                        Defectname = row["priority_defect_name"].ToString();

                        string Jdge = row["Judge"].ToString();
                        if (Jdge == "NG")
                        {
                            YRT = row["percent_defects"].ToString();
                            switch (Defectname)
                            {
                                case "Top_CG_Open":
                                    {
                                        CGOPEN = YRT;
                                        CGOPENX = 1;
                                    }
                                    break;
                                case "Top_MP_Open":
                                    {
                                        MP_Open = YRT;
                                        MP_OpenX = 1;
                                    }
                                    break;
                                case "Top_CG_Particle":
                                    {
                                        CG_Particle = YRT;
                                        CG_ParticleX = 1;
                                    }
                                    break;
                                case "Top_GS_Max":
                                    {
                                        TOP_GS_MAX = YRT;
                                        TOP_GS_MAXX = 1;
                                    }
                                    break;
                                case "Top_GS_Max_GB":
                                    {
                                        TOP_GS_MAX_GB = YRT;
                                        TOP_GS_MAX_GBX = 1;
                                    }
                                    break;
                                case "Top_GS_Min":
                                    {
                                        TOP_GS_MIN = YRT;
                                        TOP_GS_MINX = 1;
                                    }
                                    break;
                                case "Top_GS_Min_GB":
                                    {
                                        TOP_GS_MIN_GB = YRT; ;
                                        TOP_GS_MIN_GBX = 1;
                                    }
                                    break;
                                case "Top_GR_Max":
                                    {
                                        TOP_GR_MAX = YRT;
                                        TOP_GR_MAXX = 1;
                                    }
                                    break;
                                case "Top_GR_Max_GB":
                                    {
                                        TOP_GR_MAX_GB = YRT;
                                        TOP_GR_MAX_GBX = 1;
                                    }
                                    break;
                                case "Top_GR_Min":
                                    {
                                        TOP_GR_MIN = YRT;
                                        TOP_GR_MINX = 1;

                                    }
                                    break;
                                case "Top_GR_Min_GB":
                                    {
                                        TOP_GR_MIN_GB = YRT;
                                        TOP_GR_MIN_GBX = 1;

                                    }
                                    break;
                                case "Top_GT_Max":
                                    {
                                        TOP_GT_MAX = YRT;
                                        TOP_GT_MAXX = 1;
                                    }
                                    break;
                                case "Top_GT_Max_GB":
                                    {
                                        TOP_GT_MAX_GB = YRT;
                                        TOP_GT_MAX_GBX = 1;

                                    }
                                    break;
                                case "Top_GT_Min":
                                    {
                                        TOP_GT_MIN = YRT;
                                        TOP_GT_MINX = 1;
                                    }
                                    break;
                                case "Top_GT_Min_GB":
                                    {
                                        TOP_GT_MIN_GB = YRT;
                                        TOP_GT_MIN_GBX = 1;
                                    }
                                    break;
                                case "Top_GU_Max_A":
                                    {
                                        TOP_GU_Max_A = YRT;
                                        TOP_GU_Max_AX = 1;

                                    }
                                    break;
                                case "Top_GU_Max_A_GB":
                                    {
                                        TOP_GU_Max_A_GB = YRT;
                                        TOP_GU_Max_A_GBX = 1;
                                    }
                                    break;
                                case "Top_GU_Max_B":
                                    {
                                        TOP_GU_Max_B = YRT;
                                        TOP_GU_Max_BX = 1;
                                    }
                                    break;
                                case "Top_GU_Max_B_GB":
                                    {
                                        TOP_GU_Max_B_GB = YRT;
                                        TOP_GU_Max_B_GBX = 1;
                                    }
                                    break;
                                case "Top_Error":
                                    {
                                        TOPErrorAB = YRT;
                                        TOPErrorABX = 1;
                                    }
                                    break;
                                case "Top_GT_Max_Abnormal":
                                    {
                                        TOPGT_Max_Abnormal = YRT;
                                        TOPGT_Max_AbnormalX = 1;
                                    }
                                    break;
                            }
                        }
                    }

                }
            }
            if (CGOPENX == 1)
            {
                SocicalDefect.Add("- CG Open : " + CGOPEN + "% (" + CGOPEN + "%)");
            }
            else
            {
                SocicalDefect.Add("- CG Open");
            }
            if (MP_OpenX == 1)
            {
                SocicalDefect.Add("- MP Open : " + MP_Open + "% (" + MP_Open + "%)");
            }
            else
            {
                SocicalDefect.Add("- MP Open");
            }
            if (CG_ParticleX == 1)
            {
                SocicalDefect.Add("- CG Particle : " + CG_Particle + "% (" + CG_Particle + "%)");
            }
            else
            {
                SocicalDefect.Add("- CG Particle");
            }
            float GS_MAX = 0;
            float GS_MAX_GB = 0;
            if (TOP_GS_MAXX == 1)
            {
                GS_MAX = float.Parse(TOP_GS_MAX.ToString());
            }
            if (TOP_GS_MAX_GBX == 1)
            {
                GS_MAX_GB = float.Parse(TOP_GS_MAX_GB.ToString());

            }
            float TotalGS_MAX = GS_MAX + GS_MAX_GB;
            if (TotalGS_MAX > 0)
            {
                if (GS_MAX > 0)
                {
                    SocicalDefect.Add("- GS Max : " + TotalGS_MAX + "% (" + GS_MAX + "%)"); //(0.00%)
                }
                else
                {
                    SocicalDefect.Add("- GS Max : " + TotalGS_MAX + "%");
                }

            }
            else
            {
                SocicalDefect.Add("- GS Max");
            }
            //
            float GS_MIN = 0;
            float GS_MIN_GB = 0;
            if (TOP_GS_MINX == 1)
            {
                GS_MIN = float.Parse(TOP_GS_MIN.ToString());
            }
            if (TOP_GS_MIN_GBX == 1)
            {
                GS_MIN_GB = float.Parse(TOP_GS_MIN_GB.ToString());

            }
            float TotalGS_MIN = GS_MIN + GS_MIN_GB;
            if (TotalGS_MIN > 0)
            {
                if (GS_MIN > 0)
                {
                    SocicalDefect.Add("- GS Min : " + TotalGS_MIN + "% (" + GS_MIN + "%)"); //(0.00%)
                }
                else
                {
                    SocicalDefect.Add("- GS Min : " + TotalGS_MIN + "%");
                }

            }
            else
            {
                SocicalDefect.Add("- GS Min");
            }
            //
            float GR_MAX = 0;
            float GR_MAX_GB = 0;
            if (TOP_GR_MAXX == 1)
            {
                GR_MAX = float.Parse(TOP_GR_MAX.ToString());
            }
            if (TOP_GR_MAX_GBX == 1)
            {
                GR_MAX_GB = float.Parse(TOP_GR_MAX_GB.ToString());

            }
            float TotalGR_MAX = GR_MAX + GR_MAX_GB;
            if (TotalGR_MAX > 0)
            {
                if (GR_MAX > 0)
                {
                    SocicalDefect.Add("- GR Max : " + TotalGR_MAX + "% (" + GR_MAX + "%)"); //(0.00%)
                }
                else
                {
                    SocicalDefect.Add("- GR Max : " + TotalGR_MAX + "%");
                }

            }
            else
            {
                SocicalDefect.Add("- GR Max");
            }

            float GR_MIN = 0;
            float GR_MIN_GB = 0;
            if (TOP_GR_MINX == 1)
            {
                GR_MIN = float.Parse(TOP_GR_MIN.ToString());
            }
            if (TOP_GR_MIN_GBX == 1)
            {
                GR_MIN_GB = float.Parse(TOP_GR_MIN_GB.ToString());

            }
            float TotalGR_MIN = GR_MIN + GR_MIN_GB;
            if (TotalGR_MIN > 0)
            {
                if (GR_MIN > 0)
                {
                    SocicalDefect.Add("- GR Min : " + TotalGR_MIN + "% (" + GR_MIN + "%)"); //(0.00%)
                }
                else
                {
                    SocicalDefect.Add("- GR Min : " + TotalGR_MIN + "%");
                }

            }
            else
            {
                SocicalDefect.Add("- GR Min");
            }
            //
            float GT_MAX = 0;
            float GT_MAX_GB = 0;
            if (TOP_GT_MAXX == 1)
            {
                GT_MAX = float.Parse(TOP_GT_MAX.ToString());
            }
            if (TOP_GT_MAX_GBX == 1)
            {
                GT_MAX_GB = float.Parse(TOP_GT_MAX_GB.ToString());

            }
            float TotalGT_MAX = GT_MAX + GT_MAX_GB;
            if (TotalGT_MAX > 0)
            {
                if (GT_MAX > 0)
                {
                    SocicalDefect.Add("- GT Max : " + TotalGT_MAX + "% (" + GT_MAX + "%)"); //(0.00%)
                }
                else
                {
                    SocicalDefect.Add("- GT Max : " + TotalGT_MAX + "%");
                }

            }
            else
            {
                SocicalDefect.Add("- GT Max");
            }

            float GT_MIN = 0;
            float GT_MIN_GB = 0;
            if (TOP_GT_MINX == 1)
            {
                GT_MIN = float.Parse(TOP_GT_MIN.ToString());
            }
            if (TOP_GT_MIN_GBX == 1)
            {
                GT_MIN_GB = float.Parse(TOP_GT_MIN_GB.ToString());

            }
            float TotalGT_MIN = GT_MIN + GT_MIN_GB;
            if (TotalGT_MIN > 0)
            {
                if (GT_MIN > 0)
                {
                    SocicalDefect.Add("- GT Min : " + TotalGT_MIN + "% (" + GT_MIN + "%)"); //(0.00%)
                }
                else
                {
                    SocicalDefect.Add("- GT Min : " + TotalGT_MIN + "%");
                }

            }
            else
            {
                SocicalDefect.Add("- GT Min");
            }
            ///
            float GU_MAX_A = 0;
            float GU_MAX_A_GB = 0;
            if (TOP_GU_Max_AX == 1)
            {
                GU_MAX_A = float.Parse(TOP_GU_Max_A.ToString());
            }
            if (TOP_GU_Max_A_GBX == 1)
            {
                GU_MAX_A_GB = float.Parse(TOP_GU_Max_A_GB.ToString());

            }
            float TotalGU_MAXA = GU_MAX_A + GU_MAX_A_GB;
            if (TotalGU_MAXA > 0)
            {
                if (GU_MAX_A > 0)
                {
                    SocicalDefect.Add("- GU Max A : " + TotalGU_MAXA + "% (" + GU_MAX_A + "%)"); //(0.00%)
                }
                else
                {
                    SocicalDefect.Add("- GU Max A  : " + TotalGU_MAXA + "%");
                }

            }
            else
            {
                SocicalDefect.Add("- GU Max A ");
            }
            float GU_MAX_B = 0;
            float GU_MAX_B_GB = 0;
            if (TOP_GU_Max_BX == 1)
            {
                GU_MAX_B = float.Parse(TOP_GU_Max_B.ToString());
            }
            if (TOP_GU_Max_B_GBX == 1)
            {
                GU_MAX_B_GB = float.Parse(TOP_GU_Max_B_GB.ToString());

            }
            float TotalGU_MAXB = GU_MAX_B + GU_MAX_B_GB;
            if (TotalGU_MAXB > 0)
            {
                if (GU_MAX_B > 0)
                {
                    SocicalDefect.Add("- GU Max B : " + TotalGU_MAXB + "% (" + GU_MAX_B + "%)"); //(0.00%)
                }
                else
                {
                    SocicalDefect.Add("- GU Max B  : " + TotalGU_MAXB + "%");
                }

            }
            else
            {
                SocicalDefect.Add("- GU Max B ");
            }

            ///
            float ErrorAB = 0;
            float GT_Max_Abnomal = 0;
            if (TOPErrorABX == 1)
            {
                ErrorAB = float.Parse(TOPErrorAB.ToString());
            }
            if (TOPGT_Max_AbnormalX == 1)
            {
                GT_Max_Abnomal = float.Parse(TOPGT_Max_Abnormal.ToString());

            }
            float TotalError = ErrorAB + GT_Max_Abnomal;
            if (TotalError > 0)
            {
                if (ErrorAB > 0)
                {
                    SocicalDefect.Add("- TOP Error : " + TotalError + "% (" + ErrorAB + "%)"); //(0.00%)
                }
                else
                {
                    SocicalDefect.Add("- TOP Error  : " + TotalError + "%");
                }

            }
            else
            {
                SocicalDefect.Add("- TOP Error ");
            }


            string XABV = string.Join(Environment.NewLine, SocicalDefect);
            Report_IStb.Text = XABV;
            SocicalDefect.Clear();
        }*/


    }
}
