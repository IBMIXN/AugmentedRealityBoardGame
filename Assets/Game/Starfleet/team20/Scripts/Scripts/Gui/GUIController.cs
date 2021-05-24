using System;
using TbsFramework.Grid;
using TbsFramework.Grid.GridStates;
using UnityEngine;

namespace TbsFramework.Gui
{
    public class GUIController : MonoBehaviour
    {
        public CellGrid CellGrid;

        void Awake()
        {
            CellGrid.LevelLoading += onLevelLoading;
            CellGrid.LevelLoadingDone += onLevelLoadingDone;
        }

        /// <summary>
        /// While the level is loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onLevelLoading(object sender, EventArgs e)
        {
            Debug.Log("LEVEL LOADING");
        }

        /// <summary>
        /// When the level is done loading
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onLevelLoadingDone(object sender, EventArgs e)
        {
            Debug.Log("LOADING COMPLETE!");
        }

        /// <summary>
        /// On pressing n, turn is passed to the next player (ended)
        /// </summary>
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.N) && !(CellGrid.CellGridState is CellGridStateBlockInput))
            {
                CellGrid.EndTurn(); 
            }
        }
    }
}