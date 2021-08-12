using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharAnimEvents : MonoBehaviour
{
    // Start is called before the first frame update

    Animator Anim;

    public void AnimHitTheEnemyTriggerON()
    {
        Anim.SetBool("HitTheEnemy", true);
    }

    public void AnimBattleStartTriggerOff()
    {
        Anim.SetBool("BattleStart", false);
    }

    public void AnimHurtTriggerOff()
    {
        Anim.SetBool("Hurt", false);
    }
    
    public void AnimDeadTriggerOff()
    {
        Anim.SetBool("Dead", false);
    }

    public void DyingDone()
    {
        Anim.SetBool("Dying", true);
        //this.Get(false);
    }

    public void SpinSlashSlashDone()
    {
        Anim.SetBool("SpinSlashSlash", false);
        //this.Get(false);
    }

    public void MeteorSlamDone()
    {
        Anim.SetBool("MeteorSlam", false);
    }

    public void WarCryDone()
    {
        Anim.SetBool("WarCry", false);
    }

    public void PDefSpeechDone()
    {
        Anim.SetBool("PDefSpeech", false);
    }

    public void SurveyLookSDone()
    {
        Anim.SetBool("SurveyLookS", false);
    }

    public void ThinThrustDone()
    {
        Anim.SetBool("ThinThrust", false);
    }

    public void GlareDone()
    {
        Anim.SetBool("Glare", false);
    }

    

    void Start()
    {
        Anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
