using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SwapButton : MonoBehaviour
{
    [SerializeField] ButtonFunctions ButFunc;
    bool IsOneOfTheFieldsFull()
    {
        if (ButFunc.SelectedFields.Count == 2 && (!ButFunc.isFieldEmpty(ButFunc.SelectedFields[0]) || !ButFunc.isFieldEmpty(ButFunc.SelectedFields[1])))
            return true;
        else return false;
    }
    void Update()
    {
        if (IsOneOfTheFieldsFull() == true)
        {
            gameObject.GetComponent<Button>().interactable = true;
        }
        else gameObject.GetComponent<Button>().interactable = false;
    }
}
