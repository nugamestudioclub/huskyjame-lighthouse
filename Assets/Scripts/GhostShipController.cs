using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostShipController : MonoBehaviour
{
    // This class controls ghost ships

    // They will wander randomly exactly like regular ships while not disturbed
    // They will de increment the amount of ships successfully ported on reaching island
    // They will begin moving towards player when player when player is shining light and doesn't see player
    // Once they are discovered (Light flashes on them), they will begin beelining towards the lighthouse
    [Header("Behavior Parameters")]
    // >>> Original Ship Parameters <<<
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

    // >>> Ghost ship parameters <<<
    // How far away
    [SerializeField]
    float angleOutUntilInvade;

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
    public enum boatState
    {
        WANDER,
        INVADE,
        FOLLOW,
        SINK,
        BEELINE
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
        if (retAngle < 0)
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
        if (currentState != boatState.SINK)
        {
            Vector3 beamDiff = lightBeam.transform.position - gameObject.transform.position;
            beamDiff.y = 0;
            if(lightBeam.transform.GetChild(0).gameObject.activeInHierarchy && beamDiff.magnitude <= playerController.shineSize)
            {
                currentState = boatState.BEELINE;
                for (int i = 0; i < silhouetteTriggers.Length; i++)
                {
                    silhouetteTriggers[i].triggerClear();
                }
            }
            else if(currentState != boatState.BEELINE)
            {
                for (int i = 0; i < silhouetteTriggers.Length; i++)
                {
                    silhouetteTriggers[i].triggerSilhouette();
                }
            }
            if (currentState == boatState.INVADE)
            {
                Vector3 lightHouseDiff = lightHouse.transform.position - gameObject.transform.position;
                lightHouseDiff.y = 0;
                moveTowards(lightHouse.transform.position);
                Vector3 beamHouseDiff = lightBeam.transform.position - lightHouse.transform.position;
                beamHouseDiff.y = 0;
                float angleDiff = Mathf.Abs(Vector3.Angle(beamHouseDiff, -lightHouseDiff));
                if (!lightBeam.transform.GetChild(0).gameObject.activeInHierarchy || angleDiff < angleOutUntilInvade)
                {
                    currentState = boatState.WANDER;
                }
            }
            else if (currentState == boatState.WANDER)
            {
                Vector3 lightHouseDiff = lightHouse.transform.position - gameObject.transform.position;
                lightHouseDiff.y = 0;
                lightHouseDistance = lightHouseDiff.magnitude;
                if (lightHouseDiff.magnitude >= maxDistanceAway)
                {
                    print("naw");
                    moveTowards(lightHouse.transform.position);
                }
                else if (lightHouseDiff.magnitude <= minDistanceAway)
                {
                    print("wow");
                    moveAway(lightHouse.transform.position);
                }
                else
                {
                    wanderTurnVel = Mathf.Clamp(wanderTurnVel + Time.deltaTime * Random.Range(-turnAccel, turnAccel), -turnRate, turnRate);
                    currentAngle += Time.deltaTime * wanderTurnVel;
                }
                if (lightBeam.transform.GetChild(0).gameObject.activeInHierarchy)
                {
                    Vector3 beamHouseDiff = lightBeam.transform.position - lightHouse.transform.position;
                    beamHouseDiff.y = 0;
                    float angleDiff = Mathf.Abs(Vector3.Angle(beamHouseDiff, -lightHouseDiff));
                    print(angleDiff);
                    if(angleDiff >= angleOutUntilInvade)
                    {
                        currentState = boatState.INVADE;
                    }
                    else if(beamDiff.magnitude <= beamSightDistance)
                    {
                        currentState = boatState.FOLLOW;
                    }
                }
            }
            else if (currentState == boatState.FOLLOW)
            {
                if (!lightBeam.transform.GetChild(0).gameObject.activeInHierarchy || beamDiff.magnitude >= beamBreakDistance)
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
            else
            {
                moveTowards(lightHouse.transform.position);
            }
           
            float zMove = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * speed * Time.deltaTime;
            float xMove = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * speed * Time.deltaTime;
            gameObject.transform.position += new Vector3(xMove, 0, zMove);
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, -currentAngle + angleOffSet, transform.rotation.eulerAngles.z));
        }
        else
        {
            sinkTime -= Time.deltaTime;
            gameObject.transform.position += sinkSpeed * Vector3.down * Time.deltaTime;
            if (sinkTime <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
