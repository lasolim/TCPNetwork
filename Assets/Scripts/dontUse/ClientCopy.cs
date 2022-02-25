using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net.Sockets;
using System.Net;
using System;
using Crosstales;
using Crosstales.FB;
using System.Threading;


public class ClientCopy : MonoBehaviour
{
	[SerializeField]
	Text t;
	[SerializeField]
	Image recvImage;
	[SerializeField]
	GameObject serverBtn;
	[SerializeField]
	GameObject clinetBtn;
	//public GameObject sendBtn;

	public string m_Ip = "192.168.1.44";
	public int m_Port = 50001;
	private TcpClient m_Client;
	private Thread m_ThrdClientReceive;
    public void ClientBtn()
	{
		ConnectToTcpServer();
	}

	void Update()
	{
		if (incommingData != null)
		{
			OpenFIle.instance.CurrentImage(incommingData);
			OpenFIle.instance.SaveFile(incommingData);

			incommingData = null;
		}        
	}

	//public void SendBtn()
	//{
	//	SendImage();
	//}

	void OnApplicationQuit()
	{
		if(m_ThrdClientReceive != null)
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
			m_Client = new TcpClient(m_Ip, m_Port);

			m_ThrdClientReceive = new Thread(new ThreadStart(ListenForData));
			m_ThrdClientReceive.IsBackground = true;
			m_ThrdClientReceive.Start();

			t.text = "클라이언트";
			serverBtn.SetActive(false);
			clinetBtn.SetActive(false);
		}
		catch (Exception ex)
		{
			Debug.Log(ex);
			t.text = "";
			serverBtn.SetActive(true);
			clinetBtn.SetActive(true);
		}
	}
	byte[] incommingData;
	void ListenForData()
	{
		try
		{
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

	void SendImage()
	{
		if (m_Client == null) return;

		try
		{
			NetworkStream stream = m_Client.GetStream();

			if (stream.CanWrite)
			{
				stream.Write(OpenFIle.instance.byteData, 0, OpenFIle.instance.byteData.Length);
			}
		}

		catch (SocketException ex)
		{
			Debug.Log(ex);
		}
	}
}
