using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ColorSwatch : MonoBehaviour
{
    public int ID { get; private set; }
    

    public bool Selected { get; private set; }

    public int pixelCount;

    bool Completed;
    
    TextMeshProUGUI IDtext;
    TextMeshProUGUI remainingText;
    public UnityEngine.UI.Image background;
    UnityEngine.UI.Image border;


    [Header("Border Colors")]
    [SerializeField] Color selectedBorderColor = Color.yellow;
    [SerializeField] Color unselectedBorderColor = Color.black;


    private void Awake()
	{
        border = transform.Find("Border").GetComponent<UnityEngine.UI.Image>();
        background = transform.Find("Background").GetComponent<UnityEngine.UI.Image>();
        IDtext = transform.Find("IDText").GetComponent<TextMeshProUGUI>();
        remainingText = transform.Find("RemainingText").GetComponent<TextMeshProUGUI>();
	}

    public void SetData(int id, Color color, int pixNum)
	{
        ID = id;
        IDtext.text = id.ToString();
        background.color = color;

        pixelCount = pixNum;
        remainingText.text = pixelCount.ToString();
	}

    public void SetCompleted()
	{
        Completed = true;
        IDtext.text = "";
	}

    public void SetSelected(bool selected)
	{
        if (!Completed)
		{
            Selected = selected;
            if (Selected)
			{
                border.color = selectedBorderColor;
			}
            else
			{
                border.color = unselectedBorderColor;
			}
		}
	}

    public void ReducePixelCount()
	{
        pixelCount--;
        remainingText.text = pixelCount.ToString();
	}


}
