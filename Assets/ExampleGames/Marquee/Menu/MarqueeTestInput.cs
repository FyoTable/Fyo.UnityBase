using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarqueeTestInput : MonoBehaviour {
    public MarqueeMenu menu;
    void Update() {
        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            menu.CycleBackward();
        } else if (Input.GetKeyDown(KeyCode.DownArrow)) {
            menu.CycleForward();
        }

        if (Input.GetKeyDown(KeyCode.M)) {
            MarqueeLayerPanel panel;
            for (int i = 0; i < menu.LayerObjects.Count; i++) {
                panel = menu.LayerObjects[i].GetComponentInChildren<MarqueeLayerPanel>();
                if (panel != null && panel.name == "Game Select") {
                    menu.CyclePanelToFront(i);
                }
            }
        }
    }
}
