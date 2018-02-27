using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class RPGCharacterGUI : MonoBehaviour
{
    RPGCharacterControllerFREE rpgCharacter;
    bool useNav;
    bool navToggle;

    void Start()
    {
        rpgCharacter = GetComponent<RPGCharacterControllerFREE>();
    }

    void OnGUI()
    {
        if (!rpgCharacter.isDead)
        {
            //Character NavMesh navigation
            useNav = GUI.Toggle(new Rect(25, 200, 100, 30), useNav, "Use NavMesh");
            if (useNav)
            {
                if (navToggle == false)
                {
                    rpgCharacter.useNavMesh = true;
                    navToggle = true;
                }
            }
            else
            {
                rpgCharacter.useNavMesh = false;
                navToggle = false;
            }
        }
    }
}