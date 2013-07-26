﻿namespace SandcastleBuilder.Package.PropertyPages
{
    partial class ComponentPropertiesPageControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ComponentPropertiesPageControl));
            this.ilImages = new System.Windows.Forms.ImageList(this.components);
            this.gbProjectAddIns = new System.Windows.Forms.GroupBox();
            this.btnConfigure = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnAddComponent = new System.Windows.Forms.Button();
            this.lbProjectComponents = new System.Windows.Forms.CheckedListBox();
            this.gbAvailableComponents = new System.Windows.Forms.GroupBox();
            this.lbAvailableComponents = new System.Windows.Forms.ListBox();
            this.txtComponentVersion = new System.Windows.Forms.TextBox();
            this.txtComponentCopyright = new System.Windows.Forms.TextBox();
            this.txtComponentDescription = new System.Windows.Forms.TextBox();
            this.gbProjectAddIns.SuspendLayout();
            this.gbAvailableComponents.SuspendLayout();
            this.SuspendLayout();
            // 
            // ilImages
            // 
            this.ilImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilImages.ImageStream")));
            this.ilImages.TransparentColor = System.Drawing.Color.Magenta;
            this.ilImages.Images.SetKeyName(0, "ComponentAdd.png");
            this.ilImages.Images.SetKeyName(1, "Delete.bmp");
            this.ilImages.Images.SetKeyName(2, "MoveUp.bmp");
            this.ilImages.Images.SetKeyName(3, "MoveDown.bmp");
            this.ilImages.Images.SetKeyName(4, "Properties.bmp");
            // 
            // gbProjectAddIns
            // 
            this.gbProjectAddIns.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbProjectAddIns.Controls.Add(this.btnConfigure);
            this.gbProjectAddIns.Controls.Add(this.btnDelete);
            this.gbProjectAddIns.Controls.Add(this.btnAddComponent);
            this.gbProjectAddIns.Controls.Add(this.lbProjectComponents);
            this.gbProjectAddIns.Location = new System.Drawing.Point(355, 3);
            this.gbProjectAddIns.Name = "gbProjectAddIns";
            this.gbProjectAddIns.Size = new System.Drawing.Size(346, 312);
            this.gbProjectAddIns.TabIndex = 1;
            this.gbProjectAddIns.TabStop = false;
            this.gbProjectAddIns.Text = "&Build Components in This Project";
            // 
            // btnConfigure
            // 
            this.btnConfigure.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnConfigure.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConfigure.ImageIndex = 4;
            this.btnConfigure.ImageList = this.ilImages;
            this.btnConfigure.Location = new System.Drawing.Point(120, 272);
            this.btnConfigure.Name = "btnConfigure";
            this.btnConfigure.Padding = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.btnConfigure.Size = new System.Drawing.Size(106, 32);
            this.btnConfigure.TabIndex = 2;
            this.btnConfigure.Text = "&Configure";
            this.btnConfigure.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnConfigure.UseVisualStyleBackColor = true;
            this.btnConfigure.Click += new System.EventHandler(this.btnConfigure_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDelete.ImageIndex = 1;
            this.btnDelete.ImageList = this.ilImages;
            this.btnDelete.Location = new System.Drawing.Point(240, 272);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.btnDelete.Size = new System.Drawing.Size(100, 32);
            this.btnDelete.TabIndex = 3;
            this.btnDelete.Text = "&Delete";
            this.btnDelete.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnAddComponent
            // 
            this.btnAddComponent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddComponent.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAddComponent.ImageIndex = 0;
            this.btnAddComponent.ImageList = this.ilImages;
            this.btnAddComponent.Location = new System.Drawing.Point(6, 272);
            this.btnAddComponent.Name = "btnAddComponent";
            this.btnAddComponent.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.btnAddComponent.Size = new System.Drawing.Size(100, 32);
            this.btnAddComponent.TabIndex = 1;
            this.btnAddComponent.Text = "&Add";
            this.btnAddComponent.Click += new System.EventHandler(this.btnAddComponent_Click);
            // 
            // lbProjectComponents
            // 
            this.lbProjectComponents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbProjectComponents.IntegralHeight = false;
            this.lbProjectComponents.Location = new System.Drawing.Point(6, 21);
            this.lbProjectComponents.Name = "lbProjectComponents";
            this.lbProjectComponents.Size = new System.Drawing.Size(334, 245);
            this.lbProjectComponents.Sorted = true;
            this.lbProjectComponents.TabIndex = 0;
            this.lbProjectComponents.Tag = "ComponentConfigurations";
            this.lbProjectComponents.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.lbProjectComponents_ItemCheck);
            // 
            // gbAvailableComponents
            // 
            this.gbAvailableComponents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.gbAvailableComponents.Controls.Add(this.lbAvailableComponents);
            this.gbAvailableComponents.Location = new System.Drawing.Point(3, 3);
            this.gbAvailableComponents.Name = "gbAvailableComponents";
            this.gbAvailableComponents.Size = new System.Drawing.Size(346, 276);
            this.gbAvailableComponents.TabIndex = 0;
            this.gbAvailableComponents.TabStop = false;
            this.gbAvailableComponents.Text = "A&vailable Build Components";
            // 
            // lbAvailableComponents
            // 
            this.lbAvailableComponents.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lbAvailableComponents.IntegralHeight = false;
            this.lbAvailableComponents.ItemHeight = 20;
            this.lbAvailableComponents.Location = new System.Drawing.Point(6, 21);
            this.lbAvailableComponents.Name = "lbAvailableComponents";
            this.lbAvailableComponents.Size = new System.Drawing.Size(334, 249);
            this.lbAvailableComponents.Sorted = true;
            this.lbAvailableComponents.TabIndex = 0;
            this.lbAvailableComponents.SelectedIndexChanged += new System.EventHandler(this.lbAvailableComponents_SelectedIndexChanged);
            this.lbAvailableComponents.DoubleClick += new System.EventHandler(this.btnAddComponent_Click);
            // 
            // txtComponentVersion
            // 
            this.txtComponentVersion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtComponentVersion.Location = new System.Drawing.Point(3, 285);
            this.txtComponentVersion.Name = "txtComponentVersion";
            this.txtComponentVersion.ReadOnly = true;
            this.txtComponentVersion.Size = new System.Drawing.Size(346, 27);
            this.txtComponentVersion.TabIndex = 1;
            // 
            // txtComponentCopyright
            // 
            this.txtComponentCopyright.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtComponentCopyright.Location = new System.Drawing.Point(3, 321);
            this.txtComponentCopyright.Multiline = true;
            this.txtComponentCopyright.Name = "txtComponentCopyright";
            this.txtComponentCopyright.ReadOnly = true;
            this.txtComponentCopyright.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtComponentCopyright.Size = new System.Drawing.Size(699, 56);
            this.txtComponentCopyright.TabIndex = 2;
            // 
            // txtComponentDescription
            // 
            this.txtComponentDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtComponentDescription.Location = new System.Drawing.Point(3, 383);
            this.txtComponentDescription.Multiline = true;
            this.txtComponentDescription.Name = "txtComponentDescription";
            this.txtComponentDescription.ReadOnly = true;
            this.txtComponentDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtComponentDescription.Size = new System.Drawing.Size(698, 164);
            this.txtComponentDescription.TabIndex = 3;
            // 
            // ComponentPropertiesPageControl
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.Controls.Add(this.txtComponentVersion);
            this.Controls.Add(this.gbProjectAddIns);
            this.Controls.Add(this.txtComponentCopyright);
            this.Controls.Add(this.gbAvailableComponents);
            this.Controls.Add(this.txtComponentDescription);
            this.MinimumSize = new System.Drawing.Size(705, 550);
            this.Name = "ComponentPropertiesPageControl";
            this.Size = new System.Drawing.Size(705, 550);
            this.gbProjectAddIns.ResumeLayout(false);
            this.gbAvailableComponents.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ImageList ilImages;
        private System.Windows.Forms.GroupBox gbProjectAddIns;
        private System.Windows.Forms.Button btnConfigure;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAddComponent;
        private System.Windows.Forms.CheckedListBox lbProjectComponents;
        private System.Windows.Forms.GroupBox gbAvailableComponents;
        private System.Windows.Forms.TextBox txtComponentVersion;
        private System.Windows.Forms.TextBox txtComponentCopyright;
        private System.Windows.Forms.TextBox txtComponentDescription;
        private System.Windows.Forms.ListBox lbAvailableComponents;

    }
}
