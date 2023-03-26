using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using CommonLibTools;
using CommonLibTools.Libs;
using CommonLibTools.Libs.Extensions;
using MongoDB.Bson.Serialization.Attributes;

namespace WiktionaireParser.Models
{
    public class WikiPage
    {
        [BsonId]
        public string Title { get; set; }
        public string TitleInv { get; set; }
        public string AnagramKey { get; set; }
        public int AnagramCount { get; set; }
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
        public bool IsAdverbe;
        public bool IsPronom;

        // frequency 

        public long FrequencyCount { get; set; }
        public long FrequencyTotalCount { get; set; }
        public float Frequency { get; set; }
        public long MostFrequentWordCount { get; set; }

        public WikiPage(string title, string text, SectionBuilder sectionBuilder)
        {
            Title = title;
            TitleInv = title.ToLowerInvariant().RemoveDiacritics();
            AnagramKey = TitleInv.SortString();
            Text = text;
            Len = Title.Length;

            ExtractData(sectionBuilder);
        }

        public void AddDataFrom(WikiPage wikiPage)
        {
            Text += $"\r\n######################\r\n {wikiPage.Text}";
            LangText += $"\r\n######################\r\n {wikiPage.LangText}";
            if (!IsVerb) IsVerb = wikiPage.IsVerb;
            if (!IsVerbFlexion) IsVerbFlexion = wikiPage.IsVerbFlexion;
            if (!IsNomCommun) IsNomCommun = wikiPage.IsNomCommun;
            if (!IsAdjective) IsAdjective = wikiPage.IsAdjective;
            if (!IsAdverbe) IsAdverbe = wikiPage.IsAdverbe;
            if (!IsPronom) IsPronom = wikiPage.IsPronom;
        }

        public bool IsOnlyVerbFlexion()
        {
            return IsVerbFlexion == true
                   && IsNomCommun == false
                   && IsVerb == false
                   && IsAdjective == false
                   && IsPronom == false
                   && IsAdverbe == false;
        }

        void ExtractData(SectionBuilder sectionBuilder)
        {
            var lines = Text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            var sectionFound = false;
            var langLines = new List<string>();
            var builder = new StringBuilder();
            foreach (var lin in lines)
            {
                var line = lin.Trim();
                if (RegexLibFr.StartWithLangSectionRegex.IsMatch(line))
                {
                    sectionFound = true;
                    langLines.Add(line);
                    builder.AppendLine(line);

                    sectionBuilder.AddSection(lin);
                    continue;
                }

                if (sectionFound)
                {
                    if (RegexLibFr.SectionRegex.IsMatch(lin))
                    {
                        sectionBuilder.AddSection(lin);
                    }

                    // if (RegexLib.OtherLangSectionRegex.IsMatch(line))
                    if (RegexLibFr.StartWithOtherLangSectionRegex.IsMatch(line))
                    {
                        sectionFound = false;
                        // break;
                    }
                    else
                    {
                        langLines.Add(line);
                        builder.AppendLine(line);
                    }
                }
            }

            LangText = builder.ToString();
            var infosBuilder = new StringBuilder();
            for (var index = 0; index < langLines.Count; index++)
            {
                var line = langLines[index];//.ToLower();
                var lowerLine = line.ToLowerInvariant().Trim().RemoveWhitespace();

                //verbs
                if (RegexLibFr.VerbRegex.IsMatch(lowerLine) && line.Contains("flexion") == false) IsVerb = true;
                if (RegexLibFr.VerbRegex2.IsMatch(lowerLine) && line.Contains("flexion") == false) IsVerb = true;
                if (RegexLibFr.VerbFlexionRegex.IsMatch(lowerLine))
                {
                    //Debug.WriteLine($"### verb flexion {Title}");
                    sectionBuilder.AddVerbFlexion($"{Title} __ {TitleInv}");
                    IsVerbFlexion = true;
                }

                //noms
                if (RegexLibFr.NomCommunRegex.IsMatch(lowerLine)) IsNomCommun = true;
                if (RegexLibFr.NomCommunRegex2.IsMatch(lowerLine)) IsNomCommun = true;
                if (RegexLibFr.NomCommunRegex3.IsMatch(lowerLine)) IsNomCommun = true;

                //adjectif
                if (RegexLibFr.AdjectifRegex.IsMatch(lowerLine)) IsAdjective = true;
                if (RegexLibFr.AdjectifRegex2.IsMatch(lowerLine)) IsAdjective = true;
                if (RegexLibFr.AdjectifRegex3.IsMatch(lowerLine)) IsAdjective = true;

                //adverbe
                if (RegexLibFr.AdverbeRegex.IsMatch(lowerLine)) IsAdverbe = true;
                if (RegexLibFr.AdverbeRegex2.IsMatch(lowerLine)) IsAdverbe = true;


                //pronom
                if (RegexLibFr.PronomRegex.IsMatch(lowerLine)) IsPronom = true;
                if (RegexLibFr.PronomRegex2.IsMatch(lowerLine)) IsPronom = true;


                //if (line.StartsWith("=== {{S|verbe|fr}} ===")) IsVerb = true;
                //if (line.StartsWith("=== {{S|verbe|fr|flexion}} ===")) IsVerbFlexion = true;
                //if (line.StartsWith("=== {{S|nom|fr")) IsNomCommun = true;

                //antonyms 
                var endSection = false;
                if (lowerLine.StartsWith("===={{S|antonymes}}===="))
                {
                    do
                    {
                        index++;
                        line = langLines[index];

                        if (line.StartsWith("*") || line.StartsWith("#"))
                        {
                            var matches = RegexLibFr.regexLink.Matches(line);
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
                if (lowerLine.StartsWith("===={{S|synonymes}}===="))
                {
                    do
                    {
                        index++;
                        line = langLines[index];

                        if (line.StartsWith("*") || line.StartsWith("#"))
                        {
                            var matches = RegexLibFr.regexLink.Matches(line);
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
            return $"{TitleInv} - {Title}";
        }
    }
}
