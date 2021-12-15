using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Packet
{
    //18 Bytes
    public struct ZombieStruct
    {
        public ZombieStruct(int numOfZombies)
        {
            typeOfMessage = 'z';
            timestamp = 0;
            XPos = new float[numOfZombies];
            YPos = new float[numOfZombies];
        }
        public char typeOfMessage; // 2 Bytes
        public float timestamp; //4 bytes
        public float[] XPos;
        public float[] YPos;
    }

    public struct PlayerStruct
    {
        public char typeOfMessage; // 2 bytes
        public float timestamp; // +4 bytes = 6 bytes
        public float XPosP1; // +4 bytes = 10 bytes
        public float YPosP1; // +4 bytes = 14 bytes
        public float ZRotP1; // +4 bytes = 18 Bytes
        public float XPosP2; // +4 bytes = 22 bytes
        public float YPosP2; // +4 bytes = 26 bytes
        public float ZRotP2; // +4 bytes = 30 Bytes
        public float XPosP3; // +4 bytes = 34 bytes
        public float YPosP3; // +4 bytes = 38 bytes
        public float ZRotP3; // +4 bytes = 42 Bytes
        public float XPosP4; // +4 bytes = 46 bytes
        public float YPosP4; // +4 bytes = 50 bytes
        public float ZRotP4; // +4 bytes = 54 Bytes
    }


    public static ZombieStruct ConvertByteArray(byte[] data)
    {
        ZombieStruct zombieMessage = new ZombieStruct(10);
        zombieMessage.timestamp = BitConverter.ToSingle(data,2);
        int index = 6;
        for(int i = 0; i < 10; i++)
        {
            zombieMessage.XPos[i] = BitConverter.ToSingle(data, index);
            zombieMessage.YPos[i] = BitConverter.ToSingle(data, index + 4);
            index += 8;
        }

        return zombieMessage;
    }



    public static PlayerStruct ConvertPlayerArray(byte[] data)
    {
        PlayerStruct playerMessage = new PlayerStruct();
        playerMessage.timestamp = BitConverter.ToSingle(data, 2);
        playerMessage.XPosP1 = BitConverter.ToSingle(data, 6);
        playerMessage.YPosP1 = BitConverter.ToSingle(data, 10);
        playerMessage.ZRotP1 = BitConverter.ToSingle(data, 14);

        if (GameManager.numOfPlayers >= 2)
        {
            Debug.Log("Reached The ConvertPlayerArray");
            playerMessage.XPosP2 = BitConverter.ToSingle(data, 18);
            Debug.Log(playerMessage.XPosP2);
            playerMessage.YPosP2 = BitConverter.ToSingle(data, 22);
            Debug.Log(playerMessage.YPosP2);
            playerMessage.ZRotP2 = BitConverter.ToSingle(data, 26);
        }

        if (GameManager.numOfPlayers >= 3)
        {
            playerMessage.XPosP3 = BitConverter.ToSingle(data, 30);
            playerMessage.YPosP3 = BitConverter.ToSingle(data, 34);
            playerMessage.ZRotP3 = BitConverter.ToSingle(data, 38);
        }

        if (GameManager.numOfPlayers >= 4)
        {
            playerMessage.XPosP4 = BitConverter.ToSingle(data, 42);
            playerMessage.YPosP4 = BitConverter.ToSingle(data, 46);
            playerMessage.ZRotP4 = BitConverter.ToSingle(data, 50);
        }
        return playerMessage;
    }

}