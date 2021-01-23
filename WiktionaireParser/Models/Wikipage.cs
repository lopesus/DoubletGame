using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization.Attributes;

namespace WiktionaireParser.Models
{
    public static class RegexLib
    {
        public static Regex regexLink = new Regex(@"\[\[(.*)\]\]");
        public static Regex StartWithLangSectionRegex = new Regex(@"^==\s*{{langue\|fr}}\s*==");
        public static Regex ContainsLangSectionRegex = new Regex(@"==\s*{{langue\|fr}}\s*==");
        public static Regex OtherLangSectionRegex = new Regex(@"^==\s*{{langue");
    }
    public class WikiPage
    {
        [BsonId]
        public string Title { get; set; }
        public string Text { get; set; }
        public string LangText { get; set; }
        public string Antonymes { get; set; }
        public bool HasAntonymes { get; set; }
        public string Sinonymes { get; set; }
        public bool HasSinonymes { get; set; }

        public int Len;
        public bool IsVerb;
        public bool IsVerbFlexion;
        public bool IsNomCommun;
        public bool IsAdjective;


        public WikiPage(string title, string text)
        {
            Title = title;
            Text = text;
            Len = Title.Length;

            ExtractData();
        }

        public bool IsOnlyVerbFlexion()
        {
            return IsVerbFlexion == true && IsNomCommun == false && IsVerb == false;
        }
        void ExtractData()
        {
            var lines = Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var sectionFound = false;
            var langLines = new List<string>();
            var builder = new StringBuilder();
            foreach (var lin in lines)
            {
                var line = lin.Trim();
                if (RegexLib.StartWithLangSectionRegex.IsMatch(line))
                {
                    sectionFound = true;
                    langLines.Add(line);
                    builder.AppendLine(line);
                    continue;
                }
                //if (line.StartsWith("== {{langue|fr}} =="))
                //{
                //    sectionFound = true;
                //    langLines.Add(line);
                //    builder.AppendLine(line);
                //    continue;
                //}
                if (sectionFound)
                {
                    if (RegexLib.OtherLangSectionRegex.IsMatch(line))
                    {
                        //new lang section 
                        break;
                    }
                    else
                    {
                        langLines.Add(line);
                        builder.AppendLine(line);
                    }
                    //if (line.StartsWith("== {{langue"))
                    //{
                    //    //new lang section 
                    //    break;
                    //}
                    //else
                    //{
                    //    langLines.Add(line);
                    //    builder.AppendLine(line);
                    //}
                }
            }

            LangText = builder.ToString();
            var infosBuilder = new StringBuilder();
            for (var index = 0; index < langLines.Count; index++)
            {
                var line = langLines[index];
                if (line.StartsWith("=== {{S|verbe|fr}} ===")) IsVerb = true;
                if (line.StartsWith("=== {{S|verbe|fr|flexion}} ===")) IsVerbFlexion = true;
                if (line.StartsWith("=== {{S|nom|fr")) IsNomCommun = true;

                //antonyms 
                var endSection = false;
                if (line.StartsWith("==== {{S|antonymes}} ===="))
                {
                    do
                    {
                        index++;
                        line = langLines[index];

                        if (line.StartsWith("*") || line.StartsWith("#"))
                        {
                            var matches = RegexLib.regexLink.Matches(line);
                            foreach (Match match in matches)
                            {
                                infosBuilder.AppendLine(match.Groups[1].Value);
                            }
                        }

                        endSection = line.StartsWith("=") || index >= langLines.Count - 1;
                    } while (endSection == false);

                    Antonymes = infosBuilder.ToString();
                    HasAntonymes = true;
                }


                //synonymes 
                endSection = false;
                infosBuilder.Clear();
                if (line.StartsWith("==== {{S|synonymes}} ===="))
                {
                    do
                    {
                        index++;
                        line = langLines[index];

                        if (line.StartsWith("*") || line.StartsWith("#"))
                        {
                            var matches = RegexLib.regexLink.Matches(line);
                            foreach (Match match in matches)
                            {
                                infosBuilder.AppendLine(match.Groups[1].Value);
                            }
                        }

                        endSection = line.StartsWith("=") || index >= langLines.Count - 1;
                    } while (endSection == false);

                    Sinonymes = infosBuilder.ToString();
                    HasSinonymes = true;
                }


                //if (line.StartsWith("ddd")) ddd = true;
                //if (line.StartsWith("ddd")) ddd = true;
                //if (line.StartsWith("ddd")) ddd = true;
                //if (line.StartsWith("ddd")) ddd = true;
            }
        }

        public override string ToString()
        {
            return $"{Title}";
        }
    }
}
