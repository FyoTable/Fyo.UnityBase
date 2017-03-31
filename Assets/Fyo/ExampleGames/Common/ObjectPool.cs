using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour {
    public GameObject Template;
    public int Count = 1;
    protected GameObject[] Entries;
    protected int EntryPointer = 0;
    protected GameObject t;

    private void Start() {
        Entries = new GameObject[Count];
        for (EntryPointer = 0; EntryPointer < Count; EntryPointer++) {
            t = Entries[EntryPointer] = Instantiate(Template);
            t.SetActive(false);
        }
        t = null;
        EntryPointer = 0;
    }

    public GameObject GetNext(Vector3 Position, Quaternion Rotation) {
        EntryPointer++;
        if (EntryPointer > Entries.Length)
            EntryPointer = 0;

        t = Entries[EntryPointer];

        if (t.activeSelf) {
            Debug.Log("[Pool \"" + name + "\"] Pooled Object \"" + t.name + "\" is already active, consider increasing pool size.");
            t.SetActive(false);
        }

        t.transform.position = Position;
        t.transform.rotation = Rotation;
        t.SetActive(true);

        return t;
    }
}
