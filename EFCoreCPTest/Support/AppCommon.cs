// Copyright (c) 2018 Chris W. Rea. All rights reserved.
// Licensed under the Apache License, Version 2.0. See Licenses.txt in the solution root for license information.

using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EFCoreCPTest.Support
{
    public static class AppCommon
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public static void SetUpMain(IPlatformHelper platformHelper)
        {
            SetUpServiceProvider(platformHelper);
            platformHelper.InstallDatabaseResources();
        }

        private static void SetUpServiceProvider(IPlatformHelper platformHelper)
        {
            if (platformHelper == null) { throw new ArgumentNullException(nameof(platformHelper)); }

            var logFileLocation = Path.Combine(platformHelper.LogFolderPath,
                string.Format("_EFCoreCPTest_{0}.log", platformHelper.PlatformName));

            var loggerFactory = new LoggerFactory();
            loggerFactory.AddDebug(LogLevel.Trace);
            loggerFactory.AddFile(logFileLocation, append: false);
            var logger = loggerFactory.CreateLogger("EFCoreCPTest");
            logger.LogTrace("AppCommon.SetUpServiceProvider() called and default logger created");

            logger.LogTrace("Creating the ServiceCollection instance");
            var serviceCollection = new ServiceCollection()
                .AddSingleton<ILoggerFactory>(loggerFactory)
                .AddSingleton(logger)
                .AddSingleton(platformHelper)
                .AddLogging();

            logger.LogTrace("Building the ServiceProvider instance");
            ServiceProvider = serviceCollection.BuildServiceProvider();

            logger.LogTrace("AppCommon.SetUpServiceProvider() returning");
        }
    }
}