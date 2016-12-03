using UnityEngine;
using System.Collections.Generic;
using Leap.Unity;

namespace PaintUtilities {

	public class PaintLine {
	
		private List<Vector3> vertices;
		private List<int> indices;
		private List<Vector2> uvs;
		//private List<Color> colors;

		private Mesh mesh;
		private SmoothedVector3 position;
		private int rings;

		private Paint parent;

		private Vector3[] previousRings;
		private Vector3 previousNormal;

		public PaintLine (Paint parent) {
			this.parent = parent;
			position = new SmoothedVector3 ();
			previousRings = new Vector3[2];
			previousNormal = new Vector3 (0f, 0f, 0f);

			vertices = new List<Vector3> ();
			indices = new List<int> ();
			uvs = new List<Vector2> ();
			//colors = new List<Color>();
		}

		public GameObject InitPaintLine () {
			// Reset all attributes
			vertices.Clear();
			indices.Clear ();
			uvs.Clear ();
			//colors.Clear ();

			// Creates a new mesh for this object
			mesh = new Mesh ();
			rings = 0;

			// Creates a new 
			GameObject paintLine = new GameObject ();
			paintLine.transform.position = new Vector3 (0f, 0f, 0f);
			paintLine.transform.rotation = new Quaternion (0f, 0f, 0f, 1f);
			paintLine.transform.localScale = new Vector3 (1f, 1f, 1f);

			paintLine.AddComponent<MeshRenderer> ().sharedMaterial = parent.materials [0];
			paintLine.AddComponent<MeshFilter> ().mesh = mesh;

			return paintLine;
		}

		public void EndPaintLine () {
			mesh.Optimize ();
			mesh.UploadMeshData (true);
		}

		public void UpdatePaintLine(Vector3 position) {
			this.position.Update (position, Time.deltaTime);
			if (Vector3.Distance (previousRings[0], this.position.value) >= 0.005
			    || vertices.Count == 0) {
				addRing (this.position.value);
				updateMesh ();
			}
		}

		private void updateMesh() {
			mesh.SetVertices (vertices);
			mesh.SetIndices (indices.ToArray (), MeshTopology.Triangles, 0);
			mesh.SetUVs (0, uvs);
			mesh.RecalculateBounds ();
			mesh.RecalculateNormals ();
		}

		private void addRing(Vector3 position) {
			if (++rings == 1) {
				createRing();
				createRing();
				calculateIndices();
			}

			createRing();
			calculateIndices();

			Vector3 normal = new Vector3 (0f, 0f, 0f);
			if (rings == 2) {
				Vector3 direction = position - previousRings[0];
				normal = (Vector3.Cross(direction, Vector3.up)).normalized;
				previousNormal = normal;
			} else if (rings > 2) {
				Vector3 cross = Vector3.Cross(previousRings[0] - previousRings[1], previousNormal);
				normal = Vector3.Cross(cross, position - previousRings[0]).normalized;
			}

			if (rings == 2) {
				updateRingVertices(0, previousRings[0], position - previousRings[1],
					previousNormal, 0);
			}

			if (rings >= 2) {
				updateRingVertices(vertices.Count - parent.resolution, position,
					position - previousRings[0], normal, 0);
				updateRingVertices(vertices.Count - parent.resolution * 2,
					position, position - previousRings[0], normal, 1);
				updateRingVertices(vertices.Count - parent.resolution * 3,
					previousRings[0], position - previousRings[1],
					previousNormal, 1);
			}

			previousRings[1] = previousRings[0];
			previousRings[0] = position;
			previousNormal = normal;
		}

		private void createRing() {
			for (int i = 0; i < parent.resolution; i++) {
				vertices.Add (new Vector3 (0f, 0f, 0f));
				uvs.Add(new Vector2 (i / (parent.resolution - 1.0f), 0));
				//colors.Add(parent.color);
			}
		}

		private void calculateIndices() {
			int res = parent.resolution;
			for (int i = 0; i < res; i++) {
				int vert0 = vertices.Count - 1 - i;
				int vert1 = vertices.Count - 1 - ((i + 1) % res);

				indices.Add(vert0);
				indices.Add(vert1 - res);
				indices.Add(vert0 - res);

				indices.Add(vert0);
				indices.Add(vert1);
				indices.Add(vert1 - res);
			}
		}

		private void updateRingVertices(int offset, Vector3 position, Vector3 direction, Vector3 normal, float radiusScale) {
			direction = direction.normalized;
			normal = normal.normalized;

			for (int i = 0; i < parent.resolution; i++) {
				float angle = 360.0f * (i / (float)(parent.resolution));
				Quaternion rotator = Quaternion.AngleAxis(angle, direction);
				Vector3 ringSpoke = rotator * normal * parent.thickness * radiusScale;
				vertices[offset + i] = position + ringSpoke;
			}
		}
	}
}



