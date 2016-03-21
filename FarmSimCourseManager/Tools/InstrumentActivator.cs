using System;
using FarmSimCourseManager.Tools.Controls;

namespace FarmSimCourseManager.Tools
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class InstrumentActivator
    {
        public event Action<IInstrument> Started;


        public void Start(IInstrument instrument)
        {
            RaiseStarted(instrument);
        }

        private void RaiseStarted(IInstrument instrument)
        {
            var e = Started;
            if (e != null)
                e(instrument);
        }
    }
}
