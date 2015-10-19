/*==============================================================================
Copyright (c) 2010-2014 Qualcomm Connected Experiences, Inc.
All Rights Reserved.
Confidential and Proprietary - Qualcomm Connected Experiences, Inc.
==============================================================================*/

using UnityEngine;

namespace Vuforia
{
    /// <summary>
    /// A custom handler that implements the ITrackableEventHandler interface.
    /// </summary>
    public class DefaultTrackableEventHandler : MonoBehaviour,
                                                ITrackableEventHandler
    {
        #region PRIVATE_MEMBER_VARIABLES
 
        private TrackableBehaviour mTrackableBehaviour;

        private float timeAtWhichMarkerLost;

        private bool markerCurrentlyLost;

        #endregion // PRIVATE_MEMBER_VARIABLES

        public bool lastTracked;
        public PresentationHandler handler;

        #region UNTIY_MONOBEHAVIOUR_METHODS
    
        void Start()
        {
            mTrackableBehaviour = GetComponent<TrackableBehaviour>();
            if (mTrackableBehaviour)
            {
                mTrackableBehaviour.RegisterTrackableEventHandler(this);
            }
        }

        #endregion // UNTIY_MONOBEHAVIOUR_METHODS



        #region PUBLIC_METHODS

        /// <summary>
        /// Implementation of the ITrackableEventHandler function called when the
        /// tracking state changes.
        /// </summary>
        public void OnTrackableStateChanged(
                                        TrackableBehaviour.Status previousStatus,
                                        TrackableBehaviour.Status newStatus)
        {
            if (newStatus == TrackableBehaviour.Status.DETECTED ||
                newStatus == TrackableBehaviour.Status.TRACKED ||
                newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
            {
                OnTrackingFound();
            }
            else
            {
                OnTrackingLost();
            }
        }

        #endregion // PUBLIC_METHODS



        #region PRIVATE_METHODS


        private void OnTrackingFound()
        {

            markerCurrentlyLost = false;

            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            // Enable rendering:
            foreach (Renderer component in rendererComponents)
            {
                component.enabled = true;
            }

            // Enable colliders:
            foreach (Collider component in colliderComponents)
            {
                component.enabled = true;
            }

            if (!lastTracked)
            {
                for (int i = 0; i < handler.imageSlides.Length; i++)
                {
                    if (this == handler.imageSlides[i].GetComponent<DefaultTrackableEventHandler>())
                    {
                        lastTracked = true;
                        handler.SetLastTrackedIndex(i + "");
                    }
                    else
                    {
                        handler.imageSlides[i].GetComponent<DefaultTrackableEventHandler>().lastTracked = false;
                    }
                }
            }
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
        }


        private void OnTrackingLost()
        {

            timeAtWhichMarkerLost = Time.time;
            markerCurrentlyLost = true;

            /*
            Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
            Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

            if (lastTracked == false)
            {
                // Disable rendering:
                foreach (Renderer component in rendererComponents)
                {
                    component.enabled = false;
                }

                // Disable colliders:
                foreach (Collider component in colliderComponents)
                {
                    component.enabled = false;
                }
            }
            */
            Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + "temporarily lost");
            Debug.Log("Time marker got lost: " + timeAtWhichMarkerLost);
        }

        #endregion // PRIVATE_METHODS

        void FixedUpdate()
        {

            if (markerCurrentlyLost)
            {
                float currentTime = Time.time;
                //Debug.Log("CurrentTime - timeAtWhichMarkerLost = " + (currentTime - timeAtWhichMarkerLost));
                //Debug.Log("markerLost: " + markerCurrentlyLost);

                if (currentTime - timeAtWhichMarkerLost >= 2.0f)
                {

                    Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
                    Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

                    // Disable rendering:
                    foreach (Renderer component in rendererComponents)
                    {
                        component.enabled = false;
                    }

                    // Disable colliders:
                    foreach (Collider component in colliderComponents)
                    {
                        component.enabled = false;
                    }

                    Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
                }
            }
        }
    }
}
