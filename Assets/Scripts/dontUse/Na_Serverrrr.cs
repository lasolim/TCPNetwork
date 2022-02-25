using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Crosstales;
using Crosstales.FB;

public class Na_Serverrrr : MonoBehaviour
{
	public class AsyncStateData
	{
		public byte[] Buffer;
		public Socket Socket;

	}

	static List<AsyncStateData> clients;
	static List<AsyncStateData> disconnectList;

	public static bool isServer = false;
    public Text t;

	#region private members 	
	/// <summary> 	
	/// TCPListener to listen for incomming TCP connection 	
	/// requests. 	
	/// </summary> 	
	private TcpListener tcpListener;
	/// <summary> 
	/// Background thread for TcpServer workload. 	
	/// </summary> 	
	private Thread tcpListenerThread;
	/// <summary> 	
	/// Create handle to connected tcp client. 	
	/// </summary> 	
	private TcpClient connectedTcpClient;
	#endregion

	byte[] serverBuffer = new byte[39999999];
	public void ServerCreateBtn()
    {
        if (TCPTestClient.isClient) return;

		// Create listener on localhost port 8052. 			
		tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8052);
		tcpListener.Start();
		Debug.Log("Server is listening");



		t.text = "서버";
    }

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
			tcpListener.BeginAcceptTcpClient(AcceptCallback, tcpListener);

			Debug.Log("Someone has connected!!!!");

			if (clients.Count > 0)
			{
				Debug.Log("client successfully added to the list of clients");
			}

			//send a message to everyone say someone has connected
			//BroadCastData("some client has connected", clients);
		}
		catch (Exception e)
		{
			print("받기 오류 : " + e);
		}

	}

	//public void BroadCastData(string data, List<AsyncStateData> clients)
	//{

	//	foreach (var cl in clients)
	//	{
	//		try
	//		{
	//			//send data back to client
	//			Send(cl.Socket, "hello from server");
	//			Debug.Log("sent a message to the client");
	//		}
	//		catch (Exception ex)
	//		{
	//			Debug.Log("error writing data: " + ex.Message);
	//		}
	//	}

	//}

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
		catch (Exception e)
		{
			print("받기 오류 : " + e);
		}
	}

	private void ListenForIncommingRequests()
	{
		try
		{
			Byte[] bytes = new Byte[39999999];
			while (true)
			{
				using (connectedTcpClient = tcpListener.AcceptTcpClient())
				{
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream())
					{
						int length;
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
						{
							var incommingData = new byte[length];
							Array.Copy(bytes, 0, incommingData, 0, length);

							// Convert byte array to string message. 							
							//string clientMessage = Encoding.UTF8.GetString(incommingData); 							
							//recvSprite = incommingData.CTToSprite();
							//recvImage.sprite = recvSprite;

							Debug.Log("client message received as: " /*+ clientMessage*/);
						}
					}
				}
			}
		}
		catch (SocketException socketException)
		{
			Debug.Log("SocketException " + socketException.ToString());
		}
	}
}
