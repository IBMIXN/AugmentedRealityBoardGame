using System;
using TbsFramework.Cells;
using TbsFramework.Grid;
using TbsFramework.Players;
using TbsFramework.Units;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TbsFramework.Example1
{
    public class GuiController : MonoBehaviour
    {
        public CellGrid CellGrid;
        public Button NextTurnButton;

        public Text InfoText;

        void Awake()
        {
            CellGrid.GameStarted += OnGameStarted;
            CellGrid.TurnEnded += OnTurnEnded;
            CellGrid.GameEnded += OnGameEnded;
        }

        private void OnGameStarted(object sender, EventArgs e)
        {
            OnTurnEnded(sender, e);
        }

        private void OnGameEnded(object sender, EventArgs e)
        {
            InfoText.text = "Player " + ((sender as CellGrid).CurrentPlayerNumber + 1) + " wins!";
        }
        private void OnTurnEnded(object sender, EventArgs e)
        {
            NextTurnButton.interactable = ((sender as CellGrid).CurrentPlayer is HumanPlayer);

            InfoText.text = "Player " + ((sender as CellGrid).CurrentPlayerNumber + 1);
        }
        public void RestartLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
