using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Score
{
    public string initials;
    public int score;

    public Score(string initials, int score)
    {
        this.initials = initials;
        this.score = score;
    }
}
