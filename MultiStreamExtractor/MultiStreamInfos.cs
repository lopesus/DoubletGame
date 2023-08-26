using System;
using System.IO;
using System.Linq;

namespace MultiStreamExtractor;

public class MultiStreamInfos
{
    public string ArticlesPath { get; set; }
    public string IndexPath { get; set; }

    public WikiLang WikiLang { get; set; }

    public string GetExtractionFolder()
    {
        var dir = Path.GetDirectoryName(ArticlesPath);

        var name=Path.GetFileNameWithoutExtension(ArticlesPath).Split('-', StringSplitOptions.None).First();
        var combine = Path.Combine(dir, $"{name}_parsed");
        return combine;
    }

    public string GetRawPageFolder()
    {
        var dir = GetExtractionFolder();

        var combine = Path.Combine(dir, $"rawPages");
        return combine;
    }
}