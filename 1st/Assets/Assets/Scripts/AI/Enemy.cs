using UnityEngine;

public class Enemy : BaseUnit
{
    protected override bool IsValidTarget(Collider hitCollider)
    {
        return hitCollider.CompareTag("Soldier"); // Hedef asker olmal�
    }

    // D��manlara �zg� davran��lar� burada ekleyebiliriz
}
