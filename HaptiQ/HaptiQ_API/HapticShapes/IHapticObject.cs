using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Input_API;

namespace HaptiQ_API
{
    /// <summary>
    /// This interface allows an object, of any form, to communicate easily with
    /// the HaptiQ_API.
    /// The HaptiQ_API uses the observer-observable pattern on objects
    /// implementing this interface. 
    /// In order for an object to be notified, you need to add the following line of code 
    /// in the constructor of the object:
    ///     HaptiQsManager.Instance.addObserver(this);
    /// It is also possible to unsubscribe the object by the following line of code:
    ///     HaptiQsManager.Instance.removeObserver(this);   
    /// </summary>
    public interface IHapticObject
    {
        /// <summary>
        /// This method should handle a given input (position and orientation) returning 
        /// an appropriate behaviour. 
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <returns>Tuple with three elements.
        /// The first element in tuple indicates one of the following rules for 
        /// the two behaviours in the tuple:
        /// - 0 ADD (first behaviour in tuple)
        /// - 1 REMOVE (first behaviour in tuple)
        /// - 2 SUBSTITUTE (second behaviour in tuple with first one)
        /// - 3 NONE (do nothing)
        ///</returns>
        Tuple<BEHAVIOUR_RULES, IBehaviour, IBehaviour> handleInput(HaptiQ haptiQ);

        /// <summary>
        /// Handles pressure input
        /// </summary>
        /// <param name="haptiQ"></param>
        /// <param name="gestureType"></param>
        void handlePress(HaptiQ haptiQ, PRESSURE_GESTURE_TYPE gestureType);

        /// <summary>
        /// Registers a custom action object. 
        /// The run method in action is called when there is an input from the user
        /// If not action is registered then the HapticObject will use its own default action
        /// </summary>
        /// <param name="action"></param>
        void registerAction(IAction action);

        /// <summary>
        /// True if this IHapticObject is selected. 
        /// False otherwise.
        /// </summary>
        bool isSelected { get; set; }

        /// <summary>
        /// Allow this object to be selected
        /// </summary>
        /// <param name="selectable"></param>
        void makeObjectSelectable(bool selectable);
    }
}
