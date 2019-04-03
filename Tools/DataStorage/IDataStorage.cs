using System.Collections.Generic;
using System.Diagnostics;
using MyTaskManager.Models;

namespace MyTaskManager.Tools.DataStorage
{
    internal interface IDataStorage
    {
        //bool ProcessExists(int id);

        //Process GetProcessById(int id);

        void AddProcess(MyProcess process);
        List<MyProcess> ProcessesList { get; }
    }
}
