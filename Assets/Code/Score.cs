using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour {

    public Lamp[] Lamps;

    private int record;

    private int value;
    public int Value
    {
        get { return this.value; }
        set
        {
            if (value > this.record)
            {
                this.record = value;
                int id = GameStatic.id;
                string path = PlayerPrefs.GetString(id.ToString());

                string[] pars = path.Split('&');
                string pUnlock = pars[0];
                string pPaws = pars[2];

                PlayerPrefs.SetString(id.ToString(), pUnlock + "&" + this.record.ToString() + "&" + pPaws);
            }
            this.value = value;
        }
    }

    private int[] countTable;

    /// <summary>
    /// Количество активных цветов
    /// </summary>
    public int Range
    {
        get
        {
            for (var i = 1; i < countTable.Length; i++)
                if (this.value < countTable[i])
                    return i;
            return 2;
        }
    }

    private void Awake()
    {
        this.countTable = GameStatic.typesCountTable;
        this.record = GameStatic.scoreCount;
    }

    public void SetScore(int score)
    {
        this.Value = score;

        string s = score.ToString();
        string zeros = "";

        for (var i = 0; i < 4 - s.Length; i++)
        {
            zeros += "0";
        }

        s = zeros + s;

        for (var i = 0; i < 4; i++)
        {
            Lamps[i].Set((int)Char.GetNumericValue(s[i]));
        }
    }
}
