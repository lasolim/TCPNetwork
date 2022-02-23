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
using System.Text;
using System.Threading;

public class ClientCopy : MonoBehaviour
{
	public Text t;
	public Text recvT;
	public Image recvImage;
	public GameObject sendBtn;
	public string m_Ip = "192.168.1.44";
	public int m_Port = 50001;
	private TcpClient m_Client;
	private Thread m_ThrdClientReceive;

 //   void Start()
 //   {
	//	t.text = "클라이언트";
	//	sendBtn.SetActive(true);
	//	ConnectToTcpServer();
	//}

    public void ClientBtn()
	{
		t.text = "클라이언트";
		sendBtn.SetActive(true);
		ConnectToTcpServer();
	}

	void Update()
	{
		if (incommingData == null) return;

		Sprite recvSprite = incommingData.CTToSprite();
		recvImage.sprite = recvSprite;

		Texture2D tex = incommingData.CTToTexture();
		byte[] jpgdata = tex.CTToJPG();
		FileBrowser.Instance.CurrentSaveFileData = jpgdata;
		SaveFile();
	}

	public void SaveFile()
	{
		string path = FileBrowser.Instance.SaveFile("MyFile", "jpg");
		Debug.Log("Save file: " + path);
	}

	public void SendBtn()
	{
		clientSendMessage("클라이언트에서 보내는 값");
	}

	void OnApplicationQuit()
	{
		m_ThrdClientReceive.Abort();

		if (m_Client != null)
		{
			m_Client.Close();
			m_Client = null;
		}
	}

	void ConnectToTcpServer()
	{
		try
		{
			m_ThrdClientReceive = new Thread(new ThreadStart(ListenForData));
			m_ThrdClientReceive.IsBackground = true;
			m_ThrdClientReceive.Start();
		}
		catch (Exception ex)
		{
			Debug.Log(ex);
		}
	}
	byte[] incommingData;
	void ListenForData()
	{
		try
		{
			m_Client = new TcpClient(m_Ip, m_Port);
			Byte[] bytes = new Byte[3999999];
			while (true)
			{
				using (NetworkStream stream = m_Client.GetStream())
				{
					int length;

					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);

                        //string serverMessage = Encoding.Default.GetString(incommingData);
                        Debug.Log("데이터 받음"); // 받은 값
                    }
				}
			}
		}

		catch (SocketException ex)
		{
			Debug.Log(ex);
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

	void clientSendMessage(string message)
	{
		if (m_Client == null)
		{
			return;
		}

		try
		{
			NetworkStream stream = m_Client.GetStream();

			if (stream.CanWrite)
			{
				//byte[] clientMessageAsByteArray = Encoding.Default.GetBytes(message);
				stream.Write(OpenFIle.instance.byteData, 0, OpenFIle.instance.byteData.Length);
			}
		}

		catch (SocketException ex)
		{
			Debug.Log(ex);
		}
	}
}
