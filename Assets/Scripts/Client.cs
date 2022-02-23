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

public class Client : MonoBehaviour
{
	public string m_Ip = "127.0.0.1";
	public int m_Port = 50001;
	private TcpClient m_Client;
	private Thread m_ThrdClientReceive;

	//void Start()
	//{
	//	ConnectToTcpServer();
	//}

	public void ClientBtn()
    {
		ConnectToTcpServer();
	}

	//void Update()
	//{
	//	SendMessage("클라이언트에서 보내는 값");
	//}

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

	void ListenForData()
	{
		try
		{
			m_Client = new TcpClient(m_Ip, m_Port);
			Byte[] bytes = new Byte[1024];
			while (true)
			{
				using (NetworkStream stream = m_Client.GetStream())
				{
					int length;

					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
					{
						var incommingData = new byte[length];
						Array.Copy(bytes, 0, incommingData, 0, length);

						string serverMessage = Encoding.Default.GetString(incommingData);
						Debug.Log(serverMessage); // 받은 값

					}
				}
			}
		}

		catch (SocketException ex)
		{
			Debug.Log(ex);
		}
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
				byte[] clientMessageAsByteArray = Encoding.Default.GetBytes(message);
				stream.Write(clientMessageAsByteArray, 0, clientMessageAsByteArray.Length);
			}
		}

		catch (SocketException ex)
		{
			Debug.Log(ex);
		}
	}

}
