﻿using System;
using System.Diagnostics;
using Microsoft.Web.Deployment;

namespace WAWSDeploy
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                WriteLine(@"Syntax 1: WAWSDeploy.exe c:\SomeFolder MySite.PublishSettings [/p password] [/d DoNotDelete]");
                WriteLine(@"Syntax 2: WAWSDeploy.exe c:\SomeFile.zip MySite.PublishSettings [/p password]");
                WriteLine(@"Syntax 3: WAWSDeploy.exe c:\SomeFile.zip MySite.PublishSettings [/au]");
                return;
            }

            // parse the command line args
            var command = Args.Configuration.Configure<DeploymentArgs>().CreateAndBind(args);

            try
            {
                var webDeployHelper = new WebDeployHelper();

                webDeployHelper.DeploymentTraceEventHandler += Trace;


                WriteLine("Starting deployment...");
                DeploymentChangeSummary changeSummary = webDeployHelper.DeployContentToOneSite(
                    command.Folder, 
                    command.PublishSettingsFile, 
                    command.Password, 
                    command.AllowUntrusted,
                    !command.DeleteExistingFiles,
                    command.TraceLevel,
                    command.WhatIf

                    );

                WriteLine("BytesCopied: {0}", changeSummary.BytesCopied);
                WriteLine("Added: {0}", changeSummary.ObjectsAdded);
                WriteLine("Updated: {0}", changeSummary.ObjectsUpdated);
                WriteLine("Deleted: {0}", changeSummary.ObjectsDeleted);
                WriteLine("Errors: {0}", changeSummary.Errors);
                WriteLine("Warnings: {0}", changeSummary.Warnings);
                WriteLine("Total changes: {0}", changeSummary.TotalChanges);
            }
            catch (Exception e)
            {
                WriteLine("Deployment failed: {0}", e.Message);
                Environment.ExitCode = 1;
            }
        }

        static void Trace(object sender, DeploymentTraceEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        static void WriteLine(string message, params object[] args)
        {
            Console.WriteLine(String.Format(message, args));
        }
    }
}
