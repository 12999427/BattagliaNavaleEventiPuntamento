using BattagliaNavale;
using System;
using System.Collections.Generic;
using System.Drawing; // Assicurati di avere questo using per Point
using System.Linq;

namespace BattagliaNavaleEventi
{
    public class GameManager
    {
        public int[][,] GridPlayer { get; private set; } = new int[2][,] { new int[10, 10], new int[10, 10] };
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

        // VARIABILI DI STATO PER IL BOT
        private List<Point> botMosseCoda = new List<Point>(); // Coda dei colpi pianificati
        private Point _botFirstHit = new Point(-1, -1); // Primo colpo a segno sulla nave corrente
        private Point _botCurrentDelta = new Point(0, 0); //Delta tra posizione iniziale a utltima
        private bool _botTracingLine = false; // True se il bot ha capito la direzione (almeno2 colpi)

        // --- Eventi ---
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

            // Reset stato Bot
            botMosseCoda.Clear();
            _botFirstHit = new Point(-1, -1);
            _botTracingLine = false;
            _botCurrentDelta = new Point(0, 0);

            ResetPlacementGraphics?.Invoke(this, EventArgs.Empty);

            // Navi random bot? Non ho capito se è richiesto
            if (SecondIsBot)
            {
                //PlaceBotShips();
            }
        }

        public void BeginPlacement(int size)
        {
            selectedShipSize = size;
            currentPlacement.Clear();
            BeginPlacementGraphics?.Invoke(this, EventArgs.Empty);
        }

        public bool ClickBtnCella(int x, int y, int playerIdx)
        {
            if (!playing)
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
                if (SecondIsBot && CurrentPlayer == 1)
                    return false; //click sbagalito

                // umano attacca
                HandleAttackClick(playerIdx, x, y);

                //bot
                if (SecondIsBot)
                {
                    // Ritardo fittizio opzionale
                    EseguiMossaDeterminataEventualmentePuntamento();
                }

                return true;
            }
        }

        private void EseguiMossaDeterminataEventualmentePuntamento()
        {
            Point target = new Point(-1, -1);
            int pIndex = 0;

            //se la coda dei target è vuota, random
            if (botMosseCoda.Count == 0)
            {
                do
                {
                    target = new Point(random.Next(0, 10), random.Next(0, 10));
                }
                while (GridPlayer[pIndex][target.Y, target.X] < 0);
            }
            else
            {
                // prima mossa (bersagio xy)
                target = botMosseCoda[0];
                botMosseCoda.RemoveAt(0);

            }

            // esegui attacco
            HandleAttackClick(pIndex, target.X, target.Y, true);

            // ANALISI RISULTATO

            int result = GridPlayer[pIndex][target.Y, target.X];
            bool sunk = false;
            if (ShipsPlayer[pIndex][target.Y, target.X] != null)
                sunk = ShipsPlayer[pIndex][target.Y, target.X].sunk;

            if (result == -2) //nave colpita
            {
                if (sunk)
                {
                    // Nave affondata, torna random
                    botMosseCoda.Clear();
                    _botTracingLine = false;
                    _botFirstHit = new Point(-1, -1);
                    _botCurrentDelta = new Point(0, 0);
                }
                else
                {
                    if (!_botTracingLine && _botFirstHit.X == -1)
                    {
                        _botFirstHit = target;

                        // Aggiungiamo i vicini nella coda
                        AddTargetIfValid(new Point(target.X + 1, target.Y));
                        AddTargetIfValid(new Point(target.X, target.Y + 1)); 
                        AddTargetIfValid(new Point(target.X - 1, target.Y)); 
                        AddTargetIfValid(new Point(target.X, target.Y - 1));
                    }
                    else if (!_botTracingLine && _botFirstHit.X != -1)
                    {
                        //se invece è una delle mosse nella croce di una cella random (4 casi sopra)
                        _botTracingLine = true;

                        // Calcola il delta (differenza tra colpo attuale e il primo colpo)
                        int dx = target.X - _botFirstHit.X;
                        int dy = target.Y - _botFirstHit.Y;
                        _botCurrentDelta = new Point(dx, dy);

                        // rimuovi tentativi nelle altre direzioni
                        botMosseCoda.Clear();

                        // Aggiunge il prossimo punto in QUESTA direzione
                        AddTargetIfValid(new Point(target.X + dx, target.Y + dy));
                    }
                    else if (_botTracingLine)
                    {
                        // TERZO CASO, SI SEGUE UNA LINEA
                        AddTargetIfValid(new Point(target.X + _botCurrentDelta.X, target.Y + _botCurrentDelta.Y));
                    }
                }
            }
            else // ACQUA
            {
                if (_botTracingLine)
                {
                    //bisogna invertire la direzione di tentativo
                    _botCurrentDelta = new Point(-_botCurrentDelta.X, -_botCurrentDelta.Y);

                    Point reverseTarget = new Point(_botFirstHit.X + _botCurrentDelta.X, _botFirstHit.Y + _botCurrentDelta.Y);
                    AddTargetIfValid(reverseTarget);
                }
            }
        }

        private void AddTargetIfValid(Point p)
        {
            // bordi
            if (p.X >= 0 && p.X < 10 && p.Y >= 0 && p.Y < 10)
            {
                if (GridPlayer[0][p.Y, p.X] >= 0)
                {
                    // Evita duplicati
                    if (!botMosseCoda.Contains(p))
                        botMosseCoda.Add(p);
                }
            }
        }

        private void PlaceBotShips()
        {
            int botIdx = 1;
            int[] sizes = { 4, 3, 3, 2, 2, 1 };

            foreach (int size in sizes)
            {
                bool placed = false;
                while (!placed)
                {
                    int x = random.Next(0, 10);
                    int y = random.Next(0, 10);
                    bool vertical = random.Next(0, 2) == 0;

                    
                    currentPlacement.Clear();
                    selectedShipSize = size;

                    
                    bool fit = true;
                    for (int i = 0; i < size; i++)
                    {
                        int cx = vertical ? x : x + i;
                        int cy = vertical ? y + i : y;

                        if (cx >= 10 || cy >= 10 || GridPlayer[botIdx][cy, cx] != 0) // Fuori o occupato
                        {
                            fit = false;
                            break;
                        }

                        currentPlacement.Add(new Point(cx, cy));
                    }

                    if (fit)
                    {
                        FinalizePlacement(botIdx);
                        placed = true;
                    }
                }
            }

            currentPlacement.Clear();
            selectedShipSize = 0;
        }

        private void HandleAttackClick(int playerIdx, int x, int y, bool isBotAttack = false)
        {
            if (CurrentPlayer == 0)
                numAttemps1++;
            else
                numAttemps2++;

            UpdateAttepsGraphics?.Invoke(this, ((CurrentPlayer == 0 ? numAttemps1 : numAttemps2), CurrentPlayer));

            bool colpito = GridPlayer[playerIdx][y, x] >= 1; // nave

            EventoAttacco?.Invoke(this, new ColpitoEventArgs(colpito, new Point(x, y)));

            if (colpito) // nave
            {
                GridPlayer[playerIdx][y, x] = -2;
                Ship curShip = ShipsPlayer[playerIdx][y, x];
                if (curShip.removeTile()) // ultimo tile -> affondata
                {
                    UpdateShipsSunkGraphics?.Invoke(this, (CurrentPlayer == 0 ? shipsSunk1++ : shipsSunk2++, CurrentPlayer));
                    checkVictory();
                }
            }
            else if (GridPlayer[playerIdx][y, x] == 0) // acqua
            {
                GridPlayer[playerIdx][y, x] = -1;
            }

            UpdateGUI?.Invoke(this, playerIdx);

            if (multiplayerMode)
            {
                CurrentPlayer = 1 - CurrentPlayer;

                if (!SecondIsBot)
                {
                    SetGridEnabledGraphics?.Invoke(this, (CurrentPlayer, false));
                    SetGridEnabledGraphics?.Invoke(this, (1 - CurrentPlayer, true));
                }
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
                if (pts[i].X != firstX) return false;
            return true;
        }

        private List<Point> OrderPointsYX(List<Point> points)
        {
            points.Sort((a, b) =>
            {
                if (a.Y != b.Y) return a.Y - b.Y;
                return a.X - b.X;
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
                    if (pts[i].Y < best.Y) best = pts[i];
                }
                else
                {
                    if (pts[i].X < best.X) best = pts[i];
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

            bool allPlaced = shipCountsPlayer[playerIdx].Values.All(v => v == 0);

            currentPlacement.Clear();
            selectedShipSize = 0;

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
                    if (!SecondIsBot || playerIdx == 0)
                        ShowAlert?.Invoke(this, ($"Giocatore {playerIdx + 1} ha finito il posizionamento."));

                    SetGridEnabledGraphics(this, (0, false));
                    if (multiplayerMode) SetGridEnabledGraphics(this, (1, true));

                    PlacementEndedGraphics?.Invoke(this, EventArgs.Empty);
                }

                bool p0Done = shipCountsPlayer[0].Values.All(v => v == 0);
                bool p1Done = (!multiplayerMode && !SecondIsBot) ? true : shipCountsPlayer[1].Values.All(v => v == 0);

                if (p0Done && p1Done)
                {
                    playing = true;
                    PlacementEndedGraphics?.Invoke(this, EventArgs.Empty);

                    CurrentPlayer = 0;
                    UpdateAttepsGraphics?.Invoke(this, (numAttemps1, 0));
                    UpdateShipsSunkGraphics?.Invoke(this, (0, shipsSunk1));
                    UpdateGUI?.Invoke(this, 0);
                    SetGridEnabledGraphics(this, (0, true));

                    if (multiplayerMode)
                    {
                        SetGridEnabledGraphics(this, (1, true));
                        SetGridEnabledGraphics(this, (0, false));
                        UpdateGUI?.Invoke(this, 1);
                        UpdateShipsSunkGraphics?.Invoke(this, (1, shipsSunk2));
                        UpdateAttepsGraphics?.Invoke(this, (numAttemps2, 1));
                    }
                }
            }
            else
            {
                if (!SecondIsBot || playerIdx == 0)
                    SetGridEnabledGraphics?.Invoke(this, (playerIdx, true));

                PlacementEndedGraphics?.Invoke(this, EventArgs.Empty);
            }
        }

        private void checkVictory()
        {
            const int totNumShip = 6;
            int currentScore = CurrentPlayer == 0 ? shipsSunk1 : shipsSunk2;

            if (currentScore == totNumShip)
            {
                ShowVictoryGraphics?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}