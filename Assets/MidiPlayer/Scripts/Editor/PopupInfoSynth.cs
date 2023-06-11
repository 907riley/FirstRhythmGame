//#define DEBUG_STATUS_STAT // also in MidiSynth.cs

using UnityEngine;
using UnityEditor;

using System;

using System.Collections;
using System.Collections.Generic;

namespace MidiPlayerTK
{
    public class PopupInfoSynth : EditorWindow
    {
        private GUIStyle TextFieldMultiCourier;
        public MidiSynth MidiSynth;
        private GUIContent btResetStatContent;
        private Vector2 btResetStatSize;
        public PopupInfoSynth()
        {
            minSize = new Vector2(655, 73);
        }

        void OnGUI()
        {
            try
            {
                MPTKGui.LoadSkinAndStyle(false);
                GUI.skin = MPTKGui.MaestroSkin;
                //Debug.Log(this.position);

                if (TextFieldMultiCourier == null || TextFieldMultiCourier.normal.background == null)
                {
                    RectOffset SepBorder1 = new RectOffset(1, 1, 1, 1);
                    TextFieldMultiCourier = MPTKGui.BuildStyle(inheritedStyle: MPTKGui.TextField, fontSize: 14, textAnchor: TextAnchor.UpperLeft);
                    TextFieldMultiCourier.wordWrap = true;
                    TextFieldMultiCourier.richText = true;
                    TextFieldMultiCourier.font = Resources.Load<Font>("Courier");
                    MPTKGui.ColorStyle(style: TextFieldMultiCourier,
                        fontColor: Color.green,
                        backColor: MPTKGui.MakeTex(10, 10, 
                        textureColor: new Color(0.2f, 0.2f, 0.2f, 1f), 
                        border: SepBorder1, 
                        bordercolor: new Color(0.1f, 0.1f, 0.1f, 1)));
                    if (btResetStatContent == null)
                        btResetStatContent = new GUIContent("R", "Reset Stat");
                    btResetStatSize = MPTKGui.Button.CalcSize(btResetStatContent);
                }

                if (MidiSynth == null)
                    return;

                string info;

                string modeSynth = MidiSynth.name + "\tMode: " + (MidiSynth.MPTK_CorePlayer ? "Core" : "AudioSource");
                info = $"{modeSynth}\tRate:{MidiSynth.OutputRate}\tBuffer:{MidiSynth.DspBufferSize}\tDSP:{Math.Round(MidiSynth.StatDeltaAudioFilterReadMS, 2),-5:F2} ms\n";
                info += $"Voice Playing: {MidiSynth.MPTK_StatVoiceCountPlaying,-4}\tActive:{MidiSynth.MPTK_StatVoiceCountActive,-4}\n";
                info += $"Played:{MidiSynth.MPTK_StatVoicePlayed,-4}\tCached::{MidiSynth.MPTK_StatVoiceCountFree,-4}\tReused:{Mathf.RoundToInt(MidiSynth.MPTK_StatVoiceRatioReused)} %\n";

#if DEBUG_STATUS_STAT
                if (MidiSynth.StatusStat != null && MidiSynth.StatusStat.Length >= (int)fluid_voice_status.FLUID_VOICE_OFF + 2)
                {
                    info += string.Format("\t\tSustain:{0,-4}\tRelease:{1,-4}\n\n",
                        MidiSynth.StatusStat[(int)fluid_voice_status.FLUID_VOICE_SUSTAINED],
                        MidiSynth.StatusStat[(int)fluid_voice_status.FLUID_VOICE_OFF + 1]
                    );
                }
#endif
                if (MidiSynth.StatAudioFilterReadMA != null)
                {
                    info += string.Format("Stat Synth:\tSample:{0,-5:F2} ms\tMini:{1,-5:F2}\tMaxi:{2,-5:F2}\tAvg:{3,-5:F2}\n",
                        Math.Round(MidiSynth.StatAudioFilterReadMS, 2),
                        MidiSynth.StatAudioFilterReadMIN < double.MaxValue ? Math.Round(MidiSynth.StatAudioFilterReadMIN, 2) : 0,
                        Math.Round(MidiSynth.StatAudioFilterReadMAX, 2),
                        Math.Round(MidiSynth.StatAudioFilterReadAVG, 2));
                }

                if (MidiSynth.StatDspLoadMAX != 0f)
                    info += string.Format("\tLoad:{0} %\tMini:{1,-5:F2}\tMaxi:{2,-5:F2}\tAvg:{3,-5:F2}",//\tLong Avg:{3,-5:F2}",
                        Math.Round(MidiSynth.StatDspLoadPCT, 2),
                        Math.Round(MidiSynth.StatDspLoadMIN, 2),
                        Math.Round(MidiSynth.StatDspLoadMAX, 2),
                        Math.Round(MidiSynth.StatDspLoadAVG, 2));
                //Math.Round(synth.StatDspLoadLongAVG, 1));
                else
                    info += string.Format("DSP Load:{0} % ", Math.Round(MidiSynth.StatDspLoadPCT, 1));

                if (MidiSynth.StatDspLoadPCT >= 100f)
                    info += string.Format("\n\t<color=red>\tDSP Load over 100%</color>");
                else if (MidiSynth.StatDspLoadPCT >= MidiSynth.MaxDspLoad)
                    info += string.Format("\n\t<color=orange>\tDSP Load over {0}%</color>", MidiSynth.MaxDspLoad);
                else info += "\n";

                // Available only when a file Midi reader is enabled
                if (MidiSynth.StatDeltaThreadMidiMA != null && MidiSynth.StatDeltaThreadMidiMIN < double.MaxValue)
                {
                    info += string.Format("\nStat Sequencer:\tDelta:{0,-5:F2} ms\tMini:{1,-5:F2}\tMaxi:{2,-5:F2}\tAvg:{3,-5:F2}\n",
                        Math.Round(MidiSynth.StatDeltaThreadMidiMS, 2),
                        MidiSynth.StatDeltaThreadMidiMIN < double.MaxValue ? Math.Round(MidiSynth.StatDeltaThreadMidiMIN, 2) : 0,
                        Math.Round(MidiSynth.StatDeltaThreadMidiMAX, 2),
                        Math.Round(MidiSynth.StatDeltaThreadMidiAVG, 2));
                    info += string.Format("\tRead:{0,-5:F2} ms\tTreat:{1,-5:F2}\tMaxi:{2,-5:F2}",
                        Math.Round(MidiSynth.StatReadMidiMS, 2),
                        Math.Round(MidiSynth.StatProcessMidiMS, 2),
                        Math.Round(MidiSynth.StatProcessMidiMAX, 2));
                }

                GUILayout.Label(info, TextFieldMultiCourier);

                if (GUI.Button(new Rect(this.position.width - btResetStatSize.x - 5f, 20f, 20f, 20f), btResetStatContent, MPTKGui.Button))
                {
                    MidiSynth.MPTK_ResetStat();
                }
            }
            catch (System.Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }
    }
}