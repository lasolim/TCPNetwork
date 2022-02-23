using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Crosstales;
using Crosstales.FB;

public class OpenFIle : MonoBehaviour
{
	public static OpenFIle instance;

	public Image image;
	public Byte[] byteData;

    private void Awake()
    {
        if(instance == null)
        {
			instance = this;
        }
    }

    public void OpenSingleFile()
	{
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