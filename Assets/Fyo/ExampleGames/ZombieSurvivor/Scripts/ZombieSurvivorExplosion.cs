using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ZombieSurvivorExplosion : MonoBehaviour {
    public Sprite Visual;
    public CircleCollider2D Collider;
    public int Damage = 100;

    public List<string> IgnoreTags = new List<string>();

    private void Start() {
        Collider = GetComponent<CircleCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (!IgnoreTags.Contains(collision.gameObject.tag)) {
            collision.gameObject.SendMessage("Damage", Damage, SendMessageOptions.DontRequireReceiver);
        }
    }
}
