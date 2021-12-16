using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;




public class Client : MonoBehaviour
{
    //Instance of the client as there should only be one.
    public static Client instance;
    //Size of chat buffer
    public static int dataBufferSize = 8192;
    //Ip to connect to. In this case of this project, I set it to my public Ip in the editor.
    public string ip;
    //The port to connect to. I opened this port up on my router for this project.
    public int port = 11000;
    public int id = 0;
    public TCP tcp;
    //public UDP udp;

    private void Awake()
    {
        //If there is no instance
        if(instance == null)
        {
            //Make this the instance
            instance = this;
        }
        //Otherwise, destroy this
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        //Create a new TCP instance for this client.
        tcp = new TCP();
      
    }

    public void SendTCP(byte[] data)
    {
        //If the socket is connected
        if (tcp.socket.Connected)
        {
            //Send what is in the textBox
            tcp.Send(data);
        }
    }

    /*public void sendUDP(string data)
    {
        if (udp.socket.Connected)
        {
            //Send what is in the textBox
            udp.Send(data);
        }
    }*/

    public void ConnectToServer(string _ip)
    {
        //Connect to the server using the tcp socket - passing through the ip and port
        tcp.ConnectToServer(port,_ip);
       // udp.setUpUDP(ip, port);
       
    }

}
