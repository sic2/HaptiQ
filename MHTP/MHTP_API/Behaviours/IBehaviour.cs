using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MHTP_API
{
    /// <summary>
    /// IBehaviour defines an interface for behaviours of the MHTP.
    /// Any behaviour should implement IBehaviour.
    /// </summary>
    public interface IBehaviour
    {
        /// <summary>
        /// Behaviour relative time.
        /// Should be used for pulsing effects
        /// </summary>
        int TIME { get; set; }

        /// <summary>
        /// Return to the MHTP the gesture by defining 
        /// what actuators to move and by how much.
        /// Frequency must be managed by the behaviour. 
        /// Remember that behaviours' play methods are called every 10ms 
        /// (@see const MHTP.BEHAVIOUR_LOOP_MS)
        /// </summary>
        /// <returns></returns>
        Dictionary<int, double> play();

        /// <summary>
        /// Updates this behaviour given another behaviour. 
        /// </summary>
        /// <param name="behaviour"></param>
        void updateNext(IBehaviour behaviour);
    }
}
