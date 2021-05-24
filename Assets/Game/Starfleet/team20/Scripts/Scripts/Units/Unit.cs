using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using TbsFramework.Cells;
using TbsFramework.Pathfinding.Algorithms;
using TbsFramework.Units.UnitStates;

namespace TbsFramework.Units
{
    [ExecuteInEditMode]
    public abstract class Unit : MonoBehaviour
    {
        Dictionary<Cell, List<Cell>> cachedPaths = null;
        public event EventHandler UnitClicked;
        public event EventHandler UnitSelected;
        public event EventHandler UnitDeselected;
        public event EventHandler UnitHighlighted;
        public event EventHandler UnitDehighlighted;
        public event EventHandler<AttackEventArgs> UnitAttacked;
        public event EventHandler<AttackEventArgs> UnitDestroyed;
        public event EventHandler<MovementEventArgs> UnitMoved;

        public UnitState UnitState { get; set; }
        public void SetState(UnitState state)
        {
            UnitState.MakeTransition(state);
        }

        private Cell cell;
        public Cell Cell
        {
            get
            {
                return cell;
            }
            set
            {
                cell = value;
            }
        }

        public float MovementAnimationSpeed;
        public int PlayerNumber;

        public bool IsMoving { get; set; }

        protected SectionButtonManager buttonManager;
        public ShipData Data { get; private set;  }

        private int _turnsSinceLastHit = 0;
        private static readonly int TURNS_UNTIL_REGEN = 1;

        private static DijkstraPathfinding _pathfinder = new DijkstraPathfinding();
        private static IPathfinding _fallbackPathfinder = new AStarPathfinding();

        public virtual void Initialize()
        {
            UnitState = new UnitStateNormal(this);
        }

        public void SetupPowerUI(SectionButtonManager buttonManager, GameObject powerScreenPanel)
        {
            this.buttonManager = buttonManager;
            Data = gameObject.GetComponentInChildren<ShipData>();
            if (Data)
            {
                Data.updateSectionManager();
                Data.SectionManager.powerScreenPanel = powerScreenPanel;
                Data.init();
            }
            else
            {
                Debug.LogWarning("Ship '" + gameObject.name + "' doesn't have ShipData as a child!");
            }
        }

        protected virtual void OnMouseDown()
        {
            if (UnitClicked != null)
            {
                UnitClicked.Invoke(this, new EventArgs());
            }
        }
        protected virtual void OnMouseEnter()
        {
            if (UnitHighlighted != null)
            {
                UnitHighlighted.Invoke(this, new EventArgs());
            }
        }
        protected virtual void OnMouseExit()
        {
            if (UnitDehighlighted != null)
            {
                UnitDehighlighted.Invoke(this, new EventArgs());
            }
        }

        public virtual void OnTurnStart()
        {
            Data.setCurrentStatValueToMax(ShipStat.Speed);
            // Shields regenerate [10 + 5 * (Ship Tier)] per turn
            if (_turnsSinceLastHit >= TURNS_UNTIL_REGEN)
            {
                Data.addToCurrentStatValue(ShipStat.Shields, 10 + 5 * (int)Data.ShipTier);
            }
            _turnsSinceLastHit += 1;
            Data.SectionManager.resetWeapons();
            Data.SectionManager.PowerDiverted = false;
            DefenceActionPerformed();

            SetState(new UnitStateMarkedAsFriendly(this));
        }

        public virtual void OnTurnEnd()
        {
            cachedPaths = null;

            SetState(new UnitStateNormal(this));
        }

        protected virtual void OnDestroyed()
        {
            Cell.IsTaken = false;
            MarkAsDestroyed();
            Destroy(gameObject);
        }

        public virtual void OnUnitSelected()
        {
            buttonManager.setSelectedShip(Data.gameObject, this);
            SetState(new UnitStateMarkedAsSelected(this));
            if (UnitSelected != null)
            {
                UnitSelected.Invoke(this, new EventArgs());
            }
        }

        public virtual void OnUnitDeselected()
        {
            if (buttonManager.SelectedShipUnit == Data.gameObject)
            {
                buttonManager.setSelectedShip(null, null);
            }
            SetState(new UnitStateMarkedAsFriendly(this));
            if (UnitDeselected != null)
            {
                UnitDeselected.Invoke(this, new EventArgs());
            }
        }

        public virtual bool IsUnitAttackable(Unit other, Cell sourceCell, WeaponsSystem weapon)
        {
            return !weapon.Fired && other.PlayerNumber != PlayerNumber &&
                sourceCell.GetDistance(other.Cell) <= weapon.Range;
        }

        public void AttackHandler(Unit unitToAttack, WeaponsSystem weapon)
        {
            if (!IsUnitAttackable(unitToAttack, Cell, weapon))
            {
                return;
            }

            MarkAsAttacking(unitToAttack);
            unitToAttack.DefendHandler(this, weapon);
            weapon.Fired = true;
        }

        public void DefendHandler(Unit aggressor, WeaponsSystem weapon)
        {
            MarkAsDefending(aggressor);
            AttackManager.ResolveAttack(Data, weapon);

            if (UnitAttacked != null)
            {
                UnitAttacked.Invoke(this, new AttackEventArgs(aggressor, weapon, this));
            }
            if (Data.getCurrentStatValue(ShipStat.HP) <= 0)
            {
                if (UnitDestroyed != null)
                {
                    UnitDestroyed.Invoke(this, new AttackEventArgs(aggressor, weapon, this));
                }
                OnDestroyed();
            }
            DefenceActionPerformed();
            _turnsSinceLastHit = 0;
        }

        protected virtual void DefenceActionPerformed() { }

        public virtual void Move(Cell destinationCell, List<Cell> path)
        {
            var totalMovementCost = path.Sum(h => h.MovementCost);
            Data.addToCurrentStatValue(ShipStat.Speed, -totalMovementCost);

            Cell.IsTaken = false;
            Cell.CurrentUnit = null;
            Cell = destinationCell;
            destinationCell.IsTaken = true;
            destinationCell.CurrentUnit = this;

            if (MovementAnimationSpeed > 0)
            {
                StartCoroutine(MovementAnimation(path));
            }
            else
            {
                transform.position = Cell.transform.position;
            }

            if (UnitMoved != null)
            {
                UnitMoved.Invoke(this, new MovementEventArgs(Cell, destinationCell, path));
            }
        }
        protected virtual IEnumerator MovementAnimation(List<Cell> path)
        {
            IsMoving = true;
            path.Reverse();
            foreach (var cell in path)
            {
                Vector3 destination_pos = new Vector3(cell.transform.localPosition.x, cell.transform.localPosition.y, transform.localPosition.z);
                while (transform.localPosition != destination_pos)
                {
                    transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination_pos, Time.deltaTime * MovementAnimationSpeed);
                    yield return 0;
                }
            }
            IsMoving = false;
            OnMoveFinished();
        }

        protected virtual void OnMoveFinished() { }

        public virtual bool IsCellMovableTo(Cell cell)
        {
            return !cell.IsTaken;
        }

        public virtual bool IsCellTraversable(Cell cell)
        {
            return !cell.IsTaken;
        }

        public HashSet<Cell> GetAvailableDestinations(List<Cell> cells)
        {
            cachedPaths = new Dictionary<Cell, List<Cell>>();

            var paths = CachePaths(cells);
            foreach (var key in paths.Keys)
            {
                if (!IsCellMovableTo(key))
                {
                    continue;
                }
                var path = paths[key];

                var pathCost = path.Sum(c => c.MovementCost);
                if (pathCost <= Data.getCurrentStatValue(ShipStat.Speed))
                {
                    cachedPaths.Add(key, path);
                }
            }
            return new HashSet<Cell>(cachedPaths.Keys);
        }

        private Dictionary<Cell, List<Cell>> CachePaths(List<Cell> cells)
        {
            var edges = GetGraphEdges(cells);
            var paths = _pathfinder.findAllPaths(edges, Cell);
            return paths;
        }

        public List<Cell> FindPath(List<Cell> cells, Cell destination)
        {
            if (cachedPaths != null && cachedPaths.ContainsKey(destination))
            {
                return cachedPaths[destination];
            }
            else
            {
                return _fallbackPathfinder.FindPath(GetGraphEdges(cells), Cell, destination);
            }
        }

        protected virtual Dictionary<Cell, Dictionary<Cell, float>> GetGraphEdges(List<Cell> cells)
        {
            Dictionary<Cell, Dictionary<Cell, float>> ret = new Dictionary<Cell, Dictionary<Cell, float>>();
            foreach (var cell in cells)
            {
                if (IsCellTraversable(cell) || cell.Equals(Cell))
                {
                    ret[cell] = new Dictionary<Cell, float>();
                    foreach (var neighbour in cell.GetNeighbours(cells).FindAll(IsCellTraversable))
                    {
                        ret[cell][neighbour] = neighbour.MovementCost;
                    }
                }
            }
            return ret;
        }

        public abstract void MarkAsDefending(Unit aggressor);

        public abstract void MarkAsAttacking(Unit target);

        public abstract void MarkAsDestroyed();

        public abstract void MarkAsFriendly();

        public abstract void MarkAsReachableEnemy();
  
        public abstract void MarkAsSelected();

        public abstract void MarkAsFinished();
 
        public abstract void UnMark();

        [ExecuteInEditMode]
        public void OnDestroy()
        {
            if (Cell != null)
            {
                Cell.IsTaken = false;
            }
        }
    }

    public class MovementEventArgs : EventArgs
    {
        public Cell OriginCell;
        public Cell DestinationCell;
        public List<Cell> Path;

        public MovementEventArgs(Cell sourceCell, Cell destinationCell, List<Cell> path)
        {
            OriginCell = sourceCell;
            DestinationCell = destinationCell;
            Path = path;
        }
    }

    public class AttackEventArgs : EventArgs
    {
        public Unit Attacker { get; }
        public WeaponsSystem Weapon { get; }
        public Unit Defender { get; }


        public AttackEventArgs(Unit attacker, WeaponsSystem weapon, Unit defender)
        {
            Attacker = attacker;
            Weapon = weapon;
            Defender = defender;
        }
    }
    public class UnitCreatedEventArgs : EventArgs
    {
        public Transform unit;

        public UnitCreatedEventArgs(Transform unit)
        {
            this.unit = unit;
        }
    }
}
