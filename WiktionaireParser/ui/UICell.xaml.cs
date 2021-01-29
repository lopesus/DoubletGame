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
using PathFindingModel;
using WiktionaireParser.Models.CrossWord;

namespace WiktionaireParser.ui
{
    /// <summary>
    /// Interaction logic for UICell.xaml
    /// </summary>
    public partial class UICell : UserControl
    {
        public static int CellSize = 100;

        public CrossWordCell WordCell { get; set; }
       // public Coord Coord { get; set; }
        public string Letter { get; set; }

        public UICell()
        {
            InitializeComponent();
            txtGCost.Text = "";
            txtFcost.Text = "";
            txtHCost.Text = "";
            tblLetter.Text = "";
        }



        public UICell(CrossWordCell cell)
        {
            InitializeComponent();
            Letter = "";
            WordCell = cell;
            txtGCost.Text = "";
            txtFcost.Text = "";
            txtHCost.Text = "";
            tblLetter.Text = "";

            Canvas.SetLeft(this, WordCell.Coord.Col * CellSize);
            Canvas.SetTop(this, WordCell.Coord.Row * CellSize);

            txtCoord.Text = WordCell.Coord.ToString();
            Width = Height = CellSize;

            Init();
        }


        public void SetWord(CrossWordCell wordCell)
        {
            Letter = wordCell.Letter;
            this.WordCell.CopyFrom(wordCell);
            
            UpdateData();
        }

        private void UpdateData()
        {
            tblLetter.Text = Letter.ToUpper();
            if (WordCell.IsEmpty)
            {
                SetBrush(UiBrushes.Empty);
                border.BorderBrush = UiBrushes.Empty;
            }
            else
            {
                SetBrush(UiBrushes.Filled);
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
                SetBrush(UiBrushes.Filled);
            }
        }

        public void SetBrush(SolidColorBrush brush)
        {
            border.Background = brush;
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

    }
}
