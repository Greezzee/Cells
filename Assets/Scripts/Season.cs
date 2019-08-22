using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Season : MonoBehaviour
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
        int season = wi.getInfo("season");
        switch (season)
        {
            case 0: myText.text = "Season: Win"; break;
            case 1: myText.text = "Season: Spr"; break;
            case 2: myText.text = "Season: Sum"; break;
            case 3: myText.text = "Season: Aut"; break;
        }
    }
}
