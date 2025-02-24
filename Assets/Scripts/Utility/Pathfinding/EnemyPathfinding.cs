using UnityEngine;
using System.Collections.Generic;


namespace JuanIsometric2D.Pathfinding
{
    public class EnemyPathfinding : MonoBehaviour
    {
        [Header("Pathfinding Settings")]
        [SerializeField] float rayDistance = 1.5f;
        [SerializeField] float pathUpdateInterval = 0.1f;
        [SerializeField] float stuckThresholdTime = 0.5f;

        [SerializeField] int numberOfRays = 8;

        [SerializeField] LayerMask[] avoidanceLayers;


        List<Vector2> rayDirections;

        Vector2 currentPathDirection;


        float pathUpdateTimer;
        float chaseStuckTimer = 0f;
        float patrolStuckTimer = 0f;


        void Start()
        {
            InitializeRayDirections();

            pathUpdateTimer = pathUpdateInterval;

            currentPathDirection = Vector2.zero;
        }

        void InitializeRayDirections()
        {
            rayDirections = new List<Vector2>();

            float angleStep = 360f / numberOfRays;

            for (int i = 0; i < numberOfRays; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;

                Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));

                rayDirections.Add(direction.normalized);
            }
        }

        LayerMask CombineAvoidanceLayers()
        {
            LayerMask combinedMask = 0;

            foreach (LayerMask layer in avoidanceLayers)
            {
                combinedMask |= layer;  
            }
            return combinedMask;
        }

        public Vector2 CalculateMovementDirection(Vector2 currentPosition, Vector2 targetPosition, float deltaTime, bool isChaseMode)
        {
            pathUpdateTimer -= deltaTime;

            if (pathUpdateTimer <= 0)
            {
                pathUpdateTimer = pathUpdateInterval;

                float targetWeight = isChaseMode ? 1.0f : 0.8f;
                float avoidanceWeight = isChaseMode ? 0.5f : 0.7f;

                Vector2 desiredDirection = (targetPosition - currentPosition).normalized;
                Vector2 avoidanceForce = Vector2.zero;

                LayerMask combinedLayers = CombineAvoidanceLayers();

                foreach (Vector2 ray in rayDirections)
                {
                    RaycastHit2D[] hits = Physics2D.RaycastAll(currentPosition, ray, rayDistance, combinedLayers);

                    RaycastHit2D? closestHit = null;

                    float closestDistance = float.MaxValue;

                    foreach (RaycastHit2D hit in hits)
                    {
                        if (hit.collider.gameObject == gameObject)
                        {
                            continue;
                        }

                        if (hit.distance < closestDistance)
                        {
                            closestDistance = hit.distance;
                            closestHit = hit;
                        }
                    }

                    if (closestHit.HasValue)
                    {
                        float forceMagnitude = 1f - (closestHit.Value.distance / rayDistance);
                        avoidanceForce -= ray * forceMagnitude;

                        Debug.DrawRay(currentPosition, ray * rayDistance, Color.red, pathUpdateInterval);
                    }
                    else
                    {
                        Debug.DrawRay(currentPosition, ray * rayDistance, Color.green, pathUpdateInterval);
                    }
                }

                Vector2 blendedDirection = (targetWeight * desiredDirection) + (avoidanceWeight * avoidanceForce);

                if (isChaseMode)
                {
                    if (blendedDirection.magnitude < 0.1f)
                    {
                        chaseStuckTimer += pathUpdateInterval;

                        if (chaseStuckTimer > stuckThresholdTime)
                        {
                            float randomAngle = Random.Range(-90f, 90f);

                            Vector2 alternativeDirection = Quaternion.Euler(0, 0, randomAngle) * desiredDirection;

                            blendedDirection = alternativeDirection;

                            chaseStuckTimer = 0f;
                        }
                        else
                        {
                            float jitterFactor = Mathf.Lerp(0.2f, 1f, Mathf.Clamp01(chaseStuckTimer / stuckThresholdTime));

                            blendedDirection += Random.insideUnitCircle * jitterFactor;
                        }
                    }
                    else
                    {
                        chaseStuckTimer = 0f;
                    }
                }
                else
                {
                    if (blendedDirection.magnitude < 0.1f)
                    {
                        patrolStuckTimer += pathUpdateInterval;

                        if (patrolStuckTimer > stuckThresholdTime)
                        {
                            Vector2 bestDirection = Vector2.zero;

                            float bestDistance = -Mathf.Infinity;

                            foreach (Vector2 ray in rayDirections)
                            {
                                RaycastHit2D[] hits = Physics2D.RaycastAll(currentPosition, ray, rayDistance, combinedLayers);
                                float closestValidDistance = rayDistance;

                                foreach (RaycastHit2D hit in hits)
                                {
                                    if (hit.collider.gameObject == gameObject)
                                    {
                                        continue;
                                    }

                                    if (hit.distance < closestValidDistance)
                                    {
                                        closestValidDistance = hit.distance;
                                    }
                                }

                                if (closestValidDistance > bestDistance)
                                {
                                    bestDistance = closestValidDistance;
                                    bestDirection = ray;
                                }
                            }
                            if (bestDirection != Vector2.zero)
                            {
                                blendedDirection = bestDirection;
                            }
                            else
                            {
                                blendedDirection += Random.insideUnitCircle * 0.5f;
                            }
                            patrolStuckTimer = 0f;
                        }
                        else
                        {
                            blendedDirection += Random.insideUnitCircle * 0.2f;
                        }
                    }
                    else
                    {
                        patrolStuckTimer = 0f;
                    }
                }

                Vector2 finalDirection = blendedDirection.normalized;
                currentPathDirection = Vector2.Lerp(currentPathDirection, finalDirection, 0.5f).normalized;
            }

            return currentPathDirection;
        }

        void OnDrawGizmos()
        {
            if (!Application.isPlaying || rayDirections == null)
            {
                return;
            }

            Gizmos.color = Color.yellow;

            foreach (Vector2 direction in rayDirections)
            {
                Gizmos.DrawRay(transform.position, direction * rayDistance);
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawRay(transform.position, currentPathDirection * rayDistance * 1.5f);
        }
    }
}