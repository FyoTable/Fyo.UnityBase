using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ZombieSurvivor {
    public class DamageInfo : MonoBehaviour {
        public float Damage;
        public DamageType Type;
        public GameObject Source;
        
        public DamageInfo() : base() {
            Damage = 0.0f;
            Type = DamageType.Basic;
            Source = null;
        }

        public DamageInfo(float d, DamageType dt, GameObject s = null) : base() {
            Damage = d;
            Type = dt;
            Source = s;
        }
    }
}
