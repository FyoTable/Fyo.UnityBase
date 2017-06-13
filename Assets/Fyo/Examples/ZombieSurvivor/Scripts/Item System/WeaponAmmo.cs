using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    public class WeaponAmmo : DamageDealer {
        public float Duration = 1.0f;
        float Elapsed = 0.0f;
        
        private void Start() {
            Elapsed = 0.0f;
        }
        private void OnEnable() {
            Elapsed = 0.0f;
        }

        private void Update() {
            if ((Elapsed += Time.deltaTime) >= Duration) {
                gameObject.SetActive(false);
                Elapsed = 0.0f;
            }
        }
    }
}
