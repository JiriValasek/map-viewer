using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Core.Exceptions
{

    /// <summary>
    /// Represents errors that occur during a map file parsing.
    /// </summary>
    public class MapFileException : Exception
    {
        public int OffendingLine { get; }

        public MapFileException(int offendingLine)
        {
            OffendingLine = offendingLine;
        }

        public MapFileException(string message, int offendingLine) : base(message)
        {
            OffendingLine = offendingLine;
        }

        public MapFileException(string message, Exception innerException, int offendingLine) : base(message, innerException)
        {
            OffendingLine = offendingLine;
        }

    }
}
