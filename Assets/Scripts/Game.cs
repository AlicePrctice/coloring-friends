using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using System;
using TMPro;
using System.Threading;
using UnityEngine.VFX;

public class Game : MonoBehaviour
{
	[Header("Pixel Art")]
	[SerializeField] Texture2D Texture;
	[Header("Background Settings")]
	[SerializeField] [Range(0, 1)] float cameraBackgroundColorTint = 0.2f;
	[SerializeField] GameObject fireworks;
	

	Pixel[,] Pixels;
    Camera Camera;
    float pixelUnitOffset = 0.5f;
	int pixelAmount = 0;

	int numberOfSwatches = 0;
	int numberOfCompletedSwatches = 0;
    

    int ID = 1;
    Dictionary<Color, int> Colors = new Dictionary<Color, int>();

    // made public for changing color of painted pixels in Pixel.cs
    public List<ColorSwatch> ColorSwatches = new List<ColorSwatch>();

    Dictionary<int, List<Pixel>> PixelGroups = new Dictionary<int, List<Pixel>>();

    RaycastHit2D[] Hits = new RaycastHit2D[1];
    ColorSwatch SelectedColorSwatch;
    GameTools gameTools;

	




    private void Awake()
	{
        Camera = Camera.main;
        gameTools = new GameTools();

		
        

        CreatePixelMap();
        CreateColorSwatches();

		numberOfSwatches = ColorSwatches.Count;
    }

    void CreatePixelMap()
	{
        // Puts all pixels into an array of colors
        Color[] colors = Texture.GetPixels();
		
		Debug.Log(pixelAmount);
        Camera.backgroundColor = Color.Lerp(gameTools.MostFrequentColor(colors), Color.white, cameraBackgroundColorTint);
        


        // Two dimensional array of Pixels storing individual locations
        Pixels = new Pixel[Texture.width, Texture.height];

        for (int x = 0; x < Texture.width; x++)
        {
            for (int y = 0; y < Texture.height; y++)
			{
                // Instantiates Pixel object at location if the alpha layer of location is not 0
                if (colors[x + y * Texture.width].a != 0)
				{
                    GameObject go = GameObject.Instantiate(Resources.Load("Pixel") as GameObject);
                    go.transform.position = new Vector3(x + pixelUnitOffset, y + pixelUnitOffset);
                    go.transform.parent = GameObject.FindGameObjectWithTag("PixelHolder").transform;

                    int id = ID;
                    
                    //
                    if (Colors.ContainsKey(colors[x + y * Texture.width]))
					{
                        id = Colors[colors[x + y * Texture.width]];
					}
                    else
					{
                        Colors.Add(colors[x + y * Texture.width], ID);
                        ID++;
					}

                    Pixels[x, y] = go.GetComponent<Pixel>();
                    Pixels[x, y].SetData(colors[x + y * Texture.width], id);

                    // If PixelGroup doesn't have current ID, add it to the dictionary along with the Pixels array
                    if (!PixelGroups.ContainsKey(id))
					{
                        PixelGroups.Add(id, new List<Pixel>());
					}
                    PixelGroups[id].Add(Pixels[x, y]);
				}
			}
		}
	}

   void CreateColorSwatches()
	{
		
        foreach (KeyValuePair<Color, int> kvp in Colors)
		{
            GameObject go = GameObject.Instantiate(Resources.Load("ColorSwatch") as GameObject);
            go.transform.parent = GameObject.FindGameObjectWithTag("ColorSwatchHolder").transform;
            
            float offset = 1.8f;
            go.transform.localPosition = new Vector2(kvp.Value * 65 * offset, -3);
            
            ColorSwatch colorswatch = go.GetComponent<ColorSwatch>();

			
            

            ColorSwatches.Add(colorswatch);

			int pixelCount = 0;
			Color[] colors = Texture.GetPixels();
			for (int i = 0; i < Pixels.Length; i++)
			{
				if (colors[i] == kvp.Key)
				{
					pixelCount++;
				}
			}
			

			colorswatch.SetData(kvp.Value, kvp.Key, pixelCount);
			pixelAmount += pixelCount;

			if (gameTools.IsColorDark(kvp.Key))
			{
                go.transform.Find("IDText").GetComponent<TextMeshProUGUI>().color = Color.white;
                go.transform.Find("RemainingText").GetComponent<TextMeshProUGUI>().color = Color.white;
			}
		}
	}

	void DeselectAllColorSwatches()
	{
        for (int n = 0; n < ColorSwatches.Count; n++)
		{
            ColorSwatches[n].SetSelected(false);
		}
	}


	private void Update()
	{
		PlayerInput();
	}


	private void PlayerInput()
	{
		Vector2 mousePos = Camera.ScreenToWorldPoint(Input.mousePosition);
		int x = Mathf.FloorToInt(mousePos.x);
		int y = Mathf.FloorToInt(mousePos.y);

		Pixel hoveredPixel = null;

		if (x >= 0 && x < Pixels.GetLength(0) && y >= 0 && y < Pixels.GetLength(1))
		{
			if (Pixels[x, y] != null)
			{
				hoveredPixel = Pixels[x, y];

			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			//Check if we clicked on a color swatch
			int hitCount = Physics2D.RaycastNonAlloc(mousePos, Vector2.zero, Hits);

			for (int n = 0; n < hitCount; n++)
			{
				if (Hits[n].collider.CompareTag("ColorSwatch"))
				{
					SelectColorSwatch(Hits[n].collider.GetComponent<ColorSwatch>());
				}
			}
		}

		if (Input.GetMouseButton(0))
		{
			if (hoveredPixel != null && !hoveredPixel.IsFilledIn)
			{
				if (SelectedColorSwatch != null && SelectedColorSwatch.ID == hoveredPixel.ID)
				{
					FillPixel(hoveredPixel);

					if (CheckIfSelectedComplete())
					{
						SelectedColorSwatch.SetCompleted();
						
					}
				}
				else
				{
					hoveredPixel.FillWrong();
				}
			}
		}
	}

	void FillPixel(Pixel hoveredPixel)
	{
		hoveredPixel.Fill();
		pixelAmount--;
		Debug.Log(pixelAmount);
		SelectedColorSwatch.ReducePixelCounter();
		
		if (pixelAmount <= 0)
		{
			Win();
		}

	}

	void SelectColorSwatch(ColorSwatch swatch)
	{
        if (SelectedColorSwatch != null)
		{
            for (int n = 0; n < PixelGroups[SelectedColorSwatch.ID].Count; n++)
			{
                PixelGroups[SelectedColorSwatch.ID][n].SetSelected(false);
			}

            SelectedColorSwatch.SetSelected(false);
		}

        SelectedColorSwatch = swatch;
        SelectedColorSwatch.SetSelected(true);

        for (int n = 0; n < PixelGroups[SelectedColorSwatch.ID].Count; n++)
		{
            PixelGroups[SelectedColorSwatch.ID][n].SetSelected(true);
		}
	}

    bool CheckIfSelectedComplete()
	{
        if (SelectedColorSwatch != null)
		{
            for (int n = 0; n < PixelGroups[SelectedColorSwatch.ID].Count; n++)
			{
                if (PixelGroups[SelectedColorSwatch.ID][n].IsFilledIn == false)
                    return false;
			}
		}
        return true;
	}

	
	void Win()
	{
		Debug.Log("You won! " + fireworks);
		fireworks.SetActive(true); 
		
		
	}
}