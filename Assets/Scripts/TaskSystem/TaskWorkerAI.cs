using DG.Tweening;
using System;
using UnityEngine;

public class TaskWorkerAI : MonoBehaviour
{
    private IWorker _worker;
    private State _state;
    private TaskSystem _taskSystem;
    private Task _currentTask;

    private float waitingTimer;
    private float MaxWaitingTime = 0.2f; // Maximum time to wait for a task
    private float WaitingForMovingtoPosition = 1f; // Time to wait for moving to a position
    private float taskExecutionTimer;
    private float MaxTaskExecutionTime = 2f; // Maximum time to execute a task
    
    // Speed of the worker's movement
    public float speed = 5f; // Speed of the worker's movement

    private WorkerData workerData; // Reference to the worker data
    public void SetUp(TaskSystem taskSystem, WorkerData data)
    {
        this._worker = this.gameObject.GetComponent<Worker>();
        this._taskSystem = taskSystem;
        this._state = State.WaitingForTask;
        this.workerData = data; // Store the worker data
        this.speed = data.Speed; // Set the speed from worker data
        this.MaxTaskExecutionTime = data.WorkTime*60; // Set the maximum task execution time from worker data
        taskExecutionTimer = 0; // Initialize task execution timer
    }
    public int GetWorkerID()
    {
        return workerData.ID;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public bool IsIdle()
    {
        // Check if the worker is idle (waiting for a task)
        return _state == State.WaitingForTask;
    }
    // Update is called once per frame
    void Update()
    {
        if(_taskSystem == null || _worker == null)
        {
            return;
        }
        switch (_state)
        {
            case State.WaitingForTask:
                // Wait for a task to be assigned
                waitingTimer -= Time.deltaTime;
                if (waitingTimer <= 0f)
                {
                    // Check for a task assignment, if none, reset the timer
                    RequestTask();
                    waitingTimer = MaxWaitingTime; // Reset timer to wait for the next task
                }
                else
                {
                    // Optionally, perform idle behavior while waiting
                }
                break;
            case State.MoveToTask:
                // Move to the task position
                WaitingForMovingtoPosition -= Time.deltaTime;
                if(WaitingForMovingtoPosition <= 0f)
                {
                    // Arrived at the task position, start executing the task
                    _state = State.ExcutingTask;
                    taskExecutionTimer = 0; // Reset task execution timer
                }
                else
                {
                    // Continue moving towards the task position

                }
                break;
            case State.ExcutingTask:
                // Execute the task
                taskExecutionTimer += Time.deltaTime;
                if (taskExecutionTimer >= MaxTaskExecutionTime)
                {
                    // Task execution time is up, complete the task
                    OnTaskCompleted(_currentTask);
                    taskExecutionTimer = 0; // Reset timer for the next task
                }
                else
                {
                    // Continue executing the current task
                    float time = Mathf.InverseLerp(0, MaxTaskExecutionTime, taskExecutionTimer);
                    OnWorking(time);
                }
                break;
        }
    }

    private void RequestTask()
    {
        Task task = _taskSystem.RequestNextTask();
        if (task != null)
        {
            _state = State.MoveToTask;
            _currentTask = task;
            MoveToWork(task);
        }
        else
        {
            _state = State.WaitingForTask;
        }
    }
    public void StopWork(string ObjectID)
    {
        // Stop the worker if it is currently working on a task with the given ObjectID
        if (_currentTask != null && _currentTask.ObjectID == ObjectID)
        {
            _worker.StopWork();
            _state = State.WaitingForTask;
        }
        WorkerManager.Instance.UpdateStatusWorker();
    }
    private void MoveToWork(Task task)
    {
        float distance = Vector2.Distance(transform.position, task.Position);
        WaitingForMovingtoPosition = distance / speed; // Calculate time to reach the task position based on speed
        transform.DOMove(task.Position, WaitingForMovingtoPosition).SetEase(Ease.Linear).OnStart(() =>
        {
            _worker.MoveToWork(task.Position, () => _state = State.MoveToTask);
        }).OnComplete(() =>
        {
            _state = State.ExcutingTask;
            taskExecutionTimer = 0; // Reset task execution timer when arrived at position
            _worker.StartWork();
        });
        WorkerManager.Instance.UpdateStatusWorker();
    }
    private void OnTaskCompleted(Task task)
    {
        // Handle task completion
        if (_currentTask != null)
        {
            _taskSystem.CompleteTask(_currentTask);
            _currentTask = null;
            _worker.OnTaskCompleted(task, () => _state = State.WaitingForTask);
        }
        WorkerManager.Instance.UpdateStatusWorker();
    }

    private void OnWorking(float time)
    {
        // Handle the worker's ongoing work, if needed
        if (_currentTask != null)
        {
            _worker.OnWorking(time);
        }
    }
    private enum State
    {
        WaitingForTask,
        MoveToTask,
        ExcutingTask
    }
}
