using UnityEngine;

public class Soldier : BaseUnit
{
    protected override bool IsValidTarget(Collider hitCollider)
    {
        return hitCollider.CompareTag("Enemy"); // Hedef d��man olmal�
    }

    // Askerlere �zg� davran��lar� burada ekleyebiliriz
}
