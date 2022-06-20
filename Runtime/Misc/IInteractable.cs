namespace Drifter.Misc
{
    public interface IInteractable
    {
        public string Prompt { get; }

        public void EnterInteract(Interactor interactor);

        public void ExitInteract(Interactor interactor);
    }
}