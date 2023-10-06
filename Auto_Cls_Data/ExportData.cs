using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Cls_Data
{
    public class ExportData
    {
        private int ng;
        public int Ng { get => ng; set => ng = value; }
        public int Sum { get => sum; set => sum = value; }
        public string Nnn { get => nnn; set => nnn = value; }

        private int sum;
        private string nnn;
        public ExportData(int ng) 
        { 
            this.Ng = ng;
        }
    }
}
