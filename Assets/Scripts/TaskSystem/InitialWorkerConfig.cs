using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InitialWorkerConfig", menuName = "Scriptable Objects/InitialWorkerConfig")]
public class InitialWorkerConfig : ScriptableObject
{
    [Header("Initial Workers Configuration")]
    public List<WorkerEntry> initialWorkers = new List<WorkerEntry>();
}

[Serializable]
public class WorkerEntry
{
    public int WorkerID;
    public int Quantity;
}