﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Wpf.Exceptions
{
    /// <summary>
    /// Represents errors that occur when converted does not support target type.
    /// </summary>
    public class UnsupportedConversionException : Exception
    {

        public UnsupportedConversionException() { }

        public UnsupportedConversionException(string message) : base(message) { }

        public UnsupportedConversionException(string message, Exception innerException) : base(message, innerException) { }

    }
}
