using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTools.DelegatesAndEnums;

namespace jbp.business.observers
{
    public class BaseObserver<T> : IObserver<T>
    {
        private IDisposable unsubscriber;

        public virtual void Subscribe(IObservable<T> provider)
        {
            if (provider != null)
                unsubscriber = provider.Subscribe(this);
        }
        public void Unsubscribe()
        {
            unsubscriber.Dispose();
        }
        public void OnCompleted()
        {
            this.Unsubscribe();
        }
        public void OnError(Exception error)
        {
            throw error;
        }
        public virtual void OnNext(T value)
        {
            
        }
    }
}
