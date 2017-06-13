using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour { 
    public struct Information {
        public string Name;
        public string Description;
        public long stack;

        public Information(string n, string d, long stacksize) {
            Name = n;
            Description = d;
            stack = stacksize;
        }
    }

    public virtual bool Use(GameObject user, int quantity) {
        return true;
    }
}
