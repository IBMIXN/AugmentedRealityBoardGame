namespace TbsFramework.Units
{
    public interface Buff
    {
        int Duration { get; set; }

        void Apply(Unit unit);

        void Undo(Unit unit);

        Buff Clone();
    }
}