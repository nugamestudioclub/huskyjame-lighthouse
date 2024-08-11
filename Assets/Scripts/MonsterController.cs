using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MonsterController : MonoBehaviour
{

    // This class controls sea monsters

    // Monsters will spawn in randomly traversing in a set direction until a ship comes into range
    // It will chase the ship down, and go back into traversing in a set direction until another ship in a set direction
    // Monsters will dive back into the water and dissapear after being shown for a extended amount of time
    [Header("Parameters")]
    [SerializeField]
    float wanderSpeed;
    [SerializeField]
    float activeSpeed;
    [SerializeField]
    float shineSpeed;
    [SerializeField]
    float distanceUntilChase;
    [SerializeField]
    float distanceUntilAttack;
    [SerializeField]
    float distanceUntilBreak;
    [SerializeField]
    float lightUntilDive;
    [SerializeField]
    float diveTime;
    [SerializeField]
    float diveSpeed;
    [SerializeField]
    float angleAdjust;

    [Header("Cache Variables")]
    [SerializeField]
    Animator animateControl;
    [SerializeField]
    GameObject beamObject;

    [Header("State Variables")]
    [SerializeField]
    GameObject lockedOnObject;
    [SerializeField]
    ShipController lockedOnShip;
    [SerializeField]
    GhostShipController lockedOnGhostShip;
    [SerializeField]
    float directionFloat;
    [SerializeField]
    float lightUntilDiveLeft = 0;
    enum monsterState
    {
        WANDER,
        CHASE,
        DIVE,
        SHINE
    }
    [SerializeField]
    monsterState currentState = monsterState.WANDER;
    

    public static bool inLight;
    // Start is called before the first frame update
    void Awake()
    {
        animateControl = transform.GetComponentInChildren<Animator>();
        beamObject = GameObject.FindGameObjectWithTag("Beam");
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == monsterState.DIVE)
        {
            diveTime -= Time.deltaTime;
            animateControl.SetBool("Diving", true);
            transform.position += Vector3.down * Time.deltaTime * diveSpeed;
            if(diveTime <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            float currentAngle = 0;
            float currentSpeed = 0;
            Vector3 beamDiff = beamObject.transform.position - gameObject.transform.position;
            beamDiff.y = 0;
            if(currentState != monsterState.SHINE && beamObject.transform.GetChild(0).gameObject.activeInHierarchy && beamDiff.magnitude <= playerController.shineSize)
            {
                currentState = monsterState.SHINE;
                lightUntilDiveLeft = lightUntilDive;
            }
            if(currentState == monsterState.SHINE)
            {
                lightUntilDiveLeft -= Time.deltaTime;
                if (!beamObject.transform.GetChild(0).gameObject.activeInHierarchy || beamDiff.magnitude > playerController.shineSize)
                {
                    currentState = monsterState.WANDER;
                }
                else
                {
                    currentAngle = Mathf.Atan2(-beamDiff.z, -beamDiff.x) * Mathf.Rad2Deg;
                    currentSpeed = shineSpeed;
                }
                if(lightUntilDiveLeft < 0)
                {
                    currentState = monsterState.DIVE;
                }
            }
            else if (currentState == monsterState.WANDER)
            {
                currentAngle = directionFloat;
                currentSpeed = wanderSpeed;
                GameObject[] ships = GameObject.FindGameObjectsWithTag("Ship");
                for(int i = 0; i < ships.Length; i++)
                {
                    Vector3 diff = ships[i].transform.position - gameObject.transform.position;
                    diff.y = 0;
                    if(diff.magnitude <= distanceUntilChase)
                    {
                        GhostShipController gShip = ships[i].GetComponent<GhostShipController>();
                        ShipController ship = ships[i].GetComponent<ShipController>();
                        if((ship != null && ship.currentState != ShipController.boatState.SINK) || (gShip != null && gShip.currentState != GhostShipController.boatState.SINK))
                        {
                            lockedOnGhostShip = gShip;
                            lockedOnShip = ship;
                            lockedOnObject = ships[i];
                            currentState = monsterState.CHASE;
                        }
                    }
                }
            }
            else if(currentState == monsterState.CHASE)
            {
                Vector3 diff = lockedOnObject.transform.position - gameObject.transform.position;
                currentAngle = Mathf.Atan2(diff.z, diff.x) * Mathf.Rad2Deg;
                currentSpeed = activeSpeed;
                if(lockedOnShip != null)
                {
                    if (lockedOnShip.currentState == ShipController.boatState.SINK || diff.magnitude >= distanceUntilBreak)
                    {
                        currentState = monsterState.WANDER;
                    }
                    else if(diff.magnitude <= distanceUntilAttack)
                    {
                        lockedOnShip.currentState = ShipController.boatState.SINK;
                        animateControl.SetTrigger("Attack");
                        currentState = monsterState.WANDER;
                    }
                }
                else if(lockedOnGhostShip != null)
                {
                    if (lockedOnGhostShip.currentState == GhostShipController.boatState.SINK || diff.magnitude >= distanceUntilBreak)
                    {
                        currentState = monsterState.WANDER;
                    }
                    else if (diff.magnitude <= distanceUntilAttack)
                    {
                        lockedOnGhostShip.currentState = GhostShipController.boatState.SINK;
                        animateControl.SetTrigger("Attack");
                        currentState = monsterState.WANDER;
                    }
                }
                else
                {
                    currentState = monsterState.WANDER;
                }
            }
            float zMove = Mathf.Sin(Mathf.Deg2Rad * currentAngle) * currentSpeed * Time.deltaTime;
            float xMove = Mathf.Cos(Mathf.Deg2Rad * currentAngle) * currentSpeed * Time.deltaTime;
            gameObject.transform.position += new Vector3(xMove, 0, zMove);
            transform.rotation = Quaternion.Euler(new Vector3(transform.rotation.eulerAngles.x, -currentAngle + angleAdjust, transform.rotation.eulerAngles.z));
        }
    }
}
