using System.Collections.Generic;

namespace CommonLibTools.Extensions
{
   public static  class CollectionsExtensions
    {
       public static string GetValueIfExistOrEmpty(this Dictionary<string, string> dictionary,string key)
       {
           var res = "";
           if (dictionary != null)
           {
               dictionary.TryGetValue(key, out res);
           }
           return res;
       }
    }
}
