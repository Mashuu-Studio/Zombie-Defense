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
    public float Speed { get; }
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
    public float speed;
    public int hp;

    public bool IsHeal { get { return hp > 0 && dmg == 0 && def == 0 && aspeed == 0; } }
    public static BuffInfo operator +(BuffInfo a, BuffInfo b)
    {
        a.dmg += b.dmg;
        a.def += b.def;
        a.aspeed += b.aspeed;
        a.speed += b.speed;
        a.hp += b.hp;

        return a;
    }
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
    public List<BuffInfo> Buffs { get; }
    public BuffInfo ActivatedBuff
    {
        get
        {
            BuffInfo buff = new BuffInfo();
            if (Buffs != null) Buffs.ForEach(b => buff += b);
            return buff;
        }
    }
    public bool BuffIsActivated { get { return Buffs == null || Buffs.Count == 0; } }
    public void ActivateBuff(BuffInfo buff);
    IEnumerator BuffTimer(BuffInfo buff);
}

public interface ISummonObject
{
    public List<GameObject> SummonedUnits { get; }

    public bool CanSummon { get; }

    public bool DetectTarget();
    public void Summon();
    IEnumerator SummonTimer();
}
