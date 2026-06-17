using System;
using System.Media;

namespace CyberSecurityChatbot
{
    class VoiceGreeting
    {
        public static void PlayGreeting()
        {
            try
            {
                SoundPlayer player = new SoundPlayer("voiceGreeting.wav");
                player.PlaySync();
            }
            catch (Exception)
            {
                Console.WriteLine("Voice greeting could not be played.");
            }
        }
    }
}