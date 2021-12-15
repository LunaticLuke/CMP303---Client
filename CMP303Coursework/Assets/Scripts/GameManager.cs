using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public struct NPCPlayer
    {
       public int playerID;
       public bool inUse;
       public NPC playerClass;
    }


    public static GameManager instance;


    //An array to store the player prefabs, id determines which one gets spawned.
    public GameObject[] playerPrefabs = new GameObject[4];
    //The prefabs of the other players - the 3 that arent the id of the player will be spawned in.
    public GameObject[] NPCPlayerPrefabs = new GameObject[4];
    //This class stores the classes of the actual other players that are actually in the world. 
    //The client player's index will be marked as not in use as they are handled separately in the Player class.
    public NPCPlayer[] otherPlayers = new NPCPlayer[4];

    public GameObject zombiePrefab;

    public AIZombie[] zombies = new AIZombie[10];

    public Vector3[] zombieStarts = new Vector3[10];


    public static float gameTime = 0;

    float packetTimer;

    float packetInterval;

    int updatesPerSecond = 5;

    public static int numOfPlayers = 0;

    [HideInInspector]
    public bool spawnPlayer = false;

    public static bool gameStarted = false;

    public static int health = 100;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }else
        {
            Destroy(this);
        }
        packetInterval = 1 / updatesPerSecond;
    }

    private void Update()
    {
        if(spawnPlayer)
        {
            spawnPlayer = false;
            Spawn();
            SpawnZombies();
        }
        if(gameStarted)
        {
            gameTime += Time.deltaTime;
        }
    }

    public void Spawn()
    {
        UIManager.instance.startMenu.SetActive(false);
        for(int i = 0; i < numOfPlayers; i++)
        {
            //If we are at the client's ID
            if (i == Client.instance.id)
            {
                //Spawn the actual player that is affected by input
                Instantiate(playerPrefabs[Client.instance.id]);
                //The NPC slot isnt required for this index.
                otherPlayers[i].inUse = false;
            }else
            {
                //Spawn the other player's GameObject that we will update with prediction/interpolation
                otherPlayers[i].playerClass = Instantiate(NPCPlayerPrefabs[i].GetComponent<NPC>());
                //Set the player ID equal to that of our current index
                otherPlayers[i].playerID = i;
                //This slot is in use
                otherPlayers[i].inUse = true;
            }
        }
        gameTime += (UIManager.instance.pingTimer/2);
        gameStarted = true;
    }

    void SpawnZombies()
    {
        for(int i = 0; i < 10; i++)
        {
            GameObject spawnedZombie = Instantiate(zombiePrefab);
            zombies[i] = spawnedZombie.GetComponent<AIZombie>();
            zombies[i].SetPosition(zombieStarts[i]);
            zombies[i].alive = true;
        }
    }

    public void HandlePositionData(Packet.PlayerStruct playerData)
    {
        if (otherPlayers[0].inUse)
        {
            otherPlayers[0].playerClass.timeLastMessageReceived = gameTime;
            otherPlayers[0].playerClass.latestServerUpdate[0] = playerData.XPosP1;
            otherPlayers[0].playerClass.latestServerUpdate[1] = playerData.YPosP1;
            otherPlayers[0].playerClass.newServerUpdate = true;
            otherPlayers[0].playerClass.zRot = playerData.ZRotP1;
            otherPlayers[0].playerClass.latestGameTime = playerData.timestamp;
        }

        if(otherPlayers[1].inUse)
        {
            otherPlayers[1].playerClass.timeLastMessageReceived = gameTime;
            otherPlayers[1].playerClass.latestServerUpdate[0] = playerData.XPosP2;
            otherPlayers[1].playerClass.latestServerUpdate[1] = playerData.YPosP2;
            otherPlayers[1].playerClass.newServerUpdate = true;
            otherPlayers[1].playerClass.zRot = playerData.ZRotP2;
            otherPlayers[1].playerClass.latestGameTime = playerData.timestamp;
        }

        if (otherPlayers[2].inUse)
        {
            otherPlayers[2].playerClass.timeLastMessageReceived = gameTime;
            otherPlayers[2].playerClass.latestServerUpdate[0] = playerData.XPosP3;
            otherPlayers[2].playerClass.latestServerUpdate[1] = playerData.YPosP3;
            otherPlayers[2].playerClass.newServerUpdate = true;
            otherPlayers[2].playerClass.zRot = playerData.ZRotP3;
            otherPlayers[2].playerClass.latestGameTime = playerData.timestamp;
        }

        if (otherPlayers[3].inUse)
        {
            otherPlayers[3].playerClass.timeLastMessageReceived = gameTime;
            otherPlayers[3].playerClass.latestServerUpdate[0] = playerData.XPosP4;
            otherPlayers[3].playerClass.latestServerUpdate[1] = playerData.YPosP4;
            otherPlayers[3].playerClass.newServerUpdate = true;
            otherPlayers[3].playerClass.zRot = playerData.ZRotP4;
            otherPlayers[3].playerClass.latestGameTime = playerData.timestamp;
        }

    }

    public void HandleBulletData(float xOrigin, float yOrigin, float dirX, float dirY, int _id)
    {
        otherPlayers[_id].playerClass.bulletOrigin[0] = xOrigin;
        otherPlayers[_id].playerClass.bulletOrigin[1] = yOrigin;
        otherPlayers[_id].playerClass.bulletDir[0] = dirX;
        otherPlayers[_id].playerClass.bulletDir[1] = dirY;
        otherPlayers[_id].playerClass.spawnBullet = true;
    }

    public void HandleZombieData(Packet.ZombieStruct data)
    {
        for(int i = 0; i < 10; i++)
        {
            zombies[i].latestServerUpdate[0] = data.XPos[i];
            zombies[i].latestServerUpdate[1] = data.YPos[i];
            zombies[i].latestMessageTime = data.timestamp;
            zombies[i].hasMessage = true;
        }
    }
}
