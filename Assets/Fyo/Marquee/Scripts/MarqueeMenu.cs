using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarqueeMenu : MonoBehaviour {
    public bool AutoCycle = false;
    public float AutoCycleFrequency = 5.0f;
    float NextAutoCycleTime = 0.0f;

    float NextActiveTime = 0.0f;

    void Awake() {
        NextAutoCycleTime = AutoCycleFrequency;

        //Read Configuration
    }

    void Update() {
        if (AutoCycle && Time.time >= NextAutoCycleTime) {
            NextAutoCycleTime += AutoCycleFrequency;
            Cycle();
        }
    }

    protected int NextPanel = 0;
    protected MarqueeLayerPanel CurrentPanel = null;
    protected MarqueeLayerPanel PreviousPanel = null;
    public List<MarqueeLayerPanel> Panels = new List<MarqueeLayerPanel>();
    public void Cycle() {
        if (Panels.Count == 0) {
            Debug.LogError("No Panels Present");
            return;
        }

        if (CurrentPanel == null) {
            //No Current Panel, this is our first
            CurrentPanel = Panels[0];
            NextPanel = 1;
        } else {
            //Fade out current panel
            PreviousPanel = CurrentPanel;
            PreviousPanel.Fade(false);
            //Fade in next panel
            CurrentPanel = Panels[NextPanel];
            CurrentPanel.Fade(true);
            NextPanel++;
        }

        if (NextPanel >= Panels.Count) {
            NextPanel = 0;
        }
    }
}
