using alexshko.defensetower.Enemies;
using alexshko.defensetower.life;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace alexshko.defensetower.Core
{
    //class for instnace of a wave of enemies:
    //each wave has several intervals, each interval has several enemies.
    [System.Serializable]
    public class EnemyWave
    {
        [Range(1,4)]
        [Tooltip("How many enemies to instantiate each interval")]
        public int enemiesPerInterval = 3;
        [Tooltip("How many seconds between intervals")]
        public float secondsBetweenIntervals = 2;
        [Tooltip("Number of intervals in the wave")]
        public int numOfIntervals = 2;
        [Range(0,8)]
        [Tooltip("How fast is the enemy in this wave")]
        public float enemySpeed;
    }

    public class EnemySpawnEngine : MonoBehaviour
    {
        [Tooltip("all Texts that will show the number of kills. " +
            "Updated after each kill")]
        public Text[] CountOfKilledUIRef;
        [Tooltip("Prifab of the Enemy:")]
        public Transform enemyPref;
        [Tooltip("The position where all the enemies will be instantiated at.")]
        public Transform spawnStartPosition;
        [Tooltip("The position where all the enemies try to get to.")]
        public Transform spawnTargetPosition;
        [Tooltip("How many seconds to wait between waves")]
        public float secondsBetweenWaves;

        [SerializeField]
        public EnemyWave[] waves;

        private float timeLastSpawn;
        private int enemiesKilled;

        private int currentWave;
        private int currentIntervalInWave;
        private int spawnedLiveEnemies;


        private void Start()
        {
            currentWave = 0;
            currentIntervalInWave = 1;
            enemiesKilled = 0;
            spawnedLiveEnemies = 0;
            //one second to prepare until first interval of prisoners:
            //timeLastSpawn = Time.time - secondsBetweenIntervals + 1;
            timeLastSpawn = Time.time - waves[currentWave].secondsBetweenIntervals + 1;
        }

        private void Update()
        {
            //if still playing and still not all waves are finished:
            if (Core.GameController.Instance.isGamePlaying && currentWave < waves.Length)
            {
                float nextTimeToSpawn = calcNextTimeToSpawn();
                if (Time.time >= nextTimeToSpawn)
                {
                    MakeSpawn();
                    Debug.LogFormat("amount of enemies total: {0}", spawnedLiveEnemies);
                }
            }

            checkIfWon();
        }


        private float calcNextTimeToSpawn()
        {
            //if the next spawn is the beggining of a new wave, then wait secondsBetweenWaves time. 
            //if it's in the middle of a wave, then wait the amount that's defined in the wave:
            return timeLastSpawn + (currentIntervalInWave == 1 ? secondsBetweenWaves : waves[currentWave].secondsBetweenIntervals);
        }

        private void GotoNextIntervalAndWave()
        {
            currentIntervalInWave++;
            //if we passed number of intervals in the wave, then it the next wave:
            if (currentIntervalInWave > waves[currentWave].numOfIntervals)
            {
                currentWave++;
                currentIntervalInWave = 1;
            }
        }

        private void MakeSpawn()
        {
            timeLastSpawn = Time.time;
            for (int i=0;i< waves[currentWave].enemiesPerInterval; i++)
            {
                EnemyLogic enemy = Instantiate(enemyPref, spawnStartPosition.position, spawnStartPosition.rotation).GetComponent<EnemyLogic>();
                //upon the enemy's death, call function for updating the score. By adding to the OnDieEvent of prisoner's LifeEngine:
                enemy.GetComponent<LifeEngine>().OnDieEvent += EnemyActionOnDeath;
                enemy.mainTarget = spawnTargetPosition;
                enemy.GetComponent<EnemyNavigation>().speedOfNavigation = waves[currentWave].enemySpeed;

                //add to the count of live enemies:
                spawnedLiveEnemies++;
            }
            GotoNextIntervalAndWave();
        }

        private void EnemyActionOnDeath()
        {
            enemiesKilled++;
            spawnedLiveEnemies--;
            foreach (var txtUIItem in CountOfKilledUIRef)
            {
                txtUIItem.text = enemiesKilled.ToString();
            }
            Debug.LogFormat("Killed so far: {0}", enemiesKilled);
        }
        private void checkIfWon()
        {
            //if all the waves are over and all enemies are killed:
            if (spawnedLiveEnemies == 0 && currentWave >= waves.Length)
            {
                GameController.Instance.WinGame();
            }
        }
    }
}
