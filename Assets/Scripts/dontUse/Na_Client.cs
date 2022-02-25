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

public class Na_Client : MonoBehaviour
{
    Socket socket;
    bool socketReady;

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

    byte[] data;
    public void OnSendButton()
    {
        if (!socketReady) return;
        if (data == null) return;


        socket.Send(data);
        print("데이터 보냄");

        socket.Close();
        print("TCP Client socket : Closed");
    }


    public Image image;
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
