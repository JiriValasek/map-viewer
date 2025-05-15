using MapViewer.Core.Exceptions;
using MapViewer.Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MapViewer.Core.Utils
{
    /// <summary>
    /// Utility functions for working with map files.
    /// </summary>
    public static class MapFileUtils
    {
        private static readonly Regex COLUMN_COUNT_REGEX = new("ncols\\s+(?<value>\\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ROW_COUNT_REGEX = new("nrows\\s+(?<value>\\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex X_LL_CORNER_REGEX = new("xllcorner\\s+(?<value>\\d+\\.?\\d*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex Y_LL_CORNER_REGEX = new("yllcorner\\s+(?<value>\\d+\\.?\\d*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CELL_SIZE_REGEX = new("cellsize\\s+(?<value>\\d+\\.?\\d*)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex ALTITUDE_REGEX = new("(\\s+(?<value>\\S+))+", RegexOptions.Compiled | RegexOptions.ExplicitCapture);

        /// <summary>
        /// Parse ESRI like map files based on format from the assignment.
        /// </summary>
        /// <param name="filepath">Path of the map file.</param>
        /// <returns>Map data model.</returns>
        /// <exception cref="MapFileException">
        /// Thrown when <paramref name="filepath">map file</paramref> does not conform to the prescribed format.
        /// </exception>
        public static async Task<MapData> LoadMapAsync(string filepath)
        {
            int altitudeLineN = 0, altitudeColumnN;
            try
            {
                using StreamReader reader = new(filepath, Encoding.ASCII);

                // parse column count
                string line = await reader.ReadLineAsync() ?? "";
                Match match = COLUMN_COUNT_REGEX.Match(line);
                if (!match.Success)
                {
                    throw new MapFileException(String.Format("Column count parsing failed, it should match {0}.",COLUMN_COUNT_REGEX), 1);
                }
                if (! Int32.TryParse(match.Groups["value"].Value, out Int32 columnCount))
                {
                    throw new MapFileException("Unable to parse column count as a 32-bit integer.", 1);
                }

                // parse row count
                line = await reader.ReadLineAsync() ?? "";
                match = ROW_COUNT_REGEX.Match(line);
                if (!match.Success)
                {
                    throw new MapFileException(String.Format("Row count parsing failed, it should match {0}.", ROW_COUNT_REGEX), 2);
                }
                if (!Int32.TryParse(match.Groups["value"].Value, out Int32 rowCount))
                {
                    throw new MapFileException("Unable to parse row count as a 32-bit integer.", 2);
                }

                // parse xllcorner
                line = await reader.ReadLineAsync() ?? "";
                match = X_LL_CORNER_REGEX.Match(line);
                if (!match.Success)
                {
                    throw new MapFileException(String.Format("Lower left corner's X coordinate parsing failed, it should match {0}.", X_LL_CORNER_REGEX), 3);
                }
                if (!float.TryParse(match.Groups["value"].Value, CultureInfo.InvariantCulture, out float xLLCorner))
                {
                    throw new MapFileException("Unable to parse lower left corner's X coordinate as a float.", 3);
                }

                // parse yllcorner
                line = await reader.ReadLineAsync() ?? "";
                match = Y_LL_CORNER_REGEX.Match(line);
                if (!match.Success)
                {
                    throw new MapFileException(String.Format("Lower left corner's Y coordinate parsing failed, it should match {0}.", Y_LL_CORNER_REGEX), 4);
                }
                if (!float.TryParse(match.Groups["value"].Value, CultureInfo.InvariantCulture, out float yLLCorner))
                {
                    throw new MapFileException("Unable to parse lower left corner's Y coordinate as a float.", 4);
                }

                // parse cellSize
                line = await reader.ReadLineAsync() ?? "";
                match = CELL_SIZE_REGEX.Match(line);
                if (!match.Success)
                {
                    throw new MapFileException(String.Format("Cell size parsing failed, it should match {0}.", CELL_SIZE_REGEX), 5);
                }
                if (!float.TryParse(match.Groups["value"].Value, CultureInfo.InvariantCulture, out float cellSize))
                {
                    throw new MapFileException("Unable to parse cell size as a float.", 5);
                }

                // parse altitude
                Int32[,] altitude = new Int32[rowCount, columnCount];
                for (altitudeLineN = 0; altitudeLineN < rowCount; altitudeLineN++)
                {
                    line = await reader.ReadLineAsync() ?? throw new MapFileException(String.Format("Missing altitude rows."), 6 + altitudeLineN); ;
                    match = ALTITUDE_REGEX.Match(line);
                    if (!match.Success)
                    {
                        throw new MapFileException(String.Format("Altitude row parsing failed, it should match {0}.", ALTITUDE_REGEX), 6 + altitudeLineN);
                    }
                    try
                    {
                        for (altitudeColumnN = 0; altitudeColumnN < columnCount; altitudeColumnN++)
                        {
                            if (!Int32.TryParse(match.Groups["value"].Captures[altitudeColumnN].Value, out altitude[altitudeLineN, altitudeColumnN]))
                            {
                                throw new MapFileException(String.Format("Unable to parse altitude value number {0} a 32-bit integer.", altitudeColumnN), 6 + altitudeLineN);
                            }
                        }
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        throw new MapFileException(String.Format("Missing altitude columns."), e, 6 + altitudeLineN);
                    }
                }

            return new MapData(filepath, columnCount, rowCount, xLLCorner, yLLCorner, cellSize, altitude);

            }
            catch (IOException e)
            {
                // TODO Put logging here
                Debug.WriteLine("The file could not be read:");
                Debug.WriteLine(e.Message);
                throw;
            }
            catch (IndexOutOfRangeException e)
            {
                // TODO put logging here
                Debug.WriteLine("Not Enough altitude columns in a row:");
                Debug.WriteLine(e.Message);
                throw;
            }
        }

        /// <summary>
        /// Serialize loaded map data to a file.
        /// </summary>
        /// <param name="filepath">Path to the file to be created.</param>
        /// <param name="map">Loaded map data.</param>
        public static async Task SaveMapAsync(string filepath, MapData map)
        {
            using StreamWriter writer = new(filepath);
            await writer.WriteLineAsync(String.Format("ncols        {0}", map.ColumnCount));
            await writer.WriteLineAsync(String.Format("nrows        {0}", map.RowCount));
            await writer.WriteLineAsync(String.Format(CultureInfo.InvariantCulture, "xllcorner    {0:F12}", map.XLLCorner));
            await writer.WriteLineAsync(String.Format(CultureInfo.InvariantCulture, "yllcorner    {0:F12}", map.YLLCorner));
            await writer.WriteLineAsync(String.Format(CultureInfo.InvariantCulture, "cellsize     {0:F12}", map.CellSize));
            for (int i = 0; i < map.Altitude.GetLength(0); i++)
            {
                for (int j = 0; j < map.Altitude.GetLength(1); j++)
                {
                    await writer.WriteAsync(" " + map.Altitude[i, j]);
                }
                await writer.WriteAsync("\n");
            }
        }
    }
}