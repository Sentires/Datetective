using System;
using TheDates.Runtime.PlayerCore;
using UnityEngine;

namespace TheDates.Runtime.Experimental
{
    public class PlayerWarp : MonoBehaviour
    {
        public Transform incomingPosition;
        //public Collider2D trigger;
        public PlayerWarp targetWarp;
        public Vector2 direction = Vector2.down;

        private void OnTriggerEnter2D(Collider2D other) {
            Debug.Log(other.name);
            if (!other.CompareTag("Player")) return;
            var player = other.GetComponent<PlayerController>();
            Warp(player);
            //targetWarp.GetWarpDetails(out var pos, out var dir);
            //player.Warp(pos, dir);
        }

        private void OnCollisionEnter2D(Collision2D other) {
            Debug.Log(other.collider.name);
            if (!other.collider.CompareTag("Player")) return;
            var player = other.collider.GetComponent<PlayerController>();
            Warp(player);
        }

        public void Warp(PlayerController target) {
            targetWarp.GetWarpDetails(out var pos, out var dir);
            target.Warp(pos, dir);
        }

        private void GetWarpDetails(out Vector2 pos, out Vector2 dir) {
            pos = incomingPosition.position;
            dir = direction;
        }
    }
}
