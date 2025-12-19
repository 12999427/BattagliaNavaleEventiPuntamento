using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using BattagliaNavale;

namespace BattagliaNavaleEventi
{
    public partial class Game : Form
    {
        Button[][,] ButtonsPlayer = new Button[2][,] { new Button[10, 10], new Button[10, 10] };

        GameManager GM;

        public Game(bool multiplayer, bool bot=false)
        {
            InitializeComponent();

            GM = new(multiplayer, bot);
            GM.ResetPlacementGraphics += ResetPlacement;
            GM.BeginPlacementGraphics += BeginPlacement;
            GM.ResetPlacementGraphics += ResetPlacement;
            GM.ShowVictoryGraphics += ShowVictory;
            GM.UpdateShipsSunkGraphics += updateShipsSunk;
            GM.UpdateAttepsGraphics += updateAtteps;
            GM.UpdateGUI += ShowColors;
            GM.SetGridEnabledGraphics += (s, ev) =>
            {
                if (GM.playing)
                    SetGridEnabledPlaying(ev.playerId, ev.enabled);
                else
                    SetGridEnabledPlacement(ev.playerId, ev.enabled);
            };
            GM.PlacementEndedGraphics += PlacementEnded;
            GM.ShowAlert += (s, ea) => MessageBox.Show(ea);

            GenerateGrid(tbl_grid, 0);
            if (multiplayer)
                GenerateGrid(tbl_grid2, 1);
            else
            {
                tbl_grid2.Visible = false;
                lbl_shipsSunk2.Visible = false;
                lbl_NumAttemps2.Visible = false;
            }

            GM.Start();

            this.Size = multiplayer ? new Size(1597, 681) : new Size(970, 681);

            btn_PosNave4.Click += (s, e) => GM.BeginPlacement(4);
            btn_PosNave3.Click += (s, e) => GM.BeginPlacement(3);
            btn_PosNave2.Click += (s, e) => GM.BeginPlacement(2);
            btn_PosNave1.Click += (s, e) => GM.BeginPlacement(1);
            btn_ResetPlacement.Click += (s, e) => GM.ResetPlacement();

            SetShipButtonsEnabled(true);

            GM.EventoAttacco += FunzioneLogAttacco;
        }

        public void FunzioneLogAttacco(object sender, ColpitoEventArgs ea)
        {
            if (ea.colpito)
            {
                Log($"Colpito in posizione({ea.coord.X}, {ea.coord.Y})");
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"../../../Colpito.wav");
                player.Play();
            }
            else
            {
                Log($"Acqua !!");
                System.Media.SoundPlayer player = new System.Media.SoundPlayer(@"../../../Acqua.wav");
                player.Play();
            }
        }

        private void GenerateGrid(TableLayoutPanel grid, int playerIndex)
        {
            grid.Controls.Clear();
            grid.ColumnCount = 10; grid.RowCount = 10;
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Button btn_cell = new Button();
                    btn_cell.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                    btn_cell.ImageAlign = ContentAlignment.BottomLeft;
                    btn_cell.Name = $"btn_grid_p{playerIndex}_{x}x{y}";
                    btn_cell.Location = new Point(0, 0);
                    btn_cell.BackColor = Color.LightBlue;
                    btn_cell.Click += ClickBtnCella;
                    btn_cell.Tag = new Tuple<int, Point>(playerIndex, new Point(x, y));
                    btn_cell.BackgroundImageLayout = ImageLayout.Stretch;
                    grid.Controls.Add(btn_cell);
                    ButtonsPlayer[playerIndex][y, x] = btn_cell;
                    btn_cell.UseVisualStyleBackColor = true;
                }
            }
        }

        private void BeginPlacement(object sender, EventArgs ea)
        {
            SetShipButtonsEnabled(false);

            SetGridEnabledPlacement(GM.CurrentPlayer, true);
            if (GM.multiplayerMode)
                SetGridEnabledPlacement(1 - GM.CurrentPlayer, false);
        }

        private void SetGridEnabledPlacement(int playerIndex, bool enabled)
        {
            var btns = ButtonsPlayer[playerIndex];
            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                    btns[y, x].Enabled = enabled && GM.GridPlayer[playerIndex][y, x] == 0;
        }

        private void SetGridEnabledPlaying(int playerIdx, bool enabled)
        {
            foreach (Button b in ButtonsPlayer[playerIdx])
            {
                b.Enabled = enabled;
            }
        }

        private void ClickBtnCella(object? sender, EventArgs e)
        {
            Button btn_sender = (Button)sender;
            var tag = (Tuple<int, Point>)btn_sender.Tag;
            int playerIdx = tag.Item1;
            Point p = tag.Item2;
            int x = p.X; int y = p.Y;

            bool success = GM.ClickBtnCella(x, y, playerIdx);

            if (success)
            {
                //successo

                if (!GM.playing)
                {
                    ButtonsPlayer[playerIdx][y, x].BackColor = Color.Orange;

                    if (GM.selectedShipSize != 0)
                    {
                        if (GM.currentPlacement.Count == 1)
                            RestrictNextCellsToAdjacent(playerIdx, x, y);
                        else
                            RestrictLineOptions(playerIdx);
                    }
                }
            }
        }

        private void PlacementEnded(object sender, EventArgs e)
        {
            if (GM.playing)
            {
                SetShipButtonsVisible(false);
                SetGridEnabledPlaying(0, true);
                txt_Log.Visible = true;
            }
            else
            {
                //seconda fase posizionamento
                SetShipButtonsEnabled(true);
                SetShipButtonsVisible(true);
                SetGridEnabledPlacement(0, true);
            }

        }

        private Color GetColor(int v, bool? sunk)
        {
            switch (v)
            {
                case -2: //colpo a segno
                    if (sunk ?? false)
                    {
                        return Color.Black;
                    }
                    else
                    {
                        return Color.Red;
                    }
                case -1: //colpo mancato
                    return Color.White;
                case 0: //acqua inesplorata
                    return Color.Blue;
                default:
                    if (false) //metti true per mostrare navi
                    {
                        return Color.Chocolate;
                    }
                    else
                    {
                        return Color.Blue;
                    }

            }
        }

        private void ShowColors(object sender, int playerIdx)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Button b = ButtonsPlayer[playerIdx][y, x];

                    // colore base
                    b.BackColor = GetColor(GM.GridPlayer[playerIdx][y, x], GM.ShipsPlayer[playerIdx][y, x]?.sunk);
                }
            }
        }

        private void ShowVictory(object sender, EventArgs ea)
        {
            if (GM.multiplayerMode)
            {
                MessageBox.Show("Ha vinto giocatore " + (GM.CurrentPlayer+1));
            }
            else
            {
                MessageBox.Show("Hai vinto");
            }
            Close();
        }

        private void updateShipsSunk(object sender, (int numShips, int numPlayer) tupla)
        {
            (tupla.numPlayer == 0 ? lbl_shipsSunk1 : lbl_shipsSunk2).Text = "Navi affondate: " + tupla.numShips;
        }


        private void updateAtteps(object sender, (int numAttemps, int numPlayer) tupla)
        {
            if (tupla.numPlayer == 0)
                lbl_numAttemps1.Text = $"Giocatore: 1\nNumero tentativi: {tupla.numAttemps}";
            else
                lbl_NumAttemps2.Text = $"Giocatore: 2\nNumero tentativi: {tupla.numAttemps}";
        }




        private void RestrictNextCellsToAdjacent(int playerIdx, int x, int y)
        {
            var btns = ButtonsPlayer[playerIdx];
            for (int yy = 0; yy < 10; yy++)
                for (int xx = 0; xx < 10; xx++)
                {
                    if (GM.GridPlayer[playerIdx][yy, xx] != 0) //se non sono libere (c'è la nave) le disabilita
                    {
                        btns[yy, xx].Enabled = false;
                        continue;
                    }
                    bool vicina = (Math.Abs(xx - x) + Math.Abs(yy - y) == 1);
                    btns[yy, xx].Enabled = vicina;
                }
        }

        private void RestrictLineOptions(int playerIdx)
        {
            var btns = ButtonsPlayer[playerIdx];

            foreach (var b in btns)
                b.Enabled = false;

            bool vertical = IsVertical(GM.currentPlacement);

            if (vertical)
            {
                OrderPointsYX(GM.currentPlacement);

                int col = GM.currentPlacement[0].X;
                int minY = GM.currentPlacement[0].Y;
                int maxY = GM.currentPlacement[GM.currentPlacement.Count - 1].Y;

                //Celle adiacenti - solo le immediatamente adiacenti
                int up = minY - 1;
                int down = maxY + 1;

                if (up >= 0 && GM.GridPlayer[playerIdx][up, col] == 0)
                    btns[up, col].Enabled = true;

                if (down < 10 && GM.GridPlayer[playerIdx][down, col] == 0)
                    btns[down, col].Enabled = true;
            }
            else
            {
                OrderPointsYX(GM.currentPlacement);

                int row = GM.currentPlacement[0].Y;
                int minX = GM.currentPlacement[0].X;
                int maxX = GM.currentPlacement[GM.currentPlacement.Count - 1].X;

                //solo le celle adiacenti  immediate
                int left = minX - 1;
                int right = maxX + 1;

                if (left >= 0 && GM.GridPlayer[playerIdx][row, left] == 0)
                    btns[row, left].Enabled = true;

                if (right < 10 && GM.GridPlayer[playerIdx][row, right] == 0)
                    btns[row, right].Enabled = true;
            }
        }

        private bool IsVertical(List<Point> pts)
        {
            int firstX = pts[0].X;

            for (int i = 1; i < pts.Count; i++)
                if (pts[i].X != firstX)
                    return false;

            return true;
        }
        private List<Point> OrderPointsYX(List<Point> points)
        {
            // ordina prima per Y, poi nel caso essia sia uguale, per X
            points.Sort((a, b) =>
            {
                if (a.Y != b.Y)
                    return a.Y - b.Y;       // confronto Y
                return a.X - b.X;                  // se Y uguale, confronto X
            });
            return points;
        }

        private void ResetPlacement(object sender, EventArgs ea)
        {
            for (int p = 0; p < 2; p++)
            {
                for (int y = 0; y < 10; y++)
                    for (int x = 0; x < 10; x++)
                    {
                        var btn = ButtonsPlayer[p][y, x];
                        if (btn != null)
                        {
                            btn.BackColor = Color.LightBlue;
                            btn.Enabled = (p == 0) || GM.multiplayerMode;
                        }
                    }
            }
            SetShipButtonsEnabled(true);
        }

        private void SetShipButtonsEnabled(bool enabled)
        {
            btn_PosNave4.Enabled = enabled && GM.shipCountsPlayer[GM.CurrentPlayer][4] > 0;
            btn_PosNave3.Enabled = enabled && GM.shipCountsPlayer[GM.CurrentPlayer][3] > 0;
            btn_PosNave2.Enabled = enabled && GM.shipCountsPlayer[GM.CurrentPlayer][2] > 0;
            btn_PosNave1.Enabled = enabled && GM.shipCountsPlayer[GM.CurrentPlayer][1] > 0;

            btn_ResetPlacement.Enabled = true;
        }

        private void SetShipButtonsVisible(bool enabled)
        {
            btn_PosNave4.Visible = enabled;
            btn_PosNave3.Visible = enabled;
            btn_PosNave2.Visible = enabled;
            btn_PosNave1.Visible = enabled;
            btn_ResetPlacement.Visible = enabled;
        }

        private void Log(string msg)
        {
            txt_Log.Text += Environment.NewLine + msg;
        }
    }
}