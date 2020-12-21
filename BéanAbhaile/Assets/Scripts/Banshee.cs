using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Banshee : MonoBehaviour
{
    /*
    Banshee must wail every 3-7 seconds;
    -Follow a random path through the house (visiting important sites more often)
    If hears the player heads roughly in their direction, the closer the player the mosre accurate the direction.
    check a scaled sphere around the player and finds a random point in it that's on the navmesh.
    player awareness metre.
    */

    #region Variables
    public bool AIEnabled = false;
    private float _neededProximity = 1;

    //Objects
    [Header("GameObjects")]
    public GameObject player;
    public AudioSource huntWailSound;
    public AudioSource painWailSound;
    public LayerMask bansheeMask;
    //private bool _wail = true;

    private Player _playerController;
    private BoxCollider _playerSightTrigger;
    public GameObject deathLocation;

    private string _state;
    private bool _nearDeathLocation;

    [SerializeField]private float _moveSpeed = 2;
    public float _playerAwareness;
    private float _playerAwarenessMax = 75;
    private float _playerAwarenessProximityModifier = 50f;

    private Vector3 _destination;
    private Vector3 _previousLocation;
    public Transform[] keyPlaces;
    private NavMeshAgent _meshAgent;
    private NavMeshPath _navMeshPath;
    private RaycastHit _hit;

    //Senses
    private float _playerNoise;
    private float _playerDistance;
    private float _sightRange = 10;
    private bool _inSightRange = false;

    //Timers
    private float _randomDestinationTimer;
    //private float _huntWailTimer = 0;
    private float _wailTimer;
    private float _minimumTime = 20;
    private float _maximumTime = 40;
    private float _playerAwarenessDecayRate = 2;

    //ending variables
    [Header("Ending Variables")]
    public bool noose = false;
    public bool brush = false;
    public bool reflection = false;
    public bool mirrorFace = false;

    #endregion

    void OnEnable()
    {
        _playerSightTrigger = this.gameObject.AddComponent<BoxCollider>();
        _playerSightTrigger.isTrigger = true;
        _playerSightTrigger.size = new Vector3(5, 3, _sightRange);
        _playerSightTrigger.center = -this.transform.right * (_sightRange / 2);
        _playerController = player.GetComponent<Player>();
        _meshAgent = this.GetComponent<NavMeshAgent>();
        _randomDestinationTimer = Random.Range(20, 30);
        _wailTimer = Random.Range(_minimumTime, _maximumTime);
        GenerateDestination();
    }

    public void AIEnable()
    {
        _minimumTime /= 2;
        _maximumTime /= 2;
        AIEnabled = true;
    }

    void Update()
    {
        //victory conditions
        if (AIEnabled) {
            if (noose && brush && reflection)
            {
                this.GetComponentInChildren<ParticleSystem>().Play();
                Destroy(this.gameObject);
            }
            //Variable Updates
            _playerDistance = Vector3.Distance(this.transform.position, player.transform.position);

            //update awareness
            PlayerAwarenessChange();

            //update meshAgent Movement
            _meshAgent.SetDestination(_destination);
            _meshAgent.speed = _moveSpeed;

            PlayerAwarenessChange();
        }

        Wail(_minimumTime,_maximumTime);
    }

    private void GenerateDestination()
    {
        Vector3 tempVector = /*keyPlaces[Random.Range(0, keyPlaces.Length)].position;*/ GetRandomLocation();
        //SUCK MY DICK
        if (Vector3.Distance(tempVector, _destination) >= _neededProximity && _previousLocation != tempVector)
        {
            _destination = tempVector;
        }
    }

    private void GeneratePositionNearPlayer()
    {
        Vector3 randomPos = Random.insideUnitSphere * ((_playerAwarenessMax - _playerAwareness) * _playerAwarenessProximityModifier) + player.transform.position;

        NavMeshHit hit;

        NavMesh.SamplePosition(randomPos, out hit, ((_playerAwarenessMax - _playerAwareness) * _playerAwarenessProximityModifier), NavMesh.AllAreas);
        Debug.Log($"Hit pos: {hit.position} {_state}");
        _destination = hit.position;
    }

    #region Senses
    //Sight & LOS
    private float Sight()
    {
        Vector3 playerDirection = new Vector3(player.transform.position.x - this.transform.position.x, player.transform.position.y - this.transform.position.y - 2, player.transform.position.z - this.transform.position.z).normalized;
        if (Physics.Linecast(this.transform.position, Player.PlayerPosition, out _hit, bansheeMask)) { 
            Debug.Log("Test");
            if (_inSightRange == true)
            {
                Debug.Log("BEEP BEEP YOU SAD FUCK");
                return 50;
            }
            else
            {
                return 0;
            }
        }
        return 0;
    }

    //Hearing
    private float NoiseLevels()
    {
        float noise = (_playerController.noise / Vector3.Distance(player.transform.position, this.transform.position)) * _playerAwarenessProximityModifier * Time.deltaTime;
        return noise;
    }

    #endregion

    private void PlayerAwarenessChange()
    {
        float _playerAwarenessMinimum = 0;

        _playerAwarenessMinimum += NoiseLevels();
        _playerAwarenessMinimum += Sight();
        //set value to minimum or reduce it over time.
        _playerAwareness = (_playerAwareness < _playerAwarenessMinimum) ? _playerAwareness = _playerAwarenessMinimum : _playerAwareness - (_playerAwarenessDecayRate * Time.deltaTime);

        PlayerAwareness();
    }

    private void PlayerAwareness()
    {
        if (_playerAwareness > _playerAwarenessMax)
        {
            _playerAwareness = _playerAwarenessMax;
        }
        //adjust player awarenss, needs revision

        //Get awareness levels and behaviours
        if (_playerAwareness <= 10)
        {
            _state = "Roam";
        }
        else if (_playerAwareness <= 25)
        {
            _state = "Aware";
        }
        else if (_playerAwareness <= _playerAwarenessMax)
        {
            _state = "Hunt";
        }

        //universal movement checks
        //if arrived at location
        if (Vector3.Distance(transform.position, _destination) <= _neededProximity)
        {
            _previousLocation = _destination;
            if (_state == "Roam")
            {
                GenerateDestination();
            }
            else if (_state == "Aware")
            {
                GeneratePositionNearPlayer();
            }
        }
        //Debug.Log(_state);

        switch (_state)
        {
            case "Roam":
                _randomDestinationTimer -= Time.deltaTime;
                if (_randomDestinationTimer <= 0)
                {
                    GenerateDestination();
                    _randomDestinationTimer = Random.Range(20, 40);
                }
                break;

            case "Aware":
                break;

            case "Hunt":
                _destination = player.transform.position;
                break;
        }
    }

    Vector3 GetRandomLocation()
    {
        NavMeshTriangulation navMeshData = NavMesh.CalculateTriangulation();

        // Pick the first indice of a random triangle in the nav mesh
        int t = Random.Range(0, navMeshData.indices.Length - 3);

        // Select a random point on it
        Vector3 point = Vector3.Lerp(navMeshData.vertices[navMeshData.indices[t]], navMeshData.vertices[navMeshData.indices[t + 1]], Random.value);
        Vector3.Lerp(point, navMeshData.vertices[navMeshData.indices[t + 2]], Random.value);

        return point;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") _inSightRange = true;
        //if (other.name == "Mirror") mirrorFace = true;
        Debug.Log(_inSightRange);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player") _inSightRange = false;
        if (other.name == "Mirror") mirrorFace = false;
    }

    /*private void HuntWail()
    {
        if (_state == "Hunt")
        {
            if (!huntWailSound.isPlaying) {
                _huntWailTimer -= Time.deltaTime;
                if (_huntWailTimer <= 0)
                {
                    _huntWailTimer = Random.Range(4, 5);
                    huntWailSound.Play();
                }
            }
        }
    }

    private void PainWail()
    {
        _painWailTimer -= Time.deltaTime;
        if (_painWailTimer <= 0 && _wail)
        {
            _wail = false;
            painWailSound.Play();
        }
        if (Vector3.Distance(player.transform.position, deathLocation.transform.position) <= 5 && !_nearDeathLocation)
        {
            _nearDeathLocation = true;
            painWailSound.Play();
        }
        if (Vector3.Distance(player.transform.position, deathLocation.transform.position) >= 6) _nearDeathLocation = false;
    }*/

    private void Wail(float minimumTime, float maximumTime)
    {
        _wailTimer -= Time.deltaTime;
        if (_wailTimer <= 0)
        {
            int vary = Random.Range(0,1);
            if (vary == 1) painWailSound.Play();
            else huntWailSound.Play();
            _wailTimer = Random.Range(minimumTime, maximumTime);
        }
    }

    /*private void OnDrawGizmos()
    {
        Gizmos.DrawLine((this.transform.position, new Vector3(player.transform.position.x - this.transform.position.x, player.transform.position.y - this.transform.position.y - 2, player.transform.position.z - this.transform.position.z).normalized * 10);
    }*/
}
