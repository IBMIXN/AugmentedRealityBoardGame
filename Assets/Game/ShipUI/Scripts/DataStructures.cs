using System;
using System.Collections;
using System.Collections.Generic;

public enum ShipStat { Shields, Armour, Speed, HP, DmgLaser, DmgPlasma, DmgTesla, DmgMissile, DmgKinetic, DmgRipper }

public static class StatNames
{
    private static Dictionary<ShipStat, string> nameDict = new Dictionary<ShipStat, string>
    {
        { ShipStat.Shields, "Shield HP" },
        { ShipStat.Armour, "Armour HP" },
        { ShipStat.Speed, "Speed" },
        { ShipStat.HP, "Hit Points" },
        { ShipStat.DmgLaser, "Laser Damage" },
        { ShipStat.DmgPlasma, "Plasma Damage" },
        { ShipStat.DmgTesla, "Tesla Damage" },
        { ShipStat.DmgMissile, "Missile Damage" },
        { ShipStat.DmgKinetic, "Kinetic Damage" },
        { ShipStat.DmgRipper, "Ripper Damage" },
    };

    public static string ToString(ShipStat stat)
    {
        return nameDict[stat];
    }
}

public enum SystemID { Weapons, Defences, Engines, Auxiliary }

public enum ShipTier { Corvette, Destroyer, Cruiser, Battleship, Flagship }