using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{ 
    public static AnimationManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayMoveAnimation(Unit unit) {
        var animator = unit.gameObject.GetComponent<Animator>();
        animator.SetBool("move",true);
    }

    public void StopMoveAnimation(Unit unit)
    {
        var animator = unit.gameObject.GetComponent<Animator>();
        animator.SetBool("move", false);
    }

    public void PlayAttackAnimation(Unit unit) {
        var animator = unit.gameObject.GetComponent<Animator>();
        animator.SetTrigger("attack");
    }


}
