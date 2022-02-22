using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System;
using Crosstales;
using Crosstales.FB;

public class Client : MonoBehaviour
{
    string clientName;

    bool socketReady;
    Socket socket;
    NetworkStream stream;
    BinaryWriter writer;
    BinaryReader reader;

    public void OnConnectedToServer()
    {
        //이미 연결되었다면 함수 무시
        if (socketReady) return;

        string ip = "127.0.0.1";
        int port = 7777;

        // 소켓 생성
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint serverEP = new IPEndPoint(IPAddress.Loopback, port);

            socket.Connect(serverEP);
            socketReady = true;
        }
        catch (Exception e)
        {
            print($"소켓 에러 : {e.Message}");
        }
    }

    private void Update()
    {
        if (socketReady && stream.DataAvailable)
        {
            //string data = reader.ReadLine();
            byte[] data = reader.ReadBytes(0);
            if (data != null)
                OnInComingData(data);
        }
    }

    void OnInComingData(byte[] data)
    {
        //if (data == "%NAME")
        //{
        //    clientName = "Guest" + UnityEngine.Random.Range(1000, 10000);
        //    SendMessage($"&NAME|{clientName}");
        //    return;
        //}

        print(data);
    }

    void Send(byte[] data)
    {
        if (!socketReady) return;

        writer.Write(data);
        writer.Flush();
    }

    public void OnSendButton(InputField sendInput)
    {
        if (data == null) return;
        //if (!Input.GetButtonDown("Submit")) return; // 엔터로 끝나는게 아니면 리턴
        //sendInput.ActivateInputField();
        //if (sendInput.text.Trim() == "") return;

        //string message = sendInput.text;
        //sendInput.text = "";
        Send(data);
        //print(message);
    }

    private void OnApplicationQuit()
    {
        CloseSocket();
    }

    void CloseSocket()
    {
        if (!socketReady) return;

        writer.Close();
        reader.Close();
        socket.Close();
        socketReady = false;
    }

    public Image image;
    byte[] data;
    public void OpenSingleFile()
    {
        //string targetPath = @"C:\Users\PC\Desktop\TestImages";
        string[] extensions = { "jpg", "pdf" };
        string path = FileBrowser.Instance.OpenSingleFile("Open file", "", "", extensions);
        Debug.Log("Selected file: " + path);

        data = FileBrowser.Instance.CurrentOpenSingleFileData;
        //Typical use-cases
        Sprite selectedSprite = data.CTToSprite(); //returns a Texture of the data

        image.sprite = selectedSprite;
    }

}
