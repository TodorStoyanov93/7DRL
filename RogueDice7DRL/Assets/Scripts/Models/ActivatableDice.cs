using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivatableDice
{
    public bool isActive;
    public DiceData diceDataRef;
    public SideData chosenSide;
    public SideOfDice chosenSideOfDice;

    private ActivatableDice() { 
        
    }


    public static ActivatableDice CreateActivatableDice(bool isActive, DiceData diceData)
    {
        ActivatableDice result = new ActivatableDice();

        result.diceDataRef = diceData;
        result.isActive = isActive;
        result.ChooseRandomSide();

        return result;
    }

    private void ChooseRandomSide() {

        int i = Random.Range(0,5);


        SideData[] sideDatas = new[] {
            diceDataRef.sideTop,
            diceDataRef.sideBottom,
            diceDataRef.sideLeft,
            diceDataRef.sideRight,
            diceDataRef.sideFront,
            diceDataRef.sideBack
        };

        var result = sideDatas[i];

        this.chosenSide = result;

        switch (i) {
            case 0:
                this.chosenSideOfDice = SideOfDice.Top;
                break;
            case 1:
                this.chosenSideOfDice = SideOfDice.Bottom;
                break;
            case 2:
                this.chosenSideOfDice = SideOfDice.Left;
                break;
            case 3:
                this.chosenSideOfDice = SideOfDice.Right;
                break;
            case 4:
                this.chosenSideOfDice = SideOfDice.Front;
                break;
            case 5:
                this.chosenSideOfDice = SideOfDice.Back;
                break;

        }

        
    }
}
