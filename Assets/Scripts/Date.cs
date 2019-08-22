using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Date : MonoBehaviour
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
        myText.text = wi.getInfo("day").ToString() + "." + wi.getInfo("month").ToString() + "." + wi.getInfo("year").ToString();
    }
}
