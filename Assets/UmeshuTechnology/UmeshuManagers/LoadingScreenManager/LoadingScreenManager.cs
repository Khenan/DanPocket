using Umeshu.Uf;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Umeshu.USystem.LoadingScreen
{
    using Application = UnityEngine.Application;
    using Scene = UnityEngine.SceneManagement.Scene;

    /// <summary>
    /// Manages the loading screen during scene transitions.
    /// </summary>
    public sealed class LoadingScreenManager : GameSystem<LoadingScreenManager>
    {

        #region Game Element Methods

        protected override void SystemFirstInitialize()
        {
            UmeshuGameManager.Instance.onStartTransition += ActivateScene;
            UmeshuGameManager.Instance.onEndTransition += DesactivateScene;
        }
        protected override void SystemEnableAndReset() { }
        protected override void SystemPlay() { }
        protected override void SystemUpdate() { }

        #endregion

        /// <summary>
        /// Gets the name of the loading scene.
        /// </summary>
        private string LoadingSceneName => UmeshuGameManager.Instance.GetTransitionSceneName();

        /// <summary>
        /// Gets the loading scene.
        /// </summary>
        private Scene LoadingScene => SceneManager.GetSceneByName(LoadingSceneName);

        /// <summary>
        /// The parent GameObject of the transition scene.
        /// </summary>
        private GameObject transitionSceneParent = null;

        /// <summary>
        /// Gets a value indicating whether a transition is in progress.
        /// </summary>
        public static bool InTransition => FadeInProgress + FadeOutProgress + LoadingProgress != 3;

        /// <summary>
        /// Gets the progress of the fade in.
        /// </summary>
        public static float FadeInProgress => UmeshuGameManager.Instance.GetCurrentFadeInProgress();

        /// <summary>
        /// Gets the progress of the fade out.
        /// </summary>
        public static float FadeOutProgress => UmeshuGameManager.Instance.GetCurrentFadeOutProgress();

        /// <summary>
        /// Gets the progress of the loading.
        /// </summary>
        public static float LoadingProgress => UmeshuGameManager.Instance.GetCurrentLoadingProgress();

        /// <summary>
        /// Gets the current loading state.
        /// </summary>
        public static LoadingState LoadingState => UmeshuGameManager.Instance.GetCurrentLoadingState();

        /// <summary>
        /// Called when the loading scene has finished loading.
        /// </summary>
        public void FinishedLoadingLoadingScene()
        {
            transitionSceneParent = UfObject.CreateRootOfScene(LoadingScene);
            transitionSceneParent.gameObject.SetActive(false);
        }

        /// <summary>
        /// Activates the transition scene.
        /// </summary>
        private void ActivateScene()
        {
            transitionSceneParent.SetActive(true);
            Application.backgroundLoadingPriority = ThreadPriority.High;
        }

        /// <summary>
        /// Deactivates the transition scene.
        /// </summary>
        private void DesactivateScene()
        {
            transitionSceneParent.SetActive(false);
            Application.backgroundLoadingPriority = ThreadPriority.Normal;
        }
    }

    /// <summary>
    /// Represents the loading state.
    /// </summary>
    public enum LoadingState
    {
        None,
        FadeIn,
        Loading,
        FadeOut
    }
}