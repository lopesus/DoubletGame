using System.Collections.Generic;

namespace CommonLibTools.Libs.DataStructure.Dawg
{
	public class TrieNode
	{
		public IDictionary<char, TrieNode> ChildNodes = null;
		public bool IsEnd = false;
		public char value = '0';

		public bool Contains (char c)
		{
			if (ChildNodes == null) {
				return false;
			}
			return ChildNodes.ContainsKey (c);
		}


		public TrieNode GetChild (char c)
		{
			return ChildNodes [c];
		}

		public TrieNode GetChildOrNull (char car)
		{
			if (ChildNodes != null) {
				if (ChildNodes.ContainsKey (car)) {
					return ChildNodes [car];
				} 
			}
          
			return null;
		}

		public override string ToString ()
		{
			
			return base.ToString ();// $"Value : { value }, IsEnd : {IsEnd}";
		}
	}
}