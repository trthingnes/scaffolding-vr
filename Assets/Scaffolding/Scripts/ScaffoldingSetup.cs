using UnityEngine;
using System.Collections.Generic;

public class ScaffoldingSetup : MonoBehaviour
{
    public List<ScaffoldingPart> scaffoldingParts;
    scaffoldingParts.Add(new ScaffoldingPart("ScaffoldingPart1"));
    scaffoldingParts.Add(new ScaffoldingPart("ScaffoldingPart2"));
    scaffoldingParts.Add(new ScaffoldingPart("ScaffoldingPart3"));

    public List<ScaffoldingPickUpPart> scaffoldingPickUpParts;
    scaffoldingParts.Add(new ScaffoldingPickUpPart("ScaffoldingPickUpPart1"));
    scaffoldingParts.Add(new ScaffoldingPickUpPart("ScaffoldingPickUpPart2"));
    scaffoldingParts.Add(new ScaffoldingPickUpPart("ScaffoldingPickUpPart3"));

    private int currentPartIndex = 0;

    void Start()
    {
        InitializeScaffolding(scaffoldingParts, false);
    }

    void Update()
    {
        if (currentPartIndex < scaffoldingParts.Count)
        {
            ScaffoldingPart scaffoldingPart = scaffoldingParts[currentPartIndex];
            ScaffoldingPickUpPart pickUpPart = scaffoldingPickUpParts[currentPartIndex];
            SetAllObjectsVisibility(scaffoldingPart)

            if (scaffoldingPart.IsVisible)
            {
                Debug.Log($"Pick up the {scaffoldingPart.Name}.");

                PickUpPart(pickUpPart);
                Debug.Log($"Place the {pickUpPart.Name} on the scaffolding.");
                PlacePartOnScaffolding(pickUpPart, scaffoldingPart);

                // Move to the next part
                if (scaffoldingParts[currentPartIndex].DoneTask)
                    currentPartIndex++;
            }
        }
        else
        {
            Debug.Log("Congratulations! Scaffolding is complete.");
        }
    }

    void InitializeScaffolding(List<ScaffoldingPart> parts, bool isVisible)
    {
        // Make all parts invisible initially
        foreach (ScaffoldingPart child in part)
        {
            child.SetVisibility(isVisible);
        }
    }

    void SetAllObjectsVisibility(ScaffoldingPart parent, bool isVisible)
    {
        parent.SetVisibility(isVisible);
        foreach (GameObject child in parent.Part)
        {
            BlinkingEffect highlightObject = child.AddComponent<BlinkingEffect>();
        }
    }

    void PickUpPart(ScaffoldingPickUpPart part)
    {
        // Check if the part exists
        if (part == null)
            return;

        // Find the hand GameObject by name
        GameObject handObject = GameObject.Find("Hand");

        if (handObject != null)
        {
            // Get the collider component from the hand GameObject
            Collider handCollider = handObject.GetComponent<Collider>();

            // Check for collisions with the hand collider
            bool isCollidingWithHand = Physics.CheckBox(part.Part.transform.position, part.Part.transform.localScale / 2f, part.Part.transform.rotation, LayerMask.GetMask("Hand"));

            // If the part is colliding with the hand, scale it to half size
            if (isCollidingWithHand)
            {
                part.Part.transform.localScale = Vector3.one * 0.5f;
            }
            else
            {
                // If the part is not colliding with the hand anymore, scale it to its usual size
                part.Part.transform.localScale = Vector3.one;
            }
        }
    }

    void PlacePartOnScaffolding(ScaffoldingPickUpPart pickUpPart, ScaffoldingPart scaffoldingPart)
    {
        // Implement VR interactions to place the part on the scaffolding
        // the object that the player is holding should have layer name same as the object it is going to be replaces with after collision
        bool isCollidingWithHand = Physics.CheckBox(pickUpPart.Part.transform.position, pickUpPart.Part.transform.localScale / 2f, pickUpPart.Part.transform.rotation, LayerMask.GetMask(scaffoldingPart.Name));
        if (isCollidingWithHand)
        {
            BlinkingEffect highlightObject = part.Part.GetComponent<BlinkingEffect>();
            highlightObject.enabled = false;
            Destroy(pickUpPart.Part); 
            scaffoldingPart.DoneTask = true
        }
    }
}

[System.Serializable]
public class ScaffoldingPart
{
    public string Name;
    public bool IsVisible;
    public GameObject Part;
    public bool DoneTask;

    public ScaffoldingPart(string name)
    {
        Name = name;
        IsVisible = false;
        Part = GameObject.Find(name)
        DoneTask = false
    }

    public void SetVisibility(bool isVisible)
    {
        if (Part != null)
        {
            Part.SetActive(isVisible);
            IsVisible = isVisible;
        }
    }
}

[System.Serializable]
public class ScaffoldingPickUpPart
{
    public string Name;
    public GameObject Part;

    public ScaffoldingPart(string name)
    {
        Name = name;
        Part = GameObject.Find(name)
    }
}

