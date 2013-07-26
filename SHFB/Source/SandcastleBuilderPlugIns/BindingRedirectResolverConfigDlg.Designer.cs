namespace SandcastleBuilder.PlugIns
{
    partial class BindingRedirectResolverConfigDlg
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BindingRedirectResolverConfigDlg));
            this.ilButton = new System.Windows.Forms.ImageList(this.components);
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAddFile = new System.Windows.Forms.Button();
            this.lbRedirects = new SandcastleBuilder.Utils.Controls.RefreshableItemListBox();
            this.pgProps = new SandcastleBuilder.Utils.Controls.CustomPropertyGrid();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.lnkCodePlexSHFB = new System.Windows.Forms.LinkLabel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnDeleteIgnoredName = new System.Windows.Forms.Button();
            this.btnAddIgnoredName = new System.Windows.Forms.Button();
            this.epErrors = new System.Windows.Forms.ErrorProvider(this.components);
            this.chkUseGAC = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.pgBindingRedirections = new System.Windows.Forms.TabPage();
            this.pgIgnoreIfUnresolved = new System.Windows.Forms.TabPage();
            this.txtAssemblyName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lbIgnoreIfUnresolved = new SandcastleBuilder.Utils.Controls.RefreshableItemListBox();
            ((System.ComponentModel.ISupportInitialize)(this.epErrors)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.pgBindingRedirections.SuspendLayout();
            this.pgIgnoreIfUnresolved.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilButton
            // 
            this.ilButton.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilButton.ImageStream")));
            this.ilButton.TransparentColor = System.Drawing.Color.Magenta;
            this.ilButton.Images.SetKeyName(0, "AddItem.bmp");
            this.ilButton.Images.SetKeyName(1, "Delete.bmp");
            // 
            // btnDelete
            // 
            this.btnDelete.ImageIndex = 1;
            this.btnDelete.ImageList = this.ilButton;
            this.btnDelete.Location = new System.Drawing.Point(662, 43);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(32, 32);
            this.btnDelete.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnDelete, "Delete the selected binding redirect");
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAddFile
            // 
            this.btnAddFile.ImageIndex = 0;
            this.btnAddFile.ImageList = this.ilButton;
            this.btnAddFile.Location = new System.Drawing.Point(662, 5);
            this.btnAddFile.Name = "btnAddFile";
            this.btnAddFile.Size = new System.Drawing.Size(32, 32);
            this.btnAddFile.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnAddFile, "Add a new binding redirect");
            this.btnAddFile.UseVisualStyleBackColor = true;
            this.btnAddFile.Click += new System.EventHandler(this.btnAddFile_Click);
            // 
            // lbRedirects
            // 
            this.lbRedirects.FormattingEnabled = true;
            this.lbRedirects.HorizontalScrollbar = true;
            this.lbRedirects.IntegralHeight = false;
            this.lbRedirects.ItemHeight = 16;
            this.lbRedirects.Location = new System.Drawing.Point(6, 6);
            this.lbRedirects.Name = "lbRedirects";
            this.lbRedirects.Size = new System.Drawing.Size(650, 119);
            this.lbRedirects.Sorted = true;
            this.lbRedirects.TabIndex = 0;
            this.lbRedirects.SelectedIndexChanged += new System.EventHandler(this.lbReferences_SelectedIndexChanged);
            // 
            // pgProps
            // 
            this.pgProps.Location = new System.Drawing.Point(6, 131);
            this.pgProps.Name = "pgProps";
            this.pgProps.PropertyNamePaneWidth = 150;
            this.pgProps.Size = new System.Drawing.Size(688, 308);
            this.pgProps.TabIndex = 3;
            this.pgProps.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgProps_PropertyValueChanged);
            // 
            // lnkCodePlexSHFB
            // 
            this.lnkCodePlexSHFB.Location = new System.Drawing.Point(222, 524);
            this.lnkCodePlexSHFB.Name = "lnkCodePlexSHFB";
            this.lnkCodePlexSHFB.Size = new System.Drawing.Size(267, 23);
            this.lnkCodePlexSHFB.TabIndex = 3;
            this.lnkCodePlexSHFB.TabStop = true;
            this.lnkCodePlexSHFB.Text = "Sandcastle Help File Builder";
            this.lnkCodePlexSHFB.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.toolTip1.SetToolTip(this.lnkCodePlexSHFB, "http://SHFB.CodePlex.com");
            this.lnkCodePlexSHFB.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkCodePlexSHFB_LinkClicked);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(639, 519);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(88, 32);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.toolTip1.SetToolTip(this.btnCancel, "Exit without saving changes");
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(12, 519);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(88, 32);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.toolTip1.SetToolTip(this.btnOK, "Save changes to configuration");
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnDeleteIgnoredName
            // 
            this.btnDeleteIgnoredName.Location = new System.Drawing.Point(6, 407);
            this.btnDeleteIgnoredName.Name = "btnDeleteIgnoredName";
            this.btnDeleteIgnoredName.Size = new System.Drawing.Size(88, 32);
            this.btnDeleteIgnoredName.TabIndex = 4;
            this.btnDeleteIgnoredName.Text = "&Delete";
            this.toolTip1.SetToolTip(this.btnDeleteIgnoredName, "Delete the selected assembly name");
            this.btnDeleteIgnoredName.UseVisualStyleBackColor = true;
            this.btnDeleteIgnoredName.Click += new System.EventHandler(this.btnDeleteIgnoredName_Click);
            // 
            // btnAddIgnoredName
            // 
            this.btnAddIgnoredName.Location = new System.Drawing.Point(613, 21);
            this.btnAddIgnoredName.Name = "btnAddIgnoredName";
            this.btnAddIgnoredName.Size = new System.Drawing.Size(88, 32);
            this.btnAddIgnoredName.TabIndex = 2;
            this.btnAddIgnoredName.Text = "&Add";
            this.toolTip1.SetToolTip(this.btnAddIgnoredName, "Add a new assembly name to be ignored if unresolved");
            this.btnAddIgnoredName.UseVisualStyleBackColor = true;
            this.btnAddIgnoredName.Click += new System.EventHandler(this.btnAddIgnoredName_Click);
            // 
            // epErrors
            // 
            this.epErrors.ContainerControl = this;
            // 
            // chkUseGAC
            // 
            this.chkUseGAC.AutoSize = true;
            this.chkUseGAC.Location = new System.Drawing.Point(12, 12);
            this.chkUseGAC.Name = "chkUseGAC";
            this.chkUseGAC.Size = new System.Drawing.Size(373, 21);
            this.chkUseGAC.TabIndex = 0;
            this.chkUseGAC.Text = "Use the GAC to resolve unknown assembly references";
            this.chkUseGAC.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.pgBindingRedirections);
            this.tabControl1.Controls.Add(this.pgIgnoreIfUnresolved);
            this.tabControl1.Location = new System.Drawing.Point(12, 39);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(715, 474);
            this.tabControl1.TabIndex = 1;
            // 
            // pgBindingRedirections
            // 
            this.pgBindingRedirections.Controls.Add(this.lbRedirects);
            this.pgBindingRedirections.Controls.Add(this.btnDelete);
            this.pgBindingRedirections.Controls.Add(this.btnAddFile);
            this.pgBindingRedirections.Controls.Add(this.pgProps);
            this.pgBindingRedirections.Location = new System.Drawing.Point(4, 25);
            this.pgBindingRedirections.Name = "pgBindingRedirections";
            this.pgBindingRedirections.Padding = new System.Windows.Forms.Padding(3);
            this.pgBindingRedirections.Size = new System.Drawing.Size(707, 445);
            this.pgBindingRedirections.TabIndex = 0;
            this.pgBindingRedirections.Text = "Binding Redirections";
            this.pgBindingRedirections.UseVisualStyleBackColor = true;
            // 
            // pgIgnoreIfUnresolved
            // 
            this.pgIgnoreIfUnresolved.Controls.Add(this.txtAssemblyName);
            this.pgIgnoreIfUnresolved.Controls.Add(this.label1);
            this.pgIgnoreIfUnresolved.Controls.Add(this.lbIgnoreIfUnresolved);
            this.pgIgnoreIfUnresolved.Controls.Add(this.btnDeleteIgnoredName);
            this.pgIgnoreIfUnresolved.Controls.Add(this.btnAddIgnoredName);
            this.pgIgnoreIfUnresolved.Location = new System.Drawing.Point(4, 25);
            this.pgIgnoreIfUnresolved.Name = "pgIgnoreIfUnresolved";
            this.pgIgnoreIfUnresolved.Padding = new System.Windows.Forms.Padding(3);
            this.pgIgnoreIfUnresolved.Size = new System.Drawing.Size(707, 445);
            this.pgIgnoreIfUnresolved.TabIndex = 1;
            this.pgIgnoreIfUnresolved.Text = "Ignore If Unresolved";
            this.pgIgnoreIfUnresolved.UseVisualStyleBackColor = true;
            // 
            // txtAssemblyName
            // 
            this.txtAssemblyName.Location = new System.Drawing.Point(185, 26);
            this.txtAssemblyName.Name = "txtAssemblyName";
            this.txtAssemblyName.Size = new System.Drawing.Size(422, 22);
            this.txtAssemblyName.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(6, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(173, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Assembly Name to Ignore";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbIgnoreIfUnresolved
            // 
            this.lbIgnoreIfUnresolved.FormattingEnabled = true;
            this.lbIgnoreIfUnresolved.HorizontalScrollbar = true;
            this.lbIgnoreIfUnresolved.IntegralHeight = false;
            this.lbIgnoreIfUnresolved.ItemHeight = 16;
            this.lbIgnoreIfUnresolved.Location = new System.Drawing.Point(6, 59);
            this.lbIgnoreIfUnresolved.Name = "lbIgnoreIfUnresolved";
            this.lbIgnoreIfUnresolved.Size = new System.Drawing.Size(695, 342);
            this.lbIgnoreIfUnresolved.Sorted = true;
            this.lbIgnoreIfUnresolved.TabIndex = 3;
            // 
            // BindingRedirectResolverConfigDlg
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(739, 563);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.chkUseGAC);
            this.Controls.Add(this.lnkCodePlexSHFB);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BindingRedirectResolverConfigDlg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Configure Assembly Binding Redirection Plug-In";
            ((System.ComponentModel.ISupportInitialize)(this.epErrors)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.pgBindingRedirections.ResumeLayout(false);
            this.pgIgnoreIfUnresolved.ResumeLayout(false);
            this.pgIgnoreIfUnresolved.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ImageList ilButton;
        private System.Windows.Forms.Button btnAddFile;
        private SandcastleBuilder.Utils.Controls.RefreshableItemListBox lbRedirects;
        private SandcastleBuilder.Utils.Controls.CustomPropertyGrid pgProps;
        private System.Windows.Forms.LinkLabel lnkCodePlexSHFB;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.ErrorProvider epErrors;
        private System.Windows.Forms.CheckBox chkUseGAC;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage pgBindingRedirections;
        private System.Windows.Forms.TabPage pgIgnoreIfUnresolved;
        private System.Windows.Forms.TextBox txtAssemblyName;
        private System.Windows.Forms.Label label1;
        private Utils.Controls.RefreshableItemListBox lbIgnoreIfUnresolved;
        private System.Windows.Forms.Button btnDeleteIgnoredName;
        private System.Windows.Forms.Button btnAddIgnoredName;
    }
}