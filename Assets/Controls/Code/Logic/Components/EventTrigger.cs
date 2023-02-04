using System;
using UnityEngine;
using UnityEngine.Events;

namespace Controls.Logic.Components
{
    [Serializable]
    public class ActionEvent : UnityEvent 
    {
    }

    public class EventTrigger : MonoBehaviour
    {
        [SerializeField]
        ActionEvent onAwake;

        [SerializeField]
        ActionEvent onStart;

        [SerializeField]
        ActionEvent onEnable;

        [SerializeField]
        ActionEvent onDisable;

        [SerializeField]
        ActionEvent onDestroy;

        void Awake()
        {
            onAwake.Invoke();
        }

        void Start()
        {
            onStart.Invoke();
        }

        void OnEnable()
        {
            onEnable.Invoke();
        }

        void OnDisable()
        {
            onDisable.Invoke();
        }

        void OnDestroy()
        {
            onDestroy.Invoke();
        }
    }
}