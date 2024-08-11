using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextModify1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "Day: " +  SpawnBehavior.days;
        gameObject.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>().text = "Ships Needed: " + SpawnBehavior.instance.shipNeeded[SpawnBehavior.days];
        gameObject.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>().text = "Ships Guided: " + SpawnBehavior.shipsCounted;
    }
}
