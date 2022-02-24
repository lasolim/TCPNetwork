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
using System.Text;
using System.Threading;

public class Server : MonoBehaviour
{
    public Text t;
    public Text recvT;
    public Image recvImage;
    public GameObject sendBtn;
    public int m_Port = 50001;
    private TcpListener m_TcpListener;
    private List<TcpClient> m_Clients = new List<TcpClient>(new TcpClient[0]);
    private Thread m_ThrdtcpListener;
    private TcpClient m_Client;

    //void Start()
    //{
    //    m_ThrdtcpListener = new Thread(new ThreadStart(ListenForIncommingRequests));
    //    m_ThrdtcpListener.IsBackground = true;
    //    m_ThrdtcpListener.Start();
    //}

    public void ServerBtn()
    {
        t.text = "서버";
        sendBtn.SetActive(true);
        m_ThrdtcpListener = new Thread(ListenForIncommingRequests);
        m_ThrdtcpListener.IsBackground = true;
        m_ThrdtcpListener.Start();
    }

    void Update()
    {
        //for (int i = 0; i < m_Clients.Count; i++)
        //{
        //    if (!m_Clients[i].Connected)
        //        m_Clients.RemoveAt(i);

        //    else
        //        SendMessage(m_Clients[i], "서버에서 보내는 값"); // 보내는 값
        //}

        if (incommingData == null) return;

        Sprite recvSprite = incommingData.CTToSprite();
        recvImage.sprite = recvSprite;
    }

    public void SendBtn()
    {
        for (int i = 0; i < m_Clients.Count; i++)
        {
            if (!m_Clients[i].Connected)
                m_Clients.RemoveAt(i);

            else
                SendMessage(m_Clients[i], "서버에서 보내는 값"); // 보내는 값
        }
    }

    void OnApplicationQuit()
    {
        if (m_TcpListener == null) return;

        m_ThrdtcpListener.Abort();
        m_TcpListener.Stop();
        m_TcpListener = null;
    }

    void ListenForIncommingRequests()
    {
        m_TcpListener = new TcpListener(GetCurrentIPAddress(), m_Port);
        m_TcpListener.Start();
        ThreadPool.QueueUserWorkItem(ListenerWorker, null);
    }

    public static IPAddress GetCurrentIPAddress()
    {
        IPAddress[] addrs = Dns.GetHostEntry(Dns.GetHostName()).AddressList;

        foreach (IPAddress ipAddr in addrs)
        {
            if (ipAddr.AddressFamily == AddressFamily.InterNetwork)
            {
                return ipAddr;
            }
        }

        return null;
    }

    void ListenerWorker(object token)
    {
        while (m_TcpListener != null)
        {
            m_Client = m_TcpListener.AcceptTcpClient();
            m_Clients.Add(m_Client);
            ThreadPool.QueueUserWorkItem(HandleClientWorker, m_Client);
        }
    }

    byte[] incommingData;
    void HandleClientWorker(object token)
    {
        Byte[] bytes = new Byte[3999999];
        using (var client = token as TcpClient)
        using (var stream = client.GetStream())
        {
            int length;

            while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                incommingData = new byte[length];
                Array.Copy(bytes, 0, incommingData, 0, length);
                string clientMessage = Encoding.Default.GetString(incommingData);
                Debug.Log(clientMessage); // 받은 자료
            }

            if (m_Client == null)
            {
                return;
            }
        }
    }

    void SendMessage(object token, string message)
    {
        if (m_Client == null)
            return;

        else
            Debug.Log(m_Clients.Count);

        var client = token as TcpClient;
        {
            try
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    //byte[] serverMessageAsByteArray = Encoding.Default.GetBytes(message);
                    stream.Write(OpenFIle.instance.byteData, 0, OpenFIle.instance.byteData.Length);
                }
            }

            catch (SocketException ex)
            {
                Debug.Log(ex);
                return;
            }
        }
    }
}
