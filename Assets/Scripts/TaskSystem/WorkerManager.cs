using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WorkerManager : MonoBehaviour
{
    public static WorkerManager Instance { get; private set; }

    [SerializeField] private TaskManager taskManager;
    [SerializeField] private WorkerDatabase workerDatabase;
    [SerializeField] private InitialWorkerConfig workerStartConfig;

    private const string SaveKey = "WorkerSaveData";

    [System.Serializable]
    private class IntIntPair
    {
        public int key;
        public int value;
    }

    [System.Serializable]
    private class SaveData
    {
        public List<IntIntPair> workers;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    public void UpdateStatusWorker()
    {
        taskManager.UpdateStatusWorker();
    }
    public void SaveWorkers()
    {
        var allWorkers = FindObjectsOfType<TaskWorkerAI>();
        Dictionary<int, int> workerCounts = new();

        foreach (var worker in allWorkers)
        {
            int id = worker.GetWorkerID();
            if (workerCounts.ContainsKey(id))
                workerCounts[id]++;
            else
                workerCounts[id] = 1;
        }

        SaveData data = new SaveData
        {
            workers = new List<IntIntPair>()
        };

        foreach (var pair in workerCounts)
        {
            data.workers.Add(new IntIntPair { key = pair.Key, value = pair.Value });
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
        Debug.Log("Workers saved.");
    }

    public void LoadWorkers()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            if (workerStartConfig != null)
            {
                foreach (var item in workerStartConfig.initialWorkers)
                {
                    for (int i = 0; i < item.Quantity; i++)
                    {
                        taskManager.AddWorker(item.WorkerID);
                    }
                }
            }
            return;
        }
        string json = PlayerPrefs.GetString(SaveKey);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        foreach (var pair in data.workers)
        {
            for (int i = 0; i < pair.value; i++)
            {
                taskManager.AddWorker(pair.key);
            }
        }
    }
    public void AddWorker(int workerId)
    {
        taskManager.AddWorker(workerId);
        SaveWorkers();
    }
}
