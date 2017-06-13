using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    public class Spawner : Fyo.ObjectPool {
        public float SpawnRadius = 1.0f;
        public float SpawnRate = 1.0f;
        System.DateTime NextSpawnTime = System.DateTime.Now;
        
        void Update() {
            if(System.DateTime.Now >= NextSpawnTime &&
                NextSpawnIdx >= 0) {
                Vector3 SpawnPosition = new Vector3(
                    gameObject.transform.position.x + Random.Range(-SpawnRadius, SpawnRadius), 
                    gameObject.transform.position.y,
                    gameObject.transform.position.z + Random.Range(-SpawnRadius, SpawnRadius)
                );

                UnityEngine.AI.NavMeshHit nmHit;
                if (UnityEngine.AI.NavMesh.SamplePosition(SpawnPosition, out nmHit, 100, 1)) {
                    //Debug.Log("Spawning On NavMesh");
                    GameObject go = Spawn(nmHit.position, transform.rotation, transform.localScale, false);
                    if(go == null) {
                        if (Finite)
                            Debug.LogWarning(name + " is Finite and has run out of objects");
                        else
                            Debug.LogError("Error Spawning Pooled Object " + NextSpawnIdx.ToString());
                    } else
                        go.SetActive(true); 
                } else {
                    Debug.Log("Spawning off NavMesh");
                    GameObject go = Spawn(SpawnPosition, transform.rotation, transform.localScale);
                    if (go == null) {
                        if (Finite)
                            Debug.LogWarning(name + " is Finite and has run out of objects");
                        else
                            Debug.LogError("Error Spawning Pooled Object " + NextSpawnIdx.ToString());
                    } else
                        go.SetActive(true);
                }

                NextSpawnTime = System.DateTime.Now.AddSeconds(SpawnRate);
            }
        }
        
        private void OnDrawGizmos() {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, SpawnRadius);
        }
    }
}