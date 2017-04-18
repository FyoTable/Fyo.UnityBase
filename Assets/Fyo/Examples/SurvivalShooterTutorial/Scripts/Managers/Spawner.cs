using UnityEngine;
using Fyo;

namespace SurvivalShooterExampleGame {
    public class Spawner : ObjectPool {
        public float spawnTime = 3f;            // How long between each spawn.
        public Transform[] spawnPoints;         // An array of the spawn points this enemy can spawn from.
        
        void Awake () {
            // Call the Spawn function after a delay of the spawnTime and then continue to call after the same amount of time.
            InvokeRepeating ("Spawn", spawnTime, spawnTime);
        }
        
        void Spawn () {
            // If the player has no health left...
            if(!gameObject.activeSelf) {
                return;
            }

            Transform SpawnTransform = transform;
            if (spawnPoints != null && spawnPoints.Length > 0) {
                // Find a random index between zero and one less than the number of spawn points.
                int spawnPointIndex = Random.Range(0, spawnPoints.Length);

                SpawnTransform = spawnPoints[spawnPointIndex];
                if (SpawnTransform != null && SpawnTransform.gameObject != null) {
                    SphereCollider sphere = SpawnTransform.gameObject.GetComponent<SphereCollider>();
                    if (sphere != null && sphere.isTrigger) {
                        //Spawn within Sphere trigger volume
                        float Angle = UnityEngine.Random.Range(0.0f, 180.0f);
                        float Distance = sphere.radius * UnityEngine.Random.Range(-1.0f, 1.0f);

                        Vector3 pos = SpawnTransform.position + (SpawnTransform.rotation * Quaternion.AngleAxis(Angle, SpawnTransform.up)) * (Vector3.forward * Distance);
                        Spawn(pos, SpawnTransform.rotation, SpawnTransform.localScale);
                        return;
                    }
                } else {
                    SpawnTransform = transform;
                }
            }

            //Spawn a pooled object at the determined spawn transform.
            Spawn(SpawnTransform.position, SpawnTransform.rotation, SpawnTransform.localScale);
        }
    }
}
