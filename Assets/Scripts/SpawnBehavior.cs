using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnBehavior : MonoBehaviour
{
    [SerializeField]
    GameObject JumpScare;
    [SerializeField]
    GameObject ghostPrefab;
    [SerializeField]
    GameObject shipPrefab;
    [SerializeField]
    GameObject monsterPrefab;
    [SerializeField]
    string failScene;
    [SerializeField]
    public int[] shipNeeded;
    [SerializeField]
    public string[] scenePerDay;
    [SerializeField]
    public float[] shipSpawnTime;
    [SerializeField]
    float shipSpawnLeft = 0;
    [SerializeField]
    public float[] monsterSpawnTime;
    [SerializeField]
    float monsterSpawnLeft;
    [SerializeField]
    float spawnYMon;
    [SerializeField]
    float spawnYShip;
    [SerializeField]
    float spawnYGhost;
    [SerializeField]
    float monsterSpawnVarience;
    [SerializeField]
    public float[] ghostSpawnTime;
    [SerializeField]
    float ghostSpawnLeft;
    [SerializeField]
    int currentDay;
    [SerializeField]
    int setDay;
    [SerializeField]
    int spawnDist;
    [SerializeField]
    float scareTime;
    [SerializeField]
    public bool scared;

    public static int shipsCounted = 0;
    public static int days = 0;
    public static SpawnBehavior instance;
    // Start is called before the first frame update
    void Start()
    {
        if (setDay != 0)
        {
            days = setDay;
        }
        shipSpawnLeft = 0;
        monsterSpawnLeft = monsterSpawnTime[days];
        ghostSpawnLeft = ghostSpawnTime[days];
        shipsCounted = 0;
        currentDay = days;
        instance = this;
        JumpScare = GameObject.FindGameObjectWithTag("Scare");
    }

    // Update is called once per frame
    void Update()
    {
        if (scared)
        {
            scareTime -= Time.deltaTime;
            for(int i = 0; i < JumpScare.transform.childCount; i++)
            {
                JumpScare.transform.GetChild(i).transform.gameObject.SetActive(true);
            }
            if(scareTime < 0)
            {
                SceneManager.LoadScene(failScene);
            }
        }
        else
        {
            if (shipsCounted >= shipNeeded[days])
            {
                days++;
                SceneManager.LoadScene(scenePerDay[days - 1]);
            }
            shipSpawnLeft -= Time.deltaTime;
            monsterSpawnLeft -= Time.deltaTime;
            ghostSpawnLeft -= Time.deltaTime;

            if (shipSpawnLeft < 0)
            {
                shipSpawnLeft = shipSpawnTime[days];
                float randAngle = Random.Range(0, 359);
                GameObject createOb = Instantiate(shipPrefab);
                createOb.transform.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * randAngle) * spawnDist, spawnYShip, Mathf.Sin(Mathf.Deg2Rad * randAngle) * spawnDist);
            }
            if (monsterSpawnLeft < 0)
            {
                monsterSpawnLeft = monsterSpawnTime[days];
                float randAngle = Random.Range(0, 359);
                GameObject createOb = Instantiate(monsterPrefab);
                createOb.transform.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * randAngle) * spawnDist, spawnYMon, Mathf.Sin(Mathf.Deg2Rad * randAngle) * spawnDist);
                createOb.GetComponent<MonsterController>().directionFloat = -randAngle + Random.Range(-monsterSpawnVarience, monsterSpawnVarience);
            }
            if (ghostSpawnLeft < 0)
            {
                ghostSpawnLeft = ghostSpawnTime[days];
                float randAngle = Random.Range(0, 359);
                GameObject createOb = Instantiate(ghostPrefab);
                createOb.transform.position = new Vector3(Mathf.Cos(Mathf.Deg2Rad * randAngle) * spawnDist, spawnYGhost, Mathf.Sin(Mathf.Deg2Rad * randAngle) * spawnDist);
            }
        }
    }
}
