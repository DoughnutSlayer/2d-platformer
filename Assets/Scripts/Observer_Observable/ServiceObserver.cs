namespace Observer_Observable
{
    public interface ServiceObserver
    {
        void reactToChange(ObservableService observedObject);
    }
}
