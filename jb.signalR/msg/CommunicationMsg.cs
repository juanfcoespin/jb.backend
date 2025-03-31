namespace jb.signalR.msg
{
    public enum eMessageType
    {
        Info,
        Success,
        Warning,
        Error
    }
    public class CommunicationMsg
    {
        public string fecha { get; set; }
        public string userId { get; set; }
        public string message { get; set; }
        public eMessageType tipo{ get; set; }
    }
}
