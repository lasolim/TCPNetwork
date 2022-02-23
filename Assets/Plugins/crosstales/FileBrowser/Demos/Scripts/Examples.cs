using UnityEngine;
using UnityEngine.UI;

namespace Crosstales.FB.Demo
{
   /// <summary>Examples for all methods.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/FileBrowser/api/class_crosstales_1_1_f_b_1_1_demo_1_1_examples.html")]
   public class Examples : MonoBehaviour
   {
      #region Variables

      public GameObject TextPrefab;

      public GameObject ScrollView;

      public Button OpenFilesBtn;
      public Button OpenFoldersBtn;

      protected string testPath = @"D:\slaubenberger\git\assets\FileBrowser";

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         rebuildList(null);

         //Debug.Log($"{Time.timeSinceLevelLoad} - Start", this);

         FileBrowser.Instance.OnOpenFilesComplete += onOpenFilesComplete;
         FileBrowser.Instance.OnOpenFoldersComplete += onOpenFoldersComplete;
         FileBrowser.Instance.OnSaveFileComplete += onSaveFileComplete;

         if (OpenFilesBtn != null)
            OpenFilesBtn.interactable = FileBrowser.Instance.canOpenMultipleFiles;

         if (OpenFoldersBtn != null)
            OpenFoldersBtn.interactable = FileBrowser.Instance.canOpenMultipleFolders;
      }

      private void OnDestroy()
      {
         if (FileBrowser.Instance != null)
         {
            FileBrowser.Instance.OnOpenFilesComplete -= onOpenFilesComplete;
            FileBrowser.Instance.OnOpenFoldersComplete -= onOpenFoldersComplete;
            FileBrowser.Instance.OnSaveFileComplete -= onSaveFileComplete;
         }
      }

      #endregion


      #region Public methods

      public void OpenSingleFile()
      {
         //string path = FileBrowser.Instance.OpenSingleFile("Open my important single file", testPath, "Image.png", new ExtensionFilter("Image Files", "png", "jpg", "jpeg"), new ExtensionFilter("Sound Files", "mp3", "wav"), new ExtensionFilter(FileBrowser.Instance.TextAllFiles, "*"));
         //string path = FileBrowser.Instance.OpenSingleFile("Open my important single file", testPath, "Text.txt", "txt", "jpg", "pdf");
         string path = FileBrowser.Instance.OpenSingleFile("txt");
         //string path = FileBrowser.Instance.OpenSingleFile();

         Debug.Log($"OpenSingleFile: '{path}'", this);
      }

      public void OpenFiles()
      {
         ExtensionFilter[] _Filters = new[] { new ExtensionFilter("Image Files", "png", "jpg", "jpeg") };
         string[] paths = FB.FileBrowser.Instance.OpenFiles("Some Prompt", "", "", _Filters);

         //string[] paths = FileBrowser.Instance.OpenFiles("Open my important files", testPath, "Image.png", new ExtensionFilter("Image Files", "png", "jpg", "jpeg"), new ExtensionFilter("Sound Files", "mp3", "wav"), new ExtensionFilter(FileBrowser.Instance.TextAllFiles, "*"));
         //string[] paths = FileBrowser.Instance.OpenFiles("Open my important files", testPath, "Text.txt", "txt", "jpg", "pdf");
         //string[] paths = FileBrowser.Instance.OpenFiles("txt");
         //string[] paths = FileBrowser.Instance.OpenFiles();

         Debug.Log($"OpenSingleFile: {paths.CTDump()}", this);
      }

      public void OpenSingleFolder()
      {
         //string path = FileBrowser.Instance.OpenSingleFolder("Open my important folder", testPath);
         string path = FileBrowser.Instance.OpenSingleFolder();

         Debug.Log($"OpenSingleFolder: '{path}'", this);
      }

      public void OpenFolders()
      {
         //string[] paths = FileBrowser.OpenFolders("Open my important folders", testPath);
         string[] paths = FileBrowser.Instance.OpenFolders();

         Debug.Log($"OpenFolders: {paths.CTDump()}", this);
      }

      public void SaveFile()
      {
         //Add some data for WebGL
         if (Crosstales.FB.Util.Helper.isWebPlatform)
            FileBrowser.Instance.CurrentSaveFileData = System.Text.Encoding.UTF8.GetBytes($"Content created with {Crosstales.FB.Util.Constants.ASSET_NAME}: {Crosstales.FB.Util.Constants.ASSET_PRO_URL}");

         //string path = FileBrowser.Instance.SaveFile("Save my important file", testPath, "MySuperFile", new ExtensionFilter("Binary", "bin"), new ExtensionFilter("Text", "txt", "md"), new ExtensionFilter("C#", "cs"));
         //string path = FileBrowser.Instance.SaveFile("Save my important file", testPath, null, "bin", "txt", "cs");
         string path = FileBrowser.Instance.SaveFile(null, "txt");
         //string path = FileBrowser.Instance.SaveFile();
         //string path = FileBrowser.Instance.SaveFile("save_" + System.DateTime.Now.ToString("dd-MM-yyyy_HH:mm"), "json");

         Debug.Log($"SaveFile: '{path}'", this);
      }

      public void OpenSingleFileAsync()
      {
         //FileBrowser.Instance.OpenSingleFileAsync("Open my important files", testPath, "Image.png", new ExtensionFilter("Image Files", "png", "jpg", "jpeg"), new ExtensionFilter("Sound Files", "mp3", "wav"), new ExtensionFilter(FileBrowser.Instance.TextAllFiles, "*"));
         //FileBrowser.Instance.OpenSingleFileAsync("Open my important files", testPath, "Text.txt", "txt", "png");
         FileBrowser.Instance.OpenSingleFileAsync("txt");
         //FileBrowser.Instance.OpenSingleFileAsync();
      }

      public void OpenFilesAsync()
      {
         //FileBrowser.Instance.OpenFilesAsync("Open my important files", testPath, "Image.png", true, new ExtensionFilter("Image Files", "png", "jpg", "jpeg"), new ExtensionFilter("Sound Files", "mp3", "wav"), new ExtensionFilter(FileBrowser.Instance.TextAllFiles, "*"));
         //FileBrowser.Instance.OpenFilesAsync("Open my important files", testPath, "Image.png", true, new ExtensionFilter("Alle Dateien", "*"));
         //FileBrowser.Instance.OpenFilesAsync("Open my important files", testPath, "Text.txt", true, "txt", "png");
         FileBrowser.Instance.OpenFilesAsync(true, "txt");
         //FileBrowser.Instance.OpenFilesAsync();
      }

      public void OpenSingleFolderAsync()
      {
         //FileBrowser.Instance.OpenSingleFolderAsync("Open my important folder", testPath);
         FileBrowser.Instance.OpenSingleFolderAsync();
      }

      public void OpenFoldersAsync()
      {
         //FileBrowser.Instance.OpenFoldersAsync("Open my important folders", testPath, true);
         FileBrowser.Instance.OpenFoldersAsync();
      }

      public void SaveFileAsync()
      {
         //Add some data for WebGL
         if (Crosstales.FB.Util.Helper.isWebPlatform)
            FileBrowser.Instance.CurrentSaveFileData = System.Text.Encoding.UTF8.GetBytes($"Content created with {Crosstales.FB.Util.Constants.ASSET_NAME}: {Crosstales.FB.Util.Constants.ASSET_PRO_URL}");

         //FileBrowser.Instance.SaveFileAsync("Save my important file", testPath, "MySuperFile", new ExtensionFilter("Binary", "bin"), new ExtensionFilter("Text", "txt"), new ExtensionFilter("C#", "cs"));
         //FileBrowser.Instance.SaveFileAsync("Save my important file", testPath, null, "txt", "cs");
         FileBrowser.Instance.SaveFileAsync(null, "txt");
         //FileBrowser.Instance.SaveFileAsync();

         //FileBrowser.Instance.SaveFileAsync(saveAction);
      }

/*
      private void saveAction(string file)
      {
         Debug.Log(file);
      }
*/

      #endregion


      #region Private methods

      private void rebuildList(params string[] items)
      {
         for (int ii = ScrollView.transform.childCount - 1; ii >= 0; ii--)
         {
            Transform child = ScrollView.transform.GetChild(ii);
            child.SetParent(null);
            Destroy(child.gameObject);
         }

         if (items?.Length > 0 && items[0] != null)
         {
            ScrollView.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 80 * items.Length);

            for (int ii = 0; ii < items.Length; ii++)
            {
               GameObject go = Instantiate(TextPrefab, ScrollView.transform, true);

               go.transform.localScale = Vector3.one;
               go.transform.localPosition = new Vector3(10, -80 * ii, 0);
               go.GetComponent<Text>().text = items[ii];
            }
         }
         else
         {
            GameObject go = Instantiate(TextPrefab, ScrollView.transform, true);

            go.transform.localScale = Vector3.one;
            go.transform.localPosition = new Vector3(10, 0, 0);
            go.GetComponent<Text>().text = "Nothing selected!";
         }
      }

      #endregion


      #region Callbacks

      private void onOpenFilesComplete(bool selected, string singleFile, string[] files)
      {
         Debug.Log($"onOpenFilesComplete: {selected} - '{singleFile}' - {(FileBrowser.Instance.CurrentOpenSingleFileData == null ? "0" : FB.Util.Helper.FormatBytesToHRF(FileBrowser.Instance.CurrentOpenSingleFileData.Length))}", this);

         rebuildList(files);
      }

      private void onOpenFoldersComplete(bool selected, string singleFolder, string[] folders)
      {
         Debug.Log($"onOpenFoldersComplete: {selected} - '{singleFolder}'", this);

         rebuildList(folders);
      }

      private void onSaveFileComplete(bool selected, string file)
      {
         Debug.Log($"onSaveFileComplete: {selected} - '{file}' - {(FileBrowser.Instance.CurrentSaveFileData == null ? "0" : FB.Util.Helper.FormatBytesToHRF(FileBrowser.Instance.CurrentSaveFileData.Length))}", this);

         rebuildList(file);
      }

      #endregion
   }
}
// © 2017-2021 crosstales LLC (https://www.crosstales.com)