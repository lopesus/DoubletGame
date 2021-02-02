using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using CommonLibTools;
using CommonLibTools.Extensions;
using PathFindingModel;

namespace WiktionaireParser.Models.CrossWord
{
    public class CrossWordGrid
    {
        public int NumCol { get; set; }
        public int NumRow { get; set; }
        public CrossWordCell[,] MazeCellList { get; set; }

        public List<CrossWordCell> AnchorCellsList;

        public bool IsEmpty { get; set; }
        public CrossWordGrid(int numRow, int numCol)
        {
            NumCol = numCol;
            NumRow = numRow;
            MazeCellList = new CrossWordCell[NumRow, NumCol];
            AnchorCellsList = new List<CrossWordCell>();
            IsEmpty = true;

            for (int col = 0; col < NumCol; col++)
            {
                for (int row = 0; row < NumRow; row++)
                {
                    var coord = new Coord(row, col);
                    CrossWordCell cell = new CrossWordCell(coord);
                    MazeCellList[row, col] = cell;
                }
            }
        }

        public List<StartingPosition> GetStartingCoordFor(string word)
        {
            var horiz = GetStartingCoordFor(word, CrossWordDirection.Horizontal);
            var vert = GetStartingCoordFor(word, CrossWordDirection.Vertical);
            var result = new List<StartingPosition>();
            if (horiz?.Count > 0) result.AddRange(horiz);
            if (vert?.Count > 0) result.AddRange(vert);
            return result;
        }

        public List<StartingPosition> GetStartingCoordFor(string word, CrossWordDirection direction)
        {
            if (word.IsNullOrEmptyString()) return null;

            var result = new List<StartingPosition>();
            var len = word.Length;
            for (int col = 0; col < NumCol; col++)
            {
                for (int row = 0; row < NumRow; row++)
                {
                    var cell = MazeCellList[row, col];
                    int space = GetDistanceToGridEdge(cell.Coord, direction);
                    if (space >= len)
                    {
                        result.Add(new StartingPosition(cell.Coord, direction));
                    }
                }
            }

            return result;
        }

        public void PutWordAt(CrossWord word, Coord coord, CrossWordDirection direction)
        {
            if (IsEmpty)
            {
                IsEmpty = false;
            }
            foreach (var letter in word.WordLetterList)
            {
                var cellCoord = letter.Coord;
                var cell = GetCell(cellCoord);//MazeCellList[cellCoord.Row, cellCoord.Col];
                if (cell != null)
                {
                    cell.SetLetter(letter);

                    //var (behind, inFront) = GetSpaceAroundAnchorCell(cell);
                    //cell.SpaceBefore = behind;
                    //cell.SpaceAfter = inFront;


                    AnchorCellsList.Add(cell);
                }
            }

            //var wordCell = GetCell(word.BeforeStartCell.Coord);
            //wordCell?.SetLetter(word.BeforeStartCell);

            //wordCell = GetCell(word.AfterEndCell.Coord);
            //wordCell?.SetLetter(word.AfterEndCell);

            RefreshAnchorList();
        }

        private void RefreshAnchorList()
        {
            var toRemove=new List<CrossWordCell>();
            foreach (var anchor in AnchorCellsList)
            {
                var (before, after) = GetSpaceAroundAnchorCell(anchor);
                anchor.SpaceBefore = before;
                anchor.SpaceAfter = after;
                if (before==0 && after==0)
                {
                   toRemove.Add(anchor);
                }
            }
        }

        public CrossingIndex SelectRandomAnchor(string word, CrossWordDirection direction)
        {
            var crossIndexList = new List<CrossingIndex>();
            foreach (var anchor in AnchorCellsList)
            {
                var list = GetCrossingIndexListForAnchorCell(word, direction, anchor);
                crossIndexList.AddRange(list);
            }

            var selected = crossIndexList.PickRandom();
            return selected;
        }
        public CrossingIndex SelectRandomAnchor(string word)
        {
            var crossIndexList = new List<CrossingIndex>();
            foreach (var anchor in AnchorCellsList)
            {
                var list = GetCrossingIndexListForAnchorCell(word, CrossWordDirection.Horizontal, anchor);
                crossIndexList.AddRange(list);
                list = GetCrossingIndexListForAnchorCell(word, CrossWordDirection.Vertical, anchor);
                crossIndexList.AddRange(list);
            }

            var selected = crossIndexList.PickRandom();
            return selected;
        }


        List<CrossingIndex> GetCrossingIndexListForAnchorCell(string word, CrossWordDirection direction, CrossWordCell anchorCell)
        {
            var crossIndexList = new List<CrossingIndex>();

            if (word == null || anchorCell == null) return crossIndexList;
            //can only cross a word with different direction
            if (anchorCell.Direction == direction) return crossIndexList;

            var indexList = word.GetAllIndexOf(anchorCell.Letter);
            if (indexList.Count > 0)
            {
                foreach (var index in indexList)
                {
                    var (before, after) = word.GetSpaceAroundIndexPosition(index);
                    if (before <= anchorCell.SpaceBefore && after <= anchorCell.SpaceAfter)
                    {
                        crossIndexList.Add(new CrossingIndex(index, anchorCell, direction, before, after));
                    }
                }
            }

            return crossIndexList;
        }

        bool CanFitOnGrid(string word, Coord coord, CrossWordDirection direction)
        {
            var corssWord = new CrossWord(word, coord, direction);
            return false;
        }

        void CanFitHorizontally(string word, Coord coord)
        {

        }

       
        (int before, int after) GetSpaceAroundAnchorCell(CrossWordCell cell)
        {
            if (cell == null) return (0, 0);

            if (cell.ParentWord?.Count>=2)
            {
                return (0, 0);
            }

            int spaceAfter = 0;
            int spaceBefore = 0;

            int row = cell.Coord.Row;
            int col = cell.Coord.Col;


            //space after
            var direction = cell.Direction == CrossWordDirection.Horizontal ? CrossWordDirection.Vertical : CrossWordDirection.Horizontal;

            var nextCoord = cell.Coord.GetNextCoord(direction);
            var next = GetCell(nextCoord);
            var valid = true;
            while (next != null && valid == true)
            {

                var voisin = GetVoisins(next).Where(c => c.Coord.Equals(cell.Coord) == false);
                valid = voisin.All(v => v.IsEmpty == true);
                if (valid)
                {
                    spaceAfter += 1;
                    nextCoord = nextCoord.GetNextCoord(direction);
                    next = GetCell(nextCoord);
                }


            }


            //space before 
            nextCoord = cell.Coord.GetPreviousCoord(direction);
            next = GetCell(nextCoord);
            valid = true;
            while (next != null && valid == true)
            {
                var voisin = GetVoisins(next).Where(c => c.Coord.Equals(cell.Coord) == false);
                valid = voisin.All(v => v.IsEmpty == true);
                if (valid)
                {
                    spaceBefore += 1;
                    nextCoord = nextCoord.GetPreviousCoord(direction);
                    next = GetCell(nextCoord);
                }

            }
            return (spaceBefore, spaceAfter);
        }

        /// <summary>
        /// do not take into account excluded cell
        /// </summary>
        /// <param name="coord"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        int GetDistanceToGridEdge(Coord coord, CrossWordDirection direction)
        {
            switch (direction)
            {
                case CrossWordDirection.Horizontal:
                    return NumCol - coord.Col;
                case CrossWordDirection.Vertical:
                    return NumRow - coord.Row;
                default:
                    return 0;
            }
        }




        public bool IsValidCell(int x, int y)
        {
            if (x >= 0 && x < NumCol && y >= 0 && y < NumRow)
            {
                try
                {
                    var mazeCell = MazeCellList[x, y];
                    return mazeCell.ExcludedFromMaze == false;
                }
                catch (System.Exception exception)
                {
                    Debug.WriteLine(exception.Message);
                    throw;
                }
            }
            return false;
        }

        public bool IsValidCell(Coord coord)
        {
            if (coord == null) return false;

            var x = coord.Col;
            var y = coord.Row;
            if (x >= 0 && x < NumCol && y >= 0 && y < NumRow)
            {
                //Debug.Log("at " + x + " - " + y);
                if (MazeCellList != null)
                {
                    return MazeCellList[x, y].ExcludedFromMaze == false;
                }
                else
                {
                    Debug.WriteLine("MazeCellList is NULL");
                }
            }

            return false;
        }

        public CrossWordCell GetCell(Coord coord)
        {
            if (IsValidCell(coord))
            {
                return MazeCellList[coord.Row, coord.Col];
            }

            return null;
        }

        public CrossWordCell GetCell(int row, int col)
        {
            if (IsValidCell(row, col))
            {
                return MazeCellList[row, col];
            }

            return null;
        }

        public List<CrossWordCell> GetVoisins(CrossWordCell cell)
        {
            List<CrossWordCell> list = new List<CrossWordCell>();

            if (cell == null) return list;


            var coord = cell.Coord;

            //top
            var voisinCoord = coord.GetUpCoord();
            if (IsValidCell(voisinCoord))
            {
                var mazeCell = GetCell(voisinCoord);
                if (mazeCell != null) list.Add(mazeCell);
            }


            //rigth
            voisinCoord = coord.GetRightCoord();
            if (IsValidCell(voisinCoord))
            {
                var mazeCell = GetCell(voisinCoord);
                if (mazeCell != null) list.Add(mazeCell);
            }

            //bottom
            voisinCoord = coord.GetDownCoord();
            if (IsValidCell(voisinCoord))
            {
                var mazeCell = GetCell(voisinCoord);
                if (mazeCell != null) list.Add(mazeCell);
            }


            //left
            voisinCoord = coord.GetLeftCoord();
            if (IsValidCell(voisinCoord))
            {
                var mazeCell = GetCell(voisinCoord);
                if (mazeCell != null) list.Add(mazeCell);
            }


            return list;
        }

        public CrossWordCell GetVoisin(CrossWordCell cell, Voisin voisin)
        {
            //List<MazeCell> list = new List<MazeCell>();

            if (cell == null) return null;

            var x = cell.Coord.Col;
            var y = cell.Coord.Row;

            var coord = cell.Coord;

            switch (voisin)
            {
                case Voisin.Left:
                    return GetCell(coord.GetLeftCoord());

                case Voisin.Top:
                    return GetCell(coord.GetUpCoord());

                case Voisin.Rigth:
                    return GetCell(coord.GetRightCoord());

                case Voisin.Bottom:
                    return GetCell(coord.GetDownCoord());

                case Voisin.Diag1:
                    return GetCell(coord.GetDiag1Coord());

                case Voisin.Diag2:
                    return GetCell(coord.GetDiag2Coord());

                case Voisin.Diag3:
                    return GetCell(coord.GetDiag3Coord());

                case Voisin.Diag4:
                    return GetCell(coord.GetDiag4Coord());

                default:
                    return null;
            }

            return null;
        }

    }
}
