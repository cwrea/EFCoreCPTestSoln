// Copyright (c) 2018 Chris W. Rea. All rights reserved.
// Licensed under the Apache License, Version 2.0. See Licenses.txt in the solution root for license information.
//
// This runner is based on the sample console test runner from the xUnit.net samples repository:
//   - Source: https://github.com/xunit/samples.xunit/blob/master/TestRunner/Program.cs 
//   - License: https://github.com/xunit/xunit/blob/master/license.txt
//

using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using EFCoreCPTest.Support;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Runners;

namespace EFCoreCPTest
{
    public class Program
    {
        public static int Main()
        {
            AppCommon.SetUpMain(new PlatformHelper());
            Logger = AppCommon.ServiceProvider.GetRequiredService<ILogger>();

            var programName = Path.GetFileNameWithoutExtension(typeof(Program).Assembly.Location);
            WriteLine($"{programName} on {RuntimeInformation.FrameworkDescription} ({(IntPtr.Size == 8 ? 64 : 32)}-bit)");

            var programDir = Path.GetDirectoryName(typeof(Program).Assembly.Location) ?? "";
            var configFileName = Path.Combine(programDir, "xunit.runner.json");
            var testAssembly = typeof(AppCommon).Assembly.Location;

            // Using Activator.CreateInstance() below to construct the AssemblyRunner as its WithoutAppDomain() factory method doesn't
            // allow for passing a configFileName like the WithAppDomain() method; but WithAppDomain() isn't available to .NET Core.
            using (var runner = (AssemblyRunner) Activator.CreateInstance(typeof(AssemblyRunner),
                BindingFlags.NonPublic | BindingFlags.Instance,
                null, new object[] {AppDomainSupport.IfAvailable, testAssembly, configFileName, true, null}, null))
            {
                runner.OnDiscoveryComplete = OnDiscoveryComplete;
                runner.OnTestPassed = OnTestPassed; // comment out to see just failed & skipped
                runner.OnTestFailed = OnTestFailed;
                runner.OnTestSkipped = OnTestSkipped;
                runner.OnExecutionComplete = OnExecutionComplete;

                WriteLine($"Discovering tests to run in {Path.GetFileName(testAssembly)}.");
                runner.Start();
                Finished.WaitOne();
                Finished.Dispose();
            }
            Console.ResetColor();
            Console.WriteLine("Press [Enter]");
            Console.ReadLine();
            return _mainExitCode;
        }

        private static void OnDiscoveryComplete(DiscoveryCompleteInfo info)
        {
            lock (ConsoleLock)
            {
                _discoveryInfo = info;
                WriteLine($"Running {info.TestCasesToRun} test cases of {info.TestCasesDiscovered} discovered:");
            }
        }

        private static void OnTestPassed(TestPassedInfo info)
        {
            lock (ConsoleLock)
            {
                ++_testsRun;
                Console.ForegroundColor = PassColor;
                Console.Write("."); // terse console output for a passed test with detail going to log only
                WriteLineLogOnly($"... {_testsRun,3}/{_discoveryInfo.TestCasesToRun} [PASS] {info.TestDisplayName}");
            }
        }

        private static void OnTestFailed(TestFailedInfo info)
        {
            lock (ConsoleLock)
            {
                ++_testsRun;
                Console.ForegroundColor = FailColor;
                WriteLineError(
                    $"\n... {_testsRun,3}/{_discoveryInfo.TestCasesToRun} [FAIL] {info.TestDisplayName}: {info.ExceptionMessage}");
                if (info.ExceptionStackTrace != null)
                {
                    Console.ForegroundColor = StackTraceColor;
                    WriteLineError(info.ExceptionStackTrace + "\n");
                }
            }

            _mainExitCode = 1;
        }

        private static void OnTestSkipped(TestSkippedInfo info)
        {
            lock (ConsoleLock)
            {
                ++_testsRun;
                Console.ForegroundColor = SkipColor;
                Console.Write("s"); // terse console output for a skipped test with detail going to log only
                WriteLineLogOnly(
                    $"... {_testsRun,3}/{_discoveryInfo.TestCasesToRun} [SKIP] {info.TestDisplayName} reason: {info.SkipReason}");
            }
        }

        private static void OnExecutionComplete(ExecutionCompleteInfo info)
        {
            lock (ConsoleLock)
            {
                Console.ForegroundColor = info.TestsFailed > 0 ? FailColor : (info.TestsSkipped > 0 ? SkipColor : AllPassColor);
                var passed = info.TotalTests - info.TestsFailed - info.TestsSkipped;
                WriteLine(
                    $"Finished: {info.TotalTests} tests in {Math.Round(info.ExecutionTime, 3)}s ({passed} passed, {info.TestsFailed} failed, {info.TestsSkipped} skipped)");
            }
            Finished.Set();
        }

        private static void WriteLine(string format, params object[] args)
        {
            Logger.LogInformation(format, args);
            Console.WriteLine(format, args);
        }

        private static void WriteLineLogOnly(string format, params object[] args)
        {
            Logger.LogInformation(format, args);
        }

        private static void WriteLineError(string format, params object[] args)
        {
            Logger.LogError(format, args);
            Console.WriteLine(format, args);
        }

        private static ILogger Logger { get; set; }

        private const ConsoleColor FailColor = ConsoleColor.Red;
        private const ConsoleColor PassColor = ConsoleColor.DarkGreen;
        private const ConsoleColor AllPassColor = ConsoleColor.Green;
        private const ConsoleColor SkipColor = ConsoleColor.Yellow;
        private const ConsoleColor StackTraceColor = ConsoleColor.Gray;

        // As messages can arrive in parallel, use a lock to make sure we get consistent console output.
        private static readonly object ConsoleLock = new object();

        // A running count of the number of tests run so far.
        private static int _testsRun;

        // For reporting the total number of tests while displaying each running test.
        private static DiscoveryCompleteInfo _discoveryInfo;

        // Use an event to know when we're done.
        private static readonly ManualResetEvent Finished = new ManualResetEvent(false);

        // Start out assuming success; we'll set this to 1 if we get a failed test.
        private static int _mainExitCode;
    }

    public sealed class PlatformHelper : PlatformHelperBase
    {
        public override string PlatformName => "NETFull";
    }
}