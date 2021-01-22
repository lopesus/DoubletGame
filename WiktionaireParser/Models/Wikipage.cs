using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson.Serialization.Attributes;

namespace WiktionaireParser.Models
{
   public class WikiPage
    {
        [BsonId]
        public string  Title { get; set; }
        public string  Text { get; set; }

        public int Len;
        public bool IsVerb;
        public bool IsNoun;
        public bool IsAdjective;

        public WikiPage(string title, string text)
        {
            Title = title;
            Text = text;
            Len = Title.Length;
        }

        public override string ToString()
        {
            return $"{Title}";
        }
    }
}
