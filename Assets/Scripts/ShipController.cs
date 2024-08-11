using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    // This class controls ships

    // This boat randomly wanders away from the outer borders and at a distance from the lighthouse
    // This boat will traverse to the spot that the lighthouse beams
    // If a monster is in a lighthouse beam the boat will travel the oppisite direction
    // This boat will dissapear and increment the succsess counter on each
    [Header("Behavior Parameters")]
    [SerializeField]
    float speed;
    [SerializeField]
    float turnRate;
    [SerializeField]
    float turnAccel;
    [SerializeField]
    float activeTurnRate;
    // Distance that ship sees beam from
    [SerializeField]
    float beamSightDistance;
    // Distance until beam 
    [SerializeField]
    float beamBreakDistance;
    // Distance that ship moves from lighthouse until traveling back
    [SerializeField]
    float maxDistanceAway;
    // Distance that ship moves towards lighthouse until traveling away from it
    [SerializeField]
    float minDistanceAway;
    [SerializeField]
    float sinkSpeed;
    [SerializeField]
    float sinkTime;
    [SerializeField]
    float angleOffSet;

    [Header("Cache Variables")]
    [SerializeField]
    GameObject lightHouse;
    [SerializeField]
    GameObject lightBeam;
    [SerializeField]
    ShipSilhouetteTrigger[] silhouetteTriggers;

    [Header("State Variables")]
    [SerializeField]
    float currentAngle;
    public enum boatState{
        WANDER,
        FOLLOW,
        SINK
    }
    [SerializeField]
    public boatState currentState = boatState.WANDER;
    [SerializeField]
    float wanderTurnVel;

    [Header("Debug indicators")]
    [SerializeField]
    float lightHouseDistance;


    // Start is called before the first frame update
    void Awake()
    {
        lightHouse = GameObject.FindGameObjectWithTag("LightHouse");
        lightBeam = GameObject.FindGameObjectWithTag("Beam");
    }

    // Flattens angle to be from 0-359
    // Uses degrees
    float flattenAngle(float angle)
    {
        float retAngle = angle % 360;
        if(retAngle < 0)
        {
            retAngle += 360;
        }
        return retAngle;
    }
    // Causes ship to moves towards a point
    void moveTowards(Vector3 point)
    {
        Vector3 diff = point - gameObject.transform.position;
        float pointAngle = Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg;
        pointAngle = flattenAngle(pointAngle);
        float flatCurrent = flattenAngle(currentAngle);
        if(flatCurrent > pointAngle)
        {
            if(360 - flatCurrent + pointAngle >= flatCurrent - pointAngle)
            {
                currentAngle -= Time.deltaTime * activeTurnRate;
            }
            else
            {
                currentAngle += Time.deltaTime * activeTurnRate;
            }
        }
        else
        {
            if (flatCurrent + 360 - pointAngle >= pointAngle - flatCurrent)
            {
                currentAngle += Time.deltaTime * activeTurnRate;
            }
            else
            {
                currentAngle -= Time.deltaTime * activeTurnRate;
            }
        }
    }
    // Causes ship to move away from a point
    void moveAway(Vector3 point)
    {
        Vector3 diff = point - gameObject.transform.position;
        float pointAngle = -Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg;
        pointAngle = flattenAngle(pointAngle);
        float flatCurrent = flattenAngle(currentAngle);
        if (flatCurrent > pointAngle)
        {
            if (360 - flatCurrent + pointAngle >= flatCurrent - pointAngle)
            {
                currentAngle -= Time.deltaTime * activeTurnRate;
            }
            else
            {
                currentAngle += Time.deltaTime * activeTurnRate;
            }
        }
        else
        {
            if (flatCurrent + 360 - pointAngle >= pointAngle - flatCurrent)
            {
                currentAngle += Time.deltaTime * activeTurnRate;
            }
            else
            {
                currentAngle -= Time.deltaTime * activeTurnRate;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if(currentState != boatState.SINK)
        {
            Vector3 beamDiff = lightBeam.transform.position - gameObject.transform.position;
            beamDiff.y = 0;
            if (lightBeam.transform.GetChild(0).transform.gameObject.activeInHierarchy && beamDiff.magnitude <= playerController.shineSize)
            {
                for(int i = 0; i < silhouetteTriggers.Length; i++)
                {
                    silhouetteTriggers[i].triggerClear();
                }
            }
            else
            {
                for (int i = 0; i < silhouetteTriggers.Length; i++)
                {
                    silhouetteTriggers[i].triggerSilhouette();
                }
            }
            if (currentState == boatState.WANDER)
            {
                Vector3 lightHouseDiff = lightHouse.transform.position - gameObject.transform.position;
                lightHouseDiff.y = 0;
                lightHouseDistance = lightHouseDiff.magnitude;
                if (lightHouseDiff.magnitude >= maxDistanceAway)
                {
                    moveTowards(lightHouse.transform.position);
                }
                else if (lightHouseDiff.magnitude <= minDistanceAway)
                {
                    moveAway(lightHouse.transform.position);
                }
                else
                {
                    wanderTurnVel = Mathf.Clamp(wanderTurnVel + Time.deltaTime * Random.Range(-turnAccel, turnAccel), -turnRate, turnRate);
                    currentAngle += Time.deltaTime * wanderTurnVel;
                }
                if (lightBeam.transform.GetChild(0).transform.gameObject.activeInHierarchy && beamDiff.magnitude <= beamSightDistance)
                {
                    currentState = boatState.FOLLOW;
                }
            }
            else if (currentState == boatState.FOLLOW)
            {
                if (!lightBeam.transform.GetChild(0).transform.gameObject.activeInHierarchy || beamDiff.magnitude >= beamBreakDistance)
                {
                    currentState = boatState.WANDER;
                }
                else if (MonsterController.inLight)
                {
                    moveAway(lightBeam.transform.position);
                }
                else
                {
                    moveTowards(lightBeam.transform.position);
                }
            }
            float zMove = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * speed * Time.deltaTime;
            float xMove = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * speed * Time.deltaTime;
            gameObject.transform.position += new Vector3(xMove, 0, zMove);
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, -currentAngle + angleOffSet, transform.rotation.eulerAngles.z));
        }
        else
        {
            for (int i = 0; i < silhouetteTriggers.Length; i++)
            {
                silhouetteTriggers[i].triggerClear();
            }
            sinkTime -= Time.deltaTime;
            gameObject.transform.position += sinkSpeed * Vector3.down * Time.deltaTime;
            if(sinkTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
