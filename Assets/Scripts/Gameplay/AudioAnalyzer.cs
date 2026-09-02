using System.Collections;
using UnityEngine;

public class AudioAnalyzer : MonoBehaviour
{
    [Header("Audio Analysis Settings")]
    public AudioSource source;
    public int fftSize = 2048; // Higher = more accurate but slower (must be power of 2)

    [Header("Detection Thresholds - Adjust these in the Inspector")]
    [Tooltip("Volume below this is considered a quiet period")]
    public float quietThreshold = 0.15f;

    [Tooltip("Bass energy above this triggers drum beat detection")]
    public float drumThreshold = 0.4f;

    [Tooltip("High frequency energy above this triggers high note detection")]
    public float highNoteThreshold = 0.3f;

    [Header("Frequency Ranges (0-1, where 1 = Nyquist frequency)")]
    [Tooltip("Bass frequencies for drum detection")]
    public float bassRangeEnd = 0.1f;

    [Tooltip("High frequencies for melody detection")]
    public float highRangeStart = 0.6f;

    [Header("Smoothing")]
    [Tooltip("How much to smooth the readings (0 = no smoothing, 1 = very smooth)")]
    public float smoothingFactor = 0.8f;

    // Output values
    public float currentVolume { get; private set; }
    public float bassEnergy { get; private set; }
    public float highEnergy { get; private set; }
    public float midEnergy { get; private set; }

    // State flags
    public bool isQuietPeriod { get; private set; }
    public bool isDrumBeat { get; private set; }
    public bool isHighNote { get; private set; }

    // Beat detection
    private float lastBeatTime = 0f;
    public float beatInterval => Time.time - lastBeatTime;
    private float beatCooldown = 0.25f; // Minimum time between beats

    // Smoothing buffers
    private float smoothedVolume;
    private float smoothedBass;
    private float smoothedHigh;
    private float smoothedMid;

    // Spectrum data
    private float[] spectrumData;

    void Start()
    {
        if (source == null)
            source = GetComponent<AudioSource>();

        spectrumData = new float[fftSize / 2];
    }

    void Update()
    {
        if (source == null || !source.isPlaying)
            return;

        // Get spectrum data
        source.GetSpectrumData(spectrumData, 0, FFTWindow.BlackmanHarris);

        // Calculate energy in different frequency ranges
        CalculateFrequencyRanges();

        // Apply smoothing
        ApplySmoothing();

        // Update state flags
        UpdateState();
    }

    void CalculateFrequencyRanges()
    {
        int bassEnd = Mathf.FloorToInt(bassRangeEnd * spectrumData.Length);
        int highStart = Mathf.FloorToInt(highRangeStart * spectrumData.Length);

        float bassSum = 0f;
        float midSum = 0f;
        float highSum = 0f;
        float totalSum = 0f;

        for (int i = 0; i < spectrumData.Length; i++)
        {
            float value = spectrumData[i];
            totalSum += value;

            if (i < bassEnd)
                bassSum += value;
            else if (i < highStart)
                midSum += value;
            else
                highSum += value;
        }

        // Normalize by dividing by the number of samples in each range
        currentVolume = totalSum / spectrumData.Length;
        bassEnergy = bassSum / bassEnd;
        midEnergy = midSum / (highStart - bassEnd);
        highEnergy = highSum / (spectrumData.Length - highStart);
    }

    void ApplySmoothing()
    {
        smoothedVolume = Mathf.Lerp(smoothedVolume, currentVolume, 1f - smoothingFactor);
        smoothedBass = Mathf.Lerp(smoothedBass, bassEnergy, 1f - smoothingFactor);
        smoothedHigh = Mathf.Lerp(smoothedHigh, highEnergy, 1f - smoothingFactor);
        smoothedMid = Mathf.Lerp(smoothedMid, midEnergy, 1f - smoothingFactor);
    }

    void UpdateState()
    {
        // Check for quiet period
        isQuietPeriod = smoothedVolume < quietThreshold;

        // Check for drum beat (bass spike)
        bool drumDetected = smoothedBass > drumThreshold;
        if (drumDetected && (Time.time - lastBeatTime) > beatCooldown)
        {
            isDrumBeat = true;
            lastBeatTime = Time.time;
        }
        else
        {
            isDrumBeat = false;
        }

        // Check for high note
        isHighNote = smoothedHigh > highNoteThreshold;
    }

    // Helper method to get spawn recommendation based on current audio
    public SpawnRecommendation GetSpawnRecommendation()
    {
        SpawnRecommendation recommendation = new SpawnRecommendation();

        if (isQuietPeriod)
        {
            recommendation.shouldSpawn = false;
            recommendation.spawnZone = SpawnZone.None;
            return recommendation;
        }

        recommendation.shouldSpawn = true;

        // Prioritize spawn zone based on audio content
        if (isHighNote)
        {
            recommendation.spawnZone = SpawnZone.High;
        }
        else if (isDrumBeat)
        {
            recommendation.spawnZone = SpawnZone.Low;
        }
        else
        {
            // Random based on energy distribution
            if (smoothedBass > smoothedHigh)
                recommendation.spawnZone = SpawnZone.Low;
            else
                recommendation.spawnZone = SpawnZone.High;
        }

        // Intensity affects spawn probability
        recommendation.intensity = Mathf.Clamp01(smoothedVolume * 2f);

        return recommendation;
    }

    // Visualize in Scene View (optional, for debugging)
    void OnDrawGizmosSelected()
    {
        if (spectrumData == null) return;

        Gizmos.color = Color.green;
        for (int i = 0; i < spectrumData.Length; i += 10)
        {
            float height = spectrumData[i] * 10f;
            Gizmos.DrawLine(
                new Vector3(i * 0.1f, 0, 0),
                new Vector3(i * 0.1f, height, 0)
            );
        }
    }
}

// Data structures for spawn recommendations
[System.Serializable]
public class SpawnRecommendation
{
    public bool shouldSpawn;
    public SpawnZone spawnZone;
    public float intensity; // 0-1, how intense the audio is
}

public enum SpawnZone
{
    None,
    Low,    // For bass/drums
    Mid,    // For mid-range
    High    // For high notes/melody
}
