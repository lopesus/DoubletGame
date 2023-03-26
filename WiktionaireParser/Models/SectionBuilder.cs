using System.Collections.Generic;
using CommonLibTools.Libs.Extensions;

namespace WiktionaireParser.Models
{
    public class SectionBuilder
    {
        public HashSet<string> Sections { get; set; } = new HashSet<string>();
        public HashSet<string> SectionsWithNoSpace { get; set; } = new HashSet<string>();
        public HashSet<string> VerbFlexion { get; set; } = new HashSet<string>();
        public void AddSection(string sectionName)
        {
            Sections.Add(sectionName.Trim());
            SectionsWithNoSpace.Add(sectionName.Trim().RemoveWhitespace());
        }
        public void AddVerbFlexion(string verb)
        {
            VerbFlexion.Add(verb.Trim());
        }

    }
}