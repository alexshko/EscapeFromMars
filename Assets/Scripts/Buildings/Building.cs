using System;
using UnityEngine;

namespace alexshko.defensetower.buildings
{
    public enum BuildingState {canceled, choosePlaceLegal, choosePlaceIllegal, constructed, destroyed}
    public class Building : MonoBehaviour
    {
        public int treesCost = 40;
        public float width { get => buildingCollider ? 2*buildingCollider.bounds.extents.x : 1; }
        public float length { get => buildingCollider ? 2*buildingCollider.bounds.extents.z : 1; }

        private Collider buildingCollider;
        protected void Start()
        {
            buildingCollider = GetComponent<Collider>();
            state = BuildingState.constructed;
        }
        private BuildingState state;
        public BuildingState State
        {
            get => state;
            set
            {
                if (value == BuildingState.choosePlaceLegal)
                {
                    setMeshConstructionState(true);
                    gameObject.layer = LayerMask.NameToLayer("BuildingUnderConstruction");
                }
                else if (value == BuildingState.choosePlaceIllegal)
                {
                    setMeshConstructionState(false);
                    gameObject.layer = LayerMask.NameToLayer("BuildingUnderConstruction");
                }
                else if (value == BuildingState.constructed)
                {
                    setMeshConstructionState(true);
                    Debug.Log("before build it had layer: " + LayerMask.LayerToName(gameObject.layer));
                    gameObject.layer = LayerMask.NameToLayer("Building");
                    Core.ResourcesEngine.Instance.TreesCount -= treesCost;
                }
                state = value;
            }
        }

        private void setMeshConstructionState(bool isLegal)
        {
            foreach (var item in transform.GetComponentsInChildren<MeshRenderer>())
            {
                item.enabled = isLegal;
            } 
        }
        
    }
}
