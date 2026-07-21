namespace CommandSystem
{
    public interface ICommand
    {
        public void Execute(ICommandable commandable);
    }

}
