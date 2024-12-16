using UnityEngine;
using UnityEngine.UI;

using TMPro;

using System.Collections;

namespace Com.MyCompany.MyGame
{
    public class PlayerUI : MonoBehaviour
    {
        #region Private Fields

        private PlayerManager target;

        [Tooltip("プレイヤーの名前を表示するためのUIテキスト")]
        [SerializeField]
        private TMP_Text playerNameText;

        [Tooltip("プレイヤーの体力を表示するためのUIスライダー")]
        [SerializeField]
        private Slider playerHealthSlider;

        float characterControllerHeight = 0f;
        Transform targetTransform;
        Renderer targetRenderer;
        CanvasGroup _canvasGroup;
        Vector3 targetPosition;

        #endregion


        #region Public Fields
            [Tooltip("プレイヤーターゲットからのピクセルオフセット")]
            [SerializeField]
            private Vector3 screenOffset = new Vector3(0f, 30f, 0f);

        #endregion

        #region MonoBehaviour CallBacks

        void LateUpdate()
        {
            // カメラに表示されていない場合はUIを表示しないようにします。これにより、UIが表示されているがプレイヤー自体が見えないという潜在的なバグを回避します。
            if (targetRenderer != null)
            {
                this._canvasGroup.alpha = targetRenderer.isVisible ? 1f : 0f;
            }

            // #重要
            // 画面上でターゲットのGameObjectを追跡します。
            if (targetTransform != null)
            {
                targetPosition = targetTransform.position;
                targetPosition.y += characterControllerHeight;
                this.transform.position = Camera.main.WorldToScreenPoint(targetPosition) + screenOffset;
            }
        }


        void Awake()
        {
            this.transform.SetParent(GameObject.Find("Canvas").GetComponent<Transform>(), false);
            _canvasGroup = this.GetComponent<CanvasGroup>();

            target = new GameObject("PlayerManager").AddComponent<PlayerManager>();
        }


        void Update()
        {
            // プレイヤーの体力を反映
            if (playerHealthSlider != null)
            {
                playerHealthSlider.value = target.Health;
            }
        }

        #endregion

        #region Public Methods

        public void SetTarget(PlayerManager _target)
        {
            if (_target == null)
            {
                Debug.LogError("<Color=Red><a>PlayMakerManagerのターゲットがありません</a></Color> for PlayerUI.SetTarget", this);
                return;
            }
            // 効率のために参照をキャッシュ
            target = _target;
            
            targetTransform = this.target.GetComponent<Transform>();
            targetRenderer = this.target.GetComponent<Renderer>();
            CharacterController characterController = _target.GetComponent<CharacterController>();
            // このコンポーネントのライフタイム中に変更されないプレイヤーのデータを取得します
            if (characterController != null)
            {
                characterControllerHeight = characterController.height;
            }

            if (playerNameText != null)
            {
                playerNameText.text = target.photonView.Owner.NickName;
            }
        }

        #endregion

    }
}