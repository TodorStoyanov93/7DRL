using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableDice
{
    public bool isActive;
    public DiceData diceDataRef;
    public SideData chosenSide;

    private ActivatableDice() { 
        
    }


    public static ActivatableDice CreateActivatableDice(bool isActive, DiceData diceData)
    {
        ActivatableDice result = new ActivatableDice();

        result.diceDataRef = diceData;
        result.isActive = isActive;
        result.chosenSide =  result.ChooseRandomSide();

        return result;
    }

    private SideData ChooseRandomSide() {

        int i = Random.Range(0,5);


        SideData[] sideDatas = new[] {
            diceDataRef.sideTop,
            diceDataRef.sideBottom,
            diceDataRef.sideFront,
            diceDataRef.sideBack,
            diceDataRef.sideLeft,
            diceDataRef.sideBack
        };

        var result = sideDatas[i];

        return result;
    }


}
