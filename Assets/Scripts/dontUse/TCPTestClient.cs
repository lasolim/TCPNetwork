using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Crosstales;
using Crosstales.FB;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class TCPTestClient : MonoBehaviour {  	
	#region private members 	
	private TcpClient socketConnection; 	
	private Thread clientReceiveThread;
	#endregion
	// Use this for initialization 	
	//void Start () {
	//       ConnectToTcpServer();
	//   }  	
	public static bool isClient;
	public Text t;
	public void ClientJoinBtn()
    {
		if (TCPTestServer.isServer) return;
		ConnectToTcpServer();
		isClient = true;
		t.text = "클라이언트";
	}
	// Update is called once per frame
	void Update () {

		if (isClient == false) return;

		if (Input.GetKeyDown(KeyCode.Space)) {             
			SendMessage();
		}

		if (Input.GetKeyDown(KeyCode.S))
		{
			SaveFile();
		}

		if (recvData)
		{
			Sprite recvSprite = incommingData.CTToSprite();
			recvImage.sprite = recvSprite;

			Texture2D tex = incommingData.CTToTexture();
			jpgdata = tex.CTToPNG();
			FileBrowser.Instance.CurrentSaveFileData = jpgdata;
			SaveFile();

			recvData = false;
		}
	}
	byte[] jpgdata;

    public void SaveFile()
    {
        string path = FileBrowser.Instance.SaveFile("MyFile", "png");
        Debug.Log("Save file: " + path);
    }

    public void SendBtn()
    {
		if(isClient)
		SendMessage();
	}
	/// <summary> 	
	/// Setup socket connection. 	
	/// </summary> 	
	private void ConnectToTcpServer () { 		
		try {  			
			clientReceiveThread = new Thread (new ThreadStart(ListenForData)); 			
			clientReceiveThread.IsBackground = true; 			
			clientReceiveThread.Start();  		
		} 		
		catch (Exception e) { 			
			Debug.Log("On client connect exception " + e); 		
		} 	
	}
	/// <summary> 	
	/// Runs in background clientReceiveThread; Listens for incomming data. 	
	/// </summary>     
	public Image recvImage;
	bool recvData = false;
	byte[] incommingData = new byte[39999999];
	private void ListenForData() {
		try {
			IPEndPoint clientAddress = new IPEndPoint(IPAddress.Loopback, 8051);
			socketConnection = new TcpClient(clientAddress);
			Byte[] bytes = new Byte[39999999];             
			while (true) { 				
				// Get a stream object for reading 				
				using (NetworkStream stream = socketConnection.GetStream()) { 					
					int length; 					
					// Read incomming stream into byte arrary. 					
					while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 						
						incommingData = new byte[length]; 						
						Array.Copy(bytes, 0, incommingData, 0, length);
						// Convert byte array to string message. 						
						//string serverMessage = Encoding.ASCII.GetString(incommingData); 	
						recvData = true;
						Debug.Log("server message received as: " /*+ serverMessage*/);				
					} 				
				} 			
			}         
		}         
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		}     
	}  	
	/// <summary> 	
	/// Send message to server using socket connection. 	
	/// </summary> 	
	private void SendMessage() {         
		if (socketConnection == null) {
			Debug.Log("서버 없음");
			return;         
		}  		
		try { 			
			// Get a stream object for writing. 			
			NetworkStream stream = socketConnection.GetStream(); 			
			if (stream.CanWrite) {
				// Write byte array to socketConnection stream.
				stream.Write(byteData, 0, byteData.Length);                 
				Debug.Log("Client sent his message - should be received by server");             
			}         
		} 		
		catch (SocketException socketException) {             
			Debug.Log("Socket exception: " + socketException);         
		}     
	}

	public Image image;
	public Byte[] byteData = new byte[39999999];
	public void OpenSingleFile()
	{
		if (isClient == false) return;

		//string targetPath = @"C:\Users\PC\Desktop\TestImages";
		string[] extensions = { "jpg", "png" };
		string path = FileBrowser.Instance.OpenSingleFile("Open file", "", "", extensions);
		Debug.Log("Selected file: " + path);

		byteData = FileBrowser.Instance.CurrentOpenSingleFileData;
		//Typical use-cases
		Sprite selectedSprite = byteData.CTToSprite(); //returns a Texture of the data

		image.sprite = selectedSprite;
	}
}