using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Text;

//Handles what is seen on screen UIWise
public class UIManager : MonoBehaviour
{
    //The one instance of this class
    public static UIManager instance;
    //Canvas with connection screen on it
    public GameObject startMenu;
    //Field the player enters
    public InputField usernameField;
    //The chat box
    public InputField chatField;
    //Has the username be sent
    bool sentUsername = false;
    public Text posText;

    public Text pingText;
   

    public Text textUI;
    //Chat log
    [HideInInspector]
    public string[] chatMessages = new string[5];
    //Text components to display chat
    public Text[] chatText = new Text[5];

    //The send buffer that send writes to
    private byte[] sendBuffer;

    [HideInInspector]
    public float pingTimer = 0;
    [HideInInspector]
    public int ping = 0;
    [HideInInspector]
    public bool pinging = false;

    struct chatMessage
    {
        //u = username, c = chat, p = player data, t = ping test
        public char typeOfMessage;
        public string message;
    }
    private void Awake()
    {
        //If there's no instance
        if (instance == null)
        {
            //It is this
            instance = this;
            //Otherwise
        } else if (instance != this)
        {
            //destroy this as we already have an instance
            Debug.Log("Destroying A Previous Exisisting Instance");
            Destroy(this);
        }
        //Update the UI to get rid of editor placeholder text
        DisplayChat();
        sendBuffer = new byte[Client.dataBufferSize];
    }

    private void Update()
    {
        DisplayChat();
        displayUI();
        if(pinging)
        {
            pingTimer += Time.deltaTime;
        }
    }

    public void ConnectToServer()
    {
        //startMenu.SetActive(false);

        if (usernameField.text.Length > 0)
        {
            Client.instance.ConnectToServer();
        }
        else
        {

            textUI.text = "Enter Username";
            textUI.color = Color.red;
        }
        
    }

    public void SendUsername()
    {
        if(!sentUsername)
        {
            //if a username has been entered, send it
            if (usernameField.text.Length > 0)
            {
                sentUsername = true;

                chatMessage messageToSend;
                messageToSend.typeOfMessage = 'u';
                messageToSend.message = usernameField.text;


                byte[] data = new byte[messageToSend.message.Length + 2];
                Array.Copy(BitConverter.GetBytes(messageToSend.typeOfMessage), 0, data, 0, 2);
                Array.Copy(Encoding.ASCII.GetBytes(messageToSend.message), 0, data, 2, messageToSend.message.Length);


                //Debug.Log(String.Format("String From Array Is {0}", Encoding.ASCII.GetString(data, 2, data.Length - 2)));

                Client.instance.SendTCP(data);
            }
        }
    }

    public void SendMessage()
    {
        if (chatField.text.Length > 0)
        {
            chatMessage messageToSend;
            messageToSend.typeOfMessage = 'c';
            messageToSend.message = chatField.text;

            byte[] data = new byte[messageToSend.message.Length + 2];
            Array.Copy(BitConverter.GetBytes(messageToSend.typeOfMessage), 0, data, 0, 2);
            Array.Copy(Encoding.ASCII.GetBytes(messageToSend.message), 0, data, 2, messageToSend.message.Length);

            Client.instance.SendTCP(data);
        }
    }

    public void DisplayChat()
    {
        for(int i = 0; i < 5; i++)
        {
            chatText[i].text = chatMessages[i];
        }
    }

    public void CalculatePing()
    {
        char typeOfMessage = 't';
        byte[] data = new byte[2];


        Array.Copy(BitConverter.GetBytes(typeOfMessage), 0, data, 0, 2);
        ping = 0;
        pingTimer = 0;
        Client.instance.SendTCP(data);
        pinging = true;
    }

    public void PingResult()
    {
        pinging = false;
        ping = (int)(pingTimer * 1000f) % 1000;
    }

    public void AddToChat(string messageToAdd)
    {
        for(int i = 0; i < chatMessages.Length - 1; i++)
        {
            chatMessages[i] = chatMessages[i + 1];
        }
        chatMessages[chatMessages.Length - 1] = messageToAdd;
        DisplayChat();
    }

    public void displayUI()
    {
        if (GameManager.gameStarted)
        {
            //posText.text = string.Format("P1: {0} , {1}", Player.instance.transform.position.x, Player.instance.transform.position.y);
            pingText.text = string.Format("{0} Seconds", GameManager.gameTime);
        }
        else
        {
            int milliseconds = (int)(pingTimer * 1000f) % 1000;
            pingText.text = string.Format("{0}ms", milliseconds);
        }
    }
    
    
    public void setText(string text)
    {
        textUI.text = text;
    }
}
