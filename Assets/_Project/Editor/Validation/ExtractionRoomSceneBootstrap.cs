using System.Collections.Generic;
using ExtractionRoom.DI;
using ExtractionRoom.Gameplay;
using ExtractionRoom.Interaction;
using ExtractionRoom.Items;
using ExtractionRoom.Player;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExtractionRoom.Editor.Validation
{
    public static class ExtractionRoomSceneBootstrap
    {
        private const string ScenePath = "Assets/_Project/Scenes/ExtractionRoom.unity";
        private const string ConfigFolder = "Assets/_Project/Configs/Items";

        [MenuItem("ExtractionRoom/BootstrapPrototypeScene")]
        public static void Bootstrap()
        {
            EnsureFolder("Assets/_Project/Configs", "Items");

            var definitions = new[]
            {
                GetOrCreateDefinition("Fuse", ItemId.Fuse, ItemType.Fuse, 3),
                GetOrCreateDefinition("Medkit", ItemId.Medkit, ItemType.Medkit, 2),
                GetOrCreateDefinition("Keycard", ItemId.Keycard, ItemType.Keycard, 1)
            };

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var injectionRoots = new List<GameObject>();

            CreateFloor();
            CreateLight();
            CreateCamera();
            CreatePlayer(injectionRoots);
            CreateFusePickup(new Vector3(0f, 0.5f, 3f), injectionRoots);
            CreateFusePickup(new Vector3(2f, 0.5f, 5f), injectionRoots);
            CreateFusePickup(new Vector3(-2f, 0.5f, 7f), injectionRoots);
            CreateGenerator(injectionRoots);
            CreateExtractionZone(injectionRoots);

            var compositionRoot = new GameObject("CompositionRoot");
            var lifetimeScope = compositionRoot.AddComponent<GameLifetimeScope>();
            lifetimeScope.ConfigureScene(definitions, injectionRoots);

            EditorSceneManager.SaveScene(scene, ScenePath);
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeGameObject = compositionRoot;
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

        private static void CreateFusePickup(Vector3 position, ICollection<GameObject> injectionRoots)
        {
            var pickup = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            pickup.name = "Fuse Pickup";
            pickup.transform.position = position;
            pickup.transform.localScale = Vector3.one * 0.6f;
            var interactable = pickup.AddComponent<PickupItemInteractable>();
            interactable.Configure(ItemId.Fuse);
            interactable.SetActionText("Pick Up Fuse");
            SetColor(pickup, new Color(1f, 0.75f, 0.15f));
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
            SetColor(generator, new Color(0.25f, 0.8f, 0.35f));
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

        private static void SetColor(GameObject gameObject, Color color)
        {
            gameObject.GetComponent<Renderer>().material.color = color;
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
