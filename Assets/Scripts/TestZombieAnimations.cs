
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestZombieAnimations : MonoBehaviour
{
    public List<AnimationState> zombieStates = new List<AnimationState>();
    public List<GameObject> tileCollection;
    public Material[] tileMaterialCollection;
    public List<string> zombieOptions = new List<string>();
    public Animation zombieAnimationStates;
    public Dropdown zombieActionPicker;
    public GameObject zombieAnimation;
    public GameObject zombie;
    public GameObject currentTile;
    public GameObject nextTile;
    public int currentTileCount = 0;
    public int nextTileCount = 1;
    public Text debugText;
    public float walkSpeed = 15;
    public float turnSpeed = 5;
    public string currentAnimation;
    public bool moveReverse = false;
    public bool foundWinner = false;
    public bool showDebug = false;

    // Use this for initialization
    void Start()
    {
        getTileCollection();
        currentTile = tileCollection[currentTileCount];
        nextTile = tileCollection[nextTileCount];
        debugText.enabled = false;
        zombieActionPicker.onValueChanged.AddListener(delegate { zombieActionChanged(zombieActionPicker); });
        zombieActionPicker.ClearOptions();
        zombieAnimationStates = zombieAnimation.GetComponent<Animation>();
        foreach (AnimationState state in zombieAnimationStates)
        {
            zombieStates.Add(state);
            zombieOptions.Add(state.name);
            updateDebug(state.name);
        }
        updateDebug("AnimationStates: " + zombieStates.Count.ToString());
        zombieActionPicker.AddOptions(zombieOptions);
        currentAnimation = zombieOptions[0];
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
        if (currentAnimation == "Zombie_Walk_01")
        {
            moveZombie();
        }
        rotateZombie();
    }

    void moveZombie()
    {
        float step = walkSpeed * Time.deltaTime;

        zombie.transform.position = Vector3.MoveTowards(zombie.transform.position, nextTile.transform.position, step);

        // when arriving at next tile change action to idle and find new next tile
        if (zombie.transform.position == nextTile.transform.position)
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

        int i = 0;
        foreach (GameObject tile in tileCollection)
        {
            i = Random.Range(0, tileMaterialCollection.Length);
            updateDebug("Random number: " + i);
            updateDebug("Tile: " + tile.name);
            updateDebug("Tile Color: " + tileMaterialCollection[i].name);
            tile.GetComponent<Renderer>().material = tileMaterialCollection[i];
        }
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
}