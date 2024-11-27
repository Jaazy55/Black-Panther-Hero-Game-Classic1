using System;
using Game.Vehicle;

public class DrownTankAchievement : Achievement
{
	public override void VehicleDrawingEvent(DrivableVehicle vehicle)
	{
		if (vehicle is DrivableTank)
		{
			this.AchievComplite();
		}
	}
}
