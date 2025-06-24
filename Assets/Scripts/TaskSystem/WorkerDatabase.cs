using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorkerDatabase", menuName = "Scriptable Objects/WorkerDatabase")]
public class WorkerDatabase : ScriptableObject
{
    public List<WorkerData> workerDataList = new List<WorkerData>();
}
[Serializable]
public class WorkerData
{
    public int ID;
    public string Name;
    public int PurchaseCost;
    public float Speed;
    public float WorkTime;
    public Sprite IconWorker;
    public GameObject WorkerPrefab;
}
