using TbsFramework.Grid;
using TbsFramework.Grid.GridStates;

/// <summary>
/// USER CONTROLLED PLAYER (Cell waiting for an input from the user)
/// </summary>
namespace TbsFramework.Players
{
    public class HumanPlayer : Player
    {
        public override void Play(CellGrid cellGrid)
        {
            cellGrid.CellGridState = new CellGridStateWaitingForInput(cellGrid);
        }
    }
}
