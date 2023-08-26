using System.Collections.Concurrent;
using System.Collections.Generic;
using CommonLibTools.Libs.Extensions;

namespace WiktionaireParser.Models
{
    public class SectionBuilder
    {
        public ConcurrentBag<string> Sections { get; set; } = new ConcurrentBag<string>();
        public ConcurrentBag<string> SectionsWithNoSpace { get; set; } = new ConcurrentBag<string>();
        public ConcurrentBag<string> VerbFlexion { get; set; } = new ConcurrentBag<string>();
        public void AddSection(string sectionName)
        {
            Sections.Add(sectionName.Trim());
            SectionsWithNoSpace.Add(sectionName.Trim().RemoveWhitespace());
        }
        public void AddVerbFlexion(string verb)
        {
            VerbFlexion.Add(verb.Trim());
        }


        public List<string> GetFilterdSections(string languageFilter)
        {
            var sectionList = new HashSet<string>(Sections.Where(s=>s.ContainsWholeWord(languageFilter))).ToList();
            sectionList.Sort();
            return sectionList;
        }
    }
}