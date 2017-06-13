using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    public class Weapon : Item {
        public WeaponAmmoPool Ammo;
        public float RefireRate = 1.0f;
        public float ReloadRate = 2.5f;
        public System.DateTime RefireTime = System.DateTime.Now;
        public string AnimationStr = "Attack";
        public string ReloadStr = "Reload";

        private void Start() {
            Ammo = GetComponent<WeaponAmmoPool>();
            if(Ammo == null) {
                Ammo = gameObject.AddComponent<WeaponAmmoPool>();
            }
        }
        Animator animator = null;
        public override bool Use(GameObject user, int quantity = 0) {
            if(System.DateTime.Now >= RefireTime) {
                animator = user.GetComponent<Animator>();

                if (Ammo.Use(user, quantity)) {
                    Fire();
                    return true;
                } else {
                    Reload();
                }

            }

            animator = null;
            return false;
        }

        protected virtual void Fire() {
            //Fire
            RefireTime = System.DateTime.Now.AddSeconds(RefireRate);
            if (animator != null) {
                animator.SetTrigger(AnimationStr);
            }
        }

        public virtual void Reload() {
            //Reload
            if (animator != null) {
                animator.SetTrigger(ReloadStr);
            }

            RefireTime = System.DateTime.Now.AddSeconds(ReloadRate);
        }

        public virtual void Empty() {

        }

        public virtual void Unload() {

        }

        public virtual void Load(WeaponAmmo newAmmo) {

        }

        public void Equip(Character character) {
        }
    }
}