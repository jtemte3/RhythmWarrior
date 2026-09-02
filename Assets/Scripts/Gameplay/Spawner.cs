using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Level Setup")]
    public GameObject[] cubes;

    [Header("Spawn Points by Zone")]
    [Tooltip("Low spawn points for bass/drums")]
    public Transform[] lowPoints;

    [Tooltip("High spawn points for melody/high notes")]
    public Transform[] highPoints;

    [Tooltip("Optional: Mid spawn points")]
    public Transform[] midPoints;

    [Header("Audio Reactivity")]
    public AudioAnalyzer audioAnalyzer;

    [Header("Spawn Settings")]
    [Tooltip("Minimum time between spawns (seconds)")]
    public float minSpawnInterval = 0.3f;

    [Tooltip("Maximum time between spawns (seconds)")]
    public float maxSpawnInterval = 0.8f;

    [Tooltip("How much audio intensity affects spawn rate (higher = more responsive)")]
    public float intensityMultiplier = 1.5f;

    [Tooltip("Chance to spawn during quiet periods anyway (0-1)")]
    public float quietSpawnChance = 0.1f;

    // Legacy support - still use BPM as fallback
    public bool isSongActive;
    public float beat;
    public float timer;
    public float endTime;

    // Audio-reactive spawning
    private float lastSpawnTime = 0f;
    private float nextSpawnTime = 0f;
    private float spawnInterval;

    void Start()
    {
        // Fallback: if no zone-specific points, use all points for each zone
        if (lowPoints.Length == 0 && highPoints.Length == 0)
        {
            Debug.LogWarning("No zone-specific spawn points assigned. Using legacy random spawning.");
        }

        spawnInterval = (minSpawnInterval + maxSpawnInterval) / 2f;
    }

    void Update()
    {
        if (!isSongActive || Time.time >= endTime)
            return;

        // Use audio-reactive spawning if analyzer is available
        if (audioAnalyzer != null && audioAnalyzer.source.isPlaying)
        {
            AudioReactiveSpawn();
        }
        else
        {
            // Fallback to BPM-based spawning
            LegacyBPMSpawn();
        }
    }

    void AudioReactiveSpawn()
    {
        SpawnRecommendation recommendation = audioAnalyzer.GetSpawnRecommendation();

        // Check if we should spawn
        bool shouldSpawn = false;

        if (recommendation.shouldSpawn)
        {
            // Time-based check with audio-influenced interval
            float adjustedInterval = spawnInterval / (1f + recommendation.intensity * intensityMultiplier);
            adjustedInterval = Mathf.Clamp(adjustedInterval, minSpawnInterval, maxSpawnInterval);

            if (Time.time - lastSpawnTime >= adjustedInterval)
            {
                shouldSpawn = true;
            }
        }
        else if (Random.value < quietSpawnChance)
        {
            // Occasionally spawn during quiet periods for variety
            shouldSpawn = true;
        }

        if (shouldSpawn)
        {
            SpawnCube(recommendation.spawnZone);
            lastSpawnTime = Time.time;
        }
    }

    void SpawnCube(SpawnZone zone)
    {
        Transform spawnPoint = GetSpawnPointForZone(zone);

        if (spawnPoint != null)
        {
            GameObject cube = Instantiate(cubes[Random.Range(0, cubes.Length)], spawnPoint.position, spawnPoint.rotation);
            cube.transform.parent = null;

            // Optional: Adjust cube properties based on audio intensity
            Cube cubeScript = cube.GetComponent<Cube>();
            if (cubeScript != null && audioAnalyzer != null)
            {
                // Faster cubes during intense moments
                cubeScript.speed *= 1f + audioAnalyzer.currentVolume * 0.5f;
            }
        }
    }

    Transform GetSpawnPointForZone(SpawnZone zone)
    {
        switch (zone)
        {
            case SpawnZone.Low:
                return lowPoints.Length > 0 ? lowPoints[Random.Range(0, lowPoints.Length)] : null;

            case SpawnZone.High:
                return highPoints.Length > 0 ? highPoints[Random.Range(0, highPoints.Length)] : null;

            case SpawnZone.Mid:
                return midPoints.Length > 0 ? midPoints[Random.Range(0, midPoints.Length)] : null;

            default:
                // Fallback: use any available point
                List<Transform> allPoints = new List<Transform>();
                allPoints.AddRange(lowPoints);
                allPoints.AddRange(highPoints);
                allPoints.AddRange(midPoints);

                if (allPoints.Count > 0)
                    return allPoints[Random.Range(0, allPoints.Count)];

                return null;
        }
    }

    void LegacyBPMSpawn()
    {
        if (timer > beat)
        {
            // Try to use zone points, fallback to legacy behavior
            List<Transform> allPoints = new List<Transform>();
            allPoints.AddRange(lowPoints);
            allPoints.AddRange(highPoints);
            allPoints.AddRange(midPoints);

            Transform spawnPoint = allPoints.Count > 0
                ? allPoints[Random.Range(0, allPoints.Count)]
                : null;

            if (spawnPoint != null)
            {
                GameObject cube = Instantiate(cubes[Random.Range(0, cubes.Length)], spawnPoint.position, spawnPoint.rotation);
                cube.transform.parent = null;
            }

            timer -= beat;
        }

        timer += Time.deltaTime;
    }
}
