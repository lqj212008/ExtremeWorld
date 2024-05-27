using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICharacterView : MonoBehaviour {

    public GameObject[] Characters;

    private int currentCharacter = 0;

    public int CurractCharacter
    {
        get
        {
            return currentCharacter;
        }
        set
        {
            currentCharacter = value;
            this.UpdateCharacter();
        }
    }

    void UpdateCharacter()
    {
        for(int i=0; i<3; i++)
        {
            Characters[i].SetActive(i == currentCharacter);
        }
    }

}
