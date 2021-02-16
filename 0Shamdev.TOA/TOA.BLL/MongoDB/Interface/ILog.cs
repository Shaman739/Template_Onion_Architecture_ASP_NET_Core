using Shamdev.TOA.Core.Data.MongoDB;
using System.Threading.Tasks;

namespace Shamdev.TOA.BLL.MongoDB.Interface
{
    public interface ILog
    {
        public Task AddLog(LogItem log); 
    }
}
