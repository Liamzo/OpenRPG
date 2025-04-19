using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseThought : MonoBehaviour
{
    protected NonPlayerBrain brain;

    public bool canReact;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        brain = GetComponentInParent<NonPlayerBrain>();
    }

    public abstract float Evaluate();

    public abstract void Execute();
}
