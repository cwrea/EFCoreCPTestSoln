// Copyright (c) 2018 Chris W. Rea. All rights reserved.
// Licensed under the Apache License, Version 2.0. See Licenses.txt in the solution root for license information.

using System;
using System.IO;
using EFCoreCPTest.Support;
using Foundation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SQLitePCL;
using UIKit;
using Xunit.Runner;
using Xunit.Sdk;

namespace EFCoreCPTest.iOSRunner
{
    public class Application
    {
        public static void Main(string[] args)
        {
            AppCommon.SetUpMain(new IOSPlatformHelper());
            var logger = AppCommon.ServiceProvider.GetRequiredService<ILogger>();

            logger.LogTrace("Application.Main() called (just after logger created)");

            Batteries_V2.Init(); // required on iOS when using EF Core v2

            logger.LogTrace("Calling UIApplication.Main()");
            UIApplication.Main(args, null, "AppDelegate");
            logger.LogTrace("After calling UIApplication.Main()");

            logger.LogTrace("Application.Main() returning");
        }
    }

    [Register("AppDelegate")]
    public class AppDelegate : RunnerAppDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Logger = AppCommon.ServiceProvider.GetRequiredService<ILogger>();
            Logger.LogTrace("AppDelegate.FinishedLaunching() called");

            // Override xUnit default Writer with own StreamWriter to capture xUnit results to file.
            var platformHelper = AppCommon.ServiceProvider.GetRequiredService<IPlatformHelper>();
            var resultsFilePath = Path.Combine(platformHelper.LogFolderPath, "_xUnit_Results_iOS.log");
            Logger.LogInformation("xUnit results will be written to on-device file: {0}", resultsFilePath);
            Writer = new StreamWriter(resultsFilePath);

            // xUnit needs this to ensure the execution assembly is part of the app bundle
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);

            // xUnit will test these assemblies.
            AddTestAssembly(typeof(AppCommon).Assembly);

            // xUnit should automatically start all the tests
            AutoStart = true;

            Logger.LogTrace("AppDelegate.FinishedLaunching() calling base version and returning");
            return base.FinishedLaunching(app, options);
        }

        private ILogger Logger { get; set; }
    }

    public sealed class IOSPlatformHelper : PlatformHelperBase
    {
        public override string PlatformName => "iOS";

        public override string DbFolderPath => _dbFolderPath.Value;

        private readonly Lazy<string> _dbFolderPath = new Lazy<string>(() =>
        {
            var result = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "..", "Library", "Databases");
            if (!Directory.Exists(result)) { Directory.CreateDirectory(result); }
            return result;
        });
    }
}