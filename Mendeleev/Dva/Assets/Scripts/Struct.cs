using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dva
{
    public struct AtomData
    {
        public int ID;
        public string Name;
        public string Symbol;
        public string Material;
        public string Composition;
    }

    public struct ParticleData
    {
        public TMP_Text Text;
        public Material Material;
        public Vector3 Size;
        public float Speed;
    }
}