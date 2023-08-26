using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WiktionaireParser.Models
{
    public class ValidWord
    {
        public IDictionary<int, List<string>> Dictionary { get; set; }

        public ValidWord(IDictionary<int, List<string>> allPossibleWord)
        {
            Dictionary = allPossibleWord;
        }

        public override string ToString()
        {
            var builder=new StringBuilder();
            foreach (var pair in Dictionary.OrderByDescending(p=>p.Key))
            {
                builder.AppendLine($"{pair.Key} letters words - {pair.Value.Count}");
                foreach (var word in pair.Value)
                {
                    builder.Append($"{word.ToLowerInvariant()} ");
                }

                builder.AppendLine();
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}