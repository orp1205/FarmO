using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Worker : MonoBehaviour, IWorker
{
    public WorkerAnim anim;
    public UIWorkerStatus workerStatusUI; // Reference to the UIWorkerStatus component
    public void MoveToWork(Vector2 position, Action onArrivedAtPosition = null)
    {
        anim.SetWalk();
        // Flip the worker to face the target position (original facing is right)
        Vector3 scale = transform.localScale;
        if (position.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x) * -1f; // Face left
        else
            scale.x = Mathf.Abs(scale.x);       // Face right
        transform.localScale = scale;
    }

    public void OnTaskCompleted(Task taskFinish, Action onTaskCompleted = null)
    {
        anim.SetIdle();
        if(taskFinish == null)
        {
            Debug.LogError("Task cannot be null when completing a task.");
            return;
        }
        transform.DOKill(); // Stop any ongoing movement
        FarmSlot farmSlot = null;
        foreach (var item in FindObjectsOfType<FarmSlot>())
        {
            if (item.CompareUID(taskFinish.ObjectID))
            {
                farmSlot = item;
                break;
            }
        }
        switch (taskFinish.Type)
        {
            case TaskType.Feeding:
                Debug.Log($"Task {taskFinish.ObjectID} completed: Farming at position {taskFinish.Position}");
                farmSlot.Feed();
                break;
            case TaskType.GatherResource:
                Debug.Log($"Task {taskFinish.ObjectID} completed: Crafting at position {taskFinish.Position}");
                farmSlot.GatherObject();
                break;
            default:
                Debug.LogWarning($"Task {taskFinish.ObjectID} completed with unknown type at position {taskFinish.Position}");
                break;
        }
        
        onTaskCompleted?.Invoke();
    }

    public void OnWorking(float time)
    {
        workerStatusUI.SetFill(time);
    }

    public void StartWork()
    {
        anim.SetWork();
    }

    public void StopWork(Action onStopWork = null)
    {
        anim.SetIdle();
        transform.DOKill(); // Stop any ongoing movement
        onStopWork?.Invoke();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
