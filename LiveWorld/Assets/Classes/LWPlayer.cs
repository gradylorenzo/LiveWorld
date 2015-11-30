using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace LiveWorld
{
    public abstract class LWPlayer : MonoBehaviour
    {
        public string player_name;

        [System.Serializable]
        public class Statistics
        {
            public int health;
            public int energy;
            public float attack;
            public float defense;
            public float speed;
        }

        public Statistics Stats;
    }
}
