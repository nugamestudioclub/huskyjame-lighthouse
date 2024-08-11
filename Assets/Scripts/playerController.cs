using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class playerController : MonoBehaviour
{
    // This class controls the player camera and how the light works
    [Header("Parameters")]
    [SerializeField]
    float vertSensitivity;

    [SerializeField]
    float horSensitivity;

    [SerializeField]
    float yLowerBound;

    [SerializeField]
    float yUpperBound;

    [SerializeField]
    float adjustSpeed;

    [SerializeField]
    float adjustLock;

    [SerializeField]
    public static float shineSize = (float)4.5;

    [Header("Cache Variables")]
    [SerializeField]
    Camera mainCam;

    [SerializeField]
    GameObject lightObject;

    [SerializeField]
    GameObject lightIndicator;

    [Header("State Variables")]
    [SerializeField]
    bool lightBeamed;
    [SerializeField]
    float vert;
    [SerializeField]
    float hor;

    // Start is called before the first frame update
    void Start()
    {
        // Finds cache vars
        mainCam = Camera.main;
        // Locks cursor and hides it
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float horAngle = hor + (Input.GetAxis("Mouse X") * Time.deltaTime * vertSensitivity);
        float vertAngle = vert - (Input.GetAxis("Mouse Y") * Time.deltaTime * horSensitivity);
        vertAngle = Mathf.Clamp(vertAngle, yLowerBound, yUpperBound);
        vert = vertAngle;
        hor = horAngle;
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(vert + 90, hor, gameObject.transform.rotation.eulerAngles.z));
        if(Input.GetAxis("BeamRay") != 0)
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (lightBeamed)
                {
                    Vector3 diff = hit.point - lightIndicator.transform.position;
                    lightIndicator.transform.position += diff.normalized * Time.deltaTime * adjustSpeed;
                    if(diff.magnitude <= adjustLock)
                    {
                        lightIndicator.transform.position = hit.point;
                    }
                    Vector3 dir = lightIndicator.transform.position - lightObject.transform.position;
                    lightObject.transform.rotation = Quaternion.LookRotation(dir.normalized, dir.normalized);
                }
                else
                {
                    lightIndicator.transform.GetChild(0).gameObject.SetActive(true);
                    lightObject.SetActive(true);
                    lightBeamed = true;
                    lightIndicator.transform.position = hit.point;
                }
            }
        }
        else
        {
            lightBeamed = false;
            lightIndicator.transform.GetChild(0).gameObject.SetActive(false);
            lightObject.SetActive(false);
        }
    }
}
