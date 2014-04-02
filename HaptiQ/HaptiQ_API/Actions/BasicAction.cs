using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaptiQ_API
{
    public class BasicAction : IAction
    {
        private String _information;

        public BasicAction(String information)
        {
            _information = information;
        }

        public void run(uint id, Dictionary<int, double> pressureData)
        {
            Console.WriteLine("Getting input from device with id " + id);
            SpeechOutput.Instance.speak(_information);
        }
    }
}
