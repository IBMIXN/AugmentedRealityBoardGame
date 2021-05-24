using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TbsFramework.Units
{
    public static class AttackManager
    {
        private static Dictionary<ShipStat, WeaponTypeInfo> weaponTypeInfo = new Dictionary<ShipStat, WeaponTypeInfo>()
        {
            {ShipStat.DmgLaser, new WeaponTypeInfo(0.7f, 1f, 1f, 0f) },
            {ShipStat.DmgPlasma, new WeaponTypeInfo(1f, 1.2f, 0.7f, 0f) },
            {ShipStat.DmgTesla, new WeaponTypeInfo(1f, 1f, 1f, 0.15f) },
            {ShipStat.DmgMissile, new WeaponTypeInfo(1.2f, 1f, 1f, 0f) },
            {ShipStat.DmgKinetic, new WeaponTypeInfo(1f, 1.2f, 1f, 0f) },
            {ShipStat.DmgRipper, new WeaponTypeInfo(1.2f, 1f, 1f, 0f) },
        };

        public static void ResolveAttack(ShipData defendingData, WeaponsSystem attackingWeapon)
        {
            ApplyDamage(defendingData, attackingWeapon);
        }

        private static void ApplyDamage(ShipData defendingData, WeaponsSystem attackingWeapon)
        {
            WeaponTypeInfo weaponInfo = weaponTypeInfo[attackingWeapon.getShipSystem().stat];
            float remainingDamage = attackingWeapon.getShipSystem().currentValue;
            // Peneration damage is dealt directly to the HP
            if (weaponInfo.Peneration > 0f)
            {
                float damage = (1 - weaponInfo.Peneration) * remainingDamage;
                remainingDamage = weaponInfo.Peneration * remainingDamage;
                defendingData.addToCurrentStatValue(ShipStat.HP, -damage);
                if (defendingData.getCurrentStatValue(ShipStat.HP) <= 0f)
                {
                    return;
                }
            }
            // If the shields are around, they absorb the entire hit
            if (defendingData.getCurrentStatValue(ShipStat.Shields) > 0f)
            {
                float damage = remainingDamage * weaponInfo.ShieldDamage;
                defendingData.addToCurrentStatValue(ShipStat.Shields, -damage);
                return;
            }
            // Armour takes as much of the damage as it can, the remaining is passed on to the HP
            if (defendingData.getCurrentStatValue(ShipStat.Armour) > 0f)
            {
                float damage = remainingDamage * weaponInfo.ArmourDamage;
                remainingDamage = (damage - defendingData.getCurrentStatValue(ShipStat.Armour)) / weaponInfo.ArmourDamage;
                defendingData.addToCurrentStatValue(ShipStat.Armour, -damage);
            }
            // HP takes the rest of the damage
            if (remainingDamage > 0f)
            {
                float damage = remainingDamage * weaponInfo.HealthDamage;
                defendingData.addToCurrentStatValue(ShipStat.HP, -damage);
            }
        }

        private class WeaponTypeInfo
        {
            public float ShieldDamage { get; }
            public float ArmourDamage { get; }
            public float HealthDamage { get; }
            public float Peneration { get; }

            public WeaponTypeInfo(float shieldDamage, float armourDamage, float healthDamage, float penetration)
            {
                ShieldDamage = shieldDamage;
                ArmourDamage = armourDamage;
                HealthDamage = healthDamage;
                Peneration = penetration;
            }
        }
    }
}
