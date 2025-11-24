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

        Button[,] Buttons1 = new Button[10, 10];
        Button[,] Buttons2 = new Button[10, 10];
        int[,] Grid1 = new int[10, 10];
        int[,] Grid2 = new int[10, 10];
        Ship[,] Ships1 = new Ship[10, 10];
        Ship[,] Ships2 = new Ship[10, 10];

        Dictionary<int, int> countRemainingPlayer1 = new Dictionary<int, int>();
        Dictionary<int, int> countRemainingPlayer2 = new Dictionary<int, int>();
        readonly Dictionary<int, int> defaultFleet = new Dictionary<int, int>
        {
            {4,1},
            {3,2},
            {2,2},
            {1,1}
        };

        int shipsSunk = 0;
        int numAttemps = 0;
        bool canPlay = false;

        bool placementMode = true;
        int currentPlayerPlacing = 1;
        int selectedShipSize = 0;
        List<Point> tempPlaced = new List<Point>();
        Button lastSelectedShipButton = null;
        bool multiplayerMode = false;
        bool player1PlacementDone = false;
        bool player2PlacementDone = false;

        public Game(bool multiplayer)
        {
            InitializeComponent();
            multiplayerMode = multiplayer;
            GenerateGrid(tbl_grid, 1);
            ShowColors(1);
            if (multiplayer)
            {
                GenerateGrid(tbl_grid2, 2);
                ShowColors(2);
            }
            this.Size = multiplayer ? new Size(1597, 681) : new Size(970, 681);

            ResetFleetCounts();

            lbl_bestAttemps.Visible = false;
            lbl_numAttemps.Visible = false;
            lbl_shipsSunk.Visible = false;

            UpdatePlacementUI();
        }

        private void ResetFleetCounts()
        {
            countRemainingPlayer1 = new Dictionary<int, int>(defaultFleet);
            countRemainingPlayer2 = new Dictionary<int, int>(defaultFleet);
        }

        private void GenerateGrid(TableLayoutPanel gridPanel, int gridOwner)
        {
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Button btn_cell = new Button();
                    btn_cell.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                    btn_cell.ImageAlign = ContentAlignment.BottomLeft;
                    btn_cell.Name = $"btn_grid_{gridOwner}_{x}x{y}";
                    btn_cell.Location = new Point(0, 0);
                    btn_cell.BackColor = Color.LightBlue;
                    btn_cell.Click += ClickBtnCella;
                    btn_cell.Tag = new Tuple<int, Point>(gridOwner, new Point(x, y));
                    btn_cell.BackgroundImageLayout = ImageLayout.Stretch;
                    gridPanel.Controls.Add(btn_cell);
                    if (gridOwner == 1)
                    {
                        Buttons1[y, x] = btn_cell;
                    }
                    else
                    {
                        Buttons2[y, x] = btn_cell;
                    }
                    btn_cell.UseVisualStyleBackColor = true;
                }
            }
        }

        private int[,] GetGrid(int owner) => owner == 1 ? Grid1 : Grid2;
        private Ship[,] GetShips(int owner) => owner == 1 ? Ships1 : Ships2;
        private Button[,] GetButtons(int owner) => owner == 1 ? Buttons1 : Buttons2;
        private Dictionary<int, int> GetCountRemaining(int owner) => owner == 1 ? countRemainingPlayer1 : countRemainingPlayer2;
        private void SetCountRemaining(int owner, Dictionary<int, int> dict)
        {
            if (owner == 1) countRemainingPlayer1 = dict; else countRemainingPlayer2 = dict;
        }

        private void ClickBtnCella(object? sender, EventArgs e)
        {
            Button btn_sender = (Button)sender;
            var tag = (Tuple<int, Point>)btn_sender.Tag;
            int ownerGrid = tag.Item1;
            int x = tag.Item2.X;
            int y = tag.Item2.Y;

            if (placementMode)
            {

                if (ownerGrid != currentPlayerPlacing)
                {

                    return;
                }
                HandlePlacementClick(x, y, ownerGrid, btn_sender);
                return;
            }

            HandleAttackClick(x, y, ownerGrid, btn_sender);
        }

        private void HandlePlacementClick(int x, int y, int ownerGrid, Button clickedButton)
        {
            int[,] G = GetGrid(ownerGrid);
            Ship[,] S = GetShips(ownerGrid);
            Button[,] B = GetButtons(ownerGrid);

            if (G[y, x] != 0) return;

            if (selectedShipSize == 0)
            {

                Log($"Seleziona prima una nave da posizionare (player {currentPlayerPlacing}).");
                return;
            }

            if (tempPlaced.Count == 0)
            {

                tempPlaced.Add(new Point(x, y));
                HighlightTemp(tempPlaced, ownerGrid);

                clickedButton.Enabled = false;
                return;
            }
            else if (tempPlaced.Count == 1)
            {

                Point p0 = tempPlaced[0];
                if (!IsAdjacent(p0, new Point(x, y))) { Log("La seconda cella deve essere adiacente alla prima (solo N/S/E/W)."); return; }

                tempPlaced.Add(new Point(x, y));
                clickedButton.Enabled = false;
                HighlightTemp(tempPlaced, ownerGrid);

                if (tempPlaced.Count == selectedShipSize)
                {
                    MessageBox.Show("asdasd");
                    CommitTempShip(ownerGrid);
                }
                return;
            }
            else
            {

                Point p0 = tempPlaced[0];
                Point p1 = tempPlaced[1];
                int dx = p1.X - p0.X;
                int dy = p1.Y - p0.Y;

                Point leftEnd = tempPlaced.First();
                Point rightEnd = tempPlaced.Last();
                Point candidate = new Point(x, y);

                bool canExtend = false;

                if (candidate.X == leftEnd.X - dx && candidate.Y == leftEnd.Y - dy) canExtend = true;
                if (candidate.X == rightEnd.X + dx && candidate.Y == rightEnd.Y + dy) canExtend = true;

                if (!canExtend)
                {
                    Log("Devi continuare la nave nella stessa direzione (estendere uno degli estremi).");
                    return;
                }

                if (G[y, x] != 0) { Log("Cella non disponibile."); return; }

                if (candidate.X == leftEnd.X - dx && candidate.Y == leftEnd.Y - dy)
                    tempPlaced.Insert(0, candidate);
                else
                    tempPlaced.Add(candidate);

                clickedButton.Enabled = false;
                HighlightTemp(tempPlaced, ownerGrid);

                if (tempPlaced.Count == selectedShipSize)
                {
                    CommitTempShip(ownerGrid);
                }
            }
        }

        private bool IsAdjacent(Point a, Point b)
        {
            int md = Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
            return md == 1;
        }

        private void HighlightTemp(List<Point> cells, int owner)
        {
            Button[,] B = GetButtons(owner);

            for (int yy = 0; yy < 10; yy++)
                for (int xx = 0; xx < 10; xx++)
                    if (GetGrid(owner)[yy, xx] == 0)
                        B[yy, xx].BackColor = Color.LightBlue;

            foreach (var p in cells)
            {
                B[p.Y, p.X].BackColor = Color.Orange;
            }
        }

        private void CommitTempShip(int ownerGrid)
        {
            int[,] G = GetGrid(ownerGrid);
            Ship[,] S = GetShips(ownerGrid);
            Button[,] B = GetButtons(ownerGrid);
            var counts = GetCountRemaining(ownerGrid);

            Point first = tempPlaced[0];
            bool vertical = false;
            if (tempPlaced.Count >= 2)
                vertical = tempPlaced[1].X == first.X;

            Ship ship = new Ship(first.X, first.Y, selectedShipSize, vertical);

            foreach (var p in tempPlaced)
            {
                G[p.Y, p.X] = selectedShipSize;
                S[p.Y, p.X] = ship;

                B[p.Y, p.X].BackColor = Color.Brown;

                B[p.Y, p.X].Enabled = false;
            }

            counts[selectedShipSize]--;
            SetCountRemaining(ownerGrid, counts);
            UpdateShipButtonsAvailability(ownerGrid);

            tempPlaced.Clear();
            DeselectShipButton();

            Log($"Giocatore {ownerGrid}: posizionata nave dimensione {selectedShipSize}.");

            if (AllShipsPlacedForPlayer(ownerGrid))
            {
                if (ownerGrid == 1) player1PlacementDone = true;
                else player2PlacementDone = true;

                Log($"Giocatore {ownerGrid} ha finito il posizionamento.");

                if (multiplayerMode && ownerGrid == 1 && !player2PlacementDone)
                {

                    currentPlayerPlacing = 2;
                    EnableGridForPlacement(2);
                    DisableGridForPlacement(1);
                    Log("Tocca al giocatore 2 posizionare le navi.");
                }
                else
                {

                    placementMode = false;
                    canPlay = true;
                    PrepareForPlay();
                    Log("Partita iniziata!");
                }
            }
        }

        private void UpdateShipButtonsAvailability(int owner)
        {
            var counts = GetCountRemaining(owner);

            int p = currentPlayerPlacing;
            var c = GetCountRemaining(p);
            btn_PosNave4.Enabled = c[4] > 0;
            btn_PosNave3.Enabled = c[3] > 0;
            btn_PosNave2.Enabled = c[2] > 0;
            btn_PosNave1.Enabled = c[1] > 0;
        }

        private bool AllShipsPlacedForPlayer(int owner)
        {
            var counts = GetCountRemaining(owner);
            foreach (var kv in counts)
                if (kv.Value > 0) return false;
            return true;
        }

        private void EnableGridForPlacement(int owner)
        {
            Button[,] B = GetButtons(owner);
            for (int yy = 0; yy < 10; yy++)
                for (int xx = 0; xx < 10; xx++)
                    if (GetGrid(owner)[yy, xx] == 0)
                        B[yy, xx].Enabled = true;
            currentPlayerPlacing = owner;
            UpdateShipButtonsAvailability(owner);
        }

        private void DisableGridForPlacement(int owner)
        {
            Button[,] B = GetButtons(owner);
            for (int yy = 0; yy < 10; yy++)
                for (int xx = 0; xx < 10; xx++)
                    B[yy, xx].Enabled = false;

            btn_PosNave4.Enabled = false;
            btn_PosNave3.Enabled = false;
            btn_PosNave2.Enabled = false;
            btn_PosNave1.Enabled = false;
        }

        private void DeselectShipButton()
        {
            selectedShipSize = 0;
            if (lastSelectedShipButton != null)
            {
                lastSelectedShipButton.BackColor = SystemColors.Control;
                lastSelectedShipButton = null;
            }
        }

        private void ResetPlacementForPlayer(int owner)
        {

            int[,] G = GetGrid(owner);
            Ship[,] S = GetShips(owner);
            Button[,] B = GetButtons(owner);
            for (int yy = 0; yy < 10; yy++)
            {
                for (int xx = 0; xx < 10; xx++)
                {
                    G[yy, xx] = 0;
                    S[yy, xx] = null;
                    B[yy, xx].BackColor = Color.LightBlue;
                    B[yy, xx].Enabled = true;
                }
            }

            SetCountRemaining(owner, new Dictionary<int, int>(defaultFleet));
            tempPlaced.Clear();
            DeselectShipButton();
            UpdateShipButtonsAvailability(owner);
            if (owner == 1) player1PlacementDone = false;
            else player2PlacementDone = false;
            Log($"Reset posizionamento per giocatore {owner}.");
        }

        private void PrepareForPlay()
        {

            btn_PosNave1.Enabled = btn_PosNave2.Enabled = btn_PosNave3.Enabled = btn_PosNave4.Enabled = false;
            btn_ResetPlacement.Enabled = false;

            if (multiplayerMode)
            {

                EnableGridForPlay(1);
                EnableGridForPlay(2);
            }
            else
            {

                EnableGridForPlay(2);

                DisableGridForPlacement(1);
            }
        }

        private void EnableGridForPlay(int owner)
        {
            Button[,] B = GetButtons(owner);
            for (int yy = 0; yy < 10; yy++)
            {
                for (int xx = 0; xx < 10; xx++)
                {

                    int v = GetGrid(owner)[yy, xx];
                    if (v == 0) B[yy, xx].Enabled = true;
                    else if (v > 0) B[yy, xx].Enabled = true;
                    else B[yy, xx].Enabled = true;
                }
            }
        }

        private void HandleAttackClick(int x, int y, int ownerGrid, Button btn_sender)
        {

            int[,] G = GetGrid(ownerGrid);
            Ship[,] S = GetShips(ownerGrid);

            if (G[y, x] == -1 || G[y, x] == -2) return;

            updateAtteps(++numAttemps);

            if (G[y, x] >= 1)
            {
                G[y, x] = -2;
                Ship curShip = S[y, x];
                bool sunkNow = curShip.removeTile();
                if (sunkNow)
                {
                    updateShipsSunk(++shipsSunk);
                    Log($"Giocatore che attaccava: Nave affondata (size {curShip.size}) in posizione ({curShip.x},{curShip.y})");
                }
                else
                {
                    Log($"Colpito in posizione ({x},{y})");
                }
            }
            else if (G[y, x] == 0)
            {
                G[y, x] = -1;
                Log("Acqua !!");
            }
            ShowColorsForAll();
        }

        private void ShowColorsForAll()
        {
            ShowColors(1);
            if (multiplayerMode) ShowColors(2);
        }

        private void ShowColors(int owner)
        {
            int[,] G = GetGrid(owner);
            Button[,] B = GetButtons(owner);
            Ship[,] S = GetShips(owner);
            for (int y = 0; y < 10; y++)
            {
                for (int x = 0; x < 10; x++)
                {
                    Button b = B[y, x];
                    b.BackColor = getColor(G[y, x]);
                    if (G[y, x] == -2 && S[y, x] != null && S[y, x].sunk)
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
                case -2:
                    return Color.Red;
                case -1:
                    return Color.White;
                case 0:
                    return Color.Blue;
                default:
                    return Color.Blue;
            }
        }

        private void ShowVictory()
        {

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
            SelectShipSize(4, (Button)sender);
        }
        private void btn_PosNave3_Click(object sender, EventArgs e)
        {
            SelectShipSize(3, (Button)sender);
        }
        private void btn_PosNave2_Click(object sender, EventArgs e)
        {
            SelectShipSize(2, (Button)sender);
        }
        private void btn_PosNave1_Click(object sender, EventArgs e)
        {
            SelectShipSize(1, (Button)sender);
        }

        private void SelectShipSize(int size, Button btn)
        {
            if (!placementMode) return;
            var counts = GetCountRemaining(currentPlayerPlacing);
            if (counts[size] <= 0)
            {
                Log($"Nessuna nave da {size} rimanente per il giocatore {currentPlayerPlacing}.");
                return;
            }

            DeselectShipButton();
            selectedShipSize = size;
            lastSelectedShipButton = btn;
            lastSelectedShipButton.BackColor = Color.LightGreen;
            Log($"Giocatore {currentPlayerPlacing}: selezionata nave da {size} celle. Clicca le celle per posizionarla.");
        }

        private void btn_ResetPlacement_Click(object sender, EventArgs e)
        {

            ResetPlacementForPlayer(1);
            if (multiplayerMode) ResetPlacementForPlayer(2);
            placementMode = true;
            currentPlayerPlacing = 1;
            player1PlacementDone = player2PlacementDone = false;
            canPlay = false;

            btn_PosNave1.Enabled = btn_PosNave2.Enabled = btn_PosNave3.Enabled = btn_PosNave4.Enabled = true;
            btn_ResetPlacement.Enabled = true;
            UpdatePlacementUI();

            Log("Reset posizionamento completo. Inizia il posizionamento per il giocatore 1.");
        }

        private void UpdatePlacementUI()
        {

            if (!multiplayerMode)
            {

                if (tbl_grid2 != null) tbl_grid2.Visible = false;
            }
            else
            {
                if (tbl_grid2 != null) tbl_grid2.Visible = true;
            }

            EnableGridForPlacement(1);
            if (multiplayerMode)
            {

                DisableGridForPlacement(2);
            }
        }

        private void Log(string s)
        {

            try
            {
                if (this.Controls.Find("lstLog", true).FirstOrDefault() is ListBox lst)
                {
                    lst.Items.Add(s);
                    lst.TopIndex = lst.Items.Count - 1;
                }
                else if (this.Controls.Find("txtLog", true).FirstOrDefault() is TextBox tb)
                {
                    tb.AppendText(s + Environment.NewLine);
                }
                else
                {

                    Console.WriteLine(s);
                }
            }
            catch
            {
                Console.WriteLine(s);
            }
        }
    }
}