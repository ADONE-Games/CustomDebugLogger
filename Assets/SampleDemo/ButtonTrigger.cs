using UnityEngine;

namespace Scenes
{
    public class ButtonTrigger : MonoBehaviour
    {
        public void OnClick_NormalDebugLog()
        {
            Debug.Log( "Button Clicked!" );
        }
        public void OnClick_CustomDebugLog()
        {
            MyClassLoggerController.Log( "ボタン　クリック！", Color.magenta );
        }
    }
}
