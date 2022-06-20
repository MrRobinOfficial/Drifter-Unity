namespace Drifter
{
    public abstract class BaseComponent : IData
    {
        public interface IVehicleData
        {

        }

        /// <summary>
        /// Initialize all variables over here
        /// </summary>
        public abstract void Init(BaseVehicle vehicle);

        /// <summary>
        /// Update physics and precision calculations over here
        /// </summary>
        public abstract void Simulate(float deltaTime, IVehicleData data = null);

        /// <summary>
        /// 
        /// </summary>
        public abstract void Shutdown();

        #region Data Saving
        public abstract void LoadData(FileData data);
        public abstract FileData SaveData(); 
        #endregion
    }
}