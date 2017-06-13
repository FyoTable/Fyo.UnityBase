using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieSurvivor {
    /// <summary>
    /// AI Player Class
    /// </summary>
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
    public class SurvivorZombie : MonoBehaviour {
        Character character;
        UnityEngine.AI.NavMeshAgent agent;

        public float WanderRadius = 1.0f;
        
        public GameObject CurrentFood = null;

        float ThoughtElapsed = 0.0f;
        float ThoughtTime = 0.2f;
        float MoveElapsed = 0.0f;
        float MoveTime = 0.5f;

        public AI.Touch touch;
        public AI.Sight eye;

        private void Start() {
            character = GetComponent<Character>();
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            character.Afterlife = Afterlife;
        }

        private void OnEnable() {
            UnityEngine.AI.NavMeshHit closestHit;

            if (UnityEngine.AI.NavMesh.SamplePosition(gameObject.transform.position, out closestHit, 500f, UnityEngine.AI.NavMesh.AllAreas))
                gameObject.transform.position = closestHit.position;
            else
                Debug.LogError("Could not find position on NavMesh!");
            agent.enabled = true;
        }
        
        private void Awake() {
            character = GetComponent<Character>();
            agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            agent.enabled = true;
        }

        IEnumerator PassIntoTheGreatBeyond() {
            yield return new WaitForSeconds(2.5f);
            gameObject.SetActive(false);
        }

        public void Afterlife() {
            agent.enabled = false;
            touch.Touching.Clear();
            eye.Seen.Clear();
            CurrentFood = null;
            StartCoroutine(PassIntoTheGreatBeyond() );
        }

        void Think() {
            if (MoveElapsed >= MoveTime) {
                if (agent.isOnNavMesh) {
                    if (CurrentFood != null) {
                        if (touch.Touching.Contains(CurrentFood)) {
                            character.Weapon.Use(gameObject);
                        }
                        agent.SetDestination(CurrentFood.transform.position);
                        character.animator.SetBool("IsWalking", true);
                        transform.LookAt(CurrentFood.transform.position, Vector3.up);
                    } else {
                        //Find Food
                        if(eye.Seen.Count > 0) {
                            GameObject Nearest = null;
                            float Distance = float.MaxValue;
                            float dNext;
                            for(int i = 0; i < eye.Seen.Count; i++) {
                                if (Nearest != null) {
                                    //TODO: Implement View Angle
                                    dNext = (eye.Seen[i].transform.position - transform.position).magnitude;
                                    if (dNext < Distance) {
                                        Nearest = eye.Seen[i];
                                        Distance = (Nearest.transform.position - transform.position).magnitude;
                                    }
                                } else {
                                    Nearest = eye.Seen[i];
                                    Distance = (Nearest.transform.position - transform.position).magnitude;
                                }
                            }

                            if(Nearest != null) {
                                CurrentFood = Nearest;
                            }
                        }

                        if (CurrentFood != null) {
                            if (touch.Touching.Contains(CurrentFood)) {
                                character.Weapon.Use(gameObject);
                            }

                            agent.SetDestination(CurrentFood.transform.position);
                            character.animator.SetBool("IsWalking", true);
                            transform.LookAt(CurrentFood.transform.position, Vector3.up);
                        } else {
                            //Wait or Random Destination
                            int w = Random.Range(0, 2);
                            if(w == 0) {
                                Vector3 dest = new Vector3(Random.Range(-WanderRadius, WanderRadius), 0.0f, Random.Range(-WanderRadius, WanderRadius));
                                agent.SetDestination(transform.position + dest);
                                character.animator.SetBool("IsWalking", true);
                            }
                        }
                    }
                    character.animator.SetBool("HasTarget", CurrentFood != null);
                    MoveElapsed = 0.0f;
                }
            }
        }

        void Update () {
            if (character.Health.CurrentHealth > 0.0f) {
                ThoughtElapsed += Time.deltaTime;
                MoveElapsed += Time.deltaTime;
                if (ThoughtElapsed >= ThoughtTime) {
                    ThoughtElapsed = 0.0f;
                    Think();
                }
            }
        }
    }
}