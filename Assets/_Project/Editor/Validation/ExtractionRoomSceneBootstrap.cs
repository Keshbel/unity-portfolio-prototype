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

        [MenuItem("ExtractionRoom/BootstrapPrototypeScene")]
        public static void Bootstrap()
        {
            EnsureFolder("Assets/_Project/Configs", "Items");
            EnsureFolder("Assets/_Project/Configs", "AI");

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
            CreatePickup("Fuse Pickup 1", ItemId.Fuse, "Pick Up Fuse", new Vector3(0f, 0.5f, 3f), new Color(0.2f, 0.65f, 1f), injectionRoots);
            CreatePickup("Fuse Pickup 2", ItemId.Fuse, "Pick Up Fuse", new Vector3(0f, 0.5f, 5f), new Color(0.2f, 0.65f, 1f), injectionRoots);
            CreatePickup("Fuse Pickup 3", ItemId.Fuse, "Pick Up Fuse", new Vector3(0f, 0.5f, 7f), new Color(0.2f, 0.65f, 1f), injectionRoots);
            CreatePickup("Medkit Pickup", ItemId.Medkit, "Pick Up Medkit", new Vector3(-4f, 0.5f, 5f), new Color(0.2f, 0.85f, 0.35f), injectionRoots);
            CreatePickup("Keycard Pickup", ItemId.Keycard, "Pick Up Keycard", new Vector3(4f, 0.5f, 8f), new Color(1f, 0.8f, 0.15f), injectionRoots);
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
            SetColor(floor, new Color(0.2f, 0.22f, 0.25f));
        }

        private static void CreateEnvironment()
        {
            var environment = new GameObject("Environment");
            CreateEnvironmentCube(environment.transform, "Left Wall", new Vector3(-8f, 1f, 7f), new Vector3(0.5f, 3f, 18f));
            CreateEnvironmentCube(environment.transform, "Right Wall", new Vector3(8f, 1f, 7f), new Vector3(0.5f, 3f, 18f));
            CreateEnvironmentCube(environment.transform, "Back Wall", new Vector3(0f, 1f, -2f), new Vector3(16f, 3f, 0.5f));
            CreateEnvironmentCube(environment.transform, "Generator Platform", new Vector3(0f, 0.05f, 10f), new Vector3(5f, 0.1f, 3f));
            CreateEnvironmentCube(environment.transform, "Storage Crate 1", new Vector3(-5.5f, 0.75f, 8f), new Vector3(1.5f, 1.5f, 1.5f));
            CreateEnvironmentCube(environment.transform, "Storage Crate 2", new Vector3(5.5f, 0.75f, 3f), new Vector3(1.5f, 1.5f, 1.5f));
        }

        private static void CreateEnvironmentCube(Transform parent, string name, Vector3 position, Vector3 scale)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent);
            cube.transform.position = position;
            cube.transform.localScale = scale;
            SetColor(cube, new Color(0.3f, 0.32f, 0.36f));
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

            SetColor(player, new Color(0.2f, 0.65f, 1f));
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
            var pickup = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pickup.name = name;
            pickup.transform.position = position;
            pickup.transform.localScale = Vector3.one * 0.65f;
            var interactable = pickup.AddComponent<PickupItemInteractable>();
            interactable.Configure(itemId);
            interactable.SetActionText(actionText);
            SetColor(pickup, color);
            injectionRoots.Add(pickup);
        }

        private static void CreateGenerator(ICollection<GameObject> injectionRoots)
        {
            var generator = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            generator.name = "Generator";
            generator.transform.position = new Vector3(0f, 1f, 10f);
            generator.transform.localScale = new Vector3(1.5f, 1f, 1.5f);
            var interactable = generator.AddComponent<GeneratorInteractable>();
            generator.AddComponent<GeneratorActivationView>();
            interactable.SetActionText("Activate Generator");
            SetColor(generator, new Color(0.12f, 0.15f, 0.18f));
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
            SetColor(zone, new Color(0.15f, 0.85f, 0.95f));
            injectionRoots.Add(zone);
        }

        private static void CreateEnemy(EnemyConfig config, ICollection<GameObject> injectionRoots)
        {
            var enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            enemy.name = "Enemy";
            enemy.transform.position = new Vector3(6f, 1f, 12f);
            SetColor(enemy, new Color(0.85f, 0.2f, 0.2f));

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

        private static void SetColor(GameObject gameObject, Color color)
        {
            var renderer = gameObject.GetComponent<Renderer>();
            var material = new Material(renderer.sharedMaterial)
            {
                color = color
            };
            renderer.sharedMaterial = material;
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
