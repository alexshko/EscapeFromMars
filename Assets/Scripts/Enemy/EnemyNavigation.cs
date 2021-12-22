using alexshko.defensetower.buildings;
using alexshko.defensetower.life;
using UnityEngine;
using UnityEngine.AI;

namespace alexshko.defensetower.Enemies
{
    public class EnemyNavigation : MonoBehaviour
    {
        public float speedOfNavigation
        {
            get
            {
                return navAgent ? navAgent.speed : 0;
            }
            set
            {
                if (navAgent)
                {
                    navAgent.speed = value;
                }
            }
        }
        //the target to go to using the NevMeshAgent
        private Transform targetToNavigate;
        public Transform TargetToNavigate
        {
            get => targetToNavigate;
            set
            {
                targetToNavigate = value;
                //if he's during nvaigation and we changed the destination then it shoudl recalculate the path:
                if (isNavigating)
                {
                    startGoingToTarget();
                }
            }
        }
        public bool isNavigating
        {
            get
            {
                return (!(targetToNavigate.position.Equals(transform.position)));
            }
            set
            {
                if (value && !isNavigating)
                {
                    startGoingToTarget();
                }
                else if (!value && isNavigating)
                {
                    stopNavigating();
                }
            }
        }

        private NavMeshAgent navAgent;
        private void Awake()
        {
            navAgent = GetComponent<NavMeshAgent>();
        }

        public void startGoingToTarget()
        {
            navAgent.SetDestination(targetToNavigate.position);
        }

        public void stopNavigating()
        {
            navAgent.SetDestination(transform.position);
        }
    
        public bool checkIfThereIsPath(Vector3 target)
        {
            NavMeshPath path = new NavMeshPath();
            //bool hasPath = navAgent.CalculatePath(target, path);

            //bitwise area mask. we want walkable whick is 001 in binary:
            bool hasPath = NavMesh.CalculatePath(transform.position, target, 1<<0, path);
            Debug.DrawLine(transform.position, target, Color.yellow, 4);
            //so there will not be memory leaks or extra memory allocated:
            path = null;
            return hasPath;
        }
    }
}
