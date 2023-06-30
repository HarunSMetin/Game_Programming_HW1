using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameMission : MonoBehaviour
{
    public int TotalBoxCount = 12;
    public bool isCompleted = false;
    private int insideBoxes = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (insideBoxes== TotalBoxCount)
        {
            isCompleted = true;
        }
    }
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Interractable")
        {
            insideBoxes ++;
        }

    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Interractable")
        {
            insideBoxes--;
            isCompleted = false;
        }
    }
}
