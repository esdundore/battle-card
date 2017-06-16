using UnityEngine;
using DG.Tweening;

public class MonsterAttackVisual : MonoBehaviour 
{
    private WhereIsTheCardOrMonster w;

    void Awake()
    {
        w = GetComponent<WhereIsTheCardOrMonster>();
    }

    public void AttackTarget(int targetUniqueID, int damageTakenByTarget, int damageTakenByAttacker, int attackerHealthAfter, int targetHealthAfter, GameObject target)
    {
        Debug.Log(targetUniqueID);
        //GameObject target = IDHolder.GetGameObjectWithID(targetUniqueID);

        // bring this creature to front sorting-wise.
        w.BringToFront();
        VisualStates tempState = w.VisualState;
        w.VisualState = VisualStates.Transition;

        transform.DOMove(target.transform.position, 0.5f).SetLoops(2, LoopType.Yoyo).SetEase(Ease.InCubic).OnComplete(() =>
            {
                if(damageTakenByTarget>0)
                    DamageEffect.CreateDamageEffect(target.transform.position, damageTakenByTarget);
                if(damageTakenByAttacker>0)
                    DamageEffect.CreateDamageEffect(transform.position, damageTakenByAttacker);
                
                //target.GetComponent<MonsterManager>().HealthText.text = targetHealthAfter.ToString();

                w.SetTableSortingOrder();
                w.VisualState = tempState;

                //manager.HealthText.text = attackerHealthAfter.ToString();
                Sequence s = DOTween.Sequence();
                s.AppendInterval(1f);
                s.OnComplete(Command.CommandExecutionComplete);
                //Command.CommandExecutionComplete();
            });
    }
        
}
