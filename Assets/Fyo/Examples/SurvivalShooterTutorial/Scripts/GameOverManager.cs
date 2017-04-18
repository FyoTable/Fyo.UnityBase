using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;

namespace SurvivalShooterExampleGame {
    public class GameOverManager : MonoBehaviour {
        public List<PlayerHealth> playerHealth = new List<PlayerHealth>();       // Reference to the player's health.
        Animator anim;                          // Reference to the animator component.
        
        void Awake () {
            // Set up the reference.
            anim = GetComponent <Animator> ();
        }
        
        void Update () {
            // If all players player has run out of health...
            if (playerHealth.All(p => p.currentHealth <= 0)) {
                // ... tell the animator the game is over.
                anim.SetTrigger("GameOver");
            }
        }
    }
}
