using alexshko.defensetower.buildings;
using alexshko.defensetower.life;
using UnityEngine;

namespace alexshko.defensetower.Core
{
    public class GameController : MonoBehaviour
    {
        [Tooltip("the UI panel to show when the game is over")]
        public Transform gameOverUIRef;
        [Tooltip("the UI panel to show when the game is won")]
        public Transform gameWonUIRef;
        [Tooltip("the reference to the building. when it is destroyed, the game is over")]
        public LifeEngine heartBuidlingRef;

        //boolean to indicate if the game is playing currently. mainly for syncing with other classes:
        public bool isGamePlaying { get; set; }

        //singelton pattern:
        public static GameController Instance
        {
            get => instance;
        }
        private static GameController instance;

        private void Awake()
        {
            instance = this;
            isGamePlaying = true;
        }

        private void Start()
        {
            //once the heart building is destroyed, the game is over.
            heartBuidlingRef.OnDieEvent += FinishGame;
        }

        public void FinishGame()
        {
            if (gameOverUIRef)
            {
                //call the game over Panel:
                gameOverUIRef.gameObject.SetActive(true);
            }
            EndOfGame();
        }

        public void WinGame()
        {
            if (gameWonUIRef)
            {
                //call the game over Panel:
                gameWonUIRef.gameObject.SetActive(true);
            }
            EndOfGame();
        }

        private void EndOfGame()
        {
            Debug.Log("Finished Game.");
            isGamePlaying = false;
            //stop spawning enemies:
            GameObject.Find("EnemySpawner").SetActive(false);
        }
    }
}
