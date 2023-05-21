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
    public Material disabledMaterial;

    [HideInInspector]
    public bool towerIsHighlighted = false;

    [HideInInspector]
    public bool isActive = true;

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

    public void SetInactive()
    {
        this.GetComponentInChildren<MeshRenderer>().material = disabledMaterial;
        isActive = false;
    }

    public void SetActive()
    {
        if (!isActive)
        {
            this.GetComponentInChildren<MeshRenderer>().material = defaultMaterial;
            isActive = true;
        }
    }

    public override string ToString()
    {
        return gameObject.name.Replace("(Clone)", "");
    }
}
