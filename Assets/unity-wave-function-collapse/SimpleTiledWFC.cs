using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public class SimpleTiledWFC : MonoBehaviour{
	
	public TextAsset xml = null;
	private string subset = "";

	public int gridsize = 1;
	public int width = 20;
	public int depth = 20;

	public int seed = 0;
	public bool periodic = false;
	public int iterations = 0;
	public bool incremental;
	public int retries = 20;

	public SimpleTiledModel model = null;
	public GameObject[,] rendering;
	public GameObject output;
	private Transform group;
	public Dictionary<string, GameObject> obmap = new Dictionary<string, GameObject>();
    private bool undrawn = true;

	public void destroyChildren (){
		foreach (Transform child in this.transform) {
     		GameObject.DestroyImmediate(child.gameObject);
 		}
 	}

 	void Start(){
		Generate();
		Run();
	}

	void Update(){
		if (incremental){
			Run();
		}
	}


	public void Run(){
		if (model == null){return;}
        if (undrawn == false) { return; }
        // A contradiction leaves the whole wave unresolved, so Draw() would place
        // nothing at all. Throw the dead wave away and try again with a new seed.
        for (int attempt = 0; attempt <= retries; attempt++){
			if (model.Run(seed == 0 ? 0 : seed + attempt, iterations)){
				Draw();
				return;
			}
			ClearOutput();
			model.Restart();
		}
		Debug.LogWarning("SimpleTiledWFC: no solution for a "+width+"x"+depth+" grid after "+(retries+1)+" attempts. Use a smaller grid or record more neighbors.", this);
	}

	private void ClearOutput(){
		if (rendering != null){
			foreach (GameObject tile in rendering){
				if (tile == null){continue;}
				if (Application.isPlaying){Destroy(tile);} else {DestroyImmediate(tile);}
			}
		}
		rendering = new GameObject[width, depth];
		undrawn = true;
	}

	public void OnDrawGizmos(){
		Gizmos.matrix = transform.localToWorldMatrix;
		Gizmos.color = Color.magenta;
		Gizmos.DrawWireCube(new Vector3(width*gridsize/2f-gridsize*0.5f, depth*gridsize/2f-gridsize*0.5f, 0f),new Vector3(width*gridsize, depth*gridsize, gridsize));
	}

	public void Generate(){
		obmap = new  Dictionary<string, GameObject>();

		if (output == null){
			Transform ot = transform.Find("output-tiled");
			if (ot != null){output = ot.gameObject;}}
		if (output == null){
			output = new GameObject("output-tiled");
			output.transform.parent = transform;
			output.transform.position = this.gameObject.transform.position;
			output.transform.rotation = this.gameObject.transform.rotation;}

		// Backwards: DestroyImmediate reindexes the remaining children straight away.
		for (int i = output.transform.childCount - 1; i >= 0; i--){
			GameObject go = output.transform.GetChild(i).gameObject;
			if (Application.isPlaying){Destroy(go);} else {DestroyImmediate(go);}
		}
		group = new GameObject(xml.name).transform;
		group.parent = output.transform;
		group.position = output.transform.position;
		group.rotation = output.transform.rotation;
        group.localScale = new Vector3(1f, 1f, 1f);
        rendering = new GameObject[width, depth];
		this.model = new SimpleTiledModel(xml.text, subset, width, depth, periodic);
        undrawn = true;
    }

	public void Draw(){
		if (output == null){return;}
		if (group == null){return;}
        undrawn = false;
		for (int y = 0; y < depth; y++){
			for (int x = 0; x < width; x++){ 
				if (rendering[x,y] == null){
					string v = model.Sample(x, y);
					int rot = 0;
					GameObject fab = null;
					if (v != "?"){
						rot = int.Parse(v.Substring(0,1));
						v = v.Substring(1);
						if (!obmap.ContainsKey(v)){
							fab = (GameObject)Resources.Load(v, typeof(GameObject));
							obmap[v] = fab;
						} else {
							fab = obmap[v];
						}
						if (fab == null){
							continue;}
						Vector3 pos = new Vector3(x*gridsize, y*gridsize, 0f);
						GameObject tile = (GameObject)Instantiate(fab, new Vector3() , Quaternion.identity);
						Vector3 fscale = tile.transform.localScale;
						tile.transform.parent = group;
						tile.transform.localPosition = pos;
						tile.transform.localEulerAngles = new Vector3(0, 0, 360-(rot*90));
						tile.transform.localScale = fscale;
						rendering[x,y] = tile;
					} else
                    {
                        undrawn = true;
                    }
				}
			}
  		}	
	}
}

#if UNITY_EDITOR
[CustomEditor (typeof(SimpleTiledWFC))]
public class TileSetEditor : Editor {
	public override void OnInspectorGUI () {
		SimpleTiledWFC me = (SimpleTiledWFC)target;
		if (me.xml != null){
			if(GUILayout.Button("generate")){
				me.Generate();
			}
			if (me.model != null){
				if(GUILayout.Button("RUN")){
					me.Run();
				}
			}
		}
		DrawDefaultInspector ();
	}
}
#endif