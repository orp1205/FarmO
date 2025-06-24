using UnityEngine;

public class WorkerAnim : MonoBehaviour
{
    public Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetIdle()
    {
        // Implement idle animation logic here
        animator.SetTrigger("Idle");
    }
    public void SetWork()
    {
        // Implement work animation logic here
        animator.SetTrigger("Work");
    }
    public void SetWalk()
    {
        // Implement walk animation logic here
        animator.SetTrigger("Walk");
    }
}
