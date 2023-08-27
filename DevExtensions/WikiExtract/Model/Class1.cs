using System.Collections.Generic;

public class WordData
{
    public string pos { get; set; }
    public List<HeadTemplate> head_templates { get; set; }
    public List<Form> forms { get; set; }
    public string etymology_text { get; set; }
    public List<EtymologyTemplate> etymology_templates { get; set; }
    public List<Sound> sounds { get; set; }
    public string word { get; set; }
    public string lang { get; set; }
    public string lang_code { get; set; }
    public List<Derived> derived { get; set; }
    public List<Sense> senses { get; set; }
}

public class HeadTemplate
{
    public string name { get; set; }
    public Dictionary<string, string> args { get; set; }
    public string expansion { get; set; }
}

public class Form
{
    public string form { get; set; }
    public List<string> tags { get; set; }
}

public class EtymologyTemplate
{
    public string name { get; set; }
    public Dictionary<string, string> args { get; set; }
    public string expansion { get; set; }
}

public class Sound
{
    public string ipa { get; set; }
    public string audio { get; set; }
    public string text { get; set; }
    public string ogg_url { get; set; }
    public string mp3_url { get; set; }
}

public class Derived
{
    public string word { get; set; }
    public string _dis1 { get; set; }
}

public class Sense
{
    public List<Example> examples { get; set; }
    public List<List<string>> links { get; set; }
    public List<string> glosses { get; set; }
    public string id { get; set; }
    public List<string> categories { get; set; }
    public List<string> raw_glosses { get; set; }
    public string qualifier { get; set; }
    public List<string> tags { get; set; }
}

public class Example
{
    public string text { get; set; }
    public string english { get; set; }
    public string type { get; set; }
}