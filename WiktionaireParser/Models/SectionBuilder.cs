using System.Collections.Generic;

namespace WiktionaireParser.Models
{
    public class SectionBuilder
    {
        public HashSet<string> Sections { get; set; }=new HashSet<string>();
        public HashSet<string> VerbFlexion { get; set; }=new HashSet<string>();
        public void AddSection(string sectionName)
        {
            Sections.Add(sectionName.Trim());
        }
public void AddVerbFlexion(string verb)
        {
            VerbFlexion.Add(verb.Trim());
        }

    }
}