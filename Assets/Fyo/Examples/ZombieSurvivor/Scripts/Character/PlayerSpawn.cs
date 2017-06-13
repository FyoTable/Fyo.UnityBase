using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    public class PlayerSpawn : Fyo.ObjectPool {
        public Fyo.ObjectPool RespawnPool = null;

        public override GameObject Spawn(Vector3 position, Quaternion rotation, Vector3 scale, bool SetActive = true) {
            GameObject PooledObject = base.Spawn(position, rotation, scale, SetActive);
            PooledObject.GetComponent<Character>().RespawnPool = RespawnPool;

            return PooledObject;
        }

    }
}
