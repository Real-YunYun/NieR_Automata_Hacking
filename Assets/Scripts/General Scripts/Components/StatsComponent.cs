using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entities.Modules;
using Status;

namespace Entities {
    
    public class StatHandle {
        public string Name;
        public float Base;
            
        public float Flat = 0;
        public float Percent = 0;
        public float Multiplier = 1;

        public float Min = 0;
        public float Max = Mathf.Infinity;

        public Func<float> Result = () => 0;

        public StatHandle(string N, float B, float Min, float Max, float F = 0, float P = 0, float M = 1) {
            Name = N;
            Base = B;

            this.Min = Min;
            this.Max = Max;

            Flat = F;
            Percent = P; 
            Multiplier = Mathf.Clamp(M, 0.01f, Mathf.Infinity);
            Result = () => 0;
        }
    }
    
    public class StatsComponent : EntityModule {
        
        // A bunch of stats in this game my gawd....
        private Dictionary<int, StatHandle> Stats = new Dictionary<int, StatHandle>();

        #region Delegates and Events

        public delegate void OnStatsChangedDelegate(StatsComponent Comp);

        public event OnStatsChangedDelegate OnStatsChanged;
        

        #endregion
        
        private new void Awake() {
            base.Awake();
            // All the Entries of Stats found on an entity
            AddStat(StatFactory.CreateMoveSpeedStat());
            AddStat(StatFactory.CreateFireRateStat());
        }

        /// <summary>
        /// Adds a stat to this component, being able to be affected by external factors
        /// </summary>
        /// <param name="NewStat">Name of the Stat that would be added to the registry</param>
        /// <param name="Base">The starting amount this stat will use for it's calculations</param>
        /// <returns></returns>
        public StatsComponent AddStat(string NewStat, float Base, float Min, float Max) {
            int Check = Animator.StringToHash(NewStat);
            Stats.TryAdd(Check, new StatHandle(NewStat, Base, Min, Max));
            return this;
        }
        
        public StatsComponent AddStat(StatHandle NewStat) {
            int Check = Animator.StringToHash(NewStat.Name);
            Stats.TryAdd(Check, NewStat);
            return this;
        }

        public float GetStatResult(string StatName) {
            Stats.TryGetValue(Animator.StringToHash(StatName), out StatHandle Stat);
            return Stat.Result();
        }


    }
    
}