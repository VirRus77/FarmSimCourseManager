namespace FarmSimCourseManager.Tools
{
    /// <summary>
    /// Интерфейс приёмника масштаба.
    /// </summary>
    public interface IScaleTarget
    {
        /// <summary>
        /// Установить масштаб.
        /// </summary>
        /// <param name="value">значение масштаба</param>
        void SetScale(double value);
    }
}
