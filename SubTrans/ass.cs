using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubTitles
{
    public class ASS
    {
        private enum Sections { ScriptInfo = 0, Styles = 1, Events = 2, Fonts = 3, Graphics = 4, Unknown = -1 };

        [Flags]
        public enum SaveFlags { Replace = 0, Merge = 1, SRT = 2, VTT = 4 };

        public class SCRIPTINFO
        {
            internal List<string> Raw = new List<string>();
            public List<string> Comments = new List<string>();
            public string Title;
            public string OriginalScript;
            public string OriginalTranslation;
            public string OriginalEditing;
            public string OriginalTiming;
            public string SynchPoint;
            public string ScriptUpdatedBy;
            public string UpdateDetails;
            public string ScriptType;
            public string Collisions;
            public string PlayResY;
            public string PlayResX;
            public string PlayDepth;
            public string Timer;
            public string WrapStyle;
            public string Style;
            public string Dialogue;
            public string Comment;
            public string Picture;
            public string Sound;
            public string Movie;
            public string Command;

            public void Clear()
            {
                Comments.Clear();

                Title = string.Empty;
                OriginalScript = string.Empty;
                OriginalTranslation = string.Empty;
                OriginalEditing = string.Empty;
                OriginalTiming = string.Empty;
                SynchPoint = string.Empty;
                ScriptUpdatedBy = string.Empty;
                UpdateDetails = string.Empty;
                ScriptType = string.Empty;
                Collisions = string.Empty;
                PlayResY = string.Empty;
                PlayResX = string.Empty;
                PlayDepth = string.Empty;
                Timer = string.Empty;
                WrapStyle = string.Empty;
                Style = string.Empty;
                Dialogue = string.Empty;
                Comment = string.Empty;
                Picture = string.Empty;
                Sound = string.Empty;
                Movie = string.Empty;
                Command = string.Empty;
            }
        }

        public class STYLE
        {
            private SortedDictionary<string, string> fields = new SortedDictionary<string, string>();
            public SortedDictionary<string, string> Fields
            {
                get { return fields; }
            }

            public string Field(string field)
            {
                if (!fields.ContainsKey(field)) return (string.Empty);

                else if (string.Equals(field, "Name", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Name"]);
                }
                else if (string.Equals(field, "Fontname", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Fontname"]);
                }
                else if (string.Equals(field, "Fontsize", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Fontsize"]);
                }
                else if (string.Equals(field, "PrimaryColour", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["PrimaryColour"]);
                }
                else if (string.Equals(field, "SecondaryColour", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["SecondaryColour"]);
                }
                else if (string.Equals(field, "TertiaryColour", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["TertiaryColour"]);
                }
                else if (string.Equals(field, "OutlineColour", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["OutlineColour"]);
                }
                else if (string.Equals(field, "BackColour", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["BackColour"]);
                }
                else if (string.Equals(field, "Bold", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Bold"]);
                }
                else if (string.Equals(field, "Italic", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Italic"]);
                }
                else if (string.Equals(field, "Underline", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Underline"]);
                }
                else if (string.Equals(field, "StrikeOut", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["StrikeOut"]);
                }
                else if (string.Equals(field, "ScaleX", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["ScaleX"]);
                }
                else if (string.Equals(field, "ScaleY", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["ScaleY"]);
                }
                else if (string.Equals(field, "Spacing", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Spacing"]);
                }
                else if (string.Equals(field, "Angle", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Angle"]);
                }
                else if (string.Equals(field, "BorderStyle", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["BorderStyle"]);
                }
                else if (string.Equals(field, "Outline", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Outline"]);
                }
                else if (string.Equals(field, "Shadow", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Shadow"]);
                }
                else if (string.Equals(field, "Alignment", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Alignment"]);
                }
                else if (string.Equals(field, "MarginL", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["MarginL"]);
                }
                else if (string.Equals(field, "MarginR", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["MarginR"]);
                }
                else if (string.Equals(field, "MarginV", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["MarginV"]);
                }
                else if (string.Equals(field, "Encoding", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Encoding"]);
                }
                else
                {
                    return (string.Empty);
                }
            }

            public void Field(string field, string value)
            {
                if (string.Equals(field, "Name", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Name"] = value;
                }
                else if (string.Equals(field, "Fontname", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Fontname"] = value;
                }
                else if (string.Equals(field, "Fontsize", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Fontsize"] = value;
                }
                else if (string.Equals(field, "PrimaryColour", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["PrimaryColour"] = value;
                }
                else if (string.Equals(field, "SecondaryColour", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["SecondaryColour"] = value;
                }
                else if (string.Equals(field, "TertiaryColour", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["TertiaryColour"] = value;
                }
                else if (string.Equals(field, "OutlineColour", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["OutlineColour"] = value;
                }
                else if (string.Equals(field, "BackColour", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["BackColour"] = value;
                }
                else if (string.Equals(field, "Bold", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Bold"] = value;
                }
                else if (string.Equals(field, "Italic", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Italic"] = value;
                }
                else if (string.Equals(field, "Underline", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Underline"] = value;
                }
                else if (string.Equals(field, "StrikeOut", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["StrikeOut"] = value;
                }
                else if (string.Equals(field, "ScaleX", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["ScaleX"] = value;
                }
                else if (string.Equals(field, "ScaleY", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["ScaleY"] = value;
                }
                else if (string.Equals(field, "Spacing", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Spacing"] = value;
                }
                else if (string.Equals(field, "Angle", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Angle"] = value;
                }
                else if (string.Equals(field, "BorderStyle", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["BorderStyle"] = value;
                }
                else if (string.Equals(field, "Outline", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Outline"] = value;
                }
                else if (string.Equals(field, "Shadow", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Shadow"] = value;
                }
                else if (string.Equals(field, "Alignment", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Alignment"] = value;
                }
                else if (string.Equals(field, "MarginL", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["MarginL"] = value;
                }
                else if (string.Equals(field, "MarginR", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["MarginR"] = value;
                }
                else if (string.Equals(field, "MarginV", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["MarginV"] = value;
                }
                else if (string.Equals(field, "Encoding", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Encoding"] = value;
                }
            }

            // Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, 
            //         Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, 
            //         Shadow, Alignment, MarginL, MarginR, MarginV, Encoding
            public string Raw;
            public string Name
            {
                get { return Field("Name"); }
                set { Field("Name", value); }
            }
            public string Fontname
            {
                get { return Field("Fontname"); }
                set { Field("Fontname", value); }
            }
            public string Fontsize
            {
                get { return Field("Fontsize"); }
                set { Field("Fontsize", value); }
            }
            public string PrimaryColour
            {
                get { return Field("PrimaryColour"); }
                set { Field("PrimaryColour", value); }
            }
            public string SecondaryColour
            {
                get { return Field("SecondaryColour"); }
                set { Field("SecondaryColour", value); }
            }
            public string TertiaryColour
            {
                get { return Field("TertiaryColour"); }
                set { Field("TertiaryColour", value); }
            }
            public string OutlineColor
            {
                get { return Field("OutlineColor"); }
                set { Field("OutlineColor", value); }
            }
            public string BackColour
            {
                get { return Field("BackColour"); }
                set { Field("BackColour", value); }
            }
            public string Bold
            {
                get { return Field("Bold"); }
                set { Field("Bold", value); }
            }
            public string Italic
            {
                get { return Field("Italic"); }
                set { Field("Italic", value); }
            }
            public string Underline
            {
                get { return Field("Underline"); }
                set { Field("Underline", value); }
            }
            public string StrikeOut
            {
                get { return Field("StrikeOut"); }
                set { Field("StrikeOut", value); }
            }
            public string ScaleX
            {
                get { return Field("ScaleX"); }
                set { Field("ScaleX", value); }
            }
            public string ScaleY
            {
                get { return Field("ScaleY"); }
                set { Field("ScaleY", value); }
            }
            public string Spacing
            {
                get { return Field("Spacing"); }
                set { Field("Spacing", value); }
            }
            public string Angle
            {
                get { return Field("Angle"); }
                set { Field("Angle", value); }
            }
            public string BorderStyle
            {
                get { return Field("BorderStyle"); }
                set { Field("BorderStyle", value); }
            }
            public string Outline
            {
                get { return Field("Outline"); }
                set { Field("Outline", value); }
            }
            public string Shadow
            {
                get { return Field("Shadow"); }
                set { Field("Shadow", value); }
            }
            public string Alignment
            {
                get { return Field("Alignment"); }
                set { Field("Alignment", value); }
            }
            public string MarginL
            {
                get { return Field("MarginL"); }
                set { Field("MarginL", value); }
            }
            public string MarginR
            {
                get { return Field("MarginR"); }
                set { Field("MarginR", value); }
            }
            public string MarginV
            {
                get { return Field("MarginV"); }
                set { Field("MarginV", value); }
            }
            public string AlphaLevel
            {
                get { return Field("AlphaLevel"); }
                set { Field("AlphaLevel", value); }
            }
            public string Encoding
            {
                get { return Field("Encoding"); }
                set { Field("Encoding", value); }
            }
        }

        public class EVENT
        {
            private SortedDictionary<string, string> fields = new SortedDictionary<string, string>();
            public SortedDictionary<string, string> Fields
            {
                get { return fields; }
            }

            public string Field(string field)
            {
                if (!fields.ContainsKey(field)) return (string.Empty);

                else if (string.Equals(field, "Marked", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Marked"]);
                }
                else if (string.Equals(field, "Layer", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Layer"]);
                }
                else if (string.Equals(field, "Start", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Start"]);
                }
                else if (string.Equals(field, "End", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["End"]);
                }
                else if (string.Equals(field, "Style", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Style"]);
                }
                else if (string.Equals(field, "Name", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Name"]);
                }
                else if (string.Equals(field, "MarginL", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["MarginL"]);
                }
                else if (string.Equals(field, "MarginR", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["MarginR"]);
                }
                else if (string.Equals(field, "MarginV", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["MarginV"]);
                }
                else if (string.Equals(field, "Effect", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Effect"]);
                }
                else if (string.Equals(field, "Text", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Text"]);
                }
                else if (string.Equals(field, "Actor", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Actor"]);
                }
                else if (string.Equals(field, "Type", StringComparison.CurrentCultureIgnoreCase))
                {
                    return (fields["Type"]);
                }
                else
                {
                    return (string.Empty);
                }
            }

            public void Field(string field, string value)
            {
                if (string.Equals(field, "Marked", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.Marked = value;
                    fields["Marked"] = value;
                }
                else if (string.Equals(field, "Layer", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.Layer = value;
                    fields["Layer"] = value;
                }
                else if (string.Equals(field, "Start", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.Start = value;
                    fields["Start"] = value;
                }
                else if (string.Equals(field, "End", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.End = value;
                    fields["End"] = value;
                }
                else if (string.Equals(field, "Style", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.Style = value;
                    fields["Style"] = value;
                }
                else if (string.Equals(field, "Name", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.Name = value;
                    fields["Name"] = value;
                }
                else if (string.Equals(field, "MarginL", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.MarginL = value;
                    fields["MarginL"] = value;
                }
                else if (string.Equals(field, "MarginR", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.MarginR = value;
                    fields["MarginR"] = value;
                }
                else if (string.Equals(field, "MarginV", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.MarginV = value;
                    fields["MarginV"] = value;
                }
                else if (string.Equals(field, "Effect", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.Effect = value;
                    fields["Effect"] = value;
                }
                else if (string.Equals(field, "Text", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.Text = value;
                    fields["Text"] = value;
                }
                else if (string.Equals(field, "Actor", StringComparison.CurrentCultureIgnoreCase))
                {
                    //this.Actor = value;
                    fields["Actor"] = value;
                }
                else if (string.Equals(field, "Type", StringComparison.CurrentCultureIgnoreCase))
                {
                    fields["Type"] = value;
                }
            }

            public string Raw;
            public string Type
            {
                get { return Field("Type"); }
                set { Field("Type", value); }
            }
            public string Marked
            {
                get { return Field("Marked"); }
                set { Field("Marker", value); }
            }
            public string Layer
            {
                get { return Field("Layer"); }
                set { Field("Layer", value); }
            }
            public string Start
            {
                get { return Field("Start"); }
                set { Field("Start", value); }
            }
            public string End
            {
                get { return Field("End"); }
                set { Field("End", value); }
            }
            public string Style
            {
                get { return Field("Style"); }
                set { Field("Style", value); }
            }
            public string Name
            {
                get { return Field("Name"); }
                set { Field("Name", value); }
            }
            public string MarginL
            {
                get { return Field("MarginL"); }
                set { Field("MarginL", value); }
            }
            public string MarginR
            {
                get { return Field("MarginR"); }
                set { Field("MarginR", value); }
            }
            public string MarginV
            {
                get { return Field("MarginV"); }
                set { Field("MarginV", value); }
            }
            public string Effect
            {
                get { return Field("Effect"); }
                set { Field("Effect", value); }
            }
            public string Text
            {
                get { return Field("Text"); }
                set { Field("Text", value); }
            }
            public string Actor
            {
                get { return Field("Actor"); }
                set { Field("Actor", value); }
            }
            private string translated = string.Empty;
            public string Translated
            {
                get { return translated; }
                set {
                    var line = value;
                    var match_s = Regex.Matches(Text, @"\{ *\\.*?\}", RegexOptions.IgnoreCase);
                    var match_t = Regex.Matches(line, @"\{ *\\.*?\}", RegexOptions.IgnoreCase);
                    for (int m = 0; m < match_s.Count; m++)
                    {
                        var match = match_s[m].Value;
                        line = line.Replace(match_t[m].Value, match);
                    }
                    translated = line.Replace("\\ ", "\\").Replace(" \\", "\\").Replace("\\n", "\\N").Replace(" {", "{").Replace("} ", "}");
                }
            }
        }

        public SCRIPTINFO ScriptInfo = new SCRIPTINFO();
        private List<string> StylesRaw = new List<string>();
        private List<string> FontsRaw = new List<string>();
        private List<string> GraphicsRaw = new List<string>();
        private List<string> EventsRaw = new List<string>();

        private string style_format;
        private string[] style_fields;
        public string[] StyleFields
        {
            get { return style_fields; }
        }
        private string event_format;
        private string[] event_fields;
        public string[] EventFields
        {
            get { return event_fields; }
        }

        private List<EVENT> events = new List<EVENT>();
        public List<EVENT> Events
        {
            get { return events; }
            set { events = value; }
        }

        private List<STYLE> styles = new List<STYLE>();
        public List<STYLE> Styles
        {
            get { return styles; }
            set { styles = value; }
        }

        private void SetStyle(ref STYLE e, string field, string value)
        {
            if (string.Equals(field, "Name", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Name = value;
            }
            else if (string.Equals(field, "Fontname", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Fontname = value;
            }
            else if (string.Equals(field, "Fontsize", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Fontsize = value;
            }
            else if (string.Equals(field, "PrimaryColour", StringComparison.CurrentCultureIgnoreCase))
            {
                e.PrimaryColour = value;
            }
            else if (string.Equals(field, "SecondaryColour", StringComparison.CurrentCultureIgnoreCase))
            {
                e.SecondaryColour = value;
            }
            else if (string.Equals(field, "OutlineColour", StringComparison.CurrentCultureIgnoreCase))
            {
                e.OutlineColor = value;
            }
            else if (string.Equals(field, "BackColour", StringComparison.CurrentCultureIgnoreCase))
            {
                e.BackColour = value;
            }
            else if (string.Equals(field, "Bold", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Bold = value;
            }
            else if (string.Equals(field, "Italic", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Italic = value;
            }
            else if (string.Equals(field, "Underline", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Underline = value;
            }
            else if (string.Equals(field, "StrikeOut", StringComparison.CurrentCultureIgnoreCase))
            {
                e.StrikeOut = value;
            }
            else if (string.Equals(field, "ScaleX", StringComparison.CurrentCultureIgnoreCase))
            {
                e.ScaleX = value;
            }
            else if (string.Equals(field, "ScaleY", StringComparison.CurrentCultureIgnoreCase))
            {
                e.ScaleY = value;
            }
            else if (string.Equals(field, "Spacing", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Spacing = value;
            }
            else if (string.Equals(field, "Angle", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Angle = value;
            }
            else if (string.Equals(field, "BorderStyle", StringComparison.CurrentCultureIgnoreCase))
            {
                e.BorderStyle = value;
            }
            else if (string.Equals(field, "Outline", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Outline = value;
            }
            else if (string.Equals(field, "Shadow", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Shadow = value;
            }
            else if (string.Equals(field, "Alignment", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Alignment = value;
            }
            else if (string.Equals(field, "MarginL", StringComparison.CurrentCultureIgnoreCase))
            {
                e.MarginL = value;
            }
            else if (string.Equals(field, "MarginR", StringComparison.CurrentCultureIgnoreCase))
            {
                e.MarginR = value;
            }
            else if (string.Equals(field, "MarginV", StringComparison.CurrentCultureIgnoreCase))
            {
                e.MarginV = value;
            }
            else if (string.Equals(field, "Encoding", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Encoding = value;
            }
        }

        private void SetEvent(ref EVENT e, string field, string value)
        {
            if (string.Equals(field, "Marked", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Marked = value;
            }
            else if (string.Equals(field, "Layer", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Layer = value;
            }
            else if (string.Equals(field, "Start", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Start = value;
            }
            else if (string.Equals(field, "End", StringComparison.CurrentCultureIgnoreCase))
            {
                e.End = value;
            }
            else if (string.Equals(field, "Style", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Style = value;
            }
            else if (string.Equals(field, "Name", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Name = value;
            }
            else if (string.Equals(field, "MarginL", StringComparison.CurrentCultureIgnoreCase))
            {
                e.MarginL = value;
            }
            else if (string.Equals(field, "MarginR", StringComparison.CurrentCultureIgnoreCase))
            {
                e.MarginR = value;
            }
            else if (string.Equals(field, "MarginV", StringComparison.CurrentCultureIgnoreCase))
            {
                e.MarginV = value;
            }
            else if (string.Equals(field, "Effect", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Effect = value;
            }
            else if (string.Equals(field, "Text", StringComparison.CurrentCultureIgnoreCase))
            {
                e.Text = value;
            }
        }

        private void ParseScriptInfo(string v)
        {
            if (v.StartsWith(";", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.Comments.Add(v);
            }
            else if (v.StartsWith("Title:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.Title = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Original Script:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.OriginalScript = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Original Translation:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.OriginalTranslation = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Original Editing:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.OriginalEditing = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Original Timing:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.OriginalTiming = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Synch Point:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.SynchPoint = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Script Updated By:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.ScriptUpdatedBy = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Update Details:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.UpdateDetails = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("ScriptType:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.ScriptType = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Collisions:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.Collisions = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("PlayResY:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.PlayResY = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("PlayResX:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.PlayResX = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("PlayDepth:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.PlayDepth = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Timer:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.Timer = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Style:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.Style = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Dialogue:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.Dialogue = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Comment:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.Comment = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Picture:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.Picture = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Sound:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.Sound = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Movie:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.Movie = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Command:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.Command = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("WrapStyle:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.WrapStyle = v.Split(':')[1].Trim();
            }
        }

        private void ParseStyle(string v)
        {
            if (v.StartsWith("Format:", StringComparison.CurrentCultureIgnoreCase))
            {
                style_format = v.Split(':')[1].Trim();
                style_fields = style_format.Split(',');
                for (int i = 0; i < style_fields.Length; i++)
                {
                    style_fields[i] = style_fields[i].Trim();
                }
            }
            else if (v.StartsWith("Style:", StringComparison.CurrentCultureIgnoreCase))
            {
                var style = new STYLE();
                style.Raw = v.Split(':')[1].Trim();

                var fields = style_fields;
                var values = style.Raw.Split(',');

                for (int i = 0; i < fields.Length; i++)
                {
                    style.Field(fields[i], values[i].Trim());
                    //SetStyle(ref style, fields[i], values[i].Trim());
                }

                styles.Add(style);
            }
        }

        private void ParseEvent(string v)
        {
            if (v.StartsWith("Format:", StringComparison.CurrentCultureIgnoreCase))
            {
                event_format = v.Split(':')[1].Trim();
                event_fields = ("Type," + event_format).Split(',');
                for (int i = 0; i < event_fields.Length; i++)
                {
                    event_fields[i] = event_fields[i].Trim();
                }
                //var el = event_fields.ToList();
                //el.Insert(0, "Type");
                //event_fields = el.ToArray();
            }
            else if (v.StartsWith("Dialogue:", StringComparison.CurrentCultureIgnoreCase))
            {
                var evt = new EVENT();
                evt.Raw = v.Substring("Dialogue:".Length).Trim();

                var fields = event_fields;
                var values = ("Dialogue," + evt.Raw).Split(',');

                for (int i = 0; i < fields.Length - 1; i++)
                {
                    evt.Field(fields[i], values[i].Trim());
                    //SetEvent(ref evt, fields[i], values[i].Trim());
                }
                evt.Field(fields[fields.Length - 1], string.Join(",", values.Skip(fields.Length - 1)));
                //SetEvent(ref evt, fields[fields.Length - 1], string.Join(",", values.Skip(fields.Length - 1)));

                events.Add(evt);
            }
            else if (v.StartsWith("Comment:", StringComparison.CurrentCultureIgnoreCase))
            {
                var evt = new EVENT();
                evt.Raw = v.Substring("Comment:".Length).Trim();

                var fields = event_fields;
                var values = ("Comment," + evt.Raw).Split(',');

                for (int i = 0; i < fields.Length - 1; i++)
                {
                    evt.Field(fields[i], values[i].Trim());
                }
                evt.Field(fields[fields.Length - 1], string.Join(",", values.Skip(fields.Length - 1)));

                events.Add(evt);
            }
        }

        private void ParseFont(string v)
        {
            FontsRaw.Add(v);
        }

        private void ParseGraphic(string v)
        {
            GraphicsRaw.Add(v);
        }

        public ASS()
        {
            if (styles == null) styles = new List<STYLE>();
            if (events == null) events = new List<EVENT>();
        }

        public void Load(string ass)
        {
            ScriptInfo.Clear();
            styles.Clear();
            events.Clear();

            if (File.Exists(ass))
            {
                var lines = File.ReadAllLines(ass);
                Load(lines);
            }
        }

        public void Load(string[] lines)
        {
            ScriptInfo = new SCRIPTINFO();

            if (lines.Length <= 0) return;

            bool valid = Regex.IsMatch(lines[0], @"^\[Script Info\]", RegexOptions.IgnoreCase);
            if (!valid) return;
            ScriptInfo.Raw.Add(lines[0]);

            Sections section = Sections.ScriptInfo;

            for (int i = 1; i < lines.Length; i++)
            {
                string line = lines[i].Trim();

                #region Detect Where Section
                valid = Regex.IsMatch(line, @"^\[v4\+* Styles\+*\]", RegexOptions.IgnoreCase);
                if (valid)
                {
                    section = Sections.Styles;
                    StylesRaw.Add(line);
                    continue;
                }

                valid = Regex.IsMatch(line, @"^\[Events\]", RegexOptions.IgnoreCase);
                if (valid)
                {
                    section = Sections.Events;
                    EventsRaw.Add(line);
                    continue;
                }

                valid = Regex.IsMatch(line, @"^\[Fonts\]", RegexOptions.IgnoreCase);
                if (valid)
                {
                    section = Sections.Fonts;
                    continue;
                }

                valid = Regex.IsMatch(line, @"^\[Graphics\]", RegexOptions.IgnoreCase);
                if (valid)
                {
                    section = Sections.Graphics;
                    continue;
                }
                #endregion

                switch (section)
                {
                    case Sections.ScriptInfo:
                        ScriptInfo.Raw.Add(line);
                        ParseScriptInfo(line);
                        break;
                    case Sections.Styles:
                        StylesRaw.Add(line);
                        ParseStyle(line);
                        break;
                    case Sections.Events:
                        EventsRaw.Add(line);
                        ParseEvent(line);
                        break;
                    case Sections.Fonts:
                        ParseFont(line);
                        break;
                    case Sections.Graphics:
                        ParseGraphic(line);
                        break;
                }
            }
        }

        public void Save(string ass_file, SaveFlags flags = SaveFlags.Merge)
        {
            if (flags.HasFlag(SaveFlags.SRT) || flags.HasFlag(SaveFlags.VTT))
            {
                SaveAsSrtVtt(ass_file, flags);
                return;
            }

            StringBuilder sb = new StringBuilder();

            #region Save Script Info Section
            for (int i = 0; i < ScriptInfo.Raw.Count; i++)
            {
                sb.AppendLine(ScriptInfo.Raw[i]);
            }
            //sb.AppendLine();
            #endregion

            #region Save Styles Section
            for (int i = 0; i < StylesRaw.Count; i++)
            {
                sb.AppendLine(StylesRaw[i]);
            }
            //sb.AppendLine();
            #endregion
            
            #region Save Fonts Section
            for (int i = 0; i < FontsRaw.Count; i++)
            {
                sb.AppendLine(FontsRaw[i]);
            }
            #endregion

            #region Save Graphics Section
            for (int i = 0; i < GraphicsRaw.Count; i++)
            {
                sb.AppendLine(GraphicsRaw[i]);
            }
            #endregion

            #region Save Events Section
            sb.AppendLine(EventsRaw[0]);
            sb.AppendLine(EventsRaw[1]);
            for (int i = 2; i < events.Count+2; i++)
            {
                var evt = events[i - 2];
                var evo = new List<string>();
                foreach (var k in EventFields)
                {
                    if (string.Equals(k, "Text", StringComparison.CurrentCultureIgnoreCase)) continue;
                    evo.Add(evt.Field(k));
                }
                string line = string.Empty;
                switch (flags)
                {
                    case SaveFlags.Merge:
                        if (string.IsNullOrEmpty(events[i - 2].Translated))
                            line = $"{string.Join(",", evo)},{events[i - 2].Text}";
                        else
                            line = $"{string.Join(",", evo)},{events[i - 2].Text}\\N{events[i-2].Translated}";
                        break;
                    case SaveFlags.Replace:
                        if (string.IsNullOrEmpty(events[i - 2].Translated))
                            line = $"{string.Join(",", evo)}, {events[i - 2].Text}";
                        else
                            line = $"{string.Join(",", evo)}, {events[i - 2].Translated}";
                        break;
                    default:
                        break;
                }
                int idx = line.IndexOf(",");
                if (idx > 0)
                    line = $"{line.Substring(0, idx)}: {line.Substring(idx+1)}";
                sb.AppendLine(line);
            }
            sb.AppendLine();
            #endregion

            File.WriteAllText(ass_file, sb.ToString());
        }

        public void SaveAsSrtVtt(string ass_file, SaveFlags flags = SaveFlags.Merge)
        {
            StringBuilder sb = new StringBuilder();

            string ext = ".srt";

            if (flags.HasFlag(SaveFlags.VTT))
            {
                ext = ".vtt";

                sb.AppendLine($"WEBVTT");
                sb.AppendLine();
                sb.AppendLine($"video::cue {{");
                sb.AppendLine($"  background-color: transparent;");
                sb.AppendLine($"  color: yellow;");
                sb.AppendLine($"}}");
                sb.AppendLine();
                sb.AppendLine();
            }

            for (int i = 0; i < events.Count; i++)
            {
                sb.AppendLine($"{i + 1}");
                if(flags.HasFlag(SaveFlags.VTT))
                {
                    sb.AppendLine($"{events[i].Start.Replace(",", ".")} --> {events[i].End.Replace(",", ".")}");
                }
                else
                {
                    sb.AppendLine($"{events[i].Start.Replace(".", ",")} --> {events[i].End.Replace(".", ",")}");
                }

                string line = string.Empty;
                if (flags.HasFlag(SaveFlags.Replace))
                {
                    if (string.IsNullOrEmpty(events[i].Translated))
                        line = events[i].Text;
                    else
                        line = events[i].Translated;
                }
                else if(flags.HasFlag(SaveFlags.Replace))
                {
                    if (string.IsNullOrEmpty(events[i].Translated))
                        line = events[i].Text;
                    else
                        line = $"{events[i].Text}\\N{events[i].Translated}";
                }
                var lines = line.Split(new string[] { "\\N", "\r\n", "\n\r", "\n", "\r" }, StringSplitOptions.None);
                foreach (var l in lines)
                {
                    sb.AppendLine($"{Regex.Replace(l, @"\{ *\\.*?\}", "", RegexOptions.IgnoreCase)}");
                }
                sb.AppendLine();
            }
            
            File.WriteAllText(Path.ChangeExtension(ass_file, ext), sb.ToString());
        }

    }
}

