using System.Collections.Generic;

namespace CommonLibTools.Libs.CrossWord
{
    public class CrossWordGame
    {
        public Lang Language { get; set; }
        public string LangString { get; set; }

        public int ChapterCount { get; set; }
        public int LevelCount { get; set; }
        public List<CrossWordChapter> Chapters { get; set; }

        public CrossWordGame()
        {
            
        }
        public CrossWordGame(Lang lang, List<GenGrid> grids)
        {
            Language = lang;
            LangString = lang.ToString();
            Chapters = new List<CrossWordChapter>();
            var chapterCount = 1;
            var currentChapter = new CrossWordChapter(chapterCount);

            var levelPerChapter = GetlevelPerChapter(chapterCount);
            var currentLevel = 1;
            foreach (var genGrid in grids)
            {
                var level = new CrossWordLevel(genGrid, currentLevel);
                currentChapter.AddLevel(level);
                currentLevel += 1;
                if (currentLevel > levelPerChapter)
                {
                    Chapters.Add(currentChapter);
                    chapterCount++;
                    currentChapter = new CrossWordChapter(chapterCount);
                    levelPerChapter = GetlevelPerChapter(chapterCount);
                    currentLevel = 1;
                }
            }

            //add the last chapter 
            if (currentChapter.AllLevel.Count > 0)
            {
                Chapters.Add(currentChapter);
            }

            ChapterCount = Chapters.Count;
            LevelCount = grids.Count;
        }

        private int GetlevelPerChapter(int chapterCount)
        {
            if (chapterCount <= 2)
            {
                return 4;
            }

            if (chapterCount <= 8)
            {
                return 8;
            }

            if (chapterCount <= 64)
            {
                return 16;
            }



            return 32;
        }
    }
}