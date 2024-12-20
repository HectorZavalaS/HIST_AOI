using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIST_AOI.Class
{
    class CLine
    {
        private String m_name;
        private CThread m_events;
        String m_server;
        String m_SID;
        public string Name { get => m_name; set => m_name = value; }
        internal CThread Events { get => m_events; set => m_events = value; }
        public string Server { get => m_server; set => m_server = value; }
        public string SID { get => m_SID; set => m_SID = value; }

        public CLine(ref RichTextBox console, String nameLine, ref ListView lines, String server, String SID)
        {
            m_name = nameLine;
            m_server = server;
            m_SID = SID;
            Events = new CThread(ref console, m_name, ref lines, server, SID);
        }
    }
}
