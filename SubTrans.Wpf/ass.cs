using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SubTrans
{
    public class YouTubeSubtitleItem
    {
        public DateTime last_time;
        public string last_text;
        public DateTime curr_time;
        public string curr_text;
    }

    public class LRCSubtitleItem
    {
        public TimeSpan TimeFrom { get; set; }
        public TimeSpan TimeTo { get; set; }
        public string Text { get; set; }
    }

    public class LRCSubtitle
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public string Alias { get; set; }        
        public string Author { get; set; }
        public string Comment { get; set; }
        public List<LRCSubtitleItem> Events { get; set; } = new List<LRCSubtitleItem>();
    }

    public class AssCustomStyle
    {
        public ASS.STYLE Style { get; set; } = ASS.STYLE.Parse(AssStyle.ENG_Default, AssStyle.FormatOfStyle);
        public string Color { get { return ($"{{\\1c{Style.Field("PrimaryColour")}&\\2c{Style.Field("SecondaryColour")}&\\3c{Style.Field("OutlineColour")}&\\4c{Style.Field("BackColour")}&}}"); } }
        public string InvertColor { get { return ($"{{\\1c{Style.Field("OutlineColour")}&\\2c{Style.Field("SecondaryColour")}&\\3c{Style.Field("PrimaryColour")}&\\4c{Style.Field("BackColour")}&}}"); } }
        public string Font { get { return ($"{{\\fn{Style.Field("Fontname")}\\fs{Style.Field("Fontsize")}}}"); } }
    }

    public static class AssStyle
    {
        private static string FontSize { get; } = "24";

        private static string PrimaryColour { get; } = "&H20FFFFFF";
        private static string SecondaryColour { get; } = "&H20FFFFFF";
        private static string OutlineColour { get; } = "&H40000000";
        private static string BackColour { get; } = "&HA0BCBCBD";

        private static string Bold { get; } = "0";
        private static string Italic { get; } = "0";
        private static string Underline { get; } = "0";
        private static string StrikeOut { get; } = "0";

        private static string ScaleX { get; } = "100";
        private static string ScaleY { get; } = "100";

        private static string Spacing { get; } = "0";
        private static string Angle { get; } = "0";

        private static string BorderStyle { get; } = "1";
        private static string Outline { get; } = "2";
        private static string Shadow { get; } = "2";

        private static string Alignment { get; } = "2";
        private static string MarginL { get; } = "10";
        private static string MarginR { get; } = "10";
        private static string MarginV { get; } = "10";

        private static string Encoding { get; } = "1";

        private static string[] DefaultStyles = new string[]
        {
            "Default", "DefaultM", "DefaultF", "Title", "Note", "Comment",
            //"DefaultAlt", "DefaultAltM", "DefaultAltF", "TitleAlt", "NoteAlt", "CommentAlt"
        };

        public static bool Contains(string style)
        {
            return(DefaultStyles.Contains(style, StringComparer.CurrentCultureIgnoreCase));
        }

        public static string FormatOfStyle { get; set; } = $"Format: Name, Fontname, Fontsize, PrimaryColour, SecondaryColour, OutlineColour, BackColour, Bold, Italic, Underline, StrikeOut, ScaleX, ScaleY, Spacing, Angle, BorderStyle, Outline, Shadow, Alignment, MarginL, MarginR, MarginV, Encoding";
        public static string FormatOfEvent { get; set; } = $"Format: Layer, Start, End, Style, Actor, MarginL, MarginR, MarginV, Effect, Text";

        public static string ENG_Default { get; set; } = @"Style: Default,Lucida Calligraphy,24,&H20FFFFFF,&H20FFFFFF,&H40000000,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string ENG_DefaultM { get; set; } = @"Style: DefaultM,Lucida Calligraphy,24,&H20FFFFFF,&H20AD4C00,&H40FF7F7F,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string ENG_DefaultF { get; set; } = @"Style: DefaultF,Lucida Calligraphy,24,&H20FFFFFF,&H20843815,&H40202020,&HA0A6A6A8,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string ENG_Title { get; set; } = @"Style: Title,Segoe,28,&H200055FF,&H2048560E,&H40EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,4,2,2,10,10,10,1";
        public static string ENG_Note { get; set; } = @"Style: Note,Times New Roman,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string ENG_Comment { get; set; } = @"Style: Comment,Times New Roman,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string ENG_Color { get; set; } = @"{\1c&H20FFF0F0&\2c&H20843815&\3c&H20000000&\4c&HA0A6A6A8&}";
        public static string ENG_Font { get; set; } = @"{\fnLucida Calligraphy\fs24}";

        public static string ENG_DefaultAlt { get; set; } = @"Style: DefaultAlt,Lucida Calligraphy,24,&H20FFFFFF,&H20FFFFFF,&H40000040,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string ENG_DefaultAltM { get; set; } = @"Style: DefaultAltM,Lucida Calligraphy,24,&H20FFFFFF,&H20AD4C00,&H40FF7F7F,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string ENG_DefaultAltF { get; set; } = @"Style: DefaultAltF,Lucida Calligraphy,24,&H20FFFFFF,&H20843815,&H40202020,&HA0A6A6A8,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string ENG_TitleAlt { get; set; } = @"Style: TitleAlt,Segoe,28,&H200055FF,&H2048560E,&H40EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,4,2,2,10,10,10,1";
        public static string ENG_NoteAlt { get; set; } = @"Style: NoteAlt,Times New Roman,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string ENG_CommentAlt { get; set; } = @"Style: CommentAlt,Times New Roman,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string ENG_ColorAlt { get; set; } = @"{\1c&H20FFF0F0&\2c&H20843815&\3c&H20000000&\4c&HA0A6A6A8&}";
        public static string ENG_FontAlt { get; set; } = @"{\fnLucida Calligraphy\fs24}";

        public static string CHS_Default { get; set; } = @"Style: Default,更纱黑体 SC,24,&H20FFFFFF,&H20FFFFFF,&H40000000,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHS_DefaultM { get; set; } = @"Style: DefaultM,更纱黑体 SC,24,&H20FFFFFF,&H20AD4C00,&H40FF7F7F,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHS_DefaultF { get; set; } = @"Style: DefaultF,更纱黑体 SC,24,&H20FFFFFF,&H20AD4C00,&H407F7FFF,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHS_Title { get; set; } = @"Style: Title,更纱黑体 SC,28,&H200055FF,&H2048560E,&H40EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,4,2,2,10,10,10,1";
        public static string CHS_Note { get; set; } = @"Style: Note,宋体,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHS_Comment { get; set; } = @"Style: Comment,宋体,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHS_Color { get; set; } = @"{\1c&H20FFF0F0&\2c&H20843815&\3c&H20000000&\4c&HA0A6A6A8&}";
        public static string CHS_Font { get; set; } = @"{\fn更纱黑体 SC\fs24}";

        public static string CHS_DefaultAlt { get; set; } = @"Style: DefaultAlt,更纱黑体 SC,24,&H20FFFFFF,&H20FFFFFF,&H40000040,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHS_DefaultAltM { get; set; } = @"Style: DefaultAltM,更纱黑体 SC,24,&H20FFFFFF,&H20AD4C00,&H40FF7F7F,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHS_DefaultAltF { get; set; } = @"Style: DefaultAltF,更纱黑体 SC,24,&H20FFFFFF,&H20AD4C00,&H407F7FFF,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHS_TitleAlt { get; set; } = @"Style: TitleAlt,更纱黑体 SC,28,&H200055FF,&H2048560E,&H40EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,4,2,2,10,10,10,1";
        public static string CHS_NoteAlt { get; set; } = @"Style: NoteAlt,宋体,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHS_CommentAlt { get; set; } = @"Style: CommentAlt,宋体,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHS_ColorAlt { get; set; } = @"{\1c&H20FFF0F0&\2c&H20843815&\3c&H20000000&\4c&HA0A6A6A8&}";
        public static string CHS_FontAlt { get; set; } = @"{\fn更纱黑体 SC\fs24}";

        public static string CHT_Default { get; set; } = @"Style: Default,Sarasa Gothic TC,24,&H20FFFFFF,&H20FFFFFF,&H40000000,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string CHT_DefaultM { get; set; } = @"Style: DefaultM,Sarasa Gothic TC,24,&H20FFFFFF,&H20AD4C00,&H40FF7F7F,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string CHT_DefaultF { get; set; } = @"Style: DefaultF,Sarasa Gothic TC,24,&H20FFFFFF,&H20AD4C00,&H407F7FFF,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string CHT_Title { get; set; } = @"Style: Title,Sarasa Gothic TC,28,&H200055FF,&H2048560E,&H40EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,4,2,2,10,10,10,1";
        public static string CHT_Note { get; set; } = @"Style: Note,PMingLiU,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHT_Comment { get; set; } = @"Style: Comment,PMingLiU,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHT_Color { get; set; } = @"{\1c&H20FFF0F0&\2c&H20843815&\3c&H20000000&\4c&HA0A6A6A8&}";
        public static string CHT_Font { get; set; } = @"{\fnSarasa Gothic TC\fs24}";

        public static string CHT_DefaultAlt { get; set; } = @"Style: DefaultAlt,Sarasa Gothic TC,24,&H20FFFFFF,&H20FFFFFF,&H40000040,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string CHT_DefaultAltM { get; set; } = @"Style: DefaultAltM,Sarasa Gothic TC,24,&H20FFFFFF,&H20AD4C00,&H40FF7F7F,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string CHT_DefaultAltF { get; set; } = @"Style: DefaultAltF,Sarasa Gothic TC,24,&H20FFFFFF,&H20AD4C00,&H407F7FFF,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string CHT_TitleAlt { get; set; } = @"Style: TitleAlt,Sarasa Gothic TC,28,&H200055FF,&H2048560E,&H40EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,4,2,2,10,10,10,1";
        public static string CHT_NoteAlt { get; set; } = @"Style: NoteAlt,PMingLiU,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHT_CommentAlt { get; set; } = @"Style: CommentAlt,PMingLiU,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string CHT_ColorAlt { get; set; } = @"{\1c&H20FFF0F0&\2c&H20843815&\3c&H20000000&\4c&HA0A6A6A8&}";
        public static string CHT_FontAlt { get; set; } = @"{\fnSarasa Gothic TC\fs24}";

        public static string JPN_Default { get; set; } = @"Style: Default,Sarasa Gothic J,24,&H20FFFFFF,&H20FFFFFF,&H40000000,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string JPN_DefaultM { get; set; } = @"Style: DefaultM,Sarasa Gothic J,24,&H20FFFFFF,&H20AD4C00,&H40FF7F7F,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string JPN_DefaultF { get; set; } = @"Style: DefaultF,Sarasa Gothic J,24,&H20FFFFFF,&H20AD4C00,&H407F7FFF,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string JPN_Title { get; set; } = @"Style: Title,Sarasa Gothic J,28,&H200055FF,&H2048560E,&H40EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,4,2,2,10,10,10,1";
        public static string JPN_Note { get; set; } = @"Style: Note,MS PMincho,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string JPN_Comment { get; set; } = @"Style: Comment,MS PMincho,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string JPN_Color { get; set; } = @"{\1c&H20FFF0F0&\2c&H20843815&\3c&H20000000&\4c&HA0A6A6A8&}";
        public static string JPN_Font { get; set; } = @"{\fnSarasa Gothic J\fs24}";

        public static string JPN_DefaultAlt { get; set; } = @"Style: DefaultAlt,Sarasa Gothic J,24,&H20FFFFFF,&H20FFFFFF,&H40000040,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string JPN_DefaultAltM { get; set; } = @"Style: DefaultAltM,Sarasa Gothic J,24,&H20FFFFFF,&H20AD4C00,&H40FF7F7F,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string JPN_DefaultAltF { get; set; } = @"Style: DefaultAltF,Sarasa Gothic J,24,&H20FFFFFF,&H20AD4C00,&H407F7FFF,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string JPN_TitleAlt { get; set; } = @"Style: TitleAlt,Sarasa Gothic J,28,&H200055FF,&H2048560E,&H40EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,4,2,2,10,10,10,1";
        public static string JPN_NoteAlt { get; set; } = @"Style: NoteAlt,MS PMincho,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string JPN_CommentAlt { get; set; } = @"Style: CommentAlt,MS PMincho,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string JPN_ColorAlt { get; set; } = @"{\1c&H20FFF0F0&\2c&H20843815&\3c&H20000000&\4c&HA0A6A6A8&}";
        public static string JPN_FontAlt { get; set; } = @"{\fnSarasa Gothic J\fs24}";

        public static string KOR_Default { get; set; } = @"Style: Default,Sarasa Gothic K,24,&H20FFFFFF,&H20FFFFFF,&H40000000,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string KOR_DefaultM { get; set; } = @"Style: DefaultM,Sarasa Gothic K,24,&H20FFFFFF,&H20AD4C00,&H40FF7F7F,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string KOR_DefaultF { get; set; } = @"Style: DefaultF,Sarasa Gothic K,24,&H20FFFFFF,&H20AD4C00,&H407F7FFF,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string KOR_Title { get; set; } = @"Style: Title,Sarasa Gothic K,28,&H200055FF,&H2048560E,&H40EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,4,2,2,10,10,10,1";
        public static string KOR_Note { get; set; } = @"Style: Note,BatangChe,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string KOR_Comment { get; set; } = @"Style: Comment,BatangChe,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string KOR_Color { get; set; } = @"{\1c&H20FFF0F0&\2c&H20843815&\3c&H20000000&\4c&HA0A6A6A8&}";
        public static string KOR_Font { get; set; } = @"{\fnSarasa Gothic K\fs24}";

        public static string KOR_DefaultAlt { get; set; } = @"Style: DefaultAlt,Sarasa Gothic K,24,&H20FFFFFF,&H20FFFFFF,&H40000040,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string KOR_DefaultAltM { get; set; } = @"Style: DefaultAltM,Sarasa Gothic K,24,&H20FFFFFF,&H20AD4C00,&H40FF7F7F,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string KOR_DefaultAltF { get; set; } = @"Style: DefaultAltF,Sarasa Gothic K,24,&H20FFFFFF,&H20AD4C00,&H407F7FFF,&HA0BCBCBD,0,0,0,0,100,100,0,0,1,3,2,2,10,10,10,1";
        public static string KOR_TitleAlt { get; set; } = @"Style: TitleAlt,Sarasa Gothic K,28,&H200055FF,&H2048560E,&H40EAF196,&HA0969696,0,0,0,0,100,100,0,0,1,4,2,2,10,10,10,1";
        public static string KOR_NoteAlt { get; set; } = @"Style: NoteAlt,BatangChe,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string KOR_CommentAlt { get; set; } = @"Style: CommentAlt,BatangChe,20,&H20FFF907,&H20DC16C8,&H401E4454,&HA0969696,0,0,0,0,100,100,0,0,1,2,2,2,10,10,10,1";
        public static string KOR_ColorAlt { get; set; } = @"{\1c&H20FFF0F0&\2c&H20843815&\3c&H20000000&\4c&HA0A6A6A8&}";
        public static string KOR_FontAlt { get; set; } = @"{\fnSarasa Gothic K\fs24}";

        private static AssCustomStyle eng_style_default = null;
        public static AssCustomStyle ENG_Style_Default
        {
            get
            {
                if (eng_style_default == null) eng_style_default = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_Default, FormatOfStyle) };
                return (eng_style_default);
            }
        }
        private static AssCustomStyle eng_style_default_m = null;
        public static AssCustomStyle ENG_Style_DefaultMale
        {
            get
            {
                if (eng_style_default_m == null) eng_style_default_m = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_DefaultM, FormatOfStyle) };
                return (eng_style_default_m);
            }
        }
        private static AssCustomStyle eng_style_default_f = null;
        public static AssCustomStyle ENG_Style_DefaultFemale
        {
            get
            {
                if (eng_style_default_f == null) eng_style_default_f = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_DefaultF, FormatOfStyle) };
                return (eng_style_default_f);
            }
        }
        private static AssCustomStyle eng_style_title = null;
        public static AssCustomStyle ENG_Style_Title
        {
            get
            {
                if (eng_style_title == null) eng_style_title = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_Title, FormatOfStyle) };
                return (eng_style_title);
            }
        }
        private static AssCustomStyle eng_style_note = null;
        public static AssCustomStyle ENG_Style_Note
        {
            get
            {
                if (eng_style_note == null) eng_style_note = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_Note, FormatOfStyle) };
                return (eng_style_note);
            }
        }
        private static AssCustomStyle eng_style_comment = null;
        public static AssCustomStyle ENG_Style_Comment
        {
            get
            {
                if (eng_style_comment == null) eng_style_comment = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_Comment, FormatOfStyle) };
                return (eng_style_comment);
            }
        }

        private static AssCustomStyle chs_style_default = null;
        public static AssCustomStyle CHS_Style_Default
        {
            get
            {
                if (chs_style_default == null) chs_style_default = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_Default, FormatOfStyle) };
                return (chs_style_default);
            }
        }
        private static AssCustomStyle chs_style_default_m = null;
        public static AssCustomStyle CHS_Style_DefaultMale
        {
            get
            {
                if (chs_style_default_m == null) chs_style_default_m = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_DefaultM, FormatOfStyle) };
                return (chs_style_default_m);
            }
        }
        private static AssCustomStyle chs_style_default_f = null;
        public static AssCustomStyle CHS_Style_DefaultFemale
        {
            get
            {
                if (chs_style_default_f == null) chs_style_default_f = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_DefaultF, FormatOfStyle) };
                return (chs_style_default_f);
            }
        }
        private static AssCustomStyle chs_style_title = null;
        public static AssCustomStyle CHS_Style_Title
        {
            get
            {
                if (chs_style_title == null) chs_style_title = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_Title, FormatOfStyle) };
                return (chs_style_title);
            }
        }
        private static AssCustomStyle chs_style_note = null;
        public static AssCustomStyle CHS_Style_Note
        {
            get
            {
                if (chs_style_note == null) chs_style_note = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_Note, FormatOfStyle) };
                return (chs_style_note);
            }
        }
        private static AssCustomStyle chs_style_comment = null;
        public static AssCustomStyle CHS_Style_Comment
        {
            get
            {
                if (chs_style_comment == null) chs_style_comment = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_Comment, FormatOfStyle) };
                return (chs_style_comment);
            }
        }

        private static AssCustomStyle cht_style_default = null;
        public static AssCustomStyle CHT_Style_Default
        {
            get
            {
                if (cht_style_default == null) cht_style_default = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_Default, FormatOfStyle) };
                return (cht_style_default);
            }
        }
        private static AssCustomStyle cht_style_default_m = null;
        public static AssCustomStyle CHT_Style_DefaultMale
        {
            get
            {
                if (cht_style_default_m == null) cht_style_default_m = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_DefaultM, FormatOfStyle) };
                return (cht_style_default_m);
            }
        }
        private static AssCustomStyle cht_style_default_f = null;
        public static AssCustomStyle CHT_Style_DefaultFemale
        {
            get
            {
                if (cht_style_default_f == null) cht_style_default_f = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_DefaultF, FormatOfStyle) };
                return (cht_style_default_f);
            }
        }
        private static AssCustomStyle cht_style_title = null;
        public static AssCustomStyle CHT_Style_Title
        {
            get
            {
                if (cht_style_title == null) cht_style_title = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_Title, FormatOfStyle) };
                return (cht_style_title);
            }
        }
        private static AssCustomStyle cht_style_note = null;
        public static AssCustomStyle CHT_Style_Note
        {
            get
            {
                if (cht_style_note == null) cht_style_note = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_Note, FormatOfStyle) };
                return (cht_style_note);
            }
        }
        private static AssCustomStyle cht_style_comment = null;
        public static AssCustomStyle CHT_Style_Comment
        {
            get
            {
                if (cht_style_comment == null) cht_style_comment = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_Comment, FormatOfStyle) };
                return (cht_style_comment);
            }
        }

        private static AssCustomStyle jpn_style_default = null;
        public static AssCustomStyle JPN_Style_Default
        {
            get
            {
                if (jpn_style_default == null) jpn_style_default = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_Default, FormatOfStyle) };
                return (jpn_style_default);
            }
        }
        private static AssCustomStyle jpn_style_default_m = null;
        public static AssCustomStyle JPN_Style_DefaultMale
        {
            get
            {
                if (jpn_style_default_m == null) jpn_style_default_m = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_DefaultM, FormatOfStyle) };
                return (jpn_style_default_m);
            }
        }
        private static AssCustomStyle jpn_style_default_f = null;
        public static AssCustomStyle JPN_Style_DefaultFemale
        {
            get
            {
                if (jpn_style_default_f == null) jpn_style_default_f = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_DefaultF, FormatOfStyle) };
                return (jpn_style_default_f);
            }
        }
        private static AssCustomStyle jpn_style_note = null;
        public static AssCustomStyle JPN_Style_Note
        {
            get
            {
                if (jpn_style_note == null) jpn_style_note = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_Note, FormatOfStyle) };
                return (jpn_style_note);
            }
        }
        private static AssCustomStyle jpn_style_title = null;
        public static AssCustomStyle JPN_Style_Title
        {
            get
            {
                if (jpn_style_title == null) jpn_style_title = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_Title, FormatOfStyle) };
                return (jpn_style_title);
            }
        }
        private static AssCustomStyle jpn_style_comment = null;
        public static AssCustomStyle JPN_Style_Comment
        {
            get
            {
                if (jpn_style_comment == null) jpn_style_comment = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_Comment, FormatOfStyle) };
                return (jpn_style_comment);
            }
        }

        private static AssCustomStyle kor_style_default = null;
        public static AssCustomStyle KOR_Style_Default
        {
            get
            {
                if (kor_style_default == null) kor_style_default = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_Default, FormatOfStyle) };
                return (kor_style_default);
            }
        }
        private static AssCustomStyle kor_style_default_m = null;
        public static AssCustomStyle KOR_Style_DefaultMale
        {
            get
            {
                if (kor_style_default_m == null) kor_style_default_m = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_DefaultM, FormatOfStyle) };
                return (kor_style_default_m);
            }
        }
        private static AssCustomStyle kor_style_default_f = null;
        public static AssCustomStyle KOR_Style_DefaultFemale
        {
            get
            {
                if (kor_style_default_f == null) kor_style_default_f = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_DefaultF, FormatOfStyle) };
                return (kor_style_default_f);
            }
        }
        private static AssCustomStyle kor_style_title = null;
        public static AssCustomStyle KOR_Style_Title
        {
            get
            {
                if (kor_style_title == null) kor_style_title = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_Title, FormatOfStyle) };
                return (kor_style_title);
            }
        }
        private static AssCustomStyle kor_style_note = null;
        public static AssCustomStyle KOR_Style_Note
        {
            get
            {
                if (kor_style_note == null) kor_style_note = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_Note, FormatOfStyle) };
                return (kor_style_note);
            }
        }
        private static AssCustomStyle kor_style_comment = null;
        public static AssCustomStyle KOR_Style_Comment
        {
            get
            {
                if (kor_style_comment == null) kor_style_comment = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_Comment, FormatOfStyle) };
                return (kor_style_comment);
            }
        }

        private static AssCustomStyle eng_style_default_alt = null;
        public static AssCustomStyle ENG_Style_DefaultAlt
        {
            get
            {
                if (eng_style_default_alt == null) eng_style_default_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_DefaultAlt, FormatOfStyle) };
                return (eng_style_default_alt);
            }
        }
        private static AssCustomStyle eng_style_default_alt_m = null;
        public static AssCustomStyle ENG_Style_DefaultAltMale
        {
            get
            {
                if (eng_style_default_alt_m == null) eng_style_default_alt_m = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_DefaultAltM, FormatOfStyle) };
                return (eng_style_default_alt_m);
            }
        }
        private static AssCustomStyle eng_style_default_alt_f = null;
        public static AssCustomStyle ENG_Style_DefaultAltFemale
        {
            get
            {
                if (eng_style_default_alt_f == null) eng_style_default_alt_f = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_DefaultAltF, FormatOfStyle) };
                return (eng_style_default_alt_f);
            }
        }
        private static AssCustomStyle eng_style_title_alt = null;
        public static AssCustomStyle ENG_Style_TitleAlt
        {
            get
            {
                if (eng_style_title_alt == null) eng_style_title_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_TitleAlt, FormatOfStyle) };
                return (eng_style_title_alt);
            }
        }
        private static AssCustomStyle eng_style_note_alt = null;
        public static AssCustomStyle ENG_Style_NoteAlt
        {
            get
            {
                if (eng_style_note_alt == null) eng_style_note_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_NoteAlt, FormatOfStyle) };
                return (eng_style_note_alt);
            }
        }
        private static AssCustomStyle eng_style_comment_alt = null;
        public static AssCustomStyle ENG_Style_CommentAlt
        {
            get
            {
                if (eng_style_comment_alt == null) eng_style_comment_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(ENG_CommentAlt, FormatOfStyle) };
                return (eng_style_comment_alt);
            }
        }

        private static AssCustomStyle chs_style_default_alt = null;
        public static AssCustomStyle CHS_Style_DefaultAlt
        {
            get
            {
                if (chs_style_default_alt == null) chs_style_default_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_DefaultAlt, FormatOfStyle) };
                return (chs_style_default_alt);
            }
        }
        private static AssCustomStyle chs_style_default_alt_m = null;
        public static AssCustomStyle CHS_Style_DefaultAltMale
        {
            get
            {
                if (chs_style_default_alt_m == null) chs_style_default_alt_m = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_DefaultAltM, FormatOfStyle) };
                return (chs_style_default_alt_m);
            }
        }
        private static AssCustomStyle chs_style_default_alt_f = null;
        public static AssCustomStyle CHS_Style_DefaultAltFemale
        {
            get
            {
                if (chs_style_default_alt_f == null) chs_style_default_alt_f = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_DefaultAltF, FormatOfStyle) };
                return (chs_style_default_alt_f);
            }
        }
        private static AssCustomStyle chs_style_title_alt = null;
        public static AssCustomStyle CHS_Style_TitleAlt
        {
            get
            {
                if (chs_style_title_alt == null) chs_style_title_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_TitleAlt, FormatOfStyle) };
                return (chs_style_title_alt);
            }
        }
        private static AssCustomStyle chs_style_note_alt = null;
        public static AssCustomStyle CHS_Style_NoteAlt
        {
            get
            {
                if (chs_style_note_alt == null) chs_style_note_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_NoteAlt, FormatOfStyle) };
                return (chs_style_note_alt);
            }
        }
        private static AssCustomStyle chs_style_comment_alt = null;
        public static AssCustomStyle CHS_Style_CommentAlt
        {
            get
            {
                if (chs_style_comment_alt == null) chs_style_comment_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHS_CommentAlt, FormatOfStyle) };
                return (chs_style_comment_alt);
            }
        }

        private static AssCustomStyle cht_style_default_alt = null;
        public static AssCustomStyle CHT_Style_DefaultAlt
        {
            get
            {
                if (cht_style_default_alt == null) cht_style_default_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_DefaultAlt, FormatOfStyle) };
                return (cht_style_default_alt);
            }
        }
        private static AssCustomStyle cht_style_default_alt_m = null;
        public static AssCustomStyle CHT_Style_DefaultAltMale
        {
            get
            {
                if (cht_style_default_alt_m == null) cht_style_default_alt_m = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_DefaultAltM, FormatOfStyle) };
                return (cht_style_default_alt_m);
            }
        }
        private static AssCustomStyle cht_style_default_alt_f = null;
        public static AssCustomStyle CHT_Style_DefaultAltFemale
        {
            get
            {
                if (cht_style_default_alt_f == null) cht_style_default_alt_f = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_DefaultAltF, FormatOfStyle) };
                return (cht_style_default_alt_f);
            }
        }
        private static AssCustomStyle cht_style_title_alt = null;
        public static AssCustomStyle CHT_Style_TitleAlt
        {
            get
            {
                if (cht_style_title_alt == null) cht_style_title_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_TitleAlt, FormatOfStyle) };
                return (cht_style_title_alt);
            }
        }
        private static AssCustomStyle cht_style_note_alt = null;
        public static AssCustomStyle CHT_Style_NoteAlt
        {
            get
            {
                if (cht_style_note_alt == null) cht_style_note_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_NoteAlt, FormatOfStyle) };
                return (cht_style_note_alt);
            }
        }
        private static AssCustomStyle cht_style_comment_alt = null;
        public static AssCustomStyle CHT_Style_CommentAlt
        {
            get
            {
                if (cht_style_comment_alt == null) cht_style_comment_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(CHT_CommentAlt, FormatOfStyle) };
                return (cht_style_comment_alt);
            }
        }

        private static AssCustomStyle jpn_style_default_alt = null;
        public static AssCustomStyle JPN_Style_DefaultAlt
        {
            get
            {
                if (jpn_style_default_alt == null) jpn_style_default_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_DefaultAlt, FormatOfStyle) };
                return (jpn_style_default_alt);
            }
        }
        private static AssCustomStyle jpn_style_default_alt_m = null;
        public static AssCustomStyle JPN_Style_DefaultAltMale
        {
            get
            {
                if (jpn_style_default_alt_m == null) jpn_style_default_alt_m = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_DefaultAltM, FormatOfStyle) };
                return (jpn_style_default_alt_m);
            }
        }
        private static AssCustomStyle jpn_style_default_alt_f = null;
        public static AssCustomStyle JPN_Style_DefaultAltFemale
        {
            get
            {
                if (jpn_style_default_alt_f == null) jpn_style_default_alt_f = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_DefaultAltF, FormatOfStyle) };
                return (jpn_style_default_alt_f);
            }
        }
        private static AssCustomStyle jpn_style_note_alt = null;
        public static AssCustomStyle JPN_Style_NoteAlt
        {
            get
            {
                if (jpn_style_note_alt == null) jpn_style_note_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_NoteAlt, FormatOfStyle) };
                return (jpn_style_note_alt);
            }
        }
        private static AssCustomStyle jpn_style_title_alt = null;
        public static AssCustomStyle JPN_Style_TitleAlt
        {
            get
            {
                if (jpn_style_title_alt == null) jpn_style_title_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_TitleAlt, FormatOfStyle) };
                return (jpn_style_title_alt);
            }
        }
        private static AssCustomStyle jpn_style_comment_alt = null;
        public static AssCustomStyle JPN_Style_CommentAlt
        {
            get
            {
                if (jpn_style_comment_alt == null) jpn_style_comment_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(JPN_CommentAlt, FormatOfStyle) };
                return (jpn_style_comment_alt);
            }
        }

        private static AssCustomStyle kor_style_default_alt = null;
        public static AssCustomStyle KOR_Style_DefaultAlt
        {
            get
            {
                if (kor_style_default_alt == null) kor_style_default_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_DefaultAlt, FormatOfStyle) };
                return (kor_style_default_alt);
            }
        }
        private static AssCustomStyle kor_style_default_alt_m = null;
        public static AssCustomStyle KOR_Style_DefaultAltMale
        {
            get
            {
                if (kor_style_default_alt_m == null) kor_style_default_alt_m = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_DefaultAltM, FormatOfStyle) };
                return (kor_style_default_alt_m);
            }
        }
        private static AssCustomStyle kor_style_default_alt_f = null;
        public static AssCustomStyle KOR_Style_DefaultAltFemale
        {
            get
            {
                if (kor_style_default_alt_f == null) kor_style_default_alt_f = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_DefaultAltF, FormatOfStyle) };
                return (kor_style_default_alt_f);
            }
        }
        private static AssCustomStyle kor_style_title_alt = null;
        public static AssCustomStyle KOR_Style_TitleAlt
        {
            get
            {
                if (kor_style_title_alt == null) kor_style_title_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_TitleAlt, FormatOfStyle) };
                return (kor_style_title_alt);
            }
        }
        private static AssCustomStyle kor_style_note_alt = null;
        public static AssCustomStyle KOR_Style_NoteAlt
        {
            get
            {
                if (kor_style_note_alt == null) kor_style_note_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_NoteAlt, FormatOfStyle) };
                return (kor_style_note_alt);
            }
        }
        private static AssCustomStyle kor_style_comment_alt = null;
        public static AssCustomStyle KOR_Style_CommentAlt
        {
            get
            {
                if (kor_style_comment_alt == null) kor_style_comment_alt = new AssCustomStyle() { Style = ASS.STYLE.Parse(KOR_CommentAlt, FormatOfStyle) };
                return (kor_style_comment_alt);
            }
        }
    }

    public class ASS
    {
        private enum Sections { ScriptInfo = 0, Styles = 1, Events = 2, Fonts = 3, Graphics = 4, Unknown = -1 };

        [Flags]
        public enum SaveFlags { None = 0, Replace = 1, Merge = 2, SRT = 4, VTT = 8, LRC = 16, TXT = 32, ASS = 64, BOM = 256 };

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
            public string ScrollPosition;
            public string VideoZoomPercent;
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
            private static string[] FIELD_ALLOWED = new string[] {
                "Name", "Fontname", "Fontsize",
                "PrimaryColour", "SecondaryColour", "TertiaryColour", "OutlineColour", "BackColour",
                "Bold", "Italic", "Underline", "StrikeOut",
                "ScaleX", "ScaleY", "Spacing", "Angle", "BorderStyle", "Outline", "Shadow", "Alignment",
                "MarginL", "MarginR", "MarginV",
                "Encoding"
            };

            public string FormatRaw { get; set; } = string.Empty;
            public string[] Formats { get; set; } = FIELD_ALLOWED;

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

            public void ParseFormats(string text)
            {
                if (text.StartsWith("Format", StringComparison.CurrentCultureIgnoreCase))
                {
                    var fmt = FormatRaw.Split(':').Last();
                    if (!string.IsNullOrEmpty(fmt))
                    {
                        var fmts = fmt.Split(',').Select(f => f.Trim());
                        if (fmts.Count() > 1) Formats = fmts.ToArray();
                    }
                }
            }

            public void Parse(string style)
            {

            }

            public static STYLE Parse(string style_text, string format)
            {
                var result = new STYLE();

                var style_default = new STYLES();
                if(format.StartsWith("Format")) style_default.ParseFormat(format);

                if (style_text.StartsWith("Style:", StringComparison.CurrentCultureIgnoreCase))
                {
                    result.Raw = style_text.Split(':').Last().Trim();

                    var fields = style_default.Formats;
                    var values = result.Raw.Split(',');

                    for (int i = 0; i < fields.Length; i++)
                    {
                        result.Field(fields[i], values[i].Trim());
                    }
                }
                return (result);
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

        public class STYLES
        {
            public string FormatRaw { get; set; } = AssStyle.FormatOfStyle;
            private string[] formats = null;
            public string[] Formats
            {
                get
                {
                    if (formats == null) ParseFormat(FormatRaw);
                    return (formats);
                }
            }
            public List<STYLE> Items { get; set; } = new List<STYLE>();

            public void ParseFormat(string text)
            {
                if (text.StartsWith("Format", StringComparison.CurrentCultureIgnoreCase))
                {
                    var fmt = FormatRaw.Split(':').Last();
                    if (!string.IsNullOrEmpty(fmt))
                    {
                        var fmts = fmt.Split(',').Select(f => f.Trim());
                        if (fmts.Count() > 1) formats = fmts.ToArray();
                    }
                }
            }

            public void Clear()
            {
                if (Items is List<STYLE>) Items.Clear();
                FormatRaw = AssStyle.FormatOfStyle;
                formats = null;
            }
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

            public void Parse(string event_text)
            {

            }

            public static EVENT Parse(string event_text, string format)
            {
                var result = new EVENT();

                var event_default = new EVENTS();
                if (format.StartsWith("Format")) event_default.ParseFormat(format);

                if (event_text.StartsWith("Dialogue:", StringComparison.CurrentCultureIgnoreCase))
                {
                    result.Raw = event_text.Split(':').Last().Trim();

                    var fields = event_default.Formats;
                    var values = result.Raw.Split(',');

                    for (int i = 0; i < fields.Length; i++)
                    {
                        result.Field(fields[i], values[i].Trim());
                    }
                }
                return (result);
            }

            #region Event Properties
            private string id = string.Empty;
            public string ID
            {
                get { return id; }
                set { id = value; NotifyPropertyChanged("ID"); }
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
                set { Field("Type", value); NotifyPropertyChanged("Type"); }
            }
            public string Marked
            {
                get { return Field("Marked"); }
                set { Field("Marker", value); NotifyPropertyChanged("Marked"); }
            }
            public string Layer
            {
                get { return Field("Layer"); }
                set { Field("Layer", value); NotifyPropertyChanged("Layer"); }
            }
            public string Start
            {
                get { return Field("Start"); }
                set { Field("Start", value); NotifyPropertyChanged("Start"); }
            }
            public string End
            {
                get { return Field("End"); }
                set { Field("End", value); NotifyPropertyChanged("End"); }
            }
            public string Style
            {
                get { return Field("Style"); }
                set { Field("Style", value); NotifyPropertyChanged("Style"); }
            }
            public string Name
            {
                get { return Field("Name"); }
                set { Field("Name", value); NotifyPropertyChanged("Name"); }
            }
            public string MarginL
            {
                get { return Field("MarginL"); }
                set { Field("MarginL", value); NotifyPropertyChanged("MarginL"); }
            }
            public string MarginR
            {
                get { return Field("MarginR"); }
                set { Field("MarginR", value); NotifyPropertyChanged("MarginR"); }
            }
            public string MarginV
            {
                get { return Field("MarginV"); }
                set { Field("MarginV", value); NotifyPropertyChanged("MarginV"); }
            }
            public string Effect
            {
                get { return Field("Effect"); }
                set { Field("Effect", value); NotifyPropertyChanged("Effect"); }
            }
            public string Text
            {
                get { return Field("Text"); }
                set { Field("Text", value); NotifyPropertyChanged("Text"); }
            }
            public string Actor
            {
                get { return Field("Actor"); }
                set { Field("Actor", value); NotifyPropertyChanged("Actor"); }
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

        public class EVENTS
        {
            public string FormatRaw { get; set; } = AssStyle.FormatOfEvent;
            private string[] formats = null;
            public string[] Formats
            {
                get
                {
                    if (formats == null) ParseFormat(FormatRaw);
                    return (formats);
                }
            }
            public List<EVENT> Items { get; set; } = new List<EVENT>();

            public void ParseFormat(string text)
            {
                if (text.StartsWith("Format", StringComparison.CurrentCultureIgnoreCase))
                {
                    var fmt = FormatRaw.Split(':').Last();
                    if (!string.IsNullOrEmpty(fmt))
                    {
                        var fmts = fmt.Split(',').Select(f => f.Trim());
                        if (fmts.Count() > 1) formats = fmts.ToArray();
                    }
                }
            }

            public void Clear()
            {
                if (Items is List<EVENT>) Items.Clear();
                FormatRaw = AssStyle.FormatOfEvent;
                formats = null;
            }
        }

        public string OriginalFile { get; set; } = string.Empty;
        public bool SaveWithUTF8BOM { get; set; } = true;
        public SupportedLanguage YoutubeLanguage { get; set; } = SupportedLanguage.ENG;

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

        public static SupportedLanguage GuessLanguageFromTitle(string title)
        {
            var lang = SupportedLanguage.Unknown;
            if (!string.IsNullOrEmpty(title))
            {
                if (title.IndexOf(".ch", StringComparison.CurrentCultureIgnoreCase) > 0) lang = SupportedLanguage.CHS;
                else if(title.EndsWith(".ch", StringComparison.CurrentCultureIgnoreCase)) lang = SupportedLanguage.CHS;

                else if (title.IndexOf(".chs", StringComparison.CurrentCultureIgnoreCase) > 0) lang = SupportedLanguage.CHS;
                else if (title.EndsWith(".chs", StringComparison.CurrentCultureIgnoreCase)) lang = SupportedLanguage.CHS;

                else if (title.IndexOf(".cht", StringComparison.CurrentCultureIgnoreCase) > 0) lang = SupportedLanguage.CHT;
                else if (title.EndsWith(".cht", StringComparison.CurrentCultureIgnoreCase)) lang = SupportedLanguage.CHT;

                else if (title.IndexOf(".jp", StringComparison.CurrentCultureIgnoreCase) > 0) lang = SupportedLanguage.JPN;
                else if (title.EndsWith(".jp", StringComparison.CurrentCultureIgnoreCase)) lang = SupportedLanguage.JPN;
                else if (title.IndexOf(".jap", StringComparison.CurrentCultureIgnoreCase) > 0) lang = SupportedLanguage.JPN;
                else if (title.EndsWith(".jap", StringComparison.CurrentCultureIgnoreCase)) lang = SupportedLanguage.JPN;
                else if (title.IndexOf(".jpn", StringComparison.CurrentCultureIgnoreCase) > 0) lang = SupportedLanguage.JPN;
                else if (title.EndsWith(".jpn", StringComparison.CurrentCultureIgnoreCase)) lang = SupportedLanguage.JPN;

                else if (title.IndexOf(".ko", StringComparison.CurrentCultureIgnoreCase) > 0) lang = SupportedLanguage.KOR;
                else if (title.EndsWith(".ko", StringComparison.CurrentCultureIgnoreCase)) lang = SupportedLanguage.KOR;
                else if (title.IndexOf(".kor", StringComparison.CurrentCultureIgnoreCase) > 0) lang = SupportedLanguage.KOR;
                else if (title.EndsWith(".kor", StringComparison.CurrentCultureIgnoreCase)) lang = SupportedLanguage.KOR;

                else if (title.IndexOf(".en", StringComparison.CurrentCultureIgnoreCase) > 0) lang = SupportedLanguage.ENG;
                else if (title.EndsWith(".en", StringComparison.CurrentCultureIgnoreCase)) lang = SupportedLanguage.ENG;
                else if (title.IndexOf(".eng", StringComparison.CurrentCultureIgnoreCase) > 0) lang = SupportedLanguage.ENG;
                else if (title.EndsWith(".eng", StringComparison.CurrentCultureIgnoreCase)) lang = SupportedLanguage.ENG;
            }
            return (lang);
        }

        public static string[] GetDefaultHeader(string title, string comment = "", SupportedLanguage language = SupportedLanguage.Unknown)
        {
            List<string> lines = new List<string>();
            lines.Add($"[Script Info]");
            lines.Add($"Title: {title}");
            if (!string.IsNullOrEmpty(comment))
                lines.Add($"Comment: {comment}");
            lines.Add($"");
            lines.Add($"[V4+ Styles]");
            lines.Add(AssStyle.FormatOfStyle);
            if (language == SupportedLanguage.CHS)
            {
                lines.Add($"{AssStyle.CHS_Default}");
                lines.Add($"{AssStyle.CHS_DefaultM}");
                lines.Add($"{AssStyle.CHS_DefaultF}");
                lines.Add($"{AssStyle.CHS_Note}");
                lines.Add($"{AssStyle.CHS_Title}");
            }
            else if (language == SupportedLanguage.CHT)
            {
                lines.Add($"{AssStyle.CHT_Default}");
                lines.Add($"{AssStyle.CHT_DefaultM}");
                lines.Add($"{AssStyle.CHT_DefaultF}");
                lines.Add($"{AssStyle.CHT_Note}");
                lines.Add($"{AssStyle.CHT_Title}");
            }
            else if (language == SupportedLanguage.JPN)
            {
                lines.Add($"{AssStyle.JPN_Default}");
                lines.Add($"{AssStyle.JPN_DefaultM}");
                lines.Add($"{AssStyle.JPN_DefaultF}");
                lines.Add($"{AssStyle.JPN_Note}");
                lines.Add($"{AssStyle.JPN_Title}");
            }
            else if (language == SupportedLanguage.KOR)
            {
                lines.Add($"{AssStyle.KOR_Default}");
                lines.Add($"{AssStyle.KOR_DefaultM}");
                lines.Add($"{AssStyle.KOR_DefaultF}");
                lines.Add($"{AssStyle.KOR_Note}");
                lines.Add($"{AssStyle.KOR_Title}");
            }
            else
            {
                lines.Add($"{AssStyle.ENG_Default}");
                lines.Add($"{AssStyle.ENG_DefaultM}");
                lines.Add($"{AssStyle.ENG_DefaultF}");
                lines.Add($"{AssStyle.ENG_Note}");
                lines.Add($"{AssStyle.ENG_Title}");
            }
            lines.Add($"");
            lines.Add($"[Events]");
            lines.Add(AssStyle.FormatOfEvent);

            return (lines.ToArray());
        }

        public static string GetStyleDefault(SupportedLanguage lang = SupportedLanguage.Unknown)
        {
            var result = AssStyle.ENG_Default;
            if (lang == SupportedLanguage.CHS) result = AssStyle.CHS_Default;
            else if (lang == SupportedLanguage.CHT) result = AssStyle.CHT_Default;
            else if (lang == SupportedLanguage.JPN) result = AssStyle.JPN_Default;
            else if (lang == SupportedLanguage.KOR) result = AssStyle.KOR_Default;
            else if (lang == SupportedLanguage.ENG) result = AssStyle.ENG_Default;
            return (result);
        }

        public static string GetStyleColor(SupportedLanguage lang = SupportedLanguage.Unknown)
        {
            var result = AssStyle.ENG_Color;
            if (lang == SupportedLanguage.CHS) result = AssStyle.CHS_Color;
            else if (lang == SupportedLanguage.CHT) result = AssStyle.CHT_Color;
            else if (lang == SupportedLanguage.JPN) result = AssStyle.JPN_Color;
            else if (lang == SupportedLanguage.KOR) result = AssStyle.KOR_Color;
            else if (lang == SupportedLanguage.ENG) result = AssStyle.ENG_Color;
            return (result);
        }

        public static string GetStyleFont(SupportedLanguage lang = SupportedLanguage.Unknown)
        {
            var result = AssStyle.ENG_Font;
            if (lang == SupportedLanguage.CHS) result = AssStyle.CHS_Font;
            else if (lang == SupportedLanguage.CHT) result = AssStyle.CHT_Font;
            else if (lang == SupportedLanguage.JPN) result = AssStyle.JPN_Font;
            else if (lang == SupportedLanguage.KOR) result = AssStyle.KOR_Font;
            else if (lang == SupportedLanguage.ENG) result = AssStyle.ENG_Font;
            return (result);
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
            else if (v.StartsWith("Scroll Position:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.ScrollPosition = v.Split(':')[1].Trim();
            }
            else if (v.StartsWith("Video Zoom Percent:", StringComparison.CurrentCultureIgnoreCase))
            {
                ScriptInfo.VideoZoomPercent = v.Split(':')[1].Trim();
            }
        }

        private void ParseStyle(string v)
        {
            var style_default = new STYLES();

            if (v.StartsWith("Format:", StringComparison.CurrentCultureIgnoreCase))
            {
                style_default.ParseFormat(v);
                style_format = v.Split(':').Last().Trim();
                style_fields = style_format.Split(',');
                for (int i = 0; i < style_fields.Length; i++)
                {
                    style_fields[i] = style_fields[i].Trim();
                }
            }
            else if (v.StartsWith("Style:", StringComparison.CurrentCultureIgnoreCase))
            {
                var style = new STYLE();
                style.Raw = v.Split(':').Last().Trim();

                var fields = style_fields == null ? style_default.Formats : style_fields;
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
            var event_default = new EVENTS();
            if (v.StartsWith("Format:", StringComparison.CurrentCultureIgnoreCase))
            {
                event_default.ParseFormat(v);
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

                var fields = event_fields == null ? event_default.Formats : event_fields;
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
                OriginalFile = ass;
                var ext = Path.GetExtension(ass).ToLower();
                var fn = Path.GetFileNameWithoutExtension(ass);
                var lines = File.ReadAllLines(ass);
                if (ext.Equals(".srt") || ext.Equals(".vtt"))
                {
                    await LoadFromSrt(lines, fn);
                }
                else if (ext.Equals(".lrc"))
                {
                    LoadFromLRC(lines, fn);
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

        public async Task LoadFromSrt(string[] contents, string title = "Untitled")
        {
            var srt = new SrtCollection();
            await srt.Load(contents);
            var lines = srt.ToAss(title, YoutubeLanguage);
            Load(lines.ToArray());
        }

        public void LoadFromYouTube(string[] contents, string title = "Untitled")
        {
            var last_time = new DateTime(0);
            var last_text = string.Empty;
            var curr_time = new DateTime(0);
            var curr_text = string.Empty;

            var re_src = new Regex(@"^\d+:\d+$", RegexOptions.IgnoreCase);

            if (contents is string[])
            {
                List<string> lines = new List<string>();
                lines.AddRange(GetDefaultHeader(title, language:YoutubeLanguage));

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

                        if (last_time.Equals(curr_time) && string.IsNullOrEmpty(last_text)) continue;
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

        public void LoadFromLRC(string[] contents, string title = "Untitled")
        {
            var pat_album = @"\[al(bum)?\s*:\s*(.*?)\]";
            var pat_artist = @"\[ar(tist)?\s*:\s*(.*?)\]";
            var pat_title = @"\[ti(tle)?\s*:\s*(.*?)\]";
            var pat_alias = @"\[alias\s*:\s*(.*?)\]";
            var pat_by = @"\[by\s*:\s*(.*?)\]";
            var pat_lyric = @"((\[((\d{1,2}:)?\d{1,2}:\d{1,2}(\.\d{1,3})?)\])+)\s*(.*?)$";

            var ts_sep = new string[] { "[" };
            var ts_trim = new char[] { '[', ']' };

            var lrc = new LRCSubtitle() { Title = title };
            #region pharsing lyric contents
            foreach (var line in contents)
            {
                try
                {
                    if (Regex.IsMatch(line, pat_album, RegexOptions.IgnoreCase))
                        lrc.Album = Regex.Replace(line, pat_album, @"$2", RegexOptions.IgnoreCase);
                    else if (Regex.IsMatch(line, pat_artist, RegexOptions.IgnoreCase))
                        lrc.Artist = Regex.Replace(line, pat_artist, @"$2", RegexOptions.IgnoreCase);
                    else if (Regex.IsMatch(line, pat_title, RegexOptions.IgnoreCase))
                        lrc.Title = Regex.Replace(line, pat_title, @"$2", RegexOptions.IgnoreCase);
                    else if (Regex.IsMatch(line, pat_alias, RegexOptions.IgnoreCase))
                        lrc.Alias = Regex.Replace(line, pat_alias, @"$1", RegexOptions.IgnoreCase);
                    else if (Regex.IsMatch(line, pat_by, RegexOptions.IgnoreCase))
                        lrc.Author = Regex.Replace(line, pat_by, @"$1", RegexOptions.IgnoreCase);
                    else if (Regex.IsMatch(line, pat_lyric, RegexOptions.IgnoreCase))
                    {
                        Regex.Replace(line, pat_lyric, m =>
                        {
                            var ts = m.Groups[1].Value.Split(ts_sep, StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim().Trim(ts_trim).Trim());
                            foreach (var t in ts)
                            {
                                if (string.IsNullOrEmpty(t)) continue;
                                //var tv = string.IsNullOrEmpty(m.Groups[3].Value) ? $"00:{m.Groups[2].Value}" : m.Groups[2].Value;
                                var tc = t.ToArray().Where(c => c.Equals(':')).Count();
                                var tv = tc == 1 ? $"00:{t}" : t;
                                lrc.Events.Add(new LRCSubtitleItem()
                                {
                                    TimeFrom = TimeSpan.Parse(tv),
                                    Text = m.Groups[6].Value.Trim()
                                });
                            }
                            return ("");
                        }, RegexOptions.IgnoreCase);
                    }
                }
                catch { }
            }
            #endregion
            #region sort lyric by time and append lyric end time
            lrc.Events = lrc.Events.OrderBy(e => e.TimeFrom.Ticks).ToList();
            var last_from = TimeSpan.FromSeconds(20);
            for (var i = lrc.Events.Count - 1; i >= 0; i--)
            {
                if (i == lrc.Events.Count - 1)
                    lrc.Events[i].TimeTo = lrc.Events[i].TimeFrom + TimeSpan.FromSeconds(20);
                else
                    lrc.Events[i].TimeTo = last_from;
                last_from = lrc.Events[i].TimeFrom;
            }
            #endregion
            #region convert lrc to ass
            var lines = new List<string>();
            var lang = GuessLanguageFromTitle(title);
            var comments = $"Album: {lrc.Album}, Artist: {lrc.Artist}, Alias: {lrc.Alias}, Made By: {lrc.Author}, Comments:{lrc.Comment}";
            lines.AddRange(GetDefaultHeader(lrc.Title, comment: comments, language: lang));
            foreach (var e in lrc.Events)
            {
                try
                {
                    if (string.IsNullOrEmpty(e.Text)) continue;
                    lines.Add($"Dialogue: 0,{e.TimeFrom.ToString(@"hh\:mm\:ss\.ff")},{e.TimeTo.ToString(@"hh\:mm\:ss\.ff")},Default,NTP,0000,0000,0000,,{e.Text}");
                }
                catch (Exception ex) { System.Windows.MessageBox.Show($"{ex.Message}{Environment.NewLine}[{e.TimeFrom}]{e.Text}"); }
            }
            Load(lines.ToArray());
            #endregion            
        }

        public void Save(string ass_file, SaveFlags flags = SaveFlags.Merge | SaveFlags.BOM)
        {
            if (flags.HasFlag(SaveFlags.SRT) || flags.HasFlag(SaveFlags.VTT))
                SaveAsSrtVtt(ass_file, flags);
            else if (flags.HasFlag(SaveFlags.LRC))
                SaveAsLRC(ass_file, flags);
            else if (flags.HasFlag(SaveFlags.TXT))
                SaveAsTxt(ass_file, flags);
            else
            {
                var lang_src = GuessLanguageFromTitle(OriginalFile);
                var lang_dst = GuessLanguageFromTitle(ass_file);
                if (lang_src == SupportedLanguage.Unknown) lang_src = SupportedLanguage.ENG;
                //if (lang_dst == SupportedLanguage.Unknown) lang_dst = lang_src == SupportedLanguage.ENG ? SupportedLanguage.CHS : SupportedLanguage.ENG;
                if (lang_dst == SupportedLanguage.Unknown) lang_dst = SupportedLanguage.CHS;
                if (flags.HasFlag(SaveFlags.Merge))
                    ChangeStyle(lang_src);
                else if (lang_dst != SupportedLanguage.Unknown && lang_dst != SupportedLanguage.Any)
                    ChangeStyle(lang_dst);

                var style_src = GetStyleDefault(lang_src);
                var style_dst = GetStyleDefault(lang_dst);
                var color_src = GetStyleColor(lang_src);
                var color_dst = GetStyleColor(lang_dst);
                var font_src = GetStyleFont(lang_src);
                var font_dst = GetStyleFont(lang_dst);

                StringBuilder sb = new StringBuilder();

                #region Save Script Info Section
                var offset = 0;
                foreach (var l in ScriptInfo.Raw) { if (l.StartsWith("[") || l.StartsWith(";")) offset++; else break; }
                if (string.IsNullOrEmpty(ScriptInfo.ScriptType)) { ScriptInfo.ScriptType = "v4.00+"; ScriptInfo.Raw.Insert(offset + 1, "ScriptType: v4.00+"); }
                if (string.IsNullOrEmpty(ScriptInfo.ScrollPosition)) { ScriptInfo.ScrollPosition = "0"; ScriptInfo.Raw.Insert(offset + 2, "Scroll Position: 0"); }
                if (string.IsNullOrEmpty(ScriptInfo.VideoZoomPercent)) { ScriptInfo.VideoZoomPercent = "1"; ScriptInfo.Raw.Insert(offset + 3, "Video Zoom Percent: 1"); }

                ScriptInfo.Raw = ScriptInfo.Raw.Distinct().Where(l => !string.IsNullOrEmpty(l.Trim())).ToList();
                for (int i = 0; i < ScriptInfo.Raw.Count; i++)
                {
                    sb.AppendLine(ScriptInfo.Raw[i]);
                }
                sb.AppendLine();
                #endregion

                #region Save Styles Section
                StylesRaw = StylesRaw.Distinct().Where(l => !string.IsNullOrEmpty(l.Trim())).ToList();
                if (StylesRaw.Count > 0)
                {
                    for (int i = 0; i < StylesRaw.Count; i++)
                    {
                        sb.AppendLine(StylesRaw[i]);
                    }
                    sb.AppendLine();
                }
                #endregion

                #region Save Fonts Section
                FontsRaw = FontsRaw.Distinct().Where(l => !string.IsNullOrEmpty(l.Trim())).ToList();
                if (FontsRaw.Count > 0)
                {
                    for (int i = 0; i < FontsRaw.Count; i++)
                    {
                        sb.AppendLine(FontsRaw[i]);
                    }
                    sb.AppendLine();
                }
                #endregion

                #region Save Graphics Section
                GraphicsRaw = GraphicsRaw.Distinct().Where(l => !string.IsNullOrEmpty(l.Trim())).ToList();
                if (GraphicsRaw.Count > 0)
                {
                    for (int i = 0; i < GraphicsRaw.Count; i++)
                    {
                        sb.AppendLine(GraphicsRaw[i]);
                    }
                    sb.AppendLine();
                }
                #endregion

                #region Save Events Section
                sb.AppendLine(EventsRaw[0]);
                sb.AppendLine(EventsRaw[1]);
                for (int i = 2; i < events.Count + 2; i++)
                {
                    var current = i-2;
                    var evt = events[current];
                    var evo = new List<string>();
                    foreach (var k in event_fields)
                    {
                        if (string.Equals(k, "ID", StringComparison.CurrentCultureIgnoreCase)) continue;
                        if (string.Equals(k, "Text", StringComparison.CurrentCultureIgnoreCase)) continue;
                        evo.Add(evt.Field(k));
                    }
                    string line = string.Empty;
                    if (flags.HasFlag(SaveFlags.Merge))
                    {
                        if (string.IsNullOrEmpty(events[current].Translated))
                            line = $"{string.Join(",", evo)},{color_src}{font_src}{events[current].Text}";
                        else
                            line = $"{string.Join(",", evo)},{color_src}{font_src}{events[current].Text}\\N{color_dst}{font_dst}{events[current].Translated}";
                    }
                    else if (flags.HasFlag(SaveFlags.Replace))
                    {
                        if (string.IsNullOrEmpty(events[current].Translated))
                            line = $"{string.Join(",", evo)},{events[current].Text}";
                        else
                            line = $"{string.Join(",", evo)},{events[current].Translated}";
                    }
                    else
                    {
                        line = $"{string.Join(",", evo)},{events[current].Text}";
                    }
                    int idx = line.IndexOf(",");
                    if (idx > 0)
                        line = $"{line.Substring(0, idx)}: {line.Substring(idx + 1)}";
                    if (!string.IsNullOrEmpty(line))
                        sb.AppendLine(line);
                }
                sb.AppendLine();
                #endregion

                File.WriteAllText(ass_file, sb.ToString(), new UTF8Encoding(flags.HasFlag(SaveFlags.BOM)));
            }
        }

        public void SaveAsSrtVtt(string ass_file, SaveFlags flags = SaveFlags.Merge | SaveFlags.BOM)
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
                if (flags.HasFlag(SaveFlags.VTT))
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
                else if (flags.HasFlag(SaveFlags.Merge))
                {
                    if (string.IsNullOrEmpty(events[i].Translated))
                        line = events[i].Text;
                    else
                        line = $"{events[i].Text}\\N{events[i].Translated}";
                }
                else
                {
                    line = events[i].Text;
                }
                if (!string.IsNullOrEmpty(line))
                {
                    var lines = line.Split(new string[] { "\\N", "\r\n", "\n\r", "\n", "\r" }, StringSplitOptions.None);
                    foreach (var l in lines)
                    {
                        sb.AppendLine($"{Regex.Replace(l, @"\{ *\\.*?\}", "", RegexOptions.IgnoreCase)}");
                    }
                }
                sb.AppendLine();
            }

            File.WriteAllText(Path.ChangeExtension(ass_file, ext), sb.ToString(), new UTF8Encoding(flags.HasFlag(SaveFlags.BOM)));
        }

        public void SaveAsLRC(string ass_file, SaveFlags flags = SaveFlags.BOM)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"[ti:{ScriptInfo.Title}]");
            sb.AppendLine($"[ar:]");
            sb.AppendLine($"[al:]");
            sb.AppendLine($"[by:]");
            sb.AppendLine();

            foreach (var e in events)
            {
                sb.Append($"[{e.Start}]");
                if (flags.HasFlag(SaveFlags.Merge))
                {
                    if (string.IsNullOrEmpty(e.Translated))
                        sb.AppendLine($"{e.Text}");
                    else
                        sb.AppendLine($"{e.Text}{Environment.NewLine}{e.Translated}");
                }
                else if (flags.HasFlag(SaveFlags.Replace))
                {
                    if (string.IsNullOrEmpty(e.Translated))
                        sb.AppendLine($"{e.Text}");
                    else
                        sb.AppendLine($"{e.Translated}");
                }
                else
                {
                    sb.AppendLine(e.Text);
                }
            }

            File.WriteAllText(ass_file, sb.ToString(), new UTF8Encoding(flags.HasFlag(SaveFlags.BOM)));
        }

        public void SaveAsTxt(string ass_file, SaveFlags flags = SaveFlags.BOM)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var e in events)
            {
                sb.AppendLine(e.Start);
                if (flags.HasFlag(SaveFlags.Merge))
                {
                    if (string.IsNullOrEmpty(e.Translated))
                        sb.AppendLine($"{e.Text}");
                    else
                        sb.AppendLine($"{e.Text}{Environment.NewLine}{e.Translated}");
                }
                else if (flags.HasFlag(SaveFlags.Replace))
                {
                    if (string.IsNullOrEmpty(e.Translated))
                        sb.AppendLine($"{e.Text}");
                    else
                        sb.AppendLine($"{e.Translated}");
                }
                else
                {
                    sb.AppendLine(e.Text);
                }
                sb.AppendLine();
            }

            File.WriteAllText(ass_file, sb.ToString(), new UTF8Encoding(flags.HasFlag(SaveFlags.BOM)));
        }

        public void ChangeStyle(SupportedLanguage lang =  SupportedLanguage.Unknown)
        {
            if (StylesRaw.Count <= 2) return;
            try
            {
                if (lang == SupportedLanguage.Unknown) lang = YoutubeLanguage;
                for (int i = StylesRaw.Count - 1; i > 1; i--)
                {
                    if (Regex.IsMatch(StylesRaw[i], @"Style: ?(Default(M|F)?|Note|Title) ?,", RegexOptions.IgnoreCase))
                        StylesRaw.RemoveAt(i);
                }

                var style_default = AssStyle.ENG_Default;
                var style_default_m = AssStyle.ENG_DefaultM;
                var style_default_f = AssStyle.ENG_DefaultF;
                var style_note = AssStyle.ENG_Note;
                var style_title = AssStyle.ENG_Title;
                var style_comment = AssStyle.ENG_Comment;

                var style_default_alt = AssStyle.ENG_DefaultAlt;
                var style_default_alt_m = AssStyle.ENG_DefaultAltM;
                var style_default_alt_f = AssStyle.ENG_DefaultAltF;
                var style_note_alt = AssStyle.ENG_NoteAlt;
                var style_title_alt = AssStyle.ENG_TitleAlt;
                var style_comment_alt = AssStyle.ENG_CommentAlt;

                if (lang == SupportedLanguage.CHS)
                {
                    style_default = AssStyle.CHS_Default;
                    style_default_m = AssStyle.CHS_DefaultM;
                    style_default_f = AssStyle.CHS_DefaultF;
                    style_note = AssStyle.CHS_Note;
                    style_title = AssStyle.CHS_Title;
                    style_comment = AssStyle.CHS_Comment;

                    style_default_alt = AssStyle.CHS_DefaultAlt;
                    style_default_alt_m = AssStyle.CHS_DefaultAltM;
                    style_default_alt_f = AssStyle.CHS_DefaultAltF;
                    style_note_alt = AssStyle.CHS_NoteAlt;
                    style_title_alt = AssStyle.CHS_TitleAlt;
                    style_comment_alt = AssStyle.CHS_CommentAlt;
                }
                else if (lang == SupportedLanguage.CHT)
                {
                    style_default = AssStyle.CHT_Default;
                    style_default_m = AssStyle.CHT_DefaultM;
                    style_default_f = AssStyle.CHT_DefaultF;
                    style_note = AssStyle.CHT_Note;
                    style_title = AssStyle.CHT_Title;
                    style_comment = AssStyle.CHT_Comment;

                    style_default_alt = AssStyle.CHT_DefaultAlt;
                    style_default_alt_m = AssStyle.CHT_DefaultAltM;
                    style_default_alt_f = AssStyle.CHT_DefaultAltF;
                    style_note_alt = AssStyle.CHT_NoteAlt;
                    style_title_alt = AssStyle.CHT_TitleAlt;
                    style_comment_alt = AssStyle.CHT_CommentAlt;
                }
                else if (lang == SupportedLanguage.JPN)
                {
                    style_default = AssStyle.JPN_Default;
                    style_default_m = AssStyle.JPN_DefaultM;
                    style_default_f = AssStyle.JPN_DefaultF;
                    style_note = AssStyle.JPN_Note;
                    style_title = AssStyle.JPN_Title;
                    style_comment = AssStyle.JPN_Comment;

                    style_default_alt = AssStyle.JPN_DefaultAlt;
                    style_default_alt_m = AssStyle.JPN_DefaultAltM;
                    style_default_alt_f = AssStyle.JPN_DefaultAltF;
                    style_note_alt = AssStyle.JPN_NoteAlt;
                    style_title_alt = AssStyle.JPN_TitleAlt;
                    style_comment_alt = AssStyle.JPN_CommentAlt;
                }
                else if (lang == SupportedLanguage.KOR)
                {
                    style_default = AssStyle.KOR_Default;
                    style_default_m = AssStyle.KOR_DefaultM;
                    style_default_f = AssStyle.KOR_DefaultF;
                    style_note = AssStyle.KOR_Note;
                    style_title = AssStyle.KOR_Title;
                    style_comment = AssStyle.KOR_Comment;

                    style_default_alt = AssStyle.KOR_DefaultAlt;
                    style_default_alt_m = AssStyle.KOR_DefaultAltM;
                    style_default_alt_f = AssStyle.KOR_DefaultAltF;
                    style_note_alt = AssStyle.KOR_NoteAlt;
                    style_title_alt = AssStyle.KOR_TitleAlt;
                    style_comment_alt = AssStyle.KOR_CommentAlt;
                }

                var styles = new List<string>()
                {
                    style_default, style_default_m, style_default_f, style_note, style_title, style_comment,
                    style_default_alt, style_default_alt_m, style_default_alt_f, style_note_alt, style_title_alt, style_comment_alt,
                };
                StylesRaw.InsertRange(2, styles);
                StylesRaw = StylesRaw.Distinct().Where(l => !string.IsNullOrEmpty(l.Trim())).ToList();

                if (!string.IsNullOrEmpty(StylesRaw.Last())) StylesRaw.Add("");
            }
            catch (Exception) { }
        }

        public IEnumerable<string> GetCustomStyles()
        {
            try
            {
                return (Styles.Where(s => !AssStyle.Contains(s.Name)).Select(s => s.Name));
            }
            catch { return (new List<string>()); }
        }
    }
}