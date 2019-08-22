using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Time : MonoBehaviour
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
        myText.text = "Time: " + wi.getInfo("hours").ToString() + "." + wi.getInfo("minutes").ToString();
    }
}
