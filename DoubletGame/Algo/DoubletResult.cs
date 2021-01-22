using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DoubletGame.Algo
{
    public class DoubletResult
    {
        public List<string> WordList { get; set; }

        public DoubletResult()
        {
            WordList = new List<string>();
        }
        public DoubletResult(DoubletResult copy)
        {
            WordList = new List<string>();
            if (copy != null)
            {
                WordList = copy.WordList.ToList();
            }
        }

        public void AddWord(string source)
        {
            WordList.Add(source);
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine($"Len {WordList.Count}");
            for (int i = 0; i < WordList.Count; i++)
            {
                builder.Append($"{WordList[i]}-");
                if (i != 0 && i % 5 == 0)
                {
                    builder.AppendLine();
                }
            }
            builder.AppendLine();
            builder.AppendLine();

            return builder.ToString();
        }
    }
}