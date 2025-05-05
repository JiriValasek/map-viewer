using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapViewer.Core.ViewModels
{
    public abstract class WindowViewModel : BaseViewModel, IDisposable
    {
        /// <summary>
        /// Optional title overwrite for the window's title.
        /// </summary>
        public abstract string? Title { get; }

        public abstract void Dispose();
    }
}
