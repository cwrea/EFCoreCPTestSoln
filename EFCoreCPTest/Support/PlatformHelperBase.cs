// Copyright (c) 2018 Chris W. Rea. All rights reserved.
// Licensed under the Apache License, Version 2.0. See Licenses.txt in the solution root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EFCoreCPTest.Support
{
    public interface IPlatformHelper
    {
        string PlatformName { get; }
        string DbFolderPath { get; }
        string LogFolderPath { get; }
        void InstallDatabaseResources();
        void CopyPackagedDbToDbFolder(IList<string> potentialSourceFolders, string dbFileName, string dbFolderPath);
    }

    public abstract class PlatformHelperBase : IPlatformHelper
    {
        protected ILogger Logger => _logger.Value;

        public abstract string PlatformName { get; }

        public virtual string DbFolderPath => _defaultFolderPath.Value;

        public virtual string LogFolderPath => DbFolderPath;

        public virtual void InstallDatabaseResources()
        {
            Logger.LogTrace("PlatformHelperBase.SetUpDatabaseFolderForTests() called");

            var potentialSourceFolders = new List<string>();

            var initialDir = Directory.GetCurrentDirectory();
            Logger.LogInformation("Initial directory: {0}", initialDir);
            potentialSourceFolders.Add(initialDir);

            var thisAssemblyLocation = typeof(AppCommon).Assembly.Location;
            if (thisAssemblyLocation.Length > 0)
            {
                var programDir = Path.GetDirectoryName(thisAssemblyLocation);
                potentialSourceFolders.Add(programDir);
                Logger.LogInformation("Program directory: {0}", programDir);
            }

            Logger.LogInformation("DbFolderPath for this platform: {0}", DbFolderPath);

            Logger.LogInformation("Setting current directory to DB folder.");
            Directory.SetCurrentDirectory(DbFolderPath);

            var dbFileName = "northwind.db";
            if (!File.Exists(dbFileName))
            {
                Logger.LogInformation("{0} doesn't exist in the DB folder. Need to copy.", dbFileName);
                CopyPackagedDbToDbFolder(potentialSourceFolders, dbFileName, DbFolderPath);
            }

            Logger.LogTrace("PlatformHelperBase.SetUpDatabaseFolderForTests() returning");
        }

        public virtual void CopyPackagedDbToDbFolder(IList<string> potentialSourceFolders, string dbFileName, string dbFolderPath)
        {
            var copied = false;
            var destDbFullPath = Path.Combine(dbFolderPath, dbFileName);
            foreach (var potentialSourceFolder in potentialSourceFolders)
            {
                var sourceDbFullPath = Path.Combine(potentialSourceFolder, dbFileName);
                if (File.Exists(sourceDbFullPath))
                {
                    Logger.LogInformation("Copying {0} from {1} to DB folder.", dbFileName, potentialSourceFolder);
                    File.Copy(sourceDbFullPath, destDbFullPath);
                    copied = true;
                    break;
                }
                Logger.LogInformation("Didn't find {0} in folder {1}.", dbFileName, potentialSourceFolder);
            }
            if (!copied)
            {
                throw new Exception(string.Format("Unable to copy {0} from potential source folders to database folder.", dbFileName));
            }
        }

        private readonly Lazy<string> _defaultFolderPath = new Lazy<string>(() =>
            Path.GetDirectoryName(typeof(PlatformHelperBase).Assembly.Location) + Path.DirectorySeparatorChar);

        private readonly Lazy<ILogger> _logger = new Lazy<ILogger>(() => AppCommon.ServiceProvider.GetRequiredService<ILogger>());
    }
}