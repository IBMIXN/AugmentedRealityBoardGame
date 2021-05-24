using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
using TbsFramework.Cells;
using TbsFramework.Grid.GridStates;
using TbsFramework.Grid.UnitGenerators;
using TbsFramework.Players;
using TbsFramework.Units;

namespace TbsFramework.Grid
{
    public class CellGrid : MonoBehaviour
    {
        public event EventHandler LevelLoading;
        public event EventHandler LevelLoadingDone;
        public event EventHandler GameStarted;
        public event EventHandler GameEnded;
        public event EventHandler TurnEnded;

        // Power UI holding the VictoryManagerScript (invoke for a popup)
        public GameObject PowerUIManager;

        // Audio clip


        public event EventHandler<UnitCreatedEventArgs> UnitAdded;

        private CellGridState _cellGridState; 
        public CellGridState CellGridState
        {
            get
            {
                return _cellGridState;
            }
            set
            {
                if (_cellGridState != null)
                    _cellGridState.OnStateExit();
                _cellGridState = value;
                _cellGridState.OnStateEnter();
            }
        }

        public int NumberOfPlayers { get; private set; }

        public Player CurrentPlayer
        {
            get { return Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)); }
        }
        public int CurrentPlayerNumber { get; private set; }

        public Transform PlayersParent;

        public bool GameFinished { get; private set; }
        public List<Player> Players { get; private set; }
        public List<Cell> Cells { get; private set; }
        public List<Unit> Units { get; private set; }

        public SectionButtonManager buttonManager;
        public GameObject powerScreenPanel;

        private void Start()
        {
            if (LevelLoading != null)
                LevelLoading.Invoke(this, new EventArgs());

            Initialize();

            if (LevelLoadingDone != null)
                LevelLoadingDone.Invoke(this, new EventArgs());

            StartGame();
        }

        private void Initialize()
        {
            GameFinished = false;
            Players = new List<Player>();
            for (int i = 0; i < PlayersParent.childCount; i++)
            {
                var player = PlayersParent.GetChild(i).GetComponent<Player>();
                if (player != null)
                    Players.Add(player);
                else
                    Debug.LogError("Invalid object in Players Parent game object");
            }
            NumberOfPlayers = Players.Count;
            CurrentPlayerNumber = Players.Min(p => p.PlayerNumber);

            Cells = new List<Cell>();
            for (int i = 0; i < transform.childCount; i++)
            {
                var cell = transform.GetChild(i).gameObject.GetComponent<Cell>();
                if (cell != null)
                    Cells.Add(cell);
                else
                    Debug.LogError("Invalid object in cells paretn game object");
            }

            foreach (var cell in Cells)
            {
                cell.CellClicked += OnCellClicked;
                cell.CellHighlighted += OnCellHighlighted;
                cell.CellDehighlighted += OnCellDehighlighted;
                cell.GetComponent<Cell>().GetNeighbours(Cells);
            }

            Units = new List<Unit>();
            var unitGenerator = GetComponent<IUnitGenerator>();
            if (unitGenerator != null)
            {
                var units = unitGenerator.SpawnUnits(Cells);
                foreach (var unit in units)
                {
                    unit.SetupPowerUI(buttonManager, powerScreenPanel);
                    AddUnit(unit.GetComponent<Transform>());
                }
                buttonManager.cellGrid = this;
            }
            else
                Debug.LogError("No IUnitGenerator script attached to cell grid");
        }

        private void OnCellDehighlighted(object sender, EventArgs e)
        {
            CellGridState.OnCellDeselected(sender as Cell);
        }
        private void OnCellHighlighted(object sender, EventArgs e)
        {
            CellGridState.OnCellSelected(sender as Cell);
        }
        private void OnCellClicked(object sender, EventArgs e)
        {
            CellGridState.OnCellClicked(sender as Cell);
        }

        private void OnUnitClicked(object sender, EventArgs e)
        {
            CellGridState.OnUnitClicked(sender as Unit);
        }
        private void OnUnitDestroyed(object sender, AttackEventArgs e)
        {
            Units.Remove(sender as Unit);
            var totalPlayersAlive = Units.Select(u => u.PlayerNumber).Distinct().ToList(); //Checking if the game is over
            if (totalPlayersAlive.Count == 1)
            {
                CellGridState = new CellGridStateBlockInput(this);
                GameFinished = true;

                // Show relevant victory things
                handleVictory();

                if (GameEnded != null)
                    GameEnded.Invoke(this, new EventArgs());
            }
        }

        public void AddUnit(Transform unit)
        {
            Units.Add(unit.GetComponent<Unit>());
            unit.GetComponent<Unit>().UnitClicked += OnUnitClicked;
            unit.GetComponent<Unit>().UnitDestroyed += OnUnitDestroyed;

            if (UnitAdded != null)
                UnitAdded.Invoke(this, new UnitCreatedEventArgs(unit));
        }


        // Used to show a victory popup with the relevant player number and
        // play a short audio clip
        private void handleVictory()
        {
            // Call the victory popup passing the player number
            PowerUIManager.GetComponent<VictoryManagerScript>().ShowVictory(CurrentPlayerNumber + 1);

        }

        public void StartGame()
        {
            if (GameStarted != null)
                GameStarted.Invoke(this, new EventArgs());

            Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
            Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
            Debug.Log("Game started");
        }

        public void EndTurn()
        {
            CellGridState = new CellGridStateBlockInput(this);
            if (Units.Select(u => u.PlayerNumber).Distinct().Count() == 1)
            {
                return;
            }

            Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnEnd(); });

            CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
            while (Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).Count == 0)
            {
                CurrentPlayerNumber = (CurrentPlayerNumber + 1) % NumberOfPlayers;
            }

            if (TurnEnded != null)
                TurnEnded.Invoke(this, new EventArgs());

            Debug.Log(string.Format("Player {0} turn", CurrentPlayerNumber));
            Units.FindAll(u => u.PlayerNumber.Equals(CurrentPlayerNumber)).ForEach(u => { u.OnTurnStart(); });
            Players.Find(p => p.PlayerNumber.Equals(CurrentPlayerNumber)).Play(this);
        }
    }
}

