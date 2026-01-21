using System;

public class LoadingDisposable : IDisposable
{
    readonly LoadingIndicator m_indicator;

    public LoadingDisposable(LoadingIndicator indicator)
    {
        if (indicator != null)
        {
            m_indicator = indicator;
            m_indicator.StartLoading();
        }
    }

    public void Dispose()
    {
        if (m_indicator != null)
            m_indicator.StopLoading();
    }
}