namespace TardyTerm
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
            this.btn_OpenJigPort = new System.Windows.Forms.Button();
            this.cbCOMPorts = new System.Windows.Forms.ComboBox();
            this.btn_Log = new System.Windows.Forms.Button();
            this.txtBx_LogFileName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_HR = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btn_OpenJigPort
            // 
            this.btn_OpenJigPort.Location = new System.Drawing.Point(143, 12);
            this.btn_OpenJigPort.Name = "btn_OpenJigPort";
            this.btn_OpenJigPort.Size = new System.Drawing.Size(97, 24);
            this.btn_OpenJigPort.TabIndex = 7;
            this.btn_OpenJigPort.Text = "Connect!";
            this.btn_OpenJigPort.UseVisualStyleBackColor = true;
            this.btn_OpenJigPort.Click += new System.EventHandler(this.btn_OpenJigPort_Click);
            // 
            // cbCOMPorts
            // 
            this.cbCOMPorts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.cbCOMPorts.FormattingEnabled = true;
            this.cbCOMPorts.Location = new System.Drawing.Point(12, 12);
            this.cbCOMPorts.Name = "cbCOMPorts";
            this.cbCOMPorts.Size = new System.Drawing.Size(121, 21);
            this.cbCOMPorts.TabIndex = 6;
            // 
            // btn_Log
            // 
            this.btn_Log.Location = new System.Drawing.Point(260, 6);
            this.btn_Log.Name = "btn_Log";
            this.btn_Log.Size = new System.Drawing.Size(75, 37);
            this.btn_Log.TabIndex = 8;
            this.btn_Log.Text = "Start Logging";
            this.btn_Log.UseVisualStyleBackColor = true;
            this.btn_Log.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtBx_LogFileName
            // 
            this.txtBx_LogFileName.Location = new System.Drawing.Point(12, 55);
            this.txtBx_LogFileName.Name = "txtBx_LogFileName";
            this.txtBx_LogFileName.Size = new System.Drawing.Size(323, 20);
            this.txtBx_LogFileName.TabIndex = 9;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Log File Name:";
            // 
            // lbl_HR
            // 
            this.lbl_HR.AutoSize = true;
            this.lbl_HR.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_HR.Location = new System.Drawing.Point(13, 82);
            this.lbl_HR.Name = "lbl_HR";
            this.lbl_HR.Size = new System.Drawing.Size(51, 20);
            this.lbl_HR.TabIndex = 11;
            this.lbl_HR.Text = "HR: -";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(354, 129);
            this.Controls.Add(this.lbl_HR);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtBx_LogFileName);
            this.Controls.Add(this.btn_Log);
            this.Controls.Add(this.btn_OpenJigPort);
            this.Controls.Add(this.cbCOMPorts);
            this.Name = "Form1";
            this.Text = "TardyTerm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Form1_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_OpenJigPort;
        private System.Windows.Forms.ComboBox cbCOMPorts;
        private System.Windows.Forms.Button btn_Log;
        private System.Windows.Forms.TextBox txtBx_LogFileName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_HR;
    }
}

