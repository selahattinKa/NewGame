using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using CanavarZindanlari.Combat;
using CanavarZindanlari.Core;
using CanavarZindanlari.Data;
using CanavarZindanlari.Economy;
using CanavarZindanlari.Gameplay;
using CanavarZindanlari.UI;
using CanavarZindanlari.Arena;

// Player Settings'i otomatik uygula — sadece bir kez çalışır
[InitializeOnLoad]
public static class ProjectSettingsApplier
{
    static ProjectSettingsApplier()
    {
        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        PlayerSettings.allowedAutorotateToPortrait           = true;
        PlayerSettings.allowedAutorotateToPortraitUpsideDown = false;
        PlayerSettings.allowedAutorotateToLandscapeLeft      = false;
        PlayerSettings.allowedAutorotateToLandscapeRight     = false;
    }
}

namespace CanavarZindanlari.Editor
{
    public static class BattleSceneSetup
    {
        private const string ScenePath   = "Assets/Scenes/BattleScene.unity";
        private const string MonsterPath = "Assets/Scripts/Monsters";
        private const string ClassPath   = "Assets/Resources/Classes";
        private const string SkillPath   = "Assets/Resources/Skills";

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

            var panelSettings = AssetDatabase.LoadAssetAtPath<UnityEngine.UIElements.PanelSettings>(
                "Assets/Settings/PanelSettings.asset");

            // ── Sınıf Seçim Ekranı ────────────────────────────────────────────
            var selectorGo  = new GameObject("ClassSelectionUI");
            var selectorDoc = selectorGo.AddComponent<UnityEngine.UIElements.UIDocument>();
            if (panelSettings != null) selectorDoc.panelSettings = panelSettings;
            selectorDoc.sortingOrder = 10;

            var selectorUxml = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/UI/ClassSelectionScreen.uxml");
            if (selectorUxml != null)
                selectorDoc.visualTreeAsset = selectorUxml;
            else
                Debug.LogWarning("[Setup] ClassSelectionScreen.uxml bulunamadı.");

            selectorGo.AddComponent<ClassSelectionScreen>();

            // ── Savaş Ödül Ekranı ─────────────────────────────────────────────
            var uiGo = new GameObject("BattleRewardUI");
            var doc  = uiGo.AddComponent<UnityEngine.UIElements.UIDocument>();
            if (panelSettings != null) doc.panelSettings = panelSettings;
            doc.sortingOrder = 5;

            var uxmlAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(
                "Assets/UI/BattleRewardScreen.uxml");
            if (uxmlAsset != null)
                doc.visualTreeAsset = uxmlAsset;
            else
                Debug.LogWarning("[Setup] BattleRewardScreen.uxml bulunamadı.");

            // ── Combat Bootstrap (fallback — DungeonManager olmayan sahneler için) ─

            var bootstrapGo = new GameObject("CombatBootstrap");
            bootstrapGo.AddComponent<CombatBootstrap>();

            var hudGo = new GameObject("BattleHUD");
            hudGo.AddComponent<BattleHUD>();

            var rewardScreen = uiGo.AddComponent<BattleRewardScreen>();
            var rsSo = new SerializedObject(rewardScreen);
            rsSo.FindProperty("_combat").objectReferenceValue = combat;
            rsSo.ApplyModifiedProperties();

            // ── DungeonManager + DungeonMapHUD + MonsterCollection ───────────

            var dungeonGo  = new GameObject("DungeonManager");
            var dungeonMgr = dungeonGo.AddComponent<DungeonManager>();
            dungeonGo.AddComponent<DungeonMapHUD>();
            dungeonGo.AddComponent<MonsterCollection>();

            var dmSo = new SerializedObject(dungeonMgr);
            dmSo.FindProperty("_combat").objectReferenceValue = combat;
            dmSo.ApplyModifiedProperties();

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

        [MenuItem("CanavarZindanlari/Kurulum/3 — Sınıf ve Yetenek Assetleri Oluştur")]
        public static void CreateClassAssets()
        {
            EnsureFolder("Assets/Resources");
            EnsureFolder(ClassPath);
            EnsureFolder(SkillPath);

            // ── Savaşçı ───────────────────────────────────────────────────────
            var w0 = MakeSkill("Savasco_Slot0_KilicDarbesi",   "Kılıç Darbesi",    TargetType.SingleEnemy, cd: 0, mult: 1.0f);
            var w1 = MakeSkill("Savasco_Slot1_KalkanEzme",     "Kalkan Ezme",      TargetType.SingleEnemy, cd: 3, mult: 2.0f,
                hitFx: new[] { MakeFx(StatusEffectType.Stun,   0f, 1) });
            var w2 = MakeSkill("Savasco_Slot2_DemirZirh",      "Demir Zırh",       TargetType.Self,        cd: 5, mult: 0f,
                selfFx: new[] { MakeFx(StatusEffectType.DefMod, 1.5f, 3) });
            var w3 = MakeSkill("Savasco_Slot3_YikimDarbesi",   "Yıkım Darbesi",    TargetType.SingleEnemy, cd: 8, mult: 4.0f,
                hitFx: new[] { MakeFx(StatusEffectType.DefMod, 0.70f, 2) });

            var savasco = MakeClass("Savasco", "Savaşçı", DamageType.Physical,
                hp: 55, atk: 18, def: 40, spd: 20, w0, w1, w2, w3);

            // ── Büyücü ────────────────────────────────────────────────────────
            var m0 = MakeSkill("Buyucu_Slot0_BuyuOku",         "Büyü Oku",         TargetType.SingleEnemy, cd: 0, mult: 1.0f);
            var m1 = MakeSkill("Buyucu_Slot1_ElementPatlamas", "Element Patlaması", TargetType.SingleEnemy, cd: 3, mult: 2.0f,
                hitFx: new[] { MakeFx(StatusEffectType.BurnDoT, 0.05f, 3) });
            var m2 = MakeSkill("Buyucu_Slot2_BuyuZirhi",       "Büyü Zırhı",       TargetType.Self,        cd: 5, mult: 0f,
                selfFx: new[] { MakeFx(StatusEffectType.Shield, 0.25f, 3) });
            var m3 = MakeSkill("Buyucu_Slot3_ElementFirtinas", "Element Fırtınası", TargetType.AllEnemies,  cd: 8, mult: 1.5f,
                hitFx: new[] { MakeFx(StatusEffectType.BurnDoT, 0.05f, 3) });

            var buyucu = MakeClass("Buyucu", "Büyücü", DamageType.Magic,
                hp: 25, atk: 45, def: 10, spd: 28, m0, m1, m2, m3);

            // ── Hırsız ────────────────────────────────────────────────────────
            var t0 = MakeSkill("Hirsiz_Slot0_HizliBicak",      "Hızlı Bıçak",      TargetType.SingleEnemy, cd: 0, mult: 1.0f, crit: 0.35f);
            var t1 = MakeSkill("Hirsiz_Slot1_ZehirHanceri",    "Zehir Hançeri",    TargetType.SingleEnemy, cd: 3, mult: 1.5f,
                hitFx: new[] { MakeFx(StatusEffectType.PoisonDoT, 0.04f, 4), MakeFx(StatusEffectType.AtkMod, 0.80f, 2) });
            var t2 = MakeSkill("Hirsiz_Slot2_GolgeAdimi",      "Gölge Adımı",      TargetType.Self,        cd: 5, mult: 0f,
                selfFx: new[] { MakeFx(StatusEffectType.DefMod, 1.5f, 2), MakeFx(StatusEffectType.GuaranteedCrit, 0f, -1) });
            var t3 = MakeSkill("Hirsiz_Slot3_SuikastFirtinas", "Suikast Fırtınası", TargetType.SingleEnemy, cd: 8, mult: 0.8f,
                multiHit: 5, multiCrit: 0.40f);

            var hirsiz = MakeClass("Hirsiz", "Hırsız", DamageType.Physical,
                hp: 35, atk: 32, def: 15, spd: 45, t0, t1, t2, t3);

            // ── Şifacı ────────────────────────────────────────────────────────
            var h0 = MakeSkill("Sifaci_Slot0_KutsalIsik",      "Kutsal Işın",      TargetType.SingleEnemy, cd: 0, mult: 1.0f);
            var h1 = MakeSkill("Sifaci_Slot1_KutsalDarbe",     "Kutsal Darbe",     TargetType.SingleEnemy, cd: 3, mult: 2.0f,
                healSelf: 0.10f);
            var h2 = MakeSkill("Sifaci_Slot2_BuyukIyilestirm", "Büyük İyileştirme", TargetType.Self,       cd: 5, mult: 0f,
                healSelf: 0.20f);
            var h3 = MakeSkill("Sifaci_Slot3_KorumaAura",      "Koruma Aura",      TargetType.Self,        cd: 8, mult: 0f,
                healSelf: 0.40f,
                selfFx: new[] { MakeFx(StatusEffectType.DamageReduction, 0.75f, 2) });

            var sifaci = MakeClass("Sifaci", "Şifacı", DamageType.Magic,
                hp: 48, atk: 20, def: 28, spd: 25, h0, h1, h2, h3);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Setup] 4 sınıf + 16 yetenek asset'i oluşturuldu.");
            EditorUtility.DisplayDialog("Sınıf Assetleri Hazır",
                "4 sınıf oluşturuldu:\n• Savaşçı\n• Büyücü\n• Hırsız\n• Şifacı\n\n" +
                "BattleScene'deki CombatBootstrap > Player Class alanına istediğin sınıfı sürükle.",
                "Tamam");
        }

        [MenuItem("CanavarZindanlari/Kurulum/4 — Zindan Yöneticisini Ekle")]
        public static void AddDungeonManagerToScene()
        {
            // Mevcut sahnede zaten DungeonManager varsa atla
            var existing = Object.FindFirstObjectByType<DungeonManager>();
            if (existing != null)
            {
                EditorUtility.DisplayDialog("Zaten Mevcut",
                    "Sahnede zaten bir DungeonManager var.", "Tamam");
                return;
            }

            var combat = Object.FindFirstObjectByType<CombatManager>();
            if (combat == null)
            {
                EditorUtility.DisplayDialog("Hata",
                    "Sahnede CombatManager bulunamadı. Önce sahneyi oluştur.", "Tamam");
                return;
            }

            var go  = new GameObject("DungeonManager");
            var mgr = go.AddComponent<DungeonManager>();
            go.AddComponent<DungeonMapHUD>();
            go.AddComponent<MonsterCollection>();

            var so = new SerializedObject(mgr);
            so.FindProperty("_combat").objectReferenceValue = combat;
            so.ApplyModifiedProperties();

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            Debug.Log("[Setup] DungeonManager + DungeonMapHUD eklendi.");
            EditorUtility.DisplayDialog("Zindan Yöneticisi Eklendi",
                "DungeonManager ve DungeonMapHUD sahneye eklendi.\n\n" +
                "Sahneyi kaydet (Ctrl+S) ve Play modunda test et.", "Tamam");
        }

        [MenuItem("CanavarZindanlari/Kurulum/5a — Ekonomi Yöneticisi Ekle")]
        public static void AddEconomyManager()
        {
            var existing = Object.FindFirstObjectByType<CanavarZindanlari.Economy.EconomyManager>(
                FindObjectsInactive.Include);
            if (existing != null)
            {
                // Mevcut olanı seç ve logla
                Selection.activeGameObject = existing.gameObject;
                Debug.Log($"[Setup] EconomyManager bulundu: '{existing.gameObject.name}' " +
                          $"(aktif={existing.gameObject.activeInHierarchy})", existing.gameObject);
                EditorUtility.DisplayDialog("Zaten Mevcut",
                    $"EconomyManager '{existing.gameObject.name}' objesinde var.\n" +
                    "Hiyerarşide seçili hale getirildi — Inspector'dan kontrol et.", "Tamam");
                return;
            }
            var go = new GameObject("EconomyManager");
            go.AddComponent<CanavarZindanlari.Economy.EconomyManager>();
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            EditorUtility.DisplayDialog("Eklendi", "EconomyManager sahneye eklendi. Sahneyi kaydet.", "Tamam");
        }

        [MenuItem("CanavarZindanlari/Kurulum/5 — Arena + Hub Sistemini Ekle")]
        public static void AddArenaToScene()
        {
            // Hub + Pet Seçim
            if (Object.FindFirstObjectByType<HubHUD>() == null)
            {
                var hub = new GameObject("HubSystem");
                hub.AddComponent<HubHUD>();
                hub.AddComponent<PetSelectHUD>();
            }

            // Arena
            if (Object.FindFirstObjectByType<ArenaManager>() == null)
            {
                var arena = new GameObject("ArenaSystem");
                arena.AddComponent<CanavarZindanlari.Backend.FirebaseAuthManager>();
                arena.AddComponent<ArenaManager>();
                arena.AddComponent<ArenaHUD>();
            }

            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene());

            Debug.Log("[Setup] HubSystem + ArenaSystem eklendi.");
            EditorUtility.DisplayDialog("Sistemler Eklendi",
                "Eklenenler:\n• HubSystem (HubHUD + PetSelectHUD)\n• ArenaSystem (FirebaseAuthManager + ArenaManager + ArenaHUD)\n\n" +
                "FirebaseAuthManager'a Web Client ID'yi gir.\n" +
                "Sahneyi kaydet (Ctrl+S).", "Tamam");
        }

        [MenuItem("CanavarZindanlari/Kurulum/Tümünü Çalıştır (1+2+3)")]
        public static void RunAll()
        {
            CreateTestMonsters();
            CreateBattleScene();
            CreateClassAssets();
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

        // ── Sınıf / Yetenek fabrika metodları ────────────────────────────────

        private static ClassData MakeClass(string id, string displayName, DamageType dmgType,
            int hp, int atk, int def, int spd,
            SkillData s0, SkillData s1, SkillData s2, SkillData s3)
        {
            string path = $"{ClassPath}/{id}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<ClassData>(path);
            if (existing != null) return existing;

            var c = ScriptableObject.CreateInstance<ClassData>();
            c.ClassName  = displayName;
            c.DamageType = dmgType;
            c.BaseHP     = hp;
            c.BaseATK    = atk;
            c.BaseDEF    = def;
            c.BaseSPD    = spd;
            c.Skills     = new[] { s0, s1, s2, s3 };

            AssetDatabase.CreateAsset(c, path);
            return c;
        }

        private static SkillData MakeSkill(
            string id, string skillName, TargetType target, int cd, float mult,
            float crit = 0f, int multiHit = 1, float multiCrit = 0f,
            float healSelf = 0f,
            SkillEffect[] hitFx = null, SkillEffect[] selfFx = null)
        {
            string path = $"{SkillPath}/{id}.asset";
            var existing = AssetDatabase.LoadAssetAtPath<SkillData>(path);
            if (existing != null) return existing;

            var s = ScriptableObject.CreateInstance<SkillData>();
            s.SkillName         = skillName;
            s.TargetType        = target;
            s.CooldownTurns     = cd;
            s.DamageMultiplier  = mult;
            s.CritChance        = crit;
            s.CritMultiplier    = 2f;
            s.MultiHitCount     = multiHit;
            s.MultiHitCritChance = multiCrit;
            s.HealSelfPercent   = healSelf;
            s.OnHitEffects      = hitFx  ?? System.Array.Empty<SkillEffect>();
            s.OnSelfEffects     = selfFx ?? System.Array.Empty<SkillEffect>();

            // HealPercent — saf iyileştirme slotları için HealSelfPercent'i kopyala
            if (target == TargetType.Self && mult == 0f && healSelf > 0f)
                s.HealPercent = healSelf;

            AssetDatabase.CreateAsset(s, path);
            return s;
        }

        private static SkillEffect MakeFx(StatusEffectType type, float value, int duration)
            => new SkillEffect { Type = type, Value = value, Duration = duration };

        private static void EnsureFolder(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                int last = path.LastIndexOf('/');
                AssetDatabase.CreateFolder(path.Substring(0, last), path.Substring(last + 1));
            }
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
