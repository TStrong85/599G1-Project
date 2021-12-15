using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerator : MonoBehaviour
{
    public bool useSeed = false;
    public int seed = 1;
    public bool usePlacementDelay = false;
    public float placementDelay = 0.2f;
    [Space]
    public int targetTrackLength = 10;
    public int maxIterations = 30;
    public TrackSegment startSegment;
    public GameObject FinishLinePrefab;
    public Checkpoint CheckpointPrefab;
    public TrackSegment[] segmentPrefabs;  // Random pool of segments to select from

    [Space]
    // Keep track of the segments that have been spawned
    public List<TrackSegment> placedSegments;
    public List<Checkpoint> placedCheckpoints;

    private void Awake()
    {
        if (useSeed)
        {
            Random.InitState(seed);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (usePlacementDelay)
        {
            StartCoroutine(GenerateTrackWithPlacementDelay(placementDelay));
        } else
        {
            GenerateTrack();
        }
    }

    public float GetValuePerCheckpoint()
    {
        return 1.0f / placedCheckpoints.Count;
    }



    public void GenerateTrack()
    {
        IntializeTrackGenerator();

        // Start generating from the start segment
        Transform currentPivot = startSegment.EndPivot;

        int currTrackLength = 0;
        // Iteratively build the track one segement at a time
        for (int i = 0; i < maxIterations && currTrackLength < targetTrackLength; i++)
        {
            currTrackLength += TryToPlaceSegment(ref currentPivot);
        }

        PlaceCheckpointsAndFinishLine(currentPivot);
    }


    public IEnumerator GenerateTrackWithPlacementDelay(float delay)
    {
        IntializeTrackGenerator();

        // Start generating from the start segment
        Transform currentPivot = startSegment.EndPivot;

        int currTrackLength = 0;
        // Iteratively build the track one segement at a time
        for (int i = 0; i < maxIterations && currTrackLength < targetTrackLength; i++)
        {
            yield return new WaitForSeconds(delay);
            currTrackLength += TryToPlaceSegment(ref currentPivot);
        }

        PlaceCheckpointsAndFinishLine(currentPivot);
    }


    private void IntializeTrackGenerator()
    {
        // Initialized the lists used during generation
        //   Make sure to delete any existing pieces

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

            if (child.gameObject == startSegment.gameObject)
            {
                continue;
            }
            else
            {
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
        }


        if (placedSegments != null)
        {
            placedSegments.Clear();
        }
        else
        {
            placedSegments = new List<TrackSegment>(targetTrackLength);
        }

        if (placedCheckpoints != null)
        {
            placedCheckpoints.Clear();
        }
        else
        {
            placedCheckpoints = new List<Checkpoint>(targetTrackLength);
        }
    }

    // Return the length of track placed, and update the current pivot on successful placement
    private int TryToPlaceSegment(ref Transform currentPivot)
    {
        // Select a segement to spawn at random
        TrackSegment segementToSpawn = segmentPrefabs[Random.Range(0, segmentPrefabs.Length)];

        // Instantiate the segment
        TrackSegment spawnedSegment = Instantiate(segementToSpawn);
        spawnedSegment.transform.SetParent(this.transform, true);

        // Check if the segment can be placed
        if (spawnedSegment.SnapStartPivotToTransform(currentPivot))
        {
            // Reactivate this piece and spawn the next from it's end pivot
            currentPivot = spawnedSegment.EndPivot;


            placedSegments.Add(spawnedSegment);

            spawnedSegment.gameObject.SetActive(false);
            spawnedSegment.gameObject.SetActive(true);

            Debug.LogWarning("Snap!");

            // Track the length of the track so far
            return Mathf.Max(1, spawnedSegment.segmentLength);
        }
        else
        {
            // Delete this piece and try again
            spawnedSegment.gameObject.SetActive(false);
            Destroy(spawnedSegment.gameObject);

            Debug.LogWarning("Failed To Validate Segment Snap");

            return 0;
        }
    }

    private void PlaceCheckpointsAndFinishLine(Transform lastPivot)
    {
        // Instantiate and place the finish line
        Instantiate(FinishLinePrefab, lastPivot.position, lastPivot.rotation, this.transform);


        // Spawn Checkpoint for the start line
        Checkpoint spawnedCheckpoint = Instantiate(CheckpointPrefab);
        spawnedCheckpoint.transform.position = startSegment.EndPivot.position;
        spawnedCheckpoint.transform.rotation = startSegment.EndPivot.rotation;
        spawnedCheckpoint.transform.SetParent(startSegment.transform, true);

        placedCheckpoints.Add(spawnedCheckpoint);


        // Spawn in Checkpoints for all of the segments
        for (int i = 0; i < placedSegments.Count; i++)
        {
            spawnedCheckpoint = Instantiate(CheckpointPrefab);
            spawnedCheckpoint.transform.position = placedSegments[i].EndPivot.position;
            spawnedCheckpoint.transform.rotation = placedSegments[i].EndPivot.rotation;
            spawnedCheckpoint.transform.SetParent(placedSegments[i].transform, true);

            placedCheckpoints.Add(spawnedCheckpoint);
        }


        if (TryGetComponent<Checkpoints>(out var c))
        {
            c.checkPoints.Clear();
            c.checkPoints.AddRange(placedCheckpoints);
        }
    }
}
