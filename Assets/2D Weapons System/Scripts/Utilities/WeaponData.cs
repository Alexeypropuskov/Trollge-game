using System;
using UnityEngine;
using WeaponSystem;

namespace WeaponScriptableObject
{
    public class WeaponData : ScriptableObject

    {
        [HideInInspector]
        public Transform[] points;
        [Header("Muzzles")]
        [SerializeField]
        public Weapon.Muzzle[] Muzzles;
        [SerializeField]
        public PosAndRotation[] ChildsPosAndRotations;

        [Serializable]
        public class PosAndRotation
        {


            public Vector3 Position;
            public Quaternion Rotation;


            public PosAndRotation(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
            }
        }
        [Header("Fire Mode")]

        public Weapon.FireModes FireMode;


        [SerializeField]
        public float FireRate;

        [Header("Burst")]

        //burst
        [SerializeField]
        [Min(1)]
        public int BurstCount;
        [SerializeField]
        public float TimebetweenShots;
        [SerializeField]
        public float TimeBetweenBursts;

        [Header("Spread")]

        //Spread
        [SerializeField]
        [Min(1)]
        public int ProjectilesCount;
        [SerializeField]
        public bool RandomSpread;
        [SerializeField]
        public float MinAngle, MaxAngle;
        [SerializeField]
        [Range(0, 1)]
        public float SpreadFactor;

        [SerializeField]
        public bool AutoShooting;

        [Header("Recoil")]
        [SerializeField]
        public bool Recoil;
        [SerializeField]
        public float RecoilForce;
        [SerializeField]
        [Min(0)]
        public float RecoilRecoveryTime;
        [SerializeField]
        public AnimationCurve RecoilCurve;
        [Header("Projectile")]

        [SerializeField]
        public Projectile projectile;
        [SerializeField]
        public Projectile.ProjectileType projectileType;
        [SerializeField]
        public float speed;
        [SerializeField]
        public bool UnlimitedLifeTime;
        [SerializeField]
        public float lifetime;
        [SerializeField]
        public bool DestroyOnDistenceTravled;
        [SerializeField]
        public float MaxDistence;
        [SerializeField]
        public float ProjectileGravityModifier;
        [SerializeField]
        public GameObject ProjectileImpactEffect;
        public int Damege, DamegeToStructure;

        public float FollowStrength;





        [Header("Ammo")]

        //AMMO 
        [SerializeField]
        public bool UnlimitedAmmo;
        [SerializeField]
        public int MaxAmmo;
        [SerializeField]
        public bool UnlimitedMagazineSize;
        [SerializeField]
        public int MagazineSize;
        [SerializeField]
        public float ReloadTime;
        [SerializeField]
        public bool PartialAmmoFire;
        [SerializeField]
        public bool AutoReload;



        [SerializeField]
        public bool countEachProjectile;
        [SerializeField]
        public int ammoPerShot;



    }
}