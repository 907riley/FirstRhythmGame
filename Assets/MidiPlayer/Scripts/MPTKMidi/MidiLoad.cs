using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using MPTK.NAudio.Midi;
using System;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;

namespace MidiPlayerTK
{
    /// <summary>
    /// Base class for loading a Midi file. <b>It is not recommended to instanciate directly this class.</b>\n
    /// This class is used by MidiFilePlayer, MidiListPlayer, MidiFileWrite2, MidiFileLoader (see members MPTK_MidiLoaded of these classes).\n
    /// It is better to use the prefab and class MidiFileLoader if you only want to load a Midi file.
    public partial class MidiLoad
    {
        //! @cond NODOC
        public MidiFile midifile;
        public bool EndMidiEvent;
        //public double QuarterPerMinuteValue;
        public string SequenceTrackName = "";
        public string ProgramName = "";
        public string TrackInstrumentName = "";
        public string TextEvent = "";
        public string Copyright = "";
        //! @endcond

        /// <summary>@brief
        /// Lenght in millisecond of a MIDI tick. The pulse length is the minimum time in millisecond between two MIDI events.\n
        /// It's like a definition of graphical resolution but for MIDI: the MIDI sequencer will not be able to play two separate MIDI events in a time below this value.\n
        /// @note
        /// @li Depends on the current tempo, the #MPTK_DeltaTicksPerQuarterNote and the Speed.\n
        /// @li Formula: PulseLenght = MPTK_CurrentTempo / MPTK_DeltaTicksPerQuarterNote / 1000 / Speed
        /// </summary>
        public double MPTK_PulseLenght;


        //public List<TrackMidiEvent> MPTK_MidiEvents;
        public List<MPTKEvent> MPTK_MidiEvents;

        /// <summary>@brief
        /// Initial tempo found in the MIDI.
        /// </summary>
        public double MPTK_InitialTempo;

        /// <summary>@brief
        /// Get or change the current tempo played by the internal MIDI sequencer.\n
        /// Can be used only when the MIDI is playing.\n
        /// Warning: changing the current tempo when playing has no impact on the calculated duration of the MIDI.
        /// https://en.wikipedia.org/wiki/Tempo
        /// </summary>
        public double MPTK_CurrentTempo
        {
            get
            {
                return fluid_player_get_bpm();
            }
            set
            {
                if (value > 0)
                    fluid_player_set_bpm((int)value);
            }
        }

        public double Speed { get => speed; }

        /// <summary>@brief
        /// List of all tempo changes found in the MIDI.
        /// </summary>
        public List<MPTKTempo> MPTK_TempoMap;

        /// <summary>@brief
        /// Real duration expressed in TimeSpan of the full midi from the first event (tick=0) to the last event.\n
        /// If #MPTK_KeepEndTrack is false, the MIDI events End Track are not considered to calculate this time.\n
        /// The tempo changes are taken into account if #MPTK_EnableChangeTempo is set to true before loading the MIDI.
        /// </summary>
        public TimeSpan MPTK_Duration;

        /// <summary>@brief
        /// Real duration expressed in milliseconds of the full midi from the first event (tick=0) to the last event.\n
        /// If #MPTK_KeepEndTrack is false, the MIDI events End Track are not considered to calculate this time.\n
        /// The tempo changes are taken into account if #MPTK_EnableChangeTempo is set to true before loading the MIDI.
        /// </summary>
        public float MPTK_DurationMS;

        /// <summary>
        /// Tick to start playing the MIDI.
        /// @snippet MidiLoop.cs ExampleMidiLoop
        /// </summary>
        public long MPTK_TickStart;

        /// <summary>
        /// Tick to end playing the MIDI. All MIDI events with MIDI tick higher or equal are not played.
        /// @snippet MidiLoop.cs ExampleMidiLoop
        /// </summary>
        public long MPTK_TickEnd;            // stop the MIDI reading at this tick

        /// <summary>@brief
        /// Tick value of the last MIDI event found in the MIDI loaded.
        /// </summary>
        public long MPTK_TickLast;

        /// <summary>@brief
        /// Get the tick value of the last MIDI event played.\n
        /// Set the tick value of the next MIDI event to played.\n
        /// @details
        /// Midi tick is an easy way to identify a position in a song independently of the time which could vary with tempo change event.\n
        /// The count of ticks by quarter is constant all along a MIDI, it's a properties of the whole MIDI. see #MPTK_DeltaTicksPerQuarterNote.\n
        /// With a time signature of 4/4 the ticks length of a bar is 4 * #MPTK_DeltaTicksPerQuarterNote.\n
        /// More info here https://paxstellar.fr/2020/09/11/midi-timing/
        /// @notes
        /// @li Works only when the MIDI sequencer is playing.
        /// @li It's not possible to set the current tick when the MIDI is not playing. Rather use the event OnEventStartPlayMidi to change the start position.
        /// @li See also MidiFilePlayer#MPTK_Position: set or get the position in milliseconds.
        /// </summary>
        public long MPTK_TickCurrent;

        /// <summary>@brief
        /// Get the real-time tick value of the MIDI Player.\n
        /// @details
        /// More info about tick here here https://paxstellar.fr/2020/09/11/midi-timing/
        /// @notes
        /// @li Works only when the MIDI Player is playing.
        /// @li Can't be modified.
        /// @li Used by the MIDI Editor (pro) to display in real time the sequencer position.
        public long MPTK_TickPlayer;            /* the number of tempo ticks passed was cur_ticks*/

        /// <summary>@brief
        /// Current MIDI event read when the MIDI sequencer is playing the MIDI. See #MPTK_TickCurrent.
        /// </summary>
        public MPTKEvent MPTK_LastEventPlayed;

        /// <summary>@brief
        /// Tick position for the first note-on found.\n
        /// Most MIDI don't start playing a note immediately. There is often a delay.\n
        /// Use this attribute to known the tick position where the player will start to play a sound.\n
        /// See also #MPTK_PositionFirstNote
        /// </summary>
        public long MPTK_TickFirstNote;

        /// <summary>@brief
        /// Tick position for the last note-on found.\n
        /// There is often other MIDI events after the last note-on: for example event track-end.\n
        /// Use this attribute to known the tick position time when all sound will be stop.\n
        /// See also the #MPTK_PositionLastNote which provides the last tich of the MIDI.
        /// </summary>
        public long MPTK_TickLastNote;

        /// <summary>@brief
        /// Real time position in millisecond for the first note-on found.\n
        /// Most MIDI don't start playing a note immediately. There is often a delay.\n
        /// Use this attribute to known the real time wich it will start.\n
        /// See also #MPTK_TickFirstNote
        /// </summary>
        public double MPTK_PositionFirstNote;

        /// <summary>@brief
        /// Real time position in millisecond for the last note-on found in the MIDI.\n
        /// There is often other MIDI events after the last note-on: for example event track-end.\n
        /// Use this attribute to known the real time when all sound will be stop.\n
        /// See also the #MPTK_DurationMS which provides the full time of all MIDI events including track-end, control at the beginning and at the end, ....\n
        /// See also #MPTK_TickLastNote
        /// </summary>
        public double MPTK_PositionLastNote;

        /// <summary>@brief
        /// From TimeSignature event: The numerator counts the number of beats in a measure.\n
        /// For example a numerator of 4 means that each bar contains four beats.\n
        /// This is important to know because usually the first beat of each bar has extra emphasis.\n
        /// https://paxstellar.fr/2020/09/11/midi-timing/
        /// </summary>
        public int MPTK_NumberBeatsMeasure;

        /// <summary>@brief
        /// From TimeSignature event: number of quarter notes in a beat.\n
        /// Equal 2 Power TimeSigDenominator.\n
        /// https://paxstellar.fr/2020/09/11/midi-timing/
        /// </summary>
        public int MPTK_NumberQuarterBeat;

        /// <summary>@brief
        /// From TimeSignature event: The numerator counts the number of beats in a measure.\n
        /// For example a numerator of 4 means that each bar contains four beats.\n
        /// This is important to know because usually the first beat of each bar has extra emphasis.\n
        /// In MIDI the denominator value is stored in a special format. i.e. the real denominator = 2 ^ #MPTK_TimeSigNumerator\n
        /// https://paxstellar.fr/2020/09/11/midi-timing/
        /// </summary>
        public int MPTK_TimeSigNumerator;

        /// <summary>@brief
        /// From TimeSignature event: The denominator specifies the number of quarter notes in a beat.\n
        ///   2 represents a quarter-note,\n
        ///   3 represents an eighth-note, etc.\n
        /// https://paxstellar.fr/2020/09/11/midi-timing/
        /// </summary>
        public int MPTK_TimeSigDenominator;

        /// <summary>@brief
        /// From KeySignature event: Values between -7 and 7 and specifies the key signature in terms of number of flats (if negative) or sharps (if positive)
        /// https://www.recordingblogs.com/wiki/midi-key-signature-meta-message
        /// </summary>
        public int MPTK_KeySigSharpsFlats;

        /// <summary>@brief
        /// From KeySignature event: Specifies the scale of the MIDI file.
        /// @li  0 the scale is major.
        /// @li  1 the scale is minor.
        /// https://www.recordingblogs.com/wiki/midi-key-signature-meta-message
        /// </summary>
        public int MPTK_KeySigMajorMinor;

        /// <summary>@brief
        /// From TimeSignature event: The standard MIDI clock ticks every 24 times every quarter note (crotchet)\n
        /// So a #MPTK_TicksInMetronomeClick value of 24 would mean that the metronome clicks once every quarter note.\n
        /// A #MPTK_TicksInMetronomeClick value of 6 would mean that the metronome clicks once every 1/8th of a note (quaver).\n
        /// https://paxstellar.fr/2020/09/11/midi-timing/
        /// </summary>
        public int MPTK_TicksInMetronomeClick;

        /// <summary>@brief
        /// From TimeSignature event: This value specifies the number of 1/32nds of a note happen every MIDI quarter note.\n
        /// It is usually 8 which means that a quarter note happens every quarter note.\n
        /// https://paxstellar.fr/2020/09/11/midi-timing/
        /// </summary>
        public int MPTK_No32ndNotesInQuarterNote;

        /// <summary>@brief
        /// Read from the SetTempo event: The tempo is given in micro seconds per quarter beat. 
        /// To convert this to BPM we needs to use the following equation:BPM = 60,000,000/[tt tt tt]
        /// Warning: this value can change during the playing when a change tempo event is find. \n
        /// https://paxstellar.fr/2020/09/11/midi-timing/
        /// </summary>
        public int MPTK_MicrosecondsPerQuarterNote;

        /// <summary>@brief
        /// Delta Ticks Per Quarter Note (or DTPQN) represent the duration time in "ticks" which make up a quarter-note. \n
        /// For example, with 96 a duration of an eighth-note in the file would be 48.\n
        /// From a MIDI file, this value is found in the MIDI Header and remains constant for all the MIDI file.\n
        /// More info here https://paxstellar.fr/2020/09/11/midi-timing/\n
        /// </summary>
        public int MPTK_DeltaTicksPerQuarterNote;

        /// <summary>@brief
        /// Count of track in the MIDI file
        /// </summary>
        public int MPTK_TrackCount;

        /// <summary>@brief
        /// Processing time in millisecond for loading the MIDI file.
        /// </summary>
        public float MPTK_LoadTime;

        /// <summary>@brief
        /// If true display in console all midi events when a MIDI file is loaded. v2.9.0\n
        /// Set to true could increase greatly the load time. To be used only for debug.
        /// </summary>
        public bool MPTK_LogLoadEvents;

        /// <summary>@brief
        /// A MIDI file is a kind of keyboard simulation: in general, a key pressed generates a 'note-on' and a key release generates a 'note-off'.\n
        /// But there is an other possibility in a MIDI file: create a 'note-on' with a velocity=0 wich must act as a 'midi-off'\n
        /// By default, MPTK create only one MPTK event with the command NoteOn and a duration.\n
        /// But in some cases, you could want to keep the note-off events if they exist in the MIDI file.\n
        /// Set to false if there is no need (could greatly increases the MIDI list events).\n
        /// Set to true to keep 'note-off' events.
        /// </summary>
        public bool MPTK_KeepNoteOff;

        /// <summary>
        /// @deprecated 
        /// v2.9.0 rather use MPTK_KeepNoteOff
        /// </summary>
        public bool KeepNoteOff;

        /// <summary>@brief
        /// If set to true, the meta MIDI events 'End Track' are kept and the MIDI duration includes the 'End Track' events.\n
        /// Default value is false.
        /// </summary>
        public bool MPTK_KeepEndTrack;

        /// <summary>@brief
        /// Should accept change tempo from MIDI Events ? 
        /// </summary>
        public bool MPTK_EnableChangeTempo;

        public bool ReadyToStarted;
        public bool ReadyToPlay;

        private long Quantization;
        //private long CurrentTick;
        private double speed = 1d;
        //private double LastTimeFromStartMS;

        public long TickSeek;           /* new position in tempo ticks (midi ticks) for seeking was seek_ticks*/
        public long TickFromTempoChange;          /* the number of ticks passed at the last tempo change was start_tick */
        //private int begin_msec;           /* the time (msec) of the beginning of the file */
        private int next_event;
        private int start_msec;           /* the start time of the last tempo change */
        private int cur_msec;             /* the current time */
        private int miditempo;            /* as indicated by MIDI SetTempo: n 24th of a usec per midi-clock. bravo! */
        //private double deltatime;         /* milliseconds per midi tick. depends on set-tempo */

        private DateTime timeStartLoad;

        public MidiLoad()
        {
            //ReadyToStarted = false;
            ReadyToPlay = false;
        }

        /// <summary>
        /// Removes all information about the MIDI loaded
        /// </summary>
        public void MPTK_Clear()
        {
            InitMidiLoadAttributes();
        }
        /// <summary>
        /// Initializze MIDI attributs
        /// </summary>
        private void InitMidiLoadAttributes()
        {
            MPTK_LoadTime = 0;
            MPTK_PulseLenght = 0;
            MPTK_MidiEvents = null;
            MPTK_Duration = TimeSpan.Zero;
            MPTK_DurationMS = 0f;
            MPTK_TickLast = 0;
            MPTK_LastEventPlayed = null;
            MPTK_TickFirstNote = -1;
            MPTK_TickLastNote = -1;
            MPTK_PositionFirstNote = 0;
            MPTK_PositionLastNote = 0;
            MPTK_NumberBeatsMeasure = 4; // Default value if there is no TimeSignatureEvent
            MPTK_NumberQuarterBeat = 0;
            MPTK_TimeSigNumerator = 0;
            MPTK_TimeSigDenominator = 0;
            MPTK_KeySigSharpsFlats = 0;
            MPTK_KeySigMajorMinor = 0;
            MPTK_TicksInMetronomeClick = 0;
            MPTK_No32ndNotesInQuarterNote = 0;
            MPTK_MicrosecondsPerQuarterNote = 0;
            MPTK_DeltaTicksPerQuarterNote = 0;
            MPTK_TrackCount = 0;
            MPTK_TickCurrent = 0;
            ClearMetaText();

        }

        /// <summary>@brief
        /// Load Midi from midi MPTK referential (Unity resource). \n
        /// The index of the Midi file can be found in the windo "Midi File Setup". Display with menu MPTK / Midi File Setup\n
        /// If result is true, MPTK_MidiEvents contains list of MIDI events.
        /// @code
        /// public MidiLoad MidiLoaded;
        /// // .....
        /// MidiLoaded = new MidiLoad();
        /// MidiLoaded.MPTK_Load(14) // index for "Beattles - Michelle"
        /// Debug.Log("Duration:" + MidiLoaded.MPTK_Duration);
        /// @endcode
        /// </summary>
        /// <param name="index"></param>
        /// <param name="strict">If true will error on non-paired note events, default:false</param>
        /// <returns>true if loaded</returns>
        public bool MPTK_Load(int index, bool strict = false)
        {
            bool ok = true;
            InitMidiLoadAttributes();
            try
            {
                if (MidiPlayerGlobal.CurrentMidiSet.MidiFiles != null && MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count > 0)
                {
                    if (index >= 0 && index < MidiPlayerGlobal.CurrentMidiSet.MidiFiles.Count)
                    {
                        timeStartLoad = DateTime.Now;
                        string midiname = MidiPlayerGlobal.CurrentMidiSet.MidiFiles[index];
                        TextAsset mididata = Resources.Load<TextAsset>(Path.Combine(MidiPlayerGlobal.MidiFilesDB, midiname));
                        midifile = new MidiFile(mididata.bytes, strict);
                        if (midifile != null)
                        {
                            MPTK_DeltaTicksPerQuarterNote = midifile.DeltaTicksPerQuarterNote;
                            MPTK_MidiEvents = ConvertNAudioEventsToMPTKEvents();
                            AnalyseTrackMidiEvent();
                            if (MPTK_LogLoadEvents) MPTK_DisplayMidiAttributes();
                        }
                    }
                    else
                    {
                        Debug.LogWarningFormat("MidiLoad - index {0} out of range ", index);
                        ok = false;
                    }
                }
                else
                {
                    Debug.LogWarningFormat("MidiLoad - index:{0} - {1}", index, MidiPlayerGlobal.ErrorNoMidiFile);
                    ok = false;
                }
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
                ok = false;
            }
            return ok;
        }


        /// <summary>@brief
        /// Load Midi from an array of bytes. If result is true, MPTK_MidiEvents contains list of MIDI events.
        /// </summary>
        /// <param name="datamidi">byte arry midi</param>
        /// <param name="strict">If true will error on non-paired note events, default:false</param>
        /// <returns>true if loaded</returns>
        public bool MPTK_Load(byte[] datamidi, bool strict = false)
        {
            bool ok = true;
            InitMidiLoadAttributes();
            try
            {
                timeStartLoad = DateTime.Now;
                midifile = new MidiFile(datamidi, strict);
                if (midifile != null)
                {
                    if (midifile.FileFormat == 0)
                    {
                        // V2.9.0 automatic migration from format 0 to format 1
                        midifile.Events.MidiFileType = 1;
                        midifile.Events.PrepareForExport();
                    }
                    MPTK_DeltaTicksPerQuarterNote = midifile.DeltaTicksPerQuarterNote;
                    MPTK_MidiEvents = ConvertNAudioEventsToMPTKEvents();
                    AnalyseTrackMidiEvent();
                    if (MPTK_LogLoadEvents) MPTK_DisplayMidiAttributes();
                }
                else
                    ok = false;
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
                ok = false;
            }
            return ok;
        }

        /// <summary>@brief
        /// Load Midi from a Midi file from Unity resources. The Midi file must be present in Unity MidiDB ressource folder.
        /// @code
        /// public MidiLoad MidiLoaded;
        /// // .....
        /// MidiLoaded = new MidiLoad();
        /// MidiLoaded.MPTK_Load("Beattles - Michelle")
        /// Debug.Log("Duration:" + MidiLoaded.MPTK_Duration);
        /// @endcode
        /// </summary>
        /// <param name="midiname">Midi file name without path and extension</param>
        /// <param name="strict">if true, check strict compliance with the Midi norm</param>
        /// <returns>true if loaded</returns>
        public bool MPTK_Load(string midiname, bool strict = false)
        {
            try
            {
                //TextAsset mididata = Resources.Load<TextAsset>(MidiPlayerGlobal.MidiFilesDB + "/" + midiname);
                TextAsset mididata = Resources.Load<TextAsset>(Path.Combine(MidiPlayerGlobal.MidiFilesDB, midiname));
                if (mididata != null && mididata.bytes != null && mididata.bytes.Length > 0)
                    return MPTK_Load(mididata.bytes, strict);
                else
                    Debug.LogWarningFormat("MIDI {0} not loaded from folder {1}", midiname, MidiPlayerGlobal.MidiFilesDB);
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
            return false;
        }

        /// <summary>@brief
        /// Read the list of midi events available in the Midi from a ticks position to an end position.
        /// </summary>
        /// <param name="fromTicks">ticks start, default 0</param>
        /// <param name="toTicks">ticks end, default end of Midi file</param>
        /// <returns></returns>
        public List<MPTKEvent> MPTK_ReadMidiEvents(long fromTicks = 0, long toTicks = long.MaxValue)
        {
            List<MPTKEvent> midievents = new List<MPTKEvent>();
            try
            {
                if (MPTK_MidiEvents != null)
                {
                    foreach (MPTKEvent trackEvent in MPTK_MidiEvents)
                    {
                        //if (Quantization != 0)
                        //    mptkEvent.AbsoluteQuantize = ((mptkEvent.Event.AbsoluteTime + Quantization / 2) / Quantization) * Quantization;
                        //else
                        //    mptkEvent.AbsoluteQuantize = mptkEvent.Event.AbsoluteTime;

                        //Debug.Log("ReadMidiEvents - timeFromStartMS:" + Convert.ToInt32(timeFromStartMS) + " LastTimeFromStartMS:" + Convert.ToInt32(LastTimeFromStartMS) + " CurrentPulse:" + CurrentPulse + " AbsoluteQuantize:" + mptkEvent.AbsoluteQuantize);

                        //if (mptkEvent.AbsoluteQuantize >= fromTicks && mptkEvent.AbsoluteQuantize <= toTicks)
                        if (trackEvent.Tick >= fromTicks && trackEvent.Tick <= toTicks)
                        {
                            //MPTKEvent mptkEvent = ConvertTrackEventToMPTKEvent(mptkEvent);
                            //if (mptkEvent != null)
                            midievents.Add(trackEvent);
                        }
                        MPTK_TickCurrent = trackEvent.Tick;
                        MPTK_TickLast = trackEvent.Tick;

                        if (trackEvent.Tick > toTicks)
                            break;
                    }
                }
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
            return midievents;
        }

        /// <summary>@brief
        /// Convert the tick duration to a real time duration in millisecond regarding the current tempo.
        /// </summary>
        /// <param name="tick">duration in ticks</param>
        /// <returns>duration in milliseconds</returns>
        public double MPTK_ConvertTickToTime(long tick)
        {
            return tick * MPTK_PulseLenght;
        }

        /// <summary>@brief
        /// Convert a real time duration in millisecond to a number of tick regarding the current tempo.
        /// </summary>
        /// <param name="time">duration in milliseconds</param>
        /// <returns>duration in ticks</returns>
        public long MPTK_ConvertTimeToTick(double time)
        {
            if (MPTK_PulseLenght != 0d)
                return Convert.ToInt64((time / MPTK_PulseLenght) + 0.5d);
            else
                return 0;
        }

        /// <summary>@brief
        /// Search for a Midi event from a time position expressed in millisecond.\n
        /// So time=19.3 and time=19.9 will find the same event.\n
        /// </summary>
        /// <param name="time">position in milliseconds</param>
        /// <returns>MPTKEvent or null</returns>
        public MPTKEvent MPTK_SearchEventFromTime(double time)
        {
            if (time < 0d || time >= MPTK_DurationMS)
                return null;
            else
            {
                //foreach (TrackMidiEvent mptkEvent in MPTK_MidiEvents)
                //    if (mptkEvent.RealTime >= time)
                //    {
                //        MPTKEvent mptkEvent = ConvertTrackEventToMPTKEvent(mptkEvent);
                //        if (mptkEvent != null)
                //            return mptkEvent;
                //    }
                int lowIndex = 0;
                int highIndex = MPTK_MidiEvents.Count - 1;
                int found = -1;
                long lTime = (long)time;
                while (found < 0)
                {
                    int middleIndex = (lowIndex + highIndex) / 2; // bug corrected in v2.9.0 Was low + high / 2 // ;-)
                    long middleTime = (long)MPTK_MidiEvents[middleIndex].RealTime;
                    if (lTime < middleTime)
                        highIndex = middleIndex;
                    else if (lTime > middleTime)
                        lowIndex = middleIndex;
                    else
                        found = middleIndex;
                    if (lowIndex == highIndex || lowIndex + 1 == highIndex)
                        // Found exact time or index adjacent
                        found = lowIndex;
                }

                if (found >= 0)
                {
                    MPTKEvent mptkEvent = MPTK_MidiEvents[found];
                    if (mptkEvent != null)
                        return mptkEvent;
                }

            }
            return null;
        }

        /// <summary>@brief
        /// Search a tick position in the current midi from a position in millisecond.\n
        /// Warning: this method loop on the whole midi to find the position. \n
        /// Could be CPU costly but this method take care of the tempo change in the Midi.\n
        /// Use MPTK_ConvertTimeToTick if there is no tempo change in the midi. 
        /// </summary>
        /// <param name="time">position in milliseconds</param>
        /// <returns>position in ticks</returns>
        public long MPTK_SearchTickFromTime(double time)
        {
            if (time <= 0d)
                return 0L;
            else if (time >= MPTK_DurationMS)
                return MPTK_TickLast;
            else
            {
                foreach (MPTKEvent trackEvent in MPTK_MidiEvents)
                    if (trackEvent.RealTime >= time)
                        return trackEvent.Tick;
            }
            return 0;
        }

        /// <summary>@brief
        /// Convert BPM to duration of a quarter in microsecond
        /// </summary>
        /// <param name="bpm">m</param>
        /// <returns></returns>
        public static int MPTK_BPM2MPQN(int bpm)
        {
            return 60000000 / bpm;
        }

        /// <summary>@brief
        /// Convert duration of a quarter in microsecond to Beats Per Minute
        /// </summary>
        /// <param name="microsecondsPerQuaterNote"></param>
        /// <returns></returns>
        public static int MPTK_MPQN2BPM(int microsecondsPerQuaterNote)
        {
            return 60000000 / microsecondsPerQuaterNote;
        }

        /// <summary>@brief
        /// https://en.wikipedia.org/wiki/Note_value
        /// </summary>
        /// <param name="mptkEvent"></param>
        /// <returns></returns>
        public MPTKEvent.EnumLength NoteLength(MPTKEvent mptkEvent)
        {
            if (mptkEvent.Length >= MPTK_DeltaTicksPerQuarterNote * 4)
                return MPTKEvent.EnumLength.Whole;
            else if (mptkEvent.Length >= MPTK_DeltaTicksPerQuarterNote * 2)
                return MPTKEvent.EnumLength.Half;
            else if (mptkEvent.Length >= MPTK_DeltaTicksPerQuarterNote)
                return MPTKEvent.EnumLength.Quarter;
            else if (mptkEvent.Length >= MPTK_DeltaTicksPerQuarterNote / 2)
                return MPTKEvent.EnumLength.Eighth;
            return MPTKEvent.EnumLength.Sixteenth;
        }

        /// <summary>
        /// Display in console the attributes of the MIDI loaded - v2.9.0
        /// </summary>
        /// <param name="tmEvents"></param>
        public void MPTK_DisplayMidiAttributes()
        {
            if (MPTK_MidiEvents == null)
                Debug.LogWarning($"No MIDI loaded");
            else
            {
                if (string.IsNullOrEmpty(SequenceTrackName)) Debug.Log($"SequenceTrackName: {SequenceTrackName}");
                if (string.IsNullOrEmpty(ProgramName)) Debug.Log($"SequenceTrackName: {ProgramName}");
                if (string.IsNullOrEmpty(TrackInstrumentName)) Debug.Log($"SequenceTrackName: {TrackInstrumentName}");
                if (string.IsNullOrEmpty(TextEvent)) Debug.Log($"SequenceTrackName: {TextEvent}");
                if (string.IsNullOrEmpty(Copyright)) Debug.Log($"SequenceTrackName: {Copyright}");

                Debug.Log($"MPTK_DeltaTicksPerQuarterNote:\t\t{MPTK_DeltaTicksPerQuarterNote} ticks");
                Debug.Log($"MPTK_TicksInMetronomeClick:\t\t{MPTK_TicksInMetronomeClick}");
                Debug.Log($"MPTK_No32ndNotesInQuarterNote:\t\t{MPTK_No32ndNotesInQuarterNote}");
                Debug.Log($"MPTK_MicrosecondsPerQuarterNote:\t\t{MPTK_MicrosecondsPerQuarterNote}");
                Debug.Log($"MPTK_PulseLenght:\t\t{MPTK_PulseLenght} millisecond");

                Debug.Log($"MPTK_TrackCount:\t\t{MPTK_TrackCount}");
                Debug.Log($"MPTK_MidiEvents:\t\t{MPTK_MidiEvents.Count} events");

                Debug.Log($"MPTK_InitialTempo:\t\t{MPTK_InitialTempo} bpm\t\tTempo change:\t\t{MPTK_TempoMap.Count}");
                Debug.Log($"MPTK_DurationMS:\t\t{MPTK_DurationMS / 1000f} seconds \tMPTK_Duration:\t\t{MPTK_Duration}");

                Debug.Log($"MPTK_TickFirstNote:\t\t{MPTK_TickFirstNote} ticks \t\tMPTK_PositionFirstNote:\t{MPTK_PositionFirstNote / 1000f:F2} second {TimeSpan.FromMilliseconds(MPTK_PositionFirstNote)} ");
                Debug.Log($"MPTK_TickLastNote:\t\t{MPTK_TickLastNote} ticks \t\tMPTK_PositionLastNote:\t{MPTK_PositionLastNote / 1000f:F2} second {TimeSpan.FromMilliseconds(MPTK_PositionLastNote)}");

                Debug.Log($"MPTK_TickLast:\t\t{MPTK_TickLast} ticks");
                Debug.Log($"MPTK_NumberBeatsMeasure:\t\t{MPTK_NumberBeatsMeasure}");
                Debug.Log($"MPTK_NumberQuarterBeat:\t\t{MPTK_NumberQuarterBeat}");
                Debug.Log($"MPTK_TimeSigNumerator:\t\t{MPTK_TimeSigNumerator}");
                Debug.Log($"MPTK_TimeSigDenominator:\t\t{MPTK_TimeSigDenominator}");
                Debug.Log($"MPTK_KeySigSharpsFlats:\t\t{MPTK_KeySigSharpsFlats}");
                Debug.Log($"MPTK_KeySigMajorMinor:\t\t{MPTK_KeySigMajorMinor}");

                Debug.Log($"MPTK_LoadTime:\t\t{MPTK_LoadTime} millisecond");
            }
        }

        // No doc until end of file
        //! @cond NODOC

        /// <summary>@brief
        /// Build OS path to the midi file
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        static public string BuildOSPath(string filename)
        {
            try
            {
                string pathMidiFolder = Path.Combine(Application.dataPath, MidiPlayerGlobal.PathToMidiFile);
                string pathfilename = Path.Combine(pathMidiFolder, filename + MidiPlayerGlobal.ExtensionMidiFile);
                return pathfilename;
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
            return null;
        }



        // to avoid to pass as parameters ....
        int indexLoadingTrack;
        private List<MPTKEvent> ConvertNAudioEventsToMPTKEvents()
        {

            try
            {
                // Build tempo map
                // ---------------
                MPTK_TempoMap = new List<MPTKTempo>();

                indexLoadingTrack = 0;
                foreach (IList<MidiEvent> track in midifile.Events)
                {
                    foreach (MidiEvent nAudioMidievent in track)
                    {
                        if (nAudioMidievent.CommandCode == MidiCommandCode.MetaEvent)
                        {
                            MetaEvent meta = (MetaEvent)nAudioMidievent;

                            if (meta.MetaEventType == MetaEventType.SetTempo)
                            {
                                if (!MPTK_EnableChangeTempo && MPTK_TempoMap.Count >= 1)
                                {
                                    // Keep only the first tempo change
                                }
                                else
                                {
                                    double ratio = (double)((TempoEvent)meta).MicrosecondsPerQuarterNote / (double)midifile.DeltaTicksPerQuarterNote / 1000d;
                                    double cumul = 0;
                                    if (MPTK_TempoMap.Count > 0)
                                    {
                                        // v2.89.6 - now take into account tempo change for the realtime value of the MIDI
                                        var tempo = MPTK_TempoMap[MPTK_TempoMap.Count - 1];
                                        var deltaTicks = nAudioMidievent.AbsoluteTime - tempo.FromTick;
                                        cumul = tempo.Cumul + deltaTicks * tempo.Ratio;

                                        // before v.28.6 - error on the realtime value when tempo change
                                        //cumul = tempoMap[tempoMap.Count - 1].cumul + meta.DeltaTime * tempoMap[tempoMap.Count - 1].ratio;
                                    }
                                    int microsecondsPerQuarterNote = ((TempoEvent)meta).MicrosecondsPerQuarterNote;

                                    if (microsecondsPerQuarterNote <= 0)
                                    {
                                        Debug.LogWarning($"SetTempo with incorrect MicrosecondsPerQuarterNote at position {nAudioMidievent.AbsoluteTime} and track {indexLoadingTrack}");
                                        microsecondsPerQuarterNote = MPTK_BPM2MPQN((int)120);
                                    }
                                    MPTK_TempoMap.Add(new MPTKTempo()
                                    {
                                        Track = indexLoadingTrack,
                                        FromTick = nAudioMidievent.AbsoluteTime,
                                        ToTick = long.MaxValue,
                                        Ratio = ratio,
                                        Cumul = cumul,
                                        MicrosecondsPerQuarterNote = microsecondsPerQuarterNote,
                                    });
                                }
                            }
                            else if (meta.MetaEventType == MetaEventType.TimeSignature)
                                AnalyzeTimeSignature(meta);
                            else if (meta.MetaEventType == MetaEventType.KeySignature)
                                AnalyzeKeySignature(meta);
                        }
                    }
                    indexLoadingTrack++;
                }

                if (MPTK_TempoMap.Count == 0)
                    // No tempo defined, set to 120 by default (500 000 microseconds)
                    MPTK_TempoMap.Add(new MPTKTempo()
                    {
                        Track = 0,
                        FromTick = 0,
                        MicrosecondsPerQuarterNote = MPTK_BPM2MPQN(120),
                        Ratio = (double)MPTK_BPM2MPQN(120) / (double)midifile.DeltaTicksPerQuarterNote / 1000d,

                    });
                else if (MPTK_TempoMap.Count > 1)
                {
                    MPTK_TempoMap = MPTK_TempoMap.OrderBy(o => o.FromTick).ToList();
                    for (int indexTempo = 0; indexTempo < MPTK_TempoMap.Count - 1; indexTempo++)
                        MPTK_TempoMap[indexTempo].ToTick = MPTK_TempoMap[indexTempo + 1].FromTick;
                }
                if (MPTK_LogLoadEvents)
                    MPTK_TempoMap.ForEach(tempo => Debug.Log($"Tempo change Track: {tempo.Track} From:{tempo.FromTick} To:{tempo.ToTick} MPQN:{tempo.MicrosecondsPerQuarterNote,10} BPM:{MPTK_MPQN2BPM(tempo.MicrosecondsPerQuarterNote),3} Ratio:{tempo.Ratio,2:F2} Cumul:{tempo.Cumul,6:F2}"));
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }

            try
            {
                // Transform NAudio MIDI events to MPTKEvents
                // ------------------------------------------

                indexLoadingTrack = 0;
                int indexEvent = 0;
                List<MPTKEvent> mptkEvents = new List<MPTKEvent>();
                foreach (IList<MidiEvent> nAudioTrack in midifile.Events)
                {
                    // Set initial tempo for this track
                    int indexTempo = 0;
                    MPTK_MicrosecondsPerQuarterNote = MPTK_TempoMap[indexTempo].MicrosecondsPerQuarterNote;
                    MPTK_InitialTempo = MPTK_MPQN2BPM(MPTK_MicrosecondsPerQuarterNote);
                    fluid_player_set_midi_tempo(MPTK_MicrosecondsPerQuarterNote);
                    if (MPTK_LogLoadEvents)
                        Debug.Log($"Track {indexLoadingTrack} Initial Index Tempo {indexTempo}, MPQN:{MPTK_MicrosecondsPerQuarterNote} BPM:{MPTK_MPQN2BPM(MPTK_MicrosecondsPerQuarterNote)} pulse:{MPTK_PulseLenght}");
                    foreach (MidiEvent nAudioMidievent in nAudioTrack)
                    {
                        // Check next entry tempo
                        while (indexTempo < MPTK_TempoMap.Count - 1 && nAudioMidievent.AbsoluteTime >= MPTK_TempoMap[indexTempo].ToTick)
                        {
                            // Tempo change for this segment 
                            indexTempo++;
                            // Maestro compute the duration of each noteon so MPTK_PulseLenght must be updated with the right value - v2.9.0
                            MPTK_MicrosecondsPerQuarterNote = MPTK_TempoMap[indexTempo].MicrosecondsPerQuarterNote;
                            fluid_player_set_midi_tempo(MPTK_MicrosecondsPerQuarterNote);
                            if (MPTK_LogLoadEvents)
                                Debug.Log($"New Index Tempo {indexTempo}, MPQN:{MPTK_MicrosecondsPerQuarterNote} BPM:{MPTK_MPQN2BPM(MPTK_MicrosecondsPerQuarterNote)} pulse:{MPTK_PulseLenght}");
                        }

                        // V2.89.0 able to remove End Track event
                        if (nAudioMidievent.CommandCode == MidiCommandCode.MetaEvent && ((MetaEvent)nAudioMidievent).MetaEventType == MetaEventType.EndTrack && !MPTK_KeepEndTrack)
                        {
                            //Debug.Log($"Dont keep EndTrack:{indexTracks}  CommandCode:{nAudioMidievent.CommandCode} indexTempo:{indexTempo} AbsoluteTime:{nAudioMidievent.AbsoluteTime} DeltaTime:{nAudioMidievent.DeltaTime} RealTime:{newTime / 1000f:F2} Ratio:{(float)tempoMap[indexTempo].MicrosecondsPerQuarterNote / (float)midifile.DeltaTicksPerQuarterNote / 1000f}");
                        }
                        else
                        {
                            //Before v2.9.0 tmEvents.Add(new MPTKEvent()
                            //{
                            //    Index = indexTracks, // Start from 0
                            //    RealTime = (float)newTime,
                            //    //Event = nAudioMidievent//.Clone()
                            //});
                            MPTKEvent mptkEvent = ConvertNAudioEventToMPTKEvent(nAudioMidievent);
                            if (mptkEvent != null)
                            {
                                double newTime = MPTK_TempoMap[indexTempo].Cumul + (nAudioMidievent.AbsoluteTime - MPTK_TempoMap[indexTempo].FromTick) * MPTK_TempoMap[indexTempo].Ratio;
                                //Debug.Log($"Calcul real time Track:{indexTracks}  CommandCode:{nAudioMidievent.CommandCode} indexTempo:{indexTempo} AbsoluteTime:{nAudioMidievent.AbsoluteTime} DeltaTime:{nAudioMidievent.DeltaTime} RealTime:{newTime / 1000f:F2} Ratio:{(float)tempoMap[indexTempo].MicrosecondsPerQuarterNote / (float)midifile.DeltaTicksPerQuarterNote / 1000f}");
                                mptkEvent.Index = indexEvent++;
                                mptkEvent.RealTime = (float)newTime;
                                mptkEvents.Add(mptkEvent);
                            }
                        }
                    }
                    indexLoadingTrack++; // will be the count of tracks
                }

                if (mptkEvents.Count == 0)
                    throw new Exception("No midi event found");

                return MPTK_SortEvents(mptkEvents, false); // v2.9.0
                //return mptkEvents.OrderBy(o => o.Tick).ToList(); 
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
            return null;
        }

        /// <summary>@brief
        /// Sort the MIDI events list in parameter by ascending tick position.
        /// First priority is applied for 'preset change' and 'meta' event for a group of events with the same position (but 'end track' are set at end of the group). 
        /// @note
        /// @li Reallocation of the list is done, the new sorted list is available at return.
        /// @li good performance for high disorder 
        /// </summary>
        /// <param name="midiEvents"></param>
        /// <param name="logPerf"></param>
        /// <returns>sorted list</returns>
        public static List<MPTKEvent> MPTK_SortEvents(List<MPTKEvent> midiEvents, bool logPerf = false)
        {
            List<MPTKEvent> sortedEvents = midiEvents;
            if (midiEvents != null)
            {
                System.Diagnostics.Stopwatch watch = null;
                if (logPerf)
                {
                    watch = new System.Diagnostics.Stopwatch(); // High resolution time
                    watch.Start();
                }

                // Quick sort (realloc new list)
                sortedEvents = midiEvents.OrderBy(o => o.Tick).ToList();

                if (logPerf)
                {
                    Debug.Log($"Quick sort time {watch.ElapsedMilliseconds} {watch.ElapsedTicks}");
                    watch.Restart();
                }

                // Then sort with priotity on meta and preset change event (too long for a not pre-sorted list)
                Sort(sortedEvents, 0, sortedEvents.Count - 1, new MidiEventComparer());
                if (logPerf)
                {
                    Debug.Log($"Stable sort time {watch.ElapsedMilliseconds} {watch.ElapsedTicks}");
                    watch.Stop();
                }
            }
            else
                Debug.LogWarning("MidiFileWriter2 - MPTK_SortEvents - MidiEvents is null");
            return sortedEvents;
        }

        /// <summary>@brief
        /// In-place and stable implementation of MergeSort
        /// </summary>
        public static void Sort<T>(IList<T> list, int lowIndex, int highIndex, IComparer<T> comparer)
        {
            if (lowIndex >= highIndex)
                return;

            int midIndex = (lowIndex + highIndex) / 2;

            // Partition the list into two lists and Sort them recursively
            Sort(list, lowIndex, midIndex, comparer);
            Sort(list, midIndex + 1, highIndex, comparer);

            // Merge the two sorted lists
            int endLow = midIndex;
            int startHigh = midIndex + 1;

            while ((lowIndex <= endLow) && (startHigh <= highIndex))
            {
                // MRH, if use < 0 sort is not stable
                if (comparer.Compare(list[lowIndex], list[startHigh]) <= 0)
                {
                    lowIndex++;
                }
                else
                {
                    // list[lowIndex] > list[startHigh]
                    // The next element comes from the second list, 
                    // move the list[start_hi] element into the next position and shuffle all the other elements up.
                    T t = list[startHigh];

                    for (int k = startHigh - 1; k >= lowIndex; k--)
                    {
                        list[k + 1] = list[k];
                    }

                    list[lowIndex] = t;
                    lowIndex++;
                    endLow++;
                    startHigh++;
                }
            }
        }


        /// <summary>@brief
        /// Utility class for comparing MidiEvent objects
        /// </summary>
        public class MidiEventComparer : IComparer<MPTKEvent>
        {
            /// <summary>@brief
            /// Compares two MidiEvents
            /// Sorts by time, with EndTrack always sorted to the end
            /// </summary>
            public int Compare(MPTKEvent x, MPTKEvent y)
            {
                long xTime = x.Tick;
                long yTime = y.Tick;

                if (xTime == yTime)
                {
                    // set patch change position at the start of the same position
                    if (x.Command == MPTKCommand.PatchChange)
                        xTime = Int64.MinValue;

                    if (y.Command == MPTKCommand.PatchChange)
                        yTime = Int64.MinValue;

                    // sort meta events before note events, except end track
                    if (x.Command == MPTKCommand.MetaEvent)
                    {
                        if (x.Meta == MPTKMeta.EndTrack)
                            xTime = Int64.MaxValue;
                        else
                            xTime = Int64.MinValue;
                    }
                    if (y.Command == MPTKCommand.MetaEvent)
                    {
                        if (y.Meta == MPTKMeta.EndTrack)
                            yTime = Int64.MaxValue;
                        else
                            yTime = Int64.MinValue;
                    }
                }
                return xTime.CompareTo(yTime);
            }
        }
        private MPTKEvent ConvertNAudioEventToMPTKEvent(MidiEvent midiEvent)
        {
            MPTKEvent mptkEvent = null;
            switch (midiEvent.CommandCode)
            {
                case MidiCommandCode.NoteOn:
                    //if (((NoteOnEvent)mptkEvent.Event).OffEvent != null)
                    {

                        NoteOnEvent noteon = (NoteOnEvent)midiEvent;
                        //Debug.Log(string.Format("Track:{0} NoteNumber:{1,3:000} AbsoluteTime:{2,6:000000} NoteLength:{3,6:000000} OffDeltaTime:{4,6:000000} ", track, noteon.NoteNumber, noteon.AbsoluteTime, noteon.NoteLength, noteon.OffEvent.DeltaTime));
                        if (noteon.OffEvent != null)
                        // V2.88   if (noteon.Velocity != 0)
                        {
                            mptkEvent = new MPTKEvent()
                            {
                                Track = indexLoadingTrack,
                                Tick = midiEvent.AbsoluteTime,
                                Command = MPTKCommand.NoteOn,
                                Value = noteon.NoteNumber,
                                Channel = midiEvent.Channel - 1,
                                Velocity = noteon.Velocity,
                                //DurationTicks = noteon.NoteLength, // added v 3.89.5 // removed in v2.9.0
                                Duration = Convert.ToInt64(noteon.NoteLength * MPTK_PulseLenght),
                                Length = noteon.NoteLength,
                            };
                            if (MPTK_LogLoadEvents) Debug.Log(BuildInfoTrack(mptkEvent) + $"NoteOn {mptkEvent.Value:000} {noteon.NoteName,-4}\tDuration:{noteon.NoteLength,5} ticks {mptkEvent.Duration:000} ms {NoteLength(mptkEvent)}\tVelocity:{noteon.Velocity,3}");
                        }
                        else // It's a noteon which means a noteoff
                        {
                            // V2.88 if (KeepNoteOff)
                            if (MPTK_KeepNoteOff)// && noteon.OffEvent == null && noteon.NoteLength!=0)
                            {
                                mptkEvent = new MPTKEvent()
                                {
                                    Track = indexLoadingTrack,
                                    Tick = midiEvent.AbsoluteTime,
                                    Command = MPTKCommand.NoteOff, // set a noteoff
                                    Value = noteon.NoteNumber,
                                    Channel = midiEvent.Channel - 1,
                                    Velocity = noteon.Velocity,
                                    Duration = Convert.ToInt64(noteon.NoteLength * MPTK_PulseLenght),
                                    Length = noteon.NoteLength,
                                };

                                if (MPTK_LogLoadEvents) Debug.Log(BuildInfoTrack(mptkEvent) + $"NoteOff {mptkEvent.Value:000}\t{noteon.NoteName,-4}\tFrom NoteOn");
                            }
                        }
                    }
                    break;

                case MidiCommandCode.NoteOff:
                    if (MPTK_KeepNoteOff)
                    {
                        NoteEvent noteoff = (NoteEvent)midiEvent;
                        //Debug.Log(string.Format("Track:{0} NoteNumber:{1,3:000} AbsoluteTime:{2,6:000000} NoteLength:{3,6:000000} OffDeltaTime:{4,6:000000} ", track, noteon.NoteNumber, noteon.AbsoluteTime, noteon.NoteLength, noteon.OffEvent.DeltaTime));
                        mptkEvent = new MPTKEvent()
                        {
                            Track = indexLoadingTrack,
                            Tick = midiEvent.AbsoluteTime,
                            Command = MPTKCommand.NoteOff,
                            Value = noteoff.NoteNumber,
                            Channel = midiEvent.Channel - 1,
                            Velocity = noteoff.Velocity,
                            Duration = 0,
                            Length = 0,
                        };

                        if (MPTK_LogLoadEvents) Debug.Log(BuildInfoTrack(mptkEvent) + $"NoteOff {mptkEvent.Value:000}\t{noteoff.NoteName,-4}\tFrom file");
                    }
                    break;

                case MidiCommandCode.PitchWheelChange:
                    PitchWheelChangeEvent pitch = (PitchWheelChangeEvent)midiEvent;
                    mptkEvent = new MPTKEvent()
                    {
                        Track = indexLoadingTrack,
                        Tick = midiEvent.AbsoluteTime,
                        Command = MPTKCommand.PitchWheelChange,
                        Channel = midiEvent.Channel - 1,
                        Value = pitch.Pitch,  // Pitch Wheel Value 0 is minimum, 0x2000 (8192) is default, 0x3FFF (16383) is maximum
                    };
                    if (MPTK_LogLoadEvents) Debug.Log(BuildInfoTrack(mptkEvent) + string.Format("PitchWheelChange {0}", pitch.Pitch));
                    break;

                case MidiCommandCode.ControlChange:
                    ControlChangeEvent controlchange = (ControlChangeEvent)midiEvent;
                    mptkEvent = new MPTKEvent()
                    {
                        Track = indexLoadingTrack,
                        Tick = midiEvent.AbsoluteTime,
                        Command = MPTKCommand.ControlChange,
                        Channel = midiEvent.Channel - 1,
                        Controller = (MPTKController)controlchange.Controller,
                        Value = controlchange.ControllerValue,

                    };

                    //if ((MPTKController)controlchange.Controller != MPTKController.Sustain)

                    // Other midi event
                    if (MPTK_LogLoadEvents) Debug.Log(BuildInfoTrack(mptkEvent) + $"Control 0x{mptkEvent.Controller:X}/{mptkEvent.Controller} {mptkEvent.Value}");

                    break;

                case MidiCommandCode.PatchChange:
                    PatchChangeEvent change = (PatchChangeEvent)midiEvent;
                    mptkEvent = new MPTKEvent()
                    {
                        Track = indexLoadingTrack,
                        Tick = midiEvent.AbsoluteTime,
                        Command = MPTKCommand.PatchChange,
                        Channel = midiEvent.Channel - 1,
                        Value = change.Patch,
                    };
                    if (MPTK_LogLoadEvents) Debug.Log(BuildInfoTrack(mptkEvent) + string.Format("Preset   {0,3:000} {1}", change.Patch, PatchChangeEvent.GetPatchName(change.Patch)));
                    break;

                case MidiCommandCode.MetaEvent:
                    MetaEvent meta = (MetaEvent)midiEvent;
                    mptkEvent = new MPTKEvent()
                    {
                        Track = indexLoadingTrack,
                        Tick = midiEvent.AbsoluteTime,
                        Command = MPTKCommand.MetaEvent,
                        Channel = midiEvent.Channel - 1,
                        Meta = (MPTKMeta)meta.MetaEventType,
                    };

                    switch (meta.MetaEventType)
                    {
                        case MetaEventType.EndTrack:
                            mptkEvent.Info = "End Track";
                            break;

                        case MetaEventType.KeySignature:
                            AnalyzeKeySignature(meta, mptkEvent);
                            break;

                        case MetaEventType.TimeSignature:
                            AnalyzeTimeSignature(meta, mptkEvent);
                            break;

                        case MetaEventType.SetTempo:
                            if (MPTK_EnableChangeTempo)
                            {
                                TempoEvent tempoEvent = (TempoEvent)meta;
                                int microsecondsPerQuarterNote = ((TempoEvent)meta).MicrosecondsPerQuarterNote;
                                long tempo = (long)tempoEvent.Tempo;
                                if (microsecondsPerQuarterNote <= 0 || tempo <= 0)
                                {
                                    tempo = 120;
                                    microsecondsPerQuarterNote = MPTK_BPM2MPQN((int)tempo);
                                }

                                // Tempo change will be done in MidiFilePlayer
                                mptkEvent.Duration = tempo;
                                mptkEvent.Value = microsecondsPerQuarterNote;
                                //// removed from v2.9.0 - Maestro compute the duration of each noteon after building the tempo map
                                //MPTK_MicrosecondsPerQuarterNote = tempo.MicrosecondsPerQuarterNote;
                                //fluid_player_set_midi_tempo(tempo.MicrosecondsPerQuarterNote);
                                if (MPTK_LogLoadEvents) Debug.Log(BuildInfoTrack(mptkEvent) + string.Format("Meta     {0,-15} 'Tempo:{1:F2} MicrosecondsPerQuarterNote:{2}'", meta.MetaEventType, tempoEvent.Tempo, tempoEvent.MicrosecondsPerQuarterNote));
                            }
                            break;

                        case MetaEventType.SequenceTrackName:
                            mptkEvent.Info = ((TextEvent)meta).Text;
                            if (!string.IsNullOrEmpty(SequenceTrackName)) SequenceTrackName += "\n";
                            SequenceTrackName += mptkEvent.Info;
                            break;

                        case MetaEventType.ProgramName:
                            mptkEvent.Info = ((TextEvent)meta).Text;
                            ProgramName += mptkEvent.Info + " ";
                            break;

                        case MetaEventType.TrackInstrumentName:
                            mptkEvent.Info = ((TextEvent)meta).Text;
                            if (!string.IsNullOrEmpty(TrackInstrumentName)) TrackInstrumentName += "\n";
                            TrackInstrumentName += mptkEvent.Info;
                            break;

                        case MetaEventType.TextEvent:
                            mptkEvent.Info = ((TextEvent)meta).Text;
                            TextEvent += mptkEvent.Info + " ";
                            break;

                        case MetaEventType.Copyright:
                            mptkEvent.Info = ((TextEvent)meta).Text;
                            Copyright += mptkEvent.Info + " ";
                            break;

                        case MetaEventType.Lyric: // lyric
                            mptkEvent.Info = ((TextEvent)meta).Text;
                            TextEvent += mptkEvent.Info + " ";
                            break;

                        case MetaEventType.Marker: // marker
                            mptkEvent.Info = ((TextEvent)meta).Text;
                            TextEvent += mptkEvent.Info + " ";
                            break;

                        case MetaEventType.CuePoint: // cue point
                        case MetaEventType.DeviceName:
                            break;
                    }

                    if (MPTK_LogLoadEvents && !string.IsNullOrEmpty(mptkEvent.Info)) Debug.Log(BuildInfoTrack(mptkEvent) + string.Format("Meta     {0,-15} '{1}'", mptkEvent.Meta, mptkEvent.Info));

                    //Debug.Log(BuildInfoTrack(mptkEvent) + string.Format("Meta {0} {1}", meta.MetaEventType, meta.ToString()));
                    break;

                default:
                    // Other midi event
                    if (MPTK_LogLoadEvents) Debug.Log(BuildInfoTrack(mptkEvent) + string.Format("Other    {0,-15} Not handle by MPTK", midiEvent.CommandCode));
                    break;
            }
            return mptkEvent;
        }

        private void AnalyseTrackMidiEvent()
        {
            MPTK_TickFirstNote = -1;
            MPTK_TickLastNote = -1;
            MPTK_TrackCount = 0;
            if (MPTK_MidiEvents != null)
            {
                for (int indexEvent = 0; indexEvent < MPTK_MidiEvents.Count; indexEvent++)
                {
                    MPTKEvent midievent = MPTK_MidiEvents[indexEvent];
                    if (MPTK_TrackCount <= midievent.Track)
                        MPTK_TrackCount = (int)midievent.Track + 1;
                    if (midievent.Command == MPTKCommand.NoteOn)
                    {
                        if (MPTK_TickFirstNote == -1)
                        {
                            MPTK_TickFirstNote = midievent.Tick;
                            MPTK_PositionFirstNote = midievent.RealTime;
                        }
                        MPTK_TickLastNote = midievent.Tick;
                        MPTK_PositionLastNote = midievent.RealTime;
                    }

                    //if (MPTK_TickFirstNote == -1 && midievent.Event.CommandCode == MidiCommandCode.NoteOn)
                    //{
                    //    MPTK_TickFirstNote = midievent.Event.AbsoluteTime;
                    //    MPTK_PositionFirstNote = midievent.RealTime;
                    //    //break;
                    //}
                }

                MPTK_ComputeDuration();
                MPTK_LoadTime = (float)(DateTime.Now - timeStartLoad).TotalMilliseconds;
            }
        }

        /// <summary>
        /// @brief
        /// Calculate duration, tick last of the MIDI loaded.
        /// @details
        /// This is done automatically when loading a MIDI file but not when MIDI events are added by script. In this case you will have to call this method.
        /// </summary>
        public void MPTK_ComputeDuration()
        {
            if (MPTK_MidiEvents != null && MPTK_MidiEvents.Count > 0)
            {
                MPTKEvent lastEvent = MPTK_MidiEvents[MPTK_MidiEvents.Count - 1];
                MPTK_TickLast = lastEvent.Tick;
                MPTK_DurationMS = lastEvent.RealTime;
                MPTK_Duration = TimeSpan.FromMilliseconds(MPTK_DurationMS);
            }
            else
            {
                MPTK_TickLast = 0;
                MPTK_DurationMS = 0;
                MPTK_Duration = TimeSpan.FromMilliseconds(0);
            }
        }

        public void ClearMetaText()
        {
            SequenceTrackName = "";
            ProgramName = "";
            TrackInstrumentName = "";
            TextEvent = "";
            Copyright = "";
        }

        /// <summary>@brief
        /// Change speed to play. 1=normal speed
        /// </summary>
        /// <param name="speed"></param>
        public void ChangeSpeed(float speed)
        {
            try
            {
                if (speed > 0)
                {
                    //Debug.Log($"ChangeSpeed from {speed} to {speed}");
                    //double lastSpeed = speed;
                    this.speed = speed;
                    fluid_player_set_midi_tempo(miditempo);
                    // V2.88 : duration is no longer linked to speed
                    //MPTK_DurationMS = (float)((double)MPTK_DurationMS * lastSpeed / speed);
                    //MPTK_Duration = TimeSpan.FromMilliseconds(MPTK_DurationMS);
                }
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        public void ChangeQuantization(int level = 4)
        {
            try
            {
                if (level <= 0)
                    Quantization = 0;
                else
                    Quantization = MPTK_DeltaTicksPerQuarterNote / level;
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        public void StartMidi()
        {
            // Debug.Log("StartMidi core");
            //begin_msec = 0;
            start_msec = 0;
            TickFromTempoChange = 0;
            MPTK_TickPlayer = 0;
            TickSeek = -1;
            cur_msec = 0;
            next_event = 0;
            EndMidiEvent = false;
        }

        /**
         * Set the tempo of a MIDI player.
         * MPQN: microsecondsPerQuaterNote = 60000000 / bpm
         * @param tempo Tempo to set playback speed from param1
         *
         */
        private void fluid_player_set_midi_tempo(int MPQN)
        {
            if (MPTK_DeltaTicksPerQuarterNote <= 0)
                Debug.LogWarning("DeltaTicksPerQuarterNote is not set, tempo can't be defined");
            else
            {
                miditempo = MPQN;
                MPTK_PulseLenght = (double)MPQN / MPTK_DeltaTicksPerQuarterNote / 1000f / speed; /* in milliseconds */
                //Debug.Log($"Apres tempo change tempo:{MPQN} BPM:{MPTK_MPQN2BPM(MPQN)} pulse:{MPTK_PulseLenght} start:{start_msec} ms start:{TickFromTempoChange} ticks");
                start_msec = cur_msec;
                TickFromTempoChange = MPTK_TickPlayer;
            }
        }

        /**
        * Seek in the currently playing file.
        * @param player MIDI player instance
        * @param ticks the position to seek to in the current file
        * @return #FLUID_FAILED if ticks is negative or after the latest tick of the file,
        *   #FLUID_OK otherwise
        * @since 2.0.0
        *
        * The actual seek is performed during the player_callback.
        */
        public void fluid_player_seek(long ticks)
        {
            //Debug.Log("fluid_player_seek:" + ticks);
            // Include tick un parameter
            if (ticks > 0)
                TickSeek = ticks - 1;
            else
                TickSeek = 0;
        }


        /**
      * Set the tempo of a MIDI player in beats per minute.
      * @param player MIDI player instance
      * @param bpm Tempo in beats per minute
      * @return Always returns #FLUID_OK
      */
        private void fluid_player_set_bpm(int bpm)
        {
            fluid_player_set_midi_tempo(60000000 / bpm);
        }

        /**
         * Get the tempo of a MIDI player in beats per minute.
         * @param player MIDI player instance
         * @return MIDI player tempo in BPM
         * @since 1.1.7
         */

        private int fluid_player_get_bpm()
        {
            return 60000000 / miditempo;
        }

        // not possible.  QueueMidiEvents.Enqueue will send list of events by reference ...
        // and this list can be changed in this thread. We need to allocate a new list when new events are available.
        //public List<MPTKEvent> internalMidiEventsProcessed = new List<MPTKEvent>(10);

        // see previous version for comment and debugide
        public List<MPTKEvent> fluid_player_callback(int msec, int idSession)
        {
            List<MPTKEvent> midiEvents = null;

            try
            {
                if (next_event >= 0 && MPTK_MidiEvents != null /*v2.9.0*/)
                {
                    // Calculate tick player. All MIDI with tick bellow will be read
                    cur_msec = msec;
                    MPTK_TickPlayer = TickFromTempoChange + (long)(((double)(cur_msec - start_msec) / MPTK_PulseLenght) + 0.5d);

                    if (MPTK_TickEnd != 0 && MPTK_TickPlayer >= MPTK_TickEnd)
                        EndMidiEvent = true;
                    else
                    {
                        long ticks = MPTK_TickPlayer;

                        // Seek is needed?
                        if (TickSeek >= 0)
                            ticks = TickSeek;

                        if (MPTK_TickCurrent > ticks)
                            // Search events from the start of the MIDI when MPTK_TickCurrent has been increased 
                            next_event = 0;
                        
                        //Debug.Log($"Search for event from next_event:{next_event} ticks:{ticks} MPTK_TickPlayer:{MPTK_TickPlayer} TickSeek:{TickSeek} TickFromTempoChange:{TickFromTempoChange} cur_msec:{cur_msec} start_msec:{start_msec}");

                        while (true)
                        {
                            if (next_event >= MPTK_MidiEvents.Count)
                            {
                                // End playing when no MIDI event remains in the list
                                //Debug.Log($"End playing when no MIDI event remains in the list. next_event:{next_event}");
                                next_event = -1;
                                break;
                            }

                            MPTKEvent mptkEvent = MPTK_MidiEvents[next_event];
                            // 2.9.0 remove stupid shift half quanta
                            long quantizedTick = Quantization != 0 ? ((mptkEvent.Tick /*+ Quantization / 2*/) / Quantization) * Quantization : mptkEvent.Tick;

                            if (quantizedTick >= ticks) // V2.872 replace > by >=
                            {
                                // MIDI events search is over
                                //Debug.Log($"MIDI events search is over. quantizedTick:{quantizedTick} ticks:{ticks}");
                                break;
                            }
                            //if (TickSeek >= 0 &&    v2.9.0 gentle optimization ;-)
                            //   (mptkEvent.Command == MPTKCommand.NoteOn ||
                            //    mptkEvent.Command == MPTKCommand.NoteOff ||
                            //    (mptkEvent.Command == MPTKCommand.MetaEvent && mptkEvent.Meta != MPTKMeta.SetTempo)))
                            //{
                            //}
                            //else
                            if (TickSeek < 0 ||
                                   (mptkEvent.Command != MPTKCommand.NoteOn &&
                                    mptkEvent.Command != MPTKCommand.NoteOff &&
                                    !(mptkEvent.Command == MPTKCommand.MetaEvent && mptkEvent.Meta != MPTKMeta.SetTempo)))
                            {
                                if (midiEvents == null) midiEvents = new List<MPTKEvent>();

                                // Removed from v2.9.0 MPTKEvent mptkEvent = ConvertTrackEventToMPTKEvent(mptkEvent);

                                if (mptkEvent.Meta == MPTKMeta.SetTempo && MPTK_EnableChangeTempo)
                                {
                                    MPTK_MicrosecondsPerQuarterNote = mptkEvent.Value;
                                    fluid_player_set_midi_tempo(MPTK_MicrosecondsPerQuarterNote);
                                }
                                mptkEvent.IdSession = idSession;
                                midiEvents.Add(mptkEvent);
                            }
                            next_event++;
                            //Debug.Log($"next_event:{next_event} TickSeek:{TickSeek} quantizedTick:{quantizedTick}  MPTK_TickCurrent:{MPTK_TickCurrent}  TickCurrent:{MPTK_TickPlayer}");
                        } 

                        if (TickSeek >= 0)
                        {
                            // Seek sequence over
                            TickFromTempoChange = TickSeek;
                            MPTK_TickPlayer = TickSeek;
                            start_msec = msec;
                            //Debug.Log($"Reset TickSeek start_msec:{start_msec} {MPTK_TickPlayer}");
                            TickSeek = -1;
                        }
                        if (next_event < 0)
                        {
                            EndMidiEvent = true;
                            //Debug.Log($"EndMidiEvent set to true");
                        }

                        if (midiEvents != null && midiEvents.Count > 0)
                        {
                            MPTK_LastEventPlayed = midiEvents[midiEvents.Count - 1];
                            //Debug.Log($"{MPTK_LastEventPlayed.Command} {MPTK_LastEventPlayed.RealTime}");
                            MPTK_TickCurrent = MPTK_LastEventPlayed.Tick;// mptkEvent.Event.AbsoluteTime;
                        }
                        //Debug.Log($"At the end :{next_event} TickSeek:{TickSeek}  MPTK_TickCurrent:{MPTK_TickCurrent}  MPTK_TickPlayer:{MPTK_TickPlayer}");
                    }
                }

            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
            return midiEvents;
        }


        //private static string BuildNoteName(MPTKEvent midievent)
        //{
        //    return (midievent.Channel != 9) ?
        //        $"{HelperNoteLabel.LabelFromMidi(midievent.Value)}":
        //        "Drum";

        //    //String.Format("{0}{1}", NoteNames[midievent.Value % 12], midievent.Value / 12) :
        //}


        // https://www.recordingblogs.com/wiki/midi-key-signature-meta-message
        private void AnalyzeKeySignature(MetaEvent meta, MPTKEvent mptkEvent = null)
        {
            KeySignatureEvent keysig = (KeySignatureEvent)meta;
            MPTK_KeySigSharpsFlats = keysig.SharpsFlats;
            MPTK_KeySigMajorMinor = keysig.MajorMinor;

            if (mptkEvent != null)
            {
                mptkEvent.Value = MPTK_KeySigSharpsFlats;
                mptkEvent.Duration = MPTK_KeySigMajorMinor;
                mptkEvent.Info = $"SharpsFlats:{MPTK_KeySigSharpsFlats} MajorMinor:{MPTK_KeySigMajorMinor}";
            }
        }

        private void AnalyzeTimeSignature(MetaEvent meta, MPTKEvent mptkEvent = null)
        {
            TimeSignatureEvent timesig = (TimeSignatureEvent)meta;

            // Numerator: counts the number of beats in a measure. 
            // For example a numerator of 4 means that each bar contains four beats. 
            MPTK_TimeSigNumerator = timesig.Numerator;
            // Denominator: number of quarter notes in a beat.0=ronde, 1=blanche, 2=quarter, 3=eighth, etc. 
            MPTK_TimeSigDenominator = timesig.Denominator;
            MPTK_NumberBeatsMeasure = timesig.Numerator;
            MPTK_NumberQuarterBeat = System.Convert.ToInt32(Mathf.Pow(2f, timesig.Denominator));
            MPTK_TicksInMetronomeClick = timesig.TicksInMetronomeClick;
            MPTK_No32ndNotesInQuarterNote = timesig.No32ndNotesInQuarterNote;

            if (mptkEvent != null)
            {
                mptkEvent.Value = timesig.Numerator;
                mptkEvent.Duration = timesig.Denominator;
                mptkEvent.Info = $"Numerator:{timesig.Numerator} Denominator:{timesig.Denominator}";
            }
        }

        private string BuildInfoTrack(MPTKEvent e)
        {
            return $"[I:{e.Index:00000} A:{e.Tick:00000} R:{e.RealTime / 1000f:F2}] [Track:{e.Track} Channel:{e.Channel,00}] ";
        }

        public void DebugTrack()
        {
            int itrck = 0;
            foreach (IList<MidiEvent> track in midifile.Events)
            {
                itrck++;
                foreach (MidiEvent midievent in track)
                {
                    string info = string.Format("Track:{0} Channel:{1,2:00} Command:{2} AbsoluteTime:{3:0000000} ", itrck, midievent.Channel, midievent.CommandCode, midievent.AbsoluteTime);
                    if (midievent.CommandCode == MidiCommandCode.NoteOn)
                    {
                        NoteOnEvent noteon = (NoteOnEvent)midievent;
                        if (noteon.OffEvent == null)
                            info += string.Format(" OffEvent null");
                        else
                            info += string.Format(" OffEvent.DeltaTimeChannel:{0:0000.00} ", noteon.OffEvent.DeltaTime);
                    }
                    Debug.Log(info);
                }
            }
        }

        //! @endcond
    }
}

