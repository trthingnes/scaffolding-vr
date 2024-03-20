using UnityEngine;

//Class for creating blinking effect with a choosen color
public class BlinkingEffect : MonoBehaviour
{
    public Color startColor = Color.green;
    public Color endColor = Color.black;
    public Material originalMaterial;

    [Range(0, 10)]
    public float speed = 1;
    Renderer ren;

    void Start()
    {
        ren = GetComponent<Renderer>();
        originalMaterial = ren.material;
    }

    void OnDisable()
    {
        ren.material = originalMaterial;
    }

    //Creating continious blinking effect
    void Update()
    {
        ren.material.color = Color.Lerp(startColor, endColor, Mathf.PingPong(Time.time * speed, 1));
    }
}
