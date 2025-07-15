using Grpc.Core;
using System.Net;

namespace Villager.Api.Data
{
    public class ApiRespose<T> where T : class
    {
        public T Data { get; set; }
        public object Error { get; set; }
        public StatusCode code { get; set; }
    }
}
