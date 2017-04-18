using UnityEngine;
using System.Collections;

namespace SurvivalShooterExampleGame {
    public class EnemyAttack : MonoBehaviour {
        public float timeBetweenAttacks = 0.5f;     // The time in seconds between each attack.
        public int attackDamage = 10;               // The amount of health taken away per attack.
        
        Animator anim;                              // Reference to the animator component.
        public GameObject TargetPlayer;                          // Reference to the player GameObject.
        public PlayerHealth playerHealth;                  // Reference to the player's health.
        EnemyHealth enemyHealth;                    // Reference to this enemy's health.
        bool playerInRange;                         // Whether player is within the trigger collider and can be attacked.
        float timer;                                // Timer for counting up to the next attack.
        
        void Awake() {
            enemyHealth = GetComponent<EnemyHealth>();
        }

        void OnTriggerEnter(Collider other) {
            // If the entering collider is the player...
            if (other.gameObject == TargetPlayer) {
                // ... the player is in range.
                playerInRange = true;
            }
        }
        
        void OnTriggerExit(Collider other) {
            // If the exiting collider is the player...
            if (other.gameObject == TargetPlayer) {
                // ... the player is no longer in range.
                playerInRange = false;
            }
        }
        
        void Update() {
            // Add the time since Update was last called to the timer.
            timer += Time.deltaTime;

            if (timer >= timeBetweenAttacks) {
                if (TargetPlayer == null || !TargetPlayer.activeSelf) {
                    //Find nearest living player
                    GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");
                    if (Players.Length > 0) {
                        float dist = float.MaxValue;
                        float nearest = float.MaxValue;
                        GameObject NearestPlayer;
                        for (int p = 0; p < Players.Length; p++) {
                            NearestPlayer = Players[p];
                            if (NearestPlayer != null) {
                                dist = (NearestPlayer.transform.position - transform.position).magnitude;
                                if (dist < nearest) {
                                    TargetPlayer = NearestPlayer;
                                }
                            }
                        }
                    }
                }

                if (TargetPlayer != null) {
                    playerHealth = TargetPlayer.GetComponent<PlayerHealth>();
                    if (playerInRange && playerHealth.currentHealth > 0) {
                        anim = GetComponent<Animator>();
                        // If the timer exceeds the time between attacks, the player is in range and this enemy is alive...

                        // ... attack.
                        // Reset the timer.
                        timer = 0f;

                        // If the player has health to lose...
                        if (playerHealth.currentHealth > 0) {
                            // ... damage the player.
                            playerHealth.TakeDamage(attackDamage);
                        }

                        // If the player has zero or less health...
                        if (playerHealth.currentHealth <= 0) {
                            // ... tell the animator the player is dead.
                            anim.SetTrigger("PlayerDead");
                        }
                    }
                }
            }
        }
    }
}