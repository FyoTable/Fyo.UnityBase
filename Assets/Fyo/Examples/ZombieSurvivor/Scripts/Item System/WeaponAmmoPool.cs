using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    public class WeaponAmmoPool : Fyo.ObjectPool {
        protected WeaponAmmo ammoType;
        
        protected override void Start() {
            ammoType = Template.GetComponent<WeaponAmmo>();
            base.Start();
        }

        public bool Use(GameObject user, int amount) {
            return Use(user, amount, transform.position, Quaternion.identity, Vector3.one);
        }

        public bool Use(GameObject user, int amount, Vector3 position) {
            return Use(user, amount, position, Quaternion.identity, Vector3.one);
        }

        public bool Use(GameObject user, int amount, Vector3 position, Quaternion rotation) {
            return Use(user, amount, position, rotation, Vector3.one);
        }

        public bool Use(GameObject user, int amount, Vector3 position, Quaternion rotation, Vector3 scale) {
            if (Entries.Length <= amount)
                return false;

            GameObject go = null;
            WeaponAmmo ammo = null;
            if (amount > 0) {
                //Projectiles
                for (int p = 0; p < amount; p++) {
                    go = Spawn(position, rotation, scale);
                    ammo = go.GetComponent<WeaponAmmo>();
                    ammo.Damage.Source = user;
                }
            } else {
                //Melee
                go = Spawn(position, rotation, scale);
                ammo = go.GetComponent<WeaponAmmo>();
                ammo.Damage.Source = user;
                ammo.transform.parent = transform;
            }

            if(go != null) {
                go.SetActive(true);
                go.transform.parent = transform;
            }

            return (go != null);
        }
    }
}
