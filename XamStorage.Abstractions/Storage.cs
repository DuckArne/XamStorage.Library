using System;
using System.Collections.Generic;
using System.Text;

namespace XamStorage
{
    /// <summary>
    /// Since Fall creator update UWP doesnt really work with paths. Program needs to know where to fetch the root. 
    /// </summary>
    public enum Storage
    {
        /// <summary>
        /// Documents storage
        /// </summary>
        Documents,
        /// <summary>
        /// Music storage
        /// </summary>
        Music,
        /// <summary>
        /// Personal storage
        /// </summary>
        Personal,
        /// <summary>
        /// Local storage
        /// </summary>
        Local,
        /// <summary>
        /// Roaming storage UWP only
        /// </summary>
        Roaming,
        /// <summary>
        /// Videos storage
        /// </summary>
        Videos,
        /// <summary>
        /// Storage from a path
        /// </summary>
        Path,
        /// <summary>
        /// Picture Storage
        /// </summary>
        Pictures
    }
}
