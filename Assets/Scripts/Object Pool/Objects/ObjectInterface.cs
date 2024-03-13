using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagedObject
{
    public int Hp { get; }
    public int Def { get; }
    public void Damaged(int dmg);
}

public interface IAttackObject
{
    public Collider2D TargetCollider { get; }
    public int Dmg { get; }
    public float Range { get; }
    public float ADelay { get; }
    public bool WaitAttack { get; }
    public bool DetectTarget();
    public void Attack();
}

public interface IMovingObject
{
    public int Speed { get; }
    public bool DetectPath();

    public void Move();
}

[System.Serializable]
public class BuffInfo
{
    public bool area;
    public float time;

    public float delay;

    public int dmg;
    public int def;
    public float aspeed;
    public int hp;

    public bool IsHeal { get { return hp > 0 && dmg == 0 && def == 0 && aspeed == 0; } }
}

public interface IBuffObject
{
    public BuffInfo Buff { get; }
    public float BuffRange { get; }
    public float BDelay { get { return Buff.delay; } }
    public bool WaitBuff { get; set; }
    public bool DetectBuffTarget();
    public void GiveBuff();
    IEnumerator GiveBuffTimer();

}

public interface IBuffTargetObject
{
    public BuffInfo ActivatedBuff { get; }
    public bool BuffIsActivated { get; }
    public void ActivateBuff(BuffInfo buff);
    IEnumerator BuffTimer();
}

public interface ISummonObject
{
    public List<GameObject> SummonedUnits { get; }

    public bool CanSummon { get; }

    public bool DetectTarget();
    public void Summon();
    IEnumerator SummonTimer();
}
