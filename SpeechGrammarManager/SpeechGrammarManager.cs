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
        }

        private void distribute()
        {

        }

        public string Master{ get; }
        public string Chris { get; }
        public string RequestForAuthorization { get; }
        public string LogOut { get; }
        public string HoldOn { get; }
        public string Stop { get; }
        public string Continue { get; }


    }
}
