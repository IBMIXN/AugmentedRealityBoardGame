using TbsFramework.Grid;
using UnityEngine;

/// <summary>
/// Class represents a participant of the game, the Play method allows the user to interact with the players.
/// </summary>
namespace TbsFramework.Players
{
    public abstract class Player : MonoBehaviour
    {
        public int PlayerNumber;    
        public abstract void Play(CellGrid cellGrid);
    }
}