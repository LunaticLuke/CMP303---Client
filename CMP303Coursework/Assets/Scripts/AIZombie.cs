using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Similar to the NPC class except it predicts the AI zombies
public class AIZombie : MonoBehaviour
{

    float speed = 0.5f;
    [HideInInspector]
    public int id;

    Transform target;

    float[] playerDistances = new float[4];

    Vector2 Direction;

    float stoppingValue = 1.0f;

    public float health = 1;

    Vector2[] recentPositions = new Vector2[2];

    public float timeLastMessageReceived;

    float[] latestMessageTimes = new float[2];

    public float[] latestServerUpdate = new float[2];

    public float latestMessageTime;

    public bool hasMessage = false;

    public Vector2 targetPos;

    public GameObject ghostPos;

    public bool alive = false;

    bool respawning = false;

    float respawnTimer = 4.0f;



    // Start is called before the first frame update
    void Start()
    {
        //Initalise the arrays with current position data
        recentPositions[0] = transform.position;
        recentPositions[1] = transform.position;
        latestMessageTimes[0] = 0;
        latestMessageTimes[1] = 0;
        targetPos = transform.position;
        ghostPos.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //If an update is here
        if(hasMessage)
        {
            
            HandleData();
            //Predict
            targetPos = Prediction();
            hasMessage = false;
        }
        if (alive)
        {
            float step = (1 * speed) * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, targetPos, step);
            ghostPos.transform.position = new Vector2(latestServerUpdate[0], latestServerUpdate[1]);
        }
            if(!respawning && !alive)
            {
                respawning = true;
                StartCoroutine(Respawn());
            }
    }

    public void HandleData()
    {
        recentPositions[0] = recentPositions[1];
        recentPositions[1] = new Vector2(latestServerUpdate[0], latestServerUpdate[1]);

        latestMessageTimes[0] = latestMessageTimes[1];
        latestMessageTimes[1] = latestMessageTime;
    }


    Vector2 Prediction()
    {
        float predictedX, predictedY;

        Vector2 secondFromLastUpdate = recentPositions[0];
        Vector2 lastUpdate = recentPositions[1];

        float speedX = lastUpdate.x - secondFromLastUpdate.x;
        float speedY = lastUpdate.y - secondFromLastUpdate.y;

        if (latestMessageTimes[1] - latestMessageTimes[0] != 0)
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


        float timeSinceLastMessage = GameManager.gameTime - timeLastMessageReceived;


        float displacementX = speedX * timeSinceLastMessage;

        float displacementY = speedY * timeSinceLastMessage;

        predictedX = lastUpdate.x + displacementX; predictedY = lastUpdate.y + displacementY;
        return new Vector2(predictedX, predictedY);

    }

    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }

    public IEnumerator Respawn()
    {
        SetPosition(new Vector2(-500, 0));
        yield return new WaitForSeconds(respawnTimer);
        SetPosition(GameManager.instance.zombieStarts[id]);
        gameObject.SetActive(true);
        recentPositions[0] = transform.position;
        recentPositions[1] = transform.position;
        targetPos = Prediction();
        alive = true;
        respawning = false;
    }

}
