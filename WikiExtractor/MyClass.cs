using ICSharpCode.SharpZipLib.BZip2;
using System;
using System.IO;


public class Program
{
    public static void Main0()
    {
        var reader = new WikipediaReader(
            @"path_to\pages-articles-multistream.xml.bz2",
            @"path_to\index.txt"
        );

        var articleContent = reader.ExtractArticleContent("Le meilleur des mondes");
        Console.WriteLine(articleContent);
    }
}
