using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TestZombieAnimations : MonoBehaviour
{
    #region Public Variables
    public List<AnimationState> zombieStates = new List<AnimationState>();
    public List<GameObject> tileCollection = new List<GameObject>();
    public List<string> zombieOptions = new List<string>();
    public Material[] tileMaterialCollection;
    public GameObject[] signCollection;
    public Material[] tileCornerMaterialCollection;
    public GameObject zombieAnimation;
    public GameObject zombie;
    public GameObject currentTile;
    public GameObject nextTile;
    public GameObject nextColorTile;
    public Animation zombieAnimationStates;
    public Dropdown zombieActionPicker;
    public Button drawCard;
    public Button takeTurn;
    public Button startOver;
    public Text debugText;
    public RawImage imageNextColor;
    public string currentAnimation;
    public string zombieStartAnimation = "Zombie_Idle_01";
    public string zombieWalkAnimation = "Zombie_Walk_01";
    public int currentTileCount = 0;
    public int nextTileCount = 1;
    public int knockBackStepsMax = 3;
    public float walkSpeed = 15;
    public float turnSpeed = 5;
    public bool moveReverse = false;
    public bool backupZombie = false;
    public bool foundWinner = false;
    public bool showDebug = false;
    #endregion

    // Use this for initialization
    void Start()
    {
        signCollection = Resources.LoadAll<GameObject>("Prefab_Signs");
        updateDebug("Loading signCollection, found " + signCollection.Length.ToString() + " signs");
        tileMaterialCollection = Resources.LoadAll<Material>("Materials_TileColors");
        updateDebug("Loading tileMaterialCollection, found " + tileMaterialCollection.Length.ToString() + " colors");
        tileCornerMaterialCollection = Resources.LoadAll<Material>("Materials_TileCorners");
        updateDebug("Loading tileCornerMaterialCollection, found " + tileCornerMaterialCollection.Length.ToString() + " colors");

        getTileCollection();
        currentTile = tileCollection[currentTileCount];
        nextTile = tileCollection[nextTileCount];

        drawCard.onClick.AddListener(delegate { pickCard(); });
        takeTurn.onClick.AddListener(delegate { takePlayerTurn(); });
        startOver.onClick.AddListener(delegate { startGameOver(); });

        zombieActionPicker.onValueChanged.AddListener(delegate { zombieActionChanged(zombieActionPicker); });
        zombieActionPicker.ClearOptions();

        zombieAnimationStates = zombieAnimation.GetComponent<Animation>();

        foreach (AnimationState state in zombieAnimationStates)
        {
            zombieStates.Add(state);
            zombieOptions.Add(state.name);
            updateDebug("Zombie Animation Found: " + state.name);
        }

        updateDebug("AnimationStates: " + zombieStates.Count.ToString());
        zombieActionPicker.AddOptions(zombieOptions);
        currentAnimation = zombieStartAnimation;
    }

    /// Awake is called when the script instance is being loaded.
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (!foundWinner)
        {
            rotateZombie();

            debugText.enabled = showDebug;

            if (currentAnimation == zombieWalkAnimation)
            {
                if (nextColorTile != null)
                {
                    moveZombie();
                }
                else if (backupZombie)
                {
                    moveZombieBack();
                }
                else
                {
                    stopZombie();
                }
            }
            rotateZombie();
            takeTurn.enabled = true;
        }
        else
        {
            if (currentAnimation != "Zombie_Death_01")
            {
                setZombieAction(3);
            }
        }
    }

    void stopZombie()
    {
        setZombieAction(0);
        zombieActionPicker.value = 0;
        nextColorTile = null;
    }

    void moveZombie()
    {
        float step = walkSpeed * Time.deltaTime;
        zombie.transform.position = Vector3.MoveTowards(zombie.transform.position, nextTile.transform.position, step);

        updateDebug("Moving from " + currentTile.name + " to " + nextTile.name + " , looking for " + nextColorTile.name);
        if (zombie.transform.position == nextColorTile.transform.position)
        {
            stopZombie();

            currentTile = nextTile;
            currentTileCount = nextTileCount;
            updateDebug("nextTileCount: " + nextTileCount.ToString());

            if (nextTileCount == (tileCollection.Count - 1))
            {
                moveReverse = true;
            }

            if (moveReverse && currentTileCount == 0)
            {
                foundWinner = true;
                return;
            }

            nextTileCount = (moveReverse) ? nextTileCount - 1 : nextTileCount + 1;
            nextTile = tileCollection[nextTileCount];
        }
        else if (zombie.transform.position == nextTile.transform.position)
        {
            currentTile = nextTile;
            currentTileCount = nextTileCount;
            updateDebug("nextTileCount: " + nextTileCount.ToString());

            if (nextTileCount == (tileCollection.Count - 1))
            {
                moveReverse = true;
            }

            if (moveReverse && currentTileCount == 0)
            {
                stopZombie();
                foundWinner = true;
                setZombieAction(3);
                return;
            }

            nextTileCount = (moveReverse) ? nextTileCount - 1 : nextTileCount + 1;
            nextTile = tileCollection[nextTileCount];
        }
    }

    void moveZombieBack()
    {
        int numberOfSteps = Random.Range(1, knockBackStepsMax);
        updateDebug("No tile found, moving back " + numberOfSteps.ToString() + " space(s)");
        int backupTileCount = (moveReverse) ? currentTileCount + numberOfSteps : currentTileCount - numberOfSteps;
        nextTile = tileCollection[backupTileCount];

        nextColorTile = tileCollection[backupTileCount];
        nextTileCount = backupTileCount;

        float step = walkSpeed * Time.deltaTime;
        zombie.transform.position = Vector3.MoveTowards(zombie.transform.position, nextTile.transform.position, step);

        updateDebug("Moving backwards from " + currentTile.name + " to " + nextTile.name + " , looking for " + nextColorTile.name);
        if (zombie.transform.position == nextColorTile.transform.position)
        {
            stopZombie();

            currentTile = nextTile;
            currentTileCount = nextTileCount;
            updateDebug("nextTileCount: " + nextTileCount.ToString());

            nextTileCount = (moveReverse) ? nextTileCount + 1 : nextTileCount - 1;
            nextTile = tileCollection[nextTileCount];
        }
    }

    void rotateZombie()
    {
        float turn = turnSpeed * Time.deltaTime;
        Vector3 targetDir = nextTile.transform.position - zombie.transform.position;
        Vector3 newDir = Vector3.RotateTowards(zombie.transform.forward, targetDir, turn, 0.0F);
        zombie.transform.rotation = Quaternion.LookRotation(newDir);
    }

    void setZombieAction(int action)
    {
        currentAnimation = zombieStates[action].name;
        updateDebug(currentAnimation);
        zombieAnimationStates.Play(currentAnimation);
    }

    void getTileCollection()
    {
        GameObject[] allTiles;
        allTiles = GameObject.FindGameObjectsWithTag("GameTile");

        foreach (GameObject tile in allTiles)
        {
            tileCollection.Add(tile);
        }

        tileCollection.Sort(CompareListByName);

        updateDebug("tileCollection: " + tileCollection.Count.ToString());

        setTileColor();
    }

    void setTileColor()
    {
        int i = 0;
        int j = 0;
        int c = 0;

        //Create a list of color cards that are used in the game, could be less than the total list of color cards
        List<Material> tempTileMaterialCollection = new List<Material>();
        Material foundMaterial;
        Material cornerMaterial;

        foreach (GameObject tile in tileCollection)
        {
            i = Random.Range(0, tileMaterialCollection.Length);
            j = Random.Range(0, signCollection.Length);
            c = Random.Range(0, tileCornerMaterialCollection.Length);

            GameObject sign;
            GameObject text;

            foundMaterial = tileMaterialCollection[i];
            cornerMaterial = tileCornerMaterialCollection[c];

            sign = Instantiate(signCollection[j].gameObject);
            sign.name = signCollection[j].gameObject.name;
            text = sign.transform.GetChild(0).gameObject.transform.GetChild(0).gameObject;
            text.GetComponent<TextMesh>().color = foundMaterial.color;
            sign.transform.SetParent(tile.transform, false);
            updateDebug("sign: " + sign.name + " | parent: " + tile.name + " | position: " + sign.transform.position.ToString());

            tile.GetComponent<Renderer>().material = foundMaterial;

            for (int x = 1; x <= 4; x++)
            {
                tile.transform.Find("Corner_" + x.ToString()).GetComponent<Renderer>().material = cornerMaterial;
            }

            if (!tempTileMaterialCollection.Contains(foundMaterial)) { tempTileMaterialCollection.Add(foundMaterial); }

            updateDebug("Tile: " + tile.name + " | Tile Color: " + tileMaterialCollection[i].name + " | Sign: " + signCollection[j].name);
        }

        tileMaterialCollection = tempTileMaterialCollection.ToArray();
    }
    void updateDebug(string message)
    {
        if (showDebug)
        {
            Debug.Log(message);
            debugText.text = message;
        }
    }

    private static int CompareListByName(GameObject i1, GameObject i2)
    {
        return i1.name.CompareTo(i2.name);
    }

    private void zombieActionChanged(UnityEngine.UI.Dropdown target)
    {
        updateDebug(target.value.ToString());
        setZombieAction(target.value);
    }

    private void pickCard()
    {
        int i = Random.Range(0, tileMaterialCollection.Length);
        int tile = -1;
        Color nextColor = tileMaterialCollection[i].color;
        updateDebug("Picking a card: " + i + " | " + "Card Color: " + nextColor.ToString());
        imageNextColor.GetComponent<Graphic>().color = nextColor;

        i = moveReverse ? i = 0 : i = tileCollection.Count - 1;

        if (moveReverse)
        {
            for (int j = currentTileCount - 1; j >= i; j--)
            {
                updateDebug("moveReverse: " + moveReverse.ToString() + " | i:" + i.ToString() + " | j: " + j.ToString() + " | tileCollection: [" + tileCollection[j].GetComponent<Renderer>().material.color.ToString() + "] | NextColor: [" + nextColor.ToString() + "]");
                if (tileCollection[j].GetComponent<Renderer>().material.color == nextColor)
                {
                    nextColorTile = tileCollection[j].gameObject;
                    tile = j;
                    break;
                }
            }
        }
        else
        {
            for (int j = currentTileCount + 1; j <= i; j++)
            {
                updateDebug("moveReverse: " + moveReverse.ToString() + " | i:" + i.ToString() + " | j: " + j.ToString() + " | tileCollection: [" + tileCollection[j].GetComponent<Renderer>().material.color.ToString() + "] | NextColor: [" + nextColor.ToString() + "]");
                if (tileCollection[j].GetComponent<Renderer>().material.color == nextColor)
                {
                    nextColorTile = tileCollection[j].gameObject;
                    tile = j;
                    break;
                }
            }
        }
        if (tile > -1) updateDebug("Next Tile to stop at will be: " + nextColorTile.name + " [tile " + tile.ToString() + "]");

        if (tile == -1)
        {
            backupZombie = true;
        }
    }

    private void takePlayerTurn()
    {
        if (currentAnimation != zombieWalkAnimation)
        {
            updateDebug("Player taking turn");
            takeTurn.enabled = false;
            pickCard();
            setZombieAction(1);
        }
    }

    public void startGameOver()
    {
        updateDebug("Starting Over");
        currentTileCount = 0;
        nextTileCount = 1;
        nextTile = tileCollection[1];
        zombie.transform.position = tileCollection[0].transform.position;
        setZombieAction(0);
        moveReverse = false;
        foundWinner = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}