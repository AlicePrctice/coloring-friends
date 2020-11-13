using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTools : MonoBehaviour
{
    public Color MostFrequentColor(Color[] colors)
    {
        int count = 1, tempCount;
        Color frequentColor = colors[0];
        Color tempColor;

        for (int i = 0; i < (colors.Length - 1); i++)
        {
            tempColor = colors[i];
            tempCount = 0;
            for (int j = 0; j < colors.Length; j++)
            {
                if (tempColor == colors[j])
                {
                    tempCount++;
                }
            }
            if (tempCount > count)
            {
                frequentColor = tempColor;
                count = tempCount;
            }
        }

        return frequentColor;
    }

    public bool IsColorDark(Color color)
	{
        if (color.r <= 0.5f &&
            color.g <= 0.5f &&
            color.b <= 0.5f &&
            color.a == 1f)
		{
            return true;
        }
        else
		{
            return false;
		}
	}

    
}
