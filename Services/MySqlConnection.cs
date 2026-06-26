using System;

namespace CyberSecurityAwarenessBot.Services
{
    internal class MySqlConnection
    {
        private string connStr;

        public MySqlConnection(string connStr)
        {
            this.connStr = connStr;
        }

        internal MySqlCommand CreateCommand()
        {
            throw new NotImplementedException();
        }

        internal void Open()
        {
            throw new NotImplementedException();
        }
    }
}