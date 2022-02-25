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
		if (instance == null)
			instance = this;
	}

    public void OpenSingleFile()
	{
		string[] extensions = { "jpg", "png" };
		string path = FileBrowser.Instance.OpenSingleFile("Open file", "", "", extensions);
		Debug.Log("Selected file: " + path);

		byteData = FileBrowser.Instance.CurrentOpenSingleFileData;

		//Typical use-cases
		CurrentImage(byteData);
	}

	public void CurrentImage(byte[] data)
	{
		Sprite sprite = data.CTToSprite();
		float x = sprite.pivot.x;
		float y = sprite.pivot.y;
		image.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 720 * x / y);
		image.sprite = sprite;
	}

	public void SaveFile(byte[] data)
	{
		Texture2D tex = data.CTToTexture();
		byte[] pngdata = tex.CTToPNG();
		FileBrowser.Instance.CurrentSaveFileData = pngdata;
		string[] extensions = { "jpg", "png" };

		string path = FileBrowser.Instance.SaveFile("Save file", "", "", extensions);
		Debug.Log("Save file: " + path);
	}
}
