using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    [RequireComponent(typeof(DamageInfo))]
    public class DamageDealer : MonoBehaviour {
        public DamageInfo Damage;

        private void Start() {
            if(Damage == null)
                Damage = GetComponent<DamageInfo>();
        }

        protected void DealDamage(GameObject target) {
            DamageReceiver receiver = target.gameObject.GetComponent<DamageReceiver>();
            if (receiver != null) {
                receiver.Damage(Damage);
            }
        }
    }
}