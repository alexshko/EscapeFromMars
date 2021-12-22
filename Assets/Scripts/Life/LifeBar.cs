using UnityEngine;
using UnityEngine.UI;

namespace alexshko.defensetower.life
{
    public class LifeBar : MonoBehaviour
    {
        [Tooltip("Life Engine Component to show on the Lifebar")]
        public LifeEngine lifeEngRef;
        [Tooltip("The speed of the life drain animation.")]
        public float animSpeed = 4;

        private Slider sliderLife;
        private float valueToShow;
        // Start is called before the first frame update
        void Awake()
        {
            if (lifeEngRef == null)
            {
                Debug.LogError("Missing LifeEngine Reference");
            }

            sliderLife = GetComponentInChildren<Slider>();
            if (sliderLife == null)
            {
                Debug.LogError("Missing Slider in hirearchy");
            }
        }

        // Update is called once per frame
        void Update()
        {
            //calc: the amount of life left out of initial life. make it animation like by Lerp.
            valueToShow = Mathf.Clamp01(lifeEngRef.LifeLeft * 1.0f / lifeEngRef.maxLife);
            sliderLife.value = Mathf.Lerp(sliderLife.value, valueToShow , animSpeed * Time.deltaTime);
        }
    }
}
