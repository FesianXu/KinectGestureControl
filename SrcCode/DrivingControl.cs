using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FesianXu.KinectGestureControl
{
    // valid value is between min_channel_val and max_channel_val
    // which is 1000 and 2000 respectively.And the invalid value is set
    // to -1, which is not used to calculate the check bits
    public struct ppmSignalStructure
    {
        public int ElEV; // ROLL         -->No.1
        public int AILE; // PITCH        -->No.2
        public int THRO; // THROTTL      -->No.3
        public int RUDO; // YAW          -->No.4
        public int GEAR; // RADIO5       -->No.5
        public int AUX1; // RADIO6       -->No.6
        public int AUX2; // RADIO7       -->No.7
        public int AUX3; // RADIO8       -->No.8
        public int AUX4; // RADIO9       -->No.9
    };

    /*
    * 协议帧格式
    * <b>1:val,2:val,3:val,4:val,5:val,6:val,7:val,8:val,9:val,[校验位]<e>
    * 允许缺失某个通道，其值为以前改变过后的最后一个值。
    * 如果没有默认值则改为中值
    * 校验位不可缺，此处校验采用的是异或校验，将所有的val异或比较
    */

    class DrivingControl :Control
    {
        private int checkbits;
        private ppmSignalStructure ppm;
        private Communicater comm;
        private const int LEN_PPM = 9;

        public DrivingControl(ref Communicater rc)
        {
            comm = rc;
        }

        public DrivingControl()
        {
        }

        public void updateCommunicater(ref Communicater rc)
        {
            comm = rc;
        }

        public void updatePPM(ref ppmSignalStructure new_ppm)
        {
            ppm = new_ppm;
        }

        public void calculateCheckBits()
        {
            List<int> validlist = new List<int>();
            if (ppm.ElEV != -1)
                validlist.Add(ppm.ElEV);
            if (ppm.AILE != -1)
                validlist.Add(ppm.AILE);
            if (ppm.THRO != -1)
                validlist.Add(ppm.THRO);
            if (ppm.RUDO != -1)
                validlist.Add(ppm.RUDO);
            if (ppm.GEAR != -1)
                validlist.Add(ppm.GEAR);
            if (ppm.AUX1 != -1)
                validlist.Add(ppm.AUX1);
            if (ppm.AUX2 != -1)
                validlist.Add(ppm.AUX2);
            if (ppm.AUX3 != -1)
                validlist.Add(ppm.AUX3);
            if (ppm.AUX4 != -1)
                validlist.Add(ppm.AUX4);
            int re_check_val = 0;
            if (validlist != null)
            {
                re_check_val = validlist[0];
            }
            if (validlist.Count >= 2)
            {
                for (int ind = 1; ind < validlist.Count; ind++)
                {
                    re_check_val ^= validlist[ind];
                }
            }
            checkbits = re_check_val;
        }

      

    }
}
