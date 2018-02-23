using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestZombieAnimations : MonoBehaviour
{
    public List<AnimationState> zombieStates = new List<AnimationState>();
    public GameObject[] tileCollection;
    public Material[] tileMaterialCollection;
    public List<string> zombieOptions = new List<string>();
    public Animation zombieAnimationStates;
    public Dropdown zombieActionPicker;
    public GameObject zombieAnimation;
    public Text debugText;
    public bool showDebug = false;

    // Use this for initialization
    void Start()
    {
        getTileCollection();
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
    }

    /// Awake is called when the script instance is being loaded.
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        debugText.enabled = showDebug;
    }

    void setZombieAction(int action)
    {
        updateDebug(zombieStates[action].name);
        zombieAnimationStates.Play(zombieStates[action].name);
    }

    void getTileCollection()
    {
        tileCollection = GameObject.FindGameObjectsWithTag("GameTile");
        updateDebug("tileCollection: " + tileCollection.Length.ToString());

        int i = 0;
        foreach (GameObject tile in tileCollection)
        {
            i = Random.Range(0,tileMaterialCollection.Length);
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

    private void zombieActionChanged(UnityEngine.UI.Dropdown target)
    {
        updateDebug(target.value.ToString());
        setZombieAction(target.value);
    }
}