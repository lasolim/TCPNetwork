using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using Crosstales;
using Crosstales.FB;

//https://forum.unity.com/threads/tcp-sockets-how-to-receive-from-client.541502/
public class AsyncStateData 
{
    public byte[] Buffer;
    public Socket Socket;

}

public class Na_Server1 : MonoBehaviour
{
    public static int port = 1058;
    string host = "";

    static List<AsyncStateData> clients;
    static List<AsyncStateData> disconnectList;

    //creating the socket TCP
    public Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    //create the buffer size, how much info we can send and receive
    byte[] serverBuffer = new byte[39999999];

    bool serverStarted;

    public string content;

    /*instead of creating the server in Start I need to created in aother function,
    because the server gets called when the player cliks the button "host game"
    and then wait for more players to join, and we need to change scenes so
    we don't want to destroy the server after loading the new scene
    */

    public void Init()
    {
        DontDestroyOnLoad(gameObject); //don't destroy the server once the new scene is loaded

        //instaciating the lists
        clients = new List<AsyncStateData>();
        disconnectList = new List<AsyncStateData>();

        try
        {
            //call the function create server here
            CreateServer();
        }
        catch (Exception ex)
        {
            Debug.Log("Error when creating the server: " + ex.Message);
            //Show dialog error to the player
        }
    }


    // Use this for initialization
    void Start()
    {
        byte[] buf = Encoding.UTF8.GetBytes("데이터");
            int nRecv = 9;
        string txt = Encoding.UTF8.GetString(buf, 0, nRecv);

        print(txt);
        //try
        //{
        //    //start server
        //    CreateServer();
        //}
        //catch (Exception ex)
        //{
        //    Debug.Log("Error when creating the server " + ex.Message);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (!serverStarted)
        {
            return;
        }

        AcceptConnections();
        //serverSocket.BeginAccept(Na_AcceptCallback, serverSocket);

        //foreach (AsyncStateData sc in clients)
        //{
        //    // is the client still connected?
        //    if (!isConnected(sc.Socket))
        //    {
        //        sc.Socket.Close(); //close the socket
        //        disconnectList.Add(sc);
        //        continue;
        //    }
        //    //check for messages from the client, check the stream of every client
        //    else // client is connected to the server
        //    {
        //        NetworkStream stream = new NetworkStream(sc.Socket);

        //        if (stream.DataAvailable)
        //        {

        //            StreamReader reader = new StreamReader(stream, true); //reading the data
        //            string data = reader.ReadLine(); //store data

        //            CheckForData(sc);

        //            if there is data


        //        }


        //        AcceptConnections();


        //    }
        //}

        ////disconnection loop
        //for (int i = 0; i < disconnectList.Count - 1; i++)
        //{
        //    //tell our player somebody has disconnected

        //    clients.Remove(disconnectList[i]);
        //    disconnectList.RemoveAt(i);
        //}
    }

    void Na_AcceptCallback(IAsyncResult ar)
    {
        try
        {
            // Get the socket that handles the client request
            Socket server = (Socket)ar.AsyncState;
            //Socket handler = server.EndAccept(ar);

            AsyncStateData data = new AsyncStateData();
            data.Buffer = new byte[1024];
            // 캐스트 불가
            data.Socket = server.EndAccept(ar);

            byte[] revBytes = new byte[1024];

            int nRecv = data.Socket.Receive(revBytes);
            string txt = Encoding.UTF8.GetString(revBytes, 0, nRecv);

            data.Socket.Close();
        }
        catch (Exception e)
        {
            print("받기 오류 : " + e);
        }

    }


    // Bind the socket to the local endpoint and listen for incoming connections.
    public void CreateServer()
    {
        try
        {
            Debug.Log("Setting up the server...");

            //bind socket
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, port));
            Debug.Log("socket bound");

            //start listening
            serverSocket.Listen(10); //only 3 connections at a time
            Debug.Log("socket listening on port: " + port);

            //accept connections
            AcceptConnections();
            //serverSocket.BeginAccept(Na_AcceptCallback, serverSocket);

            serverStarted = true;
        }
        catch (Exception e)
        {

            Debug.Log("Error when binding to port and listening: " + e.Message);
        }

    }

    //start async socket to listen for connections
    public void AcceptConnections()
    {
       
        //Socket clntSocket = serverSocket.Accept();
        serverSocket.BeginAccept(AcceptCallback, serverSocket);

        //AsyncStateData data = new AsyncStateData();
        //data.Buffer = new byte[1024];
        //data.Socket = clntSocket;

        //clntSocket.BeginReceive(data.Buffer, 0, data.Buffer.Length, SocketFlags.None, asyncReceiveCallback, data);
    }

    private static void asyncReceiveCallback(IAsyncResult ar)
    {
        try
        {
            AsyncStateData rcvData = ar.AsyncState as AsyncStateData;
            //AsyncStateData rcvData = clients[0];

            //AsyncStateData rcvData = ar.AsyncState as AsyncStateData;

            int nRecv = rcvData.Socket.EndReceive(ar);
            string txt = Encoding.UTF8.GetString(rcvData.Buffer, 0, nRecv);

            print(txt);
            rcvData.Socket.Close();
            //byte[] sendBytes = Encoding.UTF8.GetBytes("Hello: " + txt);
            //rcvData.Socket.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, asyncSendCallback, rcvData.Socket);
        }
        catch(Exception e)
        {
            print("받기 오류 : " + e);
        }
    }

    private static void asyncSendCallback(IAsyncResult ar)
    {
        Socket socket = ar.AsyncState as Socket;
        socket.EndSend(ar);

        socket.Close();
    }
    //async socket
    void AcceptCallback(IAsyncResult ar)
    {
        try
        {
            // Get the socket that handles the client request
            Socket server = (Socket)ar.AsyncState;
            //Socket handler = server.EndAccept(ar);

            AsyncStateData data = new AsyncStateData();
            data.Buffer = new byte[1024];
            // 캐스트 불가
            data.Socket = server.EndAccept(ar);

            //begin receiving data from the client//////////////////////////////////////
            data.Socket.BeginReceive(serverBuffer, 0, serverBuffer.Length, 0,
            asyncReceiveCallback, data);

            //CheckForData(handler);


            //add client to dictionary key: client value: stake
            clients.Add(data);

            //accept incoming connections again
            serverSocket.BeginAccept(AcceptCallback, serverSocket);

            Debug.Log("Someone has connected!!!!");

            if (clients.Count > 0)
            {
                Debug.Log("client successfully added to the list of clients");
            }

            //send a message to everyone say someone has connected
            BroadCastData("some client has connected", clients);
        }
        catch (Exception e)
        {
            print("받기 오류 : " + e);
        }
       
    }


    /////////CHECK IF THERE IS DATA TO BE RECEIVED/////////

    public void CheckForData(AsyncStateData socket)
    {

        //begin receiving data from the client
        socket.Socket.BeginReceive(serverBuffer, 0, serverBuffer.Length, 0,
                            ReadCallback, socket);

    }

    void ReadCallback(IAsyncResult ar)
    {

        //client socket
        Socket handler = (Socket)ar.AsyncState;
        //ServerClient client = new ServerClient(handler);
        AsyncStateData client = new AsyncStateData();
        client.Buffer = new byte[1024];
        client.Socket = handler;

        Debug.Log("function to read data from client");


        // Read data from the client socket  
        int bytesRead = client.Socket.EndReceive(ar);

        Debug.Log("receiving from client....");


        if (bytesRead == 0)
        {
            //no data to read
            Debug.Log("no data to receive");
            return;
        }


        //////////////not sure here
        var data = new byte[bytesRead];
        Array.Copy(serverBuffer, data, bytesRead);

        // Get the data
        client.Socket.BeginReceive(serverBuffer, 0, serverBuffer.Length, 0,
                                      new AsyncCallback(ReadCallback), client.Socket);
        /////////////////

        //store the data received
        content = Encoding.ASCII.GetString(serverBuffer);


        //send data to teh client
        //Send(client.tcpSocket, "hello from the server");

        OnIncommingData(client, content);

        Debug.Log("client sent: ");

    }

    /////////PROCESS DATA RECEIVED/////////

    public void OnIncommingData(AsyncStateData client, string data)
    {

        Debug.Log("client has send: " + data);

    }

    /////////SEND DATA PROCESSED BACK TO THE CLIENT/////////

    public void BroadCastData(string data, List<AsyncStateData> clients)
    {

        foreach (var cl in clients)
        {
            try
            {
                //send data back to client
                Send(cl.Socket, "hello from server");
                Debug.Log("sent a message to the client");
            }
            catch (Exception ex)
            {
                Debug.Log("error writing data: " + ex.Message);
            }
        }

    }

    static void Send(Socket handler, String data)
    {

        // Convert the string data to byte data using ASCII encoding
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device
        handler.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), handler);
    }
    //public Image image;
    //static byte[] byteData;
    //static void Send(Socket handler, String data)
    //{
    //    // Convert the string data to byte data using ASCII encoding
    //    //byte[] byteData = Encoding.ASCII.GetBytes(data);
    //    if (byteData != null)
    //    {
    //        // Begin sending the data to the remote device
    //        handler.BeginSend(byteData, 0, byteData.Length, 0,
    //            new AsyncCallback(SendCallback), handler);

    //    }
    //}

    public void OpenSingleFile()
    {
        //string targetPath = @"C:\Users\PC\Desktop\TestImages";
        string[] extensions = { "jpg", "pdf" };
        string path = FileBrowser.Instance.OpenSingleFile("Open file", "", "", extensions);
        Debug.Log("Selected file: " + path);

        Byte[] byteData = FileBrowser.Instance.CurrentOpenSingleFileData;
        //Typical use-cases
        //Sprite selectedSprite = byteData.CTToSprite(); //returns a Texture of the data

        //image.sprite = selectedSprite;
    }

    static void SendCallback(IAsyncResult ar)
    {

        try
        {
            //client socket
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the client
            int bytesSent = handler.EndSend(ar);

            Debug.Log("bytes sent to the client: " + bytesSent);

            //Debug.Log("clients connected: " + clients[clients.Count - 1].clientName);

        }
        catch (Exception e)
        {
            Debug.Log("error: " + e.Message);
        }
    }



    /////////check if te client is connected to the server/////////

    bool isConnected(Socket c)
    {
        try
        {
            if (c != null && c != null && c.Connected)
            {
                if (c.Poll(0, SelectMode.SelectRead))
                {
                    return !(c.Receive(new byte[1], SocketFlags.Peek) == 0);
                }
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }


    /////////definition of the client/////////

    public class ServerClient
    {

        public Socket tcpSocket;

        public string clientName;


        public ServerClient(Socket clientSocket)
        {
            tcpSocket = clientSocket;
        }
    }

    public void CloseServer()
    {
        serverSocket.Close();
    }
}
