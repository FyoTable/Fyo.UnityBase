using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    [RequireComponent(typeof(Animator))]
    public class Character : MonoBehaviour {
        public CharacterMovement Movement;
        public DamageReceiver Health;
        public Inventory Backpack;
        public Inventory Hands;
        public Inventory Feet;
        public Weapon Weapon;
        public Weapon Kick;

        public Animator animator;

        public GameObject DeathParticles;

        public delegate void AfterlifeDelegate();
        public AfterlifeDelegate Afterlife;

        public Fyo.ObjectPool RespawnPool;

        public Rigidbody Body;
        public CapsuleCollider BodyCollider;

        private void Start() {
            if(animator == null)
                animator = GetComponent<Animator>();
            animator.Rebind();

            if(Health == null)
                Health = GetComponent<DamageReceiver>();
            Health.InflictedDelegate = OnDamageInflicted;
            Health.HealedDelegate = OnDamageHealed;
            Health.DeathDelegate = Death;

            if (Body == null)
                Body = GetComponent<Rigidbody>();
            Body.isKinematic = false;

            if (BodyCollider == null)
                BodyCollider = GetComponent<CapsuleCollider>();
            BodyCollider.enabled = true;
        }

        private void OnDisable() {
            DeathParticles.SetActive(false);
        }

        public void OnDamageInflicted(DamageInfo damage) {
            if (Health.CurrentHealth > 0.0f) {
                animator.SetTrigger("TakeDamage");
            }
        }

        public void OnDamageHealed(DamageInfo damage) {

        }

        public void Death(DamageInfo damage) {
            if (Health.CurrentHealth <= 0.0f) {
                animator.ResetTrigger("TakeDamage");
                animator.SetBool("IsWalking", false);
                animator.SetBool("HasTarget", false);
                animator.SetTrigger("Die");
                BodyCollider.enabled = false;
                Body.isKinematic = true;

                if (DeathParticles != null) {
                    DeathParticles.SetActive(true);
                }

                if(Afterlife != null) {
                    Afterlife();
                }
            }
        }
    }
}
