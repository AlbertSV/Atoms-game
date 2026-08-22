using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dva
{
    //main game mechanics
    public class GameControl : MonoBehaviour
    {
        [Header("Balance values")]
        [SerializeField] private Transform _particleParent;
        [FormerlySerializedAs("_maxParticleAmount")]
        [SerializeField] public int MaxParticleAmount = 10;
        [SerializeField] private int _maxSpecialAmount = 2;
        [SerializeField] private int _particleRenewTime = 15;
        [SerializeField] private int _livesAmount = 3;
        [SerializeField] private int _maxLivesInField = 1;
        [SerializeField] private float _livesTimeAppear = 30f;

        [Header("Particles")]
        [SerializeField] private GeneralParticle _neutron;
        [SerializeField] private GeneralParticle _electron;
        [SerializeField] private GeneralParticle _proton;
        [SerializeField] private SpecialParticle _blackHole;
        [SerializeField] private SpecialParticle _timeFastParticle;
        [SerializeField] private SpecialParticle _timeSlowParticle;
        [SerializeField] private SpecialParticle _fieldBiggerParticle;
        [SerializeField] private SpecialParticle _fieldSmallerParticle;
        [SerializeField] private SpecialParticle _fastNeutronParticle;
        [SerializeField] private SpecialParticle _lives;
        [SerializeField] private GameObject _livesCanvas;
        [SerializeField] private Transform _livesHolder;
        [SerializeField] private GameObject _endMenu;

        [FormerlySerializedAs("_livesList")]
        [HideInInspector]
        public List<GameObject> LivesList;

        private ParticleSpawner _particleSpawner;
        private FieldEventRunner _fieldEventRunner;
        private FeaturesManager _featuresManager;
        private Player _player;
        private Atom _atom;
        private float _liveStepCanvas = 0.1f;

        public List<GameObject> ParticlesCounter => _particleSpawner.ParticlesCounter;
        public List<GameObject> BlackHolesCounter => _particleSpawner.BlackHolesCounter;
        public List<GameObject> TimeFastCounter => _particleSpawner.TimeFastCounter;
        public List<GameObject> TimeSlowCounter => _particleSpawner.TimeSlowCounter;
        public List<GameObject> FieldBiggerCounter => _particleSpawner.FieldBiggerCounter;
        public List<GameObject> FieldSmallerCounter => _particleSpawner.FieldSmallerCounter;
        public List<GameObject> NeutronFastCounter => _particleSpawner.NeutronFastCounter;
        public List<GameObject> LivesCounter => _particleSpawner.LivesCounter;

        public List<GameObject> ParticleCounter => ParticlesCounter;

        public bool IsBlackHoleActive => _fieldEventRunner.IsBlackHoleActive;

        private void Awake()
        {
            _featuresManager = FindObjectOfType<FeaturesManager>();
            _atom = FindObjectOfType<Atom>();
            _player = FindObjectOfType<Player>();
            _particleSpawner = new ParticleSpawner(this, _featuresManager, _atom, _particleParent,
                _neutron, _electron, _proton, _blackHole, _timeFastParticle, _timeSlowParticle,
                _fieldBiggerParticle, _fieldSmallerParticle, _fastNeutronParticle, _lives,
                _maxSpecialAmount, _particleRenewTime, _maxLivesInField, _livesTimeAppear);
            _fieldEventRunner = new FieldEventRunner(this, _featuresManager, _particleSpawner, _player, _atom);
        }
        void Start()
        {
            LivesList = new List<GameObject>();
            LifesCreation(_livesAmount);
        }

        void Update()
        {
            _particleSpawner.ParticleSpawn();
            _particleSpawner.SpecialParticleSpawn();
            _particleSpawner.ParticleRenew();
        }

        public void ReturnGeneralParticle(GeneralParticleType type, GameObject instance)
        {
            _particleSpawner.ReturnGeneralParticle(type, instance);
        }

        public void ReturnSpecialParticle(SpecialParticleType type, GameObject instance)
        {
            _particleSpawner.ReturnSpecialParticle(type, instance);
        }

        public Vector3 GetRandomPosition(float leftBoarder, float rightBoarder, float topBoarder, float bottomBoarder)
        {
            float x = UnityEngine.Random.Range(leftBoarder, rightBoarder);
            float y = UnityEngine.Random.Range(bottomBoarder, topBoarder);
            float z = -1;

            return new Vector3(x, y, z);
        }

        //special particle eventcall
        public void EventCall(SpecialParticleType particle)
        {
            if (particle == SpecialParticleType.Lives)
            {
                LivesEvent();
            }
            else
            {
                _fieldEventRunner.Trigger(particle);
            }
        }

        private void LivesEvent()
        {

            if (LivesList.Count < 5)
            {
                Transform lastLifePosition = LivesList[LivesList.Count - 1].gameObject.transform;
                GameObject life = Instantiate(_livesCanvas, new Vector3(lastLifePosition.position.x + _liveStepCanvas, _livesHolder.position.y, _livesHolder.position.z)
                        , _livesCanvas.transform.rotation, _livesHolder);

                LivesList.Add(life);
                _particleSpawner.ResetLivesTimer();
            }
        }

        //add lives to the field
        private void LifesCreation(int lifeAmount)
        {
            for(int i=0; i< lifeAmount; i++)
            {
                GameObject life = Instantiate(_livesCanvas, new Vector3(_livesHolder.position.x + i * _liveStepCanvas, _livesHolder.position.y, _livesHolder.position.z)
                    , _livesCanvas.transform.rotation, _livesHolder);

                LivesList.Add(life);
            }
        }

        //ending game if amount of lives = 0
        public void EndGame()
        {
            Time.timeScale = 0f;
            _endMenu.SetActive(true);
            int maxScore = PlayerPrefs.GetInt("Statistic");
            if (_atom.StatisticScore > maxScore)
            {
                PlayerPrefs.SetInt("Statistic", _atom.StatisticScore);
                _endMenu.transform.GetChild(0).GetComponent<TMP_Text>().text = "New Score: " + _atom.StatisticScore;
            }
            else
            {
                _endMenu.transform.GetChild(0).GetComponent<TMP_Text>().text = "Score: " + _atom.StatisticScore;
            }
        }
    }
}
