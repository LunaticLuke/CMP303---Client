using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;





 public struct InputMessage
{
    public char typeOfMessage;
    public float xPos;
    public float yPos;
    public float timeStamp;
    public float zRot;
};

//18 Bytes
public struct BulletMessage
{
    public char typeOfMessage; // 2 Bytes
    public float originX; // 4 bytes
    public float originY; //4 Bytes
    public float dirX; // 4 bytes
    public float dirY; //4 bytes
    public int id; //4 bytes
}


public class Player : MonoBehaviour
{
    public static Player instance;

    Vector3 InputVector;

    public float[] latestServerUpdate = new float[2];

    InputMessage lastSentMessage;

    public List<InputMessage> serverReceivedMessages = new List<InputMessage>();

    public bool newServerUpdate = false;

    public float latestTimeUpdate;

    public float timeElapsed;

    public GameObject ghost;

    public Rigidbody2D body;

    const int ammoClip = 10;

    int currentAmmo = 10;


    public Bullet[] bullets = new Bullet[ammoClip];

    public Transform gunOrigin;



    Vector3 mouse_pos;
    Vector3 screenPoint;
    Vector2 offset;

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
        latestServerUpdate[0] = transform.position.x;
        latestServerUpdate[1] = transform.position.y;
        ghost.transform.SetParent(null);
    }

    // Update is called once per frame
    void Update()
    {
       InputVector.x = Input.GetAxisRaw("Horizontal");
       InputVector.y = Input.GetAxisRaw("Vertical");
        timeElapsed += Time.deltaTime;
        if(timeElapsed >= 0.25f)
        {
            if (transform.position.x != lastSentMessage.xPos || transform.position.y != lastSentMessage.yPos)
            {
                SendPositionData();
            }
            
            timeElapsed = 0;
        }
       
        
         if(Input.GetMouseButtonDown(0) && currentAmmo > 0)
        {
            Shoot();
            currentAmmo--;
        }


        mouse_pos = Input.mousePosition;
        screenPoint = Camera.main.WorldToScreenPoint(transform.localPosition);
        offset = new Vector2(mouse_pos.x - screenPoint.x, mouse_pos.y - screenPoint.y);
        float angle = Mathf.Atan2(offset.y, offset.x) * Mathf.Rad2Deg;
        ghost.transform.position = new Vector2(latestServerUpdate[0], latestServerUpdate[1]);
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void FixedUpdate()
    {
        body.velocity = new Vector2(InputVector.x, InputVector.y);
    }

    public void SendPositionData()
    {
            InputMessage playerData;
            playerData.typeOfMessage = 'p'; // 2 bytes
            playerData.xPos = transform.position.x; // 4 bytes
            playerData.yPos = transform.position.y; // 4 bytes
            playerData.timeStamp = GameManager.gameTime; // 4 bytes
            playerData.zRot = transform.eulerAngles.z; // 4 bytes

            byte[] data = new byte[22];
            Array.Copy(BitConverter.GetBytes(playerData.typeOfMessage), 0, data, 0, 2);
            Array.Copy(BitConverter.GetBytes(playerData.xPos), 0, data, 2, 4);
            Array.Copy(BitConverter.GetBytes(playerData.yPos), 0, data, 6, 4);
            Array.Copy(BitConverter.GetBytes(playerData.timeStamp), 0, data, 10, 4);
            Array.Copy(BitConverter.GetBytes(playerData.zRot), 0, data, 14, 4);
            Array.Copy(BitConverter.GetBytes(Client.instance.id), 0, data, 18, 4);

            lastSentMessage = playerData;
            Client.instance.SendTCP(data);
    }




    public void Shoot()
    {
        for(int i = 0; i < ammoClip; i++)
        {
            if(!bullets[i].simulating)
            {
                bullets[i].Fire(gunOrigin.position, offset);
                Physics2D.IgnoreCollision(bullets[i].GetComponent<Collider2D>(), GetComponent<Collider2D>());

                BulletMessage message;

                message.typeOfMessage = 'b';
                message.originX = gunOrigin.position.x;
                message.originY = gunOrigin.position.y;
                message.dirX = offset.x;
                message.dirY = offset.y;
                message.id = Client.instance.id;

                byte[] data = new byte[22];
                Array.Copy(BitConverter.GetBytes(message.typeOfMessage), 0, data, 0, 2);
                Array.Copy(BitConverter.GetBytes(message.originX), 0, data, 2, 4);
                Array.Copy(BitConverter.GetBytes(message.originY), 0, data, 6, 4);
                Array.Copy(BitConverter.GetBytes(message.dirX), 0, data, 10, 4);
                Array.Copy(BitConverter.GetBytes(message.dirY), 0, data, 14, 4);
                Array.Copy(BitConverter.GetBytes(message.id), 0, data, 18, 4);

                Client.instance.SendTCP(data);

                break;
            }
        }
    }


}
