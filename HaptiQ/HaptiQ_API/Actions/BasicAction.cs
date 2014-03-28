using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaptiQ_API
{
    public class BasicAction : IAction
    {
        public void run(uint id, Dictionary<int, double> pressureData)
        {
            Console.WriteLine("Getting input from device with id " + id);
        }
    }
}
