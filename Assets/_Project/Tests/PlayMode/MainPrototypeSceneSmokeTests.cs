using System.Collections;
using ExtractionRoom.DI;
using ExtractionRoom.Gameplay;
using ExtractionRoom.Player;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace ExtractionRoom.Tests.PlayMode
{
    public sealed class MainPrototypeSceneSmokeTests
    {
        [UnityTest]
        public IEnumerator MainPrototype_LoadsEssentialSceneObjects()
        {
            var operation = SceneManager.LoadSceneAsync("MainPrototype", LoadSceneMode.Single);
            Assert.That(operation, Is.Not.Null);

            while (!operation.isDone)
            {
                yield return null;
            }

            AssertComponent<GameLifetimeScope>("CompositionRoot");
            AssertComponent<PlayerController>("Player");
            AssertComponent<Canvas>("HUD");
            AssertComponent<GeneratorInteractable>("Generator");
            AssertComponent<ExtractionZone>("Extraction Zone");
        }

        private static void AssertComponent<T>(string objectName) where T : Component
        {
            var gameObject = GameObject.Find(objectName);
            Assert.That(gameObject, Is.Not.Null, $"Expected scene object '{objectName}'.");
            Assert.That(gameObject.GetComponent<T>(), Is.Not.Null, $"Expected '{objectName}' to have {typeof(T).Name}.");
        }
    }
}
