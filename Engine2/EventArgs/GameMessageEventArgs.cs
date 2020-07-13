namespace Engine.EventArgs
{
    public class GameMessageEventArgs : System.EventArgs
    {
        public GameMessageEventArgs(string message) => Message = message;

        public string Message { get; }
    }
}