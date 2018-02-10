using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestZombieAnimations : MonoBehaviour
{
    public Animation myAnim;
    public GameObject zombie;

    // Use this for initialization
    void Start()
    {
		myAnim = zombie.GetComponent<Animation>();
        foreach (AnimationState state in myAnim) {
            state.speed = 0.5F;
			Debug.Log(state.name);
        }
    }

    /// Awake is called when the script instance is being loaded.
    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}