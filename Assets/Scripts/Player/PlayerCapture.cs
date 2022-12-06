using System.Collections.Generic;
using Interfaces;
using Planets;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Player
{
    public class PlayerCapture : MonoBehaviour
    {
        private bool _canCapture;
        public PlayerInput playerInput;
        public PlayerItems playerItems;
    
        [HideInInspector] public double score;

        [HideInInspector] public List<CapturePoint> inventory;
        [HideInInspector] public CapturePoint currentCapturePoint;

        public PlayerColor color;

        [Header("Interface Elements")]
        public PlayerActionBar playerActionBar;
        public TextMeshProUGUI captureHealth;

        
        // Called every time a player presses the 'Capture' binding.
        public void Capture(InputAction.CallbackContext context)
        {
            if (!context.started) return;
            if (currentCapturePoint == null) return;
            
            currentCapturePoint.AttemptCapture(this);
        }

        /* When a player enters a 'CapturePoint' on a planet, set it
         to their currently selected CapturePoint in the script so that
         if they press any buttons (above) it will call the function on
         that CapturePoint. */
        private void OnTriggerEnter(Collider other)
        {
            // If the entered trigger is not a capture point, ignore it.
            if (!other.CompareTag("CapturePoint")) return;
            RefreshInterfaceElements();
            
            currentCapturePoint = other.GetComponent<CapturePoint>();
        }

        /* When a player exits a 'CapturePoint' on a
         planet, nullify their current capture point. */
        private void OnTriggerExit(Collider other)
        {
            // If the entered trigger is not a capture point, ignore it.
            if (!other.CompareTag("CapturePoint")) return;
            
            captureHealth.gameObject.SetActive(false);
            currentCapturePoint = null;

            //capture image turns off when player leaves trigger
            
        }

        public void RefreshInterfaceElements()
        {
            if (currentCapturePoint == null)
            {
                captureHealth.gameObject.SetActive(false);
                return;
            }
            
            captureHealth.SetText(currentCapturePoint.health.ToString("N0"));
            captureHealth.gameObject.SetActive(true);
            
            if (currentCapturePoint.owner != null)
            {
                // If the planet's owner is NOT the current player
                // but it HAS one
                playerActionBar.Send("Destroy Host Tree [A]");
            }
            else
            {
                // If the planet's owner is NOT the current player
                // but it DOES NOT have one
                playerActionBar.Send("Plant New Tree [A]");
            }
        }

        /* Called once every second. Increments the player's
         score by the amount of Capture Points they currently own. */
        public void IncrementScore()
        {
            score += (inventory.Count * 0.1);
            Debug.Log(name + "'s score changed to " + score);
            
            // If the player achieves the maximum score
            if (score >= PlayerManager.Instance.scoreboard.maxScore)
            {
                GameOver.Instance.Trigger(playerInput.playerIndex);
            }
        }
    }
}
