using BattagliaNavale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BattagliaNavaleEventi
{
    public class GameManager
    {
        public int[][,] GridPlayer { get; private set; } = new int[2][,] { new int[10, 10], new int[10, 10] }; //0=acqua, -1=acqua colpita, -2=nave colpita, N=nave di dimensione N
        public Ship[][,] ShipsPlayer { get; private set; } = new Ship[2][,] { new Ship[10, 10], new Ship[10, 10] };

        Dictionary<int, int> shipCountsTemplate = new Dictionary<int, int> { { 4, 1 }, { 3, 2 }, { 2, 2 }, { 1, 1 } };
        public Dictionary<int, int>[] shipCountsPlayer { get; private set; } = new Dictionary<int, int>[2];

        public int CurrentPlayer { get; private set; } = 0;
        public bool multiplayerMode { get; private set; } = false;
        public int selectedShipSize { get; private set; } = 0;
        public List<Point> currentPlacement { get; private set; } = new List<Point>();

        public bool playing { get; private set; } = false;
        int shipsSunk1 = 0;
        public int numAttemps1 { get; private set; } = 0;
        int shipsSunk2 = 0;
        int numAttemps2 = 0;

        Random random = new Random(Environment.TickCount);

        bool SecondIsBot;
        bool puntamentoAttivo = false; //se ha identificato un punto a cui puntare, scoprendo le celle vicine
        Point puntamentoIdx1; //i due estremi per ora trovati
        Point puntamentoIdx2;
        bool? puntamentoVerticale; //null = non lo si sa ancora
        //aggiungi altre variabili che ti serviranno

        public event EventHandler<ColpitoEventArgs> EventoAttacco = null;

        public event EventHandler ResetPlacementGraphics;
        public event EventHandler BeginPlacementGraphics;
        public event EventHandler<(int numAttemps, int numPlayer)> UpdateAttepsGraphics;
        public event EventHandler<(int, int)> UpdateShipsSunkGraphics;
        public event EventHandler ShowVictoryGraphics;
        public event EventHandler<int> UpdateGUI;
        public event EventHandler<(int playerId, bool enabled)> SetGridEnabledGraphics;
        public event EventHandler PlacementEndedGraphics;
        public event EventHandler<string> ShowAlert;

        public GameManager(bool multiplayer, bool bot = false)
        {
            multiplayerMode = multiplayer;
            SecondIsBot = bot;
        }

        public void Start()
        {
            //questa porzione di codice doveva essere nel costruttore, ma lì
            //gli eventi non sono ancora stati settati, quindi bisogna chiamare
            //questa funzione non appena gli eventi sono settati

            ResetPlacement();
        }

        public void ResetPlacement()
        {
            for (int p = 0; p < 2; p++)
            {
                for (int y = 0; y < 10; y++)
                    for (int x = 0; x < 10; x++)
                    {
                        GridPlayer[p][y, x] = 0;
                        ShipsPlayer[p][y, x] = null;
                    }
                shipCountsPlayer[p] = shipCountsTemplate.ToDictionary(kv => kv.Key, kv => kv.Value);
            }
            CurrentPlayer = 0;
            selectedShipSize = 0;
            currentPlacement.Clear();

            ResetPlacementGraphics?.Invoke(this, EventArgs.Empty);
        }

        public void BeginPlacement(int size)
        {
            selectedShipSize = size;
            currentPlacement.Clear();

            BeginPlacementGraphics?.Invoke(this, EventArgs.Empty);
        }

        public bool ClickBtnCella(int x, int y, int playerIdx)
        {
            if (!playing) //piazzamento
            {
                if (selectedShipSize > 0 && playerIdx == CurrentPlayer)
                {
                    HandlePlacementClick(playerIdx, x, y);
                    return true;
                }
                else
                    return false;
            }
            else
            {
                HandleAttackClick(playerIdx, x, y);

                if (SecondIsBot)
                {
                    Thread.Sleep(100);
                    if (!puntamentoAttivo)
                        HandleAttackClick(playerIdx, random.Next(0, 11), random.Next(0, 11), true);
                    else
                    {
                        if (puntamentoVerticale == null)
                        {
                            int xNext = puntamentoIdx1.X + 1;
                            if (xNext == 10)
                                xNext = 8;
                            HandleAttackClick(playerIdx, xNext, puntamentoIdx1.Y, true);
                        }
                        else
                        {

                        }
                    }
                }

                return true;
            }
        }

        private void HandleAttackClick(int playerIdx, int x, int y, bool bot = false)
        {
            if (CurrentPlayer == 0)
                numAttemps1++;
            else
                numAttemps2++;

            UpdateAttepsGraphics?.Invoke(this, ((CurrentPlayer==0 ? numAttemps1 : numAttemps2), CurrentPlayer));

            bool colpito = GridPlayer[playerIdx][y, x] >= 1; //nave

            EventoAttacco?.Invoke(this, new ColpitoEventArgs(colpito, new Point(x, y)));

            if (colpito) //nave
            {
                GridPlayer[playerIdx][y, x] = -2;
                Ship curShip = ShipsPlayer[playerIdx][y, x];
                if (curShip.removeTile()) //ultimo tile
                {
                    UpdateShipsSunkGraphics?.Invoke(this, (CurrentPlayer == 0 ? shipsSunk1++ : shipsSunk2++, CurrentPlayer));
                    checkVictory();
                }
            }
            else if (GridPlayer[playerIdx][y, x] == 0)
            {
                GridPlayer[playerIdx][y, x] = -1;
            }

            UpdateGUI?.Invoke(this, playerIdx);

            if (multiplayerMode)
            {
                CurrentPlayer = 1 - CurrentPlayer;

                SetGridEnabledGraphics?.Invoke(this, (CurrentPlayer, true));
                SetGridEnabledGraphics?.Invoke(this, (1-CurrentPlayer, false));
            }

        }

        private void HandlePlacementClick(int playerIdx, int x, int y)
        {
            currentPlacement.Add(new Point(x, y));

            if (currentPlacement.Count == selectedShipSize) //finita
            {
                FinalizePlacement(playerIdx);
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

            foreach (Point pt in pts)
            {
                GridPlayer[playerIdx][pt.Y, pt.X] = selectedShipSize;
                ShipsPlayer[playerIdx][pt.Y, pt.X] = s;
            }

            shipCountsPlayer[playerIdx][selectedShipSize]--;

            currentPlacement.Clear();
            selectedShipSize = 0;

            bool allPlaced = shipCountsPlayer[playerIdx].Values.All(v => v == 0);

            if (allPlaced)
            {
                if (multiplayerMode && playerIdx == 0)
                {

                    ShowAlert?.Invoke(this, "Giocatore 1 ha finito il posizionamento. Ora tocca al Giocatore 2.");
                    CurrentPlayer = 1;
                    SetGridEnabledGraphics(this, (0, false));
                    SetGridEnabledGraphics(this, (1, true));

                    PlacementEndedGraphics?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    ShowAlert?.Invoke(this, ($"Giocatore {playerIdx + 1} ha finito il posizionamento."));

                    SetGridEnabledGraphics(this, (0, false));
                    if (multiplayerMode)
                        SetGridEnabledGraphics(this, (1, true));

                    PlacementEndedGraphics?.Invoke(this, EventArgs.Empty);
                }
                if (shipCountsPlayer[0].Values.All(v => v == 0) && (!multiplayerMode ? true : shipCountsPlayer[1].Values.All(v => v == 0)))
                {
                    // terminati piazzamenti
                    playing = true;

                    PlacementEndedGraphics?.Invoke(this, EventArgs.Empty);

                    CurrentPlayer = 0;
                    UpdateAttepsGraphics?.Invoke(this, (numAttemps1, 0));

                    UpdateShipsSunkGraphics?.Invoke(this, (0, shipsSunk1));
                    UpdateGUI?.Invoke(this, 0);
                    SetGridEnabledGraphics(this, (0, true));

                    if (multiplayerMode)
                    {
                        SetGridEnabledGraphics (this, (1, false));
                        UpdateGUI?.Invoke(this, 1);
                        UpdateShipsSunkGraphics?.Invoke(this, (1, shipsSunk2));
                        UpdateAttepsGraphics?.Invoke(this, (numAttemps2, 1));

                    }
                }
            }
            else
            {
                SetGridEnabledGraphics?.Invoke(this, (playerIdx, true));
                PlacementEndedGraphics?.Invoke(this, EventArgs.Empty);
            }
        }

        private void checkVictory()
        {
            const int totNumShip = 6;
            if ((CurrentPlayer==0 ? shipsSunk1 : shipsSunk2) == totNumShip)
            {
                ShowVictoryGraphics?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
