using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;

namespace optimizerNXT {
    internal class Program {
        internal const string CurrentVersion = "1.0.0";
        internal static string LatestVersion = string.Empty;
        internal static Mutex Mutex;
        internal const string GitHubProjectUrl = "https://github.com/hellzerg/optimizerNXT";
        internal const string DonateUrl = "https://www.paypal.com/paypalme/supportoptimizer";
        internal const string GithubVersionUrl = "https://raw.githubusercontent.com/hellzerg/optimizerNXT/refs/heads/main/version.txt";

        const string MUTEX_GUID = @"{DEADMOON-0EFC7B8A-D1FC-467F-0117C643FE19-OPTIMIZER-NXT}";
        static bool _cliNotRunning;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            Mutex = new Mutex(true, MUTEX_GUID, out _cliNotRunning);
            if (!_cliNotRunning)
            {
                Logger.Info("Another instance of ΟptimizerΝΧΤ is already running.");
                Console.ReadKey();
                Environment.Exit(0);
                return;
            }

            Utilities.GetSystemDetails();

            var yamlAssembly = "optimizerNXT.Resources.YamlDotNet.dll";
            EmbeddedAssembly.Load(yamlAssembly, yamlAssembly.Replace("optimizerNXT.", string.Empty));

            string fileOrFolder = string.Empty;

            if (args.Length < 2 || args[0].ToLowerInvariant() != "apply")
            {
                Logger.Info("Provide a YAML file or a folder containing YAML files:");
                fileOrFolder = Console.ReadLine();
            }
            else
            {
                fileOrFolder = args[1];
            }

            if (string.IsNullOrWhiteSpace(fileOrFolder))
            {
                Logger.Info("Usage: optimizerNXT apply <file>|<folder>");
                Console.ReadKey();
                return;
            }

            var yamlFiles = ParseCommandAndGetYaml(fileOrFolder);
            var verifiedYamlFiles = new List<string>();
            var invalidYamlFiles = new List<string>();
            foreach (var file in yamlFiles)
            {
                if (YamlVerifier.Verify(file))
                    verifiedYamlFiles.Add(file);
                else
                    invalidYamlFiles.Add(file);
            }

            Logger.Info($"Verified {verifiedYamlFiles.Count}/{yamlFiles.Length} YAML files.");
            if (invalidYamlFiles.Count > 0)
            {
                foreach (var x in invalidYamlFiles)
                {
                    Logger.Error("Invalid signature in YAML file: " + x);
                }
                Logger.Warn("Ensure all YAML files are verified for security reasons.");
                Logger.Info("Exiting.");
                Console.ReadKey();
                Environment.Exit(0);
                return;
            }

            var validationResult = Engine.ValidateStage(yamlFiles);
            if (!validationResult.IsValid)
            {
                foreach (var result in validationResult.Errors)
                {
                    Logger.Warn($"Syntax error in {result.File}:");
                    Logger.Warn(result.Message);
                }

                Logger.Warn("Ensure all YAML files are syntactically correct.");
                Logger.Info("Exiting.");
                Console.ReadKey();
                Environment.Exit(0);
                return;
            }

            foreach (var stage in validationResult.Items)
            {

                Logger.Info($"Started executing YAML stage: {stage.Jobs[0].Name}");
                Engine.ExecuteStage(stage);
                Logger.Info($"Finished executing YAML stage: {stage.Jobs[0].Name}");
            }
            Logger.Info("Exiting.");
            Console.ReadKey();
        }

        private static string[] ParseCommandAndGetYaml(string fileOrFolder)
        {
            if (File.Exists(fileOrFolder))
            {
                Logger.Info($"Parsing YAML: {fileOrFolder}");
                return new string[] { fileOrFolder };
            }
            else if (Directory.Exists(fileOrFolder))
            {
                var yamlFiles = Directory
                    .GetFiles(fileOrFolder, "*.yaml")
                    .Concat(Directory.GetFiles(fileOrFolder, "*.yml"))
                    .ToArray();
                Logger.Info($"Parsing directory with {yamlFiles.Length} YAML: {fileOrFolder}");
                return yamlFiles;
            }
            else
            {
                Console.Out.WriteLine("Invalid directory or YAML file.");
                Console.Out.WriteLine("Usage: optimizerNXT apply <file>|<folder>");
                Console.ReadKey();
                Environment.Exit(0);
                return new string[0];
            }
        }

        private static System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
            => EmbeddedAssembly.Get(args.Name);

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
            => Logger.Error("Unhandled exception has occured: ", e.ExceptionObject as Exception);
    }
}
