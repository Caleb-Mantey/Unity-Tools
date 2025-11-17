using System;
using UnityEngine;

//
// Copyright (c) Relu Interactives. All right's reserved
//

namespace Relu.Utils
{
    public class TimedObjectDeactivator : MonoBehaviour
    {
        [SerializeField] private float m_TimeOut = 1.0f;
        [SerializeField] private bool m_DetachChildren = false;


        private void OnEnable()
        {
            Invoke("DestroyNow", m_TimeOut);
        }

        private void OnDisable()
        {
            CancelInvoke("DestroyNow");
        }

        public void StopDestroy()
        {
            CancelInvoke("DestroyNow");
        }

        private void DestroyNow()
        {
            if (m_DetachChildren)
            {
                // Detach all child transforms so they won't be deactivated with this object.
                transform.DetachChildren();
            }
            gameObject.SetActive(false);
        }
    }
}
