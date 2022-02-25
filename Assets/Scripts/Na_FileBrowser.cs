using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Crosstales;
using Crosstales.FB;

public class Na_FileBrowser : MonoBehaviour
{
    public static Na_FileBrowser instance;

    public Image image;
	public Byte[] byteData;
	public Text t;
	public GameObject saveBtn;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start()
    {
		byteData = null;
    }

    public void OpenSingleFile()
	{
		string[] extensions = { "jpg", "png" };
		string path = FileBrowser.Instance.OpenSingleFile("Open file", "", "", extensions);
		Debug.Log("Selected file: " + path);

		byteData = FileBrowser.Instance.CurrentOpenSingleFileData;

		ShowImage(byteData);
	}

	public void ShowImage(byte[] data)
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
		saveBtn.SetActive(true);
		StartCoroutine(SaveCorutine());
	}

	IEnumerator SaveCorutine()
    {
		yield return new WaitForSeconds(1f);
		SaveBtn();
	}

	public void SaveBtn()
	{
		string[] extensions = { "jpg", "png" };
		string path = FileBrowser.Instance.SaveFile("Save file", "", "", extensions);
		Debug.Log("Save file: " + path);
    }
}


