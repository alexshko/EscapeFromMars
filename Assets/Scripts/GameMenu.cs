using alexshko.defensetower.Core;
using UnityEngine;

namespace alexshko.defensetower.Menu
{
    public class GameMenu : MonoBehaviour
    {
        public void startConstructingBuilding(string buildingName)
        {
            BuildBuildingsEngine.Instance.startShowingBuilding(buildingName);
        }
    }
}
