﻿namespace SandcastleBuilder.PlugIns
{
    partial class MemberIdFixUpPlugInConfigDlg
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
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tvExpressions = new System.Windows.Forms.TreeView();
            this.txtMatchExpression = new System.Windows.Forms.TextBox();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lnkCodePlexSHFB = new System.Windows.Forms.LinkLabel();
            this.btnCPPFixes = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMatchExpression = new System.Windows.Forms.Label();
            this.epErrors = new System.Windows.Forms.ErrorProvider(this.components);
            this.chkMatchAsRegEx = new System.Windows.Forms.CheckBox();
            this.lblReplacementValue = new System.Windows.Forms.Label();
            this.txtReplacementValue = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.epErrors)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(657, 438);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 32);
            this.btnCancel.TabIndex = 11;
            this.btnCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.btnCancel, "Exit without saving changes");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 438);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 32);
            this.btnOK.TabIndex = 10;
            this.btnOK.Text = "OK";
            this.toolTip1.SetToolTip(this.btnOK, "Save configuration");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // tvExpressions
            // 
            this.tvExpressions.Font = new System.Drawing.Font("Courier New", 10F);
            this.tvExpressions.FullRowSelect = true;
            this.tvExpressions.HideSelection = false;
            this.tvExpressions.Location = new System.Drawing.Point(12, 35);
            this.tvExpressions.Name = "tvExpressions";
            this.tvExpressions.ShowRootLines = false;
            this.tvExpressions.Size = new System.Drawing.Size(733, 160);
            this.tvExpressions.TabIndex = 1;
            this.tvExpressions.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.tvExpressions_BeforeSelect);
            this.tvExpressions.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvExpressions_AfterSelect);
            // 
            // txtMatchExpression
            // 
            this.txtMatchExpression.Enabled = false;
            this.txtMatchExpression.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtMatchExpression.Location = new System.Drawing.Point(12, 239);
            this.txtMatchExpression.Multiline = true;
            this.txtMatchExpression.Name = "txtMatchExpression";
            this.txtMatchExpression.Size = new System.Drawing.Size(733, 61);
            this.txtMatchExpression.TabIndex = 6;
            this.txtMatchExpression.Enter += new System.EventHandler(this.Expression_Enter);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(469, 201);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(88, 32);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "&Add";
            this.toolTip1.SetToolTip(this.btnAdd, "Add a new match expression");
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Enabled = false;
            this.btnDelete.Location = new System.Drawing.Point(657, 201);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(88, 32);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "&Delete";
            this.toolTip1.SetToolTip(this.btnDelete, "Delete the selected match expression");
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lnkCodePlexSHFB
            // 
            this.lnkCodePlexSHFB.Location = new System.Drawing.Point(269, 443);
            this.lnkCodePlexSHFB.Name = "lnkCodePlexSHFB";
            this.lnkCodePlexSHFB.Size = new System.Drawing.Size(218, 23);
            this.lnkCodePlexSHFB.TabIndex = 12;
            this.lnkCodePlexSHFB.TabStop = true;
            this.lnkCodePlexSHFB.Text = "Sandcastle Help File Builder";
            this.lnkCodePlexSHFB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lnkCodePlexSHFB, "http://SHFB.CodePlex.com");
            this.lnkCodePlexSHFB.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCodePlexSHFB_LinkClicked);
            // 
            // btnCPPFixes
            // 
            this.btnCPPFixes.Location = new System.Drawing.Point(563, 201);
            this.btnCPPFixes.Name = "btnCPPFixes";
            this.btnCPPFixes.Size = new System.Drawing.Size(88, 32);
            this.btnCPPFixes.TabIndex = 3;
            this.btnCPPFixes.Text = "&C++ Fixes";
            this.toolTip1.SetToolTip(this.btnCPPFixes, "Add the common C++ fix-up expressions");
            this.btnCPPFixes.UseVisualStyleBackColor = true;
            this.btnCPPFixes.Click += new System.EventHandler(this.btnCPPFixes_Click);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(140, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Fix-Up Expressions";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblMatchExpression
            // 
            this.lblMatchExpression.Location = new System.Drawing.Point(12, 213);
            this.lblMatchExpression.Name = "lblMatchExpression";
            this.lblMatchExpression.Size = new System.Drawing.Size(128, 23);
            this.lblMatchExpression.TabIndex = 5;
            this.lblMatchExpression.Text = "&Match Expression";
            this.lblMatchExpression.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // epErrors
            // 
            this.epErrors.ContainerControl = this;
            // 
            // chkMatchAsRegEx
            // 
            this.chkMatchAsRegEx.AutoSize = true;
            this.chkMatchAsRegEx.Enabled = false;
            this.chkMatchAsRegEx.Location = new System.Drawing.Point(12, 396);
            this.chkMatchAsRegEx.Name = "chkMatchAsRegEx";
            this.chkMatchAsRegEx.Size = new System.Drawing.Size(239, 21);
            this.chkMatchAsRegEx.TabIndex = 9;
            this.chkMatchAsRegEx.Text = "Ma&tch using a regular expression";
            this.chkMatchAsRegEx.UseVisualStyleBackColor = true;
            // 
            // lblReplacementValue
            // 
            this.lblReplacementValue.Location = new System.Drawing.Point(12, 303);
            this.lblReplacementValue.Name = "lblReplacementValue";
            this.lblReplacementValue.Size = new System.Drawing.Size(99, 23);
            this.lblReplacementValue.TabIndex = 7;
            this.lblReplacementValue.Text = "&Replace With";
            this.lblReplacementValue.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtReplacementValue
            // 
            this.txtReplacementValue.Enabled = false;
            this.txtReplacementValue.Font = new System.Drawing.Font("Courier New", 10F);
            this.txtReplacementValue.Location = new System.Drawing.Point(12, 329);
            this.txtReplacementValue.Multiline = true;
            this.txtReplacementValue.Name = "txtReplacementValue";
            this.txtReplacementValue.Size = new System.Drawing.Size(733, 61);
            this.txtReplacementValue.TabIndex = 8;
            this.txtReplacementValue.Enter += new System.EventHandler(this.Expression_Enter);
            // 
            // MemberIdFixUpPlugInConfigDlg
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(757, 482);
            this.Controls.Add(this.btnCPPFixes);
            this.Controls.Add(this.lblReplacementValue);
            this.Controls.Add(this.txtReplacementValue);
            this.Controls.Add(this.chkMatchAsRegEx);
            this.Controls.Add(this.lnkCodePlexSHFB);
            this.Controls.Add(this.lblMatchExpression);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnAdd);
            this.Controls.Add(this.txtMatchExpression);
            this.Controls.Add(this.tvExpressions);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MemberIdFixUpPlugInConfigDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Member ID Fix-Up Plug-In";
            ((System.ComponentModel.ISupportInitialize)(this.epErrors)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.TreeView tvExpressions;
        private System.Windows.Forms.TextBox txtMatchExpression;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMatchExpression;
        private System.Windows.Forms.ErrorProvider epErrors;
        private System.Windows.Forms.LinkLabel lnkCodePlexSHFB;
        private System.Windows.Forms.Label lblReplacementValue;
        private System.Windows.Forms.TextBox txtReplacementValue;
        private System.Windows.Forms.CheckBox chkMatchAsRegEx;
        private System.Windows.Forms.Button btnCPPFixes;
    }
}