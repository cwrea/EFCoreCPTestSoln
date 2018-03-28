// Copyright (c) 2018 Chris W. Rea. All rights reserved.
// Licensed under the Apache License, Version 2.0. See Licenses.txt in the solution root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Android.App;
using Android.OS;
using EFCoreCPTest.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Runners.UI;
using Xunit.Sdk;
using Environment = System.Environment;

namespace EFCoreCPTest.AndroidRunner
{
    [Activity(Label = "EFCoreCPTest", MainLauncher = true, Theme = "@android:style/Theme.Material.Light")]
    public class MainActivity : RunnerActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            AppCommon.SetUpMain(new AndroidPlatformHelper());
            Logger = AppCommon.ServiceProvider.GetRequiredService<ILogger>();

            Logger.LogTrace("MainActivity.OnCreate() called (just after logger created)");

            // Override xUnit default Writer with own StreamWriter to capture xUnit results to file.
            var platformHelper = AppCommon.ServiceProvider.GetRequiredService<IPlatformHelper>();
            var resultsFilePath = Path.Combine(platformHelper.LogFolderPath, "_xUnit_Results_Android.log");
            Logger.LogInformation("xUnit results will be written to on-device file: {0}", resultsFilePath);
            Writer = new StreamWriter(resultsFilePath);

            // xUnit needs this to ensure the execution assembly is part of the app bundle
            AddExecutionAssembly(typeof(ExtensibilityPointFactory).Assembly);

            // xUnit will test these assemblies.
            AddTestAssembly(typeof(AppCommon).Assembly);

            // xUnit should automatically start all the tests
            AutoStart = true;

            Logger.LogTrace("Calling base.OnCreate(bundle)");
            base.OnCreate(bundle);
            Logger.LogTrace("After calling base.OnCreate(bundle)");

            Logger.LogTrace("MainActivity.OnCreate() returning");
        }

        private ILogger Logger { get; set; }
    }

    public sealed class AndroidPlatformHelper : PlatformHelperBase
    {
        public override string PlatformName => "Android";

        public override string DbFolderPath => _dbFolderPath.Value;

        public override string LogFolderPath => _logFolderPath.Value;

        public override void CopyPackagedDbToDbFolder(IList<string> potentialSourceFolders, string dbFileName, string dbFolderPath)
        {
            // Android override doesn't reference potentialSourceFolders, but Android app assets instead.
            Logger.LogInformation("Copying {0} from application assets to DB folder.", dbFileName);
            var dbPath = Path.Combine(dbFolderPath, dbFileName);
            using (var br = new BinaryReader(Application.Context.Assets.Open(dbFileName)))
            using (var bw = new BinaryWriter(new FileStream(dbPath, FileMode.Create)))
            {
                var buffer = new byte[4096];
                int len;
                while ((len = br.Read(buffer, 0, buffer.Length)) > 0) { bw.Write(buffer, 0, len); }
            }
        }

        private readonly Lazy<string> _dbFolderPath = new Lazy<string>(() =>
            Environment.GetFolderPath(Environment.SpecialFolder.Personal) + Path.DirectorySeparatorChar);

        private readonly Lazy<string> _logFolderPath = new Lazy<string>(() =>
            Application.Context.GetExternalFilesDir(Android.OS.Environment.DirectoryDownloads).AbsolutePath); // can adb pull from here
    }
}