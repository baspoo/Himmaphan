using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Interactive.Player
{
    public class pMovement : MonoBehaviour
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
        PlayerClient m_client;
        public void Init(PlayerClient client , bool network = false, Vector3? origin = null)
        {
            this.m_client = client;
            agent.enabled = true;
            //agent.agentTypeID = NavMesh.GetSettingsByIndex( network ? 2 : 1 ).agentTypeID;
            ResetAgentActions();
            TriggerNewDestination(origin == null? transform.position : (Vector3)origin);
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


        public void OnTransparencyCollider( )
        {
            //agent.enabled = false;
            m_client.collider.enabled = false;
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
            //transform.rotation = targetRotation;
            stan_rotate = targetRotation;
        }
        public void RotateTo(Quaternion rotate)
        {
            //# Rotate Character.
            stan_rotate = rotate;
        }
        string keySpecific = null;
        public void MoveTo(Vector3 destination, bool held = false , string keySpecific = null )
        {
            if (!movementEnabled)
                return;

            //# Move Character.
            this.keySpecific = keySpecific;
            var valid  = GetValidClick(destination);
            if (valid == null) return;


            //Debug.Log($"Valid:{valid.Valid} : {held}");
            if (!valid.Valid) 
            {
                return;
            }

            if (!held)
            {
                onClickMove.callall();
                StartMoving(CharacterState.Moving);
                SpawnGroundPathMarker(destination, valid.Valid);
            }
            TriggerNewDestination(valid.ValidPoint);
            StopFollow();
        }
        public void JumpTo(Vector3 destination , bool isValid = true )
        {
            //# Move Character.

            if (isValid && agent.enabled)
            {
                var valid = GetValidClick(destination);
                if (valid == null) return;
                destination = valid.ValidPoint;
            }

            StartMoving( CharacterState.Jump );
            JumpToPosition(destination);
        }
        public void FollowTo(Transform follow)
        {
            m_follow = follow;
        }
        public void StopFollow( )
        {
            m_follow = null;
        }







        public void OnSit( Vector3 position , Quaternion rotate  )
        {
            OnSitStop();
            coroSit =  StartCoroutine(_OnSit(position, rotate ));
        }
        public void OnSitStop()
        {
            agent.enabled = true;
            if (coroSit != null)
                StopCoroutine(coroSit);
        }
        Coroutine coroSit;
        IEnumerator _OnSit( Vector3 position , Quaternion rotate ) 
        {
            bool loop = true;
            while (loop) 
            {
                agent.ResetPath();
                JumpTo(position, false);
                RotateTo(rotate);
                StampOrigin(position, rotate);
                yield return new WaitForSeconds(0.1f);
                StartCoroutine(SetCharacterState(CharacterState.Sit));
                yield return new WaitForSeconds(0.5f);
                if (m_client.avatarObj != null && m_client.avatarObj.avatarAnim.currentstate == CharacterState.Sit ) 
                    loop = false;
            }
        }










        void LateUpdate()
        {
            CharacterStateLogic();
            Standing();
            Follow();
        }
        bool isStansetup = false;
        Vector3 stan_origin;
        [SerializeField] Quaternion stan_rotate;
        Transform m_follow;
        void StartMoving(CharacterState state) 
        {
            agent.enabled = true;
            m_client.collider.enabled = true;
            onStartMove.callall();
        }

        void StampOrigin( Vector3 position , Quaternion rotate ) {
            isStansetup = true;
            stan_origin = position;
            stan_rotate = rotate;
        }
        void Standing()
        {
            if (!protectLocation)
                return;

            if (currentCharacterState == CharacterState.Moving)
            {
                isStansetup = true;
                stan_origin = transform.position;
                stan_rotate = transform.rotation;
            }
            else
            {
                if (isStansetup)
                {
                    transform.position = stan_origin;
                    transform.rotation = stan_rotate;
                }
            }
        }
        void Follow() 
        {
            if (m_follow != null) 
            {
                if (Vector3.Distance(transform.position, m_follow.position) > Data.PlistData.plist.interactive.playerDistance.distanceFollow)
                {
                    StopFollow();
                    return;
                }

                MoveTo(m_follow.position,true);
            }
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
            var ray = CameraControl.CameraEngine.instance.maincamera.ScreenPointToRay(Input.mousePosition);
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
                        SaveCurrentLocation(transform.position, "stop");
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

            var effect = prefab.Pool( null , groundMarkerDuration);
            var post = new Vector3(point.x + markerPositionOffset.x, point.y + markerPositionOffset.y, point.z + markerPositionOffset.z);
            effect.transform.position = post;
            effect.transform.rotation = prefab.transform.rotation;

        }
        void SaveCurrentLocation(Vector3 location , string from)
        {
            //if (this.m_client.IsMine)
            //    $"[{from}] Update Location: {location}".Log(Color.yellow);
            lastLocation = location;
            onChangeLocation.ForEach(x => x.Invoke(lastLocation));
        }
        void JumpToPosition(Vector3 location)
        {
            SaveCurrentLocation(location,"jump");
            if (agent.enabled)
            {
                agent.SetDestination(location);
                agent.Warp(location);
            }
            stan_origin = location;
            stan_rotate = transform.rotation;
        }
        void TriggerNewDestination(Vector3 location)
        {
            SaveCurrentLocation(location, "move");
            agent.SetDestination(location);
            StartCoroutine(SetCharacterState(CharacterState.Moving));
        }
        IEnumerator SetCharacterState(CharacterState state)
        {
            yield return new WaitForEndOfFrame();
            currentCharacterState = state;
            onChangeState.ForEach(x => x.Invoke(currentCharacterState));
        }




    }
}