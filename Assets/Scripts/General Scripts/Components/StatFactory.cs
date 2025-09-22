using Entities;
using UnityEngine;
using Entities.Modules;
using Entities.Status;

namespace Entities {
    /// <summary>
    /// To create consistent stats and duplicated of default stats.
    /// </summary>
    public static class StatFactory {

        public static int DamageHash => Animator.StringToHash("Damage");
        public static int IncomingDamageHash => Animator.StringToHash("Incoming Damage");
        public static int FireRateHash => Animator.StringToHash("Fire Rate");
        public static int MoveSpeedHash => Animator.StringToHash("Move Speed");
        public static int LuckHash => Animator.StringToHash("Luck");
        
        public static string Damage => "Damage";
        public static string IncomingDamage => "Incoming Damage";
        public static string FireRate => "Fire Rate";
        public static string MoveSpeed => "Move Speed";
        public static string Luck => "Luck";
        
        /// <summary>
        /// The modifier that effects the outgoing damage the entity is going to inflict to other entities.
        /// This stats range is 0.01 &lt;= x &lt; Infinity.
        /// </summary>
        /// <param name="Base">The starting number used for the calculations</param>
        public static StatHandle CreateDamageStat(float Base = 1f) {
            StatHandle DamageStat = new StatHandle("Damage", Base, 0.01f, Mathf.Infinity);
            DamageStat.Result = () => {
                float PercentageTotal = Base * DamageStat.Percent;
                return Mathf.Clamp(
                    (DamageStat.Base - PercentageTotal) *
                    Mathf.Clamp(DamageStat.Multiplier, 0.1f, Mathf.Infinity)
                    + DamageStat.Flat,
                    DamageStat.Min, DamageStat.Max);
            };
            return DamageStat;
        }
        
        /// <summary>
        /// The modifier that effects the incoming damage the entity will receive from other entities.
        /// This stats range is 0.05 &lt;= x &lt; Infinity. Example: The player can have a reduction of
        /// damage up to 95%(0.05), and take additional damage up to Infinity.
        /// </summary>
        /// <param name="Base">The starting number used for the calculations</param>
        /// <remarks>
        /// This stat does not include "Flat" or "Multiplicative" variables. Only way to influence this
        /// stat is by changing the value of the "Percentage" variable.
        /// </remarks>
        public static StatHandle CreateIncomingDamageStat(float Base = 1f) {
            StatHandle IncomingDamageStat = new StatHandle("Incoming Damage", Base, 0.05f, Mathf.Infinity);
            IncomingDamageStat.Result = () => {
                float PercentageTotal = Base * IncomingDamageStat.Percent;
                return Mathf.Clamp(IncomingDamageStat.Base - PercentageTotal, IncomingDamageStat.Min, IncomingDamageStat.Max);
            };
            return IncomingDamageStat;
        }

        /// <summary>
        /// The modifier that effects the Fire Rate of an entity's <c>ShootingComponent</c>,
        /// </summary>
        /// <param name="Base">The starting number used of the calculations of this stat</param>
        public static StatHandle CreateFireRateStat(float Base = 8f) {
            StatHandle FireRateStat = new StatHandle("Fire Rate", Base, 0.1f, 20);
            FireRateStat.Result = () => {
                float PercentageTotal = Base * FireRateStat.Percent;
                return Mathf.Clamp(
                    (FireRateStat.Base + PercentageTotal) * 
                    Mathf.Clamp(FireRateStat.Multiplier, 0.1f, Mathf.Infinity)
                    + FireRateStat.Flat,
                    FireRateStat.Min, FireRateStat.Max);
            };
            return FireRateStat;
        }
        
        /// <summary>
        /// The modifier that effect the Movement speed of this entity on the <c>MovementComponent</c>
        /// </summary>
        /// <param name="Base">The starting value used for the calculations</param>
        /// <returns></returns>
        public static StatHandle CreateMoveSpeedStat(float Base = 10f) {
            StatHandle MovementSpeedStat = new StatHandle("Move Speed", Base, 0.25f, Mathf.Infinity);
            MovementSpeedStat.Result = () => {
                float PercentageTotal = Base * MovementSpeedStat.Percent;
                return Mathf.Clamp(
                    (MovementSpeedStat.Base + PercentageTotal) * 
                    Mathf.Clamp(MovementSpeedStat.Multiplier, 0.1f, Mathf.Infinity)
                    + MovementSpeedStat.Flat,
                    MovementSpeedStat.Min, MovementSpeedStat.Max);
            };
            return MovementSpeedStat;
        }

        /// <summary>
        /// The modifier that effects the likelihood of a probable event occuring.
        /// </summary>
        /// <param name="Base">The starting value used in calculations</param>
        /// <remarks>Not everything that has a probable chance is influenced by "Luck"</remarks>
        public static StatHandle CreateLuckStat(float Base = 1f) {
            StatHandle MovementSpeedStat = new StatHandle("Luck", Base, 0.25f, Mathf.Infinity);
            MovementSpeedStat.Result = () => {
                float PercentageTotal = Base * MovementSpeedStat.Percent;
                return Mathf.Clamp(
                    (MovementSpeedStat.Base + PercentageTotal) * 
                    Mathf.Clamp(MovementSpeedStat.Multiplier, 0.1f, Mathf.Infinity)
                    + MovementSpeedStat.Flat,
                    MovementSpeedStat.Min, MovementSpeedStat.Max);
            };
            return MovementSpeedStat;
        }
        
    }
}