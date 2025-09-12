using System.Collections.Generic;

namespace EL
{
    public class Entity
    {
        public bool IsSuccess { get; set; }
        public List<string> MessageList { get; set; }

        public Entity()
        {
            MessageList = new List<string>();
        }
    }
}
