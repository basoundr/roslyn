// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.
#r "./../../Roslyn.Test.Performance.Utilities.dll"

// IsVerbose()
#load "../util/test_util.csx"
// RunFile()
#load "../util/runner_util.csx"

using System.IO;
using Roslyn.Test.Performance.Utilities;

var directoryUtil = new RelativeDirectory();
var logger = new ConsoleAndFileLogger();
TestUtilities.InitUtilities();

// Update the Open repository
string branch = TestUtilities.StdoutFrom("git", IsVerbose(), logger, "rev-parse --abbrev-ref HEAD");
TestUtilities.ShellOutVital("git", $"pull origin {branch}", IsVerbose(), logger);

// Init.cmd belongs to the closed part of Roslyn.
var closedRoslynDirectory = Path.Combine(directoryUtil.RoslynDirectory, "..");
var initCmdPath = Path.Combine(closedRoslynDirectory, "Init.cmd");
if (File.Exists(initCmdPath))
{
    // Update and build Closed and Open Roslyn
    string closedBranch = TestUtilities.ShellOutVital("git", IsVerbose(), logger, "rev-parse --abbrev-ref HEAD", workingDirectory: closedRoslynDirectory);
    TestUtilities.ShellOutVital("git", $"pull origin {branch}", IsVerbose(), logger, workingDirectory: closedRoslynDirectory);
    TestUtilities.ShellOutVital(initCmdPath, "", IsVerbose(), logger, workingDirectory: closedRoslynDirectory);
    TestUtilities.ShellOutVital("msbuild", "./Roslyn.sln /p:Configuration=Release", IsVerbose(), logger, workingDirectory: closedRoslynDirectory);

    // Install the Vsixes to RoslynPerf hive
    await RunFile(Path.Combine(directoryUtil.MyWorkingDirectory, "install_vsixes.csx"));
}
else
{
    // Update and build Open Roslyn
    TestUtilities.ShellOutVital(Path.Combine(directoryUtil.RoslynDirectory, "Restore.cmd"), "", IsVerbose(), logger, workingDirectory: directoryUtil.RoslynDirectory);
    TestUtilities.ShellOutVital("msbuild", "./Roslyn.sln /p:Configuration=Release", IsVerbose(), logger, workingDirectory: directoryUtil.RoslynDirectory);
}

// Run run_and_report.csx
await RunFile(Path.Combine(directoryUtil.MyWorkingDirectory, "run_and_report.csx"));
