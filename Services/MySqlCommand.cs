using System;

namespace CyberSecurityAwarenessBot.Services
{
    internal class MySqlCommand
    {
        public string CommandText { get; internal set; }
        public object Parameters { get; internal set; }

        internal void ExecuteNonQuery()
        {
            throw new NotImplementedException();
        }

        internal object ExecuteReader()
        {
            throw new NotImplementedException();
        }

        internal bool ExecuteScalar()
        {
            throw new NotImplementedException();
        }
    }
}