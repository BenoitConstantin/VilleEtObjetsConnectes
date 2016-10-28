using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerDistrib : MonoBehaviour {
    [SerializeField]
    Text[] equipeRose;
    [SerializeField]
    Text[] equipeNoire;
    [SerializeField]
    Image[] teamcolors;
    [SerializeField]
    TeamScriptableObject TeamColor;

    void Awake()
    {
        for (int j=0; j<teamcolors.Length;j++)
        {
            teamcolors[j].color = TeamColor.teamInfos[j].color;
        }
    }

	void Update()
    {
        for (int k=0; k<equipeRose.Length;k++)
            {
            equipeRose[k].text = "";
            }
        for (int k = 0; k <equipeNoire.Length; k++)
        {
            equipeNoire[k].text = "";
        }
        int cpt1 = 0;
        int cpt2 = 0;

        for (int i = 0; i <GameManager.Instance.Players.Length; i++)
        {
            if (GameManager.Instance.Players[i].TeamId == 1)
            {
                equipeRose[cpt1].text = GameManager.Instance.Players[i].PlayerName;
                cpt1++;
            }
            if (GameManager.Instance.Players[i].TeamId == 0)
            {
                equipeNoire[cpt2].text = GameManager.Instance.Players[i].PlayerName;
                cpt2++;
            }
        }
    }
}
