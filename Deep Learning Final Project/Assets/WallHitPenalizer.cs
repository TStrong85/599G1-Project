using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;

public class WallHitPenalizer : MonoBehaviour
{
    public KartAgent kartAgent;
    public float rewardPerSecond = -0.1f;

    private StatsRecorder statsRecorder;


    private void Awake()
    {
        statsRecorder = Academy.Instance.StatsRecorder;
    }

    // Apply a reward/penalty every frame that this trigger volume is intersecting with a wall
    private void OnTriggerStay(Collider other)
    {
        //Debug.Log("trigger");
        if (other.gameObject.tag.Equals("Wall"))
        {
            kartAgent.AddReward(rewardPerSecond * Time.fixedDeltaTime);
            //Debug.Log("wall hit!");
            statsRecorder.Add("Wall Penalty", rewardPerSecond * Time.fixedDeltaTime, StatAggregationMethod.Sum);
        }
    }
}
