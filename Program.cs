using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

class Program
{
    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);
    [DllImport("user32.dll")]
    private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
    private const byte VK_MEDIA_PLAY_PAUSE = 0xB3;
    private const byte VK_SPACE = 0x20;
    private const uint KEYEVENTF_KEYUP = 0x0002;
    private const uint WM_CLOSE = 0x0010;

    static void Main(string[] args)
    {
        Process[] spotifyProcesses = Process.GetProcessesByName("Spotify");
        if(spotifyProcesses.Length > 0)
        {
            foreach(Process process in spotifyProcesses)
            {
                try
                {
                    if(process.MainWindowHandle != IntPtr.Zero)
                    {
                        PostMessage(process.MainWindowHandle, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    }
                    process.WaitForExit(5000);
                }
                catch
                {
                }
            }
        }

        string spotifyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spotify", "Spotify.exe");
        bool started = false;

        if(File.Exists(spotifyPath))
        {
            try
            {
                Process.Start(spotifyPath);
                started = true;
            }
            catch
            {
            }
        }

        if(!started)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "spotify:",
                    UseShellExecute = true
                });
                started = true;
            }
            catch
            {
            }
        }

        if(started)
        {
            Thread.Sleep(1000);
            try
            {
                var spotifyProcess = Process.GetProcessesByName("Spotify").FirstOrDefault();
                if(spotifyProcess != null)
                {
                    SetForegroundWindow(spotifyProcess.MainWindowHandle);
                    keybd_event(VK_MEDIA_PLAY_PAUSE, 0, 0, 0);
                    keybd_event(VK_MEDIA_PLAY_PAUSE, 0, KEYEVENTF_KEYUP, 0);
                }
            }
            catch
            {
                try
                {
                    keybd_event(VK_SPACE, 0, 0, 0);
                    keybd_event(VK_SPACE, 0, KEYEVENTF_KEYUP, 0);
                }
                catch
                {
                }
            }
        }
    }
} 