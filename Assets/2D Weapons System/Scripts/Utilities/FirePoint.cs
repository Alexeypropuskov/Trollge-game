using UnityEngine;

namespace WeaponSystemUtilities
{

public class FirePoint
{
    public Vector2 Position;
    public Vector3 Direction;
    public Transform Weapon;

    public FirePoint(Vector2 position, Vector3 Rotation, Transform weapon)
    {
        Position = position;
        this.Direction = Rotation;
        Weapon = weapon;
    }


    public Vector3 GetRelativePosition()
    {
        Vector3 AddDirection = Weapon.transform.right * Position.x + Weapon.transform.up * Position.y;
        return Weapon.position + AddDirection;
    }

    public Vector3 GetReletiveDirection()
    {
        Vector3 AddDirection = Weapon.transform.right * Direction.x + Weapon.transform.up * Direction.y;
        return (AddDirection).normalized;
    }

}
}

