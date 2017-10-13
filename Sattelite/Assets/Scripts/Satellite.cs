using UnityEngine;
using System.Collections;


public enum satTypes {
    Debris = -1,
    None,
    Research,
    Transport,
    Surveillance,
    Weather,
    Observation,
    Navigation,
    Communications,
    Recovery,
    Tether,
    Spacestation,
    Telescope,
    Weapon,
    Garbage,
    Powersupply,
    Evacuation
}

public class Satellite : MonoBehaviour {

    public GameManager gameManager;

    public satTypes satType;
    public bool derelict = false;
    public enum launchStatuses {
        Constructing,
        Waiting,
        Launching,
        Orbiting,
		Landing,
		Drifting,
        Falling
    }
    public launchStatuses launchStatus = 0;

    public factions faction;
    public float startingOrbit = 50f; // how far the satellite will be initially put in orbit at launch
    public float fuel = 100f;       // amount of fuel available for course correction.
    public float mass = 10f;        // in case of collision, heavier objects just plow through lighter objects and sustain less damage.
    public float health = 100f;     // amount of damage it can take from space debris etc. 
    public bool radioTransmitter = true; // if false, the satellite will need to reenter earth's orbit after a short time as collected data is physical.
    public bool solarPowered = false;   // replenishes fuel over time if true.
    public bool heatShield = false; // If present, heat shields will let this thing land on earth properly, and possibly protect it against space weaponry.
    public float orbitalDecay = 0f; // the rate at which the orbit slowly decreases.
    public float maxOrbit = 200f;   // how far the satellite can go from earth before gravity loses its pull and the object drifts off into space


    // factions fac, float orbS, float fue, float mas, float hea, bool rad, bool sol, bool shi

    public float downwardSpeed = 0.0f;  // thruster controlled speed to gain or lose altitude.
    public float orbitalDistance = 0f;
    public bool workActive = false; // Whether the sat is currently doing its job or not. 
    public bool workFinished = false;
    public float workProgressL = 0f;    // work progress of the entire sat
    public float workProgressS = 0f;    // work progress of this specific level of work of a sat
    float orbitSpeed = 0f;      // How fast the object moves in its orbit
    protected GameObject planetToOrbit;





    public virtual void Start() {
        planetToOrbit = GameObject.Find("Planet");
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void Initialize(factions fac, float orbS, float fue, float mas, float hea, bool rad, bool sol, bool shi) {
		faction = fac;
        startingOrbit = orbS;
        fuel = fue;
        mass = mas;
        health = hea;
        radioTransmitter = rad;
        solarPowered = sol;
        heatShield = shi;
        orbitalDecay = Random.Range(70, 130) / 100f;    // Set the orbital decay to something random so not all satellite orbits decay equally.
    }

    public virtual void Update() {              // Nove the satellite around
        // Calculate sat distance from planet
        orbitalDistance = Vector3.Distance(transform.position, planetToOrbit.transform.position) - (planetToOrbit.transform.localScale.x / 2) + 20;
        if (launchStatus == launchStatuses.Constructing) {
            // Rocket is being built. So it cannot launch even when forced to.
            ColorOverlay();
            launchStatus = launchStatuses.Waiting;
        }
        else if (launchStatus == launchStatuses.Waiting) {
            // If rocket is waiting on ground, have it rotate with the earth. 
            transform.RotateAround(Vector3.zero, Vector3.forward, Time.deltaTime);
        }
        else if (launchStatus == launchStatuses.Launching) {
			transform.position = Vector3.MoveTowards(transform.position, planetToOrbit.transform.position, -20 * Time.deltaTime);
			if (orbitalDistance >= startingOrbit) {
                transform.position = Vector3.MoveTowards(transform.position, planetToOrbit.transform.position, orbitalDistance - startingOrbit); // Compensate for overshot launches on low FPS
                launchStatus = launchStatuses.Orbiting;
                ColorOverlay();
            }
        }
        else if (launchStatus == launchStatuses.Orbiting) {
			if (orbitalDistance > 225) {
				DriftIntoSpace();
			}
			Orbit();
        }
		else if (launchStatus == launchStatuses.Landing) {
			if (orbitalDistance < 15) {
				Destroy(this.gameObject);
			}
			downwardSpeed = 0.01f;
			transform.position = Vector3.MoveTowards(transform.position, planetToOrbit.transform.position, 10 * Time.deltaTime);
		}
		else if (launchStatus == launchStatuses.Falling) {
			if (orbitalDistance < 15) {
				Destroy(this.gameObject);
			}
			downwardSpeed = 0.1f;
			Orbit();
		}
		else if (launchStatus == launchStatuses.Drifting) {
			if (orbitalDistance > 666) {
				Destroy(this.gameObject);
			}
			downwardSpeed = -0.1f;
			Orbit();
		}
    }

    public void LiftOff() {
        launchStatus = launchStatuses.Launching;
    }

    void Orbit() {
        //orbit. An orbit of double the radius will result in the satellite moving 10 times as slow. There has to be a more elegant way that allows me to balance this number 6 properly without having to recalculate everything, but I cant think of it.
        orbitSpeed = (360) / (Mathf.Pow(orbitalDistance / 9.48683f, 2f));  // distance / time. distance = 2πr. time = (r/c)^2log(8)
        transform.RotateAround(Vector3.zero, Vector3.forward, orbitSpeed * Time.deltaTime);
		transform.position = Vector3.MoveTowards(transform.position, planetToOrbit.transform.position, downwardSpeed + (orbitalDecay / 20) * Time.deltaTime);
    }

	public void AltitudeChange(float downSpeed) {
		if (fuel > 0 && launchStatus == launchStatuses.Orbiting) {
			downwardSpeed = downSpeed;
			fuel -= Mathf.Abs(downSpeed) * Time.deltaTime * 10;
		}
		else if (fuel <= 0){
			downwardSpeed = 0;
			fuel = 0;
		}
	}


    // If a color overlay is active, change the satellites color. Call this function whenever a sat's status changes.
    public void ColorOverlay() {
        if (gameManager.satellite == this && gameManager.selectedSomething) {
            return; // Don't do anything if the currently selected satellite is selected, as selected sats are highlighted separately.
        }
		if (satType == satTypes.Debris) {
			GetComponentInChildren<Renderer>().material.color = Color.white;
			return; // If the current satellite is debris, it doesn't need to be colored. Unless it is deselected.
		}
		if (launchStatus != launchStatuses.Orbiting) {
			GetComponentInChildren<Renderer>().material.color = Color.white;
			return; // no coloring needed if the sat isn't in orbit
		}
		if (GameManager.activeFilter == filters.Work) {
			if (derelict) {
				GetComponentInChildren<Renderer>().material.color = Color.magenta;
			}
			else if (workFinished) {
                GetComponentInChildren<Renderer>().material.color = Color.green;
            }
            else if (workActive) {
                GetComponentInChildren<Renderer>().material.color = Color.blue;
            }
            else {
                GetComponentInChildren<Renderer>().material.color = Color.yellow;
            }
        }
		else {
            GetComponentInChildren<Renderer>().material.color = Color.white;
        }
    }

	public virtual void Landing() {
		launchStatus = launchStatuses.Landing;
		ColorOverlay();
		gameManager.GetComponent<GameManager>().DeselectObject();
	}


	// ========================================= DAMAGE CODE =========================================== //

	void OnTriggerEnter(Collider col) {
		if (col.gameObject.tag == "Satellite") {
			StartCoroutine(col.gameObject.GetComponent<Satellite>().ApplyDamage(health, mass));
		}
		else if (col.gameObject.tag == "Planet" && (int)launchStatus >= 3) {
			Debug.Log("Something happened");
			if (!radioTransmitter) {
				Landing();
			}
			else {
				CrashIntoEarth();
			}
		}
	}


    // gotta use a coroutine for this otherwise only one of the satellites will be damaged
    public IEnumerator ApplyDamage(float hea, float mas) {
        yield return null;
        if (satType != satTypes.Debris && (int)launchStatus >= 2 && (int)launchStatus < 5) {      // dont do anything if it's space debris or unlaunched.
            health -= (hea * mas) / mass;
            if (health <= 0) {
                Destruction();
				Destruction2();
			}
        }
    }

	public void CrashIntoEarth() {
		Destruction();
		gameManager.GetComponent<GameManager>().DeselectObject();
		launchStatus = launchStatuses.Falling;
		ColorOverlay();
	}

	public void DriftIntoSpace() {
		Destruction();
		gameManager.GetComponent<GameManager>().DeselectObject();
		launchStatus = launchStatuses.Drifting;
		ColorOverlay();
	}

	public virtual void Destruction() {
        workActive = false;             // stop work!
        if (!workFinished) {
            GameManager.activeSatellites[(int)satTypes.Research]--;     // set this satellite as inactive but only if it wasn't inactive already.
        }
    }
    
    public void Destruction2() {
        for (int i = 1; i <= mass; i++) {
            gameManager.GetComponent<SatFactory>().SpawnDebris(transform.position);
            Destroy(this.gameObject);
        }
    }

}