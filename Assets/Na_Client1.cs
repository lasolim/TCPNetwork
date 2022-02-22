using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System;
using System.IO;
using Crosstales;
using Crosstales.FB;

public class Na_Client1 : MonoBehaviour
{
    public string clientName;
    private bool socketReady;
    public static string response;
    private static byte[] clientBuffer = new byte[1024];

    //creating the socket TCP
    public Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    //declare end point
    public IPEndPoint conn;


    // Use this for initialization
    void Start()
    {

        DontDestroyOnLoad(gameObject); //don't destroy the client when moving on to the next scene
    }

    // Update is called once per frame
    //void Update()
    //{

    //    if (socketReady)
    //    {
    //        //check for new messages
    //        CheckForData(clientSocket);

    //        //Debug.Log("socket ready");

    //    }

    //}

    public void ConnectBtn()
    {
        
        ConnectToServer(Na_Server1.port);
    }

    public void SendBtn()
    {
        //SendData(clientSocket, "������ ������");
        byte[] buf = Encoding.UTF8.GetBytes("������");
        clientSocket.Send(buf);

        clientSocket.Close();
        print("������ ����");
    }

    public bool ConnectToServer(int port)
    {
        //if already connected ignore this fucntion
        if (socketReady)
        {
            return false;
        }

        //connect the socket to the server
        try
        {
            //create end point to connect
            conn = new IPEndPoint(IPAddress.Loopback, port);
            //connect to server
            clientSocket.Connect(conn);
            socketReady = true;
            Debug.Log("Client socket ready: " + socketReady);

            //// Send test data to the remote device.
            //Send(clientSocket, "This is a test");
            //Debug.Log("message sent to the server");


            // Receive the response from the remote device.
            //ReceiveData(clientSocket);

            //CheckForData(clientSocket);

            // Write the response to the console

        }
        catch (Exception ex)
        {
            Debug.Log("socket error: " + ex.Message);
        }

        return socketReady;
    }

    static void Send(Socket handler, String data)
    {
        // Convert the string data to byte data using ASCII encoding
        byte[] byteData = Encoding.ASCII.GetBytes(data);
            // Begin sending the data to the remote device
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
    }

    static void SendCallback(IAsyncResult ar)
    {
        try
        {
            //client socket
            Socket handler = (Socket)ar.AsyncState;

            // Complete sending the data to the client
            int bytesSent = handler.EndSend(ar);

            Debug.Log("bytes sent to the Server: " + bytesSent);

            //Debug.Log("clients connected: " + clients[clients.Count - 1].clientName);

        }
        catch (Exception e)
        {
            Debug.Log("error: " + e.Message);
        }
    }


    //async call to connect
    static void ConnectCallback(IAsyncResult ar)
    {
        try
        {

            // Retrieve the socket
            Socket client = (Socket)ar.AsyncState;

            // Complete the connection
            client.EndConnect(ar);

            //Debug.Log("Client successfully connected!!!!!");
            Debug.Log("Client Socket connected to: " + client.RemoteEndPoint);

        }
        catch (Exception e)
        {
            Debug.Log("Error connecting: " + e);
        }
    }


    /////////SEND DATA TO THE SERVER/////////
    //send data to server
    public static void SendData(Socket client, string data)
    {
        //convert the string data to bytes
        byte[] byteData = Encoding.ASCII.GetBytes(data);

        // Begin sending the data to the remote device.
        client.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallBack), client);
    }

    static void SendCallBack(IAsyncResult ar)
    {
        try
        {
            Socket client = (Socket)ar.AsyncState;

            //send date to the server
            int bytesSent = client.EndSend(ar);

            Debug.Log("client sent: " + bytesSent);
        }
        catch (Exception e)
        {
            Debug.Log("error sending message: " + e);
        }
    }
    //enclose this in one function


    /////////RECEIVE DATA FROM THE SERVER/////////

    //process the data received
    void OnIncomingData(string data)
    {
        Debug.Log("server answer: " + data);



    }

    public static void CheckForData(Socket client)
    {

        try
        {
            // Begin receiving the data from the remote device.
            client.BeginReceive(clientBuffer, 0, clientBuffer.Length, 0,
                                new AsyncCallback(ReceiveCallback), client);
        }
        catch (Exception e)
        {
            Debug.Log("error receiving the data: " + e.Message);
        }

    }
    public static Image image;
    static void ReceiveCallback(IAsyncResult ar)
    {
        try
        {
            // Read data from the remote device.
            Socket client = (Socket)ar.AsyncState;
            int bytesRead = client.EndReceive(ar);

            //don't know why after receiving my info this gets called.
            if (bytesRead == 0)
            {
                Debug.Log("no more data to receive");
                return;
            }

            var data = new byte[bytesRead];
            Array.Copy(clientBuffer, data, bytesRead);

            // Get the data
            client.BeginReceive(clientBuffer, 0, clientBuffer.Length, 0,
                                new AsyncCallback(ReceiveCallback), client);

            //response = Encoding.Default.GetString(clientBuffer);
            Sprite recvSprite = data.CTToSprite();

            Debug.Log("data from server received in the client: "/* + response*/);
            image.sprite = recvSprite;
        }
        catch (Exception ex)
        {
            Debug.Log("Error: " + ex.Message);
        }
    }



    /////////CLOSES THE SOCKET/////////
    void OnApplicationQuit()
    {
        CloseSocket();
    }

    void OnDisable()
    {
        CloseSocket();
    }


    void CloseSocket()
    {

        if (!socketReady)
        {
            return;
        }

        clientSocket.Close();
        socketReady = false;
    }



    public class GameClient
    {
        public string name;
        public bool isHost;
    }
}
