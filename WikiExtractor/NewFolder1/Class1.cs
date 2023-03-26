//using System;
//using System.Collections.Concurrent;
//using System.IO;
//using System.Linq;
//using System.Threading.Tasks;

//class Program
//{
//    static void Main(string[] args)
//    {
//        // Change this to the directory containing your text files
//        string directory = @"C:\path\to\directory";

//        // Change this to the file extension of your text files
//        string fileExtension = "*.txt";

//        // Create a ConcurrentDictionary to store the word counts
//        var wordCounts = CountWords(directory, fileExtension);

//        // Sort the dictionary by value in descending order and print the top 10 words
//        var topWords = wordCounts.OrderByDescending(pair => pair.Value).Take(10);
//        foreach (var pair in topWords)
//        {
//            Console.WriteLine("{0}: {1}", pair.Key, pair.Value);
//        }

//        Console.ReadKey();
//    }

//    private static ConcurrentDictionary<string, int> CountWords(string directory, string fileExtension)
//    {
//        var wordCounts = new ConcurrentDictionary<string, int>();

//        // Get the list of files in the directory
//        string[] files = Directory.GetFiles(directory, fileExtension);

//        // Process each file in parallel
//        Parallel.ForEach(files, file =>
//        {
//            // Read the file into a string
//            string text = File.ReadAllText(file);

//            // Split the text into words
//            string[] words = text.Split(new char[] { ' ', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

//            // Count the occurrence of each word and add it to the dictionary
//            foreach (string word in words)
//            {
//                wordCounts.AddOrUpdate(word.ToLower(), 1, (key, oldValue) => oldValue + 1);
//            }
//        });
//        return wordCounts;
//    }
//}