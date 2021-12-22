using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace alexshko.defensetower.Menu
{
    public class MainMenu : MonoBehaviour
    {
        Coroutine sceneLoadCoroutine;
        public void LoadScene(string nameOfScene)
        {
            sceneLoadCoroutine = StartCoroutine(LoadSceneCo(nameOfScene));
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private IEnumerator LoadSceneCo(string sceneName)
        {

            AsyncOperation ao = SceneManager.LoadSceneAsync(sceneName);
            while (!(ao.isDone))
            {
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}
