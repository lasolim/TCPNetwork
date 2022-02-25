using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System;
using System.Threading;

public class Client : MonoBehaviour
{
	public GameObject serverBtn;
	public GameObject clinetBtn;
	public GameObject sendBtn;
	public GameObject openBtn;

	public string ip = "192.168.0.26";
	public int port = 50001;
	private TcpClient tcp_Client;
	private Thread thrd_receive;
	public void ClientBtn()
	{
		ConnectToTcpServer();
	}

	void Update()
	{
		if (incommingData != null)
		{
			Na_FileBrowser.instance.ShowImage(incommingData);
            Na_FileBrowser.instance.SaveFile(incommingData);
            incommingData = null;
		}
	}

	void OnApplicationQuit()
	{
		if (thrd_receive != null)
			thrd_receive.Abort();

		if (tcp_Client != null)
		{
			tcp_Client.Close();
			tcp_Client = null;
		}
	}

	void ConnectToTcpServer()
	{
		try
		{
			tcp_Client = new TcpClient(ip, port);

			thrd_receive = new Thread(new ThreadStart(ListenForData));
			thrd_receive.IsBackground = true;
			thrd_receive.Start();

			Na_FileBrowser.instance.t.text = "클라이언트";

			serverBtn.SetActive(false);
			clinetBtn.SetActive(false);
			openBtn.SetActive(false);
			sendBtn.SetActive(false);
		}
		catch (Exception ex)
		{
			Debug.Log(ex);
			Na_FileBrowser.instance.t.text = "서버 없음";
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
				using (NetworkStream stream = tcp_Client.GetStream())
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
		if (tcp_Client == null) return;

		try
		{
			NetworkStream stream = tcp_Client.GetStream();

			if (stream.CanWrite)
			{
				stream.Write(Na_FileBrowser.instance.byteData, 0, Na_FileBrowser.instance.byteData.Length);
			}
		}

		catch (SocketException ex)
		{
			Debug.Log(ex);
		}
	}
}
