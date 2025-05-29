using UnityEngine;

public class CenterDot : MonoBehaviour
{
    public Texture2D dotTexture;
    public Vector2 dotSize = new Vector2(8, 8); // Width and height of the dot

    void OnGUI()
    {
        if (dotTexture == null)
        {
            // Draw a fallback white dot if no texture is set
            GUI.color = Color.white;
            float x = (Screen.width - dotSize.x) / 2;
            float y = (Screen.height - dotSize.y) / 2;
            GUI.DrawTexture(new Rect(x, y, dotSize.x, dotSize.y), Texture2D.whiteTexture);
        }
        else
        {
            // Draw the provided texture at the center
            float x = (Screen.width - dotSize.x) / 2;
            float y = (Screen.height - dotSize.y) / 2;
            GUI.DrawTexture(new Rect(x, y, dotSize.x, dotSize.y), dotTexture);
        }
    }
}
