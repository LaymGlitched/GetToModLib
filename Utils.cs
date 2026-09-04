using GetToModLib;
using Isto.GTW.Player;
using MelonLoader.Utils;
using Steamworks;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GetToModLib
{
    public static class Utils
    {
        private static GameObject _cachedPlayer;
        private static int _cachedSceneCount = -1;

        public static GameObject GetPlayer()
        {
            if (_cachedPlayer != null && SceneManager.sceneCount == _cachedSceneCount)
                return _cachedPlayer;

            _cachedSceneCount = SceneManager.sceneCount;
            for (int i = 0; i < _cachedSceneCount; i++)
            {
                Scene scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;

                foreach (GameObject root in scene.GetRootGameObjects())
                {
                    Transform player = FindChildRecursive(root.transform, "Player");
                    if (player != null)
                    {
                        _cachedPlayer = player.gameObject;
                        return _cachedPlayer;
                    }
                }
            }
            _cachedPlayer = null;
            Debug.LogWarning("Player doesn't exist!");
            return null;
        }

        private static Transform FindChildRecursive(Transform parent, string name)
        {
            if (parent.name == name) return parent;
            foreach (Transform child in parent)
            {
                Transform result = FindChildRecursive(child, name);
                if (result != null) return result;
            }
            return null;
        }

        public static TorquePlayerPhysicsPreset GetPlayerPhysicsPreset()
        {
            var controller = GetPlayerController();
            if (controller == null) return null;
            var torquePhysics = controller.TorquePhysics;
            if (torquePhysics == null)
            {
                Debug.LogWarning("TorquePhysics not found.");
                return null;
            }
            FieldInfo field = typeof(TorquePhysics).GetField("_playerPhysicsPreset", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field != null)
                return field.GetValue(torquePhysics) as TorquePlayerPhysicsPreset;
            Debug.LogWarning("Could not find _playerPhysicsPreset field.");
            return null;
        }

        public static PlayerController GetPlayerController()
        {
            GameObject player = GetPlayer();
            if (player != null) return player.GetComponent<PlayerController>();
            Debug.LogWarning("Player Controller not found!");
            return null;
        }

        public static GameObject LoadPrefabFromMelonUserData(string name, string ext, string prefabName)
        {
            string path = System.IO.Path.Combine(
                $"{MelonEnvironment.UserDataDirectory}/GetToReWorked/{name}.{ext}");

            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null)
            {
                DebugLogger.LogError(path + " is not an assetbundle");
                return null;
            }
            GameObject gameObject = assetBundle.LoadAsset<GameObject>(prefabName);
            if (gameObject == null)
                DebugLogger.LogError(prefabName + " is not inside of the assetbundle");
            assetBundle.Unload(false);
            return gameObject;
        }

        public static AssetBundle LoadBundleFromMelonUserData(string name, string ext)
        {
            string path = System.IO.Path.Combine(
                MelonEnvironment.UserDataDirectory, "GetToReWorked", $"{name}.{ext}");

            if (!System.IO.File.Exists(path))
            {
                DebugLogger.LogError("AssetBundle not found at " + path);
                return null;
            }
            AssetBundle assetBundle = AssetBundle.LoadFromFile(path);
            if (assetBundle == null)
                DebugLogger.LogError(path + " is not an assetbundle");
            return assetBundle;
        }

        public static byte[] ReadBundleHash(string bundlePath)
        {
            string hashPath = bundlePath + ".gtmpHash";

            if (!File.Exists(hashPath))
                return null;

            return File.ReadAllBytes(hashPath);
        }

        public static List<GameObject> AllChilds(GameObject root)
        {
            List<GameObject> result = new List<GameObject>();
            if (root.transform.childCount > 0)
                foreach (Transform child in root.transform)
                    Searcher(result, child.gameObject);
            return result;
        }

        private static void Searcher(List<GameObject> list, GameObject root)
        {
            list.Add(root);
            if (root.transform.childCount > 0)
                foreach (Transform child in root.transform)
                    Searcher(list, child.gameObject);
        }

        public static T Unify<T>(float value)
        {
            if (typeof(T) == typeof(Vector3))
                return (T)(object)new Vector3(value, value, value);
            if (typeof(T) == typeof(Vector2))
                return (T)(object)new Vector2(value, value);
            throw new NotSupportedException($"Unify<{typeof(T).Name}> not supported");
        }

        /// <summary>
        /// Get or set the OS clipboard contents (plain text).
        /// </summary>
        public static string ClipBoard
        {
            get
            {
                try
                {
                    return GUIUtility.systemCopyBuffer;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Clipboard get failed: {e.Message}");
                    return string.Empty;
                }
            }
            set
            {
                try
                {
                    GUIUtility.systemCopyBuffer = value;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Clipboard set failed: {e.Message}");
                }
            }
        }

        public static Sprite GetAndCacheSteamAvatar(Dictionary<CSteamID, Sprite> avatarCache, CSteamID steamId)
        {
            if (avatarCache.TryGetValue(steamId, out Sprite cached))
                return cached;

            Sprite gotten = GetSteamAvatar(steamId);
            avatarCache[steamId] = gotten;
            return gotten;
        }

        public static Sprite GetSteamAvatar(CSteamID steamId)
        {
            int avatarHandle =
                SteamFriends.GetLargeFriendAvatar(steamId);

            if (avatarHandle == -1 || avatarHandle == 0)
                return null;

            uint width, height;

            if (!SteamUtils.GetImageSize(
                avatarHandle,
                out width,
                out height))
                return null;

            byte[] image =
                new byte[width * height * 4];

            if (!SteamUtils.GetImageRGBA(
                avatarHandle,
                image,
                image.Length))
                return null;

            Texture2D tex =
                new Texture2D(
                    (int)width,
                    (int)height,
                    TextureFormat.RGBA32,
                    false
                );

            tex.LoadRawTextureData(image);
            tex.Apply();

            // Steam image is upside down
            Color[] pixels = tex.GetPixels();
            System.Array.Reverse(pixels);
            tex.SetPixels(pixels);
            tex.Apply();

            Sprite sprite =
                Sprite.Create(
                    tex,
                    new Rect(0, 0, width, height),
                    new Vector2(0.5f, 0.5f)
                );
            return sprite;
        }

        public static bool IsSceneLoadedContaining(string substring)
        {
            // Loop through all currently loaded scenes
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(i);

                // Check if the name contains your string (case-insensitive example)
                if (loadedScene.name.ToLower().Contains(substring.ToLower()))
                {
                    return true; // Found a matching loaded scene
                }
            }

            return false; // No matching loaded scene found
        }
    }
}