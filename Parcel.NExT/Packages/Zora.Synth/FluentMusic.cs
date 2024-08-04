using FluentSynth;
using NAudio.Wave;

namespace Zora.DomainSpecific.Music
{
    public struct CommandLineOptions
    {
        public string SoundFontFile { get; set; }
    }
    /// <summary>
    /// Provides a command line based interface for conversion, playback and REPL authorization of music.
    /// </summary>
    public static class FluentMusic
    {
        #region Main
        public static void Main(string[] args)
        {
            if (args.Length == 0 || args.FirstOrDefault() == "--help")
                PrintHelp();
            else if (args.Length == 1)
            {
                string soundFontFilePath = Path.GetFullPath(args.First());
                if (File.Exists(soundFontFilePath) && Path.GetExtension(soundFontFilePath) == ".sf2")
                    REPL(soundFontFilePath);
            }
            else if (args.Length == 2)
            {
                string soundFontFilePath = Path.GetFullPath(args.First());
                string inputFilePath = Path.GetFullPath(args.Last());
                if (File.Exists(soundFontFilePath) && Path.GetExtension(soundFontFilePath) == ".sf2")
                {
                    CurrentPlaying = ProceduralMusic.PlayMediaFile(soundFontFilePath, inputFilePath, out int duration);
                    Thread.Sleep(duration);
                }
            }
        }
        #endregion

        #region Routines
        private static WaveOutEvent CurrentPlaying;
        private static void REPL(string soundFontFilePath)
        {
            Console.WriteLine("""
                Welcoe to Fluent Music, powered by Fluent Synth.
                ? Start typing notes or enter complete scores for playback. Type `exit` to quit.
                - Use `sample` to play some sample music.
                - Use `save <File Path>` to save history to a file.
                - Use `stop` to stop last playback.
                - Use `play <File Path>` to play from file.
                """);

            string fileName = Path.GetFileNameWithoutExtension(soundFontFilePath);
            string extension = Path.GetExtension(soundFontFilePath).TrimStart('.').ToUpper();
            Console.WriteLine($"Now playing using: {fileName} ({extension})");
            List<string> history = [];
            while (true)
            {
                Console.Write("> ");
                string? input = Console.ReadLine();

                if (input == "exit")
                    break;
                else if (input == "sample")
                    CurrentPlaying = new Synth(soundFontFilePath).Play("[C C G G] [A A G/2] [F F E E] [D D C/2]", out _);
                else if (input == "stop")
                    CurrentPlaying?.Stop();
                else if (input.StartsWith("save "))
                    File.WriteAllLines(input["save ".Length..].Trim().Trim('"'), history);
                else if (input.StartsWith("play "))
                    CurrentPlaying = ProceduralMusic.PlayMediaFile(soundFontFilePath, input["play ".Length..].Trim().Trim('"'), out _);
                else
                {
                    // Play melodies
                    try
                    {
                        history.Add(input);
                        new Synth(soundFontFilePath).Play(input, out _);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }
            }
        }
        public static void PrintHelp()
        {
            Console.WriteLine("""
                Fluent Music v0.1 Play music using score.
                --help: Print this help information.
                <Sound Font File>: Start REPL.
                """);
        }
        #endregion
    }
}