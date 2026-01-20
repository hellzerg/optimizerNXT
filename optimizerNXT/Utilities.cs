using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using static optimizerNXT.Enums;

namespace optimizerNXT {
    internal static class Utilities {
        internal static WindowsVersion CurrentWindowsVersion { get; set; }

        public static void GetSystemDetails()
        {
            string productName = GetOS(out string build, out string displayVersion);
            string bitness = Environment.Is64BitOperatingSystem ? "64-bit" : "32-bit";
            bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);

            Logger.Info($"-------------------------------------------------------------");
            Logger.Info($"OptimizerNXT: v{Program.CurrentVersion}");
            Logger.Info($"The finest Windows Optimizer CLI");
            Logger.Info($"Author: deadmoon © ∞");
            Logger.Info($"GitHub: {Program.GitHubProjectUrl}");
            Logger.Info($"Donate: {Program.DonateUrl}");
            Logger.Info($"Windows: {productName} ({bitness})");
            Logger.Info($"Build Number: {build} ({displayVersion})");
            Logger.Info($"Running as admin: {isAdmin}");
            Logger.Info($"-------------------------------------------------------------");
            if (IsUpdateAvailable())
            {
                Logger.Warn($"Update is available: v{Program.LatestVersion}");
                Logger.Warn($"Consider updating");
                Logger.Info($"-------------------------------------------------------------");
            }
        }

        private static string GetOS(out string buildNumber, out string displayVersion)
        {
            const string baseKey = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion";

            string productName = (string)Registry.GetValue(baseKey, "ProductName", "Unknown");
            buildNumber = (string)Registry.GetValue(baseKey, "CurrentBuild", "");
            displayVersion = (string)Registry.GetValue(baseKey, "DisplayVersion", "");

            int build = 0;
            int.TryParse(buildNumber, out build);

            if (productName.Contains("Server"))
            {
                if (productName.Contains("2008"))
                    CurrentWindowsVersion = WindowsVersion.Windows7;
                else if (productName.Contains("2012"))
                    CurrentWindowsVersion = WindowsVersion.Windows8;
                else if (productName.Contains("2016"))
                    CurrentWindowsVersion = WindowsVersion.Windows10;
                else if (productName.Contains("2019"))
                    CurrentWindowsVersion = WindowsVersion.Windows10;
                else if (productName.Contains("2022"))
                    CurrentWindowsVersion = WindowsVersion.Windows11;
                else if (productName.Contains("2025"))
                    CurrentWindowsVersion = WindowsVersion.Windows11;
                else
                    CurrentWindowsVersion = WindowsVersion.Unknown;

                return productName;
            }

            if (productName.Contains("Windows 7"))
            {
                CurrentWindowsVersion = WindowsVersion.Windows7;
            }
            else if (productName.Contains("Windows 8"))
            {
                CurrentWindowsVersion = WindowsVersion.Windows8;
            }
            else if (productName.Contains("Windows 10"))
            {
                if (build >= 22000)
                {
                    productName = productName.Replace("Windows 10", "Windows 11");
                    CurrentWindowsVersion = WindowsVersion.Windows11;
                }
                else
                {
                    CurrentWindowsVersion = WindowsVersion.Windows10;
                }
            }
            else
            {
                CurrentWindowsVersion = WindowsVersion.Unknown;
            }

            return productName;
        }

        internal static bool IsUpdateAvailable()
        {
            var client = new WebClient
            {
                Encoding = Encoding.UTF8
            };
            try
            {
                Program.LatestVersion = client.DownloadString(Program.GithubVersionUrl).Trim();
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to check for updates", ex);
                return false;
            }

            var currentParts = Program.CurrentVersion.Split('.');
            var latestParts = Program.LatestVersion.Split('.');

            for (int i = 0; i < 3; i++)
            {
                int currentPart = int.Parse(currentParts[i]);
                int latestPart = int.Parse(latestParts[i]);

                if (latestPart > currentPart) return true;
                if (currentPart > latestPart) return false;
            }

            return false;
        }

        internal static void TryDeleteRegistryValue(bool localMachine, string path, string valueName)
        {
            try
            {
                if (localMachine) Registry.LocalMachine.OpenSubKey(path, true).DeleteValue(valueName, false);
                if (!localMachine) Registry.CurrentUser.OpenSubKey(path, true).DeleteValue(valueName, false);
            }
            catch { }
        }

        internal static SecurityIdentifier RetrieveCurrentUserIdentifier()
           => WindowsIdentity.GetCurrent().User ?? throw new Exception("Unable to retrieve current user SID.");

        internal static void TakeOwnershipOnSubKey(this RegistryKey registryKey, string subkeyName)
        {
            using (RegistryKey subKey = registryKey.OpenSubKeyWritable(subkeyName, RegistryRights.TakeOwnership))
            {
                RegistrySecurity accessRules = subKey.GetAccessControl();
                accessRules.SetOwner(RetrieveCurrentUserIdentifier());
                subKey.SetAccessControl(accessRules);
            }
        }

        internal static void GrantFullControlOnSubKey(this RegistryKey registryKey, string subkeyName)
        {
            using (RegistryKey subKey = registryKey.OpenSubKeyWritable(subkeyName,
                RegistryRights.TakeOwnership | RegistryRights.ChangePermissions
            ))
            {
                if (subKey is null)
                {
                    return;
                }
                RegistrySecurity accessRules = subKey.GetAccessControl();
                SecurityIdentifier currentUser = RetrieveCurrentUserIdentifier();
                accessRules.SetOwner(currentUser);
                accessRules.ResetAccessRule(
                    new RegistryAccessRule(
                        currentUser,
                        RegistryRights.FullControl,
                        InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                        PropagationFlags.None,
                        AccessControlType.Allow
                    )
                );
                subKey.SetAccessControl(accessRules);
            }
        }

        internal static RegistryKey OpenSubKeyWritable(this RegistryKey registryKey, string subkeyName, RegistryRights? rights = null)
        {
            RegistryKey subKey;

            if (rights == null)
                subKey = registryKey.OpenSubKey(subkeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            else
                subKey = registryKey.OpenSubKey(subkeyName, RegistryKeyPermissionCheck.ReadWriteSubTree, rights.Value);

            if (subKey is null)
            {
                Logger.Warn($"Subkey not found: {subkeyName}");
            }

            return subKey;
        }

        internal static void RunCommand(string command)
        {
            if (string.IsNullOrEmpty(command)) return;

            using (Process p = new Process())
            {
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/C " + command;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = false;

                p.Start();
                p.WaitForExit();
            }
        }

        internal static int RunPowershell(string command)
        {
            using (var process = new Process())
            {
                process.StartInfo.FileName = "powershell.exe";
                process.StartInfo.Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();
                process.WaitForExit();

                return process.ExitCode;
            }
        }
    }
}
