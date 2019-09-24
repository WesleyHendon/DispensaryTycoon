/*var path : GameObject;
var moveSpeed : float = .5;
enum Direction {Forward, Backward}
var direction = Direction.Forward;
var movementOnly = false;
var loop = false;
private var doLoop : boolean;
var startpoint : int = 0;
var endpoint : int = 3;
var easeIn = false;
var easeOut = false;
var objectToTrack : Transform;
var rotationData : String;
var zoomData : String;

private var currentPoint : Vector3;
private var nextPoint : Vector3;
private var nextnextPoint : Vector3;
private var bezPoint : Vector3;
private var nextBezPoint : Vector3;
private var currentNormal : Vector3;
private var nextNormal : Vector3;
private var bezNormal : Vector3;
private var currentRotation : Vector3;
private var nextRotation : Vector3;
private var bezRotation : Vector3;

private var pathPoint : Vector3[];
private var pathBezierPoint : Vector3[];
private var pathNormal : Vector3[];
private var pathBezierNormal : Vector3[];
var pathRotation : Vector3[];
var pathBezierRotation : Vector3[];
var pathZoom : float[];
private var nextZoom : float;
var dir = 1;
var pointcount : int;
var extraRotation : boolean;
var extraZoom : boolean;
private var originalRotation : Quaternion;

var count : int;
var t : float = 0;
//private var mScript : MotionRecord;
private var recorder : boolean;

@AddComponentMenu("Camera-Control/Path Follow")
partial class PathFollow { }

function Awake() {
	originalRotation = transform.rotation;
	recorder = false;
	mScript = GetComponent(MotionRecord);
	if (mScript) {
		if (mScript.enabled) {recorder = true;}
	}

	var theMesh : Mesh = path.GetComponent(MeshFilter).sharedMesh;
	pointcount = theMesh.vertexCount/2;
	var pathVertex = theMesh.vertices;
	var pathNormals = theMesh.normals;
	
	// Get average of 2 vertices on the mesh to make a single point on the path
	pathPoint = new Vector3[pointcount];
	var pathTransform : Matrix4x4 = path.transform.localToWorldMatrix;	// Get position and rotation of path object in world space
	for (i = 0; i < pointcount*2; i += 2) {
		pathPoint[i/2] = pathTransform.MultiplyPoint3x4(Vector3.Lerp(pathVertex[i], pathVertex[i+1], .5));
	}

	if (!movementOnly) {
		// Get average of 2 vertex normals on the mesh to make a surface normal at that point
		pathNormal = new Vector3[pointcount];
		for (i = 0; i < pointcount*2; i += 2) {
			pathNormal[i/2] = pathTransform.MultiplyVector(Vector3.Lerp(pathNormals[i], pathNormals[i+1], .5)).normalized;
		}
	}
	
	// If there's rotation data present, read it into an array
	if (rotationData != "") {
		extraRotation = true;
		var splitString : String[] = rotationData.Split(" "[0]);
		if (splitString.Length != pointcount * 3) {
			print ("Rotation data has an incorrect number of entries!");
			extraRotation = false;
		}
		else {
			pathRotation = new Vector3[pointcount];
			for (i = 0; i < pointcount; i++) {
				pathRotation[i] = Vector3(parseFloat(splitString[i*3]),
										  parseFloat(splitString[i*3 + 1]),
										  parseFloat(splitString[i*3 + 2]) );
			}
			pathBezierRotation = new Vector3[pointcount];
		}
	}
	else {
		extraRotation = false;
	}
	
	// If there's zoom data present, read it into an array
	if (zoomData != "") {
		extraZoom = true;
		splitString = zoomData.Split(" "[0]);
		if (splitString.Length != pointcount) {
			print ("Zoom data has an incorrect number of entries!");
			extraZoom = false;
		}
		else {
			pathZoom = new float[pointcount];
			for (i = 0; i < pointcount; i++) {
				pathZoom[i] = parseFloat(splitString[i]);
			}
		}
	}
	else {
		extraZoom = false;
	}
	
	// Get average of points from the path to make control points for the bezier curves, same for normals and rotation data
	pathBezierPoint = new Vector3[pointcount];
	if (!movementOnly) {pathBezierNormal = new Vector3[pointcount];}
	if (direction == Direction.Forward) {
		for (i = 0; i < pointcount; i++) {
			pathBezierPoint[i] = Vector3.Lerp(pathPoint[i], pathPoint[(i+1 == pointcount) ? 0 : i+1], .5);
			if (!movementOnly) {pathBezierNormal[i] = Vector3.Lerp(pathNormal[i], pathNormal[(i+1 == pointcount) ? 0 : i+1], .5);}
			if (extraRotation) {pathBezierRotation[i] = Vector3.Lerp(pathRotation[i], pathRotation[(i+1 == pointcount) ? 0 : i+1], .5);}
		}
	}
	else {
		for (i = 0; i < pointcount; i++) {;
			pathBezierPoint[i] = Vector3.Lerp(pathPoint[i], pathPoint[(i-1 == -1) ? pointcount-1 : i-1], .5);
			if (!movementOnly) {pathBezierNormal[i] = Vector3.Lerp(pathNormal[i], pathNormal[(i-1 == -1) ? pointcount-1 : i-1], .5);}
			if (extraRotation) {pathBezierRotation[i] = Vector3.Lerp(pathRotation[i], pathRotation[(i-1 == -1) ? pointcount-1 : i-1], .5);}
		}
	}
	
	// If the manual motion record script is present, set up array there and copy extra rotation data to it if there's some already present
	if (recorder) {
		mScript.extraRotation = new Vector3[pointcount];
		mScript.zoom = new float[pointcount];
		if (extraRotation) {
			for (i = 0; i < pointcount; i++) {mScript.extraRotation[i] = pathRotation[i];}
		}
		// Set up zoom array on record script and copy zoom data to it if there's some already present
		mScript.cameraZoom = mScript.originalZoom = camera.fieldOfView;
		if (extraZoom) {
			for (i = 0; i < pointcount; i++) {mScript.zoom[i] = pathZoom[i];}
		}
		// Otherwise fill zoom array with default camera FOV
		else {
			for (i = 0; i < pointcount; i++) {mScript.zoom[i] = camera.fieldOfView;}
		}
	}

	// Initialize starting point, using startpoint if it's not zero
	dir = 1;
	count = -1;
	if (direction == Direction.Backward) {
		dir = -1;
		count = pointcount;
	}
	startpoint = Mathf.Clamp(startpoint, 0, pointcount-1);
	if (startpoint != 0) {count = startpoint - dir;}
	endpoint = Mathf.Clamp(endpoint, 0, pointcount-1);
	GetNextPoint();
}

function Start() {
	// Move around path
	var relativeEndPoint = pointcount-endpoint;
	if (dir == -1) {relativeEndPoint = endpoint;}
	var startcount = count;
	var easing = false;
	var tEase : float = 0;
	doLoop = true;
	t = 0;

	while (doLoop) {
		// Get point on curve for location/rotations
		transform.position = GetBezierPoint(currentPoint, nextPoint, bezPoint, t);
		if (!movementOnly) {
			transform.rotation = Quaternion.FromToRotation(Vector3.up, GetBezierPoint(currentNormal, nextNormal, bezNormal, t) );
			transform.LookAt(GetBezierPoint(bezPoint, nextnextPoint, nextBezPoint, t), transform.TransformDirection(0, 1, 0) );
		}
		else {transform.rotation = originalRotation;}
		// If there's an object to track, rotate to look at it
		if (objectToTrack) {
			transform.LookAt(objectToTrack);
		}
		// Add extra rotation from data, if any
		if (extraRotation) {transform.Rotate(GetBezierPoint(currentRotation, nextRotation, bezRotation, t) );}
		// Add extra zoom from data, if any
		if (extraZoom) {camera.fieldOfView = Mathf.Lerp(pathZoom[count], nextZoom, t);}
		// Add rotation from recording script, if present
		if (recorder) {transform.Rotate(mScript.xRotation, mScript.yRotation, mScript.zRotation);}
		yield;
		
		// See if we should ease in or out
		if (!loop) {
			if (easeIn && count == startcount) {easing = true;}
			if (easeOut && count == relativeEndPoint - dir) {easing = true;}
		}
		if (!easing) {
			t += moveSpeed * Time.deltaTime;
		}
		else {
			// Ease in
			if (count == startcount) {
				tEase += 0.5 * Time.deltaTime * moveSpeed;
				var lerpValue = 1 - Mathf.Sin((1-tEase) * Mathf.PI * 0.5);
				t = Mathf.Lerp(0, 1, lerpValue);
			}
			// Ease out
			else {
				tEase -= 0.5 * Time.deltaTime * moveSpeed;
				lerpValue = 1 - Mathf.Sin((1-tEase) * Mathf.PI * 0.5);
				t = Mathf.Lerp(1, 0, lerpValue);
			}
			if (t >= 1) {easing = false;}
			// We've reached the end of the path if easeOut has finished
			if (tEase < 0) {
				doLoop = false;
				if (recorder) {mScript.EndRecording();}
			}
		}
		// See if timecount has rolled over
		if (t >= 1) {
			t = t%1;
			GetNextPoint();
		}
		// If we're not looping, see if we've reached the end point
		if (!loop && count == relativeEndPoint) {
			doLoop = false;
			if (recorder) {mScript.EndRecording();}
		}
	}
}

function GetBezierPoint(p1 : Vector3, p2 : Vector3, p3 : Vector3, t : float) {
	return Vector3( (p1.x*(1-t)*(1-t)+2*p2.x*(1-t)*t+p3.x*t*t),
					(p1.y*(1-t)*(1-t)+2*p2.y*(1-t)*t+p3.y*t*t),
					(p1.z*(1-t)*(1-t)+2*p2.z*(1-t)*t+p3.z*t*t) );
}

    function GetNextPoint() {

	count += dir;
	CheckPoints();
	ComputePoints();
}

function CheckPoints() {
	// If we reached the end of the path
	if (count == pointcount) {
		count = 0;
		if (!loop) {doLoop = false;} 
		if (recorder) {mScript.EndRecording();}
	}
	if (count == -1) {
		count = pointcount-1;
		if (!loop) {doLoop = false;} 
		if (recorder) {mScript.EndRecording();}
	}
}

function ComputePoints() {
	// Get next 2 points in the arrays
	var nextcount = count + dir;
	var nextnextcount = count + (2 * dir);

	if (count == pointcount-1 && dir == 1) {nextcount = 0; nextnextcount = 1;}
	if (count == 0 && dir == -1) {nextcount = pointcount-1; nextnextcount = pointcount-2;}
	
	if (count == pointcount-2 && dir == 1) {nextnextcount = 0;}
	if (count == 1 && dir == -1) {nextnextcount = pointcount-1;}
	
	// Get the 11 points we might need to compute bezier curves...
	// 3 for location & 2 more for LookAt(), 3 for normals, and then 3 for rotation
	currentPoint = pathBezierPoint[count];
	nextPoint = pathPoint[nextcount];
	bezPoint = pathBezierPoint[nextcount];
	nextnextPoint = pathPoint[nextnextcount];
	nextBezPoint = pathBezierPoint[nextnextcount];
	if (!movementOnly) {
		currentNormal = pathBezierNormal[count];
		nextNormal = pathNormal[nextcount];
		bezNormal = pathBezierNormal[nextcount];
	}
	if (extraRotation) {
		currentRotation = pathBezierRotation[count];
		nextRotation = pathRotation[nextcount];
		bezRotation = pathBezierRotation[nextcount];
	}
	// Get next zoom point
	if (extraZoom) {
		nextZoom = pathZoom[nextcount];
	}
	
	if (recorder) {
		mScript.extraRotation[count] = Vector3(mScript.xRotation, mScript.yRotation, mScript.zRotation);
		mScript.zoom[count] = mScript.cameraZoom;
	}
}*/