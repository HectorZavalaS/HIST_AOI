using HIST_AOI.Class;
using HIST_AOI.Models;
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
    public partial class histByDJ : MetroForm
    {
        private COracle m_oracle;
        private CCogiscanCGSDW m_db2_serv11;
        private CCogiscanCGS m_db2_serv2;
        private siixsem_main_dbEntities m_db;
        CMonitor_AOI m_monitor;

        public histByDJ()
        {
            InitializeComponent();
            m_db2_serv11 = new CCogiscanCGSDW();
            m_db2_serv2 = new CCogiscanCGS();
            m_db = new siixsem_main_dbEntities();
            m_monitor = new CMonitor_AOI(ref m_console);
        }

        private void histByDJ_Load(object sender, EventArgs e)
        {
            //var Lines = m_db.getAOILines();
            //cmb_lines.DataSource = Lines;
            //cmb_lines.DisplayMember = "DESCRIPTION_LINE";
            //cmb_lines.ValueMember = "IP_AOI";
            m_console.Text = "";
        }

        private void kryptonButton1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtDjGroup.Text))
            {
                MessageBox.Show("Debes introducir una DJ.");
                txtDjGroup.Focus();
                return;
            }

            m_monitor.Batch_id = txtDjGroup.Text;
            m_monitor.checkAOIDJWAIT();
        }

        private void m_console_TextChanged(object sender, EventArgs e)
        {
            m_console.SelectionStart = m_console.Text.Length;
            m_console.ScrollToCaret();
        }
    }
}
