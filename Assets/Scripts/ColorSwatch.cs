using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ColorSwatch : MonoBehaviour
{
    public int ID { get; private set; }

    public bool Selected { get; private set; }


    bool Completed;
    
    TextMeshPro text;
    public SpriteRenderer background;
    SpriteRenderer border;


    [Header("Border Colors")]
    [SerializeField] Color selectedBorderColor = Color.yellow;
    [SerializeField] Color unselectedBorderColor = Color.black;


    private void Awake()
	{
        border = transform.Find("Border").GetComponent<SpriteRenderer>();
        background = transform.Find("Background").GetComponent<SpriteRenderer>();
        text = transform.Find("Text").GetComponent<TextMeshPro>();
	}

    public void SetData(int id, Color color)
	{
        ID = id;
        text.text = id.ToString();
        background.color = color;
	}

    public void SetCompleted()
	{
        Completed = true;
        text.text = "";
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
}
