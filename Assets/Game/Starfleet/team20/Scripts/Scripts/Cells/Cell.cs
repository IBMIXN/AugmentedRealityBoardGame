using UnityEngine;
using System;
using System.Collections.Generic;
using TbsFramework.Pathfinding.DataStructs;
using TbsFramework.Units;

namespace TbsFramework.Cells
{
    public abstract class Cell : MonoBehaviour, IGraphNode, IEquatable<Cell>
    {
        [HideInInspector]
        [SerializeField]
        private Vector2 _offsetCoord;
        public Vector2 OffsetCoord { get { return _offsetCoord; } set { _offsetCoord = value; } }
        public bool IsTaken;
        public float MovementCost = 1;
        public Unit CurrentUnit { get; set; }
        public event EventHandler CellClicked;
        public event EventHandler CellHighlighted;
        public event EventHandler CellDehighlighted;

        protected virtual void OnMouseEnter()
        {
            if (CellHighlighted != null)
                CellHighlighted.Invoke(this, new EventArgs());
        }
        protected virtual void OnMouseExit()
        {
            if (CellDehighlighted != null)
                CellDehighlighted.Invoke(this, new EventArgs());
        }
        void OnMouseDown()
        {
            if (CellClicked != null)
                CellClicked.Invoke(this, new EventArgs());
        }

        public abstract int GetDistance(Cell other);

        public abstract List<Cell> GetNeighbours(List<Cell> cells);

        public abstract Vector3 GetCellDimensions();

        public abstract void MarkAsReachable();

        public abstract void MarkAsPath();

        public abstract void MarkAsHighlighted();

        public abstract void UnMark();

        public int GetDistance(IGraphNode other)
        {
            return GetDistance(other as Cell);
        }

        public virtual bool Equals(Cell other)
        {
            return (OffsetCoord.x == other.OffsetCoord.x && OffsetCoord.y == other.OffsetCoord.y);
        }

        public override bool Equals(object other)
        {
            if (!(other is Cell))
                return false;

            return Equals(other as Cell);
        }

        public override int GetHashCode()
        {
            int hash = 23;

            hash = (hash * 37) + (int)OffsetCoord.x;
            hash = (hash * 37) + (int)OffsetCoord.y;
            return hash;
        }

        public abstract void CopyFields(Cell newCell);
    }
}