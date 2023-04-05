using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System;
using System.Linq;

public class Player : MonoBehaviour
{
    private enum Mode
    {
        Build,
        Play
    }

    public enum Difficulty
    {
        Easy = 0,
        Normal = 2,
        Hard = 5
    }

    private Mode mode = Mode.Build;

    private int DifficultyFactor = 0;

    private List<Enemy> enemies = new List<Enemy>();

    [Header("References")]
    public Transform trans;
    public Transform spawnPoint;
    public Transform leakPoint;

    [Tooltip("Reference to the sell button lock panel GameObject.")]
    public GameObject sellButtonLockPanel;

    [Header("X Bounds")]
    public float minimumX = -70;
    public float maximumX = 70;

    [Header("Y Bounds")]
    public float minimumY = 18;
    public float maximumY = 80;

    [Header("Z Bounds")]
    public float minimumZ = -130;
    public float maximumZ = 70;

    [Header("Movement")]
    [Tooltip("Distance traveled per second with the arrow keys.")]
    public float arrowKeySpeed = 80;

    [Tooltip(
        "Multiplier for mouse drag movement.  A higher value will result in the camera moving a greater distance when the mouse is moved."
    )]
    public float mouseDragSensitivity = 2.8f;

    [Tooltip("Amount of smoothing applied to camera movement.  Should be a value between 0 and 1.")]
    [Range(0, .99f)]
    public float movementSmoothing = .75f;

    private Vector3 targetPosition;

    [Header("Scrolling")]
    [Tooltip("Amount of Y distance the camera moves per mouse scroll increment.")]
    public float scrollSensitivity = 1.6f;

    [Header("Build Mode")]
    public Difficulty DifficultyLevel = Difficulty.Easy;

    [Tooltip(
        "Current gold for the player.  Set this to however much gold the player should start with."
    )]
    public int gold = 50;

    [Tooltip("Layer mask for highlighter raycasting.  Should include the layer of the stage.")]
    public LayerMask stageLayerMask;

    [Tooltip("Reference to the Transform of the Highlighter GameObject.")]
    public Transform highlighter;

    [Tooltip("Reference to the Tower Selling Panel.")]
    public RectTransform towerSellingPanel;
    public RectTransform towerActionsPanel;
    public RectTransform settingsPanel;
    public RectTransform gamePlayPanel;

    [Tooltip("Reference to the Tower Description Panel.")]
    public RectTransform towerDescriptionPanel;

    [Tooltip("Reference to the Tower Upgrade Panel.")]
    public RectTransform towerUpgradePanel;

    [Tooltip("Reference to the Text component of the Refund Text in the Tower Selling Panel.")]
    public Text sellRefundText;
    public Text upgradeDamageCostText;
    public Text upgradeRateOfFireCostText;
    public Text upgradeRangeCostText;
    public Text towerStatsText;

    [Tooltip(
        "Reference to the Text component of the current gold text in the bottom-left corner of the UI."
    )]
    public Text currentGoldText;
    public Text currentLevelText;
    public Text remainingLivesText;
    public Text enemiesText;

    [Tooltip("The color to apply to the selected build button.")]
    public Color selectedBuildButtonColor = new Color(.2f, .8f, .2f);

    //Mouse position at the last frame.
    private Vector3 lastMousePosition;

    //Current gold the last time we checked.
    private int goldLastFrame;

    //True if the cursor is over the stage right now, false if not.
    private bool cursorIsOverStage = false;

    //Reference to the Tower prefab selected by the build button.
    private Tower towerPrefabToBuild = null;

    //Reference to the currently selected build button Image component.
    private Image selectedBuildButtonImage = null;

    //Currently selected Tower instance, if any.
    private Tower selectedTower = null;

    private Dictionary<Vector3, Tower> towers = new Dictionary<Vector3, Tower>();

    private int livesLostThisRound = 0;
    private int livesAtStartOfLevel = 0;
    private int livesAtEndOfLevel = 0;

    //Play Mode:
    [Header("Play Mode")]
    [Tooltip("Reference to the Build Button Panel to deactivate it when play mode starts.")]
    public GameObject buildButtonPanel;

    [Tooltip("Reference to the Game Lost Panel.")]
    public GameObject gameLostPanel;

    [Tooltip("Reference to the Text component for the info text in the Game Lost Panel.")]
    public Text gameLostPanelInfoText;

    [Tooltip("Reference to the Play Button GameObject to deactivate it in play mode.")]
    public GameObject playButton;

    [Tooltip("Reference to the Enemy Holder Transform.")]
    public Transform enemyHolder;

    [Tooltip("Reference to the ground enemy prefab.")]
    public Enemy groundEnemyPrefab;
    public Enemy groundEnemyArmoredPrefab;

    public Enemy groundEnemyDoublePrefab;

    [Tooltip("Reference to the flying enemy prefab.")]
    public Enemy flyingEnemyPrefab;
    public Enemy flyingArmoredEnemyPrefab;

    [Tooltip("Time in seconds between each enemy spawning.")]
    public float enemySpawnRate = .35f;

    [Tooltip(
        "Determines how often flying enemy levels occur.  For example if this is set to 4, every 4th level is a flying level."
    )]
    public int flyingLevelInterval = 4;
    public int mixedLevelInterval = 6;

    [Tooltip("Number of enemies spawned each level.")]
    public int enemiesPerLevel = 15;

    [Tooltip("Gold given to the player at the end of each level.")]
    public int goldRewardPerLevel = 12;

    //The current level.
    public static int level = 1;

    //Number of enemies spawned so far for this level.
    private int enemiesSpawnedThisLevel = 0;

    //Player's number of remaining lives; once it hits 0, the game is over:
    public static int remainingLives = 40;

    private Color towerDefaultColor;

    private GameObject upgradeDamageButton = null;
    private GameObject upgradeSpeedButton = null;
    private GameObject upgradeRangeButton = null;

    //private List<Transform> buildButtons = new List<Transform>();

    private int numberToMakeEnemyArmored
    {
        get
        {
            if (level <= 5)
                return 2 + DifficultyFactor;
            else if (level <= 10)
                return 8 + DifficultyFactor;
            else if (level <= 15)
                return 3 + DifficultyFactor;
            else
                return 30 + DifficultyFactor;

            //return 20; //max 50. the higher the number, the more armored enemies
        }
    }

    private int numberToMakeDoubledEnemy
    {
        get
        {
            /*
            if (level <= 5)
                return 5;
            else if (level <= 10)
                return 10;
            else if (level <= 15)
                return 15;
            else
                return 30;*/

            //return 20; //max 50. the higher the number, the more armored enemies

            return 15;
        }
    }

    //Methods:
    void ArrowKeyMovement()
    {
        //If up arrow is held,
        if (Input.GetKey(KeyCode.UpArrow))
        {
            //...add to target Z position:
            targetPosition.z += arrowKeySpeed * Time.deltaTime;
        }
        //Otherwise, if down arrow is held,
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            //...subtract from target Z position:
            targetPosition.z -= arrowKeySpeed * Time.deltaTime;
        }

        //If right arrow is held,
        if (Input.GetKey(KeyCode.RightArrow))
        {
            //..add to target X position:
            targetPosition.x += arrowKeySpeed * Time.deltaTime;
        }
        //Otherwise, if left arrow is held,
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            //...subtract from target X position:
            targetPosition.x -= arrowKeySpeed * Time.deltaTime;
        }
    }

    void MouseDragMovement()
    {
        //If the right mouse button is held,
        if (Input.GetMouseButton(1))
        {
            //Get the movement amount this frame:
            Vector3 movement =
                new Vector3(-Input.GetAxis("Mouse X"), 0, -Input.GetAxis("Mouse Y"))
                * mouseDragSensitivity;

            //If there is any movement,
            if (movement != Vector3.zero)
            {
                //...apply it to the targetPosition:
                targetPosition += movement;
            }
        }
    }

    void Zooming()
    {
        //Get the scroll delta Y value and flip it:
        float scrollDelta = -Input.mouseScrollDelta.y;

        //If there was any delta,
        if (scrollDelta != 0)
        {
            //...apply it to the Y position:
            targetPosition.y += scrollDelta * scrollSensitivity;
        }
    }

    void MoveTowardsTarget()
    {
        //Clamp the target position to the bounds variables:
        targetPosition.x = Mathf.Clamp(targetPosition.x, minimumX, maximumX);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minimumY, maximumY);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minimumZ, maximumZ);

        //Move if we aren't already at the target position:
        if (trans.position != targetPosition)
        {
            trans.position = Vector3.Lerp(trans.position, targetPosition, 1 - movementSmoothing);
        }
    }

    void PositionHighlighter()
    {
        //If the mouse position this frame is different than last frame:
        if (Input.mousePosition != lastMousePosition)
        {
            //Get a ray at the mouse position, shooting out of the camera:
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit; //Information on what was hit will be stored here

            //Cast the ray and check if it hit anything, using our layer mask:
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, stageLayerMask.value))
            {
                //If it did hit something, use hit.point to get the location it hit:
                Vector3 point = hit.point;

                //Round the X and Z values to multiples of 10:
                point.x = Mathf.Round(hit.point.x * .1f) * 10;
                point.z = Mathf.Round(hit.point.z * .1f) * 10;

                //Clamp Z between -80 and 80 to prevent sticking over the edge of the stage:
                point.z = Mathf.Clamp(point.z, -80, 80);

                //Ensure Y is always 0:
                point.y = .2f;

                //Make sure the highlighter is active (visible) and set its position:
                highlighter.position = point;
                highlighter.gameObject.SetActive(true);
                cursorIsOverStage = true;
            }
            else //If the ray didn't hit anything,
            {
                //... mark cursorIsOverStage as false:
                cursorIsOverStage = false;

                //Deactivate the highlighter GameObject so it no longer shows:
                highlighter.gameObject.SetActive(false);
            }
        }

        //Make sure we keep track of the mouse position this frame:
        lastMousePosition = Input.mousePosition;
    }

    void OnStageClicked()
    {
        //Debug.Log("OnStageClicked()");


        //upgrade panel showing, don't trigger objects behind it
        if (towerActionsPanel.gameObject.activeInHierarchy)
        {
            return;
        }

        ClearHighlightedTowers();

        //If a build button is selected:
        if (towerPrefabToBuild != null)
        {
            //If there is no tower in that slot and we have enough gold to build the selected tower:
            if (!towers.ContainsKey(highlighter.position) && gold >= towerPrefabToBuild.goldCost)
            {
                BuildTower(towerPrefabToBuild, highlighter.position);
            }
            else //selected area already has a tower so guessing they want to upgrade
            {
                DeselectBuildButton();
                selectedTower = towers[highlighter.position];
                towerActionsPanel.gameObject.SetActive(true);
                UpdateTowerActionPanel();
            }
        }
        //If no build button is selected:
        else
        {
            //Check if a tower is at the current highlighter position:
            if (towers.ContainsKey(highlighter.position))
            {
                //Set the selected tower to this one:
                selectedTower = towers[highlighter.position];
                selectedTower.HighlightTower();

                towerActionsPanel.gameObject.SetActive(true);
                UpdateTowerActionPanel();

                //Make sure the sell tower UI panel is active so it shows:
                //towerSellingPanel.gameObject.SetActive(true);
                //towerUpgradePanel.gameObject.SetActive(true);
            }
        }
    }

    private void ClearHighlightedTowers()
    {
        foreach (var tower in towers.Values.Where(t => t.towerIsHighlighted))
        {
            tower.UnHighlightTower();
        }
    }

    public void OnTowerBuildButtonHover(Tower tower)
    {
        towerDescriptionPanel.gameObject.SetActive(true);

        if (tower is FiringTower)
        {
            var towerTemp = (FiringTower)tower;
            var name = GameObject.Find("Tower Name Text").GetComponent<Text>();
            var cost = GameObject.Find("Tower Cost Text").GetComponent<Text>();
            var speed = GameObject.Find("Tower Speed Text").GetComponent<Text>();
            var damage = GameObject.Find("Tower Damage Text").GetComponent<Text>();
            var range = GameObject.Find("Tower Range Text").GetComponent<Text>();
            var targets = GameObject.Find("Tower Targets Text").GetComponent<Text>();
            var description = GameObject.Find("Tower Description Text").GetComponent<Text>();

            name.text = tower.name;
            cost.text = $"Cost: {towerTemp.goldCost.ToString()}";
            speed.text = $"Speed: {100 - (towerTemp.fireInterval * 100)}";
            damage.text = $"Damage: {towerTemp.damage}";
            range.text = $"Range: {towerTemp.range}";

            Debug.Log(tower.name);

            String targetsStr = "Targets: ";
            if (towerTemp.canAttackGround)
            {
                if (towerTemp.canAttackFlying)
                    targetsStr += "Ground, Aerial";
                else
                    targetsStr += "Ground";
            }
            else
            {
                if (towerTemp.canAttackFlying)
                    targetsStr += "Aerial";
            }

            switch (tower.name)
            {
                case "Arrow Tower":
                    description.text =
                        "Shoots a single arrow. Can upgrade damage, speed, and range.";
                    targets.text = targetsStr;
                    break;
                case "Arrow Tower Double Barrel":
                    description.text = "Shoots two arrows. Can upgrade damage, speed, and range.";
                    targets.text = targetsStr;
                    break;
                case "Cannon Tower":
                    description.text =
                        "Shoots a cannon ball dealing splash damage. Can upgrade damage, speed, and range.";
                    targets.text = targetsStr;
                    break;

                case "Machine Gun Tower":
                    description.text =
                        "Automatic weapon of small calibre that is capable of sustained rapid fire. Can upgrade damage, speed, and range.";
                    targets.text = targetsStr;
                    break;

                case "Anti Air Tower":
                    description.text =
                        "Automatic weapon of small calibre that is capable of sustained rapid fire. Weak until upgraded. Can upgrade damage, speed, and range.";
                    targets.text = targetsStr;
                    break;
            }
        }
        else if (tower is TargetingTower)
        {
            SetTargetingTowerDescription(tower);
        }
    }

    private void SetTargetingTowerDescription(Tower tower)
    {
        var towerTemp = (TargetingTower)tower;
        var name = GameObject.Find("Tower Name Text").GetComponent<Text>();
        var cost = GameObject.Find("Tower Cost Text").GetComponent<Text>();
        var speed = GameObject.Find("Tower Speed Text").GetComponent<Text>();
        var damage = GameObject.Find("Tower Damage Text").GetComponent<Text>();
        var range = GameObject.Find("Tower Range Text").GetComponent<Text>();
        var targets = GameObject.Find("Tower Targets Text").GetComponent<Text>();
        var description = GameObject.Find("Tower Description Text").GetComponent<Text>();

        name.text = tower.name;
        cost.text = $"Cost: {towerTemp.goldCost.ToString()}";
        // speed.text = $"Speed: {100 - (towerTemp.fireInterval * 100)}";
        //damage.text = $"Damage: {towerTemp.damage}";
        range.text = $"Range: {towerTemp.range}";

        switch (tower.name)
        {
            case "Hot Plate":
                break;
            case "Slow Plate":
                description.text =
                    "A passable pad that slows down nearby enemies. Can upgrade slow down rate.";
                targets.text = "Targets: Ground, Aerial";
                break;
            case "Bomb Plate":
                description.text =
                    @"A passable bomb that explodes 1 second after the first enemy has entered its range. Can upgrade damage.";
                targets.text = "Targets: Ground, Aerial";
                break;
        }
    }

    public void OnTowerBuildButtonExit()
    {
        towerDescriptionPanel.gameObject.SetActive(false);
    }

    void UpdateTowerActionPanel()
    {
        //Debug.Log("UpdateTowerActionPanel");
        // Debug.Log(selectedTower);

        if (selectedTower == null)
            return;

        if (upgradeDamageButton == null)
        {
            upgradeDamageButton = GameObject.Find("Upgrade Damage Button");
            upgradeSpeedButton = GameObject.Find("Upgrade RateOfFire Button");
            upgradeRangeButton = GameObject.Find("Upgrade Range Button");
        }

        sellRefundText.text =
            "Sell for "
            + Mathf.CeilToInt(
                (selectedTower.goldCost + selectedTower.upgradesBought) * selectedTower.refundFactor
            )
            + " gold";

        upgradeDamageCostText.text = "+ Damage: " + selectedTower.upgradeCost.ToString() + " gold";
        upgradeRateOfFireCostText.text =
            $"+ Speed: {selectedTower.upgradeCostForRateOfFire.ToString()} gold";
        upgradeRangeCostText.text = $"+ Range: {selectedTower.upgradeCost.ToString()} gold";

        if (selectedTower is FiringTower)
        {
            var towerTemp = (FiringTower)selectedTower;

            //if(upgradeDamageBtn != null && towerTemp.damage >= 100)
            //{
            // upgradeDamageBtn.SetActive(false);
            //upgradeDamageBtn.clickable = false;
            //}


            towerStatsText.text =
                $"Upgrades $: {selectedTower.upgradesBought} {Environment.NewLine}Dmg: {towerTemp.damage} {Environment.NewLine}Rate of fire: {towerTemp.fireInterval} {Environment.NewLine}Range: {towerTemp.range}";
        }
        else if (selectedTower is TargetingTower)
        {
            var towerTemp = (TargetingTower)selectedTower;

            if (towerTemp.name.ToString().Contains("Bomb Plate"))
            {
                towerStatsText.text =
                    $"Dmg: {towerTemp.damagePerSecond}{Environment.NewLine}Range: {towerTemp.range}:";
            }
            else
            {
                towerStatsText.text =
                    $"Dmg/s: {towerTemp.damagePerSecond}{Environment.NewLine}Speed Reduction: {towerTemp.slowDownRate}:";
            }
        }

        if (!selectedTower.canUpgradeDamage)
            upgradeDamageButton.SetActive(false);
        else
            upgradeDamageButton.SetActive(true);

        if (!selectedTower.canUpgradeSpeed)
            upgradeSpeedButton.SetActive(false);
        else
            upgradeSpeedButton.SetActive(true);

        if (!selectedTower.canUpgradeRange)
            upgradeRangeButton.SetActive(false);
        else
            upgradeRangeButton.SetActive(true);
    }

    void BuildTower(Tower prefab, Vector3 position)
    {
        //Instantiate the tower at the given location and place it in the Dictionary:
        towers[position] = Instantiate<Tower>(prefab, position, Quaternion.identity);

        //Decrease player gold:
        gold -= towerPrefabToBuild.goldCost;

        //Update the path through the maze:
        UpdateEnemyPath();
    }

    void SellTower(Tower tower)
    {
        //Since it's not going to exist in a bit, deselect the tower:
        DeselectTower();

        //Refund the player:
        gold += Mathf.CeilToInt((tower.goldCost + tower.upgradesBought) * tower.refundFactor);

        //Remove the tower from the dictionary using its position:
        towers.Remove(tower.transform.position);

        //Destroy the tower GameObject:
        Destroy(tower.gameObject);

        //Refresh pathfinding:
        UpdateEnemyPath();
    }

    public void OnSellTowerButtonClicked()
    {
        //If there is a selected tower,
        if (selectedTower != null)
            //Sell it:
            SellTower(selectedTower);
    }

    public void OnUpgradeTowerDamageButtonClicked()
    {
        if (selectedTower != null)
        {
            if (gold - Mathf.CeilToInt(selectedTower.upgradeCost) < 0)
                return;

            selectedTower.upgradesBought += selectedTower.upgradeCost;
            if (selectedTower is FiringTower)
            {
                FiringTower towerTemp = (FiringTower)selectedTower;

                if (towerTemp.damage >= 100)
                    return;

                towerTemp.damage += 1;
                gold -= Mathf.CeilToInt(towerTemp.upgradeCost);
            }
            else if (selectedTower is HotPlate)
            {
                HotPlate towerTemp = (HotPlate)selectedTower;
                towerTemp.damagePerSecond += 1; // Mathf.CeilToInt(towerTemp.damagePerSecond / 4);
                gold -= Mathf.CeilToInt(towerTemp.upgradeCost);
            }
            else if (selectedTower is Bomb)
            {
                Bomb towerTemp = (Bomb)selectedTower;
                towerTemp.damagePerSecond += 1; // Mathf.CeilToInt(towerTemp.damagePerSecond / 4);
                gold -= Mathf.CeilToInt(towerTemp.upgradeCost);
            }
        }

        UpdateTowerActionPanel();
    }

    public void OnUpgradeTowerRateOfFireButtonClicked()
    {
        if (selectedTower != null)
        {
            if (gold - Mathf.CeilToInt(selectedTower.upgradeCostForRateOfFire) < 0)
                return;

            selectedTower.upgradesBought += selectedTower.upgradeCostForRateOfFire;
            if (selectedTower is FiringTower)
            {
                FiringTower towerTemp = (FiringTower)selectedTower;

                if (towerTemp.fireInterval < .06f)
                    return;

                // Debug.Log("Tower Damage: " + towerTemp.damage);
                towerTemp.fireInterval -= .05f;
                gold -= Mathf.CeilToInt(towerTemp.upgradeCostForRateOfFire);
            }
            else if (selectedTower is HotPlate)
            {
                HotPlate towerTemp = (HotPlate)selectedTower;
                // Debug.Log("Hot Plate Damage: " + towerTemp.damagePerSecond);
                towerTemp.damagePerSecond += (towerTemp.damagePerSecond / 4);
                gold -= Mathf.CeilToInt(towerTemp.upgradeCostForRateOfFire);
            }
        }

        UpdateTowerActionPanel();
    }

    public void OnUpgradeTowerRangeButtonClicked()
    {
        if (selectedTower != null)
        {
            if (gold - Mathf.CeilToInt(selectedTower.upgradeCost) < 0)
                return;

            selectedTower.upgradesBought += selectedTower.upgradeCost;
            if (selectedTower is TargetingTower)
            {
                TargetingTower towerTemp = (TargetingTower)selectedTower;
                towerTemp.range += 2;
                towerTemp.targeter.SetRange(towerTemp.range);
                gold -= Mathf.CeilToInt(towerTemp.upgradeCost);
            }
        }

        UpdateTowerActionPanel();
    }

    void PositionSellPanel()
    {
        //If there is a selected tower:
        if (selectedTower != null)
        {
            //Convert tower world position, moved forward by 8 units, to screen space:
            var screenPosition = Camera.main.WorldToScreenPoint(
                selectedTower.transform.position + Vector3.forward * 30
            );

            var screenPosition2 = Camera.main.WorldToScreenPoint(
                selectedTower.transform.position + Vector3.right * 40 + Vector3.forward * 8
            );

            //Apply the position to the tower selling panel:
            //towerSellingPanel.position = screenPosition;
            //towerUpgradePanel.position = screenPosition2;
            towerActionsPanel.position = screenPosition;
        }
    }

    void UpdateCurrentGold()
    {
        //If the gold has changed since last frame:
        if (gold != goldLastFrame)
            //Update the text to match:
            currentGoldText.text = gold + " gold";

        //Keep track of the gold value each frame:
        goldLastFrame = gold;
    }

    public void DeselectTower()
    {
        ClearHighlightedTowers();
        //Null selected tower and hide the sell tower panel:
        selectedTower = null;
        //towerSellingPanel.gameObject.SetActive(false);
        //towerUpgradePanel.gameObject.SetActive(false);
        towerActionsPanel.gameObject.SetActive(false);
    }

    void DeselectBuildButton()
    {
        //Null the tower prefab to build, if there is one:
        towerPrefabToBuild = null;

        //Reset the color of the selected build button, if there is one:
        if (selectedBuildButtonImage != null)
        {
            selectedBuildButtonImage.color = Color.white;
            selectedBuildButtonImage = null;
        }
    }

    void UpdateEnemyPath()
    {
        Invoke("PerformPathfinding", .1f);
    }

    void BuildModeLogic()
    {
        PositionHighlighter();

        PositionSellPanel();

        UpdateCurrentGold();

        //If the left mouse button is clicked while the cursor is over the stage:
        if (cursorIsOverStage && Input.GetMouseButtonDown(0))
        {
            OnStageClicked();
        }

        //If Escape is pressed:
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeselectTower();

            DeselectBuildButton();
        }
    }

    void DeselectButtons()
    {
        var buildButtons = new List<GameObject>();
        buildButtons.Add(GameObject.Find("Build Button"));
        buildButtons.Add(GameObject.Find("Build Button (1)"));
        buildButtons.Add(GameObject.Find("Build Button (2)"));
        buildButtons.Add(GameObject.Find("Build Button (3)"));
        buildButtons.Add(GameObject.Find("Build Button (4)"));
        buildButtons.Add(GameObject.Find("Build Button (5)"));
        buildButtons.Add(GameObject.Find("Build Button (6)"));
        buildButtons.Add(GameObject.Find("Build Button (7)"));
        buildButtons.Add(GameObject.Find("Build Button (8)"));
        buildButtons.Add(GameObject.Find("Build Button (9)"));

        for (int i = 0; i < buildButtons.Count; i++)
        {
            Image selectedBuildButtonImageTemp = buildButtons[i].GetComponent<Image>();

            selectedBuildButtonImageTemp.color = Color.white;
        }
    }

    public void OnBuildButtonClicked(Tower associatedTower)
    {
        //Set the prefab to build:
        towerPrefabToBuild = associatedTower;
        //Clear selected tower (if any):
        DeselectTower();
        DeselectButtons();
    }

    public void SetSelectedBuildButton(Image clickedButtonImage)
    {
        //Keep a reference to the Button that was clicked:
        selectedBuildButtonImage = clickedButtonImage;

        //Set the color of the clicked button:
        clickedButtonImage.color = selectedBuildButtonColor;
    }

    void PerformPathfinding()
    {
        //Pathfind from spawn point to leak point, storing the result in GroundEnemy.path:
        NavMesh.CalculatePath(
            spawnPoint.position,
            leakPoint.position,
            NavMesh.AllAreas,
            GroundEnemy.path
        );

        if (GroundEnemy.path.status == NavMeshPathStatus.PathComplete)
        {
            //If the path was successfully found, make sure the lock panel is inactive:
            sellButtonLockPanel.SetActive(false);
        }
        else //If the path is blocked,
        {
            //Activate the lock panel:
            sellButtonLockPanel.SetActive(true);
        }
    }

    void SpawnEnemy()
    {
        Enemy enemy = null;
        System.Random rnd = new System.Random();
        var randomNum = rnd.Next(1, 50);
        var randomNumForDoubleEmeny = rnd.Next(1, 50);

        //If this is a flying level
        if (level % flyingLevelInterval == 0)
        {
            if (randomNum > numberToMakeEnemyArmored)
            {
                enemy = Instantiate(
                    flyingEnemyPrefab,
                    spawnPoint.position + (Vector3.up * 18),
                    Quaternion.LookRotation(Vector3.back)
                );
            }
            else
            {
                enemy = Instantiate(
                    flyingArmoredEnemyPrefab,
                    spawnPoint.position + (Vector3.up * 18),
                    Quaternion.LookRotation(Vector3.back)
                );
            }

            enemies.Add(enemy);
        }
        else if (level % mixedLevelInterval == 0)
        {
            if (randomNum >= 25)
            {
                enemy = Instantiate(
                    flyingEnemyPrefab,
                    spawnPoint.position + (Vector3.up * 18),
                    Quaternion.LookRotation(Vector3.back)
                );
            }
            else
            {
                enemy = Instantiate(
                    groundEnemyPrefab,
                    spawnPoint.position,
                    Quaternion.LookRotation(Vector3.back)
                );
            }
        }
        else //If it's a ground level
        {
            if (randomNum > numberToMakeEnemyArmored)
            {
                //if (randomNumForDoubleEmeny > numberToMakeDoubledEnemy)
                //{
                enemy = Instantiate(
                    groundEnemyPrefab,
                    spawnPoint.position,
                    Quaternion.LookRotation(Vector3.back)
                );
                //}
                //else
                //{
                // enemy = Instantiate(
                //   groundEnemyDoublePrefab,
                //  spawnPoint.position,
                //  Quaternion.LookRotation(Vector3.back)
                //);
                //}
            }
            else
            {
                if (randomNumForDoubleEmeny > numberToMakeDoubledEnemy)
                {
                    enemy = Instantiate(
                        groundEnemyArmoredPrefab,
                        spawnPoint.position,
                        Quaternion.LookRotation(Vector3.back)
                    );
                }
                else
                {
                    enemy = Instantiate(
                        groundEnemyDoublePrefab,
                        spawnPoint.position,
                        Quaternion.LookRotation(Vector3.back)
                    );
                }
            }

            enemies.Add(enemy);
        }

        //Parent enemy to the enemy holder:
        enemy.trans.SetParent(enemyHolder);

        //Count that we spawned the enemy:
        enemiesSpawnedThisLevel += 1;

        //Stop invoking if we've spawned all enemies:
        if (enemiesSpawnedThisLevel >= enemiesPerLevel)
        {
            CancelInvoke("SpawnEnemy");
        }
    }

    public void PlayModeLogic()
    {
        //If no enemies are left and all enemies have already spawned
        if (enemyHolder.childCount == 0 && enemiesSpawnedThisLevel >= enemiesPerLevel)
        {
            enemiesText.text = $"Enemies: 0/{enemiesPerLevel}";

            //Return to build mode if we haven't lost yet:
            if (remainingLives > 0)
                GoToBuildMode();
            else
            {
                //Update game lost panel text with information:
                gameLostPanelInfoText.text =
                    "You had "
                    + remainingLives
                    + " lives by the end and made it to level "
                    + level
                    + ".";

                //Activate the game lost panel:
                gameLostPanel.SetActive(true);
            }
        }

        if (enemiesSpawnedThisLevel > 0)
        {
            enemiesText.text = $"Enemies: {enemyHolder.childCount}/{enemiesPerLevel}";
        }
    }

    void GoToPlayMode()
    {
        DeselectTower();
        DeselectBuildButton();

        livesAtStartOfLevel = remainingLives;
        mode = Mode.Play;

        //Deactivate build button panel and play button:
        buildButtonPanel.SetActive(false);
        playButton.SetActive(false);

        //Deactivate highlighter:
        highlighter.gameObject.SetActive(false);
        ClearHighlightedTowers();
    }

    void GoToBuildMode()
    {
        livesAtEndOfLevel = remainingLives;
        livesLostThisRound = livesAtStartOfLevel - livesAtEndOfLevel;

        if (livesLostThisRound < 2)
        {
            goldRewardPerLevel += 1;
        }

        mode = Mode.Build;

        //Activate build button panel and play button:
        buildButtonPanel.SetActive(true);
        playButton.SetActive(true);

        //Reset enemies spawned:
        enemiesSpawnedThisLevel = 0;

        //Increase level:
        level += 1;
        gold += goldRewardPerLevel;

        if (level / 5 == 1)
        {
            enemiesPerLevel += 5;
            gold += 5;
        }

        SetAvailableBuildButtons();
        ClearSelfDestructingTowers();
    }

    void ClearSelfDestructingTowers()
    {
        foreach (var i in towers.Where(d => d.Value.ToString().Contains("Bomb Plate")).ToList())
        {
            towers.Remove(i.Key);
            Destroy(i.Value.gameObject);
        }
    }

    void SetAvailableBuildButtons()
    {
        if (!buildButtonPanel.activeInHierarchy)
        {
            return;
        }

        //Debug.Log("SetAvailableBuildButtons()");
        var doubleArrowBuildBtn = GameObject.Find("Build Button (5)").GetComponent<Button>();
        var cannonBuildBtn = GameObject.Find("Build Button (1)").GetComponent<Button>();
        var slowPlateBuildBtn = GameObject.Find("Build Button (6)").GetComponent<Button>();
        var machineGuneBuildBtn = GameObject.Find("Build Button (7)").GetComponent<Button>();
        var laserBuildBtn = GameObject.Find("Build Button (4)").GetComponent<Button>();
        var bombBuildBtn = GameObject.Find("Build Button (8)").GetComponent<Button>();
        var aaBuildBtn = GameObject.Find("Build Button (9)").GetComponent<Button>();

        if (level < 3)
        {
            doubleArrowBuildBtn.interactable = false;
            bombBuildBtn.interactable = false;
        }
        else
        {
            doubleArrowBuildBtn.interactable = true;
            bombBuildBtn.interactable = true;
        }

        if (level < 5)
        {
            cannonBuildBtn.interactable = false;
            slowPlateBuildBtn.interactable = false;
            aaBuildBtn.interactable = false;
        }
        else
        {
            cannonBuildBtn.interactable = true;
            slowPlateBuildBtn.interactable = true;
            aaBuildBtn.interactable = true;
        }

        if (level < 10)
        {
            machineGuneBuildBtn.interactable = false;
            laserBuildBtn.interactable = false;
        }
        else
        {
            machineGuneBuildBtn.interactable = true;
            laserBuildBtn.interactable = true;
        }

        //Debug.Log(machineGuneBuildBtn.name);
    }

    public void SpeedUp()
    {
        //if (Time.timeScale < 1)
        //{
        Time.timeScale += .2f;
        //}
    }

    public void SetDefaultGameSpeed()
    {
        Time.timeScale = 1;
    }

    public void SpeedDown()
    {
        if (Time.timeScale > .3f)
        {
            Time.timeScale -= .2f;
        }
    }

    public void StartLevel()
    {
        //Time.timeScale = .2f;

        livesAtStartOfLevel = 0;
        livesAtEndOfLevel = 0;
        //Switch to play mode:
        GoToPlayMode();

        //Repeatedly invoke SpawnEnemy:
        InvokeRepeating("SpawnEnemy", .5f, enemySpawnRate);
    }

    private void SetDifficultySettings()
    {
        gold -= DifficultyFactor;
        enemiesPerLevel += DifficultyFactor;
        goldRewardPerLevel -= DifficultyFactor;
        remainingLives -= DifficultyFactor;
    }

    public void OnExchangeLivesForGold()
    {
        if (remainingLives > 5)
        {
            gold += 5;
            remainingLives -= 5;
        }
    }

    //Unity events:
    void Start()
    {
        buildButtonPanel.SetActive(false);

        targetPosition = trans.position;
        GroundEnemy.path = new NavMeshPath();
        UpdateEnemyPath();
    }

    void OnGUI()
    {
        // GUI.Label(new Rect(300, 100, 100, 20), "Hello World!");

        //Debug.Log("enemies: " + enemies.Count);
        UpdateEmemyHealthLabel();
    }

    void UpdateEmemyHealthLabel()
    {
        List<int> enemyIndexesToRemove = new List<int>();
        if (enemies.Count > 0)
        {
            int spaceIndex = 0;
            for (int i = 0; i < enemies.Count; i++)
            {
                var enemy = enemies[i];
                //Debug.Log(i + " - " + enemy.trans.position.y);
                //Debug.Log(enemy.name);
                if (enemy.trans != null)
                {
                    GUI.Label(new Rect(100, 10 + spaceIndex, 100, 20), enemy.health.ToString());
                }
                else
                {
                    enemyIndexesToRemove.Add(i);
                }
                spaceIndex += 20;
            }
        }

        for (int i = 0; i < enemyIndexesToRemove.Count; i++)
        {
            enemies.RemoveAt(enemyIndexesToRemove[i]);
        }
    }

    void Update()
    {
        ArrowKeyMovement();

        MouseDragMovement();

        Zooming();

        MoveTowardsTarget();

        if (!remainingLivesText.text.ToString().Contains(remainingLives.ToString()))
        {
            remainingLivesText.text = $"Lives: {remainingLives}";
        }

        //Run build mode logic if we're in build mode:
        if (mode == Mode.Build)
        {
            BuildModeLogic();
            currentLevelText.text = $"Level: {level}";
        }
        else
            PlayModeLogic();
    }

    public void OnSetDifficulty(int difficulty)
    {
        DifficultyFactor = difficulty;
        SetDifficultySettings();
        settingsPanel.gameObject.SetActive(false);
        gamePlayPanel.gameObject.SetActive(true);
        buildButtonPanel.SetActive(true);
        SetAvailableBuildButtons();
    }
}
