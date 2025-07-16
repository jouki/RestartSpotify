using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        // Najdeme a ukončíme všechny procesy s názvem "Spotify"
        Process[] spotifyProcesses = Process.GetProcessesByName("Spotify");
        if(spotifyProcesses.Length > 0)
        {
            Console.WriteLine("Ukončuji procesy Spotify...");
            foreach(Process process in spotifyProcesses)
            {
                try
                {
                    process.Kill();
                    process.WaitForExit(); // Počkáme, až proces skončí
                    Console.WriteLine($"Proces {process.Id} ukončen.");
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Chyba při ukončování procesu: {ex.Message}");
                }
            }
        }
        else
        {
            Console.WriteLine("Žádný proces Spotify neběží.");
        }

        // Spustíme Spotify - nejprve zkusíme původní cestu pro desktop verzi
        string spotifyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Spotify", "Spotify.exe");
        bool started = false;

        if(File.Exists(spotifyPath))
        {
            Console.WriteLine("Spouštím Spotify z desktop cesty...");
            try
            {
                Process.Start(spotifyPath);
                Console.WriteLine("Spotify byl úspěšně restartován (desktop verze).");
                started = true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Chyba při spuštění z desktop cesty: {ex.Message}. Zkouším alternativu...");
            }
        }
        else
        {
            Console.WriteLine("Desktop cesta nenalezena. Zkouším UWP verzi...");
        }

        // Pokud desktop verze selhala nebo neexistuje, zkusíme UWP přes URI scheme
        if(!started)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "spotify:",
                    UseShellExecute = true
                });
                Console.WriteLine("Spotify byl úspěšně restartován (UWP verze).");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Chyba při spuštění UWP verze: {ex.Message}. Zkontrolujte instalaci Spotify.");
            }
        }

        // Počkáme na stisk klávesy pro ukončení konzole
        //.WriteLine("Stiskněte libovolnou klávesu pro ukončení...");
        //Console.ReadKey();
    }
}