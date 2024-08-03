using System.Text;

namespace ProjectNine.Tooling.Generative
{
    /// <summary>
    /// Converts between 12x24 and 24x48 maps. 
    /// </summary>
    /// <remarks>
    /// TODO: Allow applying interpolation when upscaling. This way it's smoother.
    /// </remarks>
    public static class MapConverter
    {
        #region Method
        /// <summary>
        /// Per CLI "Main"
        /// </summary>
        public static void Convert(string[] args)
        {
            string source = args[0];
            string output = args[1];
            bool interpolate = args.Length == 3 ? bool.Parse(args[2]) : false;
            if (!File.Exists(source))
            {
                Console.WriteLine($"File {source} doesn't exist.");
                return;
            }

            Convert(source, output, interpolate);
        }
        #endregion

        public static void Convert(string source, string output, bool interpolate)
        {
            string[] rows = File.ReadAllLines(source);
            if (rows.Length == 12)
                File.WriteAllText(output, Convert12To24(rows, interpolate));
            else if (rows.Length == 24)
                File.WriteAllText(output, Convert24To12(rows));
        }

        public static string Convert24To12(string rawInput)
            => Convert24To12(rawInput.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries));
        public static string Convert24To12(string[] rows)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int row = 0; row < rows.Length; row += 2)
            {
                for (int col = 0; col < rows[row].Length; col += 2)
                {
                    char symbol = rows[row][col];
                    stringBuilder.Append(symbol);
                }
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString().TrimEnd();
        }

        public static string Convert12To24(string rawInput, bool interpolate = false)
            => Convert12To24(rawInput.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries), interpolate);
        public static string Convert12To24(string[] rows, bool interpolate = false)
        {
            if (!interpolate)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (string row in rows)
                {
                    StringBuilder rowBuilder = new StringBuilder();
                    foreach (var col in row)
                    {
                        rowBuilder.Append(col);
                        rowBuilder.Append(col);
                    }
                    stringBuilder.AppendLine(rowBuilder.ToString());
                    stringBuilder.AppendLine(rowBuilder.ToString());
                }
                return stringBuilder.ToString().TrimEnd();
            }
            else
            {
                int inputWidth = 48;
                int inputHeight = 24;
                int[][] baseTerrain = new int[60][/*30*/];
                for (int i = 0; i < 60; i++)
                    baseTerrain[i] = new int[30];

                if (rows.Length != inputHeight / 2 || rows.Any(r => r.Length != inputWidth / 2))
                    throw new ArgumentException("Invalid row/column size.");
                for (int row = 0; row < inputHeight; row += 2)
                {
                    for (int col = 0; col < inputWidth; col += 2)
                    {
                        char symbol = rows[row / 2][col / 2];
                        if (!SymbolMapping.ContainsKey(symbol))
                            Console.WriteLine($"Wrong map symbol: {symbol}");
                        else
                            baseTerrain[col][row] = SymbolMapping[symbol];
                    }
                }
                /* interpolate */
                for (int j = 1; j < inputHeight; j += 2)    // Interpolate rows
                    for (int i = 0; i < inputWidth; i += 2)
                        baseTerrain[i][j] = (baseTerrain[i][j - 1] + baseTerrain[i][(j + 1)]) / 2;
                for (int j = 0; j < inputHeight; j++)
                    for (int i = 1; i < inputWidth; i += 2)    // Interpolate columns
                        baseTerrain[i][j] = (baseTerrain[i - 1][j] + baseTerrain[(i + 1) % inputWidth][j]) / 2;

                StringBuilder stringBuilder = new StringBuilder();
                for (int row = 0; row < inputHeight; row++)
                {
                    for (int col = 0; col < inputWidth; col++)
                    {
                        int height = baseTerrain[col][row];
                        if (!HeightMapping.ContainsKey(height))
                            Console.WriteLine($"Wrong map height: {height}");
                        else
                            stringBuilder.Append(HeightMapping[height]);
                    }
                    stringBuilder.AppendLine();
                }
                return stringBuilder.ToString().TrimEnd();
            }
        }

        private static Dictionary<char, int> SymbolMapping = new()
        {
            { '.', -8 },
            { ',', -6 },
            { ':', -4 },
            { ';', -2 },
            { '-', 0 },
            { '*', 2 },
            { 'o', 4 },
            { 'O', 6 },
            { '@', 8 }
        };
        private static Dictionary<int, char> HeightMapping = new()
        {
            { -8, '.' },
            { -6, ',' },
            { -4, ':' },
            { -2, ';' },
            { 0, '-' },
            { 2, '*' },
            { 4, 'o' },
            { 6, 'O' },
            { 8, '@' }
        };
    }
}
