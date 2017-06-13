using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fyo {
    public class RandomSound : MonoBehaviour {
        protected float Frequency = 0.0f;
        public float FrequencyMin = 1.0f;
        public float FrequencyMax = 16.0f;
        public Vector2 PitchRange = new Vector2(0.85f, 1.1f);
        protected float Elapsed = 0.0f;
        public List<AudioSource> Sounds = new List<AudioSource>();
        public bool SingleShot = false;

        void Start() {
            Frequency = Random.Range(FrequencyMin, FrequencyMax);
        }

        AudioSource sound;
        IEnumerator FinishSingleShot() {
            yield return new WaitForSeconds(sound.clip.length);
            DestroyImmediate(this);
        }

        // Update is called once per frame
        void Update() {
            Elapsed += Time.deltaTime;
            if (Elapsed > Frequency) {
                if (Sounds.Count > 0) {
                    sound = Sounds[Random.Range(0, Sounds.Count)];
                    sound.pitch = Random.Range(PitchRange.x, PitchRange.y);
                    sound.Play();
                }
                Frequency = Random.Range(FrequencyMin, FrequencyMax);

                if (SingleShot)
                    StartCoroutine(FinishSingleShot());
                else
                    Elapsed = 0.0f;
            }
        }
    }
}