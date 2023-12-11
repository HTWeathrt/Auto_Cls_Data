using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Cls_Data.Gplus
{
    public class VH009213
    {
        // CGAOI Cl
        public int Total,ttOK,ttNG,ttNA;
        public void CaculatorCGAOI(DataTable basedataA,DataTable basedataB , float seq1, float seq2, float seq3,string Linexab)
        {
            // Tinh Toan Unit A
            int countOKA=0, countNGA=0, countNAA=0;
            int NGA1 = 0;
            int NGA2 = 0;
            int TotalRowA = basedataA.Rows.Count;
            foreach (DataRow row in basedataA.Rows)
            {
                //string eqpname = row["eqp_name"].ToString();
                string pid = row["pid"].ToString();
                string rox = row["judge"].ToString();
                if (rox == "NG")
                {
                    countNGA++;
                    if (pid != null)
                    {
                        string valueA = pid[0].ToString();
                        if (valueA == "1")
                        {
                            NGA1++;
                        }
                        if (valueA == "2")
                        {
                            NGA2++;
                        }
                    }
                }
                else if (rox == "OK")
                {
                    countOKA++;
                }
                else if (rox != "NG" && rox != "OK")
                {
                    countNAA++;
                }
            };
            caculatorB(basedataB, TotalRowA, countNGA, countOKA, countNAA,NGA1,NGA2,  seq1,  seq2,  seq3,Linexab);
        }
        private void caculatorB(DataTable basedataB, int TotalrowA,int ctNGA,int ctOKA,int ctNAA,int NGA1,int NGA2 ,float seq1, float seq2, float seq3, string numberlinexab)
        {
            int  countOKB=0, countNGB =0, countNAB = 0;
            int TotalRowB = basedataB.Rows.Count;
            int NGB1 =0;
            int NGB2 = 0;
            foreach (DataRow row in basedataB.Rows)
            {  
                string pid = row["pid"].ToString();
                string rox = row["judge"].ToString();
                if (rox == "NG")
                {
                    countNGB++;
                    if (pid != null)
                    {
                        string valueB = pid[0].ToString();
                        if (valueB == "1")
                        {
                            NGB1++;
                        }
                        if (valueB == "2")
                        {
                            NGB2++;
                        }
                    }
                }
                else if (rox == "OK")
                {
                    countOKB++;
                }
                else if (rox != "NG" && rox != "OK")
                {
                    countNAB++;
                }
            };
            
            ttOK = ctOKA + countOKB;
            ttNG = ctNGA + countNGB;
            ttNA = ctNAA + countNAB;
            Total = TotalRowB + TotalrowA;


            float AA_A = float.Parse(TotalrowA.ToString());
            float A_NG = float.Parse(ctNGA.ToString());
            float BB_B = float.Parse(TotalRowB.ToString());
            float B_NG = float.Parse(countNGB.ToString());
            //
            float TotalALL_AB = float.Parse(Total.ToString());
            float NGALLAB = float.Parse(ttNG.ToString());
            //
            float NGA1_X = float.Parse(NGA1.ToString());
            float NGA2_X = float.Parse(NGA2.ToString());
            float NGB1_X = float.Parse(NGB1.ToString());
            float NGB2_X = float.Parse(NGB2.ToString());
            // YRT Table
            float YRTTBA1 = (NGA1_X / A_NG) * 100;
            float YRTTBA2 = (NGA2_X / A_NG) * 100;
            float YRTTBB1 = (NGB1_X / B_NG) * 100;
            float YRTTBB2 = (NGB2_X / B_NG) * 100;

            float YRTNA_AB = (ttNA / TotalALL_AB) * 100;
            float YRTOK_AB = (ttOK / TotalALL_AB) * 100;
            float YRTNG_AB = (NGALLAB / TotalALL_AB) * 100;

            float YRTNG_A = (A_NG / AA_A) * 100;
            float YRTNG_B = (B_NG / BB_B) * 100;
            string YRT = $" ➤YRT \n⇢ Total: { Total} ea\n⇢ OK:{ ttOK} ea ↔ { YRTOK_AB}%\n⇢ NG:{ ttNG} ea ↔ { YRTNG_AB}%\n⇢ NA:{ ttNA} ea ↔{ YRTNA_AB}% ";
            string A_B =   $"➤YRT \n⇢ Total NG A+B: {ttNG} ea\n⇢ NG_A: {ctNGA}ea ↔ { YRTNG_A}%\n⇢ NG_B: {countNGB}ea ↔ {YRTNG_B}%";
            string A1_A2 = $"➤YRT \n⇢ Total_NG_A:{TotalrowA}ea\nNG_A1: {NGA1}ea ↔ {YRTTBA1}%\nNG_A2: {NGA2}ea ↔ {YRTTBA2}%";
            string B1_B2 = $"➤YRT \n⇢ Total_NG_B:{TotalRowB}ea\nNG_B1: {NGB1}ea ↔ {YRTTBB1}%\nNG_B2: {NGB2}ea ↔ {YRTTBB2}%";

            ///
            if (YRTNG_AB > seq1 && NGALLAB > 3)
            {
                /// Xử lý data rồi xuất ra 
                ErrorADD.Add("\nLine: " + numberlinexab + "\nSpec Defect Rate Line Over ");
                ErrorADD.Add(YRT);
            }
            else
            {
                if (YRTNG_A > YRTNG_B + seq2 && NGALLAB > 3 || YRTNG_B > YRTNG_A + seq2 && NGALLAB > 3)
                {
                    ErrorADD.Add("\n•Line: " + numberlinexab + "\nSpec Lane A & B Over ");
                    ErrorADD.Add(A_B);
                }
                else
                {
                    if (YRTTBA1 > YRTTBA2 + seq3 && A_NG > 3 || YRTTBA2 > YRTTBA1 + seq3 && A_NG > 3)
                    {
                        ErrorADD.Add("\nLine: " + numberlinexab + "\nSpec Stage A1 & A2 Over ");
                        ErrorADD.Add(A1_A2);
                    }
                    else if (YRTTBB1 > YRTTBB2 + seq3 && B_NG > 3 || YRTTBB2 > YRTTBB1 + seq3 && B_NG > 3)
                    {

                        ErrorADD.Add("\nLine: " + numberlinexab + "\nSpec Stage B1 & B2 Over ");
                        ErrorADD.Add(B1_B2);
                    }
                }
            }
            XABV = string.Join(Environment.NewLine, ErrorADD);
            ////
        }
        public List<string> ErrorADD = new List<string>();
        public string XABV;
    }
}




namespace Auto_Cls_Data
{
    public class ADBData
    {
        public int totalALL, totalOK, totalNA, totalNG;
        public void MNT_CP_IS(DataTable table, string Machine, string numberlinexab, float seq1, float seq2, float seq3)
        {

            // Tổng số của Cả bảng;
            totalALL = table.Rows.Count;
            totalOK = 0;
            totalNG = 0;
            totalNA = 0;
            // Tổng số của bảng A
            int totalA_A = 0;
            int totalOK_A = 0;
            int totalNG_A = 0;
            int totalNA_A = 0;
            // Tổng số của bảng B
            int totalB_B = 0;
            int totalOK_B = 0;
            int totalNG_B = 0;
            int totalNA_B = 0;
            // Tổng số NG của Bàn 1 và 2
            int NGA1 = 0;
            int NGA2 = 0;
            int NGB1 = 0;
            int NGB2 = 0;
            // Tổng số Total của 2 bàn 
            int A1TT = 0;
            int A2TT = 0;
            int B1TT = 0;
            int B2TT = 0;
            switch (Machine)
            {
                case "CP_AOI":
                    foreach (DataRow row in table.Rows)
                    {
                        string rowpid = "";
                        rowpid = row["localid"].ToString();
                        /// total
                        if (row["localid"].ToString() != null)
                        {
                            string[] splitValues = rowpid.Split('_');
                            string valueA = splitValues[1].Substring(0, 1);
                            string value12 = splitValues[2].Substring(0, 1);
                            if (valueA == "A")
                            {
                                totalA_A++;
                                if (value12 == "1")
                                {
                                    A1TT++;
                                }
                                if (value12 == "2")
                                {
                                    A2TT++;
                                }
                            }
                            if (valueA == "B")
                            {
                                totalB_B++;
                                if (value12 == "1")
                                {
                                    B1TT++;
                                }
                                if (value12 == "2")
                                {
                                    B2TT++;
                                }
                            }
                        }
                        if (row["judge"].ToString() == "N")
                        {
                            if (row["localid"].ToString() != null)
                            {
                                string[] splitValues = rowpid.Split('_');
                                string valueA = splitValues[1].Substring(0, 1);
                                string value12 = splitValues[2].Substring(0, 1);
                                if (valueA == "A")
                                {
                                    totalNG_A++;
                                    if (value12 == "1")
                                    {
                                        NGA1++;
                                    }
                                    if (value12 == "2")
                                    {
                                        NGA2++;
                                    }
                                }
                                if (valueA == "B")
                                {
                                    totalNG_B++;
                                    if (value12 == "1")
                                    {
                                        NGB1++;
                                    }
                                    if (value12 == "2")
                                    {
                                        NGB2++;
                                    }
                                }
                            }
                            totalNG++;
                        }
                        if (row["judge"].ToString() == "G")
                        {
                            if (row["localid"].ToString() != null)
                            {
                                string[] splitValues = rowpid.Split('_');
                                string valueA = splitValues[1].Substring(0, 1);
                                if (valueA == "A")
                                {
                                    totalOK_A++;
                                }
                                if (valueA == "B")
                                {
                                    totalOK_B++;
                                }
                            }
                            totalOK++;
                        }
                        if (row["judge"].ToString() != "N" && row["judge"].ToString() != "G")
                        {
                            if (row["localid"].ToString() != null)
                            {
                                string[] splitValues = rowpid.Split('_');
                                string valueA = splitValues[1].Substring(0, 1);
                                if (valueA == "A")
                                {
                                    totalNA_A++;
                                }
                                if (valueA == "B")
                                {
                                    totalNA_B++;
                                }

                            }
                            totalNA++;
                        }
                    };
                    break;
                case "IS_AOI":
                    foreach (DataRow row in table.Rows)
                    {
                        string rowpid = "";
                        rowpid = row["inspection_zone"].ToString();
                        string valueA = rowpid[0].ToString();
                        string value12 = rowpid[1].ToString();

                        if (row["localid"].ToString() != null)
                        {
                            if (valueA == "A")
                            {
                                totalA_A++;
                                if (value12 == "1")
                                {
                                    A1TT++;
                                }
                                if (value12 == "2")
                                {
                                    A2TT++;
                                }
                            }
                            if (valueA == "B")
                            {
                                totalB_B++;
                                if (value12 == "1")
                                {
                                    B1TT++;
                                }
                                if (value12 == "2")
                                {
                                    B2TT++;
                                }
                            }
                        }
                        if (row["judge"].ToString() == "NG")
                        {
                            if (row["localid"].ToString() != null)
                            {
                                if (valueA == "A")
                                {
                                    totalNG_A++;
                                    if (value12 == "1")
                                    {
                                        NGA1++;
                                    }
                                    if (value12 == "2")
                                    {
                                        NGA2++;
                                    }
                                }
                                if (valueA == "B")
                                {
                                    totalNG_B++;
                                    if (value12 == "1")
                                    {
                                        NGB1++;
                                    }
                                    if (value12 == "2")
                                    {
                                        NGB2++;
                                    }
                                }
                            }
                            totalNG++;
                        }
                        if (row["judge"].ToString() == "OK")
                        {
                            if (row["localid"].ToString() != null)
                            {

                                if (valueA == "A")
                                {
                                    totalOK_A++;
                                }
                                if (valueA == "B")
                                {
                                    totalOK_B++;
                                }
                            }
                            totalOK++;
                        }
                        if (row["judge"].ToString() != "NG" && row["judge"].ToString() != "OK")
                        {
                            if (row["localid"].ToString() != null)
                            {

                                if (valueA == "A")
                                {
                                    totalNA_A++;
                                }
                                if (valueA == "B")
                                {
                                    totalNA_B++;
                                }

                            }
                            totalNA++;
                        }
                    };
                    break;
            }
            float AA_A = float.Parse(totalA_A.ToString());
            float A_NG = float.Parse(totalNG_A.ToString());
            float BB_B = float.Parse(totalB_B.ToString());
            float B_NG = float.Parse(totalNG_B.ToString());
            //
            float TotalALL_AB = float.Parse(totalALL.ToString());
            float NGALLAB = float.Parse(totalNG.ToString());
            //
            float NGA1_X = float.Parse(NGA1.ToString());
            float NGA2_X = float.Parse(NGA2.ToString());
            float NGB1_X = float.Parse(NGB1.ToString());
            float NGB2_X = float.Parse(NGB2.ToString());
            // YRT Table
            float YRTTBA1 = (NGA1_X / A_NG) * 100;
            float YRTTBA2 = (NGA2_X / A_NG) * 100;
            float YRTTBB1 = (NGB1_X / B_NG) * 100;
            float YRTTBB2 = (NGB2_X / B_NG) * 100;

            float YRTNA_AB = (totalNA / TotalALL_AB) * 100;
            float YRTOK_AB = (totalOK / TotalALL_AB) * 100;
            float YRTNG_AB = (NGALLAB / TotalALL_AB) * 100;

            float YRTNG_A = (A_NG / AA_A) * 100;
            float YRTNG_B = (B_NG / BB_B) * 100;
            string YRT = $" ➤YRT \n⇢ Total: " + totalALL + " ea\n⇢ OK: " + totalOK + " ea ↔ " + YRTOK_AB + "%\n⇢ NG: " + totalNG + " ea ↔ " + YRTNG_AB + "%\n⇢ NA: " + totalNA + " ea ↔ " + YRTNA_AB + "% ";
            string A_B = $"⟹ Lane A\n⇢ Total: " + totalA_A + " ea\n⇢ NG: " + totalNG_A + " ea ↔ " + YRTNG_A + "%\n⟹ Lane B: \n⇢ Total: " + totalB_B + " ea\n⇢ NG: " + totalNG_B + " ea ↔ " + YRTNG_B + "% ";
            string A1_A2 = $"➤YRT \n⇢ Total_NG_A:{totalNG_A}ea\nNG_A1: {NGA1}ea ↔ {YRTTBA1}%\nNG_A2: {NGA2}ea ↔ {YRTTBA2}%";
            string B1_B2 = $"➤YRT \n⇢ Total_NG_B:{totalNG_B}ea\nNG_A1: {NGB1}ea ↔ {YRTTBB1}%\nNG_A2: {NGB2}ea ↔ {YRTTBB2}%";
           
            ///
            if (YRTNG_AB > seq1 && NGALLAB > 3)
            {
                /// Xử lý data rồi xuất ra 
                ErrorADD.Add("\nLine: " + numberlinexab + "\nSpec Defect Rate Line Over ");
                ErrorADD.Add(YRT);
            }
            else
            {
                if (YRTNG_A > YRTNG_B + seq2 && NGALLAB > 3 || YRTNG_B > +seq2 && NGALLAB > 3)
                {
                    ErrorADD.Add("\n•Line: " + numberlinexab + "\nSpec Lane A & B Over ");
                    ErrorADD.Add(A_B);
                }
                else
                {
                    if (YRTTBA1 > YRTTBA2 + seq3 && A_NG > 3 || YRTTBA2 > YRTTBA1 + seq3 && A_NG > 3)
                    {
                        ErrorADD.Add("\nLine: " + numberlinexab + "\nSpec Stage A1 & A2 Over ");
                        ErrorADD.Add(A1_A2);
                    }
                    else if (YRTTBB1 > YRTTBB2 + seq3 && B_NG > 3 || YRTTBB2 > YRTTBB1 + seq3 && B_NG > 3)
                    {

                        ErrorADD.Add("\nLine: " + numberlinexab + "\nSpec Stage B1 & B2 Over ");
                        ErrorADD.Add(B1_B2);
                    }

                }



            }
             XABV = string.Join(Environment.NewLine, ErrorADD);
            ////
        }
        public string XABV;
        public  List<string> ErrorADD = new List<string>();



    }
}
