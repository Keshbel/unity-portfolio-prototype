using ExtractionRoom.Gameplay;
using UnityEngine;

namespace ExtractionRoom.AI
{
    public sealed class EnemyRuntimeState
    {
        public EnemyRuntimeState(IHealth playerHealth, Vector3[] patrolPoints)
        {
            PlayerHealth = playerHealth;
            PatrolPoints = patrolPoints ?? System.Array.Empty<Vector3>();
        }

        public IHealth PlayerHealth { get; }

        public Vector3[] PatrolPoints { get; }

        public Vector3 EnemyPosition { get; set; }

        public Vector3 PlayerPosition { get; set; }

        public Vector3 Destination { get; set; }

        public bool ShouldMove { get; set; }

        public int PatrolPointIndex { get; set; }
    }
}
