using alexshko.defensetower.life;
using UnityEngine;

namespace alexshko.defensetower.Enemies
{
    public enum EnemyState { Walking, Attacking};

    [RequireComponent(typeof(EnemyNavigation))]
    public class EnemyLogic : MonoBehaviour
    {
        public Transform mainTarget;
        public float buildingSearchRadius;
        public LayerMask attackLayers;
        public float attackDistance = 1.5f;
        public float attackDamage = 50;
        public int treesWorth = 10;

        private LifeEngine buildingToAttack;
        private EnemyNavigation enemyNav;
        private EnemyState state;
        // Use this for initialization
        void Awake()
        {
            buildingToAttack = null;
            enemyNav = GetComponent<EnemyNavigation>();
            transform.LookAt(Camera.main.transform);
            //enemyNav.startGoingToTarget();
        }
        private void Start()
        {
            enemyNav.TargetToNavigate = mainTarget;

            //register event the death of the enemy to earn the tree upon death:
            GetComponent<LifeEngine>().OnDieEvent += () => {
                Core.ResourcesEngine.Instance.TreesCount += treesWorth;
            };
        }

        // Update is called once per frame
        void Update()
        {
            if (Core.GameController.Instance.isGamePlaying)
            {
                adjustState();
                AdjustNavigation();
                checkIfAttack();
            }
        }

        //if in attack state, hit the curBuilding:
        private void checkIfAttack()
        {
            if (state == EnemyState.Attacking)
            {
                //todo:   stop walking
                //enemyNav.isNavigating = false;
                buildingToAttack.TakeHit(Vector3.zero, Vector3.zero, attackDamage * Time.deltaTime);
            }
        }

        private void adjustState()
        {
            if (!buildingToAttack)
            {
                //if there is no building to attack, then it should walk.
                //during the state it night recalculate new building to attack
                state = EnemyState.Walking;
            }
            else
            {
                //if there is building to attack, check the distance to it. attack it if it's in attack radius:
                float targetDist = 0;
                targetDist = calcDiffToOuterBoundsOfMesh(buildingToAttack.transform).magnitude;
                if (targetDist < attackDistance)
                {
                    state = EnemyState.Attacking;
                }
                else
                {
                    state = EnemyState.Walking;
                }
            }
        }

        private void AdjustNavigation()
        {
            if (state == EnemyState.Walking)
            {
                //if there is building nearby to attack, go to it. otherwise go to the main building:
                CheckForBuildingsToAttackNearby();
                if (buildingToAttack)
                {
                    if (enemyNav.TargetToNavigate != buildingToAttack)
                    {
                        enemyNav.TargetToNavigate = buildingToAttack.transform;
                    }
                }
                else
                {
                    enemyNav.TargetToNavigate = mainTarget;
                }
            }
            //enable navigation according to state:
            enemyNav.isNavigating = (state == EnemyState.Walking);
        }

        //check if there are buildings in the radius that are possible to reach (using navmesh). if so, put in buildingToAttack:
        private void CheckForBuildingsToAttackNearby()
        {
            if (!buildingToAttack)
            {
                Collider[] objectsInRadius = Physics.OverlapSphere(transform.position, buildingSearchRadius, attackLayers);
                foreach (var item in objectsInRadius)
                {
                    if (item.gameObject.Equals(this.gameObject))
                    {
                        continue;
                    }
                    //item is a possible building-to-attack, need to check if there is a path to it:
                    Vector3 posToGoTo = transform.position + calcDiffToOuterBoundsOfMesh(item.transform);
                    posToGoTo = posWithAttackDistance(posToGoTo);
                    if (enemyNav.checkIfThereIsPath(posToGoTo)){
                        buildingToAttack = item.GetComponent<LifeEngine>();
                        return; 
                    }
                }
                buildingToAttack = null;
            }
        }

        //substract from a position in the world the attackditance in world space:
        private Vector3 posWithAttackDistance(Vector3 pos)
        {
            Vector3 diffToItem = pos - transform.position;
            Vector3 posToGoTo = pos - diffToItem.normalized * attackDistance;
            return posToGoTo;
        }
        // calculate the difference between the outer side of mesh and the enemy,
        // if for some reason can't calculate it, then take the center of the mesh but with y cordinate = 0. 
        private Vector3 calcDiffToOuterBoundsOfMesh(Transform mesh)
        {
            RaycastHit rhit;
            Vector3 targetCenterDiff = mesh.position - transform.position;
            targetCenterDiff.y = 0;
            Vector3 targetOuterDiff = Vector3.zero;
            if (Physics.Raycast(transform.position, targetCenterDiff.normalized, out rhit, 2 * buildingSearchRadius, attackLayers))
            {
                targetOuterDiff = rhit.point - transform.position;
            }
            else
            {
                targetOuterDiff = targetCenterDiff;
            }
            return targetOuterDiff;
        }
    }
}