using System;
using UnityEngine;

public interface IWorker
{
    void MoveToWork(Vector2 position, Action onArrivedAtPosition = null);
    void StartWork();
    void OnWorking(float time);
    void OnTaskCompleted(Task taskFinish, Action onTaskCompleted = null);
    void StopWork(Action onStopWork = null);
}
