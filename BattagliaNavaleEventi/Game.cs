using System;
using System.Collections.Generic;
using System.Drawing;
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
        bool placementInProgress => selectedShipSize > 0;

        bool playing = false;

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
            if (shipCountsPlayer[activePlayer][size] <= 0) return;
            selectedShipSize = size;
            currentPlacement.Clear();
            SetShipButtonsEnabled(false);

            SetGridEnabled(activePlayer, true);
            if (multiplayerMode)
                SetGridEnabled(1 - activePlayer, false);

            EnableFreeCells(activePlayer);
        }

        private void SetGridEnabled(int playerIndex, bool enabled)
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

            if (selectedShipSize > 0 && playerIdx == activePlayer)
            {
                HandlePlacementClick(playerIdx, x, y);
                return;
            }

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
                EnsureStraightLineAndRestrict(playerIdx);
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

        private void EnsureStraightLineAndRestrict(int playerIdx)
        {

            Point a = currentPlacement[0]; Point b = currentPlacement[1];
            if (!(a.X == b.X || a.Y == b.Y))
            {

                var last = currentPlacement[1];
                ButtonsPlayer[playerIdx][last.Y, last.X].BackColor = Color.LightBlue;
                currentPlacement.RemoveAt(1);
                MessageBox.Show("La seconda cella deve essere ortogonale rispetto alla prima.", "Errore posizionamento", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RestrictNextCellsToAdjacent(playerIdx, a.X, a.Y);
                return;
            }

            RestrictLineOptions(playerIdx);
        }

        private void RestrictLineOptions(int playerIdx)
        {

            var btns = ButtonsPlayer[playerIdx];
            for (int yy = 0; yy < 10; yy++)
                for (int xx = 0; xx < 10; xx++)
                    btns[yy, xx].Enabled = false;

            var xs = currentPlacement.Select(p => p.X).ToList();
            var ys = currentPlacement.Select(p => p.Y).ToList();
            bool vertical = xs.Distinct().Count() == 1;
            if (vertical)
            {
                int col = xs[0];
                int minY = ys.Min(); int maxY = ys.Max();

                int needed = selectedShipSize - currentPlacement.Count;

                for (int add = 1; add <= needed; add++)
                {
                    int yy = minY - add; if (yy >= 0 && GridPlayer[playerIdx][yy, col] == 0) btns[yy, col].Enabled = true;
                    yy = maxY + add; if (yy < 10 && GridPlayer[playerIdx][yy, col] == 0) btns[yy, col].Enabled = true;
                }
            }
            else
            {
                int row = ys[0];
                int minX = xs.Min(); int maxX = xs.Max();
                int needed = selectedShipSize - currentPlacement.Count;
                for (int add = 1; add <= needed; add++)
                {
                    int xx = minX - add; if (xx >= 0 && GridPlayer[playerIdx][row, xx] == 0) btns[row, xx].Enabled = true;
                    xx = maxX + add; if (xx < 10 && GridPlayer[playerIdx][row, xx] == 0) btns[row, xx].Enabled = true;
                }
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
        private List<Point> OrderPoints(List<Point> points)
        {
            // Ordina prima per Y, poi nel caso essia sia uguale, per X
            points.Sort((a, b) =>
            {
                if (a.Y != b.Y) return a.Y - b.Y; // confronto Y
                return a.X - b.X;                  // se Y uguale, confronto X
            });
            return points.ToList();
        }


        private void FinalizePlacement(int playerIdx)
        {

            if (currentPlacement.Count != selectedShipSize)
            {
                MessageBox.Show("Numero celle selezionate non corrisponde alla dimensione della nave.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                ResetCurrentPlacementVisuals(playerIdx);
                selectedShipSize = 0;
                SetShipButtonsEnabled(true);
                EnableFreeCells(playerIdx);
                return;
            }

            var pts = OrderPoints(currentPlacement);
            bool vertical = IsVertical(pts);
            Point origin = pts.OrderBy(p => vertical ? p.Y : p.X).First();
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
                    MessageBox.Show("Giocatore 1 ha finito il posizionamento. Ora tocca al Giocatore 2.", "Posizionamento", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    activePlayer = 1;
                    EnableFreeCells(activePlayer);
                    SetGridEnabled(0, false);
                    SetGridEnabled(1, true);
                    SetShipButtonsEnabled(true);
                }
                else
                {

                    MessageBox.Show($"Giocatore {playerIdx + 1} ha finito il posizionamento.", "Posizionamento", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    SetGridEnabled(0, false);
                    if (multiplayerMode) SetGridEnabled(1, false);
                }
                if (shipCountsPlayer[0].Values.All(v => v == 0) && (!multiplayerMode ? true : shipCountsPlayer[1].Values.All(v => v == 0)))
                {
                    SetShipButtonsVisible(false);
                    lbl_Log.Visible = true;
                    playing = true;
                }
            }
            else
            {

                EnableFreeCells(playerIdx);
                SetGridEnabled(playerIdx, true);
            }
        }

        private void ResetCurrentPlacementVisuals(int playerIdx)
        {
            foreach (var pt in currentPlacement)
            {
                var b = ButtonsPlayer[playerIdx][pt.Y, pt.X];
                b.BackColor = Color.LightBlue;
            }
            currentPlacement.Clear();
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
            lbl_Log.Text += "\n" + msg;
        }
    }
}