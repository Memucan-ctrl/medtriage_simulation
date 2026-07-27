#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using System.IO;

namespace Medtriage.Editor
{
    public static class MedtriageUIReskin
    {
        // ── Design Tokens ──
        static readonly Color Primary      = new Color(0.051f, 0.580f, 0.533f, 1f);   // #0D9488
        static readonly Color PrimaryHover = new Color(0.059f, 0.463f, 0.431f, 1f);   // #0F766E
        static readonly Color PrimaryPress = new Color(0.067f, 0.369f, 0.349f, 1f);   // #115E59
        static readonly Color PrimaryDisab = new Color(0.051f, 0.580f, 0.533f, 0.4f);
        static readonly Color BgColor      = new Color(0.945f, 0.961f, 0.976f, 1f);   // #F1F5F9
        static readonly Color CardColor    = Color.white;
        static readonly Color TextPrimary  = new Color(0.118f, 0.161f, 0.231f, 1f);   // #1E293B
        static readonly Color TextSecond   = new Color(0.392f, 0.455f, 0.545f, 1f);   // #64748B
        static readonly Color ErrorRed     = new Color(0.863f, 0.149f, 0.149f, 1f);   // #DC2626
        static readonly Color SuccessGreen = new Color(0.086f, 0.639f, 0.290f, 1f);   // #16A34A
        static readonly Color InputBg      = new Color(0.886f, 0.910f, 0.941f, 1f);   // #E2E8F0
        static readonly Color SecBtnNorm   = new Color(0.886f, 0.910f, 0.941f, 1f);
        static readonly Color SecBtnHover  = new Color(0.812f, 0.839f, 0.882f, 1f);

        const float TitleSize = 28f, BodySize = 16f, BtnSize = 18f, SmallSize = 14f;

        static Sprite _roundedSprite;

        // ── Menu Items ──
        [MenuItem("Medtriage/Reskin All Scenes")]
        public static void ReskinAll()
        {
            EnsureRoundedSprite();
            ReskinScene("Assets/_Frontend/Scenes/Login.unity", ReskinLoginScene);
            ReskinScene("Assets/_Frontend/Scenes/Registration.unity", ReskinRegistrationScene);
            ReskinScene("Assets/_Frontend/Scenes/MainMenu.unity", ReskinMainMenuScene);
            Debug.Log("[MedtriageUIReskin] All scenes reskinned.");
        }

        static void ReskinScene(string path, System.Action action)
        {
            var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            action();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene);
        }

        // ── Rounded Sprite Generator ──
        static void EnsureRoundedSprite()
        {
            string assetDir = "_Frontend/UI/Sprites";
            string assetPath = assetDir + "/RoundedRect.png";
            string fullDir = System.IO.Path.Combine(Application.dataPath, assetDir);
            string fullPath = System.IO.Path.Combine(Application.dataPath, assetPath);
            string dbPath = "Assets/" + assetPath;

            if (!Directory.Exists(fullDir)) Directory.CreateDirectory(fullDir);

            if (!File.Exists(fullPath))
            {
                int s = 64, r = 12;
                var tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
                for (int y = 0; y < s; y++)
                    for (int x = 0; x < s; x++)
                        tex.SetPixel(x, y, RoundedPixel(x, y, s, r));
                tex.Apply();
                File.WriteAllBytes(fullPath, tex.EncodeToPNG());
                Object.DestroyImmediate(tex);
                AssetDatabase.Refresh();
            }

            var imp = AssetImporter.GetAtPath(dbPath) as TextureImporter;
            if (imp != null)
            {
                imp.textureType = TextureImporterType.Sprite;
                imp.spriteImportMode = SpriteImportMode.Single;
                imp.spriteBorder = new Vector4(16, 16, 16, 16);
                imp.filterMode = FilterMode.Bilinear;
                imp.SaveAndReimport();
            }
            _roundedSprite = AssetDatabase.LoadAssetAtPath<Sprite>(dbPath);
        }

        static Color RoundedPixel(int x, int y, int size, int radius)
        {
            int dx = 0, dy = 0;
            if (x < radius) dx = radius - x;
            else if (x >= size - radius) dx = x - (size - radius - 1);
            if (y < radius) dy = radius - y;
            else if (y >= size - radius) dy = y - (size - radius - 1);
            float dist = Mathf.Sqrt(dx * dx + dy * dy);
            float a = 1f - Mathf.Clamp01((dist - radius + 1f));
            return new Color(1, 1, 1, a);
        }

        // ══════════════════════════════════════════════
        //  LOGIN SCENE
        // ══════════════════════════════════════════════
        static void ReskinLoginScene()
        {
            var ctrl = Object.FindFirstObjectByType<Medtriage.Frontend.UI.LoginUIController>();
            if (ctrl == null) { Debug.LogError("LoginUIController not found"); return; }

            // Get serialized refs
            var so = new SerializedObject(ctrl);
            var usernameFieldObj = (so.FindProperty("usernameField").objectReferenceValue as TMP_InputField);
            var passwordFieldObj = (so.FindProperty("passwordField").objectReferenceValue as TMP_InputField);
            var loginBtnObj      = (so.FindProperty("loginButton").objectReferenceValue as Button);
            var regBtnObj        = (so.FindProperty("goToRegistrationButton").objectReferenceValue as Button);
            var statusLabelObj   = (so.FindProperty("statusLabel").objectReferenceValue as TMP_Text);
            var spinnerObj       = (so.FindProperty("loadingSpinner").objectReferenceValue as GameObject);

            // Get canvas, set bg
            var canvas = Object.FindFirstObjectByType<Canvas>();
            var canvasRT = canvas.GetComponent<RectTransform>();
            SetupCanvasScaler(canvas);

            // Add background image to canvas
            var bgImg = canvas.gameObject.GetComponent<Image>();
            if (bgImg == null) bgImg = canvas.gameObject.AddComponent<Image>();
            bgImg.color = BgColor;
            bgImg.raycastTarget = false;

            // Create card panel
            var card = CreateChild(canvasRT, "LoginCard");
            var cardRT = card.GetComponent<RectTransform>();
            SetAnchorsCenter(cardRT, 420, 0); // height auto
            var cardImg = card.AddComponent<Image>();
            cardImg.sprite = _roundedSprite;
            cardImg.type = Image.Type.Sliced;
            cardImg.color = CardColor;
            cardImg.raycastTarget = false;

            // VLG on card
            var vlg = card.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(32, 32, 32, 32);
            vlg.spacing = 16;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var csf = card.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            // Title
            var title = CreateTMPLabel(cardRT, "TitleLabel", "Sign In", TitleSize, TextPrimary, FontStyles.Bold);
            SetLayoutElem(title, -1, 48);

            // Spacer
            CreateSpacer(cardRT, 8);

            // Username
            ReparentAndStyle(usernameFieldObj.gameObject, cardRT, "Username", 48);
            StyleInputField(usernameFieldObj, "Username");

            // Password
            ReparentAndStyle(passwordFieldObj.gameObject, cardRT, "Password", 48);
            StyleInputField(passwordFieldObj, "Password");

            // Spacer
            CreateSpacer(cardRT, 8);

            // Login button
            ReparentAndStyle(loginBtnObj.gameObject, cardRT, null, 48);
            StylePrimaryButton(loginBtnObj, "Sign In");

            // Registration button
            ReparentAndStyle(regBtnObj.gameObject, cardRT, null, 40);
            StyleSecondaryButton(regBtnObj, "Create Account");

            // Status label
            ReparentAndStyle(statusLabelObj.gameObject, cardRT, null, 24);
            StyleStatusLabel(statusLabelObj);

            // Spinner
            ReparentAndStyleSpinner(spinnerObj, cardRT);

            // Clean up orphaned objects from canvas root (old children moved into card)
            CleanOrphans(canvasRT, card.transform);

            // Re-serialize (refs are same objects, just reparented)
            Debug.Log("[Reskin] Login scene styled.");
        }

        // ══════════════════════════════════════════════
        //  REGISTRATION SCENE
        // ══════════════════════════════════════════════
        static void ReskinRegistrationScene()
        {
            var ctrl = Object.FindFirstObjectByType<Medtriage.Frontend.UI.RegistrationUIController>();
            if (ctrl == null) { Debug.LogError("RegistrationUIController not found"); return; }

            var so = new SerializedObject(ctrl);
            var usernameFieldObj  = so.FindProperty("usernameField").objectReferenceValue as TMP_InputField;
            var passwordFieldObj  = so.FindProperty("passwordField").objectReferenceValue as TMP_InputField;
            var confirmFieldObj   = so.FindProperty("confirmPasswordField").objectReferenceValue as TMP_InputField;
            var createBtnObj      = so.FindProperty("createAccountButton").objectReferenceValue as Button;
            var backBtnObj        = so.FindProperty("backToLoginButton").objectReferenceValue as Button;
            var statusLabelObj    = so.FindProperty("statusLabel").objectReferenceValue as TMP_Text;
            var spinnerObj        = so.FindProperty("loadingSpinner").objectReferenceValue as GameObject;

            var canvas = Object.FindFirstObjectByType<Canvas>();
            var canvasRT = canvas.GetComponent<RectTransform>();
            SetupCanvasScaler(canvas);

            var bgImg = canvas.gameObject.GetComponent<Image>();
            if (bgImg == null) bgImg = canvas.gameObject.AddComponent<Image>();
            bgImg.color = BgColor;
            bgImg.raycastTarget = false;

            var card = CreateChild(canvasRT, "RegistrationCard");
            var cardRT = card.GetComponent<RectTransform>();
            SetAnchorsCenter(cardRT, 420, 0);
            var cardImg = card.AddComponent<Image>();
            cardImg.sprite = _roundedSprite;
            cardImg.type = Image.Type.Sliced;
            cardImg.color = CardColor;
            cardImg.raycastTarget = false;

            var vlg = card.AddComponent<VerticalLayoutGroup>();
            vlg.padding = new RectOffset(32, 32, 32, 32);
            vlg.spacing = 16;
            vlg.childAlignment = TextAnchor.UpperCenter;
            vlg.childControlWidth = true;
            vlg.childControlHeight = true;
            vlg.childForceExpandWidth = true;
            vlg.childForceExpandHeight = false;

            var csf = card.AddComponent<ContentSizeFitter>();
            csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;

            CreateTMPLabel(cardRT, "TitleLabel", "Create Account", TitleSize, TextPrimary, FontStyles.Bold);
            CreateSpacer(cardRT, 8);

            ReparentAndStyle(usernameFieldObj.gameObject, cardRT, "Username", 48);
            StyleInputField(usernameFieldObj, "Username");

            ReparentAndStyle(passwordFieldObj.gameObject, cardRT, "Password", 48);
            StyleInputField(passwordFieldObj, "Password");

            ReparentAndStyle(confirmFieldObj.gameObject, cardRT, "Confirm Password", 48);
            StyleInputField(confirmFieldObj, "Confirm Password");

            CreateSpacer(cardRT, 8);

            ReparentAndStyle(createBtnObj.gameObject, cardRT, null, 48);
            StylePrimaryButton(createBtnObj, "Create Account");

            ReparentAndStyle(backBtnObj.gameObject, cardRT, null, 40);
            StyleSecondaryButton(backBtnObj, "Back to Sign In");

            ReparentAndStyle(statusLabelObj.gameObject, cardRT, null, 24);
            StyleStatusLabel(statusLabelObj);

            ReparentAndStyleSpinner(spinnerObj, cardRT);
            CleanOrphans(canvasRT, card.transform);

            Debug.Log("[Reskin] Registration scene styled.");
        }

        // ══════════════════════════════════════════════
        //  MAIN MENU SCENE
        // ══════════════════════════════════════════════
        static void ReskinMainMenuScene()
        {
            var ctrl = Object.FindFirstObjectByType<Medtriage.Frontend.UI.MainMenuUIController>();
            if (ctrl == null) { Debug.LogError("MainMenuUIController not found"); return; }

            var so = new SerializedObject(ctrl);
            var gridParent = so.FindProperty("gridParent").objectReferenceValue as Transform;

            var canvas = Object.FindFirstObjectByType<Canvas>();
            var canvasRT = canvas.GetComponent<RectTransform>();
            SetupCanvasScaler(canvas);

            var bgImg = canvas.gameObject.GetComponent<Image>();
            if (bgImg == null) bgImg = canvas.gameObject.AddComponent<Image>();
            bgImg.color = BgColor;
            bgImg.raycastTarget = false;

            // Header
            var header = CreateChild(canvasRT, "Header");
            var headerRT = header.GetComponent<RectTransform>();
            headerRT.anchorMin = new Vector2(0, 1);
            headerRT.anchorMax = new Vector2(1, 1);
            headerRT.pivot = new Vector2(0.5f, 1);
            headerRT.anchoredPosition = Vector2.zero;
            headerRT.sizeDelta = new Vector2(0, 72);

            var headerImg = header.AddComponent<Image>();
            headerImg.color = CardColor;
            headerImg.raycastTarget = false;

            var headerTitle = CreateTMPLabel(headerRT, "HeaderTitle", "Training Dashboard", TitleSize, TextPrimary, FontStyles.Bold);
            var htRT = headerTitle.GetComponent<RectTransform>();
            htRT.anchorMin = Vector2.zero;
            htRT.anchorMax = Vector2.one;
            htRT.offsetMin = new Vector2(24, 0);
            htRT.offsetMax = new Vector2(-24, 0);

            // Remove LayoutElement from header title so it doesn't conflict
            var hle = headerTitle.GetComponent<LayoutElement>();
            if (hle != null) Object.DestroyImmediate(hle);

            // Style grid parent
            if (gridParent != null)
            {
                var gpRT = gridParent.GetComponent<RectTransform>();
                gpRT.anchorMin = new Vector2(0, 0);
                gpRT.anchorMax = new Vector2(1, 1);
                gpRT.offsetMin = new Vector2(24, 24);
                gpRT.offsetMax = new Vector2(-24, -96);

                var glg = gridParent.GetComponent<GridLayoutGroup>();
                if (glg == null) glg = gridParent.gameObject.AddComponent<GridLayoutGroup>();
                glg.cellSize = new Vector2(280, 220);
                glg.spacing = new Vector2(20, 20);
                glg.startCorner = GridLayoutGroup.Corner.UpperLeft;
                glg.startAxis = GridLayoutGroup.Axis.Horizontal;
                glg.childAlignment = TextAnchor.UpperLeft;
                glg.constraint = GridLayoutGroup.Constraint.Flexible;
            }

            // Style the TaskTile prefab
            StyleTaskTilePrefab();

            Debug.Log("[Reskin] MainMenu scene styled.");
        }

        static void StyleTaskTilePrefab()
        {
            string prefabPath = "Assets/_Frontend/Prefabs/TaskTile.prefab";
            var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (prefab == null) { Debug.LogWarning("TaskTile prefab not found"); return; }

            var root = PrefabUtility.LoadPrefabContents(prefabPath);
            var ttb = root.GetComponent<Medtriage.Frontend.UI.TaskTileButton>();
            if (ttb == null) { PrefabUtility.UnloadPrefabContents(root); return; }

            var ttSO = new SerializedObject(ttb);
            var titleLabel = ttSO.FindProperty("titleLabel").objectReferenceValue as TMP_Text;
            var descLabel  = ttSO.FindProperty("descriptionLabel").objectReferenceValue as TMP_Text;
            var thumbImg   = ttSO.FindProperty("thumbnailImage").objectReferenceValue as Image;
            var startBtn   = ttSO.FindProperty("startButton").objectReferenceValue as Button;
            var badge      = ttSO.FindProperty("completedBadge").objectReferenceValue as GameObject;

            // Card bg
            var rootImg = root.GetComponent<Image>();
            if (rootImg == null) rootImg = root.AddComponent<Image>();
            rootImg.sprite = _roundedSprite;
            rootImg.type = Image.Type.Sliced;
            rootImg.color = CardColor;

            // Style children
            if (titleLabel != null)
            {
                titleLabel.fontSize = BtnSize;
                titleLabel.color = TextPrimary;
                titleLabel.fontStyle = FontStyles.Bold;
            }
            if (descLabel != null)
            {
                descLabel.fontSize = SmallSize;
                descLabel.color = TextSecond;
            }
            if (startBtn != null) StylePrimaryButton(startBtn, "Start");
            if (badge != null)
            {
                var badgeImg = badge.GetComponent<Image>();
                if (badgeImg != null) badgeImg.color = SuccessGreen;
                var badgeTxt = badge.GetComponentInChildren<TMP_Text>();
                if (badgeTxt != null)
                {
                    badgeTxt.text = "Completed";
                    badgeTxt.fontSize = SmallSize;
                    badgeTxt.color = Color.white;
                }
            }

            PrefabUtility.SaveAsPrefabAsset(root, prefabPath);
            PrefabUtility.UnloadPrefabContents(root);
        }

        // ══════════════════════════════════════════════
        //  UTILITY METHODS
        // ══════════════════════════════════════════════
        static void SetupCanvasScaler(Canvas canvas)
        {
            var scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler != null)
            {
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
                scaler.matchWidthOrHeight = 0.5f;
            }
        }

        static GameObject CreateChild(Transform parent, string name)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            go.layer = 5; // UI
            return go;
        }

        static void SetAnchorsCenter(RectTransform rt, float width, float height)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = Vector2.zero;
            rt.sizeDelta = new Vector2(width, height);
        }

        static GameObject CreateTMPLabel(Transform parent, string name, string text, float size, Color color, FontStyles style)
        {
            var go = CreateChild(parent, name);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = size;
            tmp.color = color;
            tmp.fontStyle = style;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.raycastTarget = false;
            SetLayoutElem(go, -1, size + 16);
            return go;
        }

        static void SetLayoutElem(GameObject go, float minH, float prefH)
        {
            var le = go.GetComponent<LayoutElement>();
            if (le == null) le = go.AddComponent<LayoutElement>();
            if (minH >= 0) le.minHeight = minH;
            le.preferredHeight = prefH;
        }

        static void CreateSpacer(Transform parent, float height)
        {
            var s = CreateChild(parent, "Spacer");
            SetLayoutElem(s, height, height);
        }

        static void ReparentAndStyle(GameObject go, Transform newParent, string name, float prefHeight)
        {
            go.transform.SetParent(newParent, false);
            if (name != null) go.name = name;
            SetLayoutElem(go, -1, prefHeight);
        }

        static void StyleInputField(TMP_InputField field, string placeholder)
        {
            var img = field.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = _roundedSprite;
                img.type = Image.Type.Sliced;
                img.color = InputBg;
            }

            // Placeholder text
            var ph = field.placeholder as TMP_Text;
            if (ph != null)
            {
                ph.text = placeholder;
                ph.color = TextSecond;
                ph.fontSize = BodySize;
                ph.fontStyle = FontStyles.Italic;
            }

            // Input text
            var txt = field.textComponent;
            if (txt != null)
            {
                txt.color = TextPrimary;
                txt.fontSize = BodySize;
            }

            field.caretColor = Primary;
            field.selectionColor = new Color(Primary.r, Primary.g, Primary.b, 0.3f);
        }

        static void StylePrimaryButton(Button btn, string label)
        {
            var img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = _roundedSprite;
                img.type = Image.Type.Sliced;
                img.color = Primary;
            }

            var cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(0.85f, 0.95f, 0.93f, 1f);
            cb.pressedColor = new Color(0.7f, 0.88f, 0.85f, 1f);
            cb.selectedColor = cb.highlightedColor;
            cb.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
            cb.fadeDuration = 0.1f;
            btn.colors = cb;

            var txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt != null)
            {
                txt.text = label;
                txt.fontSize = BtnSize;
                txt.color = Color.white;
                txt.fontStyle = FontStyles.Bold;
                txt.alignment = TextAlignmentOptions.Center;
            }
        }

        static void StyleSecondaryButton(Button btn, string label)
        {
            var img = btn.GetComponent<Image>();
            if (img != null)
            {
                img.sprite = _roundedSprite;
                img.type = Image.Type.Sliced;
                img.color = SecBtnNorm;
            }

            var cb = btn.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = new Color(0.92f, 0.93f, 0.95f, 1f);
            cb.pressedColor = new Color(0.82f, 0.84f, 0.88f, 1f);
            cb.selectedColor = cb.highlightedColor;
            cb.disabledColor = new Color(0.7f, 0.7f, 0.7f, 0.5f);
            cb.fadeDuration = 0.1f;
            btn.colors = cb;

            var txt = btn.GetComponentInChildren<TMP_Text>();
            if (txt != null)
            {
                txt.text = label;
                txt.fontSize = BodySize;
                txt.color = TextPrimary;
                txt.fontStyle = FontStyles.Normal;
                txt.alignment = TextAlignmentOptions.Center;
            }
        }

        static void StyleStatusLabel(TMP_Text label)
        {
            label.fontSize = SmallSize;
            label.color = ErrorRed;
            label.alignment = TextAlignmentOptions.Center;
            label.text = "";
            SetLayoutElem(label.gameObject, 24, 24);
        }

        static void ReparentAndStyleSpinner(GameObject spinner, Transform parent)
        {
            spinner.transform.SetParent(parent, false);
            SetLayoutElem(spinner, 32, 32);

            // Replace text with a circular image spinner
            var tmp = spinner.GetComponent<TMP_Text>();
            if (tmp != null) Object.DestroyImmediate(tmp);

            var img = spinner.GetComponent<Image>();
            if (img == null) img = spinner.AddComponent<Image>();
            img.sprite = null; // Will use default circle
            img.color = Primary;
            img.type = Image.Type.Filled;
            img.fillMethod = Image.FillMethod.Radial360;
            img.fillAmount = 0.75f;

            // Add spinner rotation component
            var uiSpinner = spinner.GetComponent<Medtriage.Frontend.UI.UISpinner>();
            if (uiSpinner == null) spinner.AddComponent<Medtriage.Frontend.UI.UISpinner>();

            // Ensure a CanvasRenderer exists
            if (spinner.GetComponent<CanvasRenderer>() == null)
                spinner.AddComponent<CanvasRenderer>();
        }

        static void CleanOrphans(Transform canvasRT, Transform card)
        {
            // Don't destroy anything — items were reparented via SetParent.
            // Just move the card to be first child for render order.
            card.SetAsLastSibling();
        }
    }
}
#endif
