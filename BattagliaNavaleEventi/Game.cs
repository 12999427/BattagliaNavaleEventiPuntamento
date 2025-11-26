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

        Random random = new Random(Environment.TickCount);
        Button[][,] ButtonsPlayer = new Button[2][,] { new Button[10, 10], new Button[10, 10] };
        int[][,] GridPlayer = new int[2][,] { new int[10, 10], new int[10, 10] };
        Ship[][,] ShipsPlayer = new Ship[2][,] { new Ship[10, 10], new Ship[10, 10] };

        Dictionary<int, int> shipCountsTemplate = new Dictionary<int, int> { { 4, 1 }, { 3, 2 }, { 2, 2 }, { 1, 1 } };
        Dictionary<int, int>[] shipCountsPlayer = new Dictionary<int, int>[2];

        int activePlayer = 0;
        bool multiplayerMode = false;
        int selectedShipSize = 0;
        List<Point> currentPlacement = new List<Point>();
        bool placementPhaseCompleteForPlayer1 = false;

        bool playing = false;
        int shipsSunk1 = 0;
        int numAttemps1 = 0;
        int shipsSunk2 = 0;
        int numAttemps2 = 0;
        int CurrentPlayer = 0;

        event EventHandler<ColpitoEventArgs> EventoAttacco = null;

        public Game(bool multiplayer)
        {
            InitializeComponent();
            multiplayerMode = multiplayer;

            GenerateGrid(tbl_grid, 0);
            if (multiplayer)
                GenerateGrid(tbl_grid2, 1);
            else
                tbl_grid2.Visible = false;

            ResetPlacement();

            this.Size = multiplayer ? new Size(1597, 681) : new Size(970, 681);

            btn_PosNave4.Click += (s, e) => BeginPlacement(4);
            btn_PosNave3.Click += (s, e) => BeginPlacement(3);
            btn_PosNave2.Click += (s, e) => BeginPlacement(2);
            btn_PosNave1.Click += (s, e) => BeginPlacement(1);
            btn_ResetPlacement.Click += (s, e) => ResetPlacement();

            UpdateShipButtonsState();

            EventoAttacco += FunzioneLogAttacco;
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

        private void BeginPlacement(int size)
        {
            selectedShipSize = size;
            currentPlacement.Clear();
            SetShipButtonsEnabled(false);

            SetGridEnabledPlacement(activePlayer, true);
            if (multiplayerMode)
                SetGridEnabledPlacement(1 - activePlayer, false);

            EnableFreeCells(activePlayer);
        }

        private void SetGridEnabledPlacement(int playerIndex, bool enabled)
        {
            var btns = ButtonsPlayer[playerIndex];
            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                    btns[y, x].Enabled = enabled && GridPlayer[playerIndex][y, x] == 0;
        }

        private void EnableFreeCells(int playerIndex)
        {
            var btns = ButtonsPlayer[playerIndex];
            for (int y = 0; y < 10; y++)
                for (int x = 0; x < 10; x++)
                {
                    btns[y, x].Enabled = GridPlayer[playerIndex][y, x] == 0;
                }
        }

        private void ClickBtnCella(object? sender, EventArgs e)
        {
            Button btn_sender = (Button)sender;
            var tag = (Tuple<int, Point>)btn_sender.Tag;
            int playerIdx = tag.Item1;
            Point p = tag.Item2;
            int x = p.X; int y = p.Y;

            if (!playing)
            {
                if (selectedShipSize > 0 && playerIdx == activePlayer)
                {
                    HandlePlacementClick(playerIdx, x, y);
                    return;
                }
            }
            else
            {
                HandleAttackClick(playerIdx, x, y);
            }

        }

        private void HandleAttackClick(int playerIdx, int x, int y)
        {
            if (CurrentPlayer == 0) numAttemps1++; else numAttemps2++;
            updateAtteps();

            bool colpito = GridPlayer[playerIdx][y, x] >= 1; //nave

            EventoAttacco?.Invoke(this, new ColpitoEventArgs(colpito, new Point(x, y)));    


            if (colpito) //nave
            {

                GridPlayer[playerIdx][y, x] = -2;
                Ship curShip = ShipsPlayer[playerIdx][y, x];
                if (curShip.removeTile())
                {
                    ShowColors(playerIdx);
                    updateShipsSunk(playerIdx == 0 ? ++shipsSunk1 : ++shipsSunk2);
                }
            }
            else if (GridPlayer[playerIdx][y, x] == 0)
            {
                GridPlayer[playerIdx][y, x] = -1;
            }

            ShowColors(playerIdx);

            if (multiplayerMode)
            {
                CurrentPlayer = 1 - CurrentPlayer;

                SetGridEnabledPlaying(CurrentPlayer, true);
                SetGridEnabledPlaying(1 - CurrentPlayer, false);
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

        private void ShowColors(int playerIdx)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Button b = ButtonsPlayer[playerIdx][y, x];

                    // colore base
                    b.BackColor = GetColor(GridPlayer[playerIdx][y, x], ShipsPlayer[playerIdx][y, x]?.sunk);
                }
            }
        }

        private void SetGridEnabledPlaying(int playerIdx, bool enabled)
        {
            foreach (Button b in ButtonsPlayer[playerIdx])
            {
                b.Enabled = enabled;
            }
        }

        private void ShowVictory()
        {
            if (multiplayerMode)
            {
                MessageBox.Show("Ha vinto giocatore " + CurrentPlayer);
            }
            else
            {
                MessageBox.Show("Hai vinto");
            }
            Close();
        }

        private void updateShipsSunk(int n)
        {
            const int totNumShip = 6;
            lbl_shipsSunk.Text = "Navi affondate: " + n;
            if (n == totNumShip)
            {
                ShowVictory();
            }
        }


        private void updateAtteps()
        {
            lbl_numAttemps.Text = $"Giocatore: {CurrentPlayer}\nNumero tentativi: {(CurrentPlayer == 0 ? numAttemps1 : numAttemps2)}";
        }


        private void HandlePlacementClick(int playerIdx, int x, int y)
        {

            if (GridPlayer[playerIdx][y, x] != 0) return;

            if (currentPlacement.Count == 0)
            {
                currentPlacement.Add(new Point(x, y));
                ButtonsPlayer[playerIdx][y, x].BackColor = Color.Orange;

                RestrictNextCellsToAdjacent(playerIdx, x, y);

                if (selectedShipSize == 1)
                    FinalizePlacement(playerIdx);
                return;
            }

            if (!ButtonsPlayer[playerIdx][y, x].Enabled) return;

            currentPlacement.Add(new Point(x, y));
            ButtonsPlayer[playerIdx][y, x].BackColor = Color.Orange;

            if (currentPlacement.Count == 2 && selectedShipSize > 1)
            {
                RestrictLineOptions(playerIdx);
                if (selectedShipSize == 2)
                {
                    FinalizePlacement(playerIdx);
                    return;
                }
                return;
            }

            if (currentPlacement.Count >= selectedShipSize)
            {

                FinalizePlacement(playerIdx);
            }
            else
            {

                if (currentPlacement.Count >= 2)
                    RestrictLineOptions(playerIdx);
            }
        }

        private void RestrictNextCellsToAdjacent(int playerIdx, int x, int y)
        {

            var btns = ButtonsPlayer[playerIdx];
            for (int yy = 0; yy < 10; yy++)
                for (int xx = 0; xx < 10; xx++)
                {
                    if (GridPlayer[playerIdx][yy, xx] != 0) { btns[yy, xx].Enabled = false; continue; }
                    bool isNeighbor = (Math.Abs(xx - x) + Math.Abs(yy - y) == 1);
                    btns[yy, xx].Enabled = isNeighbor;
                }
        }

        private void RestrictLineOptions(int playerIdx)
        {
            var btns = ButtonsPlayer[playerIdx];

            foreach (var b in btns)
                b.Enabled = false;

            bool vertical = IsVertical(currentPlacement);

            if (vertical)
            {
                OrderPointsYX(currentPlacement);
                //currentPlacement.Sort((a, b) => a.Y.CompareTo(b.Y));

                int col = currentPlacement[0].X;
                int minY = currentPlacement[0].Y;
                int maxY = currentPlacement[currentPlacement.Count - 1].Y;

                //Celle adiacenti - solo le immediatamente adiacenti
                int up = minY - 1;
                int down = maxY + 1;

                if (up >= 0 && GridPlayer[playerIdx][up, col] == 0)
                    btns[up, col].Enabled = true;

                if (down < 10 && GridPlayer[playerIdx][down, col] == 0)
                    btns[down, col].Enabled = true;
            }
            else
            {
                OrderPointsYX(currentPlacement);
                //currentPlacement.Sort((a, b) => a.X.CompareTo(b.X));

                int row = currentPlacement[0].Y;
                int minX = currentPlacement[0].X;
                int maxX = currentPlacement[currentPlacement.Count - 1].X;

                //solo le celle adiacenti  immediate
                int left = minX - 1;
                int right = maxX + 1;

                if (left >= 0 && GridPlayer[playerIdx][row, left] == 0)
                    btns[row, left].Enabled = true;

                if (right < 10 && GridPlayer[playerIdx][row, right] == 0)
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
            return points.ToList();
        }

        /*private List<Point> OrderPointsPrioritaX(List<Point> points)
        {
            points.Sort((a, b) =>
            {
                if (a.X != b.X)
                    return a.X - b.X; 
                return a.Y - b.Y;             
            });
            return points.ToList();
        }*/

        private Point GetOrigin(List<Point> pts, bool vertical)
        {
            Point best = pts[0];

            for (int i = 1; i < pts.Count; i++)
            {
                if (vertical)
                {
                    // prendi quello col valore Y più piccolo
                    if (pts[i].Y < best.Y)
                        best = pts[i];
                }
                else
                {
                    //  quello col valore X più piccolo
                    if (pts[i].X < best.X)
                        best = pts[i];
                }
            }

            return best;
        }


        private void FinalizePlacement(int playerIdx)
        {

            var pts = OrderPointsYX(currentPlacement);
            bool vertical = IsVertical(pts);
            Point origin = GetOrigin(pts, vertical);
            Ship s = new Ship(origin.X, origin.Y, selectedShipSize, vertical);
            foreach (var pt in pts)
            {
                GridPlayer[playerIdx][pt.Y, pt.X] = selectedShipSize;
            }

            foreach (var pt in pts) ShipsPlayer[playerIdx][pt.Y, pt.X] = s;

            foreach (var pt in pts)
            {
                var b = ButtonsPlayer[playerIdx][pt.Y, pt.X];
                b.BackColor = Color.Gray;
                b.Enabled = false;
            }

            shipCountsPlayer[playerIdx][selectedShipSize]--;
            UpdateShipButtonsState();

            currentPlacement.Clear();
            selectedShipSize = 0;

            bool allPlaced = shipCountsPlayer[playerIdx].Values.All(v => v == 0);

            if (allPlaced)
            {
                if (multiplayerMode && playerIdx == 0)
                {

                    placementPhaseCompleteForPlayer1 = true;
                    MessageBox.Show("Giocatore 1 ha finito il posizionamento. Ora tocca al Giocatore 2.");
                    activePlayer = 1;
                    EnableFreeCells(activePlayer);
                    SetGridEnabledPlacement(0, false);
                    SetGridEnabledPlacement(1, true);
                    SetShipButtonsEnabled(true);
                }
                else
                {

                    MessageBox.Show($"Giocatore {playerIdx + 1} ha finito il posizionamento.");

                    SetGridEnabledPlacement(0, false);
                    if (multiplayerMode)
                        SetGridEnabledPlacement(1, false);
                }
                if (shipCountsPlayer[0].Values.All(v => v == 0) && (!multiplayerMode ? true : shipCountsPlayer[1].Values.All(v => v == 0)))
                { // terminati piazzamenti
                    SetShipButtonsVisible(false);
                    txt_Log.Visible = true;
                    playing = true;
                    SetGridEnabledPlaying(0, true);
                    ShowColors(0);
                    if (multiplayerMode)
                        ShowColors(1);
                }
            }
            else
            {

                EnableFreeCells(playerIdx);
                SetGridEnabledPlaying(playerIdx, true);
            }
        }


        private void ResetPlacement()
        {

            for (int p = 0; p < 2; p++)
            {
                for (int y = 0; y < 10; y++)
                    for (int x = 0; x < 10; x++)
                    {
                        GridPlayer[p][y, x] = 0;
                        ShipsPlayer[p][y, x] = null;
                        var btn = ButtonsPlayer[p][y, x];
                        if (btn != null)
                        {
                            btn.BackColor = Color.LightBlue;
                            btn.Enabled = (p == 0) || multiplayerMode;
                        }
                    }
                shipCountsPlayer[p] = shipCountsTemplate.ToDictionary(kv => kv.Key, kv => kv.Value);
            }
            activePlayer = 0;
            selectedShipSize = 0;
            currentPlacement.Clear();
            placementPhaseCompleteForPlayer1 = false;
            UpdateShipButtonsState();
        }

        private void UpdateShipButtonsState()
        {

            btn_PosNave4.Enabled = shipCountsPlayer[activePlayer][4] > 0;
            btn_PosNave3.Enabled = shipCountsPlayer[activePlayer][3] > 0;
            btn_PosNave2.Enabled = shipCountsPlayer[activePlayer][2] > 0;
            btn_PosNave1.Enabled = shipCountsPlayer[activePlayer][1] > 0;

        }

        private void SetShipButtonsEnabled(bool enabled)
        {
            btn_PosNave4.Enabled = enabled && shipCountsPlayer[activePlayer][4] > 0;
            btn_PosNave3.Enabled = enabled && shipCountsPlayer[activePlayer][3] > 0;
            btn_PosNave2.Enabled = enabled && shipCountsPlayer[activePlayer][2] > 0;
            btn_PosNave1.Enabled = enabled && shipCountsPlayer[activePlayer][1] > 0;

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