using UnityEngine;
using System.Collections;

namespace SurvivalShooterExampleGame {
    public class EnemyMovement : MonoBehaviour {
        Transform player;               // Reference to the player's position.
        PlayerHealth playerHealth;      // Reference to the player's health.
        EnemyHealth enemyHealth;        // Reference to this enemy's health.
        EnemyAttack enemyAttack;        // Reference to this enemy's target.
        UnityEngine.AI.NavMeshAgent nav;               // Reference to the nav mesh agent.

        void Awake () {
            enemyAttack = GetComponent<EnemyAttack>();
            enemyHealth = GetComponent <EnemyHealth> ();
            nav = GetComponent <UnityEngine.AI.NavMeshAgent> ();
        }

        void Update () {
            // Set up the references.
            if (enemyAttack.TargetPlayer != null) {
                player = enemyAttack.TargetPlayer.transform;
                playerHealth = player.GetComponent<PlayerHealth>();
            } else {
                if(playerHealth != null)
                    playerHealth = null;
            }


            if (playerHealth != null) {
                // If the enemy and the player have health left...
                if (enemyHealth.currentHealth > 0 && playerHealth.currentHealth > 0) {
                    // ... set the destination of the nav mesh agent to the player.
                    nav.SetDestination(player.position);
                }
                // Otherwise...
                else {
                    // ... disable the nav mesh agent.
                    nav.enabled = false;
                }
            } 
        }
    }
}