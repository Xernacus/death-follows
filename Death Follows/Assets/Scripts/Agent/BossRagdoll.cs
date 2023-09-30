using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class BossRagdoll : MonoBehaviour
{
    Rigidbody[] rigidbodies;
    Collider[] colliders;
    public GameObject art;
    public int health;
    Animator animator;
    public Image fadeOverlay;
    public GameObject text;
    private TextMeshProUGUI winText;

    // Start is called before the first frame update
    void Start()
    {
        winText = text.GetComponent<TextMeshProUGUI>();
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        animator = GetComponentInChildren<Animator>();

        DeactivateRagdoll();
    }

    public void DeactivateRagdoll()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = true;
        }
        foreach (var collider in colliders)
        {
            if (!collider.isTrigger)
            {
                collider.enabled = false;
            }
            
        }        
        animator.enabled = true;
    }

    public void ActivateRagdoll()
    {
        foreach (var rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = false;
        }
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
        animator.enabled = false;
    }

    public void Damage(int damage)
    {
        Debug.Log("Enemy got damaged");
        health -= damage;
        if (health <= 0)
        {
            //ActivateRagdoll();
            StartCoroutine(ScreenFade());
        }
        art.SetActive(false);
    }

    IEnumerator ScreenFade()
    {
        while (fadeOverlay.color.a < 1f)
        {
            fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, fadeOverlay.color.a + .005f);
            winText.color = new Color(winText.color.r, winText.color.g, winText.color.b, winText.color.a + .1f);
            yield return null;
        }
        
        SceneManager.LoadScene("MainMenu");

    }
}
