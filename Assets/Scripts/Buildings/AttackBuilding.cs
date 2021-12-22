using alexshko.defensetower.life;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace alexshko.defensetower.buildings
{
    public class AttackBuilding : Building
    {
        public int damage = 5;
        public float numberOfLasersPerSecond = 3;
        public float radiusToAttack = 5;
        public LayerMask attackLayers;
        public Transform LaserShotPref;
        public Transform CanonHinge;

        private Transform heartBuildingToDefend;
        private Transform shootAtTarget;
        private Transform initPosOfShot;
        private LineRenderer laserShot;
        // Start is called before the first frame update
        new void Start()
        {
            base.Start();
            heartBuildingToDefend = GameObject.FindGameObjectWithTag("HeartBuilding").transform;
            foreach (var item in GetComponentsInChildren<Transform>())
            {
                if (item.name == "pointOfShoot")
                {
                    initPosOfShot = item;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (Core.GameController.Instance.isGamePlaying)
            {
                findTargetToShoot();
                if (shootAtTarget)
                {
                    //make sure it only turn'n over it's y-axis:
                    CanonHinge.LookAt(shootAtTarget);
                    CanonHinge.eulerAngles = new Vector3(0, CanonHinge.eulerAngles.y, 0);
                }
                //make the shot only if it's in the constructed state:
                if (this.State == BuildingState.constructed)
                {
                    MakeShot();
                }
            }
        }
        private void MakeShot()
        {
            if (shootAtTarget)
            {
                turnOnAndAdjustLaser();
                shootAtTarget.GetComponent<LifeEngine>().TakeHit(Vector3.zero, Vector3.zero, damage * Time.deltaTime);
            }
            else
            {
                turnOffLaser();
            }
        }

        private void turnOffLaser()
        {
            if (laserShot)
            {
                laserShot.enabled = false;
            }
        }

        private void turnOnAndAdjustLaser()
        {
            if (!laserShot)
            {
                laserShot = Instantiate(LaserShotPref, Vector3.zero, initPosOfShot.rotation, initPosOfShot).GetComponent<LineRenderer>();

            }
            laserShot.enabled = true;
            laserShot.SetPosition(0, initPosOfShot.position);
            laserShot.SetPosition(1, shootAtTarget.position);
        }

        private void findTargetToShoot()
        {
            Collider[] enemiesInAttackRange = Physics.OverlapSphere(transform.position, radiusToAttack, attackLayers);
            if (enemiesInAttackRange.Length > 0)
            {
                shootAtTarget = getEnemiesClosestToHeart(enemiesInAttackRange);
            }
            else
            {
                shootAtTarget = null;
            }
        }

        private Transform getEnemiesClosestToHeart(Collider[] enemiesInAttackRange)
        {
            Transform dangerestEnemy = enemiesInAttackRange[0].transform;
            foreach (var enemy in enemiesInAttackRange)
            {
                float distToHeartBuilding = Vector3.Distance(enemy.transform.position, heartBuildingToDefend.position);
                float curMinDistToHeartBuilding = Vector3.Distance(dangerestEnemy.position, heartBuildingToDefend.position);
                if (distToHeartBuilding < curMinDistToHeartBuilding)
                {
                    dangerestEnemy = enemy.transform;
                }
            }
            return dangerestEnemy;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, radiusToAttack);
        }

        private void OnDestroy()
        {
            if (laserShot)
            {
                Destroy(laserShot.gameObject);
            }
        }
    }
}
