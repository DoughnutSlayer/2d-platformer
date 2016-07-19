//namespace Observer_Observable
//{
//    using UnityEngine;
//    using System.Collections.Generic;

//    public abstract class ObservableServiceWithParam : ObservableService
//    {
//        private HashSet<ServiceObserverWithParam> _observersWithParam;

//        public ObservableServiceWithParam()
//        {
//            _observersWithParam = new HashSet<ServiceObserverWithParam>();
//        }

//        /// <require>newObserver != null</require>
//        /// <param name="newObserver">An Object that should be informed, if the state of the ObservableService changes.</param>
//        public void registerObserver(ServiceObserverWithParam newObserver)
//        {
//            Debug.Assert(newObserver != null, "Precondition not met: newObserver != null");

//            _observersWithParam.Add(newObserver);
//        }

//        public void informOfChanges<T>()
//        {
//            foreach (ServiceObserverWithParam observer in _observersWithParam)
//            {
//                observer.reactToChange<T>(this);
//            }
//        }
//    }
//}