using System.Collections.Generic;
using CommonLibTools.Extensions;

namespace CommonLibTools.DataStructure.Dawg.Construction
{
    public class ConstructTrieNode
    {
        public int Intsig = 0;
        public string StringSig = "";
        public bool IsCounted = false;
        public bool IsSaveCounted = false;
        public bool IsSavedToFile = false;
        public bool isSaved = false;
        public int numero = -1;
        //public string sigStr="";
        public char value = '0';
        public IDictionary<char, ConstructTrieNode> ChildNodes = null;
        public bool IsEnd = false;
        public int profondeur = -1;
        public int equalNunber = -1;
        public ConstructTrieNode parent = null;
        public bool isVisited = false;

        public ConstructTrieNode()
        {
            Intsig = 0;
            ChildNodes = null;
            // IsEnd = false;
            value = '0';
            parent = null;
        }

        public bool HasSameStringSigWith(ConstructTrieNode trieNode)
        {

            var sameChild = this.StringSig.SortCharInString() == trieNode.StringSig.SortCharInString();
            var sameWordEnding = this.IsEnd == trieNode.IsEnd;
            return sameChild && sameWordEnding;
        }
        public bool Contains(char c)
        {
            if (ChildNodes == null)
            {
                return false;
            }
            return ChildNodes.ContainsKey(c);
        }


        public ConstructTrieNode GetChild(char c)
        {
            return ChildNodes[c];
        }

        public ConstructTrieNode GetChildOrNull(char car)
        {
            if (ChildNodes.ContainsKey(car))
            {
                return ChildNodes[car];
            }
            return null;
        }
    }
}