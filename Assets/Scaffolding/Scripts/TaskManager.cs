using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Task;

public class TaskManager : MonoBehaviour
{
    private const string MAIN_TASK_NAME = "Bygg stillas";

    private Dictionary<string, ScaffoldingTask> _tasks = new Dictionary<string, ScaffoldingTask>();

    // Start is called before the first frame update
    void Start()
    {
        TaskHolder taskHolder = FindObjectOfType<TaskHolder>();
        Task.Task mainTask = taskHolder.GetTask(MAIN_TASK_NAME);

        RegisterScaffoldingTask(mainTask, "SubtaskName", "StepName", "Tag");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void RegisterScaffoldingTask(Task.Task mainTask, string subtaskName, string? stepName, string partsTag)
    {
        // We do not care if we are dealing with a task, subtask or step, so we cast to ICompletable
        ICompletable objective = stepName == null ?
            (ICompletable) mainTask.GetSubtask(subtaskName) : 
            (ICompletable) mainTask.GetSubtask(subtaskName).GetStep(stepName);

        ScaffoldingTask task = new ScaffoldingTask(objective, partsTag);
    }
}

protected class ScaffoldingTask
{
    private ICompletable _task;
    private List<GameObject> _parts = new List<GameObject>();

    public ScaffoldingTask(ICompletable objective, string partsTag)
    {
        _parts.AddRange(GameObject.FindGameObjectsWithTag(partsTag));
    }
}

public interface ICompletable
{
    bool Compleated();
    void SetCompleated(bool isCompleated);
}
