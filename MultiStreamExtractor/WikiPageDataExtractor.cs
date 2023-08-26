using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using WiktionaireParser.Models;

public static class WikiPageDataExtractor
{
    public static void Extract(List<WikiPage> pagesList)
    {
        var wikiCollection = AppResources.Instance.WikiPageCollection;
    }



    public static void SaveToDb(List<WikiPage> pagesList)
    {
        AppResources.Instance.Database.DropCollection(AppResources.Instance.WikiPageCollectionName);
        var wikiPageCollection = AppResources.Instance.WikiPageCollection;

        const int batchSize = 10000; // For example, 10,000 documents per batch
        int numberOfBatches = (int)Math.Ceiling((double)pagesList.Count / batchSize);

        for (int i = 0; i < numberOfBatches; i++)
        {
            var batch = pagesList.Skip(i * batchSize).Take(batchSize).ToList();

            wikiPageCollection.InsertMany(batch);
        }
    }


    /// <summary>
    /// Saves a list of WikiPage objects to the database. If a WikiPage with the same title already exists, 
    /// it updates the existing record; otherwise, it inserts a new record.
    /// </summary>
    /// <param name="pagesList">The list of WikiPage objects to save.</param>
    public static void SaveToDb1(List<WikiPage> pagesList)
    {
        // Get the MongoDB collection for WikiPage objects from the application resources.
        var wikiPageCollection = AppResources.Instance.WikiPageCollection;

        // Define the number of WikiPage objects to process in each batch.
        const int batchSize = 10000;

        // Calculate the total number of batches required to process all WikiPage objects.
        int numberOfBatches = (int)Math.Ceiling((double)pagesList.Count / batchSize);

        // Loop through each batch.
        for (int i = 0; i < numberOfBatches; i++)
        {
            // Extract the current batch of WikiPage objects from the list.
            var batch = pagesList.Skip(i * batchSize).Take(batchSize).ToList();

            // For each WikiPage in the batch, create a ReplaceOneModel. This model tells MongoDB to replace 
            // the document if it exists or insert it if it doesn't. The filter is based on the Title property.
            var upserts = batch.Select(page =>
                new ReplaceOneModel<WikiPage>(
                    // Filter to match documents based on the Title property.
                    filter: Builders<WikiPage>.Filter.Eq(p => p.Title, page.Title),
                    // The WikiPage object to insert or replace with.
                    replacement: page)
                {
                    // Indicate that if the document doesn't exist, it should be inserted.
                    IsUpsert = true
                }).ToList();

            // Execute the batch upsert operation on the MongoDB collection.
            wikiPageCollection.BulkWrite(upserts);
        }
    }


    private static Dictionary<string, List<string>> ExtractData(string entryContent)
    {
        var extractedData = new Dictionary<string, List<string>>();

        // Extracting word senses
        var sensesMatches = Regex.Matches(entryContent, @"# ([^#*].+)");
        extractedData["Senses"] = sensesMatches.Select(m => m.Groups[1].Value.Trim()).ToList();

        // Extracting synonyms
        var synonymsMatches = Regex.Match(entryContent, @"==== {{S|synonymes}} ====\s*(?<synonyms>(\* \[\[.+?\]\]\s*)+)");
        if (synonymsMatches.Success)
        {
            var synonyms = Regex.Matches(synonymsMatches.Groups["synonyms"].Value, @"\* \[\[(.+?)\]\]");
            extractedData["Synonyms"] = synonyms.Select(m => m.Groups[1].Value).ToList();
        }

        // Extracting derived terms
        var derivedMatches = Regex.Match(entryContent, @"==== {{S|dérivés}} ====\s*{{\(}}\s*(?<derivatives>(\* \[\[.+?\]\]\s*)+)");
        if (derivedMatches.Success)
        {
            var derivatives = Regex.Matches(derivedMatches.Groups["derivatives"].Value, @"\* \[\[(.+?)\]\]");
            extractedData["Derived Terms"] = derivatives.Select(m => m.Groups[1].Value).ToList();
        }

        // Extracting translations
        var translationsMatches = Regex.Match(entryContent, @"{{trad-début}}\s*(?<translations>(\* {{T\|.+?}} : {{.+?}}\s*)+)");
        if (translationsMatches.Success)
        {
            var translations = Regex.Matches(translationsMatches.Groups["translations"].Value, @"\* {{T\|(?<lang>.+?)}} : {{(?<type>.+?)\|(?<word>.+?)}}");
            extractedData["Translations"] = translations.Select(m => $"{m.Groups["lang"].Value}: {m.Groups["word"].Value}").ToList();
        }

        // Extracting pronunciations
        var pronunciationMatches = Regex.Matches(entryContent, @"{{pron\|(?<pronunciation>.+?)\|fr}}");
        extractedData["Pronunciations"] = pronunciationMatches.Select(m => m.Groups["pronunciation"].Value).ToList();

        return extractedData;
    }

}