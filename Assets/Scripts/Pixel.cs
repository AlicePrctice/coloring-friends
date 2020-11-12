using TMPro;
using UnityEngine;

public class Pixel : MonoBehaviour
{
    public int ID { get; private set; }

	[SerializeField] float wrongColorOpacity = 0.4f;
    TextMeshPro text;
    Color pixelColor;
	
    SpriteRenderer background;
    SpriteRenderer border;
    public bool IsFilledIn
	{
        get
		{
            if (background.color == pixelColor)
			{
                return true;
			}
            else
			{
                return false;
			}
		}
	}

	private void Awake()
	{
        border = transform.Find("Border").GetComponent<SpriteRenderer>();
        background = transform.Find("Background").GetComponent<SpriteRenderer>();
        text = transform.Find("Text").GetComponent<TextMeshPro>();
	}

    public void SetData(Color color, int colorID)
	{
        ID = colorID;
        pixelColor = color;
        border.color = new Color(0.95f, 0.95f, 0.95f, 1);
        text.text = colorID.ToString();

        background.color = Color.Lerp(new Color(pixelColor.grayscale, pixelColor.grayscale, pixelColor.grayscale), Color.white, 0.85f);
	}

    public void SetSelected(bool selected)
	{
        if (selected)
		{
            if (!IsFilledIn)
			{
                background.color = new Color(0.5f, 0.5f, 0.5f, 1);
			}
		}
		else
		{
            if (!IsFilledIn)
			{
                background.color = Color.Lerp(new Color(pixelColor.grayscale, pixelColor.grayscale, pixelColor.grayscale), Color.white, 0.85f);
            }
		}
	}

	public void Fill()
	{
        if (!IsFilledIn)
		{
			border.color = pixelColor;
			background.color = pixelColor;
			text.text = "";
		}
	}

	public void FillWrong()
	{
		if (!IsFilledIn)
		{
			foreach (ColorSwatch colorSwatch in FindObjectOfType<Game>().ColorSwatches)
			{
				if (colorSwatch.Selected == true)
				{
					Color tempColor = colorSwatch.background.color;
					tempColor.a = wrongColorOpacity;

					background.color = tempColor;

					return;
				}
			}
		}
	}
}
