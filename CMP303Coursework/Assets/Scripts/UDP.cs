using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class UDP
{
   /* struct InputMessage
    {
        public float xAxis;
        public float yAxis;
    }
    public Socket socket;

    private byte[] receiveBuffer;

    private byte[] sendBuffer;

    IPAddress ip;

    IPEndPoint endPoint;

    EndPoint ReceiveEndPoint;
   public void setUpUDP(string ipToSet,  int _port)
    {
        ip = IPAddress.Parse(ipToSet);
        endPoint = new IPEndPoint(ip, _port);

        ReceiveEndPoint = (EndPoint)endPoint;
        socket = new Socket(ip.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

        socket.ReceiveBufferSize = Client.dataBufferSize;
        socket.SendBufferSize = Client.dataBufferSize;
        receiveBuffer = new byte[Client.dataBufferSize];
        sendBuffer = new byte[Client.dataBufferSize];

        //Attach the clients udp socket to the same endpoint as it wont need to change. UDP is connectionless so all this does it make it so send can be called without stating an endpoint.
        socket.Connect(endPoint);
        socket.BeginReceive(receiveBuffer, 0, Client.dataBufferSize, 0, ReceiveCallback, socket);
    }

    private void ReceivePacket()
    {
        //A try catch block will attempt to catch any exceptions
        try
        {
            //Start an asynchornous receive method
            socket.BeginReceive(receiveBuffer, 0, Client.dataBufferSize, 0, ReceiveCallback, socket);
        }
        catch (Exception _e)
        {
            //Write the exception caught to the console
            Debug.Log(_e.ToString());
        }
    }

    private void ReceiveCallback(IAsyncResult _result)
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
        string message = Encoding.ASCII.GetString(receiveBuffer, 0, byteLength);
        //Copy it across
        Array.Copy(receiveBuffer, _data, byteLength);
        Debug.Log(string.Format("Received message Reading: {0}",message);
    }

    public void Send(string data)
    {
        //Get the bytes of the string
        sendBuffer = Encoding.ASCII.GetBytes(data);
        //Begin Sending chat message to the server.
        socket.BeginSend(sendBuffer, 0, sendBuffer.Length, 0, SendCallback, socket);
    }

    //Triggers when ready to send
    private void SendCallback(IAsyncResult _result)
    {
        try
        {
            int bytesSent = socket.EndSend(_result);
            Debug.Log(string.Format("Sent {0} bytes to the server.", bytesSent));
        }
        catch (Exception _e)
        {
            Debug.Log(_e.ToString());
        }

    }*/
}
