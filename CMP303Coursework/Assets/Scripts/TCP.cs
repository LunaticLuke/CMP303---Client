//With Help From Weiland, 2019, Connecting Unity Clients to a Dedicated Server | C# Networking Tutorial - Part 1

using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Net.NetworkInformation;

public class TCP
{
    //The TCP socket that I connect,send and receive on.
    public Socket socket;
    //The receive buffer that receive writes to
    private byte[] receiveBuffer;
    //The send buffer that send writes to
    private byte[] sendBuffer;




    


    
    public void ConnectToServer(int port,string ip)
    {
        //Parse the ip to convert it from string to IPAddress
        IPAddress ipAddress = IPAddress.Parse(ip);

        //Set the IP endpoint to the public Ip address and port given.
        IPEndPoint remoteEndPoint = new IPEndPoint(ipAddress, port);

        // Create a new TCP socket
        socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        //Set the sizes of both buffers
        socket.ReceiveBufferSize = Client.dataBufferSize;
        socket.SendBufferSize = Client.dataBufferSize;
        receiveBuffer = new byte[Client.dataBufferSize];
        sendBuffer = new byte[Client.dataBufferSize];
        socket.NoDelay = true;

        //Asynchronously begin the connect
       Debug.Log(string.Format("Connecting To Server On {0}  {1}", port,ipAddress));
        socket.BeginConnect(remoteEndPoint, ConnectCallback, socket);
    }

    //This will call when a connection is made and requires an ASync result
    private void ConnectCallback(IAsyncResult _result)
    {
        //End the async connect and pass through the result
        socket.EndConnect(_result);

        UIManager.instance.SendUsername();
        //If it isnt connected, something has went wrong.
        if(!socket.Connected)
        {
            return;
        }

        //Ready to start receiving now.
        Receive();
    }







    //Triggered if something can be received on the TCP socket.
    //Switch statement features various types of message that can come through.
    //I made use of a char to distinguish between the types of messages.
    private void Receive()
    {
        //A try catch block will attempt to catch any exceptions
        try
        {
            //Start an asynchornous receive method
            socket.BeginReceive(receiveBuffer, 0, Client.dataBufferSize, 0, ReceiveCallback, socket);
        }catch(Exception _e)
        {
            //Write the exception caught to the console
            Debug.Log(_e.ToString());
        }
    }

    //Triggered if something can be received on the TCP socket.
    private void ReceiveCallback(IAsyncResult _result)
    {
        try

        {
            //Get the byte length 
            int byteLength = socket.EndReceive(_result);
            //If its less than or equal to zero, client has disconnected, handle this
            if (byteLength <= 0)
            {
                //Disconnect Function
                return;
            }
            //Create a new byte array to store received message
            byte[] _data = new byte[byteLength];
            //Copy it across
            Array.Copy(receiveBuffer, _data, byteLength);

            char typeOfMessage = BitConverter.ToChar(_data, 0);


            switch (typeOfMessage)
            {
                //Ping Data
                case 't':
                    UIManager.instance.PingResult();
                    break;
                //Username Data
                case 'u':
                    int id = BitConverter.ToInt32(_data, 2);
                   // Debug.Log(string.Format("Assigned ID Is Now {0}", id));
                    Client.instance.id = id;
                    break;

                //Chat Data
                case 'c':
                 
                string message = Encoding.ASCII.GetString(_data, 2, byteLength - 2);
                    //Update the chat list in the UIManager class
                        for (int i = 0; i < UIManager.instance.chatMessages.Length - 1; i++)
                        {
                            //Move all the chat messages up 1.
                            UIManager.instance.chatMessages[i] = UIManager.instance.chatMessages[i + 1];
                        }
                        //Add the most recent message
                        UIManager.instance.chatMessages[UIManager.instance.chatMessages.Length - 1] = message;
                    
                    break;
                //Zombie Data
                case 'z':
                    //Allow the packet class to convert bytes to struct
                        Packet.ZombieStruct zombieMessage = Packet.ConvertByteArray(_data);
                        //Game manager handles it
                        GameManager.instance.HandleZombieData(zombieMessage);
                    break;
                 //Player Data
                case 'p':
                    //Build data into struct again
                    Packet.PlayerStruct playerData = Packet.ConvertPlayerArray(_data);

                    //Pass the struct through to the game manager that will send the data for prediction
                    GameManager.instance.HandlePositionData(playerData);
                    break;
                //Bullet Data
                case 'b':
                    //Convert the bullet data to floats and ints
                    float xOrigin = BitConverter.ToSingle(_data, 2);
                    float yOrigin = BitConverter.ToSingle(_data, 6);
                    float dirX = BitConverter.ToSingle(_data, 10);
                    float dirY = BitConverter.ToSingle(_data, 14);
                    int _id = BitConverter.ToInt32(_data, 18);

                    GameManager.instance.HandleBulletData(xOrigin, yOrigin, dirX, dirY, _id);
                    break;
                //Spawn Player Event
                case 's':
                    int numOfPlayers = BitConverter.ToInt32(_data, 2);

                    GameManager.numOfPlayers = numOfPlayers;
                    GameManager.instance.spawnPlayer = true;
                    break;
                //Zombie Kill
                case 'k':
                    int idOfPlayer = BitConverter.ToInt32(_data, 2);
                    int idOfZombie = BitConverter.ToInt32(_data, 6);

                    Debug.Log(string.Format("Id: {0}", idOfPlayer));
                    Debug.Log(string.Format("IdZombie: {0}", idOfZombie));

                    if(idOfPlayer == Client.instance.id)
                    {
                        GameManager.kills++;
                    }
                    GameManager.instance.zombies[idOfZombie].alive = false;
                    break;
                //Health Update
                case 'h':
                    int health = BitConverter.ToInt32(_data, 2);
                    GameManager.health -= health;

                    break;
            
            }



            
           socket.BeginReceive(receiveBuffer, 0, Client.dataBufferSize, 0, ReceiveCallback, socket);
        }
        catch (Exception _e)
        {
            //Debug.Log(string.Format("Error Receiving TCP Data: {0}", _e));
            //Disconnect
        }
    }


    public void Send(byte[] data)
    {
        //Debug.Log(string.Format("TCP Length of the message Struct is {0}", data.Length));
        //Begin Sending chat message to the server.
        socket.BeginSend(data, 0, data.Length,0, SendCallback, socket);
    }

    //Triggers when ready to send
    private void SendCallback(IAsyncResult _result)
    {
        try
        {
        int bytesSent = socket.EndSend(_result);
        }catch(Exception _e)
        {
           Debug.Log(_e.ToString());
        }

    }
}
