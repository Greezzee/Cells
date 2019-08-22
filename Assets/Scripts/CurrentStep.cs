using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentStep : MonoBehaviour
{
    public GameObject world;
    private WorldInfo wi;
    private Text myText;
    void Start()
    {
        wi = world.GetComponent<WorldInfo>();
        myText = GetComponent<Text>();
    }
    void FixedUpdate()
    {
        myText.text = "Current step: " + wi.getInfo("currentStep").ToString();
    }
}
