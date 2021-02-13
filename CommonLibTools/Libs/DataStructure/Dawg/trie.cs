using System;
using System.Collections.Generic;
using System.Linq;
using CommonLibTools.Libs.Extensions;

namespace CommonLibTools.Libs.DataStructure.Dawg
{
    //****************************************
    public class Trie
    {
        public string Name { get; set; }
        private TrieNode root = new TrieNode();
        //private int[] translatorChars;

        public Trie()
        {
            //translatorChars = new int[123];

            //translatorChars = Enumerable.Repeat(-1, 123).ToArray();

            //for (char c = 'a'; c <= 'z'; c++)
            //{
            //    translatorChars[(int)c] = (int)c - 97;
            //}
        }
        public TrieNode GetRoot()
        {
            return root;
        }
        public TrieNode Insert(string s)
        {
            //Console.WriteLine(s);
            char[] charArray = s.ToLower().ToCharArray();

            TrieNode currentNode = root;

            foreach (char c in charArray)
            {
                currentNode = Insert(c, currentNode);
            }
            currentNode.IsEnd = true;
            return root;
        }

        private TrieNode Insert(char c, TrieNode currentNode)
        {
            if (currentNode.Contains(c))

                return currentNode.GetChild(c);

            else
            {
                if (currentNode.ChildNodes == null)
                {
                    currentNode.ChildNodes = new Dictionary<char, TrieNode>();
                }
                TrieNode newNode = new TrieNode();
                //int val = translatorChars[c];
                currentNode.ChildNodes.Add(c, newNode);
                return newNode;
            }
        }

        public bool ContainString(string s)
        {
            char[] charArray = s.ToLower().ToCharArray();
            TrieNode node = root;

            bool contains = true;

            foreach (char c in charArray)
            {

                node = ContainChar(c, node);  //check if c is in the node
                //Console.WriteLine(c);

                if (node == null)  //c is not in the node
                {
                    contains = false;
                    break;
                }
            }

            if ((node == null) || (!node.IsEnd))
                contains = false;
            return contains;
        }

        public TrieNode GetLastNode(string s)
        {
            //char[] charArray = s.SansAccent().ToLower().ToCharArray();
            //char[] charArray = s.ToCharArray();
            TrieNode node = root;
            if (!string.IsNullOrEmpty(s))
            {
               
                foreach (char c in s)
                {

                    node = node.GetChildOrNull(c);  //check if c is in the node
                    if (node == null)  //c is not in the node
                    {
                        return null;
                    }
                }

                return node;
            }
            else return root;
        }

        private TrieNode ContainChar(char c, TrieNode node)
        {
            return node.GetChildOrNull(c);

            /* if (node.Contains(c))
             {
                 return node.GetChild(c);
             }
             else
             {
                 return null;
             }*/
        }

       public void LoadFromUniqueFileData(string[] lines)
        {
            var tmp = new TrieNode[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                tmp[i] = new TrieNode();
                var line = lines[i];
                var tokens = line.Split();
                // token[0][0]=car token[0][1]=isEnd 
                var token0 = tokens[0].ToCharArray();
                tmp[i].value = Convert.ToChar(token0[0]);
                tmp[i].IsEnd = token0[1] == '1' ? true : false;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                var tokens = lines[i].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 1)
                {
                    var tok1 = tokens[1];
                    var nodesNumber = tok1.Split(",".ToCharArray());
                    foreach (var num in nodesNumber)
                    {
                        //var numNode = num.Base16ToInt32();
                        var numNode = num.Base64ToInt32();
                        //var numNode = Convert.ToInt32(num);
                        var childNode = tmp[numNode];
                        if (tmp[i].ChildNodes == null)
                        {
                            tmp[i].ChildNodes = new Dictionary<char, TrieNode>();
                        }
                        tmp[i].ChildNodes.Add(childNode.value, childNode);
                    }
                }
            }

            root = tmp[0];
        }

        public void LoadFromUniqueFileData(IList<string> lines)
        {
            var tmp = new TrieNode[lines.Count];
            for (int i = 0; i < lines.Count-1; i++)
            {
                tmp[i] = new TrieNode();
                var tokens = lines[i].Split();
                // token[0][0]=car token[0][1]=isEnd 
                var token0 = tokens[0].ToCharArray();
                tmp[i].value = Convert.ToChar(token0[0]);
                tmp[i].IsEnd = token0[1] == '1' ? true : false;
            }

            for (int i = 0; i < lines.Count-1; i++)
            {
                var tokens = lines[i].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Length > 1)
                {
                    var tok1 = tokens[1];
                    var nodesNumber = tok1.Split(",".ToCharArray());
                    foreach (var num in nodesNumber)
                    {
                        //var numNode = num.Base16ToInt32();
                        var numNode = num.Base64ToInt32();
                        //var numNode = Convert.ToInt32(num);
                        var childNode = tmp[numNode];
                        if (tmp[i].ChildNodes == null)
                        {
                            tmp[i].ChildNodes = new Dictionary<char, TrieNode>();
                        }
                        tmp[i].ChildNodes.Add(childNode.value, childNode);
                    }
                }
            }

            root = tmp[0];
            Name = lines.Last();
        }
    }
}
