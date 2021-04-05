namespace QASE
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            this.TabControlMain = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.treeviewMain = new System.Windows.Forms.TreeView();
            this.tabAbout = new System.Windows.Forms.TabPage();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuStripTreeView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deselectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.selectInverseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.saveSettingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStripTreeViewDefault = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TabControlMain.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabAbout.SuspendLayout();
            this.contextMenuStripTreeView.SuspendLayout();
            this.contextMenuStripTreeViewDefault.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControlMain
            // 
            this.TabControlMain.Controls.Add(this.tabMain);
            this.TabControlMain.Controls.Add(this.tabAbout);
            this.TabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControlMain.Location = new System.Drawing.Point(0, 0);
            this.TabControlMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.TabControlMain.Name = "TabControlMain";
            this.TabControlMain.SelectedIndex = 0;
            this.TabControlMain.Size = new System.Drawing.Size(1247, 880);
            this.TabControlMain.TabIndex = 7;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.treeviewMain);
            this.tabMain.Location = new System.Drawing.Point(4, 29);
            this.tabMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabMain.Size = new System.Drawing.Size(1239, 847);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main";
            this.tabMain.UseVisualStyleBackColor = true;
            // 
            // treeviewMain
            // 
            this.treeviewMain.CheckBoxes = true;
            this.treeviewMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeviewMain.FullRowSelect = true;
            this.treeviewMain.Location = new System.Drawing.Point(3, 2);
            this.treeviewMain.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.treeviewMain.Name = "treeviewMain";
            this.treeviewMain.Size = new System.Drawing.Size(1233, 843);
            this.treeviewMain.TabIndex = 7;
            this.treeviewMain.MouseUp += new System.Windows.Forms.MouseEventHandler(this.TreeviewMain_MouseUp);
            // 
            // tabAbout
            // 
            this.tabAbout.Controls.Add(this.linkLabel1);
            this.tabAbout.Controls.Add(this.label2);
            this.tabAbout.Controls.Add(this.label1);
            this.tabAbout.Location = new System.Drawing.Point(4, 29);
            this.tabAbout.Name = "tabAbout";
            this.tabAbout.Padding = new System.Windows.Forms.Padding(3);
            this.tabAbout.Size = new System.Drawing.Size(1239, 847);
            this.tabAbout.TabIndex = 2;
            this.tabAbout.Text = "About";
            this.tabAbout.UseVisualStyleBackColor = true;
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(8, 804);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(205, 25);
            this.linkLabel1.TabIndex = 2;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "https://aka.ms/bvdwiki";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(187, 25);
            this.label2.TabIndex = 1;
            this.label2.Text = "Requires Azure CLI.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(662, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "Simple, synchronous AutoScale enabler for Azure CosmosDb, suports CLI.";
            // 
            // contextMenuStripTreeView
            // 
            this.contextMenuStripTreeView.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.contextMenuStripTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.selectAllToolStripMenuItem,
            this.deselectAllToolStripMenuItem,
            this.selectInverseToolStripMenuItem,
            this.toolStripMenuItem1,
            this.saveSettingsToolStripMenuItem,
            this.toolStripMenuItem3,
            this.copyToolStripMenuItem});
            this.contextMenuStripTreeView.Name = "contextMenuStripTreeView";
            this.contextMenuStripTreeView.Size = new System.Drawing.Size(296, 196);
            // 
            // selectAllToolStripMenuItem
            // 
            this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
            this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(295, 36);
            this.selectAllToolStripMenuItem.Text = "Select All (in level)";
            this.selectAllToolStripMenuItem.Click += new System.EventHandler(this.SelectAllToolStripMenuItem_Click);
            // 
            // deselectAllToolStripMenuItem
            // 
            this.deselectAllToolStripMenuItem.Name = "deselectAllToolStripMenuItem";
            this.deselectAllToolStripMenuItem.Size = new System.Drawing.Size(295, 36);
            this.deselectAllToolStripMenuItem.Text = "Deselect All (in level)";
            this.deselectAllToolStripMenuItem.Click += new System.EventHandler(this.DeselectAllToolStripMenuItem_Click);
            // 
            // selectInverseToolStripMenuItem
            // 
            this.selectInverseToolStripMenuItem.Name = "selectInverseToolStripMenuItem";
            this.selectInverseToolStripMenuItem.Size = new System.Drawing.Size(295, 36);
            this.selectInverseToolStripMenuItem.Text = "Select inverse (in level)";
            this.selectInverseToolStripMenuItem.Click += new System.EventHandler(this.SelectInverseToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(292, 6);
            // 
            // saveSettingsToolStripMenuItem
            // 
            this.saveSettingsToolStripMenuItem.Name = "saveSettingsToolStripMenuItem";
            this.saveSettingsToolStripMenuItem.Size = new System.Drawing.Size(295, 36);
            this.saveSettingsToolStripMenuItem.Text = "Save settings";
            this.saveSettingsToolStripMenuItem.Click += new System.EventHandler(this.SaveSettingsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(292, 6);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(295, 36);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.CopyToolStripMenuItem_Click);
            // 
            // contextMenuStripTreeViewDefault
            // 
            this.contextMenuStripTreeViewDefault.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.contextMenuStripTreeViewDefault.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyStripMenuItem});
            this.contextMenuStripTreeViewDefault.Name = "contextMenuStripTreeView";
            this.contextMenuStripTreeViewDefault.Size = new System.Drawing.Size(134, 40);
            // 
            // copyStripMenuItem
            // 
            this.copyStripMenuItem.Name = "copyStripMenuItem";
            this.copyStripMenuItem.Size = new System.Drawing.Size(133, 36);
            this.copyStripMenuItem.Text = "Copy";
            this.copyStripMenuItem.Click += new System.EventHandler(this.DefaultCopyToolStripMenuItem_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1247, 880);
            this.Controls.Add(this.TabControlMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "QASE (Quick Autoscale enabler)";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.TabControlMain.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabAbout.ResumeLayout(false);
            this.tabAbout.PerformLayout();
            this.contextMenuStripTreeView.ResumeLayout(false);
            this.contextMenuStripTreeViewDefault.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabControl TabControlMain;
        private System.Windows.Forms.TabPage tabMain;
        private System.Windows.Forms.TreeView treeviewMain;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTreeView;
        private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deselectAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem selectInverseToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveSettingsToolStripMenuItem;
        private System.Windows.Forms.TabPage tabAbout;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStripTreeViewDefault;
        private System.Windows.Forms.ToolStripMenuItem copyStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
    }
}

