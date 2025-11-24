using BattagliaNavale;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BattagliaNavaleEventi
{
    public partial class Game : Form
    {
        Random random = new Random(Environment.TickCount);
        Button[,] Buttons = new Button[10, 10];
        int[,] Grid = new int[10, 10];
        int[,] ShipTypes = {
            { 4, 1 },
            { 3, 2 },
            { 2, 2 },
            { 1, 1 },
        };
        Ship[,] Ships = new Ship[10, 10];
        int shipsSunk = 0;
        int numAttemps = 0;
        bool canPlay = false;

        public Game(bool multiplayer)
        {
            InitializeComponent();
            GenerateGrid(tbl_grid);

            if (multiplayer)
                GenerateGrid(tbl_grid2);

            this.Size = multiplayer ? new Size(1597, 681) : new Size(970, 681);

            lbl_bestAttemps.Visible = false;
            lbl_numAttemps.Visible = false;
            lbl_shipsSunk.Visible = false;
        }

        private void GenerateGrid(TableLayoutPanel grid)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Button btn_cell = new Button();
                    btn_cell.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                    btn_cell.ImageAlign = ContentAlignment.BottomLeft;
                    btn_cell.Name = $"btn_grid_{x}x{y}";
                    btn_cell.Location = new Point(0, 0);
                    btn_cell.BackColor = Color.LightBlue;
                    //btn_cell.Tag = new Point(y, x);
                    btn_cell.Click += ClickBtnCella;
                    btn_cell.Tag = $"{x} {y}";
                    btn_cell.BackgroundImageLayout = ImageLayout.Stretch;

                    grid.Controls.Add(btn_cell);
                    Buttons[y, x] = btn_cell;

                    btn_cell.UseVisualStyleBackColor = true;
                }
            }
        }


        private bool isSpaceUsed(bool vertical, int x, int y, int size)
        {
            for (int i = 0; i < size; i++)
            {
                if (Grid[(!vertical ? y : y + i), (vertical ? x : x + i)] != 0)
                {
                    return true;
                }
            }
            return false;
        }

        private void ShowColors()
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Button b = Buttons[y, x];
                    b.BackColor = /* Grid[y, x] == -2 ? && Ships[y, x].sunk ? Color.Black :*/ getColor(Grid[y, x]);
                    if (Grid[y, x] == -2 && Ships[y, x].sunk)
                    {
                        b.BackgroundImage = Properties.Resources.tile;
                    }
                    else
                    {
                        b.BackgroundImage = null;
                    }
                }
            }
        }

        private Color getColor(int v)
        {
            switch (v)
            {
                case -2: //colpo a segno
                    return Color.Red;
                case -1: //colpo mancato
                    return Color.White;
                case 0: //acqua inesplorata
                    return Color.Blue;
                default:
                    return (false ? Color.Chocolate : Color.Blue); //false per nascondere

            }
        }

        private void ClickBtnCella(object? sender, EventArgs e)
        {
            Button btn_sender = (Button)sender;
            string[] splitTag = ((string)(btn_sender).Tag).Split(" ");
            int x = int.Parse(splitTag[0]);
            int y = int.Parse(splitTag[1]);

            updateAtteps(++numAttemps);

            if (Grid[y, x] >= 1) //nave
            {
                Grid[y, x] = -2;
                Ship curShip = Ships[y, x];
                if (curShip.removeTile())
                {
                    ShowColors();
                    updateShipsSunk(++shipsSunk);
                }
            }
            else if (Grid[y, x] == 0)
            {
                Grid[y, x] = -1;
            }
            ShowColors();
            //MessageBox.Show(Grid[y, x] == -2 ? $"xy {Ships[y, x].x} {Ships[y, x].y} v {Ships[y, x].vertical} size {Ships[y, x].size} sunk {Ships[y, x].sunk}" : "#");
        }

        private void ShowVictory()
        {
            //qualcosa
            Close();
        }

        private void updateShipsSunk(int n)
        {
            const int totNumShip = 10;
            lbl_shipsSunk.Text = "Navi affondate: " + n;
            if (n == totNumShip)
            {
                ShowVictory();
            }
        }

        private void updateAtteps(int n)
        {
            lbl_numAttemps.Text = "Numero tentativi: " + n;
        }

        private void btn_PosNave4_Click(object sender, EventArgs e)
        {

        }

        private void btn_PosNave3_Click(object sender, EventArgs e)
        {

        }

        private void btn_PosNave2_Click(object sender, EventArgs e)
        {

        }

        private void btn_PosNave1_Click(object sender, EventArgs e)
        {

        }
    }
}

