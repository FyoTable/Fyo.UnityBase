using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    public class DamageReceiver : MonoBehaviour {
        public float BaseHealth = 1.0f;
        public float CurrentHealth = 1.0f;

        public delegate void DamageInflictedDelegate(DamageInfo damage);
        public delegate void DamageHealedDelegate(DamageInfo damage);
        public delegate void FatalDamageDelegate(DamageInfo damage);

        public DamageInflictedDelegate InflictedDelegate = null;
        public DamageHealedDelegate HealedDelegate = null;
        public FatalDamageDelegate DeathDelegate = null;

        private void OnEnable() {
            CurrentHealth = BaseHealth;
        }

        public void Damage(DamageInfo damage) {
            if(damage.Source != gameObject) {
                if(CurrentHealth > 0.0f) {
                    CurrentHealth -= damage.Damage;

                    if (damage.Damage > 0)
                        if (InflictedDelegate != null)
                            InflictedDelegate(damage);
                        else
                            Debug.LogWarning("No Damage Inflicted Delegate");
                    else
                        if (HealedDelegate != null)
                        HealedDelegate(damage);
                    else
                        Debug.LogWarning("No Damage Healed Delegate");

                    if (CurrentHealth <= 0.0f) {
                        Debug.Log(name + " fatal damage from " + damage.Source.name);
                        if (DeathDelegate != null)
                            DeathDelegate(damage);
                    }
                } else {
                    CurrentHealth -= damage.Damage;
                    //Already Dead Things
                }
            }
        }
    }
}
