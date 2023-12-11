using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auto_Cls_Data
{
    public class AlgorithmDLL
    {
        public Databasex[] Databasex { get; set; }
        public Dataalgorithm[] DataAlgorithm { get; set; }
    }
    public class Databasex
    {
        public string Machine { get; set; }
        public string Database { get; set; }
    }
    public class Dataalgorithm
    {
        public string Machine { get; set; }
        public Dataorg[] DataORG { get; set; }
        public string[] Judge { get; set; }
        public string[] Defect { get; set; }
    }
    public class Dataorg
    {
        public string Name { get; set; }
        public string IP { get; set; }
    }
}
