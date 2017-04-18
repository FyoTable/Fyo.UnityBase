using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Fyo {
    public class AutosizeGridLayoutGroup : MonoBehaviour {
        GridLayoutGroup gridLayoutGroup;
        RectTransform rect;
        public float height;
        public int cellCount = 2;

        void Start() {
            gridLayoutGroup = GetComponent<GridLayoutGroup>();
            rect = GetComponent<RectTransform>();

            gridLayoutGroup.cellSize = new Vector2(rect.rect.height, rect.rect.height);
            cellCount = GetComponentsInChildren<RectTransform>().Length;
        }

        void OnRectTransformDimensionsChange() {
            if (gridLayoutGroup != null && rect != null) {
                Vector2 Cell = gridLayoutGroup.cellSize;
                if (rect.rect.height < rect.rect.width) {
                    if (rect.rect.width > 0.0f) {
                        //Wide Screen
                        float ratio = rect.rect.height / rect.rect.width;
                        Cell.y = Cell.x = (rect.rect.width / cellCount) * ratio;
                    }
                }

                gridLayoutGroup.cellSize = Cell;
            }
        }
    }
}
