using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarqueeMenu : MonoBehaviour {
    public enum CycleDirectionEnum {
        Forward,
        Backward
    }

    public bool AutoCycle = false;
    public float AutoCycleFrequency = 5.0f;
    public CycleDirectionEnum Direction = CycleDirectionEnum.Forward;
    float NextAutoCycleTime = 0.0f;

    float NextActiveTime = 0.0f;

    void Awake() {
        NextAutoCycleTime = AutoCycleFrequency;
    }

    void Update() {
        if (AutoCycle && Time.time >= NextAutoCycleTime) {
            NextAutoCycleTime += AutoCycleFrequency;

            switch (Direction) {
                default:
                case CycleDirectionEnum.Forward:
                    CycleForward();
                    break;
                case CycleDirectionEnum.Backward:
                    CycleBackward();
                    break;
            }
        }
    }

    public List<GameObject> LayerObjects = new List<GameObject>();
    public void CycleForward() {
        MarqueeLayerPanel panel = null;
        for (int i = 0; i < LayerObjects.Count; i++) {
            if((panel = LayerObjects[i].GetComponentInChildren<MarqueeLayerPanel>()) != null) {
                //Non empty panels should animate, which triggers a parenting move
                if (i == 0) {
                    panel.animator.SetTrigger("CycleToBack");
                } else {
                    panel.animator.SetTrigger("MoveForward");
                }
            }
        }
    }

    public void CycleBackward() {
        if (Time.time >= NextActiveTime) {
            NextActiveTime = Time.time + 0.5f;
            MarqueeLayerPanel panel = null;
            for (int i = 0; i < LayerObjects.Count; i++) {
                if ((panel = LayerObjects[i].GetComponentInChildren<MarqueeLayerPanel>()) != null) {
                    //Non empty panels should animate, which triggers a parenting move
                    if (i == LayerObjects.Count - 1) {
                        panel.animator.SetTrigger("CycleToFront");
                    } else {
                        panel.animator.SetTrigger("MoveBackward");
                    }
                }
            }
        }
    }

    public IEnumerator MovePanelToFront(object panelObj) {
        MarqueeLayerPanel panel = (MarqueeLayerPanel)panelObj;
        while (panel.Layer != 0) {
            CycleForward();
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void CyclePanelToFront(int idx) {
        if (Time.unscaledTime >= NextActiveTime) {
            NextActiveTime = Time.unscaledTime + 0.5f;
            if (idx < LayerObjects.Count && idx >= 0.0f) {
                StartCoroutine("MovePanelToFront", LayerObjects[idx].GetComponentInChildren<MarqueeLayerPanel>());
            } else {
                Debug.LogError("Invalid Panel index " + idx.ToString("G20"));
            }
        }
    }
}
