﻿namespace SandcastleBuilder.Package.PropertyPages
{
    partial class GeneralOptionsControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.epErrors = new System.Windows.Forms.ErrorProvider(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnSelectMSHCViewer = new System.Windows.Forms.Button();
            this.btnSelectHxSViewer = new System.Windows.Forms.Button();
            this.lnkSHFBInfo = new System.Windows.Forms.LinkLabel();
            this.txtMSHelpViewerPath = new System.Windows.Forms.TextBox();
            this.lblMSHelpViewer = new System.Windows.Forms.Label();
            this.chkOpenHelpAfterBuild = new System.Windows.Forms.CheckBox();
            this.txtHxSViewerPath = new System.Windows.Forms.TextBox();
            this.udcASPNetDevServerPort = new System.Windows.Forms.NumericUpDown();
            this.chkVerboseLogging = new System.Windows.Forms.CheckBox();
            this.lblASPPort = new System.Windows.Forms.Label();
            this.lblHelp2Viewer = new System.Windows.Forms.Label();
            this.chkUseExternalBrowser = new System.Windows.Forms.CheckBox();
            this.chkOpenLogViewerOnFailure = new System.Windows.Forms.CheckBox();
            this.chkEnableExtendedXmlComments = new System.Windows.Forms.CheckBox();
            this.chkEnableGoToDefinition = new System.Windows.Forms.CheckBox();
            this.lblSetColorNote = new System.Windows.Forms.Label();
            this.chkEnableGoToDefinitionInCRef = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.epErrors)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcASPNetDevServerPort)).BeginInit();
            this.SuspendLayout();
            // 
            // epErrors
            // 
            this.epErrors.ContainerControl = this;
            // 
            // btnSelectMSHCViewer
            // 
            this.btnSelectMSHCViewer.Location = new System.Drawing.Point(376, 88);
            this.btnSelectMSHCViewer.Name = "btnSelectMSHCViewer";
            this.btnSelectMSHCViewer.Size = new System.Drawing.Size(32, 27);
            this.btnSelectMSHCViewer.TabIndex = 5;
            this.btnSelectMSHCViewer.Text = "...";
            this.toolTip1.SetToolTip(this.btnSelectMSHCViewer, "Select MS Help Viewer viewer application");
            this.btnSelectMSHCViewer.UseVisualStyleBackColor = true;
            this.btnSelectMSHCViewer.Click += new System.EventHandler(this.btnSelectViewer_Click);
            // 
            // btnSelectHxSViewer
            // 
            this.btnSelectHxSViewer.Location = new System.Drawing.Point(375, 32);
            this.btnSelectHxSViewer.Name = "btnSelectHxSViewer";
            this.btnSelectHxSViewer.Size = new System.Drawing.Size(32, 27);
            this.btnSelectHxSViewer.TabIndex = 2;
            this.btnSelectHxSViewer.Text = "...";
            this.toolTip1.SetToolTip(this.btnSelectHxSViewer, "Select MS Help 2 HxS viewer application");
            this.btnSelectHxSViewer.UseVisualStyleBackColor = true;
            this.btnSelectHxSViewer.Click += new System.EventHandler(this.btnSelectViewer_Click);
            // 
            // lnkSHFBInfo
            // 
            this.lnkSHFBInfo.AutoSize = true;
            this.lnkSHFBInfo.Location = new System.Drawing.Point(4, 441);
            this.lnkSHFBInfo.Name = "lnkSHFBInfo";
            this.lnkSHFBInfo.Size = new System.Drawing.Size(239, 20);
            this.lnkSHFBInfo.TabIndex = 15;
            this.lnkSHFBInfo.TabStop = true;
            this.lnkSHFBInfo.Text = "About Sandcastle Help File Builder";
            this.toolTip1.SetToolTip(this.lnkSHFBInfo, "SHFB version and contact information");
            this.lnkSHFBInfo.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkSHFBInfo_LinkClicked);
            // 
            // txtMSHelpViewerPath
            // 
            this.txtMSHelpViewerPath.Location = new System.Drawing.Point(8, 88);
            this.txtMSHelpViewerPath.Name = "txtMSHelpViewerPath";
            this.txtMSHelpViewerPath.Size = new System.Drawing.Size(358, 27);
            this.txtMSHelpViewerPath.TabIndex = 4;
            // 
            // lblMSHelpViewer
            // 
            this.lblMSHelpViewer.Location = new System.Drawing.Point(4, 62);
            this.lblMSHelpViewer.Name = "lblMSHelpViewer";
            this.lblMSHelpViewer.Size = new System.Drawing.Size(359, 23);
            this.lblMSHelpViewer.TabIndex = 3;
            this.lblMSHelpViewer.Text = "Alternate &MS Help Viewer (.mshc) Viewer Path";
            this.lblMSHelpViewer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkOpenHelpAfterBuild
            // 
            this.chkOpenHelpAfterBuild.AutoSize = true;
            this.chkOpenHelpAfterBuild.Location = new System.Drawing.Point(34, 229);
            this.chkOpenHelpAfterBuild.Name = "chkOpenHelpAfterBuild";
            this.chkOpenHelpAfterBuild.Size = new System.Drawing.Size(267, 24);
            this.chkOpenHelpAfterBuild.TabIndex = 10;
            this.chkOpenHelpAfterBuild.Text = "&Open help file after successful build";
            this.chkOpenHelpAfterBuild.UseVisualStyleBackColor = true;
            // 
            // txtHxSViewerPath
            // 
            this.txtHxSViewerPath.Location = new System.Drawing.Point(7, 32);
            this.txtHxSViewerPath.Name = "txtHxSViewerPath";
            this.txtHxSViewerPath.Size = new System.Drawing.Size(358, 27);
            this.txtHxSViewerPath.TabIndex = 1;
            // 
            // udcASPNetDevServerPort
            // 
            this.udcASPNetDevServerPort.Location = new System.Drawing.Point(293, 130);
            this.udcASPNetDevServerPort.Maximum = new decimal(new int[] {
            99999,
            0,
            0,
            0});
            this.udcASPNetDevServerPort.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.udcASPNetDevServerPort.Name = "udcASPNetDevServerPort";
            this.udcASPNetDevServerPort.Size = new System.Drawing.Size(70, 27);
            this.udcASPNetDevServerPort.TabIndex = 7;
            this.udcASPNetDevServerPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.udcASPNetDevServerPort.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            // 
            // chkVerboseLogging
            // 
            this.chkVerboseLogging.AutoSize = true;
            this.chkVerboseLogging.Location = new System.Drawing.Point(34, 169);
            this.chkVerboseLogging.Name = "chkVerboseLogging";
            this.chkVerboseLogging.Size = new System.Drawing.Size(283, 24);
            this.chkVerboseLogging.TabIndex = 8;
            this.chkVerboseLogging.Text = "&Build output verbose logging enabled";
            this.chkVerboseLogging.UseVisualStyleBackColor = true;
            // 
            // lblASPPort
            // 
            this.lblASPPort.Location = new System.Drawing.Point(3, 131);
            this.lblASPPort.Name = "lblASPPort";
            this.lblASPPort.Size = new System.Drawing.Size(284, 23);
            this.lblASPPort.TabIndex = 6;
            this.lblASPPort.Text = "&ASP.NET Development Web Server Port";
            this.lblASPPort.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblHelp2Viewer
            // 
            this.lblHelp2Viewer.Location = new System.Drawing.Point(4, 6);
            this.lblHelp2Viewer.Name = "lblHelp2Viewer";
            this.lblHelp2Viewer.Size = new System.Drawing.Size(344, 23);
            this.lblHelp2Viewer.TabIndex = 0;
            this.lblHelp2Viewer.Text = "MS &Help 2 (.HxS) Viewer Path";
            this.lblHelp2Viewer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkUseExternalBrowser
            // 
            this.chkUseExternalBrowser.AutoSize = true;
            this.chkUseExternalBrowser.Location = new System.Drawing.Point(34, 259);
            this.chkUseExternalBrowser.Name = "chkUseExternalBrowser";
            this.chkUseExternalBrowser.Size = new System.Drawing.Size(347, 24);
            this.chkUseExternalBrowser.TabIndex = 11;
            this.chkUseExternalBrowser.Text = "Use &external web browser to view help websites";
            this.chkUseExternalBrowser.UseVisualStyleBackColor = true;
            // 
            // chkOpenLogViewerOnFailure
            // 
            this.chkOpenLogViewerOnFailure.AutoSize = true;
            this.chkOpenLogViewerOnFailure.Location = new System.Drawing.Point(34, 199);
            this.chkOpenLogViewerOnFailure.Name = "chkOpenLogViewerOnFailure";
            this.chkOpenLogViewerOnFailure.Size = new System.Drawing.Size(279, 24);
            this.chkOpenLogViewerOnFailure.TabIndex = 9;
            this.chkOpenLogViewerOnFailure.Text = "Open b&uild log viewer on failed build";
            this.chkOpenLogViewerOnFailure.UseVisualStyleBackColor = true;
            // 
            // chkEnableExtendedXmlComments
            // 
            this.chkEnableExtendedXmlComments.AutoSize = true;
            this.chkEnableExtendedXmlComments.Location = new System.Drawing.Point(34, 301);
            this.chkEnableExtendedXmlComments.Name = "chkEnableExtendedXmlComments";
            this.chkEnableExtendedXmlComments.Size = new System.Drawing.Size(382, 24);
            this.chkEnableExtendedXmlComments.TabIndex = 12;
            this.chkEnableExtendedXmlComments.Text = "Enable extended XML comments completion options";
            this.chkEnableExtendedXmlComments.UseVisualStyleBackColor = true;
            // 
            // chkEnableGoToDefinition
            // 
            this.chkEnableGoToDefinition.AutoSize = true;
            this.chkEnableGoToDefinition.Location = new System.Drawing.Point(34, 331);
            this.chkEnableGoToDefinition.Name = "chkEnableGoToDefinition";
            this.chkEnableGoToDefinition.Size = new System.Drawing.Size(345, 24);
            this.chkEnableGoToDefinition.TabIndex = 13;
            this.chkEnableGoToDefinition.Text = "Enable MAML/XML comments Go To Definition";
            this.chkEnableGoToDefinition.UseVisualStyleBackColor = true;
            this.chkEnableGoToDefinition.CheckedChanged += new System.EventHandler(this.chkEnableGoToDefinition_CheckedChanged);
            // 
            // lblSetColorNote
            // 
            this.lblSetColorNote.Location = new System.Drawing.Point(52, 388);
            this.lblSetColorNote.Name = "lblSetColorNote";
            this.lblSetColorNote.Size = new System.Drawing.Size(372, 23);
            this.lblSetColorNote.TabIndex = 14;
            this.lblSetColorNote.Text = "(Set color with MAML/XML Comments Link Underline)";
            this.lblSetColorNote.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkEnableGoToDefinitionInCRef
            // 
            this.chkEnableGoToDefinitionInCRef.AutoSize = true;
            this.chkEnableGoToDefinitionInCRef.Location = new System.Drawing.Point(56, 361);
            this.chkEnableGoToDefinitionInCRef.Name = "chkEnableGoToDefinitionInCRef";
            this.chkEnableGoToDefinitionInCRef.Size = new System.Drawing.Size(279, 24);
            this.chkEnableGoToDefinitionInCRef.TabIndex = 16;
            this.chkEnableGoToDefinitionInCRef.Text = "Enable for XML comments cref values";
            this.chkEnableGoToDefinitionInCRef.UseVisualStyleBackColor = true;
            // 
            // GeneralOptionsControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.AutoScroll = true;
            this.Controls.Add(this.chkEnableGoToDefinitionInCRef);
            this.Controls.Add(this.lblHelp2Viewer);
            this.Controls.Add(this.txtHxSViewerPath);
            this.Controls.Add(this.btnSelectHxSViewer);
            this.Controls.Add(this.lblMSHelpViewer);
            this.Controls.Add(this.txtMSHelpViewerPath);
            this.Controls.Add(this.btnSelectMSHCViewer);
            this.Controls.Add(this.lblASPPort);
            this.Controls.Add(this.udcASPNetDevServerPort);
            this.Controls.Add(this.chkVerboseLogging);
            this.Controls.Add(this.chkOpenLogViewerOnFailure);
            this.Controls.Add(this.chkOpenHelpAfterBuild);
            this.Controls.Add(this.chkUseExternalBrowser);
            this.Controls.Add(this.chkEnableExtendedXmlComments);
            this.Controls.Add(this.chkEnableGoToDefinition);
            this.Controls.Add(this.lblSetColorNote);
            this.Controls.Add(this.lnkSHFBInfo);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(430, 100);
            this.Name = "GeneralOptionsControl";
            this.Size = new System.Drawing.Size(435, 479);
            ((System.ComponentModel.ISupportInitialize)(this.epErrors)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.udcASPNetDevServerPort)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ErrorProvider epErrors;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.TextBox txtMSHelpViewerPath;
        private System.Windows.Forms.Label lblMSHelpViewer;
        private System.Windows.Forms.Label lblHelp2Viewer;
        private System.Windows.Forms.Button btnSelectMSHCViewer;
        private System.Windows.Forms.CheckBox chkOpenHelpAfterBuild;
        private System.Windows.Forms.TextBox txtHxSViewerPath;
        private System.Windows.Forms.NumericUpDown udcASPNetDevServerPort;
        private System.Windows.Forms.CheckBox chkVerboseLogging;
        private System.Windows.Forms.Label lblASPPort;
        private System.Windows.Forms.Button btnSelectHxSViewer;
        private System.Windows.Forms.CheckBox chkUseExternalBrowser;
        private System.Windows.Forms.CheckBox chkOpenLogViewerOnFailure;
        private System.Windows.Forms.LinkLabel lnkSHFBInfo;
        private System.Windows.Forms.CheckBox chkEnableGoToDefinition;
        private System.Windows.Forms.CheckBox chkEnableExtendedXmlComments;
        private System.Windows.Forms.Label lblSetColorNote;
        private System.Windows.Forms.CheckBox chkEnableGoToDefinitionInCRef;
    }
}
