using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Dva
{
    public class FeaturesManager : MonoBehaviour
    {
        [SerializeField] private float _particleSpeed = 1f;
        [SerializeField] private float _electronOrbitSpeed = 1f;
        [SerializeField] private Transform _leftBoarder;
        [SerializeField] private Transform _rightBoarder;
        [SerializeField] private Transform _topBoarder;
        [SerializeField] private Transform _bottomBoarder;
        [SerializeField] private Transform _field;
        [SerializeField] private int _decayCountDown = 3;
        [SerializeField] private int _eventCounter = 10;
        [SerializeField] private int _FOVRange = 2;
        [SerializeField] private TMP_Text _atomName;
        [SerializeField] private TMP_Text _atomComposition;
        [SerializeField] private TMP_Text _atomSymbol;
        [SerializeField] private TMP_Text _atomDecay;
        [SerializeField] private TMP_Text _atomSymbolObject;
        [SerializeField] private TMP_Text _statisticText;
        [SerializeField] private ParticleSystem _particleSystem;
        [SerializeField] private Material[] _elementsMaterials;
        [SerializeField] private AudioSource _absorbAudio;
        [SerializeField] private AudioSource _explodeAudio;
        [SerializeField] private AudioSource _ticAudio;
        [SerializeField] private AudioSource _startAudio;

        public float ParticleSpeed { get { return _particleSpeed; } set { _particleSpeed = value; } }
        public float ElectronOrbitSpeed => _electronOrbitSpeed;
        public Transform LeftBoarder { get { return _leftBoarder; } set { _leftBoarder = value; } }
        public Transform RightBoarder { get { return _rightBoarder; } set { _rightBoarder = value; } }
        public Transform TopBoarder { get { return _topBoarder; } set { _topBoarder = value; } }
        public Transform BottomBoarder { get { return _bottomBoarder; } set { _bottomBoarder = value; } }
        public Transform Field { get { return _field; } set { _field = value; } }
        public int DecayCountDown { get { return _decayCountDown; } }
        public int EventCountDown { get { return _eventCounter; } }
        public TMP_Text AtomName { get { return _atomName; } }
        public TMP_Text AtomComposition { get { return _atomComposition; } }
        public TMP_Text AtomSymbol { get { return _atomSymbol; } }
        public TMP_Text AtomDecay { get { return _atomDecay; } }
        public TMP_Text AtomSymbolObject { get { return _atomSymbolObject; } }
        public TMP_Text StatisticText { get { return _statisticText; } }
        public int FOVRange { get { return _FOVRange; } }
        public ParticleSystem ParticleSystem => _particleSystem;
        public Material[] ElementsMaterials => _elementsMaterials;
        public AudioSource AbsorbAudio => _absorbAudio;
        public AudioSource ExplodeAudio => _explodeAudio;
        public AudioSource TicAudio => _ticAudio;
        public AudioSource StartAudio => _startAudio;
    }
}