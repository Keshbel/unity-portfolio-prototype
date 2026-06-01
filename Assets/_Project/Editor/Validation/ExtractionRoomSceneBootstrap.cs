using System.Collections.Generic;
using ExtractionRoom.AI;
using ExtractionRoom.DI;
using ExtractionRoom.Gameplay;
using ExtractionRoom.Interaction;
using ExtractionRoom.Items;
using ExtractionRoom.Player;
using ExtractionRoom.Presentation;
using ExtractionRoom.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ExtractionRoom.Editor.Validation
{
    public static class ExtractionRoomSceneBootstrap
    {
        private const string ScenePath = "Assets/_Project/Scenes/MainPrototype.unity";
        private const string ConfigFolder = "Assets/_Project/Configs/Items";
        private const string AiConfigFolder = "Assets/_Project/Configs/AI";
        private const string MaterialFolder = "Assets/_Project/Art/Materials";

        private static readonly Color EnvironmentColor = new Color(0.24f, 0.27f, 0.31f);
        private static readonly Color FloorColor = new Color(0.14f, 0.16f, 0.19f);
        private static readonly Color FuseColor = new Color(0.15f, 0.75f, 1f);
        private static readonly Color MedkitColor = new Color(0.2f, 0.85f, 0.35f);
        private static readonly Color KeycardColor = new Color(1f, 0.78f, 0.12f);
        private static readonly Color DangerColor = new Color(0.9f, 0.18f, 0.16f);
        private static readonly Color ExtractionColor = new Color(1f, 0.55f, 0.1f);

        [MenuItem("ExtractionRoom/BootstrapPrototypeScene")]
        public static void Bootstrap()
        {
            EnsureFolder("Assets/_Project/Configs", "Items");
            EnsureFolder("Assets/_Project/Configs", "AI");
            EnsureFolder("Assets/_Project/Art", "Materials");

            var definitions = new[]
            {
                GetOrCreateDefinition("Fuse", ItemId.Fuse, ItemType.Fuse, 3),
                GetOrCreateDefinition("Medkit", ItemId.Medkit, ItemType.Medkit, 2),
                GetOrCreateDefinition("Keycard", ItemId.Keycard, ItemType.Keycard, 1)
            };

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var injectionRoots = new List<GameObject>();
            var enemyConfig = GetOrCreateEnemyConfig();

            CreateFloor();
            CreateEnvironment();
            CreateLight();
            CreateCamera();
            CreatePlayer(injectionRoots);
            CreatePickup("Fuse Pickup 1", ItemId.Fuse, "Pick Up Fuse", new Vector3(0f, 0.55f, 3f), FuseColor, injectionRoots);
            CreatePickup("Fuse Pickup 2", ItemId.Fuse, "Pick Up Fuse", new Vector3(0f, 0.55f, 5f), FuseColor, injectionRoots);
            CreatePickup("Fuse Pickup 3", ItemId.Fuse, "Pick Up Fuse", new Vector3(0f, 0.55f, 7f), FuseColor, injectionRoots);
            CreatePickup("Medkit Pickup", ItemId.Medkit, "Pick Up Medkit", new Vector3(-4f, 0.55f, 5f), MedkitColor, injectionRoots);
            CreatePickup("Keycard Pickup", ItemId.Keycard, "Pick Up Keycard", new Vector3(4f, 0.55f, 8f), KeycardColor, injectionRoots);
            CreateGenerator(injectionRoots);
            CreateExtractionZone(injectionRoots);
            CreateEnemy(enemyConfig, injectionRoots);
            CreateHud(injectionRoots);

            var compositionRoot = new GameObject("CompositionRoot");
            var lifetimeScope = compositionRoot.AddComponent<GameLifetimeScope>();
            lifetimeScope.ConfigureScene(definitions, injectionRoots);

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeGameObject = compositionRoot;
        }

        private static EnemyConfig GetOrCreateEnemyConfig()
        {
            var assetPath = $"{AiConfigFolder}/BasicEnemy.asset";
            var config = AssetDatabase.LoadAssetAtPath<EnemyConfig>(assetPath);
            if (config == null)
            {
                config = ScriptableObject.CreateInstance<EnemyConfig>();
                AssetDatabase.CreateAsset(config, assetPath);
            }

            config.Initialize(50, 2.5f, 6f, 1.5f, 10, 1f, 1f);
            EditorUtility.SetDirty(config);
            return config;
        }

        private static ItemDefinition GetOrCreateDefinition(
            string assetName,
            ItemId itemId,
            ItemType itemType,
            int maxStack)
        {
            var assetPath = $"{ConfigFolder}/{assetName}.asset";
            var definition = AssetDatabase.LoadAssetAtPath<ItemDefinition>(assetPath);
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<ItemDefinition>();
                AssetDatabase.CreateAsset(definition, assetPath);
            }

            definition.Initialize(itemId, assetName, itemType, maxStack);
            EditorUtility.SetDirty(definition);
            return definition;
        }

        private static void CreateFloor()
        {
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.position = new Vector3(0f, -0.5f, 6f);
            floor.transform.localScale = new Vector3(16f, 1f, 20f);
            SetMaterial(floor, "Floor", FloorColor);
        }

        private static void CreateEnvironment()
        {
            var environment = new GameObject("Environment");
            CreateEnvironmentCube(environment.transform, "Left Wall", new Vector3(-8f, 1f, 7f), new Vector3(0.5f, 3f, 18f));
            CreateEnvironmentCube(environment.transform, "Right Wall", new Vector3(8f, 1f, 7f), new Vector3(0.5f, 3f, 18f));
            CreateEnvironmentCube(environment.transform, "Back Wall", new Vector3(0f, 1f, -2f), new Vector3(16f, 3f, 0.5f));
            CreateEnvironmentCube(environment.transform, "Front Wall Left", new Vector3(-5.5f, 1f, 16f), new Vector3(5f, 3f, 0.5f));
            CreateEnvironmentCube(environment.transform, "Front Wall Right", new Vector3(5.5f, 1f, 16f), new Vector3(5f, 3f, 0.5f));
            CreateEnvironmentCube(environment.transform, "Generator Platform", new Vector3(0f, 0.05f, 10f), new Vector3(5f, 0.1f, 3f));
            CreateEnvironmentCube(environment.transform, "Storage Crate 1", new Vector3(-5.5f, 0.75f, 8f), new Vector3(1.5f, 1.5f, 1.5f));
            CreateEnvironmentCube(environment.transform, "Storage Crate 2", new Vector3(5.5f, 0.75f, 3f), new Vector3(1.5f, 1.5f, 1.5f));
            CreateEnvironmentCube(environment.transform, "Utility Barrier Left", new Vector3(-5.5f, 0.5f, 12f), new Vector3(3f, 1f, 0.45f));
            CreateUtilityPipes(environment.transform);
            CreateLamps(environment.transform);
        }

        private static void CreateEnvironmentCube(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            SetMaterial(cube, "Environment", EnvironmentColor);
        }

        private static void CreateLight()
        {
            var lightObject = new GameObject("Directional Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.2f;
            lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
        }

        private static void CreateCamera()
        {
            var cameraObject = new GameObject("Main Camera");
            cameraObject.tag = "MainCamera";
            cameraObject.transform.position = new Vector3(0f, 12f, -7f);
            cameraObject.transform.rotation = Quaternion.Euler(48f, 0f, 0f);
            cameraObject.AddComponent<Camera>();
            cameraObject.AddComponent<AudioListener>();
        }

        private static void CreatePlayer(ICollection<GameObject> injectionRoots)
        {
            var player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.transform.position = new Vector3(0f, 1f, 0f);
            Object.DestroyImmediate(player.GetComponent<CapsuleCollider>());
            player.AddComponent<CharacterController>();
            var inputReader = player.AddComponent<PlayerInputReader>();
            player.AddComponent<PlayerController>();
            player.AddComponent<PlayerHealthBinder>();
            var interactionController = player.AddComponent<InteractionController>();

            var interactionOrigin = new GameObject("InteractionOrigin");
            interactionOrigin.transform.SetParent(player.transform);
            interactionOrigin.transform.localPosition = new Vector3(0f, -0.5f, 0.5f);
            interactionController.Configure(inputReader, interactionOrigin.transform);

            SetMaterial(player, "Player", new Color(0.15f, 0.52f, 0.9f));
            injectionRoots.Add(player);
        }

        private static void CreatePickup(
            string name,
            ItemId itemId,
            string actionText,
            Vector3 position,
            Color color,
            ICollection<GameObject> injectionRoots)
        {
            var primitiveType = itemId == ItemId.Fuse ? PrimitiveType.Cylinder : PrimitiveType.Cube;
            var pickup = GameObject.CreatePrimitive(primitiveType);
            pickup.name = name;
            pickup.transform.position = position;
            pickup.transform.localScale = GetPickupScale(itemId);
            if (itemId == ItemId.Fuse)
            {
                Object.DestroyImmediate(pickup.GetComponent<Collider>());
                pickup.AddComponent<BoxCollider>().size = new Vector3(2.2f, 2f, 2.2f);
            }

            var interactable = pickup.AddComponent<PickupItemInteractable>();
            interactable.Configure(itemId);
            interactable.SetActionText(actionText);
            SetMaterial(pickup, itemId.ToString(), color);
            CreatePickupDetails(pickup.transform, itemId, color);
            CreateWorldLabel(pickup.transform, itemId.ToString().ToUpperInvariant(), new Vector3(0f, 1f, 0f), color);
            injectionRoots.Add(pickup);
        }

        private static void CreateGenerator(ICollection<GameObject> injectionRoots)
        {
            var generator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            generator.name = "Generator";
            generator.transform.position = new Vector3(0f, 1f, 10f);
            generator.transform.localScale = new Vector3(1.5f, 1f, 1.5f);
            var interactable = generator.AddComponent<GeneratorInteractable>();
            interactable.SetActionText("Activate Generator");
            SetMaterial(generator, "Generator", new Color(0.1f, 0.12f, 0.14f));
            CreateDecorativePrimitive(generator.transform, "Generator Side Panel Left", PrimitiveType.Cube, new Vector3(-0.9f, 0f, 0f), new Vector3(0.35f, 0.9f, 0.8f), EnvironmentColor, "Environment");
            CreateDecorativePrimitive(generator.transform, "Generator Side Panel Right", PrimitiveType.Cube, new Vector3(0.9f, 0f, 0f), new Vector3(0.35f, 0.9f, 0.8f), EnvironmentColor, "Environment");
            var indicator = CreateDecorativePrimitive(generator.transform, "Generator Status Light", PrimitiveType.Sphere, new Vector3(0f, 0.75f, -0.7f), Vector3.one * 0.22f, new Color(0.9f, 0.25f, 0.12f), "GeneratorStatus");
            generator.AddComponent<GeneratorActivationView>().Configure(indicator.GetComponent<Renderer>(), new Color(0.2f, 1f, 0.35f));
            CreateWorldLabel(generator.transform, "GENERATOR", new Vector3(0f, 1.7f, 0f), Color.white, 0.08f);
            injectionRoots.Add(generator);
        }

        private static void CreateExtractionZone(ICollection<GameObject> injectionRoots)
        {
            var zone = GameObject.CreatePrimitive(PrimitiveType.Cube);
            zone.name = "Extraction Zone";
            zone.transform.position = new Vector3(0f, 0.25f, 15f);
            zone.transform.localScale = new Vector3(5f, 0.5f, 3f);
            zone.GetComponent<BoxCollider>().isTrigger = true;
            zone.AddComponent<ExtractionZone>();
            SetMaterial(zone, "ExtractionZone", new Color(1f, 0.55f, 0.1f, 0.55f));
            CreateExtractionMarkers(zone.transform);
            injectionRoots.Add(zone);
        }

        private static void CreateEnemy(EnemyConfig config, ICollection<GameObject> injectionRoots)
        {
            var enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = "Enemy";
            enemy.transform.position = new Vector3(6f, 1f, 12f);
            SetMaterial(enemy, "Enemy", DangerColor);
            CreateDecorativePrimitive(enemy.transform, "Enemy Warning Core", PrimitiveType.Sphere, new Vector3(0f, 0.7f, -0.45f), Vector3.one * 0.3f, new Color(1f, 0.55f, 0.15f), "DangerAccent");

            var patrolRoot = new GameObject("Enemy Patrol Points");
            var patrolPoints = new[]
            {
                CreatePatrolPoint("Patrol Point 1", patrolRoot.transform, new Vector3(6f, 0f, 9f)),
                CreatePatrolPoint("Patrol Point 2", patrolRoot.transform, new Vector3(6f, 0f, 15f))
            };

            var controller = enemy.AddComponent<EnemyAIController>();
            controller.Configure(config, patrolPoints);
            injectionRoots.Add(enemy);
        }

        private static Transform CreatePatrolPoint(string name, Transform parent, Vector3 position)
        {
            var point = new GameObject(name);
            point.transform.SetParent(parent);
            point.transform.position = position;
            return point.transform;
        }

        private static void CreateHud(ICollection<GameObject> injectionRoots)
        {
            var canvasObject = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = canvasObject.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.GetComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasObject.GetComponent<CanvasScaler>().referenceResolution = new Vector2(1920f, 1080f);

            var hudView = canvasObject.AddComponent<HudView>();
            var healthView = CreateTextView<HealthView>(
                canvasObject.transform,
                "Health",
                new Vector2(20f, -20f),
                TextAnchor.UpperLeft,
                new Vector2(360f, 48f));
            var objectiveView = CreateTextView<ObjectiveView>(
                canvasObject.transform,
                "Objective",
                new Vector2(20f, -76f),
                TextAnchor.UpperLeft,
                new Vector2(900f, 48f));
            var inventoryView = CreateInventoryView(canvasObject.transform);
            var interactionPromptView = CreateTextView<InteractionPromptView>(
                canvasObject.transform,
                "Interaction Prompt",
                new Vector2(0f, 120f),
                TextAnchor.MiddleCenter,
                new Vector2(720f, 56f),
                addCanvasGroup: true,
                centered: true);
            var endGameView = CreateTextView<EndGameView>(
                canvasObject.transform,
                "End Game",
                Vector2.zero,
                TextAnchor.MiddleCenter,
                new Vector2(1000f, 120f),
                fontSize: 52,
                addCanvasGroup: true,
                centered: true);

            hudView.Configure(healthView, objectiveView, inventoryView, interactionPromptView, endGameView);
            injectionRoots.Add(canvasObject);
        }

        private static InventoryView CreateInventoryView(Transform parent)
        {
            var inventoryObject = CreateUiObject("Inventory", parent);
            SetTopLeft(inventoryObject.GetComponent<RectTransform>(), new Vector2(20f, -140f), new Vector2(420f, 240f));
            var inventoryView = inventoryObject.AddComponent<InventoryView>();
            var slotViews = new InventorySlotView[5];

            for (var index = 0; index < slotViews.Length; index++)
            {
                var slotObject = CreateUiObject($"Inventory Slot {index + 1}", inventoryObject.transform);
                SetTopLeft(slotObject.GetComponent<RectTransform>(), new Vector2(0f, -index * 42f), new Vector2(360f, 36f));
                var text = AddText(slotObject, 24, TextAnchor.MiddleLeft);
                var slotView = slotObject.AddComponent<InventorySlotView>();
                slotView.Configure(text);
                slotViews[index] = slotView;
            }

            inventoryView.Configure(slotViews);
            return inventoryView;
        }

        private static T CreateTextView<T>(
            Transform parent,
            string name,
            Vector2 anchoredPosition,
            TextAnchor alignment,
            Vector2 size,
            int fontSize = 28,
            bool addCanvasGroup = false,
            bool centered = false)
            where T : MonoBehaviour
        {
            var viewObject = CreateUiObject(name, parent);
            var rectTransform = viewObject.GetComponent<RectTransform>();
            if (centered)
            {
                SetCentered(rectTransform, anchoredPosition, size);
            }
            else
            {
                SetTopLeft(rectTransform, anchoredPosition, size);
            }

            var text = AddText(viewObject, fontSize, alignment);
            var canvasGroup = addCanvasGroup ? viewObject.AddComponent<CanvasGroup>() : null;
            var view = viewObject.AddComponent<T>();
            switch (view)
            {
                case HealthView healthView:
                    var damageFeedback = viewObject.AddComponent<DamageFeedbackView>();
                    damageFeedback.Configure(text);
                    healthView.Configure(text, damageFeedback);
                    break;
                case ObjectiveView objectiveView:
                    objectiveView.Configure(text);
                    break;
                case InteractionPromptView interactionPromptView:
                    interactionPromptView.Configure(text, canvasGroup);
                    break;
                case EndGameView endGameView:
                    endGameView.Configure(text, canvasGroup);
                    break;
            }

            return view;
        }

        private static GameObject CreateUiObject(string name, Transform parent)
        {
            var gameObject = new GameObject(name, typeof(RectTransform));
            gameObject.transform.SetParent(parent, false);
            return gameObject;
        }

        private static Text AddText(GameObject gameObject, int fontSize, TextAnchor alignment)
        {
            var text = gameObject.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = fontSize;
            text.alignment = alignment;
            text.color = Color.white;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        private static void SetTopLeft(RectTransform rectTransform, Vector2 anchoredPosition, Vector2 size)
        {
            rectTransform.anchorMin = Vector2.up;
            rectTransform.anchorMax = Vector2.up;
            rectTransform.pivot = Vector2.up;
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;
        }

        private static void SetCentered(RectTransform rectTransform, Vector2 anchoredPosition, Vector2 size)
        {
            rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
            rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.anchoredPosition = anchoredPosition;
            rectTransform.sizeDelta = size;
        }

        private static Vector3 GetPickupScale(ItemId itemId)
        {
            return itemId switch
            {
                ItemId.Fuse => new Vector3(0.32f, 0.5f, 0.32f),
                ItemId.Medkit => new Vector3(0.85f, 0.65f, 0.42f),
                ItemId.Keycard => new Vector3(0.95f, 0.12f, 0.62f),
                _ => Vector3.one * 0.65f
            };
        }

        private static void CreatePickupDetails(Transform pickup, ItemId itemId, Color color)
        {
            switch (itemId)
            {
                case ItemId.Fuse:
                    CreateDecorativePrimitive(pickup, "Fuse Cap Top", PrimitiveType.Cylinder, new Vector3(0f, 0.55f, 0f), new Vector3(1.18f, 0.12f, 1.18f), Color.white, "Accent");
                    CreateDecorativePrimitive(pickup, "Fuse Cap Bottom", PrimitiveType.Cylinder, new Vector3(0f, -0.55f, 0f), new Vector3(1.18f, 0.12f, 1.18f), Color.white, "Accent");
                    break;
                case ItemId.Medkit:
                    CreateDecorativePrimitive(pickup, "Medkit Cross Vertical", PrimitiveType.Cube, new Vector3(0f, 0f, -0.55f), new Vector3(0.22f, 0.65f, 0.08f), Color.white, "Accent");
                    CreateDecorativePrimitive(pickup, "Medkit Cross Horizontal", PrimitiveType.Cube, new Vector3(0f, 0f, -0.55f), new Vector3(0.58f, 0.22f, 0.08f), Color.white, "Accent");
                    break;
                case ItemId.Keycard:
                    CreateDecorativePrimitive(pickup, "Keycard Stripe", PrimitiveType.Cube, new Vector3(0f, 0.56f, 0f), new Vector3(0.8f, 0.1f, 0.82f), Color.white, "Accent");
                    break;
            }
        }

        private static void CreateUtilityPipes(Transform parent)
        {
            for (var index = 0; index < 3; index++)
            {
                var pipe = CreateDecorativePrimitive(parent, $"Utility Pipe {index + 1}", PrimitiveType.Cylinder, new Vector3(-7.4f, 1.9f - index * 0.35f, 7f), new Vector3(0.12f, 8f, 0.12f), new Color(0.42f, 0.46f, 0.5f), "Pipe");
                pipe.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            }
        }

        private static void CreateLamps(Transform parent)
        {
            foreach (var position in new[] { new Vector3(-5f, 2.35f, 2f), new Vector3(5f, 2.35f, 8f), new Vector3(0f, 2.35f, 14.5f) })
            {
                var lamp = CreateDecorativePrimitive(parent, "Utility Lamp", PrimitiveType.Cube, position, new Vector3(1.5f, 0.16f, 0.4f), new Color(1f, 0.75f, 0.35f), "Lamp");
                var light = lamp.AddComponent<Light>();
                light.type = LightType.Point;
                light.range = 5f;
                light.intensity = 1.2f;
                light.color = new Color(1f, 0.72f, 0.42f);
            }
        }

        private static void CreateExtractionMarkers(Transform zone)
        {
            CreateDecorativePrimitive(zone, "Extraction Boundary Left", PrimitiveType.Cube, new Vector3(-0.48f, 0.7f, 0f), new Vector3(0.05f, 1.3f, 1f), ExtractionColor, "ExtractionMarker");
            CreateDecorativePrimitive(zone, "Extraction Boundary Right", PrimitiveType.Cube, new Vector3(0.48f, 0.7f, 0f), new Vector3(0.05f, 1.3f, 1f), ExtractionColor, "ExtractionMarker");
            CreateWorldLabel(zone, "EXTRACTION", new Vector3(0f, 3.4f, 0f), ExtractionColor, 0.035f);
        }

        private static GameObject CreateDecorativePrimitive(
            Transform parent,
            string name,
            PrimitiveType primitiveType,
            Vector3 localPosition,
            Vector3 localScale,
            Color color,
            string materialName = null)
        {
            var decoration = GameObject.CreatePrimitive(primitiveType);
            decoration.name = name;
            decoration.transform.SetParent(parent, false);
            decoration.transform.localPosition = localPosition;
            decoration.transform.localScale = localScale;
            Object.DestroyImmediate(decoration.GetComponent<Collider>());
            SetMaterial(decoration, materialName ?? name, color);
            return decoration;
        }

        private static void CreateWorldLabel(Transform parent, string text, Vector3 localPosition, Color color, float scale = 0.16f)
        {
            var labelObject = new GameObject($"{text} Label");
            labelObject.transform.SetParent(parent, false);
            labelObject.transform.localPosition = localPosition;
            labelObject.transform.localScale = Vector3.one * scale;
            labelObject.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
            var label = labelObject.AddComponent<TextMesh>();
            label.text = text;
            label.anchor = TextAnchor.MiddleCenter;
            label.alignment = TextAlignment.Center;
            label.fontSize = 42;
            label.color = color;
        }

        private static void SetMaterial(GameObject gameObject, string assetName, Color color)
        {
            gameObject.GetComponent<Renderer>().sharedMaterial = GetOrCreateMaterial(assetName, color);
        }

        private static Material GetOrCreateMaterial(string assetName, Color color)
        {
            var safeName = assetName.Replace(" ", string.Empty);
            var assetPath = $"{MaterialFolder}/{safeName}.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(assetPath);
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            if (material == null)
            {
                material = new Material(shader);
                AssetDatabase.CreateAsset(material, assetPath);
            }

            material.shader = shader;
            material.color = color;
            EditorUtility.SetDirty(material);
            return material;
        }

        private static void EnsureFolder(string parentFolder, string folderName)
        {
            var folderPath = $"{parentFolder}/{folderName}";
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(parentFolder, folderName);
            }
        }
    }
}
