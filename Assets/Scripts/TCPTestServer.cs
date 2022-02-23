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
using System.Linq;

public class TCPTestServer : MonoBehaviour {  	
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

	public Text t;

    // Use this for initialization
    void Start()
    {
		//// Start TcpServer background thread 		
		//tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests));
		//tcpListenerThread.IsBackground = true;
		//tcpListenerThread.Start();
		IPAddress ipaddrss = GetCurrentIPAddress();
		if(ipaddrss == null)
        {
			print("ip 없음");
			return;
        }

		print(ipaddrss);
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

    public static bool isServer = false;

	public void ServerCreateBtn()
    {
		if (TCPTestClient.isClient) return;
        // Start TcpServer background thread 		
        tcpListenerThread = new Thread(new ThreadStart(ListenForIncommingRequests))
        {
            IsBackground = true
        };
        tcpListenerThread.Start();
		isServer = true;
		t.text = "서버";
	}
	
	// Update is called once per frame
	void Update () {
		if (isServer == false) return;

		if (Input.GetKeyDown(KeyCode.Space)) {             
			SendMessage();         
		}

        recvSprite = incommingData.CTToSprite();
        recvImage.sprite = recvSprite;
    }

	public void SendBtn()
    {
		if(isServer)
		SendMessage();
	}

	/// <summary> 	
	/// Runs in background TcpServerThread; Handles incomming TcpClient requests 	
	/// </summary> 	
	public Image recvImage;
	Sprite recvSprite;
	byte[] incommingData = new byte[39999999];
	private void ListenForIncommingRequests () { 		
		try {
			// Create listener on localhost port 8052.
			IPEndPoint serverAddress = new IPEndPoint(IPAddress.Any, 8051);
			tcpListener = new TcpListener(serverAddress);
			tcpListener.Start();              
			Debug.Log("Server is listening");
			Byte[] bytes = new Byte[39999999]; 			
			while (true) { 				
				using (connectedTcpClient = tcpListener.AcceptTcpClient()) { 					
					// Get a stream object for reading 					
					using (NetworkStream stream = connectedTcpClient.GetStream()) {
						int length; 						
						// Read incomming stream into byte arrary. 						
						while ((length = stream.Read(bytes, 0, bytes.Length)) != 0) { 							
							incommingData = new byte[length]; 							
							Array.Copy(bytes, 0, incommingData, 0, length);

                            // Convert byte array to string message. 							
                            //string clientMessage = Encoding.UTF8.GetString(incommingData);

                            Debug.Log("client message received as: " /*+ clientMessage*/);
                        } 					
					} 				
				} 			
			}
		} 		
		catch (SocketException socketException) { 			
			Debug.Log("SocketException " + socketException.ToString());
		}     
	}

	/// <summary> 	
	/// Send message to client using socket connection. 	
	/// </summary> 	
	public Image sendImage;
	private void SendMessage() { 		
		if (connectedTcpClient == null) {
			Debug.Log("클라이언트 없음");
			return;         
		}  		
		
		try {	
			// Get a stream object for writing. 			
			NetworkStream stream = connectedTcpClient.GetStream();
			Debug.Log("보내기 시도");
			if (stream.CanWrite) {
				stream.Write(byteData, 0, byteData.Length);
				Debug.Log("Server sent his message - should be received by client");
			}       
		}
		catch (SocketException socketException) {
			Debug.Log("Socket exception: " + socketException);
		} 	
	}

	Byte[] byteData = new byte[39999999];
	public void OpenSingleFile()
	{
		if (isServer == false) return;

		//string targetPath = @"C:\Users\PC\Desktop\TestImages";
		string[] extensions = { "jpg", "png" };
		string path = FileBrowser.Instance.OpenSingleFile("Open file", "", "", extensions);
		Debug.Log("Selected file: " + path);

		byteData = FileBrowser.Instance.CurrentOpenSingleFileData;
		//Typical use-cases
		Sprite selectedSprite = byteData.CTToSprite(); //returns a Texture of the data

		sendImage.sprite = selectedSprite;
	}
}