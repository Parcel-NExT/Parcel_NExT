using NAudio.Wave;

namespace Zora.DomainSpecific.Music
{
    /// <summary>
    /// A C#/Pure/Parcel-friendly function class
    /// </summary>
    public static class ProceduralMusic
    {
        #region Configurations
        private static string FontFilePath { get; set; }
        #endregion

        #region Methods
        public static void ConfigureFontFile(string path)
            => FontFilePath = path;
        public static WaveOutEvent? PlaySimpleSnippet(string soundFontFilePath, out int duration, string snippet = "[C C G G] [A A G/2] [F F E E] [D D C/2]")
        {
            return new Synth(soundFontFilePath).Play(snippet, out duration);
        }
        public static WaveOutEvent? PlayMediaFile(string soundFontFilePath, string inputFilePath, out int duration)
        {
            Console.WriteLine($"Play {Path.GetFileNameWithoutExtension(inputFilePath)}...");
            switch (Path.GetExtension(inputFilePath))
            {
                case ".fs":
                    return new Synth(soundFontFilePath).Play(File.ReadAllText(inputFilePath), out duration);
                case ".mid":
                    return new Synth(soundFontFilePath).PlayMIDIFile(inputFilePath, out duration);
                default:
                    duration = 0;
                    return null;
            }
        }
        #endregion
    }
}
