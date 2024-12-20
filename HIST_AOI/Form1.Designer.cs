using System.Windows.Forms;

namespace HIST_AOI
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;


        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.m_console = new System.Windows.Forms.RichTextBox();
            this.reCheck = new System.Windows.Forms.Timer(this.components);
            this.checkAll = new System.Windows.Forms.Timer(this.components);
            this.btn_srAP = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btn_srWP = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.btnCheckDJGrp = new ComponentFactory.Krypton.Toolkit.KryptonButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelTimer = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.timerAllWait = new System.Windows.Forms.Timer(this.components);
            this.timerElapsed = new System.Windows.Forms.Timer(this.components);
            this.timerKoh = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // m_console
            // 
            this.m_console.BackColor = System.Drawing.Color.Black;
            this.m_console.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.m_console.ForeColor = System.Drawing.Color.White;
            this.m_console.Location = new System.Drawing.Point(23, 63);
            this.m_console.Name = "m_console";
            this.m_console.Size = new System.Drawing.Size(865, 474);
            this.m_console.TabIndex = 0;
            this.m_console.Text = "";
            this.m_console.TextChanged += new System.EventHandler(this.m_console_TextChanged);
            // 
            // reCheck
            // 
            this.reCheck.Interval = 120000;
            this.reCheck.Tick += new System.EventHandler(this.reCheck_Tick);
            // 
            // checkAll
            // 
            this.checkAll.Interval = 420000;
            this.checkAll.Tick += new System.EventHandler(this.checkAll_Tick);
            // 
            // btn_srAP
            // 
            this.btn_srAP.Location = new System.Drawing.Point(335, 32);
            this.btn_srAP.Name = "btn_srAP";
            this.btn_srAP.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparklePurple;
            this.btn_srAP.Size = new System.Drawing.Size(161, 25);
            this.btn_srAP.TabIndex = 1;
            this.btn_srAP.Values.Text = "Search Active products";
            this.btn_srAP.Visible = false;
            this.btn_srAP.Click += new System.EventHandler(this.btn_srAP_Click);
            // 
            // btn_srWP
            // 
            this.btn_srWP.Location = new System.Drawing.Point(518, 32);
            this.btn_srWP.Name = "btn_srWP";
            this.btn_srWP.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparklePurple;
            this.btn_srWP.Size = new System.Drawing.Size(162, 25);
            this.btn_srWP.TabIndex = 2;
            this.btn_srWP.Values.Text = "Search Waiting products";
            this.btn_srWP.Visible = false;
            this.btn_srWP.Click += new System.EventHandler(this.btn_srWP_Click);
            // 
            // btnCheckDJGrp
            // 
            this.btnCheckDJGrp.Location = new System.Drawing.Point(702, 32);
            this.btnCheckDJGrp.Name = "btnCheckDJGrp";
            this.btnCheckDJGrp.PaletteMode = ComponentFactory.Krypton.Toolkit.PaletteMode.SparklePurple;
            this.btnCheckDJGrp.Size = new System.Drawing.Size(153, 25);
            this.btnCheckDJGrp.TabIndex = 3;
            this.btnCheckDJGrp.Values.Text = "Analize DJ Group";
            this.btnCheckDJGrp.Visible = false;
            this.btnCheckDJGrp.Click += new System.EventHandler(this.btnCheckDJGrp_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelTimer,
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 555);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip1.Size = new System.Drawing.Size(911, 22);
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labelTimer
            // 
            this.labelTimer.Name = "labelTimer";
            this.labelTimer.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStripStatusLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(140, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Visible = false;
            // 
            // timerAllWait
            // 
            this.timerAllWait.Interval = 1000;
            this.timerAllWait.Tick += new System.EventHandler(this.timerAllWait_Tick);
            // 
            // timerElapsed
            // 
            this.timerElapsed.Interval = 1000;
            this.timerElapsed.Tick += new System.EventHandler(this.timerElapsed_Tick);
            // 
            // timerKoh
            // 
            this.timerKoh.Interval = 30000;
            this.timerKoh.Tick += new System.EventHandler(this.timerKoh_Tick);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.BackImage = global::HIST_AOI.Properties.Resources.circuit;
            this.BackMaxSize = 920;
            this.ClientSize = new System.Drawing.Size(911, 577);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnCheckDJGrp);
            this.Controls.Add(this.btn_srWP);
            this.Controls.Add(this.btn_srAP);
            this.Controls.Add(this.m_console);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.Padding = new System.Windows.Forms.Padding(0, 60, 0, 0);
            this.Resizable = false;
            this.ShadowType = MetroFramework.Forms.MetroFormShadowType.AeroShadow;
            this.Text = "AUTOMATIC AOI";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RichTextBox m_console;
        private System.Windows.Forms.Timer reCheck;
        private System.Windows.Forms.Timer checkAll;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btn_srAP;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btn_srWP;
        private ComponentFactory.Krypton.Toolkit.KryptonButton btnCheckDJGrp;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel labelTimer;
        private Timer timerAllWait;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private Timer timerElapsed;
        private Timer timerKoh;
    }
}

