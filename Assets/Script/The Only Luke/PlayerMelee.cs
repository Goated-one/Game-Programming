using UnityEngine;
using System.Collections; 
using UnityEngine.EventSystems;

public class PlayerMelee : MonoBehaviour
{
    public float dropForce = 15f;      
    public GameObject meleeHitbox;     
    public GameObject dropHitbox;      
    public float recoveryTime = 0.5f;

    private Rigidbody2D rb;
    private Animator anim;
    private PlayerController playerCtrl; 
    
    public Animator meleeSlashAnim; 
    public Animator dropSlashAnim;  

    private bool hasDropAttacked = false; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerCtrl = GetComponent<PlayerController>(); 
    }

    void Update()
    {
        // SATPAM UI & PAUSE
        if (PauseMenuManager.GameIsPaused || EventSystem.current.IsPointerOverGameObject()) return;

        if (playerCtrl.isGrounded && hasDropAttacked)
        {
            hasDropAttacked = false; 
            StartCoroutine(DropAttackRecovery()); 
        }

        if ((Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.K)) && playerCtrl.canMove)
        {
            if (playerCtrl.isGrounded)
            {
                // --- TAMBAHAN AUDIO ---
                if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Melee");

                anim.SetTrigger("Melee");
            }
            else if (!hasDropAttacked) 
            {
                hasDropAttacked = true; 
                
                // --- TAMBAHAN AUDIO ---
                if (AudioManager.instance != null) AudioManager.instance.PlaySFX("Drop Attack");

                anim.SetTrigger("DropAttack");
                rb.linearVelocity = new Vector2(0, -dropForce);
            }
        }
    }

    IEnumerator DropAttackRecovery()
    {
        playerCtrl.canMove = false; 
        rb.linearVelocity = Vector2.zero; 
        yield return new WaitForSeconds(recoveryTime); 
        playerCtrl.canMove = true; 
    }

    public void EnableMeleeHitbox()
    {
        meleeHitbox.SetActive(true);
        if (meleeSlashAnim != null) meleeSlashAnim.Play("Slash_Anim", -1, 0f); 
    }

    public void DisableMeleeHitbox()
    {
        meleeHitbox.SetActive(false);
    }

    public void EnableDropHitbox()
    {
        dropHitbox.SetActive(true);
        if (dropSlashAnim != null) dropSlashAnim.Play("DropSlash_Anim", -1, 0f); 
    }

    public void DisableDropHitbox()
    {
        dropHitbox.SetActive(false);
    }
}