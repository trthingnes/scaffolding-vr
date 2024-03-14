using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Task;

#nullable enable
public class TaskManager : MonoBehaviour
{
    private const string MAIN_TASK_NAME = "Bygg stillas";
    private Dictionary<string, BuildObjective> _objectives = new Dictionary<string, BuildObjective>();

    // Start is called before the first frame update
    void Start()
    {
        TaskHolder tabletTaskHolder = FindObjectOfType<TaskHolder>();
        Task.Task tabletMainTask = tabletTaskHolder.GetTask(MAIN_TASK_NAME);

        // Tasks should be built in reverse order because they activate the next task when they are completed
        BuildObjective secondTask = RegisterBuildObjective("Tag", "Tag", tabletMainTask, "SubtaskName", null, null);
        BuildObjective firstTask = RegisterBuildObjective("Tag", "Tag", tabletMainTask, "SubtaskName", "StepName", secondTask);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private BuildObjective RegisterBuildObjective(string loosePartsTag, string assembledPartsTag, Task.Task mainTask, string subtaskName, string? stepName, BuildObjective? next)
    {
        // We do not care if we are dealing with a task, subtask or step, so we cast to ICompletable
        ICompletable objective = stepName == null ?
            (ICompletable) mainTask.GetSubtask(subtaskName) : 
            (ICompletable) mainTask.GetSubtask(subtaskName).GetStep(stepName);

        // We find all GameObjects tagged with the given tag so they can be manipulated as a group
        List<GameObject> parts = new List<GameObject>();
        parts.AddRange(GameObject.FindGameObjectsWithTag(assembledPartsTag));

        // Provide the assembled part tag to the objective so the state can be manipulated correctly
        BuildObjective buildObjective = new BuildObjective(objective, parts, next);
        _objectives[loosePartsTag] = buildObjective;

        return buildObjective;
    }

    public void RegisterPickup(string loosePartTag)
    {
        // If we want pickup to be a step of every objective we can use this method
    }

    public void RegisterPlacement(string loosePartTag)
    {
        _objectives[loosePartTag].Complete();
    }
}

public class BuildObjective
{
    private ICompletable _objective;
    private List<GameObject> _parts;
    private BuildObjective? _next;

    internal BuildObjective(ICompletable objective, List<GameObject> parts, BuildObjective? next)
    {
        _objective = objective;
        _parts = parts;
        _next = next;
    }
    
    // Objectives are activated when the player is supposed to place it's part next
    public void Activate()
    {
        _parts.ForEach(go =>
        {
            // TODO: Manipulate game objects to show the player a skeleton of where to place an item
        });
    }

    // Objectives are completed when the player places a part correctly, this triggers the next objective's Activate method
    public void Complete()
    {
        _objective.SetCompleated(true);
        _parts.ForEach(go =>
        {
            // TODO: Manipulate game objects to show the parts as placed correctly
        });

        if (_next != null)
        {
            _next.Activate();
        }
    }
}

internal interface ICompletable
{
    bool Compleated();
    void SetCompleated(bool isCompleated);
}
