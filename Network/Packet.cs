using System;

namespace Network
{
    [Serializable]
    struct Packet
    {
        public string nickName;
        public string message;
        public ConsoleColor textColor;
    }
}