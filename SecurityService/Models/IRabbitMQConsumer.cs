using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SecurityService.Models
{

        public interface IRabbitMQConsumer
        {
            public void ConsumeMessage();
        }
    
}
