// <copyright file="FormMain.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE
{
    using System;
    using System.Windows.Forms;
    using QASE.Classes;

    /// <summary>
    /// Main form.
    /// </summary>
    public partial class FormMain : Form
    {
        private readonly AzureResourceInfoTreeViewHelper azureResourceInfoTreeViewHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormMain"/> class.
        /// </summary>
        public FormMain()
        {
            this.InitializeComponent();
            var azureResourceInfoStore = new AzureResourceInfoStore();
            this.azureResourceInfoTreeViewHelper = new AzureResourceInfoTreeViewHelper(this.treeviewMain, azureResourceInfoStore);
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
        }

        private void TreeviewMain_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                this.treeviewMain.SelectedNode = this.treeviewMain.GetNodeAt(e.X, e.Y);

                if (this.treeviewMain.SelectedNode != null &&
                    this.treeviewMain.SelectedNode.Parent != null &&
                    this.treeviewMain.SelectedNode.Parent.Text == AzureResourceInfoTreeViewHelper.TreeViewContainersRootName)
                {
                    this.contextMenuStripTreeView.Show(this.treeviewMain, e.Location);
                }
                else if (this.treeviewMain.SelectedNode != null)
                {
                    this.contextMenuStripTreeViewDefault.Show(this.treeviewMain, e.Location);
                }
            }
        }

        private void SelectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeviewMain.SelectedNode.Parent != null)
            {
                foreach (TreeNode node in this.treeviewMain.SelectedNode.Parent.Nodes)
                {
                    node.Checked = true;
                }
            }
        }

        private void DeselectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeviewMain.SelectedNode.Parent != null)
            {
                foreach (TreeNode node in this.treeviewMain.SelectedNode.Parent.Nodes)
                {
                    node.Checked = false;
                }
            }
        }

        private void SelectInverseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeviewMain.SelectedNode.Parent != null)
            {
                foreach (TreeNode node in this.treeviewMain.SelectedNode.Parent.Nodes)
                {
                    node.Checked = !node.Checked;
                }
            }
        }

        private void SaveSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeviewMain.SelectedNode.Parent != null)
            {
                this.azureResourceInfoTreeViewHelper.SaveAutoScaleChangesInContainers(this.treeviewMain.SelectedNode.Parent.Name);
            }
        }

        private void DefaultCopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeviewMain.SelectedNode != null)
            {
                System.Windows.Forms.Clipboard.SetText(this.treeviewMain.SelectedNode.Text);
            }
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.treeviewMain.SelectedNode != null)
            {
                System.Windows.Forms.Clipboard.SetText(this.treeviewMain.SelectedNode.Text);
            }
        }
    }
}