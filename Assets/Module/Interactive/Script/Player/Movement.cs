

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

    public class Movement : MonoBehaviour
    {


        // NAVIGATION
        [Header("NAVIGATION")]
        public bool movementEnabled = true;
        public bool stunned;
        public float destinationTreshold = 0.25f;
        public float minimumPathDistance = 0.5f;
        public float samplePositionDistanceMax = 5f;
        public NavMeshAgent agent;


        // INPUT FEEDBACK
        [Header("INPUT FEEDBACK")]
        public float groundMarkerDuration = 2;
        public Vector3 markerPositionOffset = new Vector3(0, 0.1f, 0);
        public GameObject validGroundPathPrefab, rectifiedGroundPathPrefab;


        // STATES
        public enum CharacterState
        {
            Idle,
            Moving,
            Standing,
            Stunned,
            Acting,
            Jump,
            Sit
        }
        [Header("STATES")]
        public CharacterState currentCharacterState;
        public Vector3 lastLocation;
        public List<System.Action<CharacterState>> onChangeState = new List<System.Action<CharacterState>>();
        public List<System.Action<Vector3>> onChangeLocation = new List<System.Action<Vector3>>();
        public TaskService.Function onStartMove = new TaskService.Function();
        public TaskService.Function onMoveDone = new TaskService.Function();
        public TaskService.Function onClickMove = new TaskService.Function();

        public class ValidCoordinate
        {
            public bool Valid;
            public Vector3 ValidPoint;
        }


        bool protectLocation;
      
        public void Init()
        {
            agent.enabled = true;
            ResetAgentActions();
            TriggerNewDestination(transform.position);
            protectLocation = true;
        }
        public void Clean()
        {
            onChangeState.Clear();
            onChangeLocation.Clear();
        }




        public void AgentStart()
        {
            agent.enabled = true;
            agent.ResetPath();
            protectLocation = true;
        }
        public void AgentStop()
        {
            agent.ResetPath();
            agent.enabled = false;
            protectLocation = false;
        }


        public void OnStanding()
        {
            //# Stop Character.
            ResetAgentActions();
            StartCoroutine(SetCharacterState(CharacterState.Standing));
        }
        public void StopMove()
        {
            //# Stop Character.
            ResetAgentActions();
            OnStanding();
        }

        public void HandleStanding()
        {
            //# Stop & Rotate Character Follow by Mouse.
            ValidCoordinate validCoordinate = GetGroundRayPoint();
            if (!validCoordinate.Valid) return;
            var targetRotation = Quaternion.LookRotation(validCoordinate.ValidPoint - transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
        }
        string keySpecific = null;
        public void MoveTo(Vector3 destination, bool held = false, string keySpecific = null)
        {
            if (!movementEnabled)
                return;

            //# Move Character.
            this.keySpecific = keySpecific;
            var valid = GetValidClick(destination);
            if (valid == null) return;

            if (!held)
            {
                onClickMove.callall();
                StartMoving(CharacterState.Moving);
            }

            TriggerNewDestination(valid.ValidPoint);
            if (!held)
            {
                SpawnGroundPathMarker(destination, valid.Valid);
            }
            //else SpawnGroundPathMarker(destination, valid.Valid);

    }
        public void JumpTo(Vector3 destination, bool isValid = true)
        {
            //# Move Character.

            if (isValid && agent.enabled)
            {
                var valid = GetValidClick(destination);
                if (valid == null) return;
                destination = valid.ValidPoint;
            }

            StartMoving(CharacterState.Jump);
            JumpToPosition(destination);
        }


















        void LateUpdate()
        {
            CharacterStateLogic();
        }
        void StartMoving(CharacterState state)
        {
            agent.enabled = true;
            onStartMove.callall();
        }
        public bool IsValidClick(Vector3 destination)
        {
            if (IsPathTooClose(destination)) return false;
            if (!IsPathAllowed(destination))
            {
                ValidCoordinate newResult = closestAllowedDestination(destination);
                if (!newResult.Valid)
                {
                    return false;
                }
            }
            return true;
        }
        ValidCoordinate GetValidClick(Vector3 destination)
        {
            var valid = new ValidCoordinate();
            valid.Valid = true;
            valid.ValidPoint = destination;

            if (IsPathTooClose(destination)) return null;
            if (!IsPathAllowed(destination))
            {
                ValidCoordinate newResult = closestAllowedDestination(destination);
                if (newResult.Valid)
                {
                    valid.ValidPoint = newResult.ValidPoint;
                    valid.Valid = false;
                }
                else
                {
                    return null;
                }
            }
            return valid;
        }
        bool IsPathTooClose(Vector3 point)
        {
            return Vector3.Distance(transform.position, point) < minimumPathDistance;
        }
        bool IsPathAllowed(Vector3 point)
        {
            NavMeshPath path = new NavMeshPath();
            return NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, path);
        }
        ValidCoordinate closestAllowedDestination(Vector3 point)
        {
            ValidCoordinate newResult = new ValidCoordinate();
            if (!NavMesh.SamplePosition(point, out var hit, samplePositionDistanceMax, NavMesh.AllAreas))
                return newResult;
            newResult.Valid = true;
            newResult.ValidPoint = hit.position;
            return newResult;
        }
        private ValidCoordinate GetGroundRayPoint()
        {
            var playerPlane = new Plane(Vector3.up, transform.position);
            var ray = SceneHandle.instance.cameraManager.mainCamera.ScreenPointToRay(Input.mousePosition);
            ValidCoordinate validCoordinate = new ValidCoordinate();
            if (!playerPlane.Raycast(ray, out var hitDist)) return validCoordinate;
            validCoordinate.Valid = true;
            validCoordinate.ValidPoint = ray.GetPoint(hitDist);
            return validCoordinate;
        }
        bool IsDestinationReached()
        {
            return !agent.hasPath || agent.remainingDistance <= (agent.stoppingDistance + destinationTreshold);
        }
        void CharacterStateLogic()
        {
            switch (currentCharacterState)
            {
                case CharacterState.Idle:
                    break;

                case CharacterState.Moving:
                    if (IsDestinationReached())
                    {
                        ResetAgentActions();
                        onMoveDone.callall(this.keySpecific);
                    }
                    break;
            }
        }
        void ResetAgentActions()
        {
            agent.ResetPath();
            StartCoroutine(SetCharacterState(CharacterState.Idle));
        }
        void SpawnGroundPathMarker(Vector3 point, bool rectified)
        {
            GameObject prefab = rectified ? validGroundPathPrefab : rectifiedGroundPathPrefab;
            if (prefab == null) return;

            var marker = prefab.Pool(SceneHandle.instance.effectRoot, groundMarkerDuration);
            var pos = new Vector3(point.x + markerPositionOffset.x, point.y + markerPositionOffset.y, point.z + markerPositionOffset.z);
            marker.transform.position = pos;
        }
        void JumpToPosition(Vector3 location)
        {
            lastLocation = location;
            onChangeLocation.ForEach(x => x.Invoke(lastLocation));
            if (agent.enabled)
            {
                agent.SetDestination(location);
                agent.Warp(location);
            }
        }
        void TriggerNewDestination(Vector3 location)
        {
            lastLocation = location;
            onChangeLocation.ForEach(x => x.Invoke(lastLocation));
            agent.SetDestination(location);
            StartCoroutine(SetCharacterState(CharacterState.Moving));
        }
        IEnumerator SetCharacterState(CharacterState state)
        {
            yield return new WaitForEndOfFrame();
            currentCharacterState = state;
            onChangeState.ForEach(x => x.Invoke(currentCharacterState));

            //StartAnimation(state);
        }

        public Vector3 FindAroundValidPosition(float radius)
        {
            Vector3 path = Vector3.zero;
            for (int i = 0; i < 20; i++)
            {
                Vector3 vec = transform.RandomPointOnXZCircle(radius.Random());
                if (IsValidClick(vec))
                {
                    path = vec;
                }
            }
            return path;
        }


    }
