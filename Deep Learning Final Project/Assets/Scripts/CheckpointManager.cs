using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public float MaxTimeToReachNextCheckpoint = 30f;
    public float StartTimeLeft = 30f;
    private float TimeLeft = 30f;
    
    public KartAgent kartAgent;
    public Checkpoint nextCheckPointToReach;
    
    private int CurrentCheckpointIndex;
    private List<Checkpoint> Checkpoints;
    private Checkpoint lastCheckpoint;

    public event Action<Checkpoint> reachedCheckpoint;

    private float RewardForAllCheckpoints = 4;
    private float RewardForCompletion = 1;
    private float RewardForSingleCheckpoint;

    void Start()
    {
        UpdateCheckPoints();


        RewardForSingleCheckpoint = (Checkpoints.Count > 0) ? (RewardForAllCheckpoints) / Checkpoints.Count : Mathf.Epsilon;
    }

    public void ResetCheckpoints()
    {
        CurrentCheckpointIndex = 0;
        TimeLeft = (MaxTimeToReachNextCheckpoint > 0) ? MaxTimeToReachNextCheckpoint : StartTimeLeft;
        
        SetNextCheckpoint();
    }

    private void Update()
    {
        TimeLeft -= Time.deltaTime;

        if (TimeLeft < 0f)
        {
            kartAgent.AddReward(-1f);
            kartAgent.EndEpisodeWithPartialReward();
        }
    }

    public void CheckPointReached(Checkpoint checkpoint)
    {
        if (nextCheckPointToReach != checkpoint) return;
        
        lastCheckpoint = Checkpoints[CurrentCheckpointIndex];
        reachedCheckpoint?.Invoke(checkpoint);
        CurrentCheckpointIndex++;

        if (CurrentCheckpointIndex >= Checkpoints.Count)
        {
            kartAgent.AddReward(RewardForCompletion);
            kartAgent.EndEpisode();
        }
        else
        {
            kartAgent.AddReward(RewardForSingleCheckpoint);
            SetNextCheckpoint();
        }
    }

    private void SetNextCheckpoint()
    {
        if (Checkpoints.Count > 0)
        {
            // Only ever give time for reaching a checkpoint, never take it away
            TimeLeft = Mathf.Max(TimeLeft, MaxTimeToReachNextCheckpoint);
            nextCheckPointToReach = Checkpoints[CurrentCheckpointIndex];
            
        }
    }

    public float GetPartialReward()
    {

        // Validate that there is a next and previous checkpoint to reference
        if (CurrentCheckpointIndex < 1 || CurrentCheckpointIndex >= Checkpoints.Count)
        {
            return 0;
        }


        float distBetweenCheckpoints = Vector3.Distance(
                Checkpoints[CurrentCheckpointIndex].transform.position,
                Checkpoints[CurrentCheckpointIndex -1].transform.position);
        float distToNextCheckpoint = Vector3.Distance(
                Checkpoints[CurrentCheckpointIndex].transform.position,
                kartAgent.transform.position);

        // Use distance heuristic to evaluate partial progress
        //   Penalize moving backwards along the track
        float normalizedDist = distToNextCheckpoint / distBetweenCheckpoints;
        return Mathf.Lerp(RewardForSingleCheckpoint,0, normalizedDist);
        
    }

    public void UpdateCheckPoints()
    {
        Checkpoints = kartAgent._trackGenerator.GetComponent<Checkpoints>().checkPoints;
        ResetCheckpoints();
    }
}
