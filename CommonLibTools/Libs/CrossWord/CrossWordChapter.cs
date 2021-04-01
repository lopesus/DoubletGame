using System.Collections.Generic;
using System.Text;

namespace CommonLibTools.Libs.CrossWord
{
    public class CrossWordChapter
    {

        public int  Id { get; set; }
        public List<CrossWordLevel> AllLevel { get; set; }

        public CrossWordChapter()
        {
            
        }
        public CrossWordChapter(int id)
        {
            Id = id;
            AllLevel=new List<CrossWordLevel>();
        }

        //public override string ToString()
        //{
        //    var builder = new StringBuilder();
        //    foreach (var level in AllLevel)
        //    {
        //        builder.AppendLine($"{level}");
        //    }
        //    return builder.ToString();
        //}

        public void AddLevel(CrossWordLevel level)
        {
            AllLevel.Add(level);
        }

        
    }
}