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
        [SerializeField] private float _spawnExclusionRadius = 1.5f;

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

        private ParticleSpawner _particleSpawner;
        private FieldEventRunner _fieldEventRunner;
        private LivesController _livesController;
        private FeaturesManager _featuresManager;
        private Player _player;
        private Atom _atom;

        public List<GameObject> ParticlesCounter => _particleSpawner.ParticlesCounter;
        public List<GameObject> BlackHolesCounter => _particleSpawner.BlackHolesCounter;
        public List<GameObject> TimeFastCounter => _particleSpawner.TimeFastCounter;
        public List<GameObject> TimeSlowCounter => _particleSpawner.TimeSlowCounter;
        public List<GameObject> FieldBiggerCounter => _particleSpawner.FieldBiggerCounter;
        public List<GameObject> FieldSmallerCounter => _particleSpawner.FieldSmallerCounter;
        public List<GameObject> NeutronFastCounter => _particleSpawner.NeutronFastCounter;
        public List<GameObject> LivesCounter => _particleSpawner.LivesCounter;

        public List<GameObject> ParticleCounter => ParticlesCounter;
        public List<GameObject> LivesList => _livesController.LivesList;

        public bool IsBlackHoleActive => _fieldEventRunner.IsBlackHoleActive;

        private void Awake()
        {
            _featuresManager = FindObjectOfType<FeaturesManager>();
            _atom = FindObjectOfType<Atom>();
            _player = FindObjectOfType<Player>();
            _particleSpawner = new ParticleSpawner(this, _featuresManager, _atom, _particleParent,
                _neutron, _electron, _proton, _blackHole, _timeFastParticle, _timeSlowParticle,
                _fieldBiggerParticle, _fieldSmallerParticle, _fastNeutronParticle, _lives,
                _maxSpecialAmount, _particleRenewTime, _maxLivesInField, _livesTimeAppear, _spawnExclusionRadius);
            _fieldEventRunner = new FieldEventRunner(this, _featuresManager, _particleSpawner, _player, _atom);
        }
        void Start()
        {
            _livesController = new LivesController(_particleSpawner, _livesCanvas, _livesHolder, _livesAmount);
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

        //fraction of the field's width/height kept clear along each edge, so patrol/spawn points
        //never land flush against the boundary (particles could never quite reach a point sitting
        //on the boundary wall itself, and would just sit stuck against it)
        private const float c_EdgeMarginFraction = 0.05f;

        public Vector3 GetRandomPosition(float leftBoarder, float rightBoarder, float topBoarder, float bottomBoarder)
        {
            return GetRandomPosition(leftBoarder, rightBoarder, topBoarder, bottomBoarder, null, 0f);
        }

        //same as above, but rejects points closer than excludeRadius to excludeCenter - used when
        //spawning particles so they don't appear right on top of the atom
        public Vector3 GetRandomPosition(float leftBoarder, float rightBoarder, float topBoarder, float bottomBoarder,
            Vector3? excludeCenter, float excludeRadius)
        {
            float marginX = (rightBoarder - leftBoarder) * c_EdgeMarginFraction;
            float marginY = (topBoarder - bottomBoarder) * c_EdgeMarginFraction;

            Vector3 position;
            int attempts = 0;
            do
            {
                float x = UnityEngine.Random.Range(leftBoarder + marginX, rightBoarder - marginX);
                float y = UnityEngine.Random.Range(bottomBoarder + marginY, topBoarder - marginY);
                position = new Vector3(x, y, -1);
                attempts++;
            }
            while (excludeCenter.HasValue && Vector3.Distance(position, excludeCenter.Value) < excludeRadius && attempts < 10);

            return position;
        }

        //special particle eventcall
        public void EventCall(SpecialParticleType particle)
        {
            if (particle == SpecialParticleType.Lives)
            {
                _livesController.LivesEvent();
            }
            else
            {
                _fieldEventRunner.Trigger(particle);
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
