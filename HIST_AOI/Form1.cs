using HIST_AOI.Class;
using MetroFramework.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HIST_AOI
{
    public partial class Form1 : MetroForm
    {
        CMonitor_AOI m_monitor;
        bool checking;
        bool checkingAllAOI;
        double count = 0;
        private const int CP_NOCLOSE_BUTTON = 0x200;
        int seconds = 420;
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams myCp = base.CreateParams;
                myCp.ClassStyle = myCp.ClassStyle | CP_NOCLOSE_BUTTON;
                return myCp;
            }
        }
        public Form1()
        {
            InitializeComponent();
            m_monitor = new CMonitor_AOI(ref m_console /*,ref listLines*/);
            checking = false;
            checkingAllAOI = false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //reCheck.Start(); 
            //m_monitor.checkWaiting();
            //timerAllWait.Start();
            //timerElapsed.Start();
            
            //m_monitor.start();
            //checkAll.Start();
            //m_monitor.checkAOIWAIT();
            timerKoh.Start();

            m_monitor.checkAOIWAITKoh();

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {

            //m_monitor.stop();
            //reCheck.Stop();
            //checkAll.Stop();
            timerKoh.Stop();
            return;
        }

        private void m_console_TextChanged(object sender, EventArgs e)
        {
            m_console.SelectionStart = m_console.Text.Length;
            m_console.ScrollToCaret();
        }

        private void reCheck_Tick(object sender, EventArgs e)
        {
            if (!checking)
            {
                
                checking = true;
                m_monitor.checkWaiting();
                m_monitor.checkActive();
                checking = false;
            }
        }

        private void checkAll_Tick(object sender, EventArgs e)
        {
            try
            {
                if (!checkingAllAOI)
                {
                    checkingAllAOI = true;
                    m_monitor.checkAOIWAIT();
                    //m_monitor.checkAllActive();
                    checkingAllAOI = false;
                }

                toolStripStatusLabel1.Text = "00:00" + " minutes to automatic process.";
            }
            catch(Exception ex) { }
        }

        private void btn_srAP_Click(object sender, EventArgs e)
        {
            //m_monitor.checkAllActive();
        }

        private void btn_srWP_Click(object sender, EventArgs e)
        {
            try
            {
                m_monitor.checkAOIWAIT();
            }
            catch(Exception ex) { }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void btnCheckDJGrp_Click(object sender, EventArgs e)
        {
            histByDJ his = new histByDJ();

            his.ShowDialog();
        }

        private void timerAllWait_Tick(object sender, EventArgs e)
        {
            count++;
            labelTimer.Text = (count/60).ToString() + " minutes elpased";
        }

        private void timerElapsed_Tick(object sender, EventArgs e)
        {
            seconds--;
            if (seconds == 0) seconds = 420;
            Int32 horas = (seconds / 3600);
            Int32 minutos = ((seconds - horas * 3600) / 60);
            Int32 segundos = seconds - (horas * 3600 + minutos * 60);

            toolStripStatusLabel1.Text = string.Format("{0:00}", minutos)  + ":" + string.Format("{0:00}", segundos) + " minutes to automatic process.";
            
        }

        private void timerKoh_Tick(object sender, EventArgs e)
        {
            m_monitor.checkAOIWAITKoh();
        }
    }
}
