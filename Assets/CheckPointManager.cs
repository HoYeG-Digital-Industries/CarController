using System.Collections.Generic;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{
    public enum CheckpointMode
    {
        Ordered,
        Random
    }
    
    [Header("Settings")]
    public CheckpointMode mode = CheckpointMode.Ordered;
    public List<Checkpoint> checkpoints = new List<Checkpoint>();

    private int currentIndex = -1;
    private Checkpoint currentCheckpoint;

    private void Start()
    {
        if (checkpoints.Count == 0)
        {
            Debug.LogError("No checkpoints assigned!");
            return;
        }

        foreach (Checkpoint checkpoint in checkpoints)
        {
            checkpoint.Deactivate();
        }
        
        // Enable the first checkpoint
        SetNextCheckpoint();
    }

    public void OnCheckpointReached(Checkpoint checkpoint)
    {
        if (checkpoint != currentCheckpoint)
            return; // Ignore wrong checkpoint

        Debug.Log($"Checkpoint {checkpoint.name} reached!");

        // Disable current checkpoint
        checkpoint.Deactivate();

        // Enable next checkpoint
        SetNextCheckpoint();
    }

    private void SetNextCheckpoint()
    {
        if (checkpoints.Count == 0)
            return;

        if (mode == CheckpointMode.Ordered)
        {
            currentIndex = (currentIndex + 1) % checkpoints.Count;
        }
        else if (mode == CheckpointMode.Random)
        {
            int nextIndex;
            do
            {
                nextIndex = Random.Range(0, checkpoints.Count);
            } while (nextIndex == currentIndex && checkpoints.Count > 1); // Avoid repeating the same one

            currentIndex = nextIndex;
        }

        currentCheckpoint = checkpoints[currentIndex];
        currentCheckpoint.Activate();
        Debug.Log($"Next checkpoint: {currentCheckpoint.name}");
    }
}
