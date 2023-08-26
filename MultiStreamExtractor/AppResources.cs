using System;
using System.IO;
using System.Linq;
using MongoDB.Driver;
using MultiStreamExtractor;
using WiktionaireParser.Models;

public class AppResources
{
    private static readonly Lazy<AppResources> _instance = new Lazy<AppResources>(() => new AppResources());

    public static AppResources Instance => _instance.Value;

    // Properties for your shared resources.
    public string WiktioParserResultFolder { get; private set; }
    public string DicoName { get; private set; }
    public string DicoTrieName { get; private set; }
    public MongoClient Client { get; private set; }
    public IMongoDatabase Database { get; private set; }
    public string WikiPageCollectionName { get; private set; }
    public IMongoCollection<WikiPage> WikiPageCollection { get; private set; }
    public string AnagramCollectionName { get; private set; }
    public IMongoCollection<Anagram> AnagramCollection { get; private set; }
    public IMongoCollection<WordFrequency> WordFrequencyCollection { get; private set; }

    // Private constructor to prevent multiple instances.
    private AppResources()
    {
        // This remains empty. Initialization is done in the Initialize method.
    }

    public void Initialize(MultiStreamInfos wiktionaryInfos, int minLen, int maxLen)
    {
        var resultFolder = wiktionaryInfos.GetExtractionFolder();// Path.GetFileNameWithoutExtension(wiktionaryInfos.GetExtractionFolder()).Split('-').FirstOrDefault();
        WiktioParserResultFolder = wiktionaryInfos.GetExtractionFolder();// Path.Combine(Path.GetDirectoryName(wiktionaryInfos.GetExtractionFolder()), $"{resultFolder}_Parse");
        Directory.CreateDirectory(WiktioParserResultFolder);
        DicoName = Path.Combine(WiktioParserResultFolder, $"wordbox_valid_word_{minLen}_{maxLen}.txt");
        DicoTrieName = Path.Combine(WiktioParserResultFolder, $"wordbox_valid_word_{minLen}_{maxLen}_trie.txt");

        Client = new MongoClient();
        Database = Client.GetDatabase("wiktio");
        WikiPageCollectionName = $"WikiPage_{wiktionaryInfos.WikiLang}";
        WikiPageCollection = Database.GetCollection<WikiPage>(WikiPageCollectionName);

        AnagramCollectionName = $"WordBox_anagram_{wiktionaryInfos.WikiLang}";
        AnagramCollection = Database.GetCollection<Anagram>(AnagramCollectionName);
        WordFrequencyCollection = Database.GetCollection<WordFrequency>("WordFrequency");
    }
}
