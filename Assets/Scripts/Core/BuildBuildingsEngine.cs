using alexshko.defensetower.buildings;
using System;
using UnityEngine;

namespace alexshko.defensetower.Core
{

    public class BuildBuildingsEngine : MonoBehaviour
    {
        [Tooltip("Layers to build on")]
        public LayerMask layersBuildOn;
        [Tooltip("the algorithm works by dividing the mesh ground surface to a net of squared patches. every vertice in the square will be checked if it has the correct height." +
            "Here we choose how many patches")]
        public Vector2 patchesToCheckOnWorld;
        public float AcceptableDifferenceInHeights = 0.05f;
        public Terrain gameWorld;
        private Building curBuilding;
        //singleton:
        private static BuildBuildingsEngine inst;
        public static BuildBuildingsEngine Instance
        {
            get => inst;
        }

        private void Awake()
        {
            inst = this;
            curBuilding = null;
            ////for testing purposes:
            //curBuilding = Instantiate(Resources.Load<Building>("LaserCanon"));
        }

        public void startShowingBuilding(string buildingName)
        {
            if (curBuilding)
            {
                //if it was during setting the new position, then destroy it because it wasn't set.
                if (curBuilding.State == BuildingState.choosePlaceIllegal || curBuilding.State == BuildingState.choosePlaceLegal)
                {
                    Destroy(curBuilding.gameObject);
                }
            }
            //Instantiate a new building in construction mode:
            Building buildingInfo = Resources.Load<Building>(buildingName);
            if (ResourcesEngine.Instance.TreesCount >= buildingInfo.treesCost)
            {
                curBuilding = Instantiate(buildingInfo);
                curBuilding.State = BuildingState.choosePlaceIllegal;
            }
            buildingInfo = null;
        }

        private void Update()
        {
            checkIfPRessedNewBuildingShortcut();
            showCurBuildingOnWorld();
            checkIfPressedConstruct();
            checkIfPressedCancel();
        }

        private void checkIfPRessedNewBuildingShortcut()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                startShowingBuilding("LaserCanon");
            }
            if (Input.GetKeyDown(KeyCode.F2))
            {
                startShowingBuilding("Fence");
            }
        }

        private void checkIfPressedCancel()
        {
            if (Input.GetButtonDown("Cancel"))
            {
                Destroy(curBuilding.gameObject);
                curBuilding = null;
            }
        }

        private void checkIfPressedConstruct()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (curBuilding && curBuilding.State == BuildingState.choosePlaceLegal)
                {
                    curBuilding.State = BuildingState.constructed;
                    curBuilding = null;
                }
            }
        }

        private void showCurBuildingOnWorld()
        {
            if (curBuilding)
            {
                Vector2 mousePos = Input.mousePosition;
                //Debug.LogFormat("mouse cor: {0}", mousePos);
                Ray r = Camera.main.ScreenPointToRay(mousePos);
                //Debug.DrawRay(r.origin, r.direction, Color.red, 3);
                RaycastHit hit;
                if (Physics.Raycast(r, out hit, Mathf.Infinity, layersBuildOn))
                {
                    //Debug.LogFormat("y cordinate of hit: {0}", hit.point.y);
                    if (isLegalToBuild(hit.point))
                    {
                        Debug.Log("Build");
                        Debug.DrawLine(hit.point, hit.point + Vector3.up, Color.green, 5);
                        curBuilding.transform.position = hit.point;
                        curBuilding.State = BuildingState.choosePlaceLegal;
                    }
                    else
                    {
                        Debug.DrawLine(hit.point, hit.point + Vector3.up, Color.red, 5);
                        curBuilding.State = BuildingState.choosePlaceIllegal;
                    }
                }
            }
        }

        private bool isLegalToBuild(Vector3 pointToBuild)
        {
            //check if there are already other objects in the area.
            //one of the colliders found is the current building itself, so need to check if there are more buildings:
            Collider[] nearObjects = Physics.OverlapBox(pointToBuild, new Vector3(curBuilding.width / 2.0f, 0.5f, curBuilding.length / 2.0f), curBuilding.transform.rotation, ~layersBuildOn);
            if (nearObjects.Length > 1)
            {
                Debug.Log("object near: " + nearObjects[0].name);
                return false;
            }

            //check if the height is the same in all the possible area:
            float xStart = pointToBuild.x - curBuilding.width / 2.0f;
            float xEnd = pointToBuild.x + curBuilding.width / 2.0f;
            float zStart = pointToBuild.z - curBuilding.length / 2.0f;
            float zEnd = pointToBuild.z + curBuilding.length / 2.0f;
            float patchWidth = curBuilding.width / patchesToCheckOnWorld.x;
            float patchLength = curBuilding.length / patchesToCheckOnWorld.y;
            for (float i = xStart; i <= xEnd; i += patchWidth)
            {
                for (float j = zStart; j <= zEnd; j += patchLength)
                {
                    Vector3 neighboorToCheck = new Vector3(i, 0, j);
                    float pointToCheckHeight = gameWorld.SampleHeight(neighboorToCheck);
                    if (Mathf.Abs(pointToCheckHeight - pointToBuild.y) > AcceptableDifferenceInHeights)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
