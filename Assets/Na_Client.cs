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
        //�̹� ����Ǿ��ٸ� �Լ� ����
        if (socketReady) return;

        string ip = "127.0.0.1";
        int port = 7777;

        // ���� ����
        try
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            EndPoint serverEP = new IPEndPoint(IPAddress.Loopback, port);

            socket.Connect(serverEP);
            socketReady = true;
        }
        catch (Exception e)
        {
            print($"���� ���� : {e.Message}");
        }
    }

    byte[] data;
    public void OnSendButton()
    {
        if (!socketReady) return;
        if (data == null) return;


        socket.Send(data);
        print("������ ����");

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
