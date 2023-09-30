using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemyHurtResponder : MonoBehaviour, IHurtResponder
{
    private List<HurtBox> m_hurtboxes = new List<HurtBox>();

    public GameObject hitParticle;
    private GameObject hitParticleClone;

    public AudioSource audioSource;
    public AudioClip deathSound;

    private void Start()
    {
        m_hurtboxes = new List<HurtBox>(GetComponentsInChildren<HurtBox>());
        foreach (HurtBox _hurtBox in m_hurtboxes)
        {
            _hurtBox.HurtResponder = this;
        }
    }

    bool IHurtResponder.CheckHit(HitData hitdata)
    {
        return true;
    }

    void IHurtResponder.Response(HitData data) 
    {
        audioSource.pitch = UnityEngine.Random.Range(0.8f, 1.2f);
        audioSource.PlayOneShot(deathSound, 1f);
        hitParticleClone = Instantiate(hitParticle, transform.position + Vector3.up, transform.rotation);
        Destroy(hitParticleClone, 1f);
        gameObject.GetComponent<Ragdoll>().Damage(data.damage);
    }
}
