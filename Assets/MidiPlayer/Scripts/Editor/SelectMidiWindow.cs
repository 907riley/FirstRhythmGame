using System;
using System.Collections.Generic;
using System.IO;
using MPTK.NAudio.Midi;
namespace MidiPlayerTK
{
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using static Codice.Client.BaseCommands.Import.Commit;

    public class SelectMidiWindow : EditorWindow
    {
        static public SelectMidiWindow winSelectMidi;
        public int ColWidth = 255;
        public bool KeepOpen = true;
        public object Tag;
        public int SelectedIndexMidi;
        public List<MPTKListItem> midiList;
        private Vector2 scrollPos;
        public Action<object, int> OnSelect;

        List<MPTKListItem> filteredList;

        public string midiFilter;
        Rect rectClear;
        int selectedInFilterList;

        private void OnDisable()
        {
            //Debug.Log($"OnDisable SelectMidiWindow {this.GetInstanceID()}");
            //Close();
        }

        void OnGUI()
        {
            try
            {
                if (MPTKGui.MaestroSkin == null || rectClear == null)
                {
                    rectClear = new Rect();
                    MPTKGui.LoadSkinAndStyle();
                }

                // Skin must defined at each OnGUI cycle (certainly a global GUI variable)
                GUI.skin = MPTKGui.MaestroSkin;
                GUI.skin.settings.cursorColor = Color.white;
                GUI.skin.settings.cursorFlashSpeed = 0f;

                midiList = new List<MPTKListItem>();
                foreach (string midiname in MidiPlayerGlobal.CurrentMidiSet.MidiFiles)
                    midiList.Add(new MPTKListItem() { Label = midiList.Count.ToString() + " - " + midiname, Index = midiList.Count });

                Event currentEvent = Event.current;
                if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.Return)
                    ApplySelected(selectedInFilterList);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Filter:", MPTKGui.LabelLeft, GUILayout.Width(40));

                // Check clearing the textfield before processing the textfield
                // Is a mouse down and the mouse position inside the X label ?
                if (currentEvent.type == EventType.MouseDown && rectClear.Contains(currentEvent.mousePosition))
                    midiFilter = "";

                string newFilter = GUILayout.TextField(midiFilter, MPTKGui.TextField, GUILayout.MinWidth(200), GUILayout.ExpandWidth(true));
                if (newFilter != midiFilter)
                {
                    midiFilter = newFilter;
                    selectedInFilterList = 0;
                }
                if (currentEvent.type != EventType.Layout && currentEvent.type != EventType.Used)
                {
                    // Get the rect position of the textfield
                    Rect last = GUILayoutUtility.GetLastRect();
                    // Build the position of the X label, rectClear must be an instance properties
                    rectClear.x = last.x + last.width - 20;
                    rectClear.y = last.y + 1;
                    rectClear.width = rectClear.height = 20;
                }
                // Build a label wich overlap the textfield 
                GUI.Label(rectClear, new GUIContent(MPTKGui.IconDeleteGray, "Clear Filter"), MPTKGui.Label);

                if (GUILayout.Button($"Select index {SelectedIndexMidi}", MPTKGui.Button, GUILayout.ExpandWidth(true))) /* {selectedInFilterList}*/
                {
                    ApplySelected(selectedInFilterList);
                }

                GUILayout.FlexibleSpace();

                KeepOpen = GUILayout.Toggle(KeepOpen, "Keep Open", MPTKGui.styleToggle, GUILayout.ExpandWidth(false));

                GUILayout.EndHorizontal();

                scrollPos = GUILayout.BeginScrollView(scrollPos);

                try
                {
                    // Build filtered list
                    filteredList = new List<MPTKListItem>();
                    midiList.ForEach(item =>
                    {
                        if (item != null && (string.IsNullOrEmpty(midiFilter) || item.Label.ToUpper().Contains(midiFilter.ToUpper())))
                        {
                            if (item.Index == SelectedIndexMidi)
                                selectedInFilterList = filteredList.Count;
                            filteredList.Add(item);
                        }
                    });

                    if (filteredList.Count > 0)
                    {
                        GUIContent[] listLabel = new GUIContent[filteredList.Count];
                        int i = 0;
                        float maxLen = 10f;

                        filteredList.ForEach(s =>
                        {
                            listLabel[i] = new GUIContent(s.Label);
                            maxLen = Mathf.Max(maxLen, MPTKGui.Button.CalcSize(listLabel[i]).x);
                            i++;
                        });

                        int colCount = Mathf.Clamp(Convert.ToInt32(this.position.size.x / maxLen + 0.5f), 1, 15) - 1;
                        if (colCount <= 0) colCount = 1;
                        int selected = GUILayout.SelectionGrid(selectedInFilterList, listLabel, colCount/*, MPTKGui.Button*/);
                        if (selectedInFilterList != selected)
                        {
                            selectedInFilterList = selected;
                            ApplySelected(selectedInFilterList);
                        }
                    }
                }
                catch (Exception)
                {
                }
                GUILayout.EndScrollView();
            }
            catch (Exception ex)
            {
                MidiPlayerGlobal.ErrorDetail(ex);
            }
        }

        private void ApplySelected(int selected)
        {
            if (selected >= 0 && selected < filteredList.Count)
            {
                SelectedIndexMidi = filteredList[selected].Index;
                if (OnSelect != null)
                    OnSelect(Tag, SelectedIndexMidi);
                if (!KeepOpen)
                    this.Close();
            }
        }
    }
}