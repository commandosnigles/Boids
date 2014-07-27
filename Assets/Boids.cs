using UnityEngine;
using System.Collections;

public class Boids : MonoBehaviour {

	public Rigidbody Boid;
	public GameObject Target;
	public int Count = 10;
	public float MaxSpeed = 0.01f;
	public float SpawnRadius = 3f;
	public float NeighborhoodRange = 1.0f;
	public float TargetWeight = 1f;
	public float CenterWeight = 1f;
	public float DirectionWeight = 1f;
	public float AvoidanceWeight = 1f;
	public float AvoidanceRange = 0.5f;
	public Vector3 Current;

	private ArrayList boids;

	void Awake () {
		boids = new ArrayList();
		if (Boid == null || Count <= 0) { 
			this.enabled = false;
			return;
		}
		for (int b = 0; b < Count; b++){
			boids.Add (Instantiate (Boid, RandomV3 (SpawnRadius) + transform.position , Quaternion.identity) as Rigidbody);
		}
		
	}

	void FixedUpdate () {
		UpdateBoidList();

		if (boids.Count > 0) {
			foreach (Rigidbody boid in boids)
				MoveBoid (boid);
		}

	}

	void UpdateBoidList () {
		if (boids.Count == Count)
			return;

		int difference = Mathf.Abs(boids.Count - Count);
		if (boids.Count < Count){
			//make more boids
			for (int b = 0; b < difference; b++){
				boids.Add (Instantiate (Boid, RandomV3 (SpawnRadius) + transform.position, Quaternion.identity) as Rigidbody);
			}
		}
		else {
			//destroy boids
			for (int b = 0; b < difference; b++){

				if (boids[0] is Rigidbody) {
					Rigidbody destroyThis = boids[0] as Rigidbody;
					boids.RemoveAt (0);
					GameObject.Destroy (destroyThis.gameObject);
				}
				else {
					break;
				}

			}
		}

	}

	void MoveBoid (Rigidbody self) {
		Vector3 target = Vector3.zero;
		Vector3 center = Vector3.zero;
		Vector3 direction = Vector3.zero;
		Vector3 avoidance = Vector3.zero;
		ArrayList neighborhood = new ArrayList();



		if (boids.Count > 1) {

			//build neighborhood
			foreach (Rigidbody boid in boids) {
				if (boid != self && Vector3.Distance (self.transform.position, boid.transform.position) < NeighborhoodRange)
					neighborhood.Add(boid);
			}
			if (neighborhood.Count > 0) {
				//find velocity toward center of flock
				foreach (Rigidbody boid in neighborhood)
					center += boid.position;
				center = ((center / (neighborhood.Count)) - self.transform.position) * CenterWeight;


				//find velocity in direction of flock travel
				foreach (Rigidbody boid in neighborhood) {
					direction += boid.velocity;
					//if (boid.velocity != Vector3.zero) Debug.Log (boid.velocity);
				}
				direction = direction / (neighborhood.Count) * DirectionWeight;


				//find velocity required to avoid collision
				foreach (Rigidbody boid in neighborhood) {
					Vector3 gap = boid.transform.position - self.transform.position;
					avoidance -= gap.normalized * (AvoidanceRange - gap.magnitude) * AvoidanceWeight;	
				}
			}
		}

		//find velocity toward target
		if (Target != null) {
			if (neighborhood.Count > 0)
				target = (Target.transform.position - self.transform.position) * TargetWeight;
			else
				target = (Target.transform.position - self.transform.position);
		}

		Vector3 translation = Vector3.ClampMagnitude (target + center + direction + avoidance, MaxSpeed) + Current;
		if (translation == Vector3.zero)
			translation = self.velocity;
		self.velocity = translation;
	}

	Vector3 RandomV3 (float radius) {
		Vector3 random = Vector3.ClampMagnitude(new Vector3 (Random.Range(-radius, radius), Random.Range(-radius, radius), Random.Range(-radius, radius)), radius);
		return random;
	}

	void OnDrawGizmos(){
		Gizmos.DrawWireSphere(this.transform.position, SpawnRadius);
	}
}






































