public class WordInfo
{
    public string Word { get; set; }
    public string Pronunciation { get; set; }
    public string GenderAndNumber { get; set; }
    public string Variability { get; set; }
    public List<string> Definitions { get; set; } = new List<string>();
}