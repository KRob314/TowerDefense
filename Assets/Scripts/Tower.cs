using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public int goldCost = 5;
    public int upgradesBought = 0;
    public int upgradeCost = 2;
    public int upgradeCostForRateOfFire = 10;

    public bool canUpgradeDamage = true;
    public bool canUpgradeSpeed = true;
    public bool canUpgradeRange = true;
    public Material defaultMaterial;
    public Material highlightMaterial;

    [HideInInspector]
    public bool towerIsHighlighted = false;

    //= Resources.Load("Enemy", typeof(Material)) as Material;

    [Range(0f, 1f)]
    public float refundFactor = .5f;

    public void HighlightTower()
    {
        this.GetComponentInChildren<MeshRenderer>().material = highlightMaterial;
        towerIsHighlighted = true;
    }

    public void UnHighlightTower()
    {
        this.GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
        towerIsHighlighted = false;
    }
}
