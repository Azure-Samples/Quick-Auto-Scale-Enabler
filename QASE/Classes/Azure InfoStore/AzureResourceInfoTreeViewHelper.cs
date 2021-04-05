// <copyright file="AzureResourceInfoTreeViewHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE.Classes
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    /// <summary>
    /// Class to help view/change settings of Azure resources (scoped to autoscale settings)
    /// </summary>
    public class AzureResourceInfoTreeViewHelper
    {
        /// <summary>
        /// TreeViewContainersRootName
        /// </summary>
        public const string TreeViewContainersRootName = "Containers";

        /// <summary>
        /// TVIFSTATE
        /// </summary>
        private const int TVIFSTATE = 0x8;

        /// <summary>
        /// TVISSTATEIMAGEMASK
        /// </summary>
        private const int TVISSTATEIMAGEMASK = 0xF000;

        /// <summary>
        /// TVFIRST
        /// </summary>
        private const int TVFIRST = 0x1100;

        /// <summary>
        /// TVMSETITEM
        /// </summary>
        private const int TVMSETITEM = TVFIRST + 63;

        /// <summary>
        /// TreeViewSubscriptionsRootName
        /// </summary>
        private const string TreeViewSubscriptionsRootName = "Available Subscriptions";

        /// <summary>
        /// TreeViewResourceGroupsRootName
        /// </summary>
        private const string TreeViewResourceGroupsRootName = "ResourceGroups";

        /// <summary>
        /// TreeViewCosmosDbAccountsRootName
        /// </summary>
        private const string TreeViewCosmosDbAccountsRootName = "CosmosDbAccounts";

        /// <summary>
        /// TreeViewDatabasesRootName
        /// </summary>
        private const string TreeViewDatabasesRootName = "Databases";

        private const string TreeViewSubscriptionsPathSeparator = @"\";
        private const string TreeViewDelimiter = ",";
        private const string TreeViewRootSentinel = "R@";

        private readonly TreeView mainTreeView;
        private readonly AzureResourceInfoStore azureResourceInfoStore;

        /// <summary>
        /// Default max autoscale throughput
        /// </summary>
        private readonly int defaultMaxAutoScaleThroughput;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureResourceInfoTreeViewHelper"/> class.
        /// </summary>
        /// <param name="treeView">treeView</param>
        /// <param name="azureResourceInfoStore">azureResourceInfoStore</param>
        public AzureResourceInfoTreeViewHelper(TreeView treeView, AzureResourceInfoStore azureResourceInfoStore)
        {
            // Read and set default max throughput settings.
            this.defaultMaxAutoScaleThroughput = Common.DefaultMaxAutosscaleThroughput();

            this.mainTreeView = treeView ?? throw new ArgumentNullException(nameof(treeView));
            this.azureResourceInfoStore = azureResourceInfoStore ?? throw new ArgumentNullException(nameof(azureResourceInfoStore));

            this.mainTreeView.NodeMouseDoubleClick += this.TreeMainAfterDoubleClick;
            this.mainTreeView.DrawMode = TreeViewDrawMode.OwnerDrawText;
            this.mainTreeView.DrawNode += new DrawTreeNodeEventHandler(this.TreeviewDrawNode);

            this.mainTreeView.Nodes.Add(AzureResourceInfoTreeViewHelper.TreeViewSubscriptionsRootName, AzureResourceInfoTreeViewHelper.TreeViewSubscriptionsRootName);
            this.mainTreeView.PathSeparator = AzureResourceInfoTreeViewHelper.TreeViewSubscriptionsPathSeparator;
            this.RemoveCheckBoxes(this.mainTreeView);
        }

        /// <summary>
        /// Saves autoscale settings of all container nodes given the parent CosmosDb Database's node key.
        /// </summary>
        /// <param name="parentContainerKey">parentContainerKey</param>
        public void SaveAutoScaleChangesInContainers(string parentContainerKey)
        {
            TreeNode[] containerParent = this.mainTreeView.Nodes.Find(parentContainerKey, true);
            if (containerParent.Length != 0)
            {
                List<Tuple<string, bool>> azureContainerToUpdateWithNewSettings = new List<Tuple<string, bool>>();
                List<TreeNode> treeNodesToUpdate = new List<TreeNode>();

                foreach (TreeNode node in containerParent[0].Nodes)
                {
                    if (node.Checked != ((Tuple<bool, int?>)node.Tag).Item1)
                    {
                        azureContainerToUpdateWithNewSettings.Add(new Tuple<string, bool>(node.Text, node.Checked));
                        treeNodesToUpdate.Add(node);
                    }
                }

                if (azureContainerToUpdateWithNewSettings.Count > 0)
                {
                    var containersToDisplay = string.Join(Environment.NewLine, azureContainerToUpdateWithNewSettings);
                    if (MessageBox.Show(string.Format("Are you sure you want to change auto-scale settings for the following containers?{0}{1}", Environment.NewLine, containersToDisplay), "Confirmation", MessageBoxButtons.YesNo) != DialogResult.Yes)
                    {
                        return;
                    }
                    else
                    {
                        string input = Microsoft.VisualBasic.Interaction.InputBox("Enter new Autoscale max / throughput. (Default shown)", "Max Autoscale / throughput entry", this.defaultMaxAutoScaleThroughput.ToString());
                        var isInteger = int.TryParse(input, out int userEnteredDefaultMaxAutoscaleThroughput);

                        if (!isInteger)
                        {
                            MessageBox.Show("Invalid Max Autoscale Throughput, aborting update");
                            return;
                        }

                        var cosmosDbDatabaseName = this.mainTreeView.SelectedNode.Parent.Parent.Text;
                        var azureCosmosDbAccountInfoOfContainer = (AzureCosmosDbAccountInfo)this.mainTreeView.SelectedNode.Parent.Parent.Parent.Parent.Tag;
                        var azureResourceGroupInfoOfContainer = (AzureResourceGroupInfo)this.mainTreeView.SelectedNode.Parent.Parent.Parent.Parent.Parent.Parent.Tag;

                        string unencryptedPrimaryKey = new System.Net.NetworkCredential(string.Empty, azureCosmosDbAccountInfoOfContainer.PrimaryKey).Password;

                        foreach (var azureCosmosDbContainer in azureContainerToUpdateWithNewSettings)
                        {
                            var containerName = azureCosmosDbContainer.Item1;
                            bool newState = azureCosmosDbContainer.Item2;

                            var nodeKeyName = string.Join(
                                AzureResourceInfoTreeViewHelper.TreeViewDelimiter,
                                AzureResourceInfoTreeViewHelper.TreeViewRootSentinel,
                                AzureResourceInfoTreeViewHelper.TreeViewContainersRootName,
                                azureResourceGroupInfoOfContainer.Name,
                                azureCosmosDbAccountInfoOfContainer.Name,
                                cosmosDbDatabaseName);

                            ThroughputManagerHelper.SetAutoScaleSetting(
                                    azureCosmosDbAccountInfoOfContainer.DocumentEndpoint,
                                    cosmosDbDatabaseName,
                                    containerName,
                                    unencryptedPrimaryKey,
                                    newState,
                                    userEnteredDefaultMaxAutoscaleThroughput);

                            this.SearchAndModifyCheckBox(nodeKeyName, newState);

                            foreach (var node in treeNodesToUpdate)
                            {
                                var currentTag = (Tuple<bool, int?>)node.Tag;
                                Tuple<bool, int?> newTag = new Tuple<bool, int?>(newState, currentTag.Item2);
                                node.Tag = newTag;
                            }
                        }

                        MessageBox.Show("Update complete.");
                    }
                }
                else
                {
                    MessageBox.Show("No autoscale changes to save.");
                }
            }
        }

        /// <summary>
        /// Loads Azure subscriptions that are available to the user.
        /// </summary>
        /// <param name="resetTree">Clears tree nodes if true</param>
        public void LoadAvailableSubscriptionsIntoTreeView(bool resetTree = true)
        {
            if (resetTree)
            {
                this.mainTreeView.Nodes.Clear();
                this.mainTreeView.BeginUpdate();
                this.mainTreeView.Nodes.Add(AzureResourceInfoTreeViewHelper.TreeViewSubscriptionsRootName, AzureResourceInfoTreeViewHelper.TreeViewSubscriptionsRootName);
                this.mainTreeView.EndUpdate();
            }

            var availableSubscriptions = this.azureResourceInfoStore.RetrieveAvailableAzureSubscriptions();

            this.mainTreeView.BeginUpdate();
            foreach (var subscription in availableSubscriptions)
            {
                this.SearchAndAdd(TreeViewSubscriptionsRootName, string.Empty, subscription.Name, subscription);
            }

            this.SearchAndExpand(TreeViewSubscriptionsRootName);
            this.mainTreeView.Nodes[0].EnsureVisible();
            this.mainTreeView.EndUpdate();
        }

        /// <summary>
        /// SendMessage
        /// </summary>
        /// <param name="hWnd">hWnd</param>
        /// <param name="msg">Msg</param>
        /// <param name="wParam">wParam</param>
        /// <param name="lParam">lParam</param>
        /// <returns>IntPtr</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        private void RemoveCheckBoxes(TreeView tree)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            foreach (TreeNode n in tree.Nodes)
            {
                if (n.Nodes.Count > 0)
                {
                    nodes.AddRange(this.GetNodes(n));
                }
            }

            foreach (TreeNode n in nodes)
            {
                TVITEM tvi = new TVITEM
                {
                    HItem = n.Handle,
                    Mask = TVIFSTATE,
                    SSateMask = TVISSTATEIMAGEMASK,
                    State = 0
                };

                IntPtr lparam = Marshal.AllocHGlobal(Marshal.SizeOf(tvi));
                Marshal.StructureToPtr(tvi, lparam, false);
                SendMessage(this.mainTreeView.Handle, TVMSETITEM, IntPtr.Zero, lparam);
            }
        }

        private void TreeviewDrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            if (e.Node.Level == 0 || (e.Node.Parent != null && e.Node.Parent.Text != AzureResourceInfoTreeViewHelper.TreeViewContainersRootName))
            {
                this.HideCheckBox(e.Node);
                e.DrawDefault = true;
            }
            else
            {
                e.Graphics.DrawString(e.Node.Text, e.Node.TreeView.Font, Brushes.Black, e.Node.Bounds.X, e.Node.Bounds.Y);
            }
        }

        private List<TreeNode> GetNodes(TreeNode node)
        {
            List<TreeNode> nodes = new List<TreeNode>();
            if (node.Nodes.Count > 0)
            {
                nodes.Add(node);
            }

            foreach (TreeNode n in node.Nodes)
            {
                if (node.Nodes.Count > 0)
                {
                    nodes.AddRange(this.GetNodes(n));
                }
            }

            return nodes;
        }

        private void HideCheckBox(TreeNode node)
        {
            TVITEM tvi = new TVITEM
            {
                HItem = node.Handle,
                Mask = TVIFSTATE,
                SSateMask = TVISSTATEIMAGEMASK,
                State = 0
            };

            IntPtr lparam = Marshal.AllocHGlobal(Marshal.SizeOf(tvi));
            Marshal.StructureToPtr(tvi, lparam, false);
            SendMessage(node.TreeView.Handle, TVMSETITEM, IntPtr.Zero, lparam);
        }

        private void SearchAndAdd(string searchKey, string newNodeKey, string newNodeValue, object tag)
        {
            TreeNode[] list = this.mainTreeView.Nodes.Find(searchKey, true);
            if (list.Length != 0)
            {
                var addedNode = list[0].Nodes.Add(newNodeKey, newNodeValue);
                addedNode.Tag = tag;
            }
        }

        private void SearchAndModifyCheckBox(string searchKey, bool newState)
        {
            TreeNode[] list = this.mainTreeView.Nodes.Find(searchKey, true);
            if (list.Length != 0)
            {
                list[0].Checked = newState;
            }
        }

        private void SearchAndExpand(string searchKey)
        {
            TreeNode[] list = this.mainTreeView.Nodes.Find(searchKey, true);
            if (list.Length != 0)
            {
                list[0].Expand();
            }
        }

        private void TreeMainAfterDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node.Text == AzureResourceInfoTreeViewHelper.TreeViewSubscriptionsRootName)
            {
                if (e.Node.Nodes.Count == 0)
                {
                    this.LoadAvailableSubscriptionsIntoTreeView(false);
                    return;
                }
            }

            if (e.Node.Parent == null || e.Node.Name.StartsWith(AzureResourceInfoTreeViewHelper.TreeViewDelimiter))
            {
                return;
            }

            switch (e.Node.Parent.Text)
            {
                // ResourceGroup-list of selected subscription expanded.
                case AzureResourceInfoTreeViewHelper.TreeViewSubscriptionsRootName:
                    {
                        if (e.Node.Nodes.Count == 0)
                        {
                            var subscriptionInfoOfResourceGroup = (AzureSubscriptionInfo)e.Node.Tag;

                            var azureResouceGroupsInfo = this.azureResourceInfoStore.RetrieveAzureResourceGroupInfoUnderAzureSubscription(subscriptionInfoOfResourceGroup.Name);

                            var nodeKeyName = string.Join(
                                AzureResourceInfoTreeViewHelper.TreeViewDelimiter,
                                AzureResourceInfoTreeViewHelper.TreeViewRootSentinel,
                                AzureResourceInfoTreeViewHelper.TreeViewResourceGroupsRootName,
                                subscriptionInfoOfResourceGroup.Name);

                            this.mainTreeView.BeginUpdate();

                            e.Node.Nodes.Add(
                                nodeKeyName,
                                AzureResourceInfoTreeViewHelper.TreeViewResourceGroupsRootName);

                            foreach (var azureResouceGroupInfo in azureResouceGroupsInfo)
                            {
                                var childNodeKeyName = nodeKeyName + azureResouceGroupInfo.Name;
                                this.SearchAndAdd(nodeKeyName, childNodeKeyName, azureResouceGroupInfo.Name, azureResouceGroupInfo);
                            }

                            e.Node.Expand();
                            this.SearchAndExpand(nodeKeyName);
                            e.Node.EnsureVisible();

                            this.RemoveCheckBoxes(this.mainTreeView);
                            this.mainTreeView.EndUpdate();
                        }

                        break;
                    }

                // CosmosDbAccount-list of selected Resource group expanded
                case AzureResourceInfoTreeViewHelper.TreeViewResourceGroupsRootName:
                    {
                        if (e.Node.Nodes.Count == 0)
                        {
                            var azureResourceGroupInfoOfContainer = (AzureResourceGroupInfo)e.Node.Tag;
                            var azureSubscriptionInfoOFContainer = (AzureSubscriptionInfo)e.Node.Parent.Parent.Tag;

                            var nodeKeyName = string.Join(
                                AzureResourceInfoTreeViewHelper.TreeViewDelimiter,
                                AzureResourceInfoTreeViewHelper.TreeViewRootSentinel,
                                AzureResourceInfoTreeViewHelper.TreeViewCosmosDbAccountsRootName,
                                azureResourceGroupInfoOfContainer.Name);

                            var azureCosmosDbAccounts = this.azureResourceInfoStore.RetrieveAzureCosmosDbAccountInfoUnderAzureResourceGroup(azureResourceGroupInfoOfContainer.Name, azureSubscriptionInfoOFContainer.Name);
                            this.mainTreeView.BeginUpdate();

                            e.Node.Nodes.Add(
                                nodeKeyName,
                                AzureResourceInfoTreeViewHelper.TreeViewCosmosDbAccountsRootName);

                            foreach (var azureCosmosDbAccount in azureCosmosDbAccounts)
                            {
                                var childNodeKeyName = nodeKeyName + azureCosmosDbAccount.Name;
                                this.SearchAndAdd(nodeKeyName, childNodeKeyName, azureCosmosDbAccount.Name, azureCosmosDbAccount);
                            }

                            e.Node.Expand();
                            this.SearchAndExpand(nodeKeyName);
                            e.Node.EnsureVisible();

                            this.RemoveCheckBoxes(this.mainTreeView);
                            this.mainTreeView.EndUpdate();
                        }

                        break;
                    }

                // CosmosDbDatabase-list of selected CosmosDb account expanded.
                case AzureResourceInfoTreeViewHelper.TreeViewCosmosDbAccountsRootName:
                    {
                        if (e.Node.Nodes.Count == 0)
                        {
                            var azureCosmosDbAccountInfoOfCosmosDbDatabase = (AzureCosmosDbAccountInfo)e.Node.Tag;
                            var azureResourceGroupInfoOfCosmosDbDatabase = (AzureResourceGroupInfo)e.Node.Parent.Parent.Tag;
                            var subscriptionInfoOfCosmosDbDatabase = (AzureSubscriptionInfo)e.Node.Parent.Parent.Parent.Parent.Tag;

                            var nodeKeyName = string.Join(
                                AzureResourceInfoTreeViewHelper.TreeViewDelimiter,
                                AzureResourceInfoTreeViewHelper.TreeViewRootSentinel,
                                AzureResourceInfoTreeViewHelper.TreeViewDatabasesRootName,
                                azureResourceGroupInfoOfCosmosDbDatabase.Name,
                                azureCosmosDbAccountInfoOfCosmosDbDatabase.Name);

                            var azureCosmosDbDatabases = this.azureResourceInfoStore.RetrieveCosmosDbDatabasesUnderCosmosDbAccount(azureCosmosDbAccountInfoOfCosmosDbDatabase.Name, azureResourceGroupInfoOfCosmosDbDatabase.Name, subscriptionInfoOfCosmosDbDatabase.Name);
                            this.mainTreeView.BeginUpdate();

                            e.Node.Nodes.Add(
                                nodeKeyName,
                                AzureResourceInfoTreeViewHelper.TreeViewDatabasesRootName);

                            foreach (var azureCosmosDbDatabase in azureCosmosDbDatabases)
                            {
                                var childNodeKeyName = nodeKeyName + azureCosmosDbDatabase;
                                this.SearchAndAdd(nodeKeyName, childNodeKeyName, azureCosmosDbDatabase, azureCosmosDbDatabase);
                            }

                            e.Node.Expand();
                            this.SearchAndExpand(nodeKeyName);
                            e.Node.EnsureVisible();

                            this.RemoveCheckBoxes(this.mainTreeView);
                            this.mainTreeView.EndUpdate();
                        }

                        break;
                    }

                // Container-list of selected CosmosDb Database expanded.
                case AzureResourceInfoTreeViewHelper.TreeViewDatabasesRootName:
                    {
                        if (e.Node.Nodes.Count == 0)
                        {
                            var cosmosDbDatabaseName = e.Node.Text;
                            var azureCosmosDbAccountInfoOfContainer = (AzureCosmosDbAccountInfo)e.Node.Parent.Parent.Tag;
                            var azureResourceGroupInfoOfContainer = (AzureResourceGroupInfo)e.Node.Parent.Parent.Parent.Parent.Tag;
                            var azureSubscriptionInfoOfContainer = (AzureSubscriptionInfo)e.Node.Parent.Parent.Parent.Parent.Parent.Parent.Tag;

                            var nodeKeyName = string.Join(
                                AzureResourceInfoTreeViewHelper.TreeViewDelimiter,
                                AzureResourceInfoTreeViewHelper.TreeViewRootSentinel,
                                AzureResourceInfoTreeViewHelper.TreeViewContainersRootName,
                                azureResourceGroupInfoOfContainer.Name,
                                azureCosmosDbAccountInfoOfContainer.Name,
                                cosmosDbDatabaseName);

                            var azureCosmosDbContainers = this.azureResourceInfoStore.RetrieveCosmosDbContainersUnderCosmosDbDatabase(
                                cosmosDbDatabaseName,
                                azureCosmosDbAccountInfoOfContainer.Name,
                                azureResourceGroupInfoOfContainer.Name,
                                azureSubscriptionInfoOfContainer.Name);

                            this.mainTreeView.BeginUpdate();

                            e.Node.Nodes.Add(
                                nodeKeyName,
                                AzureResourceInfoTreeViewHelper.TreeViewContainersRootName);

                            // Add child node for each container
                            foreach (var azureCosmosDbContainer in azureCosmosDbContainers)
                            {
                                var childNodeKeyName = nodeKeyName + azureCosmosDbContainer;

                                // Get autoscale setting for the added-container.
                                string unencryptedPrimaryKey = new System.Net.NetworkCredential(string.Empty, azureCosmosDbAccountInfoOfContainer.PrimaryKey).Password;

                                var isAutoScaleTurnedOnForContainer = ThroughputManagerHelper.GetAutoScaleSetting(
                                        azureCosmosDbAccountInfoOfContainer.DocumentEndpoint,
                                        cosmosDbDatabaseName,
                                        azureCosmosDbContainer,
                                        unencryptedPrimaryKey);

                                this.SearchAndAdd(nodeKeyName, childNodeKeyName, azureCosmosDbContainer, isAutoScaleTurnedOnForContainer);

                                this.SearchAndModifyCheckBox(
                                    childNodeKeyName,
                                    isAutoScaleTurnedOnForContainer.Item1);
                            }

                            e.Node.Expand();
                            this.SearchAndExpand(nodeKeyName);
                            e.Node.EnsureVisible();

                            this.RemoveCheckBoxes(this.mainTreeView);
                            this.mainTreeView.EndUpdate();
                        }

                        break;
                    }
            }
        }

        /// <summary>
        /// For intercepting default checkboxes being drawn.
        /// </summary>
        private struct TVITEM
        {
            /// <summary>
            /// Mask
            /// </summary>
            public int Mask;

            /// <summary>
            /// HItem
            /// </summary>
            public IntPtr HItem;

            /// <summary>
            /// State
            /// </summary>
            public int State;

            /// <summary>
            /// sSateMask
            /// </summary>
            public int SSateMask;
            ////[MarshalAs(UnmanagedType.LPTStr)]
        }
    }
}
