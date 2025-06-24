using System.Collections.Generic;
using UnityEngine;

public class TaskSystem
{
    public List<Task> Tasks { get; private set; }
    public List<Task> InprogressTasks { get; private set; }
    public TaskSystem()
    {
        Tasks = new List<Task>();
        InprogressTasks = new List<Task>();
    }
    public Task RequestNextTask()
    {
        // Worker requests the next task from the task system.
        if (Tasks.Count <= 0)
        {
            return null;
        }
        else
        {
            Task task = Tasks[0];
            task.StartTask();
            InprogressTasks.Add(task);
            Tasks.RemoveAt(0);
            return task;
        }
    }
    public void CompleteTask(Task task)
    {
        // Worker completes the task and returns it to the task system.
        if (task != null && InprogressTasks.Contains(task))
        {
            task.CompleteTask();
            InprogressTasks.Remove(task);
            Debug.Log($"Task '{task.ObjectID}' completed and removed from in-progress tasks.");
        }
        else
        {
            Debug.LogWarning("Cannot complete a task that is not in progress or is null.");
        }
    }
    public void CancelTask(string ObjectID)
    {
        // Remove from InprogressTasks if present
        Task inProgressTask = InprogressTasks.Find(task => task.ObjectID == ObjectID);
        if (inProgressTask != null)
        {
            InprogressTasks.Remove(inProgressTask);
            Debug.Log($"Task '{ObjectID}' removed from in-progress tasks.");
        }

        // Remove from Tasks if present
        Task queuedTask = Tasks.Find(task => task.ObjectID == ObjectID);
        if (queuedTask != null)
        {
            Tasks.Remove(queuedTask);
            Debug.Log($"Task '{ObjectID}' removed from queued tasks.");
        }

        if (inProgressTask == null && queuedTask == null)
        {
            Debug.Log($"Task '{ObjectID}' not found in either in-progress or queued tasks.");
        }
    }
    public void AddTask(Task task)
    {
        // Add a new task to the task system.
        if (task != null)
        {
            if(Tasks.Exists(t => t.ObjectID == task.ObjectID))
            {
                return;
            }
            Tasks.Add(task);
            Debug.Log($"Task '{task.ObjectID}' added to the task system.");
        }
        else
        {
            Debug.LogWarning("Cannot add a null task to the task system.");
        }
    }
}

public class Task
{
    public string ObjectID { get; set; }
    public Vector2 Position { get; set; }
    public TaskType Type { get; set; } = TaskType.None;
    public TaskStatus Status { get; set; }
    public Task(string objectId, Vector2 position, int taskType)
    {
        ObjectID = objectId;
        Status = TaskStatus.NotStarted;
        Type = (TaskType)taskType; // Assuming taskType is an int that maps to TaskType enum
        this.Position = position;
    }
    public void StartTask()
    {
        if (Status == TaskStatus.NotStarted)
        {
            Status = TaskStatus.InProgress;
            Debug.Log($"Task '{ObjectID}' started.");
        }
        else
        {
            Debug.LogWarning($"Task '{ObjectID}' cannot be started because it is already in progress or completed.");
        }
    }
    public void CompleteTask()
    {
        if (Status == TaskStatus.InProgress)
        {
            Status = TaskStatus.Completed;
            Debug.Log($"Task '{ObjectID}' completed.");
        }
        else
        {
            Debug.LogWarning($"Task '{ObjectID}' cannot be completed because it is not in progress.");
        }
    }
}

public enum TaskStatus
{
    NotStarted,
    InProgress,
    Completed
}

public enum TaskType
{
    None,
    Feeding,
    GatherResource,
}
