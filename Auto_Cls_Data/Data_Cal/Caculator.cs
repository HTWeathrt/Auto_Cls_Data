using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

namespace Auto_Cls_Data.Data_Cal
{
    internal class Caculator
    {
        
        public Caculator() 
        {
            
        }
        public void GitDataoption()
        {
        }
        public void Stop()
        {
            timer.Stop();
        }
        private static Timer timer;

        public void Hehe(int KMD)
        {
            int DataTimer = KMD;
            int XYAB = DataTimer * 100;

            // Tạo một đối tượng Timer với khoảng thời gian hẹn giờ là 5 giây (5000 milliseconds)
            timer = new Timer(XYAB);

            // Đăng ký sự kiện Elapsed để xử lý khi thời gian hẹn giờ đã qua
            timer.Elapsed += OnTimerElapsed;

            // Bắt đầu hẹn giờ
            timer.Start();
        }

        private static void OnTimerElapsed(object sender, ElapsedEventArgs e)
        {
            Data_LD data_LD = new Data_LD();
            data_LD.Stepbystop();

        }

        
    

}
}
