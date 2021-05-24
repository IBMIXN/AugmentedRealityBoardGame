using System.Collections.Generic;
using TbsFramework.Cells;
using TbsFramework.Units;
using TbsFramework.Units.UnitStates;

namespace TbsFramework.Grid.GridStates
{
    class CellGridStateFiring : CellGridState
    {
        private Unit _unit;
        private WeaponsSystem _weapon;
        private List<Unit> _unitsInRange;
        private List<Unit> _unitsMarkedInRange;
        private Cell _unitCell;

        public CellGridStateFiring(CellGrid cellGrid, Unit unit, WeaponsSystem weapon) : base(cellGrid)
        {
            _unit = unit;
            _weapon = weapon;
            _unitsInRange = new List<Unit>();
            _unitsMarkedInRange = new List<Unit>();
        }

        // When a cell is clicked, exit firing mode
        public override void OnCellClicked(Cell cell)
        {
            if (_unit.IsMoving)
                return;

            _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, _unit);
            return;
        }

        // If you click an enemy that's in range, fire at them
        // If it's an ally, exit firing mode
        public override void OnUnitClicked(Unit unit)
        {
            if (unit.Equals(_unit) || _unit.IsMoving)
                return;

            if (_unitsInRange.Contains(unit) && !_unit.IsMoving)
            {
                _unit.AttackHandler(unit, _weapon);
                if (!_cellGrid.GameFinished)
                {
                    _cellGrid.CellGridState = new CellGridStateUnitSelected(_cellGrid, _unit);
                }
            }

            if (unit.PlayerNumber.Equals(_unit.PlayerNumber))
            {
                return;
            }
        }

        public override void OnCellDeselected(Cell cell)
        {
            base.OnCellDeselected(cell);
            foreach (var unit in _unitsMarkedInRange)
            {
                unit.UnMark();
            }
            _unitsMarkedInRange.Clear();
            foreach (var unit in _unitsInRange)
            {
                unit.MarkAsReachableEnemy();
            }
        }

        public override void OnCellSelected(Cell cell)
        {
            foreach (var unit in _unitsInRange)
            {
                unit.UnMark();
            }
            foreach (var currentUnit in _cellGrid.Units)
            {
                if (_unit.IsUnitAttackable(currentUnit, _unitCell, _weapon))
                {
                    currentUnit.SetState(new UnitStateMarkedAsReachableEnemy(currentUnit));
                    _unitsMarkedInRange.Add(currentUnit);
                }
            }
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();
            _unit.OnUnitSelected();
            _unitCell = _unit.Cell;

            foreach (var currentUnit in _cellGrid.Units)
            {
                if (currentUnit.PlayerNumber.Equals(_unit.PlayerNumber))
                    continue;

                if (_unit.IsUnitAttackable(currentUnit, _unit.Cell, _weapon))
                {
                    currentUnit.SetState(new UnitStateMarkedAsReachableEnemy(currentUnit));
                    _unitsInRange.Add(currentUnit);
                }
            }
        }

        public override void OnStateExit()
        {
            _unit.OnUnitDeselected();
            foreach (var unit in _unitsInRange)
            {
                if (unit == null) continue;
                unit.SetState(new UnitStateNormal(unit));
            }
            foreach (var cell in _cellGrid.Cells)
            {
                cell.UnMark();
            }
        }
    }
}