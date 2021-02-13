using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommonLibTools.Libs;
using CommonLibTools.Libs.CrossWord;

namespace WiktionaireParser.ui
{
    /// <summary>
    /// Interaction logic for UICell.xaml
    /// </summary>
    public partial class UICell : UserControl
    {
        public static int CellSize = 100;
        public bool IsAsGridStartingCell { get; set; }

        public CrossWordCell WordCell { get; set; }
        // public Coord Coord { get; set; }
        public string Letter { get; set; }

        public UICell()
        {
            InitializeComponent();
            txtBehind.Text = "";
            txtInFront.Text = "";
            txtHCost.Text = "";
            tblLetter.Text = "";
        }



        public UICell(CrossWordCell cell)
        {
            InitializeComponent();
            Letter = "";
            WordCell = cell;
            txtBehind.Text = "";
            txtInFront.Text = "";
            txtHCost.Text = "";
            tblLetter.Text = "";

            Canvas.SetLeft(this, WordCell.Coord.Col * CellSize);
            Canvas.SetTop(this, WordCell.Coord.Row * CellSize);

            txtCoord.Text = WordCell.Coord.ToString();
            txtOrthoCoord.Text = WordCell.OrthoCoord.ToString();
            Width = Height = CellSize;

            Init();
        }


        public void SetWord(CrossWordLetter wordCell)
        {
            Letter = wordCell.Letter;

            //this.WordCell.SetLetter(wordCell);

            UpdateData();
        }
        public void DrawLetter(CrossWordLetter letter, bool firstLetter)
        {
            Letter = letter.Letter;
            WordCell.IsEmpty = false;
            IsAsGridStartingCell = firstLetter == true || IsAsGridStartingCell;
            //this.WordCell.SetLetter(wordCell);

            UpdateData();
        }

        public void UpdateData()
        {
            tblLetter.Text = Letter.ToUpper();
            txtBehind.Text = WordCell.SpaceBefore.ToString();
            txtInFront.Text = WordCell.SpaceAfter.ToString();

            if (WordCell.IsEmpty)
            {
                SetBrush(UiBrushes.Empty);
                border.BorderBrush = UiBrushes.Empty;
            }
            else
            {
                SetBrush(UiBrushes.White);
                SetTextColor(UiBrushes.Black);
            }

            if (WordCell.ExcludedFromMaze)
            {
                SetBrush(UiBrushes.Black);
                border.BorderBrush = UiBrushes.Black;
                SetTextColor(UiBrushes.White);
            }

            if (IsAsGridStartingCell)
            {
                border.Background = UiBrushes.StartBrush;
            }
        }

        public void Init()
        {
            if (WordCell.IsEmpty)
            {
                SetBrush(UiBrushes.Empty);
                border.BorderBrush = UiBrushes.Empty;
            }
            else
            {
                SetBrush(UiBrushes.White);
            }
        }

        public void SetBrush(SolidColorBrush brush)
        {
            border.Background = brush;
        }

        public void SetTextColor(SolidColorBrush brush)
        {
            txtCoord.Foreground = brush;
        }


        public void SetData(Coord cell)
        {
            //txtFcost.Text = cell.FCost.ToString();
            //txtGCost.Text = cell.GCost.ToString();
            //txtHCost.Text = cell.HCost.ToString();
        }

        private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Messenger.Default.Send<UiMessage>(new UiMessage()
            //{
            //    Cell = this,
            //    ClickType = ClickType.Left,
            //});
        }

        private void UserControl_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //Messenger.Default.Send<UiMessage>(new UiMessage()
            //{
            //    Cell = this,
            //    ClickType = ClickType.Right,
            //});
        }

        public void SetAsGrigStartingCell()
        {
            IsAsGridStartingCell = true;
        }

    }
}
