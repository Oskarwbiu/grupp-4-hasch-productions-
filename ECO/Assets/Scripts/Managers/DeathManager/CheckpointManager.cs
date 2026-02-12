using UnityEngine;
using System.Collections.Generic;

public class CheckpointManager : MonoBehaviour
{
    public Checkpoint CurrentActiveInstance;
    public static CheckpointManager Instance;
    private HashSet<int> activatedCheckpoints = new HashSet<int>();
    private Vector3 lastCheckpointPosition = Vector3.zero;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    public bool IsCheckpointActivated(int checkpointID)
    {
        return activatedCheckpoints.Contains(checkpointID);
    }

    public void ActivateCheckpoint(int checkpointID, Vector3 position)
    {
        activatedCheckpoints.Add(checkpointID);
        lastCheckpointPosition = position;
    }

    public Vector3 GetLastCheckpointPosition()
    {
        return lastCheckpointPosition;
    }

    public void ResetCheckpoints()
    {
        activatedCheckpoints.Clear();
        lastCheckpointPosition = Vector3.zero;
    }
}