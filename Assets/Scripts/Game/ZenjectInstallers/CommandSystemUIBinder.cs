using UnityEngine;
using Zenject;
using CommandSystem;
using CommandSystem.UI;

namespace ZenjectInstallers
{
    /// <summary>
    /// Game-layer bridge that keeps the reusable CommandSystem assembly free of any Zenject
    /// dependency. The command-system presenters expose plain public <c>Init(...)</c> methods;
    /// this component resolves their dependencies from the container and forwards them.
    ///
    /// Zenject injects this component (via <see cref="Construct"/>) during the SceneContext
    /// injection pass, which runs before any scene MonoBehaviour's Awake(). Because the presenters
    /// use their dependencies inside their own Awake(), forwarding here preserves that ordering.
    ///
    /// Place this on a GameObject in the scene and wire the four presenter references in the
    /// inspector.
    /// </summary>
    public class CommandSystemUIBinder : MonoBehaviour
    {
        [SerializeField]
        private UI_CommandPalettePresenter commandPalettePresenter;
        [SerializeField]
        private UI_CommandableListPresenter commandableListPresenter;
        [SerializeField]
        private UI_CommandTargetListPresenter commandTargetListPresenter;
        [SerializeField]
        private UI_CommandFlowPresenter commandFlowPresenter;

        [Inject]
        public void Construct(
            CommandManager commandManager,
            CommandTargetManager commandTargetManager,
            ICommandPallette commandPalette)
        {
            commandPalettePresenter.Init(commandPalette);
            commandableListPresenter.Init(commandManager);
            commandTargetListPresenter.Init(commandTargetManager);
            commandFlowPresenter.Init(commandManager);
        }
    }
}
