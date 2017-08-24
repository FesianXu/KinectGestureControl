//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/24
// Description: The DrivingControl, a implement of Control interface
// version: v1.1
// type: class
// inherit from: DrivingControlStrategy
// implement of: Control
//////////////////////////////////////////////////////////////////////////


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
    * the frame format of the protocol
    * <b>1:val,2:val,3:val,4:val,5:val,6:val,7:val,8:val,9:val,[check bits]<e>
    * allow the miss of some channels, the value will be the last changed value
    * before changing
    * if it does not have the default value, it will be set to the middle value between
    * min_channel_val and min_channel_val
    * the check bits cannot be missing, the check bits is using the Exclusive Or(xor)
    * to check the frame, i.e. xor all the channels' value and check if it equal to
    * the check bits.
    */

    class DrivingControl :DrivingControlStrategy, Control
    {
        private int checkbits;
        private ppmSignalStructure ppm;
        private Communicater comm;
        private const int LEN_PPM = 9;
        private string frameBeginPattern = "<b>";
        private string frameEndPattern = "<e>";
        private string rollBeginPattern = "1:";
        private string pitchBeginPattern = "2:";
        private string throttlBeginPattern = "3:";
        private string yawBeginPattern = "4:";
        private string radio5BeginPattern = "5:";
        private string radio6BeginPattern = "6:";
        private string radio7BeginPattern = "7:";
        private string radio8BeginPattern = "8:";

        private readonly double proportion = 50/9 ;


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

        private void calculateCheckBits()
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

        private void initPPM()
        {
            ppm.ElEV = -1;
            ppm.AILE = -1;
            ppm.THRO = -1;
            ppm.RUDO = -1;
            ppm.GEAR = -1;
            ppm.AUX1 = -1;
            ppm.AUX2 = -1;
            ppm.AUX3 = -1;
            ppm.AUX4 = -1;
        }


        public void driveYaw(double angle)
        {
            initPPM();
            ppm.RUDO = (int)(angle * proportion + 1500);
            ppm.THRO = 2000;
            calculateCheckBits();
            string check = string.Format(",[{0}]", checkbits);
            string cmd = 
                frameBeginPattern + 
                yawBeginPattern + ppm.RUDO.ToString()+","+
                throttlBeginPattern+ppm.THRO.ToString()+
                check+
                frameEndPattern;
            comm.writeLines(cmd);
        }

        

    }
}
