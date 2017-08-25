//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/25
// Description: The System Control Manager, for the management of the 
// version: v1.1
// type: class
// inherit from: None
// implement of: VoiceAssistant
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FesianXu.KinectGestureControl
{
    enum KinectRunBeginWayEnum { VA_Init, Button_Init, NoRunningNow};

    class SystemControlManager: SystemControlManagers
    {
        
        public SystemControlManager()
        {
            KinectRunningBeginWay = KinectRunBeginWayEnum.NoRunningNow;
        }


        protected void execute()
        {
        }


        public KinectRunBeginWayEnum KinectRunningBeginWay { get; set; }
    }
}
