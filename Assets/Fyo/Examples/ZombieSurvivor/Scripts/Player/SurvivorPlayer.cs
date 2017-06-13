using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    [RequireComponent(typeof(Character))]
    public class SurvivorPlayer : Fyo.FyoPlayer {
        public Character character;

        private void Start() {
            character = GetComponent<Character>();
            character.Afterlife = Afterlife;
        }

        private void FixedUpdate() {
            if (Gamepad != null) {
                Vector3 MovementAxis = new Vector3(Gamepad.GetAxis("axis 0"), 0.0f, -Gamepad.GetAxis("axis 1"));
                if (MovementAxis.magnitude > 0) {
                    character.Movement.Move = new Vector3(
                        MovementAxis.x,
                        0.0f,
                        MovementAxis.z
                    ).normalized;
                    character.transform.LookAt(character.transform.position + character.Movement.Move);
                } else
                    character.Movement.Move = Vector3.zero;

                if(Gamepad.GetButton("button 0")) {
                    //Kick precedes weapon use
                    if (character.Kick)
                        character.Kick.Use(gameObject);
                        //character.animator.SetTrigger("Kick");
                } else if(Gamepad.GetButton("button 1")) {
                    //Use Weapon
                    if (character.Weapon)
                        character.Weapon.Use(gameObject);
                        //character.animator.SetTrigger("Attack");
                }
            } else {
                character.Movement.Move = Vector3.zero; 
            }
        }

        public void TakeDamage(DamageInfo damage) {
            character.animator.SetTrigger("TakeDamage");
        }

        public void HealDamage(DamageInfo health) {

        }
        
        IEnumerator PassIntoTheGreatBeyond() {
            yield return new WaitForSeconds(2.5f);
            if(character.RespawnPool != null) {
                GameObject newMe = character.RespawnPool.Spawn(transform.position, transform.rotation, transform.localScale, false);

                SurvivorPlayer newSurvivor = newMe.AddComponent<SurvivorPlayer>();
                newSurvivor.PlayerId = PlayerId;
                newSurvivor.Gamepad = Gamepad;

                FyoApp.ActiveGamepads[Gamepad] = newSurvivor;

                newMe.SetActive(true);
            }
            gameObject.SetActive(false);
            Destroy(this);
        }

        public void Afterlife() {
            StartCoroutine(PassIntoTheGreatBeyond());
        }
    }
}
