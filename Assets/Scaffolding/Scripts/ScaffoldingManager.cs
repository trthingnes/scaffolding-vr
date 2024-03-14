using System.Collections.Generic;
using UnityEngine;
using Task;

#nullable enable
public class ScaffoldingManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TaskManager taskManager = new TaskManager(FindObjectOfType<TaskHolder>());

        // TODO: Create tasks by providing GameObject tags and subtask/step names
        taskManager.CreateBuildTask("FixedPartTag", "MatchingMovablePartTag", "SubtaskName", "StepNameOrNull");
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Check for pickup/placement/etc
        // TODO: TaskManager.AttemptToCompleteActiveTask(movablePart)
    }
}

[System.Serializable]
public class TaskManager
{
    private const string MAIN_TASK_NAME = "Bygg stillas";

    private Task.Task _mainTask;
    private List<BuildTask> _buildTasks;

    public TaskManager(TaskHolder taskHolder)
    {
        _mainTask = taskHolder.GetTask(MAIN_TASK_NAME);
        _buildTasks = new List<BuildTask>();
    }

    public BuildTask activeTask { get => _buildTasks.Find(bt => bt.status == BuildTask.Status.ACTIVE); }
    public BuildTask nextTask { get => _buildTasks[_buildTasks.IndexOf(activeTask) + 1]; }

    public void CreateBuildTask(string fixedPartTag, string requiredMovablePartsTag, string subtaskName, string? stepName)
    {
        // We find all FixedParts/GameObjects tagged with the given tag so they can be manipulated as a group
        List<FixedPart> fixedParts = new List<FixedPart>();
        foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag(fixedPartTag))
        {
            fixedParts.Add(new FixedPart(gameObject));
        }

        // We do not care if we are dealing with a task, subtask or step, so we cast to ICompletable
        ICompletable tabletTask = stepName == null ?
            (ICompletable) _mainTask.GetSubtask(subtaskName) :
            (ICompletable) _mainTask.GetSubtask(subtaskName).GetStep(stepName);

        // Provide the assembled part tag to the objective so the state can be manipulated correctly
        _buildTasks.Add(new BuildTask(fixedParts, requiredMovablePartsTag, tabletTask));
    }

    public bool AttemptToCompleteActiveTask(MovablePart movablePart)
    {
        if (activeTask.IsCorrectPart(movablePart))
        {
            activeTask.Complete();
            nextTask.Activate();
            return true;
        }

        return false; 
    }
}

public interface ICompletable
{
    bool Compleated();
    void SetCompleated(bool isCompleated);
}

[System.Serializable]
public class BuildTask
{
    private List<FixedPart> _fixedParts;
    private string _requiredMovablePartTag;
    private ICompletable _tabletTask;
    private Status _status;

    internal BuildTask(List<FixedPart> fixedParts, string requiredMovablePartTag, ICompletable tabletTask)
    {
        _fixedParts = fixedParts;
        _requiredMovablePartTag = requiredMovablePartTag;
        _tabletTask = tabletTask;
        _status = Status.INACTIVE;
    }

    public Status status { get => _status; }

    public void Activate()
    {
        _fixedParts.ForEach(part => part.SetState(FixedPart.State.OUTLINED));
        _status = Status.ACTIVE;
    }

    public void Complete()
    {
        _fixedParts.ForEach(part => part.SetState(FixedPart.State.VISIBLE));
        _tabletTask.SetCompleated(true);
        _status = Status.COMPLETED;
    }

    public bool IsCorrectPart(MovablePart movablePart)
    {
        return _requiredMovablePartTag == movablePart.tag;
    }

    public enum Status
    {
        INACTIVE, ACTIVE, COMPLETED
    }
}

[System.Serializable]
public class MovablePart
{
    private GameObject _gameObject;

    public MovablePart(string name)
    {
        _gameObject = GameObject.Find(name);
    }

    public string name { get => _gameObject.name; }
    public string tag { get => _gameObject.tag; }
}

[System.Serializable]
public class FixedPart
{
    private GameObject _gameObject;
    private BlinkingEffect _outline;
    private State _state;

    public FixedPart(GameObject gameObject)
    {
        _gameObject = gameObject;
        _outline = _gameObject.GetComponent<BlinkingEffect>();
        _state = State.INVISIBLE;
    }

    public string name { get => _gameObject.name; }
    public string tag { get => _gameObject.tag; }
    public State state { get => _state; }

    public void SetState(State state)
    {
        if (_gameObject == null)
            return;

        switch (state)
        {
            case State.INVISIBLE:
                _gameObject.SetActive(false);
                _outline.enabled = false;
                break;
            case State.OUTLINED:
                _gameObject.SetActive(true);
                _outline.enabled = true;
                break;
            case State.VISIBLE:
                _gameObject.SetActive(true);
                _outline.enabled = false;
                break;
        }

        _state = state;
    }

    public enum State
    {
        INVISIBLE, OUTLINED, VISIBLE
    }
}
