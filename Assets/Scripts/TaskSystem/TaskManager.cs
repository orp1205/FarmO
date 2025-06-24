using TMPro;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    public TaskSystem taskQueue;
    public WorkerDatabase workerDatabase; // Reference to the WorkerDatabase ScriptableObject
    public Transform spawnPos;

    public InitialWorkerConfig initialWorkerConfig;

    public TextMeshProUGUI workerStatusText;


    // Will change if Upgrade this system
    public void UpdateStatusWorker()
    {
        int numWorker = CountWorker();
        int numIdleWorker = CountIdleWorkers();

        workerStatusText.text = $"{numIdleWorker} / {numWorker}";
    }

    private void Start()
    {
        taskQueue = new TaskSystem();

        WorkerManager.Instance.LoadWorkers();
    }
    public void AddWorker(int WorkerID)
    {
        if (workerDatabase == null)
        {
            Debug.LogError("WorkerDatabase is not assigned in the TaskManager.");
            return;
        }
        WorkerData workerData = workerDatabase.workerDataList.Find(w => w.ID == WorkerID);
        GameObject workerObject = Instantiate(workerData.WorkerPrefab, spawnPos.position, Quaternion.identity, spawnPos);
        TaskWorkerAI taskWorkerAI = workerObject.AddComponent<TaskWorkerAI>();
        taskWorkerAI.SetUp(taskQueue, workerData);
        UpdateStatusWorker();
    }
    public int CountWorker()
    {
        return FindObjectsOfType<TaskWorkerAI>().Length;
    }
    public int CountIdleWorkers()
    {
        int idleCount = 0;
        foreach (var worker in FindObjectsOfType<TaskWorkerAI>())
        {
            if (worker != null && worker.IsIdle())
            {
                idleCount++;
            }
        }
        return idleCount;
    }

    public void AddTask(string ObjectID, Vector2 position, int taskType)
    {
        if(ObjectID == null)
        {
            Debug.LogError("ObjectID cannot be null or empty.");
            return;
        }
        Task task = new Task(ObjectID, position, taskType);

        taskQueue.AddTask(task);
    }
    
    public void CancelTask(string ObjectID)
    {
        if(ObjectID == null)
        {
            Debug.LogError("ObjectID cannot be null or empty.");
            return;
        }
        taskQueue.CancelTask(ObjectID);
        foreach (var worker in FindObjectsOfType<TaskWorkerAI>())
        {
            if (worker != null)
            {
                worker.StopWork(ObjectID); // Stop worker that is working on this task
            }
        }
    }
}
