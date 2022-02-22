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

public class Na_Server : MonoBehaviour
{
    Socket servSocket, clntSocket;
    bool serverStarted;
    public Text text;
    public void ServerCreate()
    {
        try
        {
            int port = 7777;
            servSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Any, port);
            servSocket.Bind(endPoint);
            servSocket.Listen(10);

            clntSocket = servSocket.Accept();
            serverStarted = true;
            print($"������ {port}���� ���۵Ǿ����ϴ�");
        }
        catch (Exception e)
        {
            print($"���� : {e}");
        }
    }

    public Image image;
    private void Update()
    {
        if (!serverStarted) return;

        //servSocket.Close();
        if (!servSocket.Connected)
        {
            servSocket.Close();
            print("���� ����");
        }
        else
        {
            try
            {
                byte[] recvBytes = new byte[1024];

                //clntSocket.Receive(recvBytes);
                Sprite recvImage = recvBytes.CTToSprite();
                image.sprite = recvImage;
                clntSocket.Close();
                text.text = "������ ����";
            }
            catch (Exception e)
            {
                print($"���� : {e}");
            }
        }

    }


    void OnApplicationQuit()
    {
        CloseServer();
    }

    void CloseServer()
    {
        if (clntSocket != null)
        {
            clntSocket.Close();
            clntSocket = null;
        }

        if (servSocket != null)
        {
            servSocket.Close();
            servSocket = null;
        }
    }
}
