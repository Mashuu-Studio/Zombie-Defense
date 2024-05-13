using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagedObject
{
    public int Hp { get; }
    public int Def { get; }
    public void Damaged(int dmg, ObjectData.Attribute attribute = ObjectData.Attribute.NONE);
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

    public void AdjustMove(bool b);

    public static Vector2 GetPos(Pathfinding.Int3 pos)
    {
        return new Vector2(pos.x / 1000f, pos.y / 1000f);
    }

    public static bool EndOfPath(Vector2 pos, Vector2 next, Vector2 dir, float radius)
    {
        float dist1 = Vector2.Distance(pos, next);
        float dist2 = Vector2.Distance(pos, next + dir);

        int layermask = 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Turret");
        var ray1 = Physics2D.Raycast(pos, dir, radius + 0.2f, layermask);
        var ray2 = Physics2D.Raycast(pos + Vector2.Perpendicular(dir.normalized) * 0.05f, dir, radius + 0.2f, layermask);
        var ray3 = Physics2D.Raycast(pos - Vector2.Perpendicular(dir.normalized) * 0.05f, dir, radius + 0.2f, layermask);

        return dist1 < 0.1f || dist1 >= dist2 
            || (ray1.collider != null || ray2.collider != null || ray3.collider != null);
    }
}

[System.Serializable]
public class BuffInfo
{
    public bool area;
    public float time = 0;

    public float delay;

    public int dmg = 0;
    public int def = 0;
    public float aspeed = 0;
    public float speed = 0;
    public int hp = 0;

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

public interface ICombineObject
{
    // 합체의 트리거가 되는 필드. 이동을 시작함.
    public bool IsCombined { get; }
    // 합체를 시작하게 되면 사용되는 필드. 이동을 멈춤.
    public bool IsCombining { get; }
    public int Hp { get; }
    public bool CheckHPState { get; }
    public bool DetectOtherObject();
    public void Combine();
    IEnumerator Combining();
    public void Combined(bool b);
}

public interface IRestObject
{
    public bool CheckHPState { get; }
    public bool IsHealed { get; }
    public bool IsRunningAway { get; }
    public bool IsArrived { get; }
    public void Runaway();
    public void Rest();
}