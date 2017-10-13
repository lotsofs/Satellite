using UnityEngine;
using System.Collections;

public class SatFactory : MonoBehaviour {

    protected SatFactory instance;
    int spawnPick = 0;
    int spawnPickUSSR = 0;
    int spawnPickUSA = 6;

    public GameObject debrisPrefab;
    public GameObject[] nonePrefab;
    public GameObject[] researchPrefab;

	public Satellite specialSat;

    public GameObject[] launchPads;

    void Start() {
        instance = this;
    }

    // Spawn spacejunk
    public void SpawnDebris(Vector3 loc) {
        SatDebris satellite = Object.Instantiate(instance.debrisPrefab).GetComponent<SatDebris>();
        loc.x += Random.Range(-1f, 1f);
        loc.y += Random.Range(-1f, 1f);
        satellite.gameObject.transform.position = loc;
    }
    
    // Check which factions is launching a sattelite, and pick an appropriate launchpad.
    public int selectSpawn(factions fac) {
        switch (fac) {
            case factions.USSR:
                spawnPickUSSR++;
                if (spawnPickUSSR >= 3) {
                    spawnPickUSSR = 0;
                }
                return spawnPickUSSR;
            case factions.USA:
                spawnPickUSA++;
                if (spawnPickUSA >= 9) {
                    spawnPickUSA = 6;
                }
                return spawnPickUSA;
            default:
                return 0;
        }
    }

	/*// special satellites that give bonus points or stuff
	public void LaunchSpecial(int satNo) {
		switch (satNo) {
			case 1:
				// Tutorial Part 4 Research Satellite to Return to Earth
				GameManager.activeSatellites[(int)satTypes.Research]++;
				spawnPick = selectSpawn(factions.USA);                               
				SatResearch satellite = Object.Instantiate(instance.researchPrefab[(int)factions.USA]).GetComponent<SatResearch>();
				satellite.gameObject.transform.position = launchPads[spawnPick].transform.position;
				satellite.gameObject.transform.rotation = launchPads[spawnPick].transform.rotation;
				satellite.Initialize(20, 3, factions.USA, 50, 25, 2, 10, false, false, false);
				specialSat = satellite;
				break;
			default:
				break;
		}
	}*/

    // launch a sat with specific properties, and return it in case it needs even more specific stuff done to it.
    public SatResearch LaunchResearchSpecific(int resQ, int resA, factions fac, float orbS, float fue, float mas, float hea, bool rad, bool sol, bool shi) {
        GameManager.activeSatellites[(int)satTypes.Research]++;
        spawnPick = selectSpawn(fac);                               // Pick a spawnpoint (launchpad)
        // instantiate satellite, put it in the correct spot, then give it all its variables.
        SatResearch satellite = Object.Instantiate(instance.researchPrefab[(int)fac]).GetComponent<SatResearch>();
        satellite.gameObject.transform.position = launchPads[spawnPick].transform.position;
        satellite.gameObject.transform.rotation = launchPads[spawnPick].transform.rotation;
        satellite.Initialize(resQ, resA, fac, orbS, fue, mas, hea, rad, sol, shi);
		return satellite;
    }

    public IEnumerator DelayedLaunchResearchSpecific(int t, int resQ, int resA, factions fac, float orbS, float fue, float mas, float hea, bool rad, bool sol, bool shi) {
        yield return new WaitForSeconds(t);
        LaunchResearchSpecific(resQ, resA, fac, orbS, fue, mas, hea, rad, sol, shi);
    }

}