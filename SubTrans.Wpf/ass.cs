using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubTitles
{
    public class YouTubeSubtitleItem
    {
        public DateTime last_time;
        public string last_text;
        public DateTime curr_time;
        public string curr_text;
    }

    public class ASS
    {
        public static string Style_Default_ENG = @"Style: Default,Tahoma,20,&H19000000,&H19843815,&H37A4F2F7,&HA0A6A6A8,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string Style_Note_ENG = @"Style: Note,Times New Roman,22,&H19FFF907,&H19DC16C8,&H371E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string Style_Title_ENG = @"Style: Title,Arial,28,&H190055FF,&H1948560E,&H37EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";

        public static string Style_Default_CHS = @"Style: Default,微软雅黑,20,&H19000000,&H19843815,&H37A4F2F7,&HA0A6A6A8,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string Style_Note_CHS = @"Style: Note,宋体,22,&H19FFF907,&H19DC16C8,&H371E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string Style_Title_CHS = @"Style: Title,全真細隸書,28,&H190055FF,&H1948560E,&H37EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";

        private enum Sections { ScriptInfo = 0, Styles = 1, Events = 2, Fonts = 3, Graphics = 4, Unknown = -1 };

        [Flags]
        public enum SaveFlags { None = 0, Replace = 1, Merge = 2, SRT = 4, VTT = 8, BOM = 256 };

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
                Raw.Clear();
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
            // Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, 
            //         Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, 
            //         Shadow, Alignment, MarginL, MarginR, MarginV, Encoding
            private string[] FIELD_ALLOWED = new string[] {
                "Name", "Fontname", "Fontsize",
                "PrimaryColour", "SecondaryColour", "TertiaryColour", "OutlineColour", "BackColour",
                "Bold", "Italic", "Underline", "StrikeOut",
                "ScaleX", "ScaleY", "Spacing", "Angle", "BorderStyle", "Outline", "Shadow", "Alignment",
                "MarginL", "MarginR", "MarginV",
                "Encoding"
            };

            private SortedDictionary<string, string> fields = new SortedDictionary<string, string>();
            public SortedDictionary<string, string> Fields
            {
                get { return fields; }
            }

            public string Field(string field)
            {
                if (string.IsNullOrEmpty(field)) return (string.Empty);

                if (!fields.ContainsKey(field)) return (string.Empty);

                if (FIELD_ALLOWED.Contains(field, StringComparer.CurrentCultureIgnoreCase))
                {
                    return (fields[field]);
                }
                else
                {
                    return (string.Empty);
                }
            }

            public void Field(string field, string value)
            {
                if (string.IsNullOrEmpty(field)) return;

                if (FIELD_ALLOWED.Contains(field, StringComparer.CurrentCultureIgnoreCase))
                {
                    fields[field] = value;
                }
            }

            #region Style Properties
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
            #endregion
        }

        public class EVENT : INotifyPropertyChanged
        {
            // Format: Marked, Layer, Start, End, Style, Name,
            //         MarginL, MarginR, MarginV, Effect, Text,
            //         Actor, Type
            private string[] FIELD_ALLOWED = new string[] {
                "Marked", "Layer", "Start", "End", "Style", "Name",
                "MarginL", "MarginR", "MarginV", "Effect", "Text",
                "Actor", "Type"
            };

            public event PropertyChangedEventHandler PropertyChanged;
            private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            private SortedDictionary<string, string> fields = new SortedDictionary<string, string>();
            public SortedDictionary<string, string> Fields
            {
                get { return fields; }
            }

            public string Field(string field)
            {
                if(string.IsNullOrEmpty(field)) return(string.Empty);

                if (!fields.ContainsKey(field)) return (string.Empty);

                if (FIELD_ALLOWED.Contains(field, StringComparer.CurrentCultureIgnoreCase))
                {
                    return (fields[field]);
                }
                else
                {
                    return (string.Empty);
                }
            }

            public void Field(string field, string value)
            {
                if (string.IsNullOrEmpty(field)) return;

                if (FIELD_ALLOWED.Contains(field, StringComparer.CurrentCultureIgnoreCase))
                {
                    fields[field] = value;
                }
            }

            #region Event Properties
            private string id = string.Empty;
            public string ID
            {
                get { return id; }
                set { id = value; }
            }
            private string raw = string.Empty;
            public string Raw
            {
                get { return raw; }
                set { raw = value; }
            }
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
                set { Field("Text", value); NotifyPropertyChanged("Text"); }
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
                    for (int m = 0; m < Math.Min(match_s.Count, match_t.Count); m++)
                    {
                        var match = match_s[m].Value;
                        line = line.Replace(match_t[m].Value, match);
                    }
                    translated = line.Replace("\\ ", "\\").Replace(" \\", "\\").Replace("\\n", "\\N").Replace(" {", "{").Replace("} ", "}");
                    NotifyPropertyChanged("Translated");
                }
            }
            #endregion
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

                evt.ID = $"{events.Count + 1}";
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

                evt.ID = $"{events.Count + 1}";
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

        public ASS(string ass)
        {
            if (styles == null) styles = new List<STYLE>();
            if (events == null) events = new List<EVENT>();

            new Action(async () => {
                await Load(ass);
            }).Invoke(); 
        }

        public ASS(string[] lines)
        {
            if (styles == null) styles = new List<STYLE>();
            if (events == null) events = new List<EVENT>();

            Load(lines);
        }

        public async Task Load(string ass)
        {
            ScriptInfo.Clear();
            styles.Clear();
            events.Clear();

            if (File.Exists(ass))
            {
                var ext = Path.GetExtension(ass).ToLower();
                var fn = Path.GetFileNameWithoutExtension(ass);
                var lines = File.ReadAllLines(ass);
                if (ext.Equals(".srt"))
                {
                    await LoadFromSrt(lines, fn);
                }
                else if (ext.Equals(".ass") || ext.Equals(".ssa"))
                {
                    Load(lines);
                }
            }
        }

        public void Load(string[] lines)
        {
            ScriptInfo = new SCRIPTINFO();
            StylesRaw.Clear();
            FontsRaw.Clear();
            GraphicsRaw.Clear();
            EventsRaw.Clear();

            Events.Clear();
            Styles.Clear();

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

        public async Task LoadFromSrt(string[] contents, string title="Untitled")
        {
            var srt = new SRTContent();
            await srt.Load(contents);
            var lines = srt.ToAss(title);
            Load(lines.ToArray());
        }

        public void LoadFromYouTube(string[] contents, string title="Untitled")
        {
            var last_time = new DateTime(0);
            var last_text = string.Empty;
            var curr_time = new DateTime(0);
            var curr_text = string.Empty;

            var re_src = new Regex(@"^\d+:\d+$", RegexOptions.IgnoreCase);

            if (contents is string[])
            {
                List<string> lines = new List<string>();
                lines.Add($"[Script Info]\n");
                lines.Add($"Title: {title}\n");
                lines.Add($"\n");
                lines.Add($"[V4+ Styles]\n");
                lines.Add($"Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding\n");
                lines.Add($"{Style_Default_CHS}\n");
                lines.Add($"{Style_Note_CHS}\n");
                lines.Add($"{Style_Title_CHS}\n");
                lines.Add($"\n");
                lines.Add($"[Events]\n");
                lines.Add($"Format: Layer, Start, End, Style, Actor, MarginL, MarginR, MarginV, Effect, Text\n");

                var c = contents.Length;
                int th = 0, tm = 0, ts = 0;

                for (int i = 0; i < c; i++)
                {
                    var l = contents[i];
                    if (re_src.IsMatch(l))
                    {
                        last_time = curr_time;
                        last_text = curr_text;

                        var tt = l.Replace("\n", "").Split(':');
                        var tc = tt.Length;
                        if (tc == 2)
                        {
                            th = Convert.ToInt32(tt[0]) / 60;
                            tm = Convert.ToInt32(tt[0]) - th * 60;
                            ts = Convert.ToInt32(tt[1]);
                        }
                        else if (tc == 3)
                        {
                            th = Convert.ToInt32(tt[0]);
                            tm = Convert.ToInt32(tt[1]);
                            ts = Convert.ToInt32(tt[2]);
                        }

                        curr_time = new DateTime(1, 1, 1, th, tm, ts);
                        curr_text = contents[i + 1].Replace("\n", "");

                        lines.Add($"Dialogue: 0,{last_time.ToString("HH:mm:ss.ff")},{curr_time.ToString("HH:mm:ss.ff")},Default,NTP,0000,0000,0000,,{last_text}");
                        //lines.Add(new YouTubeSubtitleItem() { last_time = last_time, curr_time = curr_time, last_text = last_text });
                    }
                }
                var end_time = curr_time;
                end_time = end_time + TimeSpan.FromSeconds(10);
                lines.Add($"Dialogue: 0,{curr_time.ToString("HH:mm:ss.ff")},{end_time.ToString("HH:mm:ss.ff")},Default,NTP,0000,0000,0000,,{curr_text}");
                //lines.Add(new YouTubeSubtitleItem() { last_time = curr_time, curr_time = end_time, last_text = curr_text });

                Load(lines.ToArray());
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
                foreach (var k in event_fields)
                {
                    if (string.Equals(k, "ID", StringComparison.CurrentCultureIgnoreCase)) continue;
                    if (string.Equals(k, "Text", StringComparison.CurrentCultureIgnoreCase)) continue;
                    evo.Add(evt.Field(k));
                }
                string line = string.Empty;
                switch (flags)
                {
                    case SaveFlags.None:
                        line = $"{string.Join(",", evo)},{events[i - 2].Text}";
                        break;
                    case SaveFlags.Merge:
                        if (string.IsNullOrEmpty(events[i - 2].Translated))
                            line = $"{string.Join(",", evo)},{events[i - 2].Text}";
                        else
                            line = $"{string.Join(",", evo)},{events[i - 2].Text}\\N{events[i-2].Translated}";
                        break;
                    case SaveFlags.Replace:
                        if (string.IsNullOrEmpty(events[i - 2].Translated))
                            line = $"{string.Join(",", evo)},{events[i - 2].Text}";
                        else
                            line = $"{string.Join(",", evo)},{events[i - 2].Translated}";
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

            File.WriteAllText(ass_file, sb.ToString(), new UTF8Encoding(true));
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

                if (flags.HasFlag(SaveFlags.None))
                {
                    line = events[i].Text;
                }
                else if (flags.HasFlag(SaveFlags.Replace))
                {
                    if (string.IsNullOrEmpty(events[i].Translated))
                        line = events[i].Text;
                    else
                        line = events[i].Translated;
                }
                else if(flags.HasFlag(SaveFlags.Merge))
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
            
            File.WriteAllText(Path.ChangeExtension(ass_file, ext), sb.ToString(), new UTF8Encoding(true));
        }

    }
}

