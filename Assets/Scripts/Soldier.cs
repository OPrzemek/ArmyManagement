using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Soldier : MonoBehaviour
{
    public List<GameObject> SoldierSize = new List<GameObject>();
    public List<Sprite> Sprites = new List<Sprite>();
    public Image Image;
    public Text Text;
    public GameObject Field;
    [SerializeField]
    private ButtonFunctions ButFunc;
    private void Start()
    {
        ButFunc = GetComponentInParent<ButtonFunctions>();
        Sprites.Add(Resources.Load<Sprite>("ButtonSprites/default"));
        Sprites.Add(Resources.Load<Sprite>("ButtonSprites/selected"));
    }
    private void Update()
    {
        Field = gameObject.transform.parent.gameObject;
        Image = GetComponent<Image>();
        Text = GetComponentInChildren<Text>();
        if (ButFunc.SelectedFields.Contains(Field))  //Zmiana obrazu w zależności od stanu zaznaczenia pola
            Image.sprite = Sprites[1];
        else
            Image.sprite = Sprites[0];

        if(SoldierSize.Count <= 0)
        {
            Text.text = "";
            return;
        }
        for (int i = 0; i < SoldierSize.Count; i++)
        {
            if (SoldierSize[i] == null) continue;
            GameObject Mate = SoldierSize[i];
            Soldier MateSoldier = Mate.GetComponent<Soldier>();
            if (MateSoldier == null || MateSoldier.Text == null) continue;
            MateSoldier.Text.text = $"Soldier {(i + 1)}/{MateSoldier.SoldierSize.Count}";
        }
    }
}
