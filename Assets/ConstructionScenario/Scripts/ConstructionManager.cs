using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour
{

    public static ConstructionManager Instance { get; private set; }
    public Narrator _narrator
    {
        get
        {
            if (Narr!= null)
            {
                return Narr;
            }
            else
            {
                Narr = gameObject.GetComponent<Narrator>();
                if (Narr == null)
                {
                    Debug.LogError("Error - Missing Narrator on the ConstructionManager object.");
                    return null;
                }
                else return _narrator;
            }
        }
    }
    private Narrator Narr;
    public Material placeableMat, unplaceableMat;
    public List<GameObject> _temporarySubtaskObjects = new();//to wipe when we switch subtask
    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.

        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _subtask = SubTaskEnum.ZERO;
            SwitchElementVisibility(subtaskObjects[0]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            _subtask = SubTaskEnum.ONE;
            SwitchElementVisibility(subtaskObjects[1]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _subtask = SubTaskEnum.TWO;
            SwitchElementVisibility(subtaskObjects[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            _subtask = SubTaskEnum.THREE;
            SwitchElementVisibility(subtaskObjects[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            _subtask = SubTaskEnum.FOUR;
            SwitchElementVisibility(subtaskObjects[4]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            _subtask = SubTaskEnum.FIVE;
            SwitchElementVisibility(subtaskObjects[5]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            _subtask = SubTaskEnum.SIX;
            SwitchElementVisibility(subtaskObjects[6]);
        }

    }

    public enum SubTaskEnum
    {
        ZERO,
        ONE,
        TWO,
        THREE,
        FOUR,
        FIVE, //technically t2s1
        SIX//technically t2s2
    }


    public SubTaskEnum _subtask = SubTaskEnum.ZERO;
    public GameObject S2_Prefab_WrapRipDecal;
    public GameObject S2_Prefab_Staple;
    public int max_rips;


    public List<GameObject> subtaskObjects = new();

    public GameObject T1S0_VisibleElements;
    public GameObject T1S1_VisibleElements;
    public GameObject T1S2_VisibleElements;
    public GameObject T1S3_VisibleElements;
    public GameObject T1S4_VisibleElements;
    public GameObject T2S1_VisibleElements;
    public GameObject T2S2_VisibleElements;




    public void HasFinishedSubtask(SubTaskEnum stask)
    {
        foreach (var item in _temporarySubtaskObjects)
        {
            Destroy(item);
        }
        _temporarySubtaskObjects.Clear();

        Debug.LogWarning("Player has finished subtask" + stask);
        _subtask = stask + 1;
        switch (_subtask)
        {
            case SubTaskEnum.ZERO:
                SwitchElementVisibility(subtaskObjects[0]);
                _narrator.Narrate("Task 1, Subtask 0 - Safety equipment!<br>Pick up the correct protective equipment.");
                break;
            case SubTaskEnum.ONE:
                SwitchElementVisibility(subtaskObjects[1]);
                _narrator.Narrate("Task 1, Subtask 1 - Create a wooden frame wall \n Pick up a beam from the table and attach it to the corresponding spot at one of the green outlines.");
                break;
            case SubTaskEnum.TWO:
                SwitchElementVisibility(subtaskObjects[2]);
                _narrator.Narrate("Task 1, Subtask 2 - Attach the housewrap for the outer layer, then staple it. Tape the holes, if any.");
                break;
            case SubTaskEnum.THREE:
                SwitchElementVisibility(subtaskObjects[3]);
                _narrator.Narrate("Task 1, Subtask 3 - Insert wood fiber insulation into the frame, then arrange it neatly into place.");
                break;
            case SubTaskEnum.FOUR:
                SwitchElementVisibility(subtaskObjects[4]);
                _narrator.Narrate("Task 1, Subtask 4 - Apply the vapor foil, stapling and taping it.");
                break;

            case SubTaskEnum.FIVE:
                SwitchElementVisibility(subtaskObjects[5]);
                _narrator.Narrate("Task 2, Subtask 1 - Learn the layers of a roof, then fill out a short quiz.");
                break;

            case SubTaskEnum.SIX:
                SwitchElementVisibility(subtaskObjects[6]);
                _narrator.Narrate("Task 2, Subtask 2 - Assemble the tiling of a roof.");
                break;
            default:
                break;
        }

    }


    void DefineSubTaskObjects()
    {
        subtaskObjects = new List<GameObject> { T1S0_VisibleElements ,
    T1S1_VisibleElements ,
    T1S2_VisibleElements ,
    T1S3_VisibleElements ,
    T1S4_VisibleElements ,
    T2S1_VisibleElements ,
    T2S2_VisibleElements };
    }
    void Start()
    {
        DefineSubTaskObjects();

        SwitchElementVisibility(subtaskObjects[0]);
    }

    void SwitchElementVisibility(GameObject subtask)
    {
        foreach (var item in subtaskObjects)
        {
            item.SetActive(false);
        }
        foreach (var item in _temporarySubtaskObjects)
        {
            Destroy(item);
        }
        subtask.SetActive(true);
    }
}
