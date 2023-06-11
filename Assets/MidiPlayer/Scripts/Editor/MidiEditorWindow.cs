//#define MPTK_PRO
#define DEBUG_EDITOR
using System;
using System.Collections.Generic;
using System.IO;

namespace MidiPlayerTK
{
    using NUnit.Framework.Internal.Execution;
    using System.Xml.Serialization;
    using UnityEditor;
    using UnityEditor.Compilation;
    using UnityEngine;
    using Debug = UnityEngine.Debug;

    /// <summary>@brief
    /// MIDI Editor
    ///     Features:
    ///         Create a MIDI sequence from scratch.
    ///         Load a MIDI sequence from Maestro MIDI DB or an external MIDI file.
    ///         Save a MIDI sequence to Maestro MIDI DB or to an external MIDI file.
    ///         Keep current edition safe even when compiling or relaunching Unity editor (serialize/deserialize).
    ///         Display MIDI events by channel with a piano roll view
    ///         Create/Modify properties of notes and presets change.
    ///         Mouse editing functions: drag & drop event, change length.
    ///         Editing with quantization (whole, half, quarter, ...).
    ///         Integrated MIDI player, playing available when editing (but not when running).
    ///         Looping on the whole sequence or between two points.
    ///         Lean mode for looping at end (immediately, when all voices are released, when all voices are finished)
    ///         
    ///     Backlog by priority order:
    ///         Investigate why midiPlayerGlobal.InitInstance() takes time occasionally
    ///         Hide/show channel
    ///         Redesign time banner with real time in relation of tempo change.
    ///         Manage time signature other than 4/4.
    ///         Disable/enable playing by channel.
    ///         Copy/paste event? multi events?
    ///         Merging classes AreaUI and SectionAll?
    ///         Undo/Redo.
    ///         Add velocity edition with UI.
    ///         Add control change edition with UI.
    ///         Add tempo event section with edition.
    ///         Playing and edtiting in run mode, feasability?
    ///         Redesign color and texture like MIDI event, keyboard ... 
    ///         Specific MIDI events for integration with Unity.
    ///         Multi events selection, feasability?
    ///         Helper for building chords.
    ///         Helper for scale.
    ///         Helper for chords progression?
    ///         Percussion library integrated in MIDI DB?
    ///         Add text event edition + others meta.
    ///         Add effects section.
    ///         Partial import/insert of MIDI: select channels, select tick. Example: import only drum from a lib.
    ///         Manage MIDI tracks, useful?
    ///         Resizable panel, like between piano and note, useful?
    ///         Rhythm generator with Euclidean algo.
    ///         Send MIDI events to an external MIDI keyboard + MIDI beat clock?
    ///         
    ///         
    /// </summary>
    [ExecuteAlways, InitializeOnLoadAttribute]
    public partial class MidiEditorWindow : EditorWindow
    {
        static private MidiEditorWindow window;

        // % (ctrl on Windows, cmd on macOS), # (shift), & (alt).
#if MPTK_PRO
        [MenuItem("Maestro/Midi Editor &E", false, 12)]
#else
        [MenuItem("Maestro/Midi Editor [Pro] &E", false, 12)]
#endif
        public static void Init()
        {
#if MPTK_PRO
            try
            {
                MidiPlayerGlobal.InitPath();
                ToolsEditor.LoadMidiSet();
                ToolsEditor.CheckMidiSet();
                AssetDatabase.Refresh();


                if (MidiPlayerGlobal.CurrentMidiSet == null || MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo == null)
                    EditorUtility.DisplayDialog($"MIDI Editor", MidiPlayerGlobal.ErrorNoSoundFont, "OK");
                else if (MidiPlayerGlobal.CurrentMidiSet.ActiveSounFontInfo.PatchCount == 0)
                    EditorUtility.DisplayDialog($"MIDI Editor", MidiPlayerGlobal.ErrorNoPreset, "OK");
                else
                {
                    window = GetWindow<MidiEditorWindow>(false, "MIDI Editor " + ToolsEditor.version);
                    //window = GetWindowWithRect<MidiEditorWindow>(new Rect(0, 0, 180, 80),false, "MIDI Editor (beta) - Maestro " + ToolsEditor.version);
                    window.wantsMouseMove = true;
                    window.minSize = new Vector2(300, 350);
                }
            }
            catch (Exception /*ex*/)
            {
                //MidiPlayerGlobal.ErrorDetail(ex);
            }
#else
            PopupWindow.Show(new Rect(100, 100, 30, 18), new GetFullVersion());
#endif
        }
    }
}