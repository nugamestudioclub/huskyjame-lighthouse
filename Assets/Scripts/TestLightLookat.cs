using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TestLightLookat : MonoBehaviour
{
    [SerializeField]
    private GameObject lightObject;
    [SerializeField]
    private float rotationSpeed = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out hit))
        {
            print(hit.point);
            Vector3 dir = hit.point - lightObject.transform.position;
            Quaternion toRotation = Quaternion.FromToRotation(transform.forward, dir);
            lightObject.transform.rotation = Quaternion.Lerp(lightObject.transform.rotation, toRotation, rotationSpeed*Time.deltaTime);
        }
    }


}
