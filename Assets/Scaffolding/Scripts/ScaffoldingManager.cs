using System.Collections.Generic;
using UnityEngine;
using Task;

#nullable enable

namespace Scaffolding
{
    public class ScaffoldingManager : MonoBehaviour
    {
        private TaskManager _taskManager;

        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Initializing scaffolding task manager");
            _taskManager = new TaskManager(FindObjectOfType<TaskHolder>());
            _taskManager.CreateBuildTask("FootPiece", "MovableFootPiece", "Plasser bunnskruer");
            _taskManager.CreateBuildTask("LongBeamBottom", "MovableLongBeam", "Fest nedre horisontale bjelker", "Fest lengdebjelker");
            _taskManager.CreateBuildTask("CrossBeamBottom", "MovableCrossBeam", "Fest nedre horisontale bjelker", "Fest tverrbjelker");
            _taskManager.CreateBuildTask("StandardBottom", "MovableStandard", "Sett p� bunnspirer");
            _taskManager.CreateBuildTask("LongBeamTop", "MovableLongBeam", "Fest �vre horisontale bjelker", "Fest lengdebjelker");
            _taskManager.CreateBuildTask("CrossBeamTop", "MovableCrossBeam", "Fest �vre horisontale bjelker", "Fest tverrbjelker");
            _taskManager.CreateBuildTask("Bracing", "MovableBracing", "Fest diagonalstag");
            _taskManager.CreateBuildTask("SteelDeck", "MovableSteelDeck", "Mont�r innplanking");
            _taskManager.CreateBuildTask("LadderBeam", "MovableLadderBeam", "Mont�r stige", "Fest stigebjelke");
            _taskManager.CreateBuildTask("LadderStandard", "MovableLadderStandard", "Mont�r stige", "Fest stigespire");
            _taskManager.CreateBuildTask("Ladder", "MovableLadder", "Mont�r stige", "Fest stige");
            _taskManager.CreateBuildTask("StandardTop", "MovableStandard", "Sett p� toppspirer");
            _taskManager.CreateBuildTask("Railing", "MovableRailing", "Mont�r rekkverk");
            _taskManager.CreateBuildTask("Kickboard", "MovableKickboard", "Mont�r sparkebrett");
            _taskManager.CreateBuildTask("RailingFront", "MovableRailing", "Mont�r framre rekkverk");
            Debug.Log($"Successfully created {_taskManager.count} tasks");
        }

        // Update is called once per frame
        void Update()
        {
            //_taskManager.currentTask.AttemptToCompleteAndActivateNext(_taskManager.GetNextTask());
        }

        [System.Serializable]
        public class TaskManager
        {
            private Task.Task _mainTask;
            private List<BuildTask> _buildTasks;

            public TaskManager(TaskHolder taskHolder)
            {
                _mainTask = taskHolder.GetTask("Bygg stillas");
                _buildTasks = new List<BuildTask>();
            }

            public int count { get => _buildTasks.Count; }
            public BuildTask currentTask { get => _buildTasks.Find(bt => bt.status == BuildTask.Status.ACTIVE); }

            public BuildTask? GetNextTask()
            {
                int nextIndex = _buildTasks.IndexOf(currentTask);
                return nextIndex > count ? _buildTasks[nextIndex] : null;
            }

            public void CreateBuildTask(string fixedPartTag, string matchingMovablePartsTag, string subtaskName, string? stepName = null)
            {
                // We find all FixedParts/GameObjects tagged with the given tag so they can be manipulated as a group
                List<FixedPart> fixedParts = new List<FixedPart>();
                foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag(fixedPartTag))
                {
                    fixedParts.Add(new FixedPart(gameObject));
                }

                // We find all MovableParts/GameObjects tagged with the given tag so they can be checked for collision as a group
                List<MovablePart> movableParts = new List<MovablePart>();
                foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag(matchingMovablePartsTag))
                {
                    movableParts.Add(new MovablePart(gameObject));
                }

                // We do not care if we are dealing with a task, subtask or step, so we cast to ICompletable
                ICompletable tabletTask = stepName == null ?
                    (ICompletable)_mainTask.GetSubtask(subtaskName) :
                    (ICompletable)_mainTask.GetSubtask(subtaskName).GetStep(stepName);

                // Provide the assembled part tag to the objective so the state can be manipulated correctly
                _buildTasks.Add(new BuildTask(fixedParts, movableParts, tabletTask));
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
            private List<MovablePart> _matchingMovableParts;
            private ICompletable _tabletTask;
            private Status _status;

            internal BuildTask(List<FixedPart> fixedParts, List<MovablePart> matchingMovableParts, ICompletable tabletTask)
            {
                _fixedParts = fixedParts;
                _matchingMovableParts = matchingMovableParts;
                _tabletTask = tabletTask;
                _status = Status.INACTIVE;
            }

            public string name { get => _tabletTask.ToString(); }
            public Status status { get => _status; }

            public void AttemptToCompleteAndActivateNext(BuildTask? nextTask)
            {
                foreach (MovablePart mPart in _matchingMovableParts)
                {
                    foreach (FixedPart fPart in _fixedParts)
                    {
                        if (Physics.CheckBox(
                            mPart.gameObject.transform.position,
                            mPart.gameObject.transform.localScale / 10f,
                            mPart.gameObject.transform.rotation,
                            LayerMask.GetMask(fPart.gameObject.tag)
                        ))
                        {
                            Destroy(mPart.gameObject);
                            _matchingMovableParts.Remove(mPart);

                            CompleteTask();
                            Debug.Log($"Completed current task: '{this}'");

                            if (nextTask != null)
                            {
                                nextTask.ActivateTask();
                                Debug.Log($"Activated next task: '{nextTask}'");
                            }
                            else
                            {
                                Debug.Log("Congratulations! Scaffolding is complete.");
                            }
                        }
                    }
                }
            }

            private void ActivateTask()
            {
                _fixedParts.ForEach(part => part.SetState(FixedPart.State.OUTLINED));
                _status = Status.ACTIVE;
            }

            private void CompleteTask()
            {
                _fixedParts.ForEach(part => part.SetState(FixedPart.State.VISIBLE));
                _tabletTask.SetCompleated(true);
                _status = Status.COMPLETED;
            }

            public override string ToString()
            {
                return _tabletTask.ToString();
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

            public MovablePart(GameObject gameObject)
            {
                _gameObject = gameObject;
            }

            public GameObject gameObject { get => _gameObject; }
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
                _outline = _gameObject.AddComponent<BlinkingEffect>();
                SetState(State.INVISIBLE);
            }

            public GameObject gameObject { get => _gameObject; }
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
    }
}
