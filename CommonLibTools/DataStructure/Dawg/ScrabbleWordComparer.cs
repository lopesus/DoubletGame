using System;
using System.Collections.Generic;
using CommonLibTools.Extensions;

namespace CommonLibTools.DataStructure.Dawg
{
    public class ScrabbleWordComparer : EqualityComparer<string>
    {
        public override bool Equals(string x, string y)
        {

            //Check whether the compared objects reference the same data. 
            if (Object.ReferenceEquals(x, y))
                return true;

            //Check whether any of the compared objects is null. 
            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                return false;

            var s1 = x.RemoveAllMarks().ToLower();
            var s2 = y.RemoveAllMarks().ToLower();
            if (s1 == "art")
            {
                s1.PrintObject();
            }
            return s1 == s2;
        }

        // If Equals() returns true for a pair of objects  
        // then GetHashCode() must return the same value for these objects. 

        public override int GetHashCode(string scrabbleWord)
        {
            //Check whether the object is null 
            if (Object.ReferenceEquals(scrabbleWord, null)) return 0;

            return scrabbleWord.Replace("*", "").GetHashCode();
        }

    }
}