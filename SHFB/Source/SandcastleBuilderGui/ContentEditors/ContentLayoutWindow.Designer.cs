namespace SandcastleBuilder.Gui.ContentEditors
{
    partial class ContentLayoutWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContentLayoutWindow));
            this.ehContentLayoutEditorHost = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // ehContentLayoutEditorHost
            // 
            this.ehContentLayoutEditorHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ehContentLayoutEditorHost.Location = new System.Drawing.Point(0, 0);
            this.ehContentLayoutEditorHost.Name = "ehContentLayoutEditorHost";
            this.ehContentLayoutEditorHost.Size = new System.Drawing.Size(882, 555);
            this.ehContentLayoutEditorHost.TabIndex = 0;
            this.ehContentLayoutEditorHost.Child = null;
            // 
            // ContentLayoutWindow
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.ClientSize = new System.Drawing.Size(882, 555);
            this.Controls.Add(this.ehContentLayoutEditorHost);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ContentLayoutWindow";
            this.ShowHint = WeifenLuo.WinFormsUI.Docking.DockState.Document;
            this.ShowInTaskbar = false;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ContentLayoutWindow_FormClosing);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Integration.ElementHost ehContentLayoutEditorHost;

    }
}
