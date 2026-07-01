using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using CanavarZindanlari.Combat;
using CanavarZindanlari.Core;
using CanavarZindanlari.Data;
using CanavarZindanlari.Economy;
using CanavarZindanlari.Gameplay;
using CanavarZindanlari.UI;
using UnityEngine.SceneManagement;

namespace CanavarZindanlari.Editor
{
    public static class BattleSceneSetup
    {
        private const string ScenePath   = "Assets/Scenes/BattleScene.unity";
        private const string MonsterPath = "Assets/Scripts/Monsters";

        // ── Ana menü ──────────────────────────────────────────────────────────

        [MenuItem("CanavarZindanlari/Kurulum/1 — Test Canavarları Oluştur")]
        public static void CreateTestMonsters()
        {
            CreateMonster("AtesGoblin_F",  "Ateş Goblin",    Element.Ates,   Archetype.Saldirgan, Rarity.F, 1,  hp: 20, atk: 35, def: 20, spd: 25);
            CreateMonster("BuzEjderi_C",   "Buz Ejderi",     Element.Su,     Archetype.Saldirgan, Rarity.C, 1,  hp: 30, atk: 52, def: 30, spd: 37);
            CreateMonster("TasGolem_B",    "Taş Golem",      Element.Toprak, Archetype.Tank,      Rarity.B, 1,  hp: 55, atk: 37, def: 64, spd: 27);
            CreateMonster("HavaPeri_D",    "Hava Perisi",    Element.Hava,   Archetype.Destekci,  Rarity.D, 1,  hp: 34, atk: 24, def: 36, spd: 26);
            CreateMonster("AtesEjderi_A",  "Ateş Ejderi",   Element.Ates,   Archetype.Saldirgan, Rarity.A, 1,  hp: 43, atk: 75, def: 43, spd: 53);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Setup] 5 test canavarı oluşturuldu → Assets/Scripts/Monsters/");
        }

        [MenuItem("CanavarZindanlari/Kurulum/2 — BattleScene Oluştur")]
        public static void CreateBattleScene()
        {
            if (File.Exists(Application.dataPath + "/../" + ScenePath))
            {
                if (!EditorUtility.DisplayDialog("BattleScene Var",
                        "BattleScene zaten mevcut. Üzerine yaz?", "Evet", "Hayır"))
                    return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            var camGo = new GameObject("Main Camera");
            var cam   = camGo.AddComponent<Camera>();
            cam.clearFlags          = CameraClearFlags.SolidColor;
            cam.backgroundColor     = new Color(0.08f, 0.08f, 0.12f);
            cam.orthographic        = true;
            cam.orthographicSize    = 5f;
            camGo.tag               = "MainCamera";

            // ── Managers root ─────────────────────────────────────────────────

            var managersRoot = new GameObject("[Managers]");

            // GameManager
            var gmGo = new GameObject("GameManager");
            gmGo.transform.SetParent(managersRoot.transform);
            gmGo.AddComponent<GameManager>();
            var economy = gmGo.AddComponent<EconomyManager>();
            gmGo.AddComponent<IAPManager>();
            gmGo.AddComponent<AdManager>();

            // Gameplay systems
            var gameplayGo = new GameObject("GameplaySystems");
            gameplayGo.transform.SetParent(managersRoot.transform);
            gameplayGo.AddComponent<RevengeManager>();
            gameplayGo.AddComponent<WantedBoard>();

            // ── CombatManager ─────────────────────────────────────────────────

            var combatGo = new GameObject("CombatManager");
            var combat   = combatGo.AddComponent<CombatManager>();

            // SkillData atama — Resources/Skills klasöründen yükle
            var normalSkill = AssetDatabase.LoadAssetAtPath<SkillData>(
                "Assets/Resources/Skills/Skill_NormalAttack.asset");
            var heavySkill = AssetDatabase.LoadAssetAtPath<SkillData>(
                "Assets/Resources/Skills/Skill_HeavyAttack.asset");
            var healSkill = AssetDatabase.LoadAssetAtPath<SkillData>(
                "Assets/Resources/Skills/Skill_Heal.asset");

            if (normalSkill != null && heavySkill != null && healSkill != null)
            {
                var so = new SerializedObject(combat);
                var prop = so.FindProperty("_playerSkills");
                prop.arraySize = 3;
                prop.GetArrayElementAtIndex(0).objectReferenceValue = normalSkill;
                prop.GetArrayElementAtIndex(1).objectReferenceValue = heavySkill;
                prop.GetArrayElementAtIndex(2).objectReferenceValue = healSkill;
                so.ApplyModifiedProperties();
                Debug.Log("[Setup] SkillData'lar CombatManager'a atandı.");
            }
            else
            {
                Debug.LogWarning("[Setup] SkillData bulunamadı — CombatManager'a manuel atama yapın.");
            }

            // ── Enemy Display ─────────────────────────────────────────────────

            var enemyDisplayGo  = new GameObject("EnemyDisplay");
            var enemyRenderer   = enemyDisplayGo.AddComponent<SpriteRenderer>();
            enemyRenderer.sortingOrder = 0;
            enemyDisplayGo.transform.position = Vector3.up * 1.5f;
            enemyDisplayGo.transform.localScale = Vector3.one * 4f;

            // VideoPlayer — saldırı animasyonu için (Kling AI videoları buraya)
            var vp = enemyDisplayGo.AddComponent<VideoPlayer>();
            vp.renderMode     = VideoRenderMode.MaterialOverride;
            vp.targetMaterialProperty = "_MainTex";
            vp.isLooping      = true;
            vp.playOnAwake    = false;

            // ── UI ────────────────────────────────────────────────────────────

            var uiGo = new GameObject("BattleRewardUI");
            var doc  = uiGo.AddComponent<UnityEngine.UIElements.UIDocument>();

            var panelSettings = AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.PanelSettings>(
                "Assets/Settings/PanelSettings.asset");
            if (panelSettings != null)
                doc.panelSettings = panelSettings;

            var uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/UI/BattleRewardScreen.uxml");
            if (uxmlAsset != null)
                doc.visualTreeAsset = uxmlAsset;
            else
                Debug.LogWarning("[Setup] BattleRewardScreen.uxml bulunamadı.");

            // ── Combat Bootstrap + HUD ─────────────────────────────────────────

            var bootstrapGo = new GameObject("CombatBootstrap");
            bootstrapGo.AddComponent<CombatBootstrap>();

            var hudGo = new GameObject("BattleHUD");
            hudGo.AddComponent<BattleHUD>();

            var rewardScreen = uiGo.AddComponent<BattleRewardScreen>();
            var rsSo = new SerializedObject(rewardScreen);
            rsSo.FindProperty("_combat").objectReferenceValue = combat;
            rsSo.ApplyModifiedProperties();

            // RevengeManager → CombatManager bağlantısı
            var revengeComp = gameplayGo.GetComponent<RevengeManager>();
            if (revengeComp != null)
            {
                var rvSo = new SerializedObject(revengeComp);
                rvSo.FindProperty("_combat").objectReferenceValue = combat;
                rvSo.ApplyModifiedProperties();
            }

            // WantedBoard → CombatManager bağlantısı
            var wantedComp = gameplayGo.GetComponent<WantedBoard>();
            if (wantedComp != null)
            {
                var wbSo = new SerializedObject(wantedComp);
                wbSo.FindProperty("_combat").objectReferenceValue = combat;
                wbSo.ApplyModifiedProperties();
            }

            // ── EconomyManager → Revenge/Wanted bağlantıları ─────────────────

            var econSo = new SerializedObject(economy);
            econSo.FindProperty("_revenge").objectReferenceValue      = revengeComp;
            econSo.FindProperty("_wantedBoard").objectReferenceValue  = wantedComp;
            econSo.ApplyModifiedProperties();

            // ── Kaydet ────────────────────────────────────────────────────────

            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.Refresh();

            // Build Settings'e ekle
            AddSceneToBuildSettings(ScenePath);

            Debug.Log($"[Setup] BattleScene oluşturuldu: {ScenePath}");
            EditorUtility.DisplayDialog("Kurulum Tamamlandı",
                "BattleScene hazır!\n\nSıradaki adımlar:\n" +
                "• Önce '1 — Test Canavarları Oluştur' çalıştır (henüz yapmadıysan)\n" +
                "• EnemyDisplay > SpriteRenderer'a test canavar sprite'ı ata\n" +
                "• Play modunda test et",
                "Tamam");
        }

        [MenuItem("CanavarZindanlari/Kurulum/Tümünü Çalıştır (1+2)")]
        public static void RunAll()
        {
            CreateTestMonsters();
            CreateBattleScene();
        }

        // ── Yardımcılar ───────────────────────────────────────────────────────

        private static MonsterData CreateMonster(
            string assetName, string displayName,
            Element el, Archetype arch, Rarity rarity, int evolutionStage,
            int hp, int atk, int def, int spd)
        {
            string dir  = MonsterPath;
            if (!AssetDatabase.IsValidFolder(dir))
                AssetDatabase.CreateFolder("Assets/Scripts", "Monsters");

            string path = $"{dir}/{assetName}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<MonsterData>(path);
            if (existing != null)
            {
                Debug.Log($"[Setup] Var — atlandı: {assetName}");
                return existing;
            }

            var data = ScriptableObject.CreateInstance<MonsterData>();
            data.MonsterId     = assetName;
            data.DisplayName   = displayName;
            data.Element       = el;
            data.Archetype     = arch;
            data.BaseRarity    = rarity;
            data.EvolutionStage = evolutionStage;
            data.BaseHP        = hp;
            data.BaseATK       = atk;
            data.BaseDEF       = def;
            data.BaseSPD       = spd;

            AssetDatabase.CreateAsset(data, path);
            Debug.Log($"[Setup] Oluşturuldu: {displayName} ({rarity} {arch})");
            return data;
        }

        private static void AddSceneToBuildSettings(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes;
            foreach (var s in scenes)
                if (s.path == scenePath) return;

            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            System.Array.Copy(scenes, newScenes, scenes.Length);
            newScenes[scenes.Length] = new EditorBuildSettingsScene(scenePath, true);
            EditorBuildSettings.scenes = newScenes;
            Debug.Log($"[Setup] Build Settings'e eklendi: {scenePath}");
        }
    }
}
