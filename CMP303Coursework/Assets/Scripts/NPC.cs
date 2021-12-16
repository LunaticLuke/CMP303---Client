using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{

    public float[] latestServerUpdate = new float[2];
    public bool newServerUpdate = false;
    public float latestGameTime;

    public float timeLastMessageReceived;

    public float zRot;

    public float[] latestXValues = new float[2];
    public float[] latestYValues = new float[2];
    public float[] latestMessageTimes = new float[2];


    const int ammoClip = 10;

    public Bullet[] bullets = new Bullet[ammoClip];

    public bool spawnBullet = false;

    int idToSpawn;

    public float[] bulletDir = new float[2];

    public float[] bulletOrigin = new float[2];

    public GameObject ghostObject;

    public Vector2 targetPos;
    public Vector2 startPos;

    float elapsedTime;
    float estimatedDelay;


    // Start is called before the first frame update
    void Start()
    {
        latestXValues[0] = transform.position.x;
        latestXValues[1] = transform.position.x;
        latestYValues[0] = transform.position.y;
        latestYValues[1] = transform.position.y;
        startPos = transform.position;
        targetPos = transform.position;
        latestMessageTimes[0] = 0;
        latestMessageTimes[1] = 0;
        ghostObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if(newServerUpdate)
        {
            HandleData();
            targetPos = Prediction();
            newServerUpdate = false;
            startPos = transform.position;
            transform.rotation = Quaternion.Euler(0, 0, zRot);
        }
        float step = 1 * Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position,targetPos,  step);
        ghostObject.transform.position = new Vector2(latestServerUpdate[0], latestServerUpdate[1]);
       

        //transform.position = Prediction();

        if(spawnBullet)
        {
            spawnBullet = false;
            Shoot();
        }
    }

     
    public void HandleData()
    {
        latestXValues[0] = latestXValues[1];
        latestYValues[0] = latestYValues[1];

        latestXValues[1] = latestServerUpdate[0];
        latestYValues[1] = latestServerUpdate[1];

        latestMessageTimes[0] = latestMessageTimes[1];
        latestMessageTimes[1] = latestGameTime;
        estimatedDelay = latestMessageTimes[1] - latestMessageTimes[0];
        elapsedTime = 0;
    }

     Vector2 Prediction()
    {
        float predictedX, predictedY;

        Vector2 secondFromLastUpdate = new Vector2(latestXValues[0],latestYValues[0]);
        Vector2 lastUpdate = new Vector2(latestXValues[1], latestYValues[1]);
        
       float speedX = lastUpdate.x - secondFromLastUpdate.x;
       float speedY = lastUpdate.y - secondFromLastUpdate.y;

        float timeDifference = latestMessageTimes[1] - latestMessageTimes[0];

        if (timeDifference != 0)
        {
            if (speedX != 0)
            {
                speedX /= latestMessageTimes[1] - latestMessageTimes[0];

            }

            if (speedY != 0)
            {
                speedY /= latestMessageTimes[1] - latestMessageTimes[0];
            }
        }
       

         float timeSinceLastMessage = timeLastMessageReceived - latestMessageTimes[1];


        float displacementX = speedX * timeSinceLastMessage;

        float displacementY = speedY * timeSinceLastMessage;

        predictedX = lastUpdate.x + displacementX; predictedY = lastUpdate.y + displacementY;
        elapsedTime = 0;
        return new Vector2(predictedX, predictedY);

    }

    float calcDistance(Vector2 One, Vector2 Two)
    {
        return Mathf.Sqrt(Mathf.Pow(One.x - Two.x, 2) + Mathf.Pow(One.y - Two.y, 2));
        
    }

    public void Shoot()
    {
        for (int i = 0; i < ammoClip; i++)
        {
            if (!bullets[i].simulating)
            {
                bullets[i].gameObject.SetActive(true);
                bullets[i].Fire(new Vector2(bulletOrigin[0], bulletOrigin[1]), new Vector2(bulletDir[0], bulletDir[1]));
                //Physics2D.IgnoreCollision(bullets[i].GetComponent<Collider2D>(), transform.GetComponent<Collider2D>());
                break;
            }
        }
    }


}
