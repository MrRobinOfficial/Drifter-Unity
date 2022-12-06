using UnityEngine;

namespace uDrifter
{
	/// <summary>
	/// 
	/// </summary>
	public class CarController : BaseVehicle
	{
        public DrifterSDK.EEngineType engineType;
        public DrifterSDK.EClutchType clutchType;
        public DrifterSDK.EDriveType driveType;
        public DrifterSDK.ETransmissionType transmissionType;

        private void Start()
        {
            Debug.Log(nameof(Start), this);
        }
    }
}
