using UnityEngine;
using System.Collections;

namespace SharedAssets.GuiMain
{
    [RequireComponent(typeof(UIWidget))]
    /// <summary>
    /// A simple helepr to connect UI Elements for fast & fixed access from the outside
    /// </summary>
    public class LiveScoreInstanceWidgetBehaviour : MonoBehaviour
    {
        public UILabel Label = null;
        public UISprite PointUp = null;
        public UISprite PointRightUp = null;
        public UISprite PointRight = null;
        public UISprite PointRightDown = null;
        public UISprite PointDown = null;
        public UISprite PointLeftDown = null;
        public UISprite PointLeft = null;
        public UISprite PointLeftUp = null;


        private UIWidget mWidget = null;
        public UIWidget Widget
        {
            get
            {
                if (mWidget == null)
                    mWidget = this.GetComponent<UIWidget>();
                return mWidget;
            }
        }

        public static implicit operator UIWidget(LiveScoreInstanceWidgetBehaviour value)
        {
            return value.Widget;
        }
    }
}