using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items.Threads.Conditional;
using Items.Threads;
using Entities;
using Entities.Projectiles;
using Entities.Status;
using Status;

namespace Items {

    public class DeathMark : Conditional {

        [Header("Death Mark Parameters")] 
        private ShootingComponent ShootingComp;
        
        // Start is called before the first frame update
        private new void Awake() {
            base.Awake();
            
            Name = "Death Mark";
            Description = "Apply Weakness to an Enemy after 4 unique statuses are inflicted";
            Sprite = "Player/UI Images/None";
            Duration = 5f;
            Cooldown = 0f;
            Upkeep = 0f;
            
            Type = EThreadBehaviour.Conditional;
        }

        private void OnEnable() {
            if (Owner == null) return;
            if (!Owner.TryGetComponent(out ShootingComp)) return;

            ShootingComp.OnFireEnded += OnFire;
        }

        private void OnDisable() {
            if (Owner == null) return;
            if (ShootingComp == null) return;
            
            ShootingComp.OnFireEnded -= OnFire;
        }

        private void OnFire(Projectile Proj) {
            Proj.OnProjectileHit += OnProjHit;
        }

        private void OnProjHit(Entity Hit) {
            if (!Hit.TryGetComponent(out StatusComponent StatusComp)) return;
            StatusComp.ApplyStatus("Status.Marked", new HitResult(Owner, Hit));
        }
        
    }
}

/*
 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Items.Threads.Conditional;
using Items.Threads;
using Entities;
using Entities.Projectiles;
using Entities.Status;

namespace Items {

    public class DeathMark : Conditional {

        [System.Serializable]
        private struct MarkedDebuff {
            public float Upkeep;
            public Entity Affected;
            public int StatusCount;
            public GameObject Effect;
            public bool Applied;

            public MarkedDebuff(Entity E) {
                Upkeep = 0;
                StatusCount = 0;
                Affected = E;
                Effect = null;
                Applied = false;
            }
        };

        [Header("Death Mark Parameters")] 
        private ShootingComponent ShootingComp;
        [SerializeField] private GameObject EffectPrefab;
        [SerializeField] private List<MarkedDebuff> Inflictions = new List<MarkedDebuff>();
        
        
        // Start is called before the first frame update
        private new void Awake() {
            base.Awake();
            
            Name = "Death Mark";
            Description = "Apply Weakness to an Enemy after 4 unique statuses are inflicted";
            Sprite = "Player/UI Images/None";
            Duration = 5f;
            Cooldown = 0f;
            Upkeep = 0f;
            
            Type = EThreadBehaviour.Conditional;
            EffectPrefab = Resources.Load<GameObject>("Effects/Weakness");
        }

        private void OnEnable() {
            if (Owner == null) return;
            if (!Owner.TryGetComponent(out ShootingComp)) return;

            ShootingComp.OnFireEnded += OnFire;
        }

        private void OnDisable() {
            if (Owner == null) return;
            if (ShootingComp == null) return;
            
            ShootingComp.OnFireEnded -= OnFire;
        }


        // Update is called once per frame
        void Update() {
            if (Inflictions.Count == 0) return;

            List<Entity> IndicesRemove = new List<Entity>();
            for (int i = 0; i < Inflictions.Count; i++) {
                MarkedDebuff Status = Inflictions[i];
                Status.Upkeep += Time.deltaTime;
                Inflictions[i] = Status;
                if (Status.Upkeep > 5) IndicesRemove.Add(Inflictions[i].Affected);
            }

            foreach (Entity E in IndicesRemove) {
                for (int i = 0; i < Inflictions.Count; i++) {
                    if (Inflictions[i].Affected == E) {
                        Destroy(Inflictions[i].Effect);
                        Debug.Log("Removed: " + Inflictions[i].Affected.name);
                        Inflictions.Remove(Inflictions[i]);
                    }
                }
            }
        }

        private void OnFire(Projectile Proj) {
            Proj.OnProjectileHit += OnProjHit;
        }

        private void OnProjHit(Entity Hit) {
            for (int i = 0; i < Inflictions.Count; i++) {
                if (Inflictions[i].Affected == Hit) {
                    MarkedDebuff Status = new MarkedDebuff(Hit) {
                        StatusCount = Inflictions[i].StatusCount + 1, // Add Stack
                        Upkeep = 0, // Reset the counter
                        Effect = Inflictions[i].Effect,
                        Applied = Inflictions[i].Applied
                    };

                    if (Status.StatusCount == 4) {
                        if (!Inflictions[i].Applied) {
                            Status.Applied = true;
                            Status.Effect = Instantiate(EffectPrefab, Status.Affected.transform);
                            OnDebuffApplied(Status.Affected);
                        }
                    }
                    
                    Inflictions[i] = Status;
                    return;
                }
            }
            
            AddInfliction(Hit);
        }

        private void AddInfliction(Entity Hit) {
            MarkedDebuff Inflict = new MarkedDebuff(Hit) { StatusCount = 1 };
            Inflictions.Add(Inflict);
        }

        private void OnDebuffApplied(Entity Affected) {
            StatusComponent Comp;
            if (!TryGetComponent(out Comp)) return;
            Comp.ApplyStatus("Status.Weakness");
        }
        
    }
}

 */
