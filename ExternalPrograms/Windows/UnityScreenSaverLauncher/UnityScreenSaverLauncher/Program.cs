using Microsoft.Win32;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UnityScreenSaverLauncher
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static int Main(string[] args)
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Show config window from context menu
            if (args.Length == 0)
            {
                return ShowConfig(nint.Zero);
            }

            var firstArg = args[0].ToUpper();

            // Show config window from screen saver settings
            if (firstArg.StartsWith("/C"))
            {
                var hWnd = nint.Parse(firstArg.StartsWith("/C:") ? firstArg[3..] : args[1]);
                return ShowConfig(hWnd);
            }

            // Show preview
            if (firstArg.StartsWith("/P"))
            {
                var hWndStr = firstArg.StartsWith("/P:") ? firstArg[3..] : args[1];
                var hWnd = nint.Parse(hWndStr);

                SetPreviewModeToPlayerPrefs(true);

                var parentWindow = NativeWindow.FromHandle(hWnd);
                return UnityMain(Process.GetCurrentProcess().Handle, nint.Zero,
                    $"-parentHWND {hWndStr}", 5);
            }

            // Start screen saver
            if (firstArg == "/S") {
                SetPreviewModeToPlayerPrefs(false);
                var unityArgs = "-screen-fullscreen 1 -window-mode borderless";
                if (Screen.PrimaryScreen is Screen primaryScreen)
                {
                    unityArgs = $"{unityArgs} -screen-width {primaryScreen.Bounds.Width} -screen-height {primaryScreen.Bounds.Height}";
                }
                return UnityMain(Process.GetCurrentProcess().Handle, nint.Zero,
                    unityArgs, 1);
            }

            return ShowConfig(nint.Zero);
        }

        /// <summary>
        /// Show a config UI.
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        static int ShowConfig(nint hWnd)
        {
            MessageBox.Show(NativeWindow.FromHandle(hWnd),
                "This screen saver has no options that you can set.", "Unity Screen Saver");
            return 0;
        }

        /// <summary>
        /// Set whether this is preview or not.
        /// </summary>
        /// <param name="isPreview">true when this is preview</param>
        static void SetPreviewModeToPlayerPrefs(bool isPreview)
        {
            // Change the registry key by your Unity setting
            string regKey = @"Software\TkoolerLufar\ScreenSaverByUnity";
            var playerPrefs = Registry.CurrentUser.OpenSubKey(regKey, true);
            if (playerPrefs is null)
            {
                playerPrefs = Registry.CurrentUser.CreateSubKey(regKey, true);
            }
            try
            {
                playerPrefs.SetValue("isPreview", isPreview ? 1 : 0, RegistryValueKind.DWord);
            }
            finally
            {
                playerPrefs.Close();
            }
        }

        /// <summary>
        /// The entry point to Unity.
        /// </summary>
        /// <param name="hInstance">Instance of the parent process.</param>
        /// <param name="hPrevInstance">Unused since Win32.</param>
        /// <param name="lpCmdLine">Command line arguments.</param>
        /// <param name="nShowCmd">Prefered window mode.</param>
        /// <returns>The exit code.</returns>
        [DllImport("UnityPlayer.dll", CallingConvention = CallingConvention.StdCall)]
        extern static int UnityMain(IntPtr hInstance, IntPtr hPrevInstance, [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, int nShowCmd);
    }
}
