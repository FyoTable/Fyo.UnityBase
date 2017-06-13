using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    [RequireComponent(typeof(Light))]

    public class ZombieLight : MonoBehaviour {
        public List<Fyo.ObjectPool> ZombiePools = new List<Fyo.ObjectPool>();

        public int MaxDensity = 20;
        public int Density = 0;
        new Light light;

        public Color SafeColor = Color.green * 0.6f;
        public Color DangerColor = Color.red * 0.8f;
        public float LerpFloat = 0.0f;

        private void Start() {
            light = GetComponent<Light>();
        }

        public Color newColor = Color.white;
        // Update is called once per frame
        void Update() {
            if(ZombiePools.Count > 0) {
                Density = 0;
                for(int p = 0; p < ZombiePools.Count; p++) {
                    Density += ZombiePools[p].Count - (ZombiePools[p].Available);
                }

                if (MaxDensity <= 0)
                    MaxDensity = 1;

                light.intensity = 1 + (Mathf.Min((float)MaxDensity, (float)Density) / (float)MaxDensity);
                LerpFloat = (float)Density / (float)MaxDensity;
                newColor = Color.LerpUnclamped(SafeColor, DangerColor, LerpFloat);
                light.color = newColor;
            }
        }
    }
}