using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHTP_API
{
    public interface IAction
    {
        /// <summary>
        /// Executes the action
        /// </summary>
        void run(uint id, Dictionary<int, double> pressureData);
    }
}
