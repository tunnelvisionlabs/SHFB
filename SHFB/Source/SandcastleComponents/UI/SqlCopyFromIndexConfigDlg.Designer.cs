﻿namespace SandcastleBuilder.Components.UI
{
    partial class SqlCopyFromIndexConfigDlg
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
            if(disposing && (components != null))
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
            this.btnSetConnectionString = new System.Windows.Forms.Button();
            this.txtConnectionString = new System.Windows.Forms.TextBox();
            this.lnkCodePlexSHFB = new System.Windows.Forms.LinkLabel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.epErrors = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnPurge = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkEnableLocalCache = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.udcLocalCacheSize = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.udcInMemoryCacheSize = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.epErrors)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udcLocalCacheSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcInMemoryCacheSize)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSetConnectionString
            // 
            this.btnSetConnectionString.Location = new System.Drawing.Point(12, 95);
            this.btnSetConnectionString.Name = "btnSetConnectionString";
            this.btnSetConnectionString.Size = new System.Drawing.Size(88, 32);
            this.btnSetConnectionString.TabIndex = 1;
            this.btnSetConnectionString.Text = "&Setup";
            this.toolTip1.SetToolTip(this.btnSetConnectionString, "Set the connection string and configure the database");
            this.btnSetConnectionString.UseVisualStyleBackColor = true;
            this.btnSetConnectionString.Click += new System.EventHandler(this.btnSetConnectionString_Click);
            // 
            // txtConnectionString
            // 
            this.txtConnectionString.Location = new System.Drawing.Point(12, 24);
            this.txtConnectionString.Multiline = true;
            this.txtConnectionString.Name = "txtConnectionString";
            this.txtConnectionString.ReadOnly = true;
            this.txtConnectionString.Size = new System.Drawing.Size(612, 65);
            this.txtConnectionString.TabIndex = 0;
            this.txtConnectionString.Enter += new System.EventHandler(this.txtConnectionString_Enter);
            // 
            // lnkCodePlexSHFB
            // 
            this.lnkCodePlexSHFB.Location = new System.Drawing.Point(282, 269);
            this.lnkCodePlexSHFB.Name = "lnkCodePlexSHFB";
            this.lnkCodePlexSHFB.Size = new System.Drawing.Size(218, 23);
            this.lnkCodePlexSHFB.TabIndex = 9;
            this.lnkCodePlexSHFB.TabStop = true;
            this.lnkCodePlexSHFB.Text = "Sandcastle Help File Builder";
            this.lnkCodePlexSHFB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lnkCodePlexSHFB, "http://SHFB.CodePlex.com");
            this.lnkCodePlexSHFB.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCodePlexSHFB_LinkClicked);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(576, 264);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 32);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.btnCancel, "Cancel without saving changes");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 264);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 32);
            this.btnOK.TabIndex = 7;
            this.btnOK.Text = "OK";
            this.toolTip1.SetToolTip(this.btnOK, "Save settings");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // epErrors
            // 
            this.epErrors.ContainerControl = this;
            // 
            // btnPurge
            // 
            this.btnPurge.Location = new System.Drawing.Point(106, 264);
            this.btnPurge.Name = "btnPurge";
            this.btnPurge.Size = new System.Drawing.Size(88, 32);
            this.btnPurge.TabIndex = 8;
            this.btnPurge.Text = "Purge";
            this.toolTip1.SetToolTip(this.btnPurge, "Purge the content ID and target caches");
            this.btnPurge.UseVisualStyleBackColor = true;
            this.btnPurge.Click += new System.EventHandler(this.btnPurge_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtConnectionString);
            this.groupBox4.Controls.Add(this.btnSetConnectionString);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(652, 138);
            this.groupBox4.TabIndex = 0;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "&Connection String";
            // 
            // chkEnableLocalCache
            // 
            this.chkEnableLocalCache.AutoSize = true;
            this.chkEnableLocalCache.Location = new System.Drawing.Point(187, 166);
            this.chkEnableLocalCache.Name = "chkEnableLocalCache";
            this.chkEnableLocalCache.Size = new System.Drawing.Size(308, 21);
            this.chkEnableLocalCache.TabIndex = 1;
            this.chkEnableLocalCache.Text = "&Enable caching of current project index data";
            this.chkEnableLocalCache.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(495, 192);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(170, 23);
            this.label6.TabIndex = 4;
            this.label6.Text = "(0 to disable local cache)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // udcLocalCacheSize
            // 
            this.udcLocalCacheSize.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.udcLocalCacheSize.Location = new System.Drawing.Point(433, 193);
            this.udcLocalCacheSize.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.udcLocalCacheSize.Name = "udcLocalCacheSize";
            this.udcLocalCacheSize.Size = new System.Drawing.Size(56, 22);
            this.udcLocalCacheSize.TabIndex = 3;
            this.udcLocalCacheSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.udcLocalCacheSize.Value = new decimal(new int[] {
            2500,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(300, 192);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 23);
            this.label3.TabIndex = 2;
            this.label3.Text = "Local Cache Size";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // udcInMemoryCacheSize
            // 
            this.udcInMemoryCacheSize.Location = new System.Drawing.Point(433, 221);
            this.udcInMemoryCacheSize.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.udcInMemoryCacheSize.Name = "udcInMemoryCacheSize";
            this.udcInMemoryCacheSize.Size = new System.Drawing.Size(42, 22);
            this.udcInMemoryCacheSize.TabIndex = 6;
            this.udcInMemoryCacheSize.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.udcInMemoryCacheSize.Value = new decimal(new int[] {
            15,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(11, 220);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(416, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "Maximum number of uncached files kept in memory during build";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // SqlReflectionIndexConfigDlg
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(676, 308);
            this.Controls.Add(this.chkEnableLocalCache);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.udcLocalCacheSize);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.udcInMemoryCacheSize);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.btnPurge);
            this.Controls.Add(this.lnkCodePlexSHFB);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SqlReflectionIndexConfigDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure SQL Copy From Index Component";
            ((System.ComponentModel.ISupportInitialize)(this.epErrors)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udcLocalCacheSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcInMemoryCacheSize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSetConnectionString;
        private System.Windows.Forms.TextBox txtConnectionString;
        private System.Windows.Forms.LinkLabel lnkCodePlexSHFB;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ErrorProvider epErrors;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnPurge;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.CheckBox chkEnableLocalCache;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown udcLocalCacheSize;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown udcInMemoryCacheSize;
        private System.Windows.Forms.Label label2;
    }
}