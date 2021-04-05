// <copyright file="Program.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using CommandLine;
    using CommandLine.Text;
    using QASE.Classes.CLI;

    /// <summary>
    /// Program
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// <param name="args">args</param>
        [STAThread]
        public static void Main(string[] args)
        {
            // Check for required Azure dependency
            if (!Common.IsApplicationInstalled(Common.AzureCliSoftwareName))
            {
                throw new Exception("Azure CLI is required for QASE to run");
            }

            if (args.Length > 0)
            {
                // Process command line arguments, if any.
                Parser.Default.ParseArguments<GetCliOptions, SetCliOptions>(args)
                .WithParsed<GetCliOptions>(options =>
                {
                    if (options.OrderedAzureContainerNames.Count > 0)
                    {
                        // User has not specified container names. Show all containers under specified database.
                        var orderedListOfContainerNames = options.OrderedAzureContainerNames;

                        var result = CliHelper.GetAutoScaleSettings(
                            options.AzureSubscriptionId,
                            options.AzureResourceGroupName,
                            options.AzureCosmosDbAccountName,
                            options.AzureCosmosDbDatabaseName,
                            orderedListOfContainerNames);

                        Console.WriteLine(
                            string.Format(
                                "\nAzure subscription ID: {0}{1}" +
                                "Azure ResourceGroup Name : {2}{3}" +
                                "Azure CosmosDb account Name : {4}{5}" +
                                "Azure CosmosDb database Name : {6}{7}",
                                options.AzureSubscriptionId,
                                Environment.NewLine,
                                options.AzureResourceGroupName,
                                Environment.NewLine,
                                options.AzureCosmosDbAccountName,
                                Environment.NewLine,
                                options.AzureCosmosDbDatabaseName,
                                Environment.NewLine));

                        if (result.Count == 0)
                        {
                            Console.WriteLine("No containers were found with the specified parameters");
                            Environment.Exit(1);
                        }

                        int index = 0;
                        foreach (var autoscaleSeting in result)
                        {
                            Console.WriteLine(
                                string.Format(
                                    "Container Name: {0}, AutoScale setting: {1}, max throughput {2}",
                                    orderedListOfContainerNames[index++],
                                    autoscaleSeting.Item1,
                                    autoscaleSeting.Item1 ? string.Format("{0} RU/s", autoscaleSeting.Item2) : "Autoscale turned off"));
                        }
                    }
                    else
                    {
                        var result = CliHelper.GetContainersUnderCosmosDbDatabaseAndAccount(
                            options.AzureSubscriptionId,
                            options.AzureResourceGroupName,
                            options.AzureCosmosDbAccountName,
                            options.AzureCosmosDbDatabaseName);

                        foreach (var container in result)
                        {
                            Console.WriteLine(container);
                        }
                    }

                    Environment.Exit(1);
                })
                .WithParsed<SetCliOptions>(options =>
                {
                    var orderedListOfContainerNames = options.OrderedAzureContainerNames;
                    var orderedNewAutoScaleSettings = options.OrderedNewAutoScaleSettings;
                    var orderedNewMaxAutoScaleThroughput = options.OrderedNewMaxAutoScaleThroughput;

                    // Ensure ordered lists have equal number of items.
                    if (
                        new int[]
                        {
                    orderedListOfContainerNames.Count,
                    orderedNewAutoScaleSettings.Count,
                    orderedNewMaxAutoScaleThroughput.Count
                        }.All(value => value == orderedListOfContainerNames.Count) == false)
                    {
                        throw new ArgumentException("Ordered lists of inputs are not equal");
                    }

                    // Amalgamate ordereded input lists
                    IList<Tuple<string, bool, int?>> orderdListOfArgumentsToPass = new List<Tuple<string, bool, int?>>();
                    for (int itr = 0; itr < orderedListOfContainerNames.Count; itr++)
                    {
                        orderdListOfArgumentsToPass.Add(
                            new Tuple<string, bool, int?>(
                                orderedListOfContainerNames[itr],
                                orderedNewAutoScaleSettings[itr],
                                orderedNewMaxAutoScaleThroughput[itr]));
                    }

                    // Set autoscale settings.
                    var result = CliHelper.SetAutoScaleSettings(
                            options.AzureSubscriptionId,
                            options.AzureResourceGroupName,
                            options.AzureCosmosDbAccountName,
                            options.AzureCosmosDbDatabaseName,
                            orderdListOfArgumentsToPass);

                    if (result.Count == 0)
                    {
                        Console.WriteLine("No containers were found with the specified parameters || Unable to update containers (do you need JIT?)");
                        Environment.Exit(1);
                    }

                    Console.WriteLine(
                        string.Format(
                            "Azure subscription ID: {0}{1}" +
                            "Azure ResourceGroup Name : {2}{3}" +
                            "Azure CosmosDb account Name : {4}{5}" +
                            "Azure CosmosDb database Name : {6}{7}",
                            options.AzureSubscriptionId,
                            Environment.NewLine,
                            options.AzureResourceGroupName,
                            Environment.NewLine,
                            options.AzureCosmosDbAccountName,
                            Environment.NewLine,
                            options.AzureCosmosDbDatabaseName,
                            Environment.NewLine));

                    int index = 0;
                    foreach (var autoscaleSeting in result)
                    {
                        Console.WriteLine(
                            string.Format(
                                "Container Name: {0}, AutoScale setting: {1}, max throughput {2}",
                                orderedListOfContainerNames[index++],
                                autoscaleSeting.Item1,
                                autoscaleSeting.Item1 ? string.Format("{0} RU/s", autoscaleSeting.Item2) : "Autoscale turned off"));
                    }

                    Environment.Exit(1);
                })
                .WithNotParsed(errors =>
                {
                    var sentenceBuilder = SentenceBuilder.Create();
                    foreach (var error in errors)
                    {
                        Console.WriteLine(sentenceBuilder.FormatError(error));
                    }

                    Environment.Exit(1);
                });
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FormMain());
            }
        }
    }
}
