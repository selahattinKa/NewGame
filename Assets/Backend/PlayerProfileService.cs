using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;
using UnityEngine;
using CanavarZindanlari.Core;
using CanavarZindanlari.Data;

namespace CanavarZindanlari.Backend
{
    /// <summary>
    /// Firestore'da oyuncu profili CRUD.
    /// Collection: "players" — her belge uid ile eşleşir.
    /// </summary>
    public static class PlayerProfileService
    {
        private const string Col = "players";

        // ── CP hesaplama ─────────────────────────────────────────────────────

        /// <summary>
        /// Oyuncunun anlık savaş gücünü hesaplar.
        /// Sınıf seviyesi + koleksiyondaki canavarlar.
        /// </summary>
        public static int CalculateCombatPower(MonsterCollection collection)
        {
            int cp  = 100; // temel CP
            var pet = collection?.SelectedPet;
            if (pet == null) return cp;

            cp += pet.Tier switch
            {
                Rarity.B => 150,
                Rarity.C => 70,
                Rarity.D => 30,
                _        => 10,  // F
            };
            return cp;
        }

        // ── Yükle / Oluştur ──────────────────────────────────────────────────

        public static async Task<ArenaProfile> LoadOrCreate(string uid, string displayName, int combatPower)
        {
            var db  = FirebaseFirestore.DefaultInstance;
            var doc = db.Collection(Col).Document(uid);
            var snap = await doc.GetSnapshotAsync();

            if (snap.Exists)
            {
                return SnapshotToProfile(snap);
            }

            // Yeni oyuncu
            var profile = new ArenaProfile
            {
                Uid         = uid,
                DisplayName = displayName,
                CombatPower = combatPower,
                ArenaPoints = 0,
                Wins        = 0,
                Losses      = 0,
            };
            await doc.SetAsync(ProfileToDict(profile));
            Debug.Log($"[Profile] Yeni profil oluşturuldu: {displayName}");
            return profile;
        }

        // ── CP güncelle ──────────────────────────────────────────────────────

        public static async Task UpdateCombatPower(string uid, int combatPower)
        {
            var db  = FirebaseFirestore.DefaultInstance;
            await db.Collection(Col).Document(uid).UpdateAsync(new Dictionary<string, object>
            {
                { "combatPower", combatPower },
            });
        }

        // ── Maç sonucu kaydet ────────────────────────────────────────────────

        public static async Task<ArenaProfile> ApplyMatchResult(ArenaProfile profile, bool won)
        {
            int pointDelta = won ? 25 : -15;
            profile.ArenaPoints = Mathf.Max(0, profile.ArenaPoints + pointDelta);
            if (won) profile.Wins++;
            else     profile.Losses++;

            var db  = FirebaseFirestore.DefaultInstance;
            await db.Collection(Col).Document(profile.Uid).UpdateAsync(new Dictionary<string, object>
            {
                { "arenaPoints", profile.ArenaPoints },
                { "wins",        profile.Wins        },
                { "losses",      profile.Losses      },
            });
            return profile;
        }

        // ── Rakip bul ────────────────────────────────────────────────────────

        /// <summary>
        /// Aynı ligden en fazla 10 aday çeker, kendiniz hariç rastgele biri döner.
        /// Eşleşme yoksa null (bot savaşı).
        /// </summary>
        public static async Task<ArenaProfile> FindOpponent(ArenaProfile self)
        {
            var db = FirebaseFirestore.DefaultInstance;

            var query = db.Collection(Col)
                          .WhereEqualTo("leagueTier", self.LeagueTier)
                          .Limit(10);

            var snaps = await query.GetSnapshotAsync();
            var candidates = new List<ArenaProfile>();

            foreach (var s in snaps.Documents)
            {
                var p = SnapshotToProfile(s);
                if (p.Uid != self.Uid)
                    candidates.Add(p);
            }

            if (candidates.Count == 0) return null;
            return candidates[UnityEngine.Random.Range(0, candidates.Count)];
        }

        // ── Liderlik tablosu (top 20) ────────────────────────────────────────

        public static async Task<List<ArenaProfile>> GetLeaderboard()
        {
            var db    = FirebaseFirestore.DefaultInstance;
            var query = db.Collection(Col).OrderByDescending("arenaPoints").Limit(20);
            var snaps = await query.GetSnapshotAsync();

            var list = new List<ArenaProfile>();
            foreach (var s in snaps.Documents)
                list.Add(SnapshotToProfile(s));
            return list;
        }

        // ── Dönüşüm yardımcıları ─────────────────────────────────────────────

        private static ArenaProfile SnapshotToProfile(DocumentSnapshot s) => new ArenaProfile
        {
            Uid         = s.Id,
            DisplayName = s.GetValue<string>("displayName"),
            CombatPower = s.GetValue<int>("combatPower"),
            ArenaPoints = s.GetValue<int>("arenaPoints"),
            Wins        = s.GetValue<int>("wins"),
            Losses      = s.GetValue<int>("losses"),
        };

        private static Dictionary<string, object> ProfileToDict(ArenaProfile p) =>
            new Dictionary<string, object>
            {
                { "uid",         p.Uid         },
                { "displayName", p.DisplayName },
                { "combatPower", p.CombatPower },
                { "arenaPoints", p.ArenaPoints },
                { "wins",        p.Wins        },
                { "losses",      p.Losses      },
                { "leagueTier",  p.LeagueTier  },
            };
    }

    // ── Profil verisi ─────────────────────────────────────────────────────────

    [Serializable]
    public class ArenaProfile
    {
        public string Uid;
        public string DisplayName;
        public int    CombatPower;
        public int    ArenaPoints;
        public int    Wins;
        public int    Losses;

        public string LeagueTier => PointsToTier(ArenaPoints);
        public string LeagueIcon => PointsToIcon(ArenaPoints);
        public int    WinRate    => (Wins + Losses) == 0 ? 0 : Mathf.RoundToInt(100f * Wins / (Wins + Losses));

        public static string PointsToTier(int pts)
        {
            if (pts >= 5000) return "Platin";
            if (pts >= 2500) return "Altın";
            if (pts >= 1000) return "Gümüş";
            return "Bronz";
        }

        public static string PointsToIcon(int pts)
        {
            if (pts >= 5000) return "💎";
            if (pts >= 2500) return "🥇";
            if (pts >= 1000) return "🥈";
            return "🥉";
        }
    }
}
