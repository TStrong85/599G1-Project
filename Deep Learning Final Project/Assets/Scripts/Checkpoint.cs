using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<CheckpointManager>(out var c))
        {
            c.CheckPointReached(this);
        }
    }
}
