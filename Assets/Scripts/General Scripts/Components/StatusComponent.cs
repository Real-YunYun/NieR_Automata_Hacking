using System;
using System.Collections;
using System.Collections.Generic;
using Entities.Modules;
using Entities.Projectiles;
using UnityEngine;
using Entities.Status;

namespace Entities {

    public class StatusComponent : EntityModule {

        [SerializeReference, SerializeField, NonReorderable]
        private List<StatusInfliction> Inflictions = new List<StatusInfliction>();
        
        #region Events & Delegates

        // These are for Other items or interactions in the game, NOT FOR StatusInflictions
        
        public delegate void OnStatusInflictedDelegate(StatusInfliction Status, Entity Affected, StatusComponent Comp);
        public event OnStatusInflictedDelegate OnStatusInflicted;
        
        public delegate void OnStatusTickDelegate(StatusInfliction Status, Entity Affected, StatusComponent Comp);
        public event OnStatusTickDelegate OnStatusTick;
        
        public delegate void OnStatusRelievedDelegate(StatusInfliction Status, Entity Affected, StatusComponent Comp);
        public event OnStatusRelievedDelegate OnStatusRelieved;

        #endregion

        private void Update() {
            if (Inflictions.Count == 0) return; // No Status to calculate

            List<StatusInfliction> RemovingInflictions = new List<StatusInfliction>();
            for (int i = 0; i < Inflictions.Count; i++) {
                // Debug.Log("Solving Status: " + Inflictions[i].GetStatusDetails());
                // If it doesn't expire, don't do anything
                if (Inflictions[i].StatusExpires) {
                    // If it does expire then calculate it's time
                    Inflictions[i].StatusInflictionTime -= Time.deltaTime;
                    if (Inflictions[i].StatusInflictionTime <= 0) {
                        RemovingInflictions.Add(Inflictions[i]);
                    }
                }

                // Calculate the Tick of the Status
                if (Inflictions[i].CanTick) {
                    Inflictions[i].TickTime -= Time.deltaTime;
                    if (Inflictions[i].TickTime <= 0) {
                        Inflictions[i].TickTime = Inflictions[i].TickDuration;
                        Inflictions[i].OnStatusTick();
                        if (OnStatusTick != null) OnStatusTick(Inflictions[i], Owner, this);
                    }
                }
            }

            foreach (StatusInfliction Remove in RemovingInflictions) {
                if (Inflictions.Contains(Remove)) {
                    Remove.OnStatusRelieved();
                    Inflictions.Remove(Remove);
                }
            }
        }

        public void ApplyStatus(string StatusName, HitResult Hit) {
            Type StatusType = Type.GetType(StatusName);
            if (!DoesStatusExist(StatusName)) return;
            
            // Creation of our Status!
            StatusInfliction NewInfliction = Activator.CreateInstance(StatusType) as StatusInfliction;
            if (NewInfliction == null) return;
            NewInfliction.HitResult = Hit;
            
            // Filter and see if this status "CanStack" or what else is happening
            foreach (StatusInfliction CheckStatus in Inflictions) {
                // Both status' are the same so we have one, call it's "Inflicted" again
                if (CheckStatus.Name == NewInfliction.Name) {
                    CheckStatus.OnStatusInflicted();
                    if (OnStatusInflicted != null) OnStatusInflicted(CheckStatus, Owner, this);
                    return;
                }
            }
            
            // Didn't find a status of this type so just add it and call it's "Inflicted"
            NewInfliction.StatusComp = this;
            Inflictions.Add(NewInfliction);
            NewInfliction.OnStatusInflicted();
        }

        public void ApplyStatus(StatusInfliction Status) {
            foreach (var Check in Inflictions) {
                if (Status.Name == Check.Name) {
                    Check.OnStatusInflicted();
                    return;
                }
            }
            
            Status.StatusComp = this;
            Inflictions.Add(Status);
            Status.OnStatusInflicted();
        }

        public void RelieveStatus(string StatusName) {
            if (!DoesStatusExist(StatusName)) return;
            for (int i = 0; i < Inflictions.Count; i++) {
                if (Inflictions[i].Name == StatusName) {
                    Inflictions[i].OnStatusRelieved();
                    if (OnStatusRelieved != null) OnStatusRelieved(Inflictions[i], Owner, this);
                    Inflictions.RemoveAt(i);
                    return;
                }
            }
        }
        
        public void RelieveStatus(StatusInfliction Status) {
            foreach (var Check in Inflictions) {
                if (Status.Name == Check.Name) {
                    Check.OnStatusRelieved();
                    if (OnStatusRelieved != null) OnStatusRelieved(Check, Owner, this);
                    Inflictions.Remove(Check);
                    return;
                }
            }
        }

        #region Helpers

        public static bool DoesStatusExist(string StatusName) {
            Type StatusType = Type.GetType(StatusName);
            if (StatusType == null) return false;
            return true;
        }

        public StatusInfliction GetInfliction(string StatusName) {
            foreach (StatusInfliction Status in Inflictions) {
                if (Status.Name == StatusName) {
                    return Status;
                }
            }
            
            return null;
        }

        public int GetUniqueInflictionCount() => Inflictions.Count;

        #endregion

    }
    
}