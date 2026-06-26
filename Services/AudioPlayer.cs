using System;
using System.IO;

namespace CyberSecurityAwarenessBot.Services
{
    public static class AudioPlayer
    {
        public static void PlayGreeting(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    var player = new System.Media.SoundPlayer(filePath);
                    player.Play();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Audio] Could not play greeting: {ex.Message}");
            }
        }
    }
}