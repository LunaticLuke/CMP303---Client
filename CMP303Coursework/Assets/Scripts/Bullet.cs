using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{

    Vector2 Direction;

    public Rigidbody2D body;

    public bool simulating = false;

    float speed = 5;


    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        this.transform.parent.SetParent(null);
        Direction = new Vector2(0.0f, 0.0f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        body.velocity = Direction * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
     
        if(collision.transform.tag == "Wall")
        {
            simulating = false;
            gameObject.SetActive(false);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.transform.tag == "Enemy")
        {
            simulating = false;
            byte[] data = Packet.createCollisionInfo(other.GetComponent<AIZombie>().id);
            Client.instance.SendTCP(data);
            gameObject.SetActive(false);
            Debug.Log("Hit Zombie");
        }
    }


    public void Fire(Vector2 origin,Vector2 _dir)
    {
        gameObject.SetActive(true);
        transform.position = origin;
        Direction =  _dir.normalized;
        simulating = true;
    }
}
