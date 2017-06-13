using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    [RequireComponent(typeof(SphereCollider))]
    public class DamageTriggerVolume : WeaponAmmo {
        public List<string> IgnoreTags = new List<string>();
        SphereCollider sphereCollider;
        public float KineticForce = 0.0f;

        private void Start() {
            if(sphereCollider == null)
                sphereCollider = GetComponent<SphereCollider>();
        }

        private void OnEnable() {
        }

        private void OnTriggerEnter(Collider other) {
            for (int i = 0; i < IgnoreTags.Count; i++) {
                if (other.CompareTag(IgnoreTags[i]))
                    return;
            }

            DealDamage(other.gameObject);

            if (KineticForce != 0.0f) {
                Rigidbody otherBody = other.gameObject.GetComponent<Rigidbody>();
                if (otherBody != null) {
                    otherBody.AddForce(transform.root.forward * KineticForce * otherBody.mass, ForceMode.Impulse);
                }
            }

            //Debug.Log("Damaged " + other.name + " " + Damage.Damage);
        }

        private void OnTriggerStay(Collider other) {
            
        }

        private void OnTriggerExit(Collider other) {
            
        }

        private void OnDrawGizmos() {
            if(sphereCollider == null)
                sphereCollider = GetComponent<SphereCollider>();

            Gizmos.color = Color.Lerp(Color.red, Color.yellow, 0.5f);
            Gizmos.DrawWireSphere(transform.position + sphereCollider.center, sphereCollider.radius);
        }
    }
}