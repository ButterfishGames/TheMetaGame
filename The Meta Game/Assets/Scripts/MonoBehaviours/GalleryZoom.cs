using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GalleryZoom : MonoBehaviour
{
    private GameController.ArtStr art;

    public GameObject zoomImg;
    public GameObject descTextBox;

    public TextMeshProUGUI descText;

    private void Start()
    {
        int ind = int.Parse(gameObject.name.Substring(8, gameObject.name.Length - 9));
        GetComponent<Button>().onClick.AddListener(() => ShowZoomedView(ind));

        art = GameController.singleton.artList[ind];
    }

    public void ShowZoomedView(int artInd)
    {
        Image img = zoomImg.GetComponent<Image>();

        if (!art.unlocked)
        {
            return;
        }

        float w = art.img.texture.width;
        float h = art.img.texture.height;

        float tempW = w;
        float tempH = h;

        for (int i = 2; tempW > 1860 || tempH > 720; i++)
        {
            tempW = w / i;
            tempH = h / i;
        }

        float xBound = (1920 - tempW) / 2;
        float yBound = (780 - tempH) / 2;

        img.rectTransform.offsetMin = new Vector2(xBound, yBound);
        img.rectTransform.offsetMax = new Vector2(-xBound, -yBound);
        img.sprite = art.img;
        
        string desc = "<b><size=48>" + art.name + "</b></size>\n";
        desc += "<size=40>" + art.desc + "</size>";
        descText.text = desc;

        zoomImg.SetActive(true);
        descTextBox.SetActive(true);
    }
}
