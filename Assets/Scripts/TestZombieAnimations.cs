using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestZombieAnimations : MonoBehaviour
{
    public GameObject zombie_Animation;
    public List<AnimationState> zStates = new List<AnimationState>();
    public Animation myAnim;
    public GameObject zombie;
    public Text debugText;
    public Dropdown zAction;
    public List<string> zOptions = new List<string>();

    // Use this for initialization
    void Start()
    {
        zAction.onValueChanged.AddListener(delegate { zActionChanged(zAction); });
        zAction.ClearOptions();
        myAnim = zombie_Animation.GetComponent<Animation>();
        foreach (AnimationState state in myAnim)
        {
            zStates.Add(state);
            zOptions.Add(state.name);
            updateDebug(state.name);
        }
        updateDebug("AnimationStates: " + zStates.Count.ToString());
        zAction.AddOptions(zOptions);
    }

    /// Awake is called when the script instance is being loaded.
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void zActionChanged(UnityEngine.UI.Dropdown target)
    {
        updateDebug(target.value.ToString());
        setZombieAction(target.value);
    }

    void setZombieAction(int action)
    {
        updateDebug(zStates[action].name);
        myAnim.Play(zStates[action].name);
    }
    void updateDebug(string message)
    {
        Debug.Log(message);
        //test
    }
}