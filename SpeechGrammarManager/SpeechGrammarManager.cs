//////////////////////////////////////////////////////////////////////////
// Author: FesianXu
// Date: 2017/8/26
// Description: speech grammar manager, for extract the tag word in grammar xml doc
// version: v1.1
// type: class
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace FesianXu.KinectGestureControl
{
    class SpeechGrammarManager
    {
        private string xmlpath = @"../../Resources/SpeechGrammars/SpeechGrammar.xml";
        private XmlDocument docxml = new XmlDocument();
        private XmlNamespaceManager m;
        private string namespace_self = "http://www.w3.org/2001/06/grammar";
        private string header = "index";
        private XmlNodeList nodelist;
        private XmlNode root;
        private List<string> tag_list = new List<string>();

        private string master;
        private string chris;
        private string requestForAuthorization;
        private string logOut;
        private string holdOn;
        private string goOn;
        private string stopTheKinect;
        private string runTheKinect;
        private string guest;

        public SpeechGrammarManager()
        {
            docxml.Load(xmlpath);
            m = new XmlNamespaceManager(docxml.NameTable);
            m.AddNamespace(header, namespace_self);
            root = docxml.DocumentElement;
            nodelist = root.SelectNodes("//" + header + ":" + "tag", m);
            foreach (XmlNode c in nodelist)
            {
                tag_list.Add(c.InnerText);
            }
            binding();
        }

        /// <summary>
        /// binding the visitor to the speech recognition TAG
        /// </summary>
        private void binding()
        {
            master = tag_list[0];
            chris = tag_list[1];
            requestForAuthorization = tag_list[2];
            logOut = tag_list[3];
            holdOn = tag_list[4];
            goOn = tag_list[5];
            stopTheKinect = tag_list[6];
            runTheKinect = tag_list[7];
            guest = tag_list[8];
        }

        public string Master{ get { return master; } }
        public string Chris { get { return chris; } }
        public string RequestForAuthorization { get { return requestForAuthorization; } }
        public string LogOut { get { return logOut; } }
        public string HoldOn { get { return holdOn; } }
        public string Continue { get { return goOn; } }
        public string StopTheKinect { get { return stopTheKinect; } }
        public string RunTheKinect { get { return runTheKinect; } }
        public string Guest { get { return guest; } }

    }
}
