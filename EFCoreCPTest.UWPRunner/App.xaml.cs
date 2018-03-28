using System;
using System.IO;
using Windows.Storage;
using EFCoreCPTest.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit.Runners.UI;

namespace EFCoreCPTest.UWPRunner
{
    public sealed partial class App : RunnerApplication
    {
        protected override void OnInitializeRunner()
        {
            AppCommon.SetUpMain(new UwpPlatformHelper());
            var logger = AppCommon.ServiceProvider.GetRequiredService<ILogger>();

            logger.LogTrace("App.OnInitializeRunner() called (just after logger created)");

            // xUnit will test these assemblies.
            AddTestAssembly(typeof(AppCommon).Assembly);

            // xUnit should automatically start all the tests
            AutoStart = true;

            logger.LogTrace("App.OnInitializeRunner() returning");
        }
    }

    public sealed class UwpPlatformHelper : PlatformHelperBase
    {
        public override string PlatformName => "UWP";

        public override string DbFolderPath => _dbFolderPath.Value;

        private readonly Lazy<string> _dbFolderPath =
            new Lazy<string>(() => ApplicationData.Current.LocalFolder.Path + Path.DirectorySeparatorChar);
    }
}