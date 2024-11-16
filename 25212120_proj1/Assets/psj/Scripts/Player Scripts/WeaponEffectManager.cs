using System.Collections;
using UnityEngine;

public class WeaponEffectManager : MonoBehaviour
{
    public ParticleSystem SwordShield_1;
    public ParticleSystem SwordShield_2;
    public ParticleSystem SingleTwoHandSword_1;
    public ParticleSystem SingleTwoHandSword_2;
    public ParticleSystem DoubleSwords_1;
    public ParticleSystem DoubleSwords_2;
    public ParticleSystem WhirlWind;

    public void PlaySwordShield_1()
    {
        SwordShield_1.Stop();
        SwordShield_1.Play();
    }

    public void PlaySwordShield_2()
    {
        SwordShield_2.Stop();
        SwordShield_2.Play();
    }

    public void PlaySingleTwoHandSword_1()
    {
        SingleTwoHandSword_1.Stop();
        SingleTwoHandSword_1.Play();
    }
    public void PlaySingleTwoHandSword_2()
    {
        SingleTwoHandSword_2.Stop();
        SingleTwoHandSword_2.Play();
    }

    public void PlayDoubleSwords_1()
    {
        DoubleSwords_1.Stop();
        DoubleSwords_1.Play();
    }
    public void PlayDoubleSwords_2()
    {
        DoubleSwords_2.Stop();
        DoubleSwords_2.Play();
    }
    
    public IEnumerator PlayWhirlWind()
    {
        WhirlWind.Stop();
        WhirlWind.Play();

        yield return new WaitForSeconds(0.3f);
        WhirlWind.Stop();
    }
}
