using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CommonLibTools;
using PathFindingModel;

namespace WiktionaireParser.Models.CrossWord
{
    public class CrossWordGrid
    {
        public int NumCol { get; set; }
        public int NumRow { get; set; }
        public CrossWordCell[,] MazeCellList { get; set; }

        public bool IsEmpty { get; set; }
        public CrossWordGrid(int numRow, int numCol)
        {
            NumCol = numCol;
            NumRow = numRow;
            MazeCellList = new CrossWordCell[NumRow, NumCol];
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

        public void PutWordAt(CrossWordWord word, Coord coord, CrossWordDirection direction)
        {
            foreach (var crossWordCell in word.WordCellsList)
            {
                var cellCoord = crossWordCell.Coord;
                var cell = GetCell(cellCoord);//MazeCellList[cellCoord.Row, cellCoord.Col];
                cell?.CopyFrom(crossWordCell);
            }

            var wordCell = GetCell(word.BeforeStartCell.Coord);
            wordCell?.CopyFrom(word.BeforeStartCell);

            wordCell = GetCell(word.AfterEndCell.Coord);
            wordCell?.CopyFrom(word.AfterEndCell);
        }

        bool CanFitOnGrid(string word, Coord coord, CrossWordDirection direction)
        {
            var corssWord = new CrossWordWord(word, coord, direction);
            return false;
        }

        void CanFitHorizontally(string word, Coord coord)
        {

        }

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
                return MazeCellList[coord.Col, coord.Row];
            }

            return null;
        }

        public CrossWordCell GetCell(int x, int y)
        {
            if (IsValidCell(x, y))
            {
                return MazeCellList[x, y];
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
