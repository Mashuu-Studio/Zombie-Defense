using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamagedObject
{
    public int Hp { get; }
    public void Damaged(int dmg);
}

public interface IAttackObject
{
    public Collider2D TargetCollider { get; }
    public int Dmg { get; }
    public float Range { get; }
    public float ADelay { get; }
    public bool WaitAttack { get; set; }
    public bool DetectTarget();
    public void Attack();
    IEnumerator AttackTimer();
}

public interface IMovingObject
{
    public int Speed { get; }
    public bool DetectPath();
    public void Move();
}
