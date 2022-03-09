using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    public List<DiceData> diceList;

    public static DiceManager Instance;
    private void Awake()
    {
        Instance = this;
        if (diceList.Count == 0) {
            Debug.Log("No dice in list");
        }
    }

    public DiceData GetRandomDiceFromList() {
        int randomInt = Random.Range(0,diceList.Count-1);
        DiceData dice = diceList[randomInt];
        return dice;
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
