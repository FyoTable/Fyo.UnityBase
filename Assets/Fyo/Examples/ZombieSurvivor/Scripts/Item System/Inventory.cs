using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    public class Inventory : MonoBehaviour {
        int itemIdx = 0;

        public List<Item> Contents = new List<Item>();

        public void AddContents(Item item) {
            AddContents(new List<Item>() { item });
        }

        public void AddContents(List<Item> items) {
            List<Item> AddItems = items;
            if(items.Count > 0) {
                for(int i = 0; i < items.Count; i++) {
                    if(Contents.Contains(items[i])) {
                        AddItems.Remove(items[i]);
                        return;
                    }
                }
            } 
            Contents.AddRange(AddItems);
        }

        public Item GetItem(int itemId) {
            return (Contents.Count < itemId) ? Contents[itemId] : null;
        }

        public Item RemoveItem(int itemId) {
            Item item = null;
            if(Contents.Count < itemId) {
                item = Contents[itemId];
                return Contents[itemId];
            }
            return item;
        }

        public void RemoveItem(Item item) {
            if(Contents.Contains(item)) {
                Contents.Remove(item);
            }
        }
    }
}