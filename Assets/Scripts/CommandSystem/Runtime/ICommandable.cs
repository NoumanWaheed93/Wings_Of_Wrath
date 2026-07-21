namespace CommandSystem
{
    public interface ICommandable
    {
        public string Name { get; }

        public void GiveCommand(ICommand command);
    }

}
