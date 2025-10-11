using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FDIM.Framework
{
    public static class UIExtensions
    {
        // TextMeshPro
        public static TMP_Text SetText(this TMP_Text text, string str)
        {
            text.text = str;
            return text;
        }

        public static TMP_Text SetColor(this TMP_Text text, Color color)
        {
            text.color = color;
            return text;
        }

        public static TMP_Text SetFontSize(this TMP_Text text, float size)
        {
            text.fontSize = size;
            return text;
        }

        // Image
        public static Image SetSprite(this Image img, Sprite sprite)
        {
            img.sprite = sprite;
            return img;
        }

        public static Image SetColor(this Image img, Color color)
        {
            img.color = color;
            return img;
        }
    }
}