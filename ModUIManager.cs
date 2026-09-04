using MelonLoader;
using Mono.WebBrowser;
using System.Collections;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GetToModLib.UI
{
    public class ModUIManager : MonoBehaviour
    {
        public bool uiInitalized;

        public class ModFrontUI
        {
            public GetToMod mod;
            public GameObject ModButton;
            public GameObject ModButtonTitle;
            public GameObject ModButtonNotif;
            public Button ModButtonButton;
            public bool IsMenuOpen;
        }

        public ModFrontUI[] frontUIs;

        private void Awake()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            if (arg0.name == "2-TitleScreen")
            { StartCoroutine(InitializeUIDelayed()); }
            else
            { CleanupTitleUI(); }
        }

        private void CleanupTitleUI()
        {
            foreach(var modUI in frontUIs)
            {
                if(modUI.ModButton != null)
                    Destroy(modUI.ModButton);

                modUI.ModButton = null;
                modUI.IsMenuOpen = false;
            }

            /*if (MultiplayerButton != null)
                Destroy(MultiplayerButton);

            MultiplayerButton = null;
            MultiplayerButtonButton = null;
            MultiplayerButtonTitle = null;
            MultiplayerButtonNotif = null;

            menuOpen = false;
            uiInitalized = false;*/
        }

        private IEnumerator InitializeUIDelayed()
        {
            yield return null;

            uiInitalized = UIRefs.Init();
            if (!uiInitalized)
            {
                DebugLogger.LogError("Something went wrong finding UI, see logs above.");
                yield break;
            }

            foreach (var modUI in frontUIs)
            {
                modUI.ModButton = GameObject.Instantiate(UIRefs.DoinklerPortfolioButton);
                modUI.ModButton.transform.SetParent(UIRefs.RightSideMenu.transform, false);
                RectTransform rt = modUI.ModButton.GetComponent<RectTransform>();

                rt.anchoredPosition += new Vector2(0, 160);

                modUI.ModButtonTitle = UIFinder.Find(modUI.ModButton.transform, "title_ChallengerLvls").gameObject;
                Transform uiNotifParent = UIFinder.Find(modUI.ModButton.transform, "img_NewNotif");
                modUI.ModButtonNotif = UIFinder.Find(uiNotifParent, "title_New").gameObject;

                modUI.ModButtonNotif.GetComponent<TMP_Text>().SetText(modUI.mod.flair);
                modUI.ModButtonTitle.GetComponent<TMP_Text>().SetText(modUI.mod.name);

                modUI.ModButtonButton = modUI.ModButton.GetComponent<Button>();

                if (modUI.ModButtonButton == null)
                { modUI.ModButtonButton = modUI.ModButton.AddComponent<Button>(); }
                else
                { modUI.ModButtonButton.onClick = new Button.ButtonClickedEvent(); }

                modUI.ModButtonButton.onClick.AddListener(() => {
                    modUI.IsMenuOpen = true;
                });

                modUI.IsMenuOpen = false;
            }
        }
    }

    public static class UIRefs
    {
        public static Canvas MainMenuCanvas;
        public static GameObject RightSideMenu;
        public static GameObject DoinklerPortfolioButton;
        public static GameObject DoinklerPortfolioTitle;
        public static GameObject DoinklerPortfolioNotif;

        public static GameObject ModMenuUI;

        public static Transform Before;
        public static Button JoinButton;
        public static TMP_InputField LobbyIDInput;
        public static TMP_InputField MaxPlayersInput;
        public static TMP_Dropdown LobbyTypeDropdown;
        public static Button HostButton;
        public static Toggle PlayerCollisionToggle;
        public static GameObject PerformanceWarning;

        public static Transform After;
        public static ScrollRect PlayerScrollView;
        public static Transform PlayerListContent;
        public static Button CopyLobbyIDButton;
        public static Button InviteButton;
        public static Button StartButton;
        public static Button LeaveButton;

        public static Button MainGameSelect;
        public static Button DoinklerPortfolioSelect;
        public static Button DoinklerSpecialSelect;

        public static bool Initalized;

        /// <summary>
        /// globally initialize all mod ui
        /// </summary>
        /// <returns></returns>
        public static bool Init()
        {
            if (Initalized &&
                ModMenuUI)
            {
                return true;
            }

            Transform canvasTransform = UIFinder.FindGlobal("Main Menu Canvas");
            if (canvasTransform == null)
            {
                DebugLogger.LogError("CRITICAL: 'Main Menu Canvas' not found!");
                return false;
            }

            MainMenuCanvas = canvasTransform.GetComponent<Canvas>();
            if (MainMenuCanvas == null)
            {
                DebugLogger.LogError("CRITICAL: 'Main Menu Canvas' missing Canvas component!");
                return false;
            }

            Transform rightSideMenuTransform = UIFinder.Find(MainMenuCanvas.transform, "UI_MainMenu");
            if (rightSideMenuTransform == null)
            {
                DebugLogger.LogError("CRITICAL: 'UI_MainMenu' not found!");
                return false;
            }
            RightSideMenu = rightSideMenuTransform.gameObject;

            Transform buttonTransform = UIFinder.Find(RightSideMenu.transform, "btn_DoinklerPortfolio");
            if (buttonTransform == null)
            {
                DebugLogger.LogError("CRITICAL: 'btn_DoinklerPortfolio' not found!");
                return false;
            }
            DoinklerPortfolioButton = buttonTransform.gameObject;

            Transform titleTransform = UIFinder.Find(DoinklerPortfolioButton.transform, "title_ChallengerLvls");
            if (titleTransform == null)
            {
                DebugLogger.LogError("CRITICAL: 'title_ChallengerLvls' not found!");
                return false;
            }
            DoinklerPortfolioTitle = titleTransform.gameObject;

            Transform notifParentTransform = UIFinder.Find(DoinklerPortfolioButton.transform, "img_NewNotif");
            if (notifParentTransform == null)
            {
                DebugLogger.LogError("CRITICAL: 'img_NewNotif' not found!");
                return false;
            }

            Transform notifTitleTransform = UIFinder.Find(notifParentTransform, "title_New");
            if (notifTitleTransform == null)
            {
                DebugLogger.LogError("CRITICAL: 'title_New' not found!");
                return false;
            }
            DoinklerPortfolioNotif = notifTitleTransform.gameObject;

            DebugLogger.Log("UI references found.");

            var menuUiAsset = Utils.LoadPrefabFromMelonUserData("ui", "gtmpfile", "MultiplayerMenuUI");
            ModMenuUI = GameObject.Instantiate(menuUiAsset, canvasTransform);
            if (ModMenuUI == null)
            {
                DebugLogger.LogError("CRITICAL: Couldn't load ModMenuUI!");
                return false;
            }

            foreach (var mod in Core.Instance.LoadedMods)
            {
                Transform menuRoot = ModMenuUI.transform;
                if (mod != null)
                {
                    string version = mod.version.ToString();
                    TMP_Text VersionText = UIFinder.Find(menuRoot, "Version").GetComponent<TMP_Text>();
                    VersionText.text = version;
                }

                /*Before = UIFinder.Find(menuRoot, "Before");
                if (Before != null)
                {
                    JoinButton = UIFinder.Find(Before, "Join")?.GetComponent<Button>();
                    LobbyIDInput = UIFinder.Find(Before, "LobbyIDInput")?.GetComponent<TMP_InputField>();
                    MaxPlayersInput = UIFinder.Find(Before, "MaxPlayersInput")?.GetComponent<TMP_InputField>();
                    LobbyTypeDropdown = UIFinder.Find(Before, "LobbyTypeDropdown")?.GetComponent<TMP_Dropdown>();
                    HostButton = UIFinder.Find(Before, "Host")?.GetComponent<Button>();
                    PlayerCollisionToggle = UIFinder.Find(Before, "PlayerCollisionToggle")?.GetComponent<Toggle>();
                    PerformanceWarning = UIFinder.Find(Before, "PerfNotice")?.gameObject;
                }

                After = UIFinder.Find(menuRoot, "After");
                if (After != null)
                {
                    PlayerScrollView = UIFinder.Find(After, "Scroll View")?.GetComponent<ScrollRect>();
                    PlayerListContent = UIFinder.Find(After, "Content");
                    CopyLobbyIDButton = UIFinder.Find(After, "Button")?.GetComponent<Button>();
                    InviteButton = UIFinder.Find(After, "Button")?.GetComponent<Button>();
                    StartButton = UIFinder.Find(After, "Start")?.GetComponent<Button>();
                    LeaveButton = UIFinder.Find(After, "Leave")?.GetComponent<Button>();
                    Transform Selection = UIFinder.Find(After, "SelectGame");
                    if (Selection != null)
                    {
                        MainGameSelect = UIFinder.Find(Selection, "MainGame")?.GetComponent<Button>();
                        DoinklerPortfolioSelect = UIFinder.Find(Selection, "DoinkPortfolio")?.GetComponent<Button>();
                        DoinklerSpecialSelect = UIFinder.Find(Selection, "DoinkSpecial")?.GetComponent<Button>();
                    }
                }*/

                ModMenuUI.transform.localScale = Utils.Unify<Vector3>(2);
                ModMenuUI.SetActive(false);

                if (HostButton == null) DebugLogger.LogWarning("HostButton missing");
                if (JoinButton == null) DebugLogger.LogWarning("JoinButton missing");
                if (LeaveButton == null) DebugLogger.LogWarning("LeaveButton missing");
            }

            Initalized = true;
            DebugLogger.Log("UIRefs initialized successfully!");
            return true;
        }
    }

    public static class UIFinder
    {
        public static Transform Find(Transform root, params string[] names)
        {
            Transform[] transforms;
            if (root == null)
                transforms = GameObject.FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.InstanceID);
            else
                transforms = root.GetComponentsInChildren<Transform>(true);

            foreach (var t in transforms)
                foreach (var name in names)
                    if (t.name == name)
                        return t;
            return null;
        }

        public static Transform FindGlobal(params string[] names) => Find(null, names);
    }
}