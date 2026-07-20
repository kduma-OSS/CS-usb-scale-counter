using Microsoft.Win32;
using ScaleCounter.Core;
using System;
using System.IO;
using System.Windows.Forms;

namespace USBScaleCounter
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            TryRegisterFileAssociation();

            // If launched with a preset file (e.g. double-clicked), import it before the UI opens.
            foreach (var arg in args)
            {
                if (!string.IsNullOrEmpty(arg)
                    && arg.EndsWith(PresetFile.Extension, StringComparison.OrdinalIgnoreCase)
                    && File.Exists(arg))
                {
                    try { new PresetStore().ImportFile(arg); }
                    catch { /* ignore a bad file; the app still opens */ }
                }
            }

            Application.Run(new MainForm());
        }

        /// <summary>
        /// Best-effort per-user association of the ".uscpreset" extension with this app, so a
        /// double-clicked preset file opens and imports it. Uses HKCU (no admin needed).
        /// </summary>
        private static void TryRegisterFileAssociation()
        {
            try
            {
                var exePath = Application.ExecutablePath;
                const string progId = "USBScaleCounter.Preset";

                using (var ext = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + PresetFile.Extension))
                    ext?.SetValue("", progId);

                using (var cls = Registry.CurrentUser.CreateSubKey(@"Software\Classes\" + progId))
                {
                    cls?.SetValue("", PresetFile.Description);
                    using (var icon = cls?.CreateSubKey("DefaultIcon"))
                        icon?.SetValue("", "\"" + exePath + "\",0");
                    using (var cmd = cls?.CreateSubKey(@"shell\open\command"))
                        cmd?.SetValue("", "\"" + exePath + "\" \"%1\"");
                }
            }
            catch
            {
                // Association is a convenience, not a requirement.
            }
        }
    }
}
