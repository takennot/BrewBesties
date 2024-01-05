using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Collections.Shaders.CircleTransition {
    public class CircleTransitionTutorial : MonoBehaviour {

        private Canvas _canvas;
        private Image _blackScreen;
        private Material mat;

        private Vector2 _gameObjectCanvasPos;

        private static readonly int RADIUS = Shader.PropertyToID("_Radius");
        private static readonly int CENTER_X = Shader.PropertyToID("_CenterX");
        private static readonly int CENTER_Y = Shader.PropertyToID("_CenterY");

        [SerializeField] private float duration = 1f;
        //[SerializeField] private float middleIncreaseDuration = 0.2f;
        //[SerializeField] private float increaseAmount = 0.05f;
        [SerializeField] private float beginRadius = 1f;
        [SerializeField] private float endRadius = Mathf.Epsilon;

        [SerializeField] private GameObject gameObjectToFocusOn;

        [SerializeField] private Image textToPosition;

        private void Awake() {
            _canvas = GetComponent<Canvas>();
            _blackScreen = GetComponentInChildren<Image>();
            mat = _blackScreen.material;
        }

        private void Start() {

            mat.SetFloat(RADIUS, beginRadius);

        }

        private void Update() {
            
            if (Input.GetKeyDown(KeyCode.Y)) {
                CloseBlackScreen();
            } else if (Input.GetKeyDown(KeyCode.H)) {
                OpenBlackScreen();
            }
            
            DrawBlackScreen(gameObjectToFocusOn);
            DrawTextAtCenter(gameObjectToFocusOn);
        }

        public void CloseBlackScreen() {
            DrawBlackScreen(gameObjectToFocusOn);
            DrawTextAtCenter(gameObjectToFocusOn);
            StartCoroutine(Transition(duration, endRadius)); // Change the endRadius to middleRadius
        }

        public void OpenBlackScreen() {
            DrawBlackScreen(gameObjectToFocusOn);
            StartCoroutine(Transition(duration, beginRadius)); // Change the beginRadius to middleRadius
        }

        private void DrawTextAtCenter(GameObject gameObject)
        {
            var gameObjectScreenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            var canvasRect = _canvas.GetComponent<RectTransform>().rect;
            var canvasWidth = canvasRect.width;
            var canvasHeight = canvasRect.height;

            var gameObjectCanvasPos = new Vector2(
                gameObjectScreenPos.x / Screen.width * canvasWidth,
                gameObjectScreenPos.y / Screen.height * canvasHeight
            );

            textToPosition.rectTransform.anchoredPosition = gameObjectCanvasPos;
        }


        private void DrawBlackScreen(GameObject gameObject)
        {
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;

            var gameObjectScreenPos = Camera.main.WorldToScreenPoint(gameObject.transform.position);

            // To Draw to Image to Full Screen, we get the Canvas Rect size
            var canvasRect = _canvas.GetComponent<RectTransform>().rect;
            var canvasWidth = canvasRect.width;
            var canvasHeight = canvasRect.height;

            // But because the Black Screen is now square (different to Screen). So we much added the different of width/height to it
            // Now we convert Screen Pos to Canvas Pos
            _gameObjectCanvasPos = new Vector2
            {
                x = (gameObjectScreenPos.x / screenWidth) * canvasWidth,
                y = (gameObjectScreenPos.y / screenHeight) * canvasHeight,
            };

            var squareValue = 0f;
            if (canvasWidth > canvasHeight)
            {
                // Landscape
                squareValue = canvasWidth;
                _gameObjectCanvasPos.y += (canvasWidth - canvasHeight) * 0.5f;
            } else
            {
                // Portrait            
                squareValue = canvasHeight;
                _gameObjectCanvasPos.x += (canvasHeight - canvasWidth) * 0.5f;
            }

            _gameObjectCanvasPos /= squareValue;

            mat = _blackScreen.material;
            mat.SetFloat(CENTER_X, _gameObjectCanvasPos.x);
            mat.SetFloat(CENTER_Y, _gameObjectCanvasPos.y);


            _blackScreen.rectTransform.sizeDelta = new Vector2(squareValue, squareValue);
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
        }
        public void SetGameObjectToFocusOn(GameObject gameObject)
        {
            gameObjectToFocusOn = gameObject;
        }
        public GameObject GetGameObjectToFocusOn()
        {
            return gameObjectToFocusOn;
        }
        public void SetTextToPosition(Image text)
        {
            textToPosition = text;
        }

        public Image GetTextToPosition()
        {
            return textToPosition;
        }
    }


    /*
    private void DrawBlackScreen()
    {
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
        _gameObjectCanvasPos = new Vector2
        {
            x = (playerScreenPos.x / screenWidth) * canvasWidth,
            y = (playerScreenPos.y / screenHeight) * canvasHeight,
        };

        var squareValue = 0f;
        if (canvasWidth > canvasHeight)
        {
            // Landscape
            squareValue = canvasWidth;
            _gameObjectCanvasPos.y += (canvasWidth - canvasHeight) * 0.5f;
        } else
        {
            // Portrait            
            squareValue = canvasHeight;
            _gameObjectCanvasPos.x += (canvasHeight - canvasWidth) * 0.5f;
        }

        _gameObjectCanvasPos /= squareValue;

        mat = _blackScreen.material;
        mat.SetFloat(CENTER_X, _gameObjectCanvasPos.x);
        mat.SetFloat(CENTER_Y, _gameObjectCanvasPos.y);


        _blackScreen.rectTransform.sizeDelta = new Vector2(squareValue, squareValue);
    }
    */
}

