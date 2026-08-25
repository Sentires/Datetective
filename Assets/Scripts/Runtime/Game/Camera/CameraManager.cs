using System;
using System.Collections;
using System.Collections.Generic;
using TheDates.Runtime.General;
using TheDates.Runtime.PlayerCore;
using UnityEngine;
using UnityEngine.U2D;

namespace TheDates
{
    public class CameraManager : BasicSingleton<CameraManager>
    {
        public event Action<RenderState> OnCameraStateChange = delegate { };
        public enum RenderState { Disabled, PixelWorld, HighDefWorld }

        [field: SerializeField, ReadOnly] public RenderState currentState { get; private set; } = RenderState.Disabled;
        
        [SerializeField] private Camera2DHandler pixelWorldCamera;
        [SerializeField] private Camera2DHandler highDefWorldCamera;
        //[SerializeField] private Camera uiCamera;
        [SerializeField] private RenderState defaultState = RenderState.PixelWorld;

        public Dictionary<RenderState, Camera2DHandler> cameraMap { get; private set; }
        
        // Start is called before the first frame update
        void Start()
        {
            cameraMap = new Dictionary<RenderState, Camera2DHandler>
            {
                { RenderState.Disabled, null },
                { RenderState.PixelWorld, pixelWorldCamera },
                { RenderState.HighDefWorld, highDefWorldCamera }
            };
            pixelWorldCamera.AssertNull("Camera Manager");
            highDefWorldCamera.AssertNull("Camera Manager");
            //uiCamera.AssertNull("Camera Manager");
            
            SetState(defaultState);
        }

        public void SetState(RenderState newState) {
            if (currentState == newState) return;
            currentState = newState;
            
            pixelWorldCamera.enabled = currentState == RenderState.PixelWorld;
            highDefWorldCamera.enabled = currentState == RenderState.HighDefWorld;
            
            OnCameraStateChange(currentState);
        }

        public void SnapToPosition(Vector2 position) {
            cameraMap[currentState].SnapToPosition(position);
        }
        
    }
}
