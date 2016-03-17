#load "./util/test_util.csx"
#load "./util/DownloadCPC_util.csx"

using System.IO;

InitUtilities();

// Copy CPC binaries from the share to the local machine
DownloadCPC();

// ShellOutVital("msbuild", "./Roslyn.sln /p:Configuration=Release", workingDirectory: RoslynDirectory());

//string from = BinReleaseDirectory();
//string to = BootStrapedBinariesDirectory();
// System.IO.Directory.Move(from, to);
