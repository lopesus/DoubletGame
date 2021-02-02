using System.Collections.Generic;
using CommonLibTools;
using PathFindingModel;

namespace WiktionaireParser.Models.CrossWord
{
    public class CrossWord
    {
        public Coord StartCoord;
        public Coord EndCoord;
        public string Word { get; set; }
        public CrossWordDirection Direction { get; set; }
        public List<CrossWordLetter> WordLetterList { get; set; }

        public CrossWordCell BeforeStartCell;
        public CrossWordCell AfterEndCell;



        public CrossWord(string word, Coord coord, CrossWordDirection direction)
        {
            this.Word = word;
            Direction = direction;
            StartCoord = coord;
            WordLetterList = new List<CrossWordLetter>();
            switch (direction)
            {
                case CrossWordDirection.Horizontal:
                    EndCoord = new Coord(coord.Row, coord.Col + word.Length - 1);
                    break;
                case CrossWordDirection.Vertical:
                    EndCoord = new Coord(coord.Row + word.Length - 1, coord.Col);
                    break;
            }

            //split the word into cell
            switch (direction)
            {
                case CrossWordDirection.Horizontal:
                    for (var index = 0; index < word.Length; index++)
                    {
                        var car = word[index];
                        var cellCoord = new Coord(coord.Row, coord.Col + index);

                        CrossWordLetter wordCell = new CrossWordLetter(car, cellCoord, direction, this);
                        WordLetterList.Add(wordCell);
                    }

                    BeforeStartCell =  new CrossWordCell(StartCoord.GetLeftCoord());
                    BeforeStartCell.ExcludedFromMaze = true;
                    BeforeStartCell.IsEmpty = true;

                    AfterEndCell = new CrossWordCell(EndCoord.GetRightCoord());
                    AfterEndCell.ExcludedFromMaze = true;
                    AfterEndCell.IsEmpty = true;


                    break;
                case CrossWordDirection.Vertical:
                    for (var index = 0; index < word.Length; index++)
                    {
                        var car = word[index];
                        var cellCoord = new Coord(coord.Row + index, coord.Col);

                        CrossWordLetter wordCell = new CrossWordLetter(car, cellCoord, direction, this);
                        WordLetterList.Add(wordCell);
                    }

                    BeforeStartCell = new CrossWordCell(StartCoord.GetUpCoord());
                    BeforeStartCell.ExcludedFromMaze = true;

                    AfterEndCell = new CrossWordCell(EndCoord.GetDownCoord());
                    AfterEndCell.ExcludedFromMaze = true;
                    break;
            }
        }
    }
}