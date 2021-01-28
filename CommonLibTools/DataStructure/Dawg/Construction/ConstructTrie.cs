using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using CommonLibTools.Extensions;

namespace CommonLibTools.DataStructure.Dawg.Construction
{
    /* 
     * 
     */

    public class ConstructTrie
    {
        private ConstructTrieNode[] tmp = null;
        //public int equalNumberGen = 0;
        public static int maxLength = 100;
        public IDictionary<char, IList<ConstructTrieNode>>[] depthList = new IDictionary<char, IList<ConstructTrieNode>>[maxLength];
        public ConstructTrieNode root = new ConstructTrieNode();
        private int[] translatorChars;
        public const int wordEnd = 67108864;
        public bool IsVisited = false;

        public ConstructTrie()
        {
            for (int i = 0; i < depthList.Length; i++)
            {
                depthList[i] = new Dictionary<char, IList<ConstructTrieNode>>();
            }
            translatorChars = new int[123];

            translatorChars = Enumerable.Repeat(-1, 123).ToArray();

            for (char c = 'a'; c <= 'z'; c++)
            {
                translatorChars[(int)c] = (int)c - 97;
            }
        }

        public void GenerateTrieFromFile(string fileName)
        {
            var lines = File.ReadAllLines(fileName);
            var dir = Path.GetDirectoryName(fileName);
            var temP = $"{Path.GetFileNameWithoutExtension(fileName)}_trie.txt";
            //var saveToFileName = Path.Combine(dir, temP);
            var saveToFileName = @"D:\__programs_datas\" + temP;
            ConstructTrieWorker(lines, saveToFileName);
        }

        private async void ConstructTrieWorker(IList<string> listeMots, string saveToFileName)
        {
            long maxLine = int.MaxValue;
            Console.WriteLine("###################### " + saveToFileName);
            ConstructTrie constructTrie = new ConstructTrie();
            //var encoding = Encoding.GetEncoding("Windows-1252");
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            TimeSpan duree = end.Subtract(start);
            long numMot = Math.Min(maxLine, listeMots.Count);
            var dd = listeMots.ToList();
            Console.WriteLine(dd.Count);


            var content = string.Format("\nbegin construction trie please wait  ");
            Console.WriteLine(content);

            for (int i = 0; i < numMot; i++)
            {
                // Console.WriteLine("inserting   " + i);
                constructTrie.Insert(listeMots[i].ToLower());
                //var reverse = Utils.Reverse(Utils.SansAccent00(listeMots[i]));
                //trie.Insert(reverse);
            }
            end = DateTime.Now;
            duree = end.Subtract(start);
            content = string.Format("\nend construction trie duration   {0} node number {1} ", duree, constructTrie.CountNode(false));
            Console.WriteLine(content);
            //findall(trie, listeMots, 0, numMot);

            //profondeur
            start = DateTime.Now;
            content = string.Format("\nbegin calculprofondeur trie please wait  ");
            Console.WriteLine(content);
            constructTrie.CalculProfondeur();
            end = DateTime.Now;
            duree = end.Subtract(start);
            content = string.Format("end calculprofondeur trie duration   {0}  ", duree);
            Console.WriteLine(content);
            //findall(trie, listeMots, 0, numMot);

            //combine
            start = DateTime.Now;
            content += string.Format("\nbegin combine trie please wait  ");
            Console.WriteLine(content);
            constructTrie.CombineNode(listeMots, 0, numMot);
            end = DateTime.Now;
            duree = end.Subtract(start);
            content += string.Format("\nend combine trie duration   {0} node number {1} ", duree, constructTrie.CountNode(true));
            Console.WriteLine(content);
            content = string.Format("\npress enter to test all word");

            Findall(constructTrie, listeMots, 0, numMot);
            content += string.Format("\nsaving to file");
            Console.WriteLine(content);
            constructTrie.SaveToUniqueFile(saveToFileName);
            string.Format("saved to file");
            Console.WriteLine(content);
            constructTrie = null;

            var trie2 = new Trie();

            content += string.Format("\nLoad From Unique File");
            Console.WriteLine(content);
            start = DateTime.Now;


            var lines = File.ReadAllLines(saveToFileName);
            trie2.LoadFromUniqueFileData(lines);
            end = DateTime.Now;
            duree = end.Subtract(start);
            content += string.Format("\nloaded From Unique File {0}", duree);
            Console.WriteLine(content);
            FindallSimpleTrie(trie2, listeMots, 0, numMot);
            Console.WriteLine("###################### " + saveToFileName);
        }

        bool FindallSimpleTrie(Trie trie, IList<string> list, int startIndex, long endIndex)
        {
            var error = false;
            string content = "";
            for (int i = startIndex; i < endIndex; i++)
            {
                var mot = list[i].ToLower();
                if (trie.ContainString(mot) == false)
                {
                    error = true;
                    content += string.Format("not all word finded  FindallSimpleTrie " + list[i]);
                    Console.WriteLine(content);

                    return false;

                }
                else
                {
                    // Console.WriteLine(" word finded  " + mot + "  " + i);
                }
            }
            if (error == false)
            {
                content += string.Format("\nall words finded  FindallSimpleTrie");
                Console.WriteLine(content);

            }
            return true;
        }

        bool Findall(ConstructTrie trie, IList<string> list, int startIndex, long endIndex)
        {
            var error = false;
            string content = "";
            for (int i = startIndex; i < endIndex; i++)
            {
                if (trie.Contains(list[i].ToLower()) == false)
                {
                    error = true;
                    content += string.Format("not all word finded  " + list[i]);
                    Console.WriteLine(content);

                    return false;

                }
                else;
                // Console.WriteLine(" word finded  " + list[i] + "  "+ i);
            }
            if (error == false)
            {
                content += string.Format("\nall words finded ");
                Console.WriteLine(content);

            }
            return true;
        }


        public async void SaveToUniqueFile(string filePath)
        {
            // Create sample file; replace if exists.
            var path = Path.GetDirectoryName(filePath);
            Directory.CreateDirectory(path);
            // FileStream fs = File.Create(filePath);
            StreamWriter sw = new StreamWriter(new FileStream(filePath,FileMode.Create));
            int counter = 0;
            SaveToUniqueFileWorker(root, ref counter, sw);
            //la derniere ligne contient le nom du dico
            sw.WriteLine(filePath);
            sw.Flush();
            sw.Dispose();
        }

        private void SaveToUniqueFileWorker(ConstructTrieNode node, ref int counter, StreamWriter writer)
        {

            var count = counter;
            AssignationnumeroDesNoeud(node, ref count);

            if (node != null && node.IsSavedToFile == false)
            {
                node.IsSavedToFile = true;
                node.IsSaveCounted = true;
                //le numero de la ligne dans le fichier correspond au numero du noeud
                //la racine a toujours le numero 0
                //format =num noeud + valeur du noeud + marque de fin de mot + liste numero noeuds enfants


                var format = /*node.numero + " " + */ node.value + "" + (node.IsEnd ? 1 : 0);//+" ";
                if (node.ChildNodes != null)
                {
                    var infosEnfants = "";
                    foreach (char car in node.ChildNodes.Keys)
                    {
                        //var numero = node.GetChild(car).numero.Int32ToBase16();
                        var numero = node.GetChild(car).numero.Int32ToBase64();
                        infosEnfants += numero + ",";
                    }
                    if (infosEnfants.IsNotNullOrEmptyString())
                    {
                        format += " " + infosEnfants;
                    }
                }
                writer.WriteLine(format.TrimEnd(",".ToCharArray()));
                if (node.ChildNodes != null)
                {
                    foreach (char car in node.ChildNodes.Keys)
                    {
                        SaveToUniqueFileWorker(node.GetChild(car), ref counter, writer);
                    }
                }
            }
        }

        public void LoadFromUniqueFile(string nodeValueFilePath)
        {
            var lines = File.ReadAllLines(nodeValueFilePath);
            tmp = new ConstructTrieNode[lines.Length];
            for (int i = 0; i < lines.Length; i++)
            {
                tmp[i] = new ConstructTrieNode();
                var tokens = lines[i].Split();
                // token[0][0]=car token[0][1]=isEnd 
                var token0 = tokens[0].ToCharArray();
                tmp[i].value = Convert.ToChar(token0[0]);
                tmp[i].IsEnd = token0[1] == '1' ? true : false;
            }

            for (int i = 0; i < lines.Length; i++)
            {
                var tokens = lines[i].Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (tokens.Count() > 1)
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
                            tmp[i].ChildNodes = new Dictionary<char, ConstructTrieNode>();
                        }
                        tmp[i].ChildNodes.Add(childNode.value, childNode);
                    }
                }
            }

            root = tmp[0];
        }

        public ConstructTrieNode Insert(string s)
        {
            //Console.WriteLine(s);
            char[] charArray = s.ToLower().ToCharArray();

            ConstructTrieNode currentNode = root;

            foreach (char c in charArray)
            {
                currentNode = Insert(c, currentNode);
            }
            currentNode.IsEnd = true;
            currentNode.Intsig |= wordEnd;
            return root;
        }

        private ConstructTrieNode Insert(char c, ConstructTrieNode currentNode)
        {
            if (currentNode.Contains(c))

                return currentNode.GetChild(c);

            else
            {
                if (currentNode.ChildNodes == null)
                {
                    currentNode.ChildNodes = new Dictionary<char, ConstructTrieNode>();
                }
                ConstructTrieNode newNode = new ConstructTrieNode();
                int val = c;// translatorChars[c];
                currentNode.Intsig |= (1 << val);
                currentNode.StringSig += c;
                newNode.value = c;
                newNode.parent = currentNode;
                currentNode.ChildNodes.Add(c, newNode);
                return newNode;
            }
        }

        public int CalculProfondeur()
        {
            return CalculProfondeurNode(root);
        }

        public void AssignationnumeroDesNoeud(ConstructTrieNode node, ref int count)
        {

            if (node != null && node.IsSaveCounted == false)
            {
                node.numero = count++;
                node.IsSaveCounted = true;
                if (node.ChildNodes != null)
                {
                    foreach (char car in node.ChildNodes.Keys)
                    {
                        AssignationnumeroDesNoeud(node.GetChild(car), ref count);
                    }
                }
            }
        }

        private int CalculProfondeurNode(ConstructTrieNode node)
        {

            if (node.ChildNodes == null)
            {
                node.profondeur = 0;
                var a = depthList[0];
                if (a.ContainsKey(node.value) == false)
                {
                    depthList[0].Add(node.value, new List<ConstructTrieNode>());
                    //Console.WriteLine("profondeur {0} value {1}",0,node.value);
                }

                var b = depthList[0][node.value];
                depthList[0][node.value].Add(node);
                return 0;
            }
            else
            {
                int prof = 0;
                foreach (char c in node.ChildNodes.Keys)
                {
                    var child = node.GetChildOrNull(c);
                    if (child == null)
                    {
                        //Console.WriteLine(node.ChildNodes);
                        throw new ArgumentNullException();
                    }
                    else
                    {
                        prof = Math.Max(prof, CalculProfondeurNode(child));
                    }
                }
                node.profondeur = prof + 1;
                var a = depthList[prof + 1];
                if (a.ContainsKey(node.value) == false)
                    depthList[prof + 1].Add(node.value, new List<ConstructTrieNode>());
                var b = depthList[prof + 1][node.value];
                depthList[prof + 1][node.value].Add(node);
                return prof + 1;
            }
        }

        public bool nodeEquals(ConstructTrieNode node1, ConstructTrieNode node2)
        {
            if (node1.value == node2.value)
            {
                if (node1.Intsig == node2.Intsig)
                {
                    if (node1.profondeur == node2.profondeur)
                    {
                        if (node1.profondeur != 0)
                        {
                            foreach (char car in node1.ChildNodes.Keys)
                            {
                                //Console.WriteLine(node1.Intsig+"  vs  "+node2.Intsig);
                                if (node1.GetChild(car).equalNunber == node2.GetChild(car).equalNunber)
                                {
                                    return true;
                                }
                            }
                        }
                        else //profondeur ==0
                        {
                            return node1.IsEnd == node2.IsEnd;
                        }

                    }

                }
            }
            return false;
        }

        public bool nodeEquals2(ConstructTrieNode node1, ConstructTrieNode node2)
        {
            if (node1 == null || node2 == null)
            {
                return false;
            }
            if (node1.value == node2.value)
            {
                //if (node1.Intsig == node2.Intsig)
                if (node1.HasSameStringSigWith(node2))
                {
                    if (node1.profondeur == node2.profondeur)
                    {
                        if (node1.profondeur != 0)
                        {
                            foreach (char car in node1.ChildNodes.Keys)
                            {
                                //Console.WriteLine(node1.Intsig+"  vs  "+node2.Intsig);
                                var child1 = node1.GetChildOrNull(car);
                                var child2 = node2.GetChildOrNull(car);
                                if (nodeEquals2(child1, child2) == false)
                                {
                                    return false;
                                }
                            }
                            return true;
                        }
                        else //profondeur ==0
                        {
                            return node1.IsEnd == node2.IsEnd;
                        }

                    }

                }
            }
            return false;
        }

        public void CombineNode(IList<string> liststr, int startIndex, long endIndex)
        {
            for (int i = 0; i < depthList.Length; i++)
            {
                Console.WriteLine("depth = " + i);
                foreach (char car in depthList[i].Keys)
                {
                    var list = depthList[i][car];
                    if (list.Count > 0)
                    {
                        var node1 = list[0];
                        //node1.equalNunber = equalNumberGen;
                        // equalNumberGen++;
                        for (int j = 1; j < list.Count; j++)
                        {
                            var node2 = list[j];
                            if (nodeEquals2(node1, node2))
                            {
                                //Console.WriteLine("node equal");
                                var r = node2.parent.ChildNodes[node2.value] == node2;
                                if (r == false)
                                {
                                    throw new System.Exception("parent node and node are diff");
                                }
                                // Console.WriteLine("assert equal parent node  " + r);
                                node2.parent.ChildNodes[node2.value] = node1;
                                //dispose
                                list[j].parent = null;
                                list[j].ChildNodes = null;
                            }
                        }
                    }

                }
                //findall(liststr,startIndex,endIndex,i);
            }
        }

        public void find(string word)
        {
            Console.WriteLine(word + " " + this.Contains(word));
        }
        public bool Contains(string s)
        {
            char[] charArray = s.ToLower().ToCharArray();
            ConstructTrieNode node = root;

            bool contains = true;

            foreach (char c in charArray)
            {

                node = Contains(c, node);  //check if c is in the node
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

        private ConstructTrieNode Contains(char c, ConstructTrieNode node)
        {

            if (node.Contains(c))
            {
                return node.GetChild(c);
            }
            else
            {
                return null;
            }
        }

        public long CountNode(bool mark)
        {
            this.IsVisited = mark;
            long res = 0;
            CountNodeWorker(root, ref res);
            return res;
        }

        void CountNodeWorker(ConstructTrieNode node, ref long res)
        {
            if (node.isVisited == false)
            {
                if (node.ChildNodes != null)
                {
                    foreach (char car in node.ChildNodes.Keys)
                    {
                        CountNodeWorker(node.GetChild(car), ref res);
                    }
                }
                if (IsVisited)
                {
                    node.isVisited = true;
                }

                res++;
            }

        }

        void findall(string[] list, int startIndex, long endIndex, int depht)
        {
            var error = false;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (this.Contains(list[i]) == false)
                {
                    error = true;
                    Console.WriteLine("not all word finded {0} in depht {1} ", list[i], depht);
                    //return;

                }
                else;
                // Console.WriteLine(" word finded  " + list[i]);
            }
            if (error == false)
                Console.WriteLine("\n\tall words finded in depht " + depht);
        }
    }
}
