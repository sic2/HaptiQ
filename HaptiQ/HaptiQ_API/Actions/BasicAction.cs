using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HaptiQ_API
{
    /// <summary>
    /// BasicAction class
    /// </summary>
    public class BasicAction : IAction
    {
        private String _information;

        /// <summary>
        /// BasicAction constructor
        /// </summary>
        /// <param name="information"></param>
        public BasicAction(String information)
        {
            _information = information;
        }

        /// <summary>
        /// Execute this action: outputs the textual information to the speakers
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pressureData"></param>
        public void run(uint id, Dictionary<int, double> pressureData)
        {
            Console.WriteLine("Getting input from device with id " + id);
            SpeechOutput.Instance.speak(_information);
        }
    }
}
