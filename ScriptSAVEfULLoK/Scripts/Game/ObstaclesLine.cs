using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstaclesLine : MonoBehaviour
{
    public static float speed = 2f; // Initial speed
    public static float speedIncrement = 0.02f; // How much speed increases over time
    public static float maxSpeed = 8f; // Maximum allowed speed

    private List<GameObject> obstacles;
    private GameObject coin;
    private GameObject shield; // Added shield prefab reference

    private float duration;
    private Vector3 startPos, endPos;
    private bool newLineSpawned;

    void Awake()
    {
        // Set new line position to the top of the screen before anything else.
        transform.position = new Vector3(transform.position.x, 7, transform.position.z);
    }

    void Start()
    {
        // Load obstacles from resources.
        LoadObstacles();
        // Load coin from resources.
        LoadCoin();
        // Spawn obstacles and coin.
        SpawnLineOfObstacles();
        // Get line start position.
        startPos = transform.position;
        // Set line end position.
        endPos = new Vector3(transform.position.x, -7, transform.position.z);
    }

    void Update()
    {
        // Check if GameOver instance exists first
        if (GameOver.instance == null) return;

        // Only proceed if game is not over
        if (!GameOver.instance.crashed)
        {
            // Gradually increase speed, capped at maxSpeed
            if (speed < maxSpeed)
            {
                speed += speedIncrement * Time.deltaTime; // Smoothly increase over time
            }

            // If line hasn't reached end position.
            if (transform.position != endPos)
            {
                // If player speed is higher than 0.
                if (speed != 0)
                {
                    // How long line will travel to the bottom of the screen.
                    duration += Time.deltaTime / (10 - speed);
                    // Move line to the bottom of the screen.
                    transform.position = Vector3.Lerp(startPos, endPos, duration);

                    // How much line has to travel to spawn the new line.
                    if (!newLineSpawned && duration > ObstacleLineSpawner.instance.spawnPlace)
                    {
                        // Spawn new line.
                        ObstacleLineSpawner.instance.SpawnLine();
                        newLineSpawned = true;
                    }
                }
            }
            else
            {
                // Destroy line when it reaches endPos.
                Destroy(gameObject);
            }
        }
    }

    // Load obstacles from resources.
    private void LoadObstacles()
    {
        obstacles = new List<GameObject>();
        Object[] objects = Resources.LoadAll("Obstacles") as Object[];
        foreach (Object item in objects)
        {
            obstacles.Add(item as GameObject);
        }
    }

    // Load coin from resources.
    private void LoadCoin()
    {
        coin = Resources.Load("Coin") as GameObject;
        shield = Resources.Load("Shield") as GameObject; // Added shield loading
    }

    // Spawn obstacle in one of five lanes.
    private void SpawnObstacle(int lane)
    {
        int randomObstacleIndex = Random.Range(0, obstacles.Count);
        float randomObstacleOffest = Random.Range(0, ObstacleLineSpawner.instance.randomizeObstaclesOffest);

        Instantiate(obstacles[randomObstacleIndex], new Vector3(lane, 7 + randomObstacleOffest, 0), Quaternion.identity, transform);
    }

    // Spawn coin in one of five lanes.
    private void SpawnCoin(int lane)
    {
        float randomCoinOffest = Random.Range(0, ObstacleLineSpawner.instance.randomizeObstaclesOffest);

        Instantiate(coin, new Vector3(lane, 7 + randomCoinOffest, 0), Quaternion.identity, transform);
    }

    // New method to spawn shield
    private void SpawnShield(int lane)
    {
        float randomShieldOffset = Random.Range(0, ObstacleLineSpawner.instance.randomizeObstaclesOffest);
        Instantiate(shield, new Vector3(lane, 7 + randomShieldOffset, 0), Quaternion.identity, transform);
    }

    // Spawn obstacles into the line.
    private void SpawnLineOfObstacles()
{
    int minObstacles = ObstacleLineSpawner.instance.minObstacles;
    int maxObstacles = ObstacleLineSpawner.instance.maxObstacles;

    // Get all available lanes.
    List<int> availableLanes = new List<int>() { -2, -1, 0, 1, 2 };
    
    // First, decide if we want to spawn a shield (rare) or coin (more common)
    float shieldSpawnChance = 0.2f; // 5% chance for shield
    bool willSpawnShield = Random.value < shieldSpawnChance;
    bool willSpawnCoin = Random.value < ObstacleLineSpawner.instance.coinSpawnRate && !willSpawnShield; // Only spawn coin if not spawning shield

    // Calculate how many obstacles to spawn, leaving room for powerup
    int maxPossibleObstacles = willSpawnShield || willSpawnCoin ? maxObstacles - 1 : maxObstacles;
    int obstaclesAmount = Random.Range(minObstacles, maxPossibleObstacles + 1);

    // Spawn obstacles
    for (int i = 0; i < obstaclesAmount; i++)
    {
        int randomLaneIndex = Random.Range(0, availableLanes.Count);
        SpawnObstacle(availableLanes[randomLaneIndex]);
        availableLanes.RemoveAt(randomLaneIndex);
    }

    // Spawn shield or coin if we still have lanes available
    if (availableLanes.Count > 0)
    {
        if (willSpawnShield)
        {
            int randomLaneIndex = Random.Range(0, availableLanes.Count);
            SpawnShield(availableLanes[randomLaneIndex]);
        }
        else if (willSpawnCoin)
        {
            int randomLaneIndex = Random.Range(0, availableLanes.Count);
            SpawnCoin(availableLanes[randomLaneIndex]);
        }
    }
}}