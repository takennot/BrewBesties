using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Collections.Shaders.CircleTransition {
    public class CircleTransition : MonoBehaviour {
        public Transform[] players;
        private Transform player;

        private Canvas _canvas;
        private Image _blackScreen;
        private Material mat;

        private Vector2 _playerCanvasPos;

        private static readonly int RADIUS = Shader.PropertyToID("_Radius");
        private static readonly int CENTER_X = Shader.PropertyToID("_CenterX");
        private static readonly int CENTER_Y = Shader.PropertyToID("_CenterY");

        [SerializeField] private float duration = 1f;
        //[SerializeField] private float middleIncreaseDuration = 0.2f;
        //[SerializeField] private float increaseAmount = 0.05f;
        [SerializeField] private float beginRadius = 1f;
        [SerializeField] private float middleRadius = 0.5f; 
        [SerializeField] private float endRadius = Mathf.Epsilon;


        private void Awake() {
            _canvas = GetComponent<Canvas>();
            _blackScreen = GetComponentInChildren<Image>();
            mat = _blackScreen.material;
        }

        private void Start() {
            
            GameObject[] playerObjects = GameObject.FindGameObjectsWithTag("Player");
            List<Transform> initializedPlayers = new List<Transform>();

            foreach (GameObject p in playerObjects)
            {
                PlayerScript playerScript = p.GetComponent<PlayerScript>();
                if (playerScript != null && playerScript.IsInitialized())
                {
                    initializedPlayers.Add(p.transform);
                }
            }

            players = initializedPlayers.ToArray();
            

            mat.SetFloat(RADIUS, beginRadius);
            player = players[Random.Range(0, players.Length)];
            if(players != null)
            {
                DrawBlackScreen();
            }

        }

        private void Update() {
            /*
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                CloseBlackScreen();
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                OpenBlackScreen();
            }
            */
            DrawBlackScreen();
        }

        public void CloseBlackScreen() {
            DrawBlackScreen();
            StartCoroutine(Transition(duration, middleRadius)); // Change the endRadius to middleRadius
        }

        public void OpenBlackScreen() {
            DrawBlackScreen();
            StartCoroutine(Transition(duration, beginRadius)); // Change the beginRadius to middleRadius
        }

        private void DrawBlackScreen() {
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            // Need a target
            //Debug.Log("Player" + player);
            var playerScreenPos = Camera.main.WorldToScreenPoint(player.position);

            // To Draw to Image to Full Screen, we get the Canvas Rect size
            var canvasRect = _canvas.GetComponent<RectTransform>().rect;
            var canvasWidth = canvasRect.width;
            var canvasHeight = canvasRect.height;

            // But because the Black Screen is now square (different to Screen). So we much added the different of width/height to it
            // Now we convert Screen Pos to Canvas Pos
            _playerCanvasPos = new Vector2 {
                x = (playerScreenPos.x / screenWidth) * canvasWidth,
                y = (playerScreenPos.y / screenHeight) * canvasHeight,
            };

            var squareValue = 0f;
            if (canvasWidth > canvasHeight) {
                // Landscape
                squareValue = canvasWidth;
                _playerCanvasPos.y += (canvasWidth - canvasHeight) * 0.5f;
            } else {
                // Portrait            
                squareValue = canvasHeight;
                _playerCanvasPos.x += (canvasHeight - canvasWidth) * 0.5f;
            }

            _playerCanvasPos /= squareValue;

            mat = _blackScreen.material;
            mat.SetFloat(CENTER_X, _playerCanvasPos.x);
            mat.SetFloat(CENTER_Y, _playerCanvasPos.y);


            _blackScreen.rectTransform.sizeDelta = new Vector2(squareValue, squareValue);

            // Now we want the circle to follow the player position
            // So First, we must get the player world position, convert it to screen position, and normalize it (0 -> 1)
            // And input into the shader
        }


        private IEnumerator Transition(float duration, float targetRadius) {
            float initialRadius = mat.GetFloat(RADIUS);
            float startTime = Time.time;

            while (Time.time - startTime < duration) {
                float t = (Time.time - startTime) / duration;
                float radius = Mathf.Lerp(initialRadius, targetRadius, t);

                mat.SetFloat(RADIUS, radius);
                yield return null;
            }

            yield return new WaitForSeconds(3f); // Delay for 3 seconds

            // Transition from middleRadius to endRadius with a slight increase
            float middleRadius = mat.GetFloat(RADIUS);
            startTime = Time.time;
            

            while (Time.time - startTime < (duration / 3)) {
                float t = (Time.time - startTime) / (duration / 3);
                float radius = Mathf.Lerp(middleRadius, endRadius/* + increaseAmount*/, t); // Increase slightly before reaching endRadius

                mat.SetFloat(RADIUS, radius);
                yield return null;
            }

            // Finally, transition from the increased radius to endRadius
            float finalRadius = mat.GetFloat(RADIUS);
            startTime = Time.time;

            while (Time.time - startTime < (duration / 2)) {
                float t = (Time.time - startTime) / (duration / 2);
                float radius = Mathf.Lerp(finalRadius, endRadius, t);

                mat.SetFloat(RADIUS, radius);
                yield return null;
            }
        }
        
        public void SetPlayers(List<PlayerScript> newPlayers) 
        {
            for(int i = 0; i < newPlayers.Count; i++)
            {
                players[0] = newPlayers[i].gameObject.transform;
            }
            player = players[Random.Range(0, players.Length)];
            if (players != null)
            {
                DrawBlackScreen();
            }
        }
    }
}

