using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LearningMgmtSys.Model
{

        public interface IRabitMQProducer
        {
            public void SendProductMessage<T>(T message);
        }
    
}
