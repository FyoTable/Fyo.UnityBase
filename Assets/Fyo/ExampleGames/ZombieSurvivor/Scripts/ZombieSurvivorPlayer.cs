using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class ZombieSurvivorPlayer : GamePlayer {
    protected Animator anim;
    protected Rigidbody2D body;
    protected CircleCollider2D coll;
    protected int health = 100;

    public float RotationRate = 45.0f;
    public float MoveSpeed = 1.0f;

    public ZombieSurvivorWeapon Weapon;

    protected bool Controllable = false;
    
    private void Start() {
        if (Weapon == null)
            Weapon = GetComponentInChildren<ZombieSurvivorWeapon>();

        anim = GetComponent<Animator>();
        body = GetComponent<Rigidbody2D>();
        coll = GetComponent<CircleCollider2D>();
    }

    private void Awake() {
        Controllable = true;
    }

    protected void Fire() {
        if (Weapon != null) {
            Weapon.Fire();
        }
    }

    protected void Die() {
        //TODO: Play death animation, throw weapon, disable control, then respawn
        anim.SetTrigger("Die");
    }

    public void Damage(int damage) {
        health -= damage;

        if (health <= 0) {
            Die();
        } else {
            anim.SetTrigger("Injured");
        }
    }

    protected virtual void UpdateInput() {
        if (Gamepad != null) {
            body.MoveRotation(body.rotation + (Gamepad.GetAxis(0) * Time.deltaTime));

            if (!Mathf.Approximately(Gamepad.GetAxis(1), 0.0f)) {
                body.MovePosition(transform.position + (transform.right * Gamepad.GetAxis(1) * Time.deltaTime));
                anim.SetFloat("Walk", Gamepad.GetAxis(1));
            } else {
                anim.SetFloat("Walk", 0);
            }

            if (Gamepad.GetButton(0))
                Fire();

            if (Gamepad.GetButton(1)) {
                //Throw grenade, use item, whatever?
            }
        }
    }
}
