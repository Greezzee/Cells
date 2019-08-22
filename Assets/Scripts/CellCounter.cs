using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CellCounter : MonoBehaviour
{
    int totalCells;
    Text myText;
    void Start()
    {
        totalCells = 0;
        myText = GetComponent<Text>();
    }
    void FixedUpdate()
    {
        totalCells = GameObject.FindGameObjectsWithTag("Cell").Length;
        myText.text = "Cells alive: " + totalCells.ToString();
    }
}
