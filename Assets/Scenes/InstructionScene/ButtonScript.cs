using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// image switcher
public class ButtonScript : MonoBehaviour
{
    public Image screenshotImage;
    public List<Sprite> imageList;

    void Start()
    {
        // Load all the sprites from the "imageFolder" folder in the Resources folder
        Sprite[] sprites = Resources.LoadAll<Sprite>("InstructionImages");

        // Add the sprites to the imageList
        imageList.AddRange(sprites);
    }

    public void NextImage()
    {
        // Cycle through the images in the imageList
        int newIndex = (imageList.IndexOf(screenshotImage.sprite) + 1) % imageList.Count;
        screenshotImage.sprite = imageList[newIndex];
    }
}
