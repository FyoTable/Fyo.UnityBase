using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarqueeLayerPanel : MonoBehaviour {
    public int Layer = 0;
    MarqueeMenu menu = null;
    public Animator animator;

	// Use this for initialization
	void Start () {
        menu = FindObjectOfType<MarqueeMenu>();
        animator = GetComponent<Animator>();
	}

    public void AA_MoveTowardBackground() {
        if (Layer == menu.LayerObjects.Count - 1) {
            //Move To Front
            Layer = 0;
        } else {
            //Move Layer Backward
            Layer++;
        }

        transform.SetParent(menu.LayerObjects[Layer].transform);
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
    }

    public void AA_MoveTowardForeground() {
        if (Layer == 0) {
            //Move to Back
            Layer = menu.LayerObjects.Count - 1;
        } else {
            //Move Layer Forward
            Layer--;
        }

        transform.SetParent(menu.LayerObjects[Layer].transform);
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
    }
}
