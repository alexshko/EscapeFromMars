using UnityEngine;
using UnityEngine.UI;

namespace alexshko.defensetower.Core
{
    [ExecuteInEditMode]
    public class ResourcesEngine : MonoBehaviour
    {
        [SerializeField]
        private int treesCount;
        public int TreesCount
        {
            get => Mathf.Max(0, treesCount);
            set
            {
                treesCount = value;
            }
        }

        //singelton pattern:
        private static ResourcesEngine instance;
        public static ResourcesEngine Instance
        {
            get => instance;
        }

        [Tooltip("Reference to the trees text UI element")]
        public Text treesTextRef;

        private void Awake()
        {
            instance = this;
            //treesCount = 0;
        }

        private void Update()
        {
            treesTextRef.text = TreesCount.ToString();
        }
    }
}
