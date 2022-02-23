using UnityEngine;

namespace Crosstales.FB.Demo.Util
{
   /// <summary>Controls the custom wrapper in demo builds.</summary>
   [HelpURL("https://www.crosstales.com/media/data/assets/rtvoice/api/class_crosstales_1_1_r_t_voice_1_1_demo_1_1_util_1_1_custom_provider_controller.html")] //TODO update
   public class CustomWrapperController : MonoBehaviour
   {
      #region Variables

      public Wrapper.BaseCustomFileBrowser Wrapper;

      private bool isCustom;
      private Wrapper.BaseCustomFileBrowser previousWrapper;

      #endregion


      #region MonoBehaviour methods

      private void Start()
      {
         isCustom = FileBrowser.Instance.CustomMode;
         previousWrapper = FileBrowser.Instance.CustomWrapper;

         FileBrowser.Instance.CustomWrapper = Wrapper;
         FileBrowser.Instance.CustomMode = true;
      }

      private void OnDestroy()
      {
         if (FileBrowser.Instance != null)
         {
            FileBrowser.Instance.CustomMode = isCustom;
            FileBrowser.Instance.CustomWrapper = previousWrapper;
         }
      }

      #endregion
   }
}
// © 2020-2021 crosstales LLC (https://www.crosstales.com)