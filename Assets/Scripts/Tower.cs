using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int goldCost = 5;
    public int upgradesBought = 0;
    public int upgradeCost = 2;
    public int upgradeCostForRateOfFire = 10;

    [Range(0f, 1f)]
    public float refundFactor = .5f;
}
