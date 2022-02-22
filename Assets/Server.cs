using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Crosstales;
using Crosstales.FB;

public class Server : MonoBehaviour
{
    List<ServerClient> clients;
    List<ServerClient> disconnectList;

    TcpListener server;
    bool serverStarted;

    public void ServerCreate()
    {
        clients = new List<ServerClient>();
        disconnectList = new List<ServerClient>();

        try
        {
            int port = 7777;
            server = new TcpListener(IPAddress.Any, port);
            server.Start();

            StartListening();
            serverStarted = true;
            print($"서버가 {port}에서 시작되었습니다");
        }
        catch(Exception e)
        {
            print($"에러 : {e}");
        }
    }

    void StartListening()
    {
        server.BeginAcceptTcpClient(AcceptTcpClient, server); //비동기로 바로바로 듣기 준비
    }

    void AcceptTcpClient(IAsyncResult ar)
    {
        TcpListener listener = (TcpListener)ar.AsyncState;
        clients.Add(new ServerClient(listener.EndAcceptTcpClient(ar)));
        StartListening();

        //메시지를 연결된 모두에게 보냄
        //Broadcast("")
    }

    public Image image;
    private void Update()
    {
        if (!serverStarted) return;

        foreach(ServerClient c in clients)
        {
            if (!IsConnected(c.tcp))
            {
                c.tcp.Close();
                disconnectList.Add(c);
                continue;
            }
            else
            {
                NetworkStream s = c.tcp.GetStream();
                if (s.DataAvailable)
                {
                    MemoryStream ms = new MemoryStream();
                    BinaryReader br = new BinaryReader(ms);

                    byte[] data = br.ReadBytes(0);
                    if (data != null)
                    {
                        //OnInComingData(c, data);
                        Sprite recvSprite = data.CTToSprite();
                        image.sprite = recvSprite;
                    }
                }
            }
        }
    }

    bool IsConnected(TcpClient c)
    {
        try
        {
            if (c != null && c.Client != null && c.Client.Connected)
            {
                if (c.Client.Poll(0, SelectMode.SelectRead)) 
                    return !(c.Client.Receive(new byte[1], SocketFlags.Peek) == 0);// 1바이트 먼저 보내보고 제대로 받으면 연결이 돼서 true가 됨

                return true;
            }
            else
                return false;
        }
        catch
        {
            return false;
        }
    }

    void OnInComingData(ServerClient c, byte data)
    {
        //if (data.Contains("&NAME"))
        //{
        //    c.clientName = data.Split('|')[1];
        //    Broadcast($"{c.clientName}이 연결되었습니다", clients);
        //}

        //Broadcast($"{c.clientName} : {data}", clients);
    }

    void Broadcast(byte[] data, List<ServerClient> cl)
    {
        foreach(var c in cl)
        {
            try
            {
                BinaryWriter writer = new BinaryWriter(c.tcp.GetStream());
                writer.Write(data);
                writer.Flush(); // 쓴 데이터 강제로 내보내기
            }
            catch(Exception e)
            {
                print($"쓰기에러 : {e.Message}를 클라이언트에게 {c.clientName}");
            }
        }
    }
}

public class ServerClient 
{
    public TcpClient tcp;
    public string clientName;

    public ServerClient(TcpClient clientSocket)
    {
        clientName = "Guest";
        tcp = clientSocket;
    }
}
