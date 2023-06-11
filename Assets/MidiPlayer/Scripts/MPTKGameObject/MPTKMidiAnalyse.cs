using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;

namespace MidiPlayerTK
{
    // @cond NODOC
    // Not yet mature to be published.

    /// <summary>
    /// Contains some information about the count of MIDI events.
    /// @version Maestro 1.9.0
    /// @note beta
    /// </summary>
    public partial class MPTKStat
    {
        public int CountAll;
        public int CountNote;
        public int CountPreset;
    }

    /// <summary>
    /// Contains some information about the tempo change map.
    /// @version Maestro 1.9.0
    /// @note beta
    /// </summary>
    public class MPTKTempo
    {
        public int Track;
        public long FromTick;
        public long ToTick;
        public double Cumul;
        public double Ratio;
        /// <summary>
        /// BPM = 60000000 / MicrosecondsPerQuarterNote
        /// </summary>
        public int MicrosecondsPerQuarterNote;
    }

    // @endcond

}
