using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestZombieAnimations : MonoBehaviour
{
    #region Public Variables
    public List<AnimationState> zombieStates = new List<AnimationState>();
    public List<GameObject> tileCollection = new List<GameObject>();
    public List<string> zombieOptions = new List<string>();
    public Material[] tileMaterialCollection;
    public GameObject[] signCollection;
    public GameObject zombieAnimation;
    public GameObject zombie;
    public GameObject currentTile;
    public GameObject nextTile;
    public GameObject nextColorTile;
    public Animation zombieAnimationStates;
    public Dropdown zombieActionPicker;
    public Button drawCard;
    public Text debugText;
    public RawImage imageNextColor;
    public string currentAnimation;
    public string zombieStartAnimation = "Zombie_Idle_01";
    public string zombieWalkAnimation = "Zombie_Walk_01";
    public int currentTileCount = 0;
    public int nextTileCount = 1;
    public float walkSpeed = 15;
    public float turnSpeed = 5;
    public bool moveReverse = false;
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

        getTileCollection();
        currentTile = tileCollection[currentTileCount];
        nextTile = tileCollection[nextTileCount];
        nextColorTile = tileCollection[0];

        drawCard.onClick.AddListener(delegate { pickCard(); });

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
        rotateZombie();

        debugText.enabled = showDebug;

        if (currentAnimation == zombieWalkAnimation)
        {
            moveZombie();
        }

        rotateZombie();
    }

    void moveZombie()
    {
        float step = walkSpeed * Time.deltaTime;
        zombie.transform.position = Vector3.MoveTowards(zombie.transform.position, nextTile.transform.position, step);

        updateDebug("Moving from " + currentTile.name + " to " + nextTile.name + " , looking for " + nextColorTile.name);
        if (zombie.transform.position == nextColorTile.transform.position)
        {
            setZombieAction(0);
            zombieActionPicker.value = 0;

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
                setZombieAction(0);
                foundWinner = true;
                return;
            }

            nextTileCount = (moveReverse) ? nextTileCount - 1 : nextTileCount + 1;
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

        //Create a list of color cards that are used in the game, could be less than the total list of color cards
        List<Material> tempTileMaterialCollection = new List<Material>();
        Material foundMaterial;

        foreach (GameObject tile in tileCollection)
        {
            i = Random.Range(0, tileMaterialCollection.Length);
            j = Random.Range(0, signCollection.Length);
            j = 0;
            GameObject sign;
            sign = Instantiate(signCollection[j].gameObject);
            sign.name = signCollection[j].gameObject.name;
            sign.transform.position = new Vector3(0, 1, 0);
            sign.transform.SetParent(tile.transform, false);

            sign.GetComponent<Renderer>().material = tileMaterialCollection[i];
            updateDebug("sign: " + sign.name + " | parent: " + tile.name + " | position: " + sign.transform.position.ToString());

            foundMaterial = tileMaterialCollection[i];
            tile.GetComponent<Renderer>().material = foundMaterial;
            if (!tempTileMaterialCollection.Contains(foundMaterial)) { tempTileMaterialCollection.Add(foundMaterial); }

            updateDebug("Tile: " + tile.name + " | Tile Color: " + tileMaterialCollection[i].name + " | Sign: " + signCollection[j].name);
            updateDebug("Sign: " + sign.name + " | Sign Cords: " + sign.transform.rotation.x.ToString());
        }

        //tileMaterialCollection = tempTileMaterialCollection.ToArray();
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

        updateDebug("Picking a card: " + i);
        Color nextColor = tileMaterialCollection[i].color;
        updateDebug("Card Color: " + nextColor.ToString());
        imageNextColor.GetComponent<Graphic>().color = nextColor;

        i = moveReverse ? i = 0 : i = tileCollection.Count - 1;

        if (moveReverse)
        {
            for (int j = currentTileCount; j >= i; j--)
            {
                updateDebug("moveReverse: " + moveReverse.ToString() + " | i:" + i.ToString() + " | j: " + j.ToString() + " | tileCollection: [" + tileCollection[j].GetComponent<Renderer>().material.color.ToString() + "] | NextColor: [" + nextColor.ToString() + "]");
                if (tileCollection[j].GetComponent<Renderer>().material.color == nextColor)
                {
                    nextColorTile = tileCollection[j].gameObject;
                    break;
                }
            }
        }
        else
        {
            for (int j = currentTileCount; j <= i; j++)
            {
                updateDebug("moveReverse: " + moveReverse.ToString() + " | i:" + i.ToString() + " | j: " + j.ToString() + " | tileCollection: [" + tileCollection[j].GetComponent<Renderer>().material.color.ToString() + "] | NextColor: [" + nextColor.ToString() + "]");
                if (tileCollection[j].GetComponent<Renderer>().material.color == nextColor)
                {
                    nextColorTile = tileCollection[j].gameObject;
                    break;
                }
            }
        }
        updateDebug("Next Tile to stop at will be: " + nextColorTile.name);
    }
}