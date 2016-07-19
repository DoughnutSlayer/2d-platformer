namespace Observer_Observable
{
    using UnityEngine;
    using System.Collections.Generic;

    public abstract class ObservableService : MonoBehaviour
    {
        private HashSet<ServiceObserver> _observers;

        public ObservableService()
        {
            _observers = new HashSet<ServiceObserver>();
        }
        
        /// <require>newObserver != null</require>
        /// <param name="newObserver">An Object that should be informed, if the state of the ObservableService changes.</param>
        public void registerObserver(ServiceObserver newObserver)
        {
            Debug.Assert(newObserver != null, "Precondition not met: newObserver != null");

            _observers.Add(newObserver);
        }

        public void informOfChanges()
        {
            foreach (ServiceObserver observer in _observers)
            {
                observer.reactToChange(this);
            }
        }
    }
}