using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static int points = 0;

    public static int Points
    {
        get => points;
        set
        {
            points = value;
        }
    }

    public void AddPoints(int pointsPerTarget)
    {
        points += pointsPerTarget;
    }
}
