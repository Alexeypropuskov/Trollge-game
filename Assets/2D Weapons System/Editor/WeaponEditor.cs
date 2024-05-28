using UnityEditor;
using UnityEngine;
using WeaponSystem;

using WeaponSystemUtilities;

namespace WeaponEditor
{
    [CustomEditor(typeof(Weapon))]

    public class WeaponEditor : Editor
    {
        private string WeaponName;
        private FirePoint[] Points;
        private SerializedProperty Muzzles;
        private bool eventsFoldOut = false;

        public string ScriptableObjectSavePath = "Assets/";

        private void OnEnable()
        {

        }
        public override void OnInspectorGUI()
        {

            Weapon weapon = target as Weapon;




            Texture TitelWeaponmaker = (Texture)AssetDatabase.LoadAssetAtPath("Assets/2D Weapons System/Editor/Titel(Weapon maker).png", typeof(Texture));
            GUILayout.Box(TitelWeaponmaker);



            Texture TitleSave = (Texture)AssetDatabase.LoadAssetAtPath("Assets/2D Weapons System/Editor/title(Save & Load).png", typeof(Texture));
            GUILayout.Box(TitleSave);
            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Name");
            WeaponName = EditorGUILayout.TextField(WeaponName);


            if (GUILayout.Button("Save"))
            {

                weapon.SaveAsWeaponDataScript(WeaponName, ScriptableObjectSavePath);

            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label("Data");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("weaponData"), GUIContent.none);
            serializedObject.ApplyModifiedProperties();
            if (GUILayout.Button("   X  "))
            {
                weapon.weaponData = null;
                serializedObject.ApplyModifiedProperties();

            }
            EditorGUILayout.EndHorizontal();


            if (weapon.weaponData != null)
                weapon.LoadWeaponData();
            GUI.contentColor = Color.red;
            new GUIContent("A Button", "This is the tooltip");
            if (weapon.weaponData != null)
                if (GUILayout.Button("Recreate Muzzles Positions Transform"))
                {

                    weapon.CreateChildAndAssigneThem(weapon);

                }
            serializedObject.ApplyModifiedProperties();
            GUI.contentColor = Color.white;


            if (weapon.weaponData != null)
                GUI.enabled = false;



            Texture TitleMuzzlePos = (Texture)AssetDatabase.LoadAssetAtPath("Assets/2D Weapons System/Editor/title(Muzzle Positions).png", typeof(Texture));
            GUILayout.Box(TitleMuzzlePos);






            Muzzles = serializedObject.FindProperty("Muzzles");
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Muzzles", GUILayout.Width(100));
            GUI.enabled = false;
            EditorGUILayout.IntField(Muzzles.arraySize, GUILayout.Width(30));
            GUI.enabled = true;

            if (weapon.weaponData != null)
                GUI.enabled = false;
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {

                Muzzles.InsertArrayElementAtIndex(Muzzles.arraySize);
            }
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                if (Muzzles.arraySize > 0)
                    Muzzles.DeleteArrayElementAtIndex(Muzzles.arraySize - 1);
            }
            GUILayout.Space(100);
            if (GUILayout.Button("X", GUILayout.Width(20)))
            {

                Muzzles.ClearArray();
            }


            serializedObject.ApplyModifiedProperties();

            GUILayout.EndHorizontal();

            for (int i = 0; i < Muzzles.arraySize; i++)
            {
                GUILayout.Label("Muzzle " + (i + 1) + "-------------------------------------------------------");

                SerializedProperty muzzle = serializedObject.FindProperty("Muzzles").GetArrayElementAtIndex(i);
                if (weapon.weaponData != null)
                    GUI.enabled = true;
                EditorGUILayout.PropertyField(muzzle.FindPropertyRelative("position"));
                if (weapon.weaponData != null)
                    GUI.enabled = false;
                EditorGUILayout.PropertyField(muzzle.FindPropertyRelative("MuzzleMode"));
                EditorGUILayout.PropertyField(muzzle.FindPropertyRelative("PointsNumber"));
                serializedObject.ApplyModifiedProperties();
                switch (weapon.Muzzles[i].MuzzleMode)
                {
                    case Weapon.MuzzleModes.line:

                        EditorGUILayout.PropertyField(muzzle.FindPropertyRelative("DistenceBetweenPoints"));

                        EditorGUILayout.PropertyField(muzzle.FindPropertyRelative("FireAngleModifier"));
                        serializedObject.ApplyModifiedProperties();

                        break;


                    case Weapon.MuzzleModes.circle:


                        EditorGUILayout.PropertyField(muzzle.FindPropertyRelative("radius"));
                        EditorGUILayout.PropertyField(muzzle.FindPropertyRelative("AutoCalculateStepAngle"));
                        if (!weapon.Muzzles[i].AutoCalculateStepAngle)
                        {
                            EditorGUILayout.PropertyField(muzzle.FindPropertyRelative("stepangle"));
                        }
                        EditorGUILayout.PropertyField(muzzle.FindPropertyRelative("FireAngleModifier"));
                        serializedObject.ApplyModifiedProperties();


                        break;

                }
                GUILayout.Label("-------------------------------------------------------");
            }

            serializedObject.ApplyModifiedProperties();


            Texture TitleFireModes = (Texture)AssetDatabase.LoadAssetAtPath("Assets/2D Weapons System/Editor/title(Fire Modes).png", typeof(Texture));
            GUILayout.Box(TitleFireModes);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("FireMode"));

            serializedObject.ApplyModifiedProperties();






            switch (weapon.FireMode)
            {

                case Weapon.FireModes.single:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FireRate"));
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Recoil"));
                    serializedObject.ApplyModifiedProperties();
                    if (weapon.Recoil)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Recoil Force", GUILayout.Width(80));
                        weapon.RecoilForce = EditorGUILayout.FloatField(weapon.RecoilForce, GUILayout.Width(50));
                        GUILayout.Label("Recoil Recovery Time", GUILayout.Width(145));
                        weapon.RecoilRecoveryTime = EditorGUILayout.FloatField(weapon.RecoilRecoveryTime, GUILayout.Width(50));
                        GUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("RecoilCurve"));
                        serializedObject.ApplyModifiedProperties();

                    }

                    break;
                case Weapon.FireModes.auto:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("FireRate"));
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Recoil"));
                    serializedObject.ApplyModifiedProperties();
                    if (weapon.Recoil)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Recoil Force", GUILayout.Width(80));
                        weapon.RecoilForce = EditorGUILayout.FloatField(weapon.RecoilForce, GUILayout.Width(50));
                        GUILayout.Label("Recoil Recovery Time", GUILayout.Width(145));
                        weapon.RecoilRecoveryTime = EditorGUILayout.FloatField(weapon.RecoilRecoveryTime, GUILayout.Width(50));
                        GUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("RecoilCurve"));
                        serializedObject.ApplyModifiedProperties();

                    }

                    break;

                case Weapon.FireModes.burst:

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("BurstCount"));
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TimebetweenShots"));
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("TimeBetweenBursts"));
                    serializedObject.ApplyModifiedProperties();

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoShooting"));
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("Recoil"));
                    serializedObject.ApplyModifiedProperties();
                    if (weapon.Recoil)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Recoil Force", GUILayout.Width(80));
                        weapon.RecoilForce = EditorGUILayout.FloatField(weapon.RecoilForce, GUILayout.Width(50));
                        GUILayout.Label("Recoil Recovery Time", GUILayout.Width(145));
                        weapon.RecoilRecoveryTime = EditorGUILayout.FloatField(weapon.RecoilRecoveryTime, GUILayout.Width(50));
                        GUILayout.EndHorizontal();
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("RecoilCurve"));
                        serializedObject.ApplyModifiedProperties();

                    }

                    break;


                case Weapon.FireModes.Spread:

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ProjectilesCount"));
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("RandomSpread"));
                    serializedObject.ApplyModifiedProperties();
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MinAngle"));
                    serializedObject.ApplyModifiedProperties();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxAngle"));
                    serializedObject.ApplyModifiedProperties();
                    GUILayout.EndHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("SpreadFactor"));
                    serializedObject.ApplyModifiedProperties();
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoShooting"));
                    serializedObject.ApplyModifiedProperties();
                    if (weapon.AutoShooting)
                    {
                        EditorGUILayout.PropertyField(serializedObject.FindProperty("FireRate"));
                        serializedObject.ApplyModifiedProperties();
                    }
                    GUILayout.EndHorizontal();
                    break;
            }

            Texture TitleProjectileSettings = (Texture)AssetDatabase.LoadAssetAtPath("Assets/2D Weapons System/Editor/title(Projectile Settings).png", typeof(Texture));
            GUILayout.Box(TitleProjectileSettings);

            EditorGUILayout.PropertyField(serializedObject.FindProperty("projectile"));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("projectileType"));
            serializedObject.ApplyModifiedProperties();

            if (weapon.projectileType == Projectile.ProjectileType.Ballistic)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ProjectileGravityModifier"));
                serializedObject.ApplyModifiedProperties();
            }
            if (weapon.projectileType == Projectile.ProjectileType.Follow)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ProjectileTarget"));
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FollowStrength"));
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("speed"));
            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UnlimitedLifeTime"));
            serializedObject.ApplyModifiedProperties();
            if (!weapon.UnlimitedLifeTime)
            {

                EditorGUILayout.PropertyField(serializedObject.FindProperty("lifetime"));
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DestroyOnDistenceTravled"));
            serializedObject.ApplyModifiedProperties();
            if (weapon.DestroyOnDistenceTravled)
            {

                EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxDistence"));
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ProjectileImpactEffect"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("Damege"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DamegeToStructure"));
            serializedObject.ApplyModifiedProperties();





            Texture TitleAmmoSettings = (Texture)AssetDatabase.LoadAssetAtPath("Assets/2D Weapons System/Editor/title(AMMO Settings).png", typeof(Texture));
            GUILayout.Box(TitleAmmoSettings);


            EditorGUILayout.PropertyField(serializedObject.FindProperty("UnlimitedAmmo"));
            serializedObject.ApplyModifiedProperties();
            if (!weapon.UnlimitedAmmo)
            {

                EditorGUILayout.PropertyField(serializedObject.FindProperty("MaxAmmo"));
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UnlimitedMagazineSize"));
            serializedObject.ApplyModifiedProperties();
            if (!weapon.UnlimitedMagazineSize)
            {

                EditorGUILayout.PropertyField(serializedObject.FindProperty("MagazineSize"));
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ReloadTime"));
                serializedObject.ApplyModifiedProperties();
                GUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoReload"));
                serializedObject.ApplyModifiedProperties();
                GUILayout.EndHorizontal();


            }
            if (!weapon.UnlimitedAmmo || !weapon.UnlimitedMagazineSize)
            {

                EditorGUILayout.PropertyField(serializedObject.FindProperty("countEachProjectile"));
                serializedObject.ApplyModifiedProperties();
                if (!weapon.countEachProjectile)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("ammoPerShot"));
                    serializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("PartialAmmoFire"));
                serializedObject.ApplyModifiedProperties();
            }

            GUI.enabled = true;

            Texture TitleSounds = (Texture)AssetDatabase.LoadAssetAtPath("Assets/2D Weapons System/Editor/title(Sounds).png", typeof(Texture));
            GUILayout.Box(TitleSounds);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("ShootSound"));
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("ReloadSound"));
            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("NoAmmoSound"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("noAmmoSoundCooldown"));
            serializedObject.ApplyModifiedProperties();




            Texture TitleAIM = (Texture)AssetDatabase.LoadAssetAtPath("Assets/2D Weapons System/Editor/title(AIM).png", typeof(Texture));
            GUILayout.Box(TitleAIM);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("AimActive"));
            serializedObject.ApplyModifiedProperties();

            if (weapon.AimActive)
            {

                EditorGUILayout.PropertyField(serializedObject.FindProperty("AimAtCursor"));
                serializedObject.ApplyModifiedProperties();
                if (!weapon.AimAtCursor)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("AimTarget"));
                    serializedObject.ApplyModifiedProperties();
                }
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RotateSpeed"));
                serializedObject.ApplyModifiedProperties();
                EditorGUILayout.PropertyField(serializedObject.FindProperty("RotationRadius"));
                serializedObject.ApplyModifiedProperties();




            }

            Texture TitleUnputs = (Texture)AssetDatabase.LoadAssetAtPath("Assets/2D Weapons System/Editor/title(Inputs).png", typeof(Texture));
            GUILayout.Box(TitleUnputs);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("UsePlayerInputs"));
            serializedObject.ApplyModifiedProperties();
            if (weapon.UsePlayerInputs)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("ReloadKey"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("FireKey"));
                serializedObject.ApplyModifiedProperties();

            }
            GUILayout.Label("Debug");
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentAmmoText"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("magazineSizeText"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("currentTotalAmmoText"));
            serializedObject.ApplyModifiedProperties();



            Texture TitleEvents = (Texture)AssetDatabase.LoadAssetAtPath("Assets/2D Weapons System/Editor/title( Events).png", typeof(Texture));
            GUILayout.Box(TitleEvents);
            eventsFoldOut = EditorGUILayout.BeginFoldoutHeaderGroup(eventsFoldOut, "Events");
            if (eventsFoldOut)
            {

                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnShoot"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnReloadStart"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("OnReloadEnd"));
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();




            weapon.UpdateFirePos();




        }






    }
}
