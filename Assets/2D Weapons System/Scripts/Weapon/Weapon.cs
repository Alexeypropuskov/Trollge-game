using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;
using WeaponScriptableObject;
using WeaponSystemUtilities;
namespace WeaponSystem
{

    public class Weapon : MonoBehaviour
    {
        [SerializeField]
        public WeaponData weaponData;
        #region Muzzles 

        [Tooltip("Assigne the Points Manually.")]
        [SerializeField]
        public Muzzle[] Muzzles;
        [Serializable]
        public struct Muzzle
        {
            [HideInInspector]
            public Transform position;
            public MuzzleModes MuzzleMode;
            [Min(1f)]
            public int PointsNumber;
            // In Line Mode
            [SerializeField]
            [Min(0.0001f)]

            public float DistenceBetweenPoints;
            //In Circle Mode
            [Min(0.0001f)]
            [SerializeField]
            public float radius;
            [SerializeField]
            public bool AutoCalculateStepAngle;
            [Min(0.0001f)]
            [SerializeField]
            public float stepangle;

            [Range(0, 1)]
            [SerializeField]
            public float FireAngleModifier;

        }

        public enum MuzzleModes
        {
            line, circle

        }



        private List<FirePoint> Points = new List<FirePoint>();
        #endregion

        #region FireModes And Projectile Settings
        public enum FireModes
        {
            auto, burst, single, Spread
        }
        public FireModes FireMode;
        [SerializeField]
        private Projectile projectile;
        [SerializeField]
        public Projectile.ProjectileType projectileType;
        public int Damege, DamegeToStructure;
        [SerializeField]
        private float speed;
        [SerializeField]
        public bool UnlimitedLifeTime;
        [SerializeField]
        public float lifetime;
        [SerializeField]
        public bool DestroyOnDistenceTravled;
        [SerializeField]
        public float MaxDistence;
        [SerializeField]
        private float ProjectileGravityModifier;
        [SerializeField]
        public Transform ProjectileTarget;
        [SerializeField]
        private GameObject ProjectileImpactEffect;
        [SerializeField]
        private float FollowStrength;


        public bool UsePlayerInputs = true;
        private bool CanShoot;
        private bool FireInputHold, FireInputDown;
        [SerializeField]
        public KeyCode FireKey;
        [SerializeField]
        public bool Recoil;
        [SerializeField]
        public float RecoilForce;
        [SerializeField]
        [Min(0)]
        public float RecoilRecoveryTime;
        [SerializeField]
        private AnimationCurve RecoilCurve;
        [SerializeField]
        private float recoilfactor;
        [SerializeField]
        private float ShootingTimer, NotShootingTimer;

        //auto && Single
        [SerializeField]
        private float FireRate;


        //burst
        [SerializeField]
        [Min(1)]
        private int BurstCount;
        [SerializeField]
        private float TimebetweenShots;
        [SerializeField]
        private float TimeBetweenBursts;


        //Spread
        [SerializeField]
        [Min(1)]

        private int ProjectilesCount;
        [SerializeField]
        private bool RandomSpread;
        [SerializeField]
        private float MinAngle, MaxAngle;
        [SerializeField]
        [Range(0, 1)]
        private float SpreadFactor;

        [SerializeField]
        public bool AutoShooting;

        #endregion
        #region Ammo

        //AMMO 
        [SerializeField]
        public bool UnlimitedAmmo;
        [SerializeField]
        private int MaxAmmo;
        [SerializeField]
        public bool UnlimitedMagazineSize;
        [SerializeField]
        private int MagazineSize;
        [SerializeField]
        private float ReloadTime;
        [SerializeField]
        private bool PartialAmmoFire;
        [SerializeField]
        public bool AutoReload;
        [SerializeField]
        private KeyCode ReloadKey;


        private int currentAmmo, currentTotalAmmo;
        private bool Reloading = false;

        [SerializeField]
        public bool countEachProjectile;
        [SerializeField]
        private int ammoPerShot;
        #endregion
        #region Sounds
        //Sounds
        [Serializable]
        public struct SoundSettings
        {
            [SerializeField]
            public AudioClip Clip;
            [SerializeField]
            public bool CreateAudioSource;
            [SerializeField]
            public AudioSource Source;
            [SerializeField]
            public float volume;
            [SerializeField]
            public float DelayTime;
        }

        //Shoot
        [SerializeField]
        public SoundSettings ShootSound;

        //Reload
        [SerializeField]
        public SoundSettings ReloadSound;


        //NoAmmo
        [SerializeField]
        public SoundSettings NoAmmoSound;


        #endregion

        //Debugging
        public TextMeshProUGUI currentAmmoText, magazineSizeText, currentTotalAmmoText;


        #region Aim
        //AIM
        public bool AimActive;
        public bool AimAtCursor;
        [SerializeField]
        private Transform AimTarget;
        [SerializeField]
        private float RotateSpeed;
        [SerializeField]
        [Tooltip("needs to have a parent , its the distence between the weapon and the parent")]
        public float RotationRadius;

        #endregion
        #region events
        //EVENTS
        public UnityEvent OnShoot;
        public UnityEvent OnReloadStart;
        public UnityEvent OnReloadEnd;

        #endregion
        [SerializeField]
        private float noAmmoSoundCooldown;
        private float noAmmoSoundTimer;

        private void Start()
        {

            if (weaponData != null)
            {
                LoadWeaponData();
            }
            UpdateFirePos();

            if (!UnlimitedAmmo)
                currentTotalAmmo = MaxAmmo;
            if (!UnlimitedMagazineSize)
                currentAmmo = MagazineSize;

        }


        float timer;


        public void Update()
        {


            #region Reload

            if (Input.GetKeyDown(ReloadKey))
            {
                TryReload();
            }

            #endregion



            #region unlimited ammo

            if (UnlimitedAmmo)
                currentTotalAmmo = int.MaxValue;
            #endregion

            #region Timers

            if (NotShootingTimer > RecoilRecoveryTime)
                recoilfactor = 0;

            if (timer > 0)
            {
                ShootingTimer += Time.deltaTime;
                recoilfactor += Time.deltaTime;
                NotShootingTimer = 0;
            }
            else if (timer < -0.1f)
            {
                ShootingTimer = 0;
                NotShootingTimer += Time.deltaTime;
            }


            #endregion

            #region HundleInputs

            if (UsePlayerInputs)
            {

                FireInputHold = Input.GetKey(FireKey);
                FireInputDown = Input.GetKeyDown(FireKey);

            }

            #endregion

            #region HundleShooting
            switch (FireMode)
            {

                case FireModes.single:
                    timer -= Time.deltaTime;

                    CanShoot = timer <= 0;
                    if (FireInputDown && CanShoot)
                    {
                        CanShoot = false;
                        #region shoot

                        Shoot();

                        #endregion
                        timer = FireRate;

                    }
                    break;
                case FireModes.auto:

                    timer -= Time.deltaTime;
                    CanShoot = timer <= 0;




                    if (FireInputHold && CanShoot)
                    {
                        CanShoot = false;
                        #region shoot

                        Shoot();



                        #endregion
                        timer = FireRate;

                    }

                    break;

                case FireModes.burst:

                    timer -= Time.deltaTime;
                    CanShoot = timer <= 0;

                    if ((FireInputDown || (FireInputHold && AutoShooting)) && CanShoot)
                    {
                        CanShoot = false;
                        timer = 10000;
                        StartCoroutine(IBurst());

                    }

                    break;


                case FireModes.Spread:


                    timer -= Time.deltaTime;
                    CanShoot = timer <= 0;
                    if ((FireInputDown || (FireInputHold && AutoShooting)) && CanShoot)
                    {
                        CanShoot = false;
                        #region shoot

                        SpreadShot(ProjectilesCount);



                        #endregion
                        timer = FireRate;

                    }




                    break;
            }

            #endregion
            #region Aim

            if (AimActive)
            {
                Vector3 target = Vector2.right;
                Vector3 HolderPos = Camera.main.WorldToScreenPoint(transform.position);

                if (AimAtCursor)
                    target = Input.mousePosition;
                else if (AimTarget != null)
                    target = AimTarget.position;

                if (transform.parent != null)
                {
                    HolderPos = Camera.main.WorldToScreenPoint(transform.parent.position);
                    transform.localPosition = new Vector2(target.x - HolderPos.x, target.y - HolderPos.y).normalized * RotationRadius;

                }


                Vector3 AimDirection = new Vector2(target.x - HolderPos.x, target.y - HolderPos.y);
                float gunangle = Mathf.Atan2(AimDirection.y, AimDirection.x) * Mathf.Rad2Deg;


                if (target.magnitude > RotationRadius)
                    if (target.x < Camera.main.WorldToScreenPoint(transform.position).x)
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(180f, 0f, -gunangle)), Time.deltaTime * RotateSpeed);
                    }
                    else
                    {
                        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0f, 0f, gunangle)), Time.deltaTime * RotateSpeed);

                    }




            }

            #endregion

            noAmmoSoundTimer -= Time.deltaTime;

            #region ammotexts



            if (currentAmmoText != null)
                currentAmmoText.text = currentAmmo.ToString();
            if (currentTotalAmmoText != null)
                currentTotalAmmoText.text = "/ " + currentTotalAmmo.ToString();
            if (magazineSizeText != null)
                magazineSizeText.text = MagazineSize.ToString();





            #endregion











        }

        private void Shoot()
        {
            if (Reloading)
            {

                return;
            }


            int ammoparshot;
            int numberofprojectilesneeded;
            int numberofavaibleprojectile = 1;

            if (!UnlimitedMagazineSize || !UnlimitedAmmo)
            {

                if (FireMode == FireModes.burst)
                    numberofprojectilesneeded = Points.Count * BurstCount;
                else
                    numberofprojectilesneeded = Points.Count;



                if (countEachProjectile)
                {
                    ammoparshot = numberofprojectilesneeded;
                }
                else
                {
                    ammoparshot = ammoPerShot;
                    numberofavaibleprojectile = currentAmmo * numberofprojectilesneeded / ammoPerShot;
                    numberofavaibleprojectile = Mathf.Clamp(numberofavaibleprojectile, 1, numberofprojectilesneeded);


                }

                if (!UnlimitedMagazineSize)
                {

                    if (currentAmmo < ammoparshot && !PartialAmmoFire)
                    {
                        HundleNoAmmo();
                        return;
                    }
                    if (PartialAmmoFire && currentAmmo <= 0)
                    {

                        HundleNoAmmo();
                        return;
                    }

                    if (!PartialAmmoFire || !countEachProjectile)
                    {
                        currentAmmo = Mathf.Clamp(currentAmmo - ammoparshot, 0, currentAmmo - ammoparshot);

                    }
                }
                else
                {
                    if (currentTotalAmmo <= 0)
                    {

                        HundleNoAmmo();
                        return;
                    }
                    numberofavaibleprojectile = currentTotalAmmo * numberofprojectilesneeded / ammoPerShot;
                    numberofavaibleprojectile = Mathf.Clamp(numberofavaibleprojectile, 1, numberofprojectilesneeded);
                    currentTotalAmmo -= ammoparshot;
                }
            }

            OnShoot.Invoke();
            PlaySound(this, ref ShootSound);
            foreach (FirePoint point in Points)
            {
                if (!UnlimitedMagazineSize)
                {

                    if (PartialAmmoFire && countEachProjectile && currentAmmo <= 0)
                    {
                        HundleNoAmmo();
                        break;
                    }
                    if (PartialAmmoFire && !countEachProjectile && numberofavaibleprojectile <= 0)
                    {
                        HundleNoAmmo();

                        break;
                    }
                }
                else
                {

                    if (PartialAmmoFire && countEachProjectile && currentTotalAmmo <= 0)
                    {
                        HundleNoAmmo();

                        break;
                    }
                    if (PartialAmmoFire && !countEachProjectile && numberofavaibleprojectile <= 0)
                    {
                        HundleNoAmmo();

                        break;
                    }
                }

                Vector3 Position = point.GetRelativePosition();
                Vector3 Direction = point.GetReletiveDirection();

                float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
                float _recoilangle = 0;

                if (Recoil)
                    _recoilangle = Random.Range(-RecoilForce, RecoilForce);

                GameObject bullet = Instantiate(projectile.gameObject, Position, Quaternion.Euler(0, 0, angle + _recoilangle * RecoilCurve.Evaluate(recoilfactor)));
                if (PartialAmmoFire)
                    if (countEachProjectile)
                        if (UnlimitedMagazineSize)
                            currentTotalAmmo--;
                        else
                            currentAmmo--;
                    else
                    {
                        numberofavaibleprojectile--;
                    }
                Projectile _projectile = bullet.GetComponent<Projectile>();
                _projectile.speed = speed;
                _projectile.lifetime = lifetime;
                _projectile.type = projectileType;
                if (projectileType == Projectile.ProjectileType.Ballistic)
                    _projectile.GravityModifier = ProjectileGravityModifier;
                _projectile.MaxDistence = MaxDistence;
                _projectile.DestroyOnDistenceTravled = DestroyOnDistenceTravled;
                _projectile.UnlimitedLifeTime = UnlimitedLifeTime;
                _projectile.Damege = Damege;
                _projectile.DamegeToStructure = DamegeToStructure;
                _projectile.Target = ProjectileTarget;
                _projectile.FollowStrength = FollowStrength;
                if (ProjectileImpactEffect != null)
                    _projectile.OnDestroyEffect = ProjectileImpactEffect;



            }
        }
        private void HundleNoAmmo()
        {
            if (AutoReload)
            {
                TryReload();
            }
            else if (noAmmoSoundTimer <= 0)
            {
                PlaySound(this, ref NoAmmoSound);
                noAmmoSoundTimer = noAmmoSoundCooldown;
            }
        }

        private void TryReload()
        {
            if (Reloading || currentAmmo == MagazineSize)
            {
                return;
            }
            if (currentTotalAmmo <= 0)
            {
                PlaySound(this, ref NoAmmoSound);
                return;
            }


            Reloading = true;
            OnReloadStart.Invoke();
            PlaySound(this, ref ReloadSound);
            StartCoroutine(IReload());
        }

        private IEnumerator IReload()
        {
            yield return new WaitForSeconds(ReloadTime);
            if (UnlimitedAmmo)
            {
                currentAmmo = MagazineSize;
            }
            else
            {

                int neededammo = MagazineSize - currentAmmo;
                if (neededammo > 0)
                {

                    if (currentTotalAmmo >= neededammo)
                    {
                        currentTotalAmmo -= neededammo;
                        currentAmmo = MagazineSize;

                    }
                    else if (currentTotalAmmo > 0)
                    {
                        currentAmmo += currentTotalAmmo;
                        currentTotalAmmo = 0;
                    }
                }
            }

            OnReloadEnd.Invoke();
            Reloading = false;
        }

        private void SpreadShot(int projectilesNumber)
        {
            if (Reloading)
                return;

            int ammoparshot;
            int numberofprojectilesneeded;
            int numberofavaibleprojectile = 1;


            numberofprojectilesneeded = Points.Count * ProjectilesCount;


            if (countEachProjectile)
            {
                ammoparshot = numberofprojectilesneeded;
            }
            else
            {
                ammoparshot = ammoPerShot;
                if (ammoPerShot == 0)
                    ammoPerShot = 1;
                numberofavaibleprojectile = currentAmmo * numberofprojectilesneeded / ammoPerShot;
                numberofavaibleprojectile = Mathf.Clamp(numberofavaibleprojectile, 1, numberofprojectilesneeded);


            }
            if (!UnlimitedMagazineSize)
            {

                if (currentAmmo < ammoparshot && !PartialAmmoFire)
                {

                    HundleNoAmmo();
                    return;
                }
                if (PartialAmmoFire && currentAmmo <= 0)
                {

                    HundleNoAmmo();
                    return;
                }

                if (!PartialAmmoFire || !countEachProjectile)
                {
                    currentAmmo = Mathf.Clamp(currentAmmo - ammoparshot, 0, currentAmmo - ammoparshot);

                }
            }
            else
            {
                if (currentTotalAmmo <= 0)
                {

                    HundleNoAmmo();
                    return;
                }
                if (ammoPerShot == 0)
                    ammoPerShot = 1;
                numberofavaibleprojectile = currentTotalAmmo * numberofprojectilesneeded / ammoPerShot;
                numberofavaibleprojectile = Mathf.Clamp(numberofavaibleprojectile, 1, numberofprojectilesneeded);
                currentTotalAmmo -= ammoparshot;
            }



            foreach (FirePoint point in Points)
            {



                Vector3 Position = point.GetRelativePosition();
                Vector3 Direction = point.GetReletiveDirection();

                float angle = Mathf.Atan2(Direction.y, Direction.x) * Mathf.Rad2Deg;
                for (int j = 0; j < projectilesNumber; j++)
                {

                    if (!UnlimitedMagazineSize)
                    {

                        if (PartialAmmoFire && countEachProjectile && currentAmmo <= 0)
                        {
                            HundleNoAmmo();
                            return;
                        }
                        if (PartialAmmoFire && !countEachProjectile && numberofavaibleprojectile <= 0)
                        {
                            HundleNoAmmo();
                            return;
                        }
                    }
                    else
                    {

                        if (PartialAmmoFire && countEachProjectile && currentTotalAmmo <= 0)
                        {
                            HundleNoAmmo();
                            return;
                        }
                        if (PartialAmmoFire && !countEachProjectile && numberofavaibleprojectile <= 0)
                        {
                            HundleNoAmmo();
                            return;
                        }
                    }


                    float errorangle = 0;
                    if (RandomSpread)
                    {

                        errorangle = Random.Range(MinAngle, MaxAngle);
                    }
                    else
                    {
                        if (projectilesNumber > 1)
                        {
                            float _stepangle = (MaxAngle - MinAngle) / (projectilesNumber - 1);

                            errorangle = MinAngle + j * _stepangle;
                        }

                    }

                    if (PartialAmmoFire)
                    {

                        if (countEachProjectile)
                        {

                            if (UnlimitedMagazineSize)
                                currentTotalAmmo--;
                            else
                                currentAmmo--;
                        }
                        else
                        {
                            numberofavaibleprojectile--;
                        }
                    }
                    GameObject bullet = Instantiate(projectile.gameObject, Position, Quaternion.Euler(0, 0, angle + errorangle * SpreadFactor));
                    Projectile _projectile = bullet.GetComponent<Projectile>();
                    _projectile.speed = speed;
                    _projectile.lifetime = lifetime;
                    _projectile.type = projectileType;
                    if (projectileType == Projectile.ProjectileType.Ballistic)
                        _projectile.GravityModifier = ProjectileGravityModifier;
                    _projectile.MaxDistence = MaxDistence;
                    _projectile.DestroyOnDistenceTravled = DestroyOnDistenceTravled;
                    _projectile.UnlimitedLifeTime = UnlimitedLifeTime;
                    _projectile.Damege = Damege;
                    _projectile.DamegeToStructure = DamegeToStructure;
                    _projectile.Target = ProjectileTarget;
                    _projectile.FollowStrength = FollowStrength;
                    if (ProjectileImpactEffect != null)
                        _projectile.OnDestroyEffect = ProjectileImpactEffect;


                }



            }
        }


        private IEnumerator IBurst()
        {


            for (int i = 0; i < BurstCount; i++)
            {
                #region shoot

                Shoot();



                #endregion
                yield return new WaitForSeconds(TimebetweenShots);
            }
            timer = TimeBetweenBursts;

        }

        public void UpdateFirePos()
        {
            Points.Clear();



            if (Muzzles != null)
                foreach (Muzzle muzzle in Muzzles)
                {

                    if (muzzle.MuzzleMode == MuzzleModes.line)
                    {
                        float halfdistence = ((muzzle.PointsNumber - 1) * muzzle.DistenceBetweenPoints) / 2f;
                        Vector2 startpos = Vector2.up * halfdistence;
                        for (int i = 0; i < muzzle.PointsNumber; i++)
                        {

                            Vector2 postion = startpos - Vector2.up * (i * muzzle.DistenceBetweenPoints);
                            Vector3 Rotation = Vector2.Lerp(Vector2.right, postion, muzzle.FireAngleModifier);
                            Points.Add(new FirePoint(postion, Rotation, muzzle.position));
                        }

                    }
                    else if (muzzle.MuzzleMode == MuzzleModes.circle)
                    {
                        float _stepangle;

                        if (muzzle.AutoCalculateStepAngle)
                        {
                            _stepangle = 360f / muzzle.PointsNumber;

                        }
                        else
                        {
                            _stepangle = muzzle.stepangle;
                        }



                        float minangle = -((muzzle.PointsNumber - 1) * muzzle.stepangle) / 2f;
                        for (int i = 0; i < muzzle.PointsNumber; i++)
                        {
                            Vector2 postion = new Vector2(Mathf.Cos(Mathf.Deg2Rad * (minangle + i * _stepangle)), Mathf.Sin(Mathf.Deg2Rad * (minangle + i * _stepangle))) * muzzle.radius;
                            Vector3 Rotation = Vector2.Lerp(postion.normalized, Vector2.right, muzzle.FireAngleModifier);
                            Points.Add(new FirePoint(postion, Rotation, muzzle.position));
                        }
                    }
                }


        }

        private void OnDrawGizmos()
        {
            foreach (var point in Points)
            {
                if (point != null)
                {
                    if (point.Weapon != null)
                        DrawArrow(point.GetRelativePosition(), point.GetReletiveDirection());
                }
            }
        }

        public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(180 + arrowHeadAngle, 0, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(180 - arrowHeadAngle, 0, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }
        public void SaveAsWeaponDataScript(string name, string savepath)
        {
            print("name : " + name);
            if (name == "")
            {
                Debug.LogWarning("Save Failed Weapon Name Unvalid ");
                return;
            }


            WeaponData asset = ScriptableObject.CreateInstance<WeaponData>();


            asset.ChildsPosAndRotations = new WeaponData.PosAndRotation[Muzzles.Length];
            for (int i = 0; i < Muzzles.Length; i++)
            {
                asset.ChildsPosAndRotations[i] = new WeaponData.PosAndRotation(Muzzles[i].position.localPosition, Muzzles[i].position.localRotation);
            }


            asset.Muzzles = Muzzles;

            for (int i = 0; i < asset.Muzzles.Length; i++)
            {
                asset.Muzzles[i].position = null;

            }
            asset.FireMode = FireMode;

            asset.projectile = projectile;

            asset.projectileType = projectileType;
            asset.speed = speed;
            asset.UnlimitedLifeTime = UnlimitedLifeTime;
            asset.lifetime = lifetime;
            asset.DestroyOnDistenceTravled = DestroyOnDistenceTravled;
            asset.MaxDistence = MaxDistence;
            asset.ProjectileGravityModifier = ProjectileGravityModifier;
            asset.ProjectileImpactEffect = ProjectileImpactEffect;


            asset.Recoil = Recoil;
            asset.RecoilForce = RecoilForce;
            asset.RecoilRecoveryTime = RecoilRecoveryTime;
            asset.RecoilCurve = RecoilCurve;
            asset.FollowStrength = FollowStrength;



            asset.Damege = Damege;
            asset.DamegeToStructure = DamegeToStructure;
            //auto && Single
            asset.FireRate = FireRate;


            //burst
            asset.BurstCount = BurstCount;
            asset.TimebetweenShots = TimebetweenShots;
            asset.TimeBetweenBursts = TimeBetweenBursts;


            //Spread
            asset.ProjectilesCount = ProjectilesCount;
            asset.RandomSpread = RandomSpread;
            asset.MinAngle = MinAngle;
            asset.MaxAngle = MaxAngle;
            asset.SpreadFactor = SpreadFactor;

            asset.AutoShooting = AutoShooting;



            //AMMO 
            asset.UnlimitedAmmo = UnlimitedAmmo;
            asset.MaxAmmo = MaxAmmo;
            asset.UnlimitedMagazineSize = UnlimitedMagazineSize;
            asset.MagazineSize = MagazineSize;
            asset.ReloadTime = ReloadTime;
            asset.PartialAmmoFire = PartialAmmoFire;
            asset.AutoReload = AutoReload;


            asset.countEachProjectile = countEachProjectile;
            asset.ammoPerShot = ammoPerShot;



            AssetDatabase.CreateAsset(asset, savepath + name + ".asset");
            AssetDatabase.SaveAssets();
            weaponData = asset;

        }
        public void LoadWeaponData()
        {
            if (weaponData == null)
                return;



            if (weaponData.Muzzles.Length != Muzzles.Length)
                Muzzles = new Muzzle[weaponData.Muzzles.Length];

            for (int i = 0; i < weaponData.Muzzles.Length; i++)
            {


                Muzzles[i].MuzzleMode = weaponData.Muzzles[i].MuzzleMode;
                Muzzles[i].PointsNumber = weaponData.Muzzles[i].PointsNumber;
                Muzzles[i].DistenceBetweenPoints = weaponData.Muzzles[i].DistenceBetweenPoints;
                Muzzles[i].radius = weaponData.Muzzles[i].radius;
                Muzzles[i].AutoCalculateStepAngle = weaponData.Muzzles[i].AutoCalculateStepAngle;
                Muzzles[i].stepangle = weaponData.Muzzles[i].stepangle;
                Muzzles[i].FireAngleModifier = weaponData.Muzzles[i].FireAngleModifier;

            }




            FireMode = weaponData.FireMode;

            projectile = weaponData.projectile;

            projectileType = weaponData.projectileType;
            speed = weaponData.speed;
            UnlimitedLifeTime = weaponData.UnlimitedLifeTime;
            lifetime = weaponData.lifetime;
            DestroyOnDistenceTravled = weaponData.DestroyOnDistenceTravled;
            MaxDistence = weaponData.MaxDistence;
            ProjectileGravityModifier = weaponData.ProjectileGravityModifier;
            ProjectileImpactEffect = weaponData.ProjectileImpactEffect;
            Damege = weaponData.Damege;
            DamegeToStructure = weaponData.DamegeToStructure;
            FollowStrength = weaponData.FollowStrength;

            Recoil = weaponData.Recoil;
            RecoilForce = weaponData.RecoilForce;
            RecoilRecoveryTime = weaponData.RecoilRecoveryTime;
            RecoilCurve = weaponData.RecoilCurve;

            //auto && Single
            FireRate = weaponData.FireRate;


            //burst
            BurstCount = weaponData.BurstCount;
            TimebetweenShots = weaponData.TimebetweenShots;
            TimeBetweenBursts = weaponData.TimeBetweenBursts;


            //Spread
            ProjectilesCount = weaponData.ProjectilesCount;
            RandomSpread = weaponData.RandomSpread;
            MinAngle = weaponData.MinAngle;
            MaxAngle = weaponData.MaxAngle;
            SpreadFactor = weaponData.SpreadFactor;

            AutoShooting = weaponData.AutoShooting;



            //AMMO 
            UnlimitedAmmo = weaponData.UnlimitedAmmo;
            MaxAmmo = weaponData.MaxAmmo;
            UnlimitedMagazineSize = weaponData.UnlimitedMagazineSize;
            MagazineSize = weaponData.MagazineSize;
            ReloadTime = weaponData.ReloadTime;
            PartialAmmoFire = weaponData.PartialAmmoFire;
            AutoReload = weaponData.AutoReload;


            countEachProjectile = weaponData.countEachProjectile;
            ammoPerShot = weaponData.ammoPerShot;



        }


        public void CreateChildAndAssigneThem(Weapon weapon)
        {
            if (weapon.weaponData == null)
                return;




            int i = 0;
            foreach (WeaponData.PosAndRotation posAndRotation in weapon.weaponData.ChildsPosAndRotations)
            {
                GameObject muzzle = new GameObject(weaponData.name + " Muzzle " + (i + 1));
                muzzle.transform.parent = this.transform;
                muzzle.transform.localPosition = posAndRotation.Position;
                muzzle.transform.localRotation = posAndRotation.Rotation;


                weapon.Muzzles[i].position = muzzle.transform;

                i++;
            }

        }
        public static void PlaySound(MonoBehaviour caller, ref SoundSettings soundsettings)
        {
            if (soundsettings.Clip == null || (!soundsettings.CreateAudioSource && soundsettings.Source == null))
                return;


            AudioSource source;

            if (!soundsettings.CreateAudioSource)
                source = soundsettings.Source;
            else
            {
                source = caller.AddComponent<AudioSource>();
                soundsettings.Source = source;
                soundsettings.CreateAudioSource = false;
            }

            source.volume = soundsettings.volume;
            source.clip = soundsettings.Clip;
            source.playOnAwake = false;
            source.loop = false;
            if (soundsettings.DelayTime > 0)
            {
                caller.StartCoroutine(Iplaysound(soundsettings.DelayTime, source));
            }
            else
            {
                source.Play();

            }



        }
        private static IEnumerator Iplaysound(float delay, AudioSource audioSource)
        {
            yield return new WaitForSeconds(delay);
            audioSource.Play();
        }



        // Utilities




        public void SetProjectileTarget(Transform target)
        {
            ProjectileTarget = target;

        }

        public void AddAmmo(int amount)
        {
            currentTotalAmmo += amount;
        }

        public IEnumerator ActiveInfinityAmmo(float duration)
        {
            int currentammo = currentTotalAmmo;
            UnlimitedAmmo = true;
            yield return new WaitForSeconds(duration);
            UnlimitedAmmo = false;
            currentTotalAmmo = currentammo;
        }



    }
}

