using System;
using System.Diagnostics;
using Microsoft.Win32;

namespace ProxyToggleExtension
{
    internal sealed class ProxySettingsController
    {
        private void CopyProxySettings(bool enable = true)
        {
            const string subKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Internet Settings\\Connections";
            const string sourceRoot = "HKEY_CURRENT_USER";
            const string targetRoot = "HKEY_USERS\\S-1-5-18";

            const string sourceKey = sourceRoot + "\\" + subKey;
            const string targetKey = targetRoot + "\\" + subKey;

            const string parameter = "DefaultConnectionSettings";

            if (Registry.GetValue(sourceKey, parameter, null) is byte[] userSettings)
            {
                userSettings[8] = enable ? (byte)3 : (byte)1;

                Registry.SetValue(targetKey, parameter, userSettings);
            }
        }

        public void EnableSystemProxy()
        {
            CopyProxySettings();

            var psi = new ProcessStartInfo(Environment.SystemDirectory + @"\netsh.exe", "winhttp import proxy source=ie")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            var process = Process.Start(psi);
            process?.WaitForExit(10 * 1000);
        }

        public void DisableSystemProxy()
        {
            CopyProxySettings(false);

            var psi = new ProcessStartInfo(Environment.SystemDirectory + @"\netsh.exe", "winhttp reset proxy")
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };

            var process = Process.Start(psi);
            process?.WaitForExit(10 * 1000);
        }
    }
}
