using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LikeProcessing;

public class Sketch : PSketch {

	int ribbonCnt = 1;
	int StickLen = Screen.width / 5;

	Ribbon[] ribbons;

	void Start () {
		ribbons = new Ribbon[ribbonCnt];
		for(int i=0; i<ribbonCnt; i++) {
			Ribbon ribbon = new Ribbon ();
			ribbons [i] = ribbon;
		}

//		sphere = new GameObject ();
////		sphere.transform.localScale *= 100;
//		sphere.AddComponent<MeshFilter>();
//		sphere.AddComponent<MeshRenderer>();
//		Mesh mesh = sphere.GetComponent<MeshFilter>().mesh;
//		mesh.Clear();
//		mesh.vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 100, 0), new Vector3(100, 100, 0)};
//		mesh.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};
//		mesh.triangles = new int[] {0, 1, 2};
	}

	GameObject sphere;

	void Update () {
		float alpha = Mathf.PI - (1.0f * Input.mousePosition.x / Screen.width) * Mathf.PI;
		float beta = (1.0f * Input.mousePosition.y / Screen.height) * Mathf.PI;
		Vector3 stickHead = new Vector3 (
			Mathf.Cos(alpha) * StickLen,
			Mathf.Sin(alpha) * Mathf.Sin(beta - Mathf.PI/2) * StickLen,
			1.0f * Mathf.Sin(alpha) * Mathf.Sin(beta) * StickLen
		);
		foreach (Ribbon ribbon in ribbons) {
			ribbon.update (stickHead);
		}
//		sphere.transform.position = stickHead;

	}

	public class Ribbon {
		public int particleCnt           = 20;
		public Color Color               = Color.white;
		public float rightDownRandomness = 0.1f;
		public float radiusMax           = 12;   // maximum width of ribbon
		public float radiusDivide        = 10;   // distance between current and next point / this = radius for first half of the ribbon
		public Vector3 gravity           = new Vector3(0,.01f,0); // gravity applied to each particle
		public float friction            = 1.1f; // friction applied to the gravity of each particle
		public int maxDistance           = 40;   // if the distance between particles is larger than this the drag comes into effect
		public float drag                = 2.5f;  // if distance goes above maxDistance - the points begin to grag. high numbers = less drag
		public float dragFlare           = .015f;  // degree to which the drag makes the ribbon flare out
		Particle[] particles;

		GameObject lineObj;

		public Ribbon () {
			particles = new Particle[particleCnt];
			for (int i=0; i<particleCnt; i++) {
				Particle p = new Particle(this, Vector3.zero);
				particles[i] = p;
			}

			lineObj = new GameObject("ribbon");
//			lineObj.AddComponent<LineRenderer>();
			lineObj.AddComponent<MeshFilter>();
			lineObj.AddComponent<MeshRenderer>();
		}

		public void update (Vector3 stickHead) {
			move (stickHead);
			draw ();
		}

		void move(Vector3 stickHead) {
			Particle newParticle = particles[0];
			newParticle.reset (stickHead);
			for (int i = 1; i < particleCnt; i++) {
				particles[i-1] = particles[i];
			}
			particles [particleCnt - 1] = newParticle;

			for (int i=1; i<particleCnt-1; i++) {
				Particle p = particles [i];
				Particle back = particles [i - 1];
				Particle head = particles [i + 1];
				p.move (back, head, i);
			}
		}

		void draw() {
			List<Vector3> vertices = new List<Vector3> ();
			List<int> triangles = new List<int> ();
			Mesh mesh = lineObj.GetComponent<MeshFilter>().mesh;
			mesh.Clear();
//			lineObj.vertices = new Vector3[] {new Vector3(0, 0, 0), new Vector3(0, 100, 0), new Vector3(100, 100, 0)};
			//lineObj.uv = new Vector2[] {new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1)};

			for(int i = particleCnt-1; i>1; i--) {
				Particle p = particles [i];
				Particle pback = particles [i - 1];
//				if (i == particleCnt - 1)
//					vertices.Add (p.loc);
//				vertices.Add (pback.loc);
				int len = vertices.Count;
				vertices.Add (p.leftHead);
				vertices.Add (p.rightHead);
				vertices.Add (pback.rightHead);
				vertices.Add (pback.leftHead);
				int[] indeces = { len, len + 1, len + 2, len + 2, len + 3, len };
				triangles.AddRange (indeces);
			}
//			lineObj.triangles = new int[] {0, 1, 2};
//			LineRenderer lineRenderer = lineObj.GetComponent<LineRenderer> ();
//			lineRenderer.SetWidth (20,20);
//			lineRenderer.SetVertexCount (vertices.Count);
////			lineRenderer.SetPosition (0, new Vector3(0,0,0));
////			lineRenderer.SetPosition (1, new Vector3(0,200,0));
//			lineRenderer.SetPositions(vertices.ToArray());
			mesh.vertices = vertices.ToArray();
			mesh.triangles = triangles.ToArray();
		}
	}

	public class Particle {
		public float randomness = .1f;
		public Vector3 loc = Vector3.zero;
		public Vector3 speed = Vector3.zero;
		public Vector3 left = Vector3.zero;
		public Vector3 right = Vector3.zero;
		public Vector3 leftHead = Vector3.zero;
		public Vector3 rightHead = Vector3.zero;
		public float radius;
		Ribbon ribbon;

//		GameObject obj;

		public Particle(Ribbon ribbon, Vector3 loc) {
			this.ribbon = ribbon;
			reset(loc);
//			obj = GameObject.CreatePrimitive(PrimitiveType.Sphere);
//			obj.transform.localScale *= 20;
		}

		public void reset(Vector3 head) {
			loc = left = right = leftHead = rightHead = head;
			speed = Vector3.zero;
		}

		public void move(Particle pback, Particle phead, int index) {
			Vector3 direction = (phead.loc + loc) / 2 - (pback.loc + loc) / 2;
			float radian = Mathf.Atan2 (direction.y, direction.x);
			float distance = direction.magnitude;
			if (distance > ribbon.maxDistance) {
				Vector3 preloc = loc;
				direction = direction.normalized * (ribbon.maxDistance / ribbon.drag);
				loc += direction;
				speed += (loc - preloc) * ribbon.dragFlare;
			}
			speed += ribbon.gravity;
			speed *= ribbon.friction;
			loc += speed;
			Vector3 randomnessv = new Vector3 (
				Random.value * ribbon.rightDownRandomness,
				Random.value * ribbon.rightDownRandomness,
				0
			);
			loc += randomnessv;
			Vector3 randomnessv2 = new Vector3 (
				(randomness/2 - Random.value*randomness) * distance,
				(randomness/2 - Random.value*randomness) * distance,
				0
			);
			loc += randomnessv2;
			if (index > ribbon.particleCnt / 2) {
				radius = distance / ribbon.radiusDivide;
			} else {
				radius = phead.radius * .9f;
			}

			radius = Mathf.Min (radius, ribbon.radiusMax);

			if (index == ribbon.particleCnt -2 || index == 1) {
				if (radius > 1)
					radius = 1;
			}

			left = loc + new Vector3 (
				Mathf.Cos(radian+(Mathf.PI/2*3)) * radius,
				Mathf.Cos(radian+(Mathf.PI/2*3)) * radius,
				0
			);
			right = loc + new Vector3 (
				Mathf.Cos(radian+(Mathf.PI/2)) * radius,
				Mathf.Cos(radian+(Mathf.PI/2)) * radius,
				0
			);
			leftHead  = (phead.left + left) / 2;
			rightHead = (phead.right + right) / 2;
//			obj.transform.position = loc;
			return;
		}
	}


}
