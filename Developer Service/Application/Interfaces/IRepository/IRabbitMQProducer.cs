using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IRepository
{
    public interface IRabbitMQProducer
    {
        void Publish<T>(T message);
    }
}
