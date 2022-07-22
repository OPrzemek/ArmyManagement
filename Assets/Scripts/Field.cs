using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Field : MonoBehaviour  //Klasa zawierająca informacje o danym polu i metody do ich korekty
{
    public bool isSelected = false; //Czy przycisk jest zaznaczony?
    public List<GameObject> FieldSoldierSize = new List<GameObject>(); //Współlokatorzy żołnierza
    public GameObject SoldierPrefab;
    public GameObject Soldier;      //Żołnierz tego pola
    public Button Button;
    [SerializeField]
    ButtonFunctions ButFunc;
    void Start()
    {
        Soldier = Instantiate(SoldierPrefab);
        Soldier.transform.SetParent(transform);
        Soldier.transform.localPosition = new Vector3(0, 0, 0);
        Button = GetComponent<Button>();
        Button.onClick.AddListener(OnClick);
    }
    public void OnClick() //Zaznaczanie pola
    {
        if(isSelected == false && ButFunc.SelectedFields.Count <= 2)
        {
            if (ButFunc.SelectedFields.Count == 2)
            {
                ButFunc.SelectedFields[0].GetComponent<Field>().isSelected = false;
                ButFunc.SelectedFields.RemoveAt(0);
            }
            isSelected = true;
            ButFunc.SelectedFields.Add(gameObject);
        }
        else
        {
            isSelected = false;
            ButFunc.SelectedFields.Remove(gameObject);
        }
    }
    void Update()
    {
        FieldSoldierSize = Soldier.GetComponent<Soldier>().SoldierSize;
        if (!ButFunc.SelectedFields.Contains(gameObject))
        {
            isSelected = false;
        }
        if (FieldSoldierSize.Count > 0 && Soldier != FieldSoldierSize[0]) //Sprawdzenie, czy żołnierz tego pola jest skrajnie lewy, wyłączenie w innym przypadku
        {
            Button.interactable = false;
            if (Soldier.GetComponent<Soldier>().Image == null) return;
            Soldier.GetComponent<Soldier>().Image.color = Color.gray;
        }
        else
        {
            Button.interactable = true;
            if (Soldier.GetComponent<Soldier>().Image == null) return;
            Soldier.GetComponent<Soldier>().Image.color = Color.white;
        }
    }
}
