using System;
using System.Collections.Generic;
using CommonLibTools.DataStructure.Dawg.Algo;

namespace CommonLibTools.DataStructure.Dawg
{


    public class DawgService
    {
        //private static volatile DawgService instance = null;
        private static object syncRoot = new Object();
        Trie trie = new Trie();

        public Trie GetTrie()
        {
            return trie;
        }
        private DawgService(string nodeValueFilePath, string nodeInfosFilePath)
        {
            //trie.LoadFromFile(nodeValueFilePath, nodeInfosFilePath);
        }

        public DawgService(string[] data)
        {
            trie.LoadFromUniqueFileData(data);
        }
        public DawgService(IList<string> data)
        {
            trie.LoadFromUniqueFileData(data);
        }

        //public static DawgService GetInstance(string nodeValueFilePath, string nodeInfosFilePath)
        //{
        //    if (instance == null)
        //    {
        //        lock (syncRoot)
        //        {
        //            if (instance == null)
        //            {
        //                instance = new DawgService(nodeValueFilePath, nodeInfosFilePath);
        //            }
        //        }
        //    }
        //    return instance;
        //}

        //public  DawgService GetInstance(string[] data, bool reset = false)
        //{
        //    DawgService instance = null;

        //    lock (syncRoot)
        //    {
        //        instance = new DawgService(data);
        //    }

        //    return instance;
        //}

        //public  DawgService GetInstance(IList<string> data, bool reset = false)
        //{
        //    DawgService instance = null;
        //    if (reset)
        //    {
        //        instance = null;
        //    }
        //    lock (syncRoot)
        //    {
        //        instance = new DawgService(data);
        //    }
        //    return instance;
        //}


        public bool ContainString(string s)
        {
            return trie.ContainString(s);
        }

        public List<string> FindExactAnagramme(string s)
        {
            return TrieUtils.FindExactAnagrams(s, trie.GetRoot());
        }

        public Dictionary<int, List<string>> MotFinissantPar(string tirage, string mot, bool useTirage, Range range, bool includeComplementToTirage)
        {
            if (mot != null)
            {
                mot = mot.RemoveAllJokerAndNonAlpha().ToLower();
                return TrieAlgo.MotFinissantPar(tirage, mot, trie, useTirage, range, includeComplementToTirage);
            }
            return null;
        }
        public Dictionary<int, List<string>> MotCommencantPar(string tirage, string mot, bool useTirage, Range range, bool includeComplementToTirage)
        {
            if (mot != null)
            {
                //mot = mot.RemoveAllJoker().ToLower();
                mot = mot.RemoveAllJokerAndNonAlpha().ToLower();
                return TrieAlgo.MotCommencantPar(tirage, mot, trie, useTirage, range, includeComplementToTirage);
            }
            return null;
        }
        public Dictionary<int, List<string>> MotContenant(string tirage, string mot, bool useTirage, Range range, bool includeComplementToTirage)
        {
            if (mot != null)
            {
                mot = mot.RemoveAllJokerAndNonAlpha().ToLower();
                return TrieAlgo.MotContenant(tirage, mot, trie, useTirage, range, includeComplementToTirage);
            }
            return null;
        }


        public Dictionary<int, List<string>> RallongeFin(string complement, string tirage, bool useTirage, Range range = null)
        {
            complement = complement.RemoveAllJokerAndNonAlpha().ToLower();
            return TrieAlgo.AllRallongeFinMot(complement, tirage, trie, useTirage, range);
        }

        public IDictionary<int, List<string>> AllRallonge(string complement, string tirage, bool useTirage, Range range, bool includeComplementToTirage)
        {
            complement = complement.RemoveAllJokerAndNonAlpha().ToLower();
            return TrieAlgo.AllRallongeMot(complement, tirage, trie, useTirage, range, includeComplementToTirage);
        }

        public IDictionary<int, List<string>> RallongeDebut(string complement, string tirage, bool useTirage, Range range, bool includeComplementToTirage)
        {
            complement = complement.RemoveAllJokerAndNonAlpha().ToLower();
            return TrieAlgo.AllRallongeDebutMot(complement, tirage, trie, useTirage, range, includeComplementToTirage);
        }


        public List<string> FindAllWordFollowingPattern(string pattern, string tirage, bool limitToTirage, /*bool useMyLetterToFillJokerOnly,*/ Range range)
        {
            pattern = pattern.RemoveNonAlphabeticalChar().ToLower();
            return FindAllWordFollowingPatternAlgo.FindAllWordFollowingPattern(pattern, tirage, trie.GetRoot(), limitToTirage, range);
            //return TrieAlgoForDisplay.FindAllWordFollowingPatternOld(pattern, tirage, trie.GetRoot(), limitToTirage, useMyLetterToFillJokerOnly, range);
            //return TrieUtils.AllPossibleFixedPosWord(tirage, complement, trie.GetRoot(), useTirage);
        }




        public List<string> FindExactAnagrammeWithPosition(string s, string tirage)
        {
            return TrieUtils.FindExactAnagramsWithFixedPosition(s, tirage, trie.GetRoot());
        }

        public IDictionary<int, List<string>> FindAllPossibleWord(string s, bool toUpperCase = false, bool showJoker = false, Range range = null)
        {
            var options = new DisplayOptions(true, true, true);
            return AllPossibleWordAlgo.FindAllPossibleWord(s, trie.GetRoot(), options, range, mustContainCar: "");
            //return TrieUtils.FinadAnna(s, trie.GetRoot(), toUpperCase, showJoker);
        }
    }
}