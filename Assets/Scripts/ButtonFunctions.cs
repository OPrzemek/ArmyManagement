using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFunctions : MonoBehaviour //Zawiera zachowania wszystkich przycisków (funkcyjnych jak i przycisków pól)
{
    public List<GameObject> SelectedFields = new List<GameObject>(); //Przechowanie obecnie zaznaczonych pól w liście
    public List<GameObject> AllyArmy = new List<GameObject>();       //Przechowanie liczby pól sojusznika
    public List<GameObject> EnemyArmy = new List<GameObject>();      //Przechowanie liczby pól wroga
    private void Start()
    {
        foreach (GameObject Ally in GameObject.FindGameObjectsWithTag("AllyArmyField"))   //Dodanie wszystkich pól będących polami sojusznika
        {
            AllyArmy.Add(Ally);
        }
        foreach (GameObject Enemy in GameObject.FindGameObjectsWithTag("EnemyArmyField")) //Dodanie wszystkich pól będących polami wroga
        {
            EnemyArmy.Add(Enemy);
        }
    }
    public void AddSingleHandler() //Metoda dla przycisku dodania jedno-polowego żołnierza
    {
        Debug.Log("Dodawanie jedno-polowych żołnierzy...");
        foreach (GameObject field in SelectedFields)
            AddSizedUnit(field, 1);
        SelectedFields.Clear();
    }
    public void AddDoubleHandler() //Metoda dla przycisku dodania dwu-polowego żołnierza
    {
        Debug.Log("Dodawanie dwu-polowych żołnierzy...");
        if(SelectedFields.Count == 2) //Jeżeli zaznaczone pola są obok siebie, chcemy zaczynać zawsze od prawego pola 
        {                             //(w ten sposób, jeżeli to możliwe, utworzy się dwóch żołnierzy)
            if (AllyArmy.Contains(SelectedFields[0]))
            {
                if (AllyArmy.Contains(SelectedFields[1]) 
                    && AllyArmy.IndexOf(SelectedFields[0]) > AllyArmy.IndexOf(SelectedFields[1]))
                    SelectedFields.Reverse();
            }
            if (EnemyArmy.Contains(SelectedFields[0]))
            {
                if (EnemyArmy.Contains(SelectedFields[1]) 
                    && EnemyArmy.IndexOf(SelectedFields[0]) > EnemyArmy.IndexOf(SelectedFields[1]))
                    SelectedFields.Reverse();
            }
        }
        for (int i = SelectedFields.Count - 1; i >= 0; i--)
        {
            GameObject Field = SelectedFields[i];
            if (Field.GetComponent<Field>().FieldSoldierSize.Count != 0)
            {
                Debug.LogWarning("Nie można tu utworzyć żołnierza!");
                continue;
            }
            AddSizedUnit(Field, 2);
        }
        SelectedFields.Clear();
    }
    public void DeleteButton() //Metoda dla przycisku usuwania żołnierzy z zaznaczonych pól
    {
        Debug.Log("Usuwanie żołnierzy...");
        foreach (GameObject _Field in SelectedFields)
            DeleteUnit(_Field);
        SelectedFields.Clear();
    }
    public void SwapButton()  //Metoda dla przycisku zamieniania żołnierzy na zaznaczonych polach
    {
        Debug.Log("Zamienianie żołnierzy...");
        Field field1 = SelectedFields[0].GetComponent<Field>();
        Field field2 = SelectedFields[1].GetComponent<Field>();
        List<GameObject> neighbors1, neighbors2;

        if (field1.FieldSoldierSize.Count == field2.FieldSoldierSize.Count) // 1. Takie same rozmiary bez problemu się zamienią
        {
            SwapUnit(SelectedFields[0], field1.FieldSoldierSize.Count,
                     SelectedFields[1], field2.FieldSoldierSize.Count);
            SelectedFields.Clear();
            return;
        }
        int field1_FieldSoldierSize = field1.FieldSoldierSize.Count;
        int field2_FieldSoldierSize = field2.FieldSoldierSize.Count;
        //Kiedy żołnierze są różnego rozmiaru, chcemy aby pierwszy zawsze był większy niż drugi
        if (field1.FieldSoldierSize.Count > field2.FieldSoldierSize.Count)
        {
            int tmp = field1_FieldSoldierSize;
            field1_FieldSoldierSize = field2_FieldSoldierSize;
            field2_FieldSoldierSize = tmp;
            SelectedFields.Reverse();
        }
        neighbors1 = new List<GameObject>(FieldNeighbors(SelectedFields[0])); //Sąsiedzi mniejszego żołnierza
        neighbors2 = new List<GameObject>(FieldNeighbors(SelectedFields[1])); //Sąsiedzi większego żołnierza

        //Kiedy jedno z pól jest puste...
        if (field1_FieldSoldierSize == 0)
        {
            // 1. Jest pojedynczy, zmieści się
            if (field2_FieldSoldierSize == 1)
            {
                SwapUnit(SelectedFields[0], field2_FieldSoldierSize,
                         SelectedFields[1], 0);
                return;
            }
            // 2. i 3. Lewy lub prawy sąsiad pustego też pusty
            if (isFieldEmpty(neighbors1[0]) || isFieldEmpty(neighbors1[1]))
            {
                SwapUnit(SelectedFields[0], field2_FieldSoldierSize,
                         SelectedFields[1], 0);
                return;
            }
            // 4. Nie można tu zmieścić dwu-polowego żołnierza
            Debug.Log("Nie można tu zmieścić żołnierza!");
            SelectedFields.Clear();
            return;
        }

        //Co, jeśli jeden jest większy? Mogą się zamienić, kiedy...
        // 2. Mniejszy po lewej stronie, prawy sąsiad mniejszego pusty
        if (isFieldEmpty(neighbors1[1]))
        { 
            SwapUnit(SelectedFields[1], field1_FieldSoldierSize,
                     SelectedFields[0], field2_FieldSoldierSize);
            return;
        }
        // 3. Lewy sąsiad mniejszego pusty
        if (isFieldEmpty(neighbors1[0]))
        { 
            SwapUnit(SelectedFields[1], field1_FieldSoldierSize,
                     neighbors1[0], field2_FieldSoldierSize);
            return;
        }

        // 4. Prawy sąsiad mniejszego jest zaznaczonym polem większego
        if (neighbors1[1] == SelectedFields[1])
        {  // #4
            SwapUnit(SelectedFields[0], field2_FieldSoldierSize,
                     neighbors2[1], field1_FieldSoldierSize);
            return;
        }

        // 5. Lewy sąsiad mniejszego jest prawym sąsiadem większego
        if (neighbors1[0] == neighbors2[1]
            && !isFieldEmpty(neighbors2[0]))
        { 
            SwapUnit(SelectedFields[1], field1_FieldSoldierSize,
                     neighbors2[1], field2_FieldSoldierSize);
            return;
        }

        // 6. Mniejszy po prawej stronie, prawy sąsiad mniejszego pusty
        if (isFieldEmpty(neighbors2[0]))
        { 
            SwapUnit(SelectedFields[0], field2_FieldSoldierSize,
                     SelectedFields[1], field1_FieldSoldierSize);
            return;
        }
        // 7. Większy się nie zmieści
        Debug.LogWarning("Nie można dokonać zamiany żołnierzy!");
        SelectedFields.Clear();
        return;
    }
    //Metoda, która usuwa żołnierzy z wybranych pól i wstawia nowych na te pola, o podanych rozmiarach 
    private void SwapUnit(GameObject FirstField, int FirstSize, GameObject SecondField, int SecondSize)
    {
        DeleteUnit(SelectedFields[0]);
        DeleteUnit(SelectedFields[1]);

        AddSizedUnit(FirstField, FirstSize);
        AddSizedUnit(SecondField, SecondSize);
        SelectedFields.Clear();
    }
    public bool isFieldEmpty(GameObject Field)
    {
        if (Field == null) return false;
        if (Field.GetComponent<Field>().FieldSoldierSize.Count == 0) return true;
        else return false;
    }
    private static void AddSingleUnit(GameObject _Field) //Metoda, która do każdego zaznaczonego pola, o ile to możliwe, dodaje jedno-polowego żołnierza
    {
        Field Field = _Field.GetComponent<Field>();
        Soldier Soldier = Field.Soldier.GetComponent<Soldier>();
        if (Soldier.SoldierSize.Count == 0)
        {
            Soldier.SoldierSize.Add(Field.Soldier);
        }
        else Debug.LogWarning("Nie można dodać jedno-polowego żołnierza na to pole!");
    }
    private void AddSizedUnit(GameObject Field, int armySize) //Nadpisana metoda dodawania żołnierza o wybranym rozmiarze, która dodatkowo sprawdza do której armii należy zaznaczone pole, jeżeli jej nie podamy
    {
        if(armySize == 0)
        {
            return;
        }
        if (armySize == 1)
        {
            AddSingleUnit(Field);
            return;
        }

        if (AllyArmy.Contains(Field))
        {
            AddSizedUnit(Field, AllyArmy, armySize);
        }
        else if (EnemyArmy.Contains(Field)) // TODO: if do wywalenia?
        {
            AddSizedUnit(Field, EnemyArmy, armySize);
        }
    }
    private void AddSizedUnit(GameObject Field, List<GameObject> Army, int armySize) //Metoda, która do każdego zaznaczonego pola, o ile to możliwe, dodaje żołnierza o wybranym rozmiarze
    {
        int fieldIndex = Army.IndexOf(Field);   //Indeks obecnego pola w wybranej armii
        GameObject FieldSoldier = Army[fieldIndex].GetComponent<Field>().Soldier; //Żołnierz obecnego pola
        GameObject LeftFieldSoldier = null;            //Żołnierz lewego pola, co do obecnego
        GameObject RightFieldSoldier = null;           //Żołnierz prawego pola, co do obecnego
        if (fieldIndex != 0)
        {
            LeftFieldSoldier = Army[fieldIndex - 1].GetComponent<Field>().Soldier;
        }
        if (fieldIndex != (Army.Count - 1))
        {
            RightFieldSoldier = Army[fieldIndex + 1].GetComponent<Field>().Soldier;
        }
        List<GameObject> FieldSoldierSize = FieldSoldier.GetComponent<Soldier>().SoldierSize;
        if (FieldSoldierSize.Count == 0 && RightFieldSoldier != null            //Jeżeli nie ma żołnierza na tym polu,
            && RightFieldSoldier.GetComponent<Soldier>().SoldierSize.Count == 0)//oraz nie ma żołnierza na prawym polu, dodaj żołnierza
        {
            FieldSoldierSize.Add(FieldSoldier);
            FieldSoldierSize.Add(RightFieldSoldier);
            RightFieldSoldier.GetComponent<Soldier>().SoldierSize = new List<GameObject>(FieldSoldierSize);
        }
        else if (FieldSoldierSize.Count == 0 && LeftFieldSoldier != null
            && LeftFieldSoldier.GetComponent<Soldier>().SoldierSize.Count == 0) //Analogicznie do pierwszego
        {
            FieldSoldierSize.Add(LeftFieldSoldier);
            FieldSoldierSize.Add(FieldSoldier);
            LeftFieldSoldier.GetComponent<Soldier>().SoldierSize = new List<GameObject>(FieldSoldierSize);
        }
        else Debug.LogWarning("Nie można tu dodać dwu-polowego żołnierza!");
    }
    private static void DeleteUnit(GameObject _Field) //Metoda, która usuwa żołnierzy ze zaznaczonych pól oraz całego rodzeństwa żołnierzy, o ile istnieje
    {
        Field Field = _Field.GetComponent<Field>();
        for (int i = Field.FieldSoldierSize.Count - 1; i >= 0; i--)
        {
            Field ArmyMateField = Field.FieldSoldierSize[i].GetComponentInParent<Field>();
            ArmyMateField.Soldier.GetComponent<Soldier>().SoldierSize.Clear();
            Destroy(ArmyMateField.Soldier);
            ArmyMateField.Soldier = Instantiate(ArmyMateField.SoldierPrefab);
            ArmyMateField.Soldier.transform.SetParent(ArmyMateField.transform);
            ArmyMateField.Soldier.transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    private List<GameObject> FieldNeighbors(GameObject Field) //Metoda, która zwraca listę sąsiadów wybranego pola
    {

        List<GameObject> Neighbors = new List<GameObject>();
        if (AllyArmy.Contains(Field))
        {
            if (AllyArmy.IndexOf(Field) != 0)
                Neighbors.Add(AllyArmy[(AllyArmy.IndexOf(Field) - 1)]);
            else
                Neighbors.Add(null);
            if (AllyArmy.IndexOf(Field) != (AllyArmy.Count - 1))
                Neighbors.Add(AllyArmy[(AllyArmy.IndexOf(Field) + 1)]);
            else
                Neighbors.Add(null);
        }
        else if (EnemyArmy.Contains(Field))
        {
            if (EnemyArmy.IndexOf(Field) != 0)
                Neighbors.Add(EnemyArmy[(EnemyArmy.IndexOf(Field) - 1)]);
            else
                Neighbors.Add(null);
            if (EnemyArmy.IndexOf(Field) != (EnemyArmy.Count - 1))
                Neighbors.Add(EnemyArmy[(EnemyArmy.IndexOf(Field) + 1)]);
            else
                Neighbors.Add(null);
        }
        return Neighbors;
    }
    //KONIEC SKRYPTU
    //MOWA KOŃCOWA...
    //Chciałem spróbować stworzyć algorytm, gdzie żołnierz mógłby być większy niż 2, ale ostatecznie go nie dokończyłem ;'(
    private void AddSizedUnitUNFINISHED(GameObject Field, List<GameObject> Army, int armySize)
    {
        GameObject CurrentField = null; //Obecne pole, na które chcemy dodać żołnierza
        int soldierSizeCount;
        for (int i = 0; i < armySize - 1; i++)
        {
            if (i == 0)
            {
                CurrentField = Army[Army.IndexOf(Field)];
                soldierSizeCount = 0;
            }
            else
            {
                soldierSizeCount = i + 1;
            }
            int fieldIndex = Army.IndexOf(CurrentField);   //Indeks obecnego pola w wybranej armii
            GameObject FieldSoldier = Army[fieldIndex].GetComponent<Field>().Soldier; //Żołnierz obecnego pola
            GameObject LeftFieldSoldier = null;            //Żołnierz lewego pola, co do obecnego
            GameObject RightFieldSoldier = null;           //Żołnierz prawego pola, co do obecnego
            if (fieldIndex != 0)
            {
                LeftFieldSoldier = Army[fieldIndex - 1].GetComponent<Field>().Soldier;
            }
            if (fieldIndex != (Army.Count - 1))
            {
                RightFieldSoldier = Army[fieldIndex + 1].GetComponent<Field>().Soldier;
            }
            List<GameObject> FieldSoldierSize = FieldSoldier.GetComponent<Soldier>().SoldierSize;
            if ((FieldSoldierSize.Count == 0 || FieldSoldierSize.Count == soldierSizeCount)                      //Jeżeli nie ma żołnierza na tym polu lub jest on rozmiaru obecnie tworzonego żołnierza,
                && RightFieldSoldier != null && RightFieldSoldier.GetComponent<Soldier>().SoldierSize.Count == 0)//oraz nie ma żołnierza na prawym polu, dodaj żołnierza
            {
                if (!FieldSoldierSize.Contains(FieldSoldier))
                {
                    FieldSoldierSize.Add(FieldSoldier);
                }
                if (!FieldSoldierSize.Contains(RightFieldSoldier))
                {
                    FieldSoldierSize.Add(RightFieldSoldier);
                }
                foreach (GameObject MateSoldier in FieldSoldierSize)
                {
                    MateSoldier.GetComponent<Soldier>().SoldierSize = new List<GameObject>(FieldSoldierSize);
                }
                CurrentField = Army[(Army.IndexOf(CurrentField) + 1)];
            }
            else if ((FieldSoldierSize.Count == 0 || FieldSoldierSize.Count == soldierSizeCount) //Analogicznie do pierwszego
                && LeftFieldSoldier != null && LeftFieldSoldier.GetComponent<Soldier>().SoldierSize.Count == 0)
            {
                if (!FieldSoldierSize.Contains(LeftFieldSoldier))
                    FieldSoldierSize.Add(LeftFieldSoldier);
                if (!FieldSoldierSize.Contains(FieldSoldier))
                    FieldSoldierSize.Add(FieldSoldier);
                foreach (GameObject MateSoldier in FieldSoldierSize)
                    MateSoldier.GetComponent<Soldier>().SoldierSize = new List<GameObject>(FieldSoldierSize);
                CurrentField = Army[(Army.IndexOf(CurrentField) - 1)];
            }
        }
        if (Field.GetComponent<Field>().Soldier.GetComponent<Soldier>().SoldierSize.Count != armySize) //Jeżeli powstały żołnierz nie jest takiego rozmiaru jaki ma ostatecznie powstać,
        {                                                                                              //oznacza to, że w pewnym momencie nie mógł się powiększyć, więc go usuwa
            Debug.LogWarning("Nie można utworzyć na tym polu " + armySize + "-polowego żołnierza!");
        }
    }
}
