using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jbp.business.observables
{
    public class BaseObservable<T> : IObservable<T>
    {
        public List<IObserver<T>> observers;
        public BaseObservable() {
            this.observers = new List<IObserver<T>>();
        }
        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (!observers.Contains(observer))
                observers.Add(observer);
            return new Unsubscriber<T>(observers, observer);
        }
        public void EndTransmission()
        {
            foreach (var observer in observers.ToArray())
                if (observers.Contains(observer))
                    observer.OnCompleted();

            observers.Clear();
        }
    }
    public class Unsubscriber<T> : IDisposable
    {
        private List<IObserver<T>> _observers;
        private IObserver<T> _observer;

        public Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        {
            this._observers = observers;
            this._observer = observer;
        }

        public void Dispose()
        {
            if (_observer != null && _observers.Contains(_observer))
                _observers.Remove(_observer);
        }
    }
}
