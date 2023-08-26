using System.Collections.Generic;

namespace MultiStreamExtractor;

public static class Wiktionaries
{
    public static Dictionary<WikiLang, MultiStreamInfos> MultiStreamInfosMap { get; set; } = new Dictionary<WikiLang, MultiStreamInfos>();

    static Wiktionaries()
    {
        MultiStreamInfosMap.Add(WikiLang.French, new MultiStreamInfos()
        {
            ArticlesPath = @"D:\zzzWiktionnaire\multistream\frwiktionary-latest-pages-articles-multistream.xml.bz2",
            IndexPath = @"D:\zzzWiktionnaire\multistream\frwiktionary-latest-pages-articles-multistream-index.txt",
            WikiLang = WikiLang.French,
        });

        MultiStreamInfosMap.Add(WikiLang.English, new MultiStreamInfos()
        {
            WikiLang = WikiLang.English,
            ArticlesPath = @"D:\zzzWiktionnaire\multistream\enwiktionary-latest-pages-articles-multistream.xml.bz2",
            IndexPath = @"D:\zzzWiktionnaire\multistream\enwiktionary-latest-pages-articles-multistream-index.txt"
        });

        MultiStreamInfosMap.Add(WikiLang.Italian, new MultiStreamInfos()
        {
            WikiLang = WikiLang.Italian,
            ArticlesPath = @"gfgdfgD:\zzzWiktionnaire\multistream\itwiktionary-latest-pages-articles-multistream.xml.bz2",
            IndexPath = @"dfgdgD:\zzzWiktionnaire\multistream\enwiktionary-latest-pages-articles-multistream-index.txt"
        });

    }

    public static MultiStreamInfos GetWiktionary(WikiLang lang)
    {
        if (MultiStreamInfosMap.ContainsKey(lang))
        {
            return MultiStreamInfosMap[lang];
        }

        return null;
    }
}