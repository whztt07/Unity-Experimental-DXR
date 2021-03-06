using System;
using UnityEngine.UIElements;

namespace UnityEditor.PackageManager.UI
{
#if !UNITY_2018_3_OR_NEWER
    internal class AlertFactory : UxmlFactory<Alert>
    {
        protected override Alert DoCreate(IUxmlAttributes bag, CreationContext cc)
        {
            return new Alert();
        }
    }
#endif

    internal class Alert : VisualElement
    {
#if UNITY_2018_3_OR_NEWER
        internal new class UxmlFactory : UxmlFactory<Alert> {}
#endif

        private const string TemplatePath = PackageManagerWindow.ResourcesPath + "Templates/Alert.uxml";
        private readonly VisualElement root;
        private const float originalPositionRight = 5.0f;
        private const float positionRightWithScroll = 12.0f;

        public Action OnCloseError;

        public Alert()
        {
            UIUtils.SetElementDisplay(this, false);

            root = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(TemplatePath).CloneTree();
            Add(root);
            root.StretchToParentSize();

            CloseButton.clickable.clicked += () =>
            {
                if (null != OnCloseError)
                    OnCloseError();
                ClearError();
            };
        }

        public void SetError(Error error)
        {
            var message = "An error occured.";
            if (error != null)
                message = error.message ?? string.Format("An error occurred ({0})", error.errorCode.ToString());

            AlertMessage.text = message;
            UIUtils.SetElementDisplay(this, true);
        }

        public void ClearError()
        {
            UIUtils.SetElementDisplay(this, false);
            AdjustSize(false);
            AlertMessage.text = "";
            OnCloseError = null;
        }

        public void AdjustSize(bool verticalScrollerVisible)
        {
            if (verticalScrollerVisible)
                style.right = originalPositionRight + positionRightWithScroll;
            else
                style.right = originalPositionRight;
        }

        private Label _alertMessage;
        private Label AlertMessage { get { return _alertMessage ?? (_alertMessage = root.Q<Label>("alertMessage")); } }

        private Button _closeButton;
        private Button CloseButton { get { return _closeButton ?? (_closeButton = root.Q<Button>("close")); } }
    }
}
