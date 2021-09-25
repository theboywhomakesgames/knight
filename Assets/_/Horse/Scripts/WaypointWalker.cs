using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using MalbersAnimations;

namespace DB.Knight.Horse{
    public class WaypointWalker : MonoBehaviour
    {
        [SerializeField] private bool _goOnStart, _loop = false;
        [FoldoutGroup("Variables", expanded: false)]
        [SerializeField] private float _movementSpeed, _rotationSpeed;
        [FoldoutGroup("Variables", expanded: false)]
        [SerializeField] private Animator _animator;

        [SerializeField] public Transform[] _waypoints;

        private int _curIndex = -1;
        private bool _hasAnimator = false;

        private Tweener _rotationTween, _moveTween;

        #region MonoBehaviour
        private void Start()
        {
            _hasAnimator = _animator != null;
            if (_goOnStart)
            {
                GotoNext(true);
            }
        }
        #endregion

        public void GotoNext(bool keepGoing = false)
        {
            // get things ready
            if (_waypoints.Length == 0)
                return;

            if (_loop)
            {
                _curIndex = (_curIndex + 1) % _waypoints.Length;
                GotoIndex(_curIndex, keepGoing);
            }
            else
            {
                _curIndex++;

                if (_curIndex < _waypoints.Length)
                {
                    GotoIndex(_curIndex, keepGoing);
                }
                else
                {
                    Stop();
                }
            }
        }

        private void Stop()
        {
            if (_hasAnimator)
            {
                _animator.SetBool("Run", false);
            }
        }

        private void GotoIndex(int idx, bool keepGoing = false)
        {
            // get things ready
            Transform nextWaypoint = _waypoints[idx];
            Vector3 diff = nextWaypoint.position - transform.position;

            // rotate towards next waypoint
            _rotationTween = transform.DORotateQuaternion(
                Quaternion.LookRotation(diff.normalized, Vector3.up),
                1 / _rotationSpeed
            ).SetEase(Ease.Linear);

            // move towards next waypoint
            _moveTween = transform.DOMove(
                new Vector3(nextWaypoint.position.x, transform.position.y, nextWaypoint.position.z),
                diff.magnitude / _movementSpeed
            ).SetEase(Ease.Linear).OnComplete(() =>
            {
                if (keepGoing)
                {
                    GotoNext(keepGoing);
                }
                else
                {
                    Wait(-1f);
                }
            });

            // activate the animator
            if (_hasAnimator)
            {
                _animator.SetBool("Run", true);
            }
        }

        public void Wait(float duration)
        {
            if (duration == -1)
                return;
        }

        public void Continue(bool keepGoing)
        {
            GotoIndex(_curIndex, keepGoing);
        }
    }
}