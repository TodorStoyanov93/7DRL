using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public Animator playerAnimator;
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

    private void Init() {
        if (playerAnimator == null) { 
            playerAnimator = BoardManager.Instance.player.GetComponent<Animator>();
        }
    }

    public void PlayMoveAnimation() {
        Init();
        playerAnimator.SetBool("move",true);
    }

    public void StopMoveAnimation()
    {
        Init();
        playerAnimator.SetBool("move", false);
    }

    public void PlayAttackAnimation() {
        Init();
        playerAnimator.SetTrigger("attack");
    }


}
