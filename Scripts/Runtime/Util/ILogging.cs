namespace cpvr_vr_suite.Scripts.Util
{
    public interface ILogging
    {
        public bool EnableLog { get; set; }
        public Logger Logger { get; set; }
        void Log(string key, string msg, Logger logger);
    }
}
