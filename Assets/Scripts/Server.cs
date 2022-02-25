using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using System.Threading;

public class Server : MonoBehaviour
{
    public GameObject serverBtn;
    public GameObject clientBtn;
    public GameObject sendBtn;
    public GameObject openBtn;

    public int port = 50001;
    private TcpListener tcp_Listener;
    private List<TcpClient> clients = new List<TcpClient>(new TcpClient[0]);
    private Thread thrd_Listener;
    private TcpClient tcp_Client;

    private void Start()
    {
        print(GetCurrentIPAddress());
    }

    public void ServerBtn()
    {
        try
        {
            tcp_Listener = new TcpListener(GetCurrentIPAddress(), port);
            tcp_Listener.Start();

            thrd_Listener = new Thread(ListenForIncommingRequests);
            thrd_Listener.IsBackground = true;
            thrd_Listener.Start();

            Na_FileBrowser.instance.t.text = "서버";

            serverBtn.SetActive(false);
            clientBtn.SetActive(false);
            sendBtn.SetActive(true);
            openBtn.SetActive(true);
        }
        catch (SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());

            Na_FileBrowser.instance.t.text = "서버 생성 불가";
        }
    }

    public void SendBtn()
    {
        for (int i = 0; i < clients.Count; i++)
        {
            if (clients[i].Connected == false)
                clients.RemoveAt(i);

            else
            {
                if (Na_FileBrowser.instance.byteData != null)
                    SendImage(clients[i]);
                else
                    Na_FileBrowser.instance.t.text = "이미지를 열어주세요";
            }
        }
    }


    void OnApplicationQuit()
    {
        if (thrd_Listener != null)
            thrd_Listener.Abort();

        if (tcp_Listener != null)
        {
            tcp_Listener.Stop();
            tcp_Listener = null;
        }
    }

    void ListenForIncommingRequests()
    {
        try
        {
            while (tcp_Listener != null)
            {
                tcp_Client = tcp_Listener.AcceptTcpClient();
                clients.Add(tcp_Client);
            }
        }
        catch(SocketException socketException)
        {
            Debug.Log("SocketException " + socketException.ToString());
        }
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

    void SendImage(object token)
    {
        if (tcp_Client == null)
            return;

        else
            Debug.Log(clients.Count);

        var client = token as TcpClient;
        {
            try
            {
                NetworkStream stream = client.GetStream();
                if (stream.CanWrite)
                {
                    stream.Write(Na_FileBrowser.instance.byteData, 0, Na_FileBrowser.instance.byteData.Length);
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
