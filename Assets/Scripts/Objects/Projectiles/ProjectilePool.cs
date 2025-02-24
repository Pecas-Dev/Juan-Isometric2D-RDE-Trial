using UnityEngine;
using System.Collections.Generic;


namespace JuanIsometric2D.Combat
{
    public class ProjectilePool : MonoBehaviour
    {
        [SerializeField] GameObject projectilePrefab;
        [SerializeField] int poolSize = 20;


        readonly Queue<GameObject> availableProjectiles = new();
        readonly List<GameObject> allProjectiles = new();


        void Awake()
        {
            InitializePool();
        }

        void InitializePool()
        {
            for (int i = 0; i < poolSize; i++)
            {
                GameObject projectile = Instantiate(projectilePrefab, transform);
                projectile.SetActive(false);

                availableProjectiles.Enqueue(projectile);
                allProjectiles.Add(projectile);
            }
        }

        public GameObject GetProjectile()
        {
            if (availableProjectiles.Count == 0)
            {
                foreach (GameObject projectiles in allProjectiles)
                {
                    if (!projectiles.activeInHierarchy)
                    {
                        availableProjectiles.Enqueue(projectiles);
                    }
                }

                if (availableProjectiles.Count == 0)
                {
                    GameObject oldestActive = null;

                    float oldestTime = float.MinValue;

                    foreach (GameObject projectiles in allProjectiles)
                    {
                        if (projectiles.activeInHierarchy)
                        {
                            float projectileTime = projectiles.GetComponent<Projectile>().CurrentLifetime;

                            if (projectileTime > oldestTime)
                            {
                                oldestTime = projectileTime;
                                oldestActive = projectiles;
                            }
                        }
                    }

                    if (oldestActive != null)
                    {
                        oldestActive.SetActive(false);
                        availableProjectiles.Enqueue(oldestActive);
                    }
                }
            }

            GameObject projectile = availableProjectiles.Dequeue();
            projectile.SetActive(true);

            return projectile;
        }

        public void ReturnProjectile(GameObject projectile)
        {
            projectile.SetActive(false);
            availableProjectiles.Enqueue(projectile);
        }
    }
}
