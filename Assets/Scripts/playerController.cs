using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{
    // This class controls the player camera and the spawning in of
    [Header("Parameters")]
    [SerializeField]
    float vertSensitivity;

    [SerializeField]
    float horSensitivity;

    [SerializeField]
    float yLowerBound;

    [SerializeField]
    float yUpperBound;

    [Header("Cache Variables")]
    [SerializeField]
    Camera mainCam;

    [Header("State Variables")]
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
    }
}
