using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesLine : MonoBehaviour
{
    public static float speed = 2f;
    public static float speedIncrement = 0.02f;
    public static float maxSpeed = 8f;

    private List<GameObject> obstacles;
    private GameObject coin;
    private GameObject shield;
    private GameObject magnet;

    private float duration;
    private Vector3 startPos, endPos;
    private bool newLineSpawned;

    void Awake()
    {
        transform.position = new Vector3(transform.position.x, 7, transform.position.z);
    }

    void Start()
    {
        LoadObstacles();
        LoadPowerUps();
        SpawnLineOfObstacles();
        startPos = transform.position;
        endPos = new Vector3(transform.position.x, -7, transform.position.z);
    }

    void Update()
    {
        if (GameOver.instance == null) return;

        if (!GameOver.instance.crashed)
        {
            if (speed < maxSpeed)
            {
                speed += speedIncrement * Time.deltaTime;
            }

            if (transform.position != endPos)
            {
                if (speed != 0)
                {
                    duration += Time.deltaTime / (10 - speed);
                    transform.position = Vector3.Lerp(startPos, endPos, duration);

                    if (!newLineSpawned && duration > ObstacleLineSpawner.instance.spawnPlace)
                    {
                        ObstacleLineSpawner.instance.SpawnLine();
                        newLineSpawned = true;
                    }
                }
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    private void LoadObstacles()
    {
        obstacles = new List<GameObject>();
        Object[] objects = Resources.LoadAll("Obstacles") as Object[];
        foreach (Object item in objects)
        {
            obstacles.Add(item as GameObject);
        }
    }

    private void LoadPowerUps()
    {
        coin = Resources.Load("Coin") as GameObject;
        shield = Resources.Load("Shield") as GameObject;
        magnet = Resources.Load("Magnet") as GameObject;
    }

    private void SpawnObstacle(int lane)
    {
        int randomObstacleIndex = Random.Range(0, obstacles.Count);
        float randomObstacleOffset = Random.Range(0, ObstacleLineSpawner.instance.randomizeObstaclesOffest);
        Instantiate(obstacles[randomObstacleIndex], new Vector3(lane, 7 + randomObstacleOffset, 0), Quaternion.identity, transform);
    }

    private void SpawnCoin(int lane)
    {
        float randomOffset = Random.Range(0, ObstacleLineSpawner.instance.randomizeObstaclesOffest);
        Instantiate(coin, new Vector3(lane, 7 + randomOffset, 0), Quaternion.identity, transform);
    }

    private void SpawnShield(int lane)
    {
        float randomOffset = Random.Range(0, ObstacleLineSpawner.instance.randomizeObstaclesOffest);
        Instantiate(shield, new Vector3(lane, 7 + randomOffset, 0), Quaternion.identity, transform);
    }

    private void SpawnMagnet(int lane)
    {
        float randomOffset = Random.Range(0, ObstacleLineSpawner.instance.randomizeObstaclesOffest);
        Instantiate(magnet, new Vector3(lane, 7 + randomOffset, 0), Quaternion.identity, transform);
    }

    private void SpawnLineOfObstacles()
    {
        int minObstacles = ObstacleLineSpawner.instance.minObstacles;
        int maxObstacles = ObstacleLineSpawner.instance.maxObstacles;
        List<int> availableLanes = new List<int>() { -2, -1, 0, 1, 2 };

        // Power-up spawn chances
        float shieldSpawnChance = 0.1f;  // 10% chance for shield
        float magnetSpawnChance = 0.1f;  // 10% chance for magnet
        
        // Determine what will spawn
        bool willSpawnShield = Random.value < shieldSpawnChance;
        bool willSpawnMagnet = !willSpawnShield && Random.value < magnetSpawnChance;
        bool willSpawnCoin = !willSpawnShield && !willSpawnMagnet && 
                            Random.value < ObstacleLineSpawner.instance.coinSpawnRate;

        // Calculate obstacles leaving room for power-up
        int maxPossibleObstacles = (willSpawnShield || willSpawnMagnet || willSpawnCoin) 
                                  ? maxObstacles - 1 : maxObstacles;
        int obstaclesAmount = Random.Range(minObstacles, maxPossibleObstacles + 1);

        // Spawn obstacles
        for (int i = 0; i < obstaclesAmount; i++)
        {
            int randomLaneIndex = Random.Range(0, availableLanes.Count);
            SpawnObstacle(availableLanes[randomLaneIndex]);
            availableLanes.RemoveAt(randomLaneIndex);
        }

        // Spawn power-up if we have lanes available
        if (availableLanes.Count > 0)
        {
            int randomLaneIndex = Random.Range(0, availableLanes.Count);
            if (willSpawnShield)
            {
                SpawnShield(availableLanes[randomLaneIndex]);
            }
            else if (willSpawnMagnet)
            {
                SpawnMagnet(availableLanes[randomLaneIndex]);
            }
            else if (willSpawnCoin)
            {
                SpawnCoin(availableLanes[randomLaneIndex]);
            }
        }
    }
}