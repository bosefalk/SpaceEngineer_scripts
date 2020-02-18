public IMyMotorStator rightHip; //https://github.com/malware-dev/MDK-SE/wiki/Sandbox.ModAPI.Ingame.IMyMotorStator
public IMyMotorStator rightKnee;
public IMyMotorStator rightAnkle;
public IMyMotorStator rightSidewaysHip;
public IMyLandingGear rightFoot; //https://github.com/malware-dev/MDK-SE/wiki/SpaceEngineers.Game.ModAPI.Ingame.IMyLandingGear
public IMyMotorStator leftHip;
public IMyMotorStator leftKnee;
public IMyMotorStator leftAnkle;
public IMyMotorStator leftSidewaysHip;
public IMyLandingGear leftFoot;
public string stepState = "none";
public string halfState = "none";

public void Main(string argument, UpdateType updateSource)

{

rightHip = GridTerminalSystem.GetBlockWithName("Hip Rotor Right") as IMyMotorStator;
rightKnee = GridTerminalSystem.GetBlockWithName("Knee Rotor Right") as IMyMotorStator;
rightAnkle = GridTerminalSystem.GetBlockWithName("Ankle Rotor Right") as IMyMotorStator;
rightSidewaysHip = GridTerminalSystem.GetBlockWithName("Sideways Hip Rotor Right") as IMyMotorStator;
rightFoot = GridTerminalSystem.GetBlockWithName("Foot Right") as IMyLandingGear;



leftHip = GridTerminalSystem.GetBlockWithName("Hip Rotor Left") as IMyMotorStator;
leftKnee = GridTerminalSystem.GetBlockWithName("Knee Rotor Left") as IMyMotorStator;
leftAnkle = GridTerminalSystem.GetBlockWithName("Ankle Rotor Left") as IMyMotorStator;
leftSidewaysHip = GridTerminalSystem.GetBlockWithName("Sideways Hip Rotor Left") as IMyMotorStator;
leftFoot = GridTerminalSystem.GetBlockWithName("Foot Left") as IMyLandingGear;

Echo($"Right Hip: {rightHip.Angle}");
Echo($"Right Knee: {rightKnee.Angle}");
Echo($"Right Ankle: {rightAnkle.Angle}");
Echo($"Left Hip: {leftHip.Angle}");
Echo($"Left Knee: {leftKnee.Angle}");
Echo($"Left Ankle: {leftAnkle.Angle}");

if (argument == "0") { // Return to upright position
	rightFoot.AutoLock = true;
	leftFoot.AutoLock = true;
	rightFoot.Lock();
	leftFoot.Lock();
	setAngle(0, rightHip);
	setAngle(-0.7, rightKnee);
	rightAnkle.Enabled = true;
	setAngle(-0.7, rightAnkle);
	setAngle(0, leftHip);
	setAngle(0.7, leftKnee);
	leftAnkle.Enabled = true;
	setAngle(0.7, leftAnkle);
}

if (argument == "1") { // Lift right leg
	rightFoot.Unlock();
	setAngle(0.2, rightHip);
	setAngle(-1, rightKnee);
	rightAnkle.TargetVelocityRad = Convert.ToSingle(0);
	rightAnkle.UpperLimitRad = Convert.ToSingle(-0.4);
	rightAnkle.LowerLimitRad = Convert.ToSingle(-1);
	rightAnkle.Enabled = false;
}

if (argument == "2") { // Straighten left leg
	setAngle(0.5, leftAnkle);
	setAngle(0.5, leftKnee);
}

if (argument == "3") { // Put down right leg
	setAngle(-0.2, rightHip);
	setAngle(-0.5, rightKnee);
	rightFoot.AutoLock = true;
}

if (argument == "4") { // Lift left leg
	leftFoot.Unlock();
	setAngle(-0.2, leftHip);
	setAngle(1, leftKnee);
	leftAnkle.TargetVelocityRad = Convert.ToSingle(0);
	leftAnkle.UpperLimitRad = Convert.ToSingle(0.4);
	leftAnkle.LowerLimitRad = Convert.ToSingle(1);
	leftAnkle.Enabled = false;
	
}

if (argument == "5") { // Reset right legs position
	setAngle(0, rightHip);
	setAngle(-0.7, rightKnee);
	rightAnkle.Enabled = true;

}

if (argument == "6") { // Put down left leg
	setAngle(0, leftHip);
	setAngle(0.7, leftKnee);
	leftFoot.AutoLock = true;
}


	if (argument == "half1" | halfState != "none") {
		if (halfState == "none") {
			rightFoot.Unlock();
			setAngle(0.2, rightHip);
			setAngle(-1, rightKnee);
			rightAnkle.TargetVelocityRad = Convert.ToSingle(0);
			rightAnkle.UpperLimitRad = Convert.ToSingle(-0.4);
			rightAnkle.LowerLimitRad = Convert.ToSingle(-1);
			rightAnkle.Enabled = false;
			
			setAngle(0.5, leftAnkle);
			setAngle(0.5, leftKnee);
			Runtime.UpdateFrequency = UpdateFrequency.Update1;
			halfState = "rightUp";
		}
		
		if (halfState == "rightUp" & 
			isAngle(0.2, rightHip) & isAngle(-1, rightKnee) &
			isAngle(0.5, leftAnkle) & isAngle(0.5, leftKnee)) {
			
				setAngle(-0.2, rightHip);
				setAngle(-0.5, rightKnee);
				rightFoot.AutoLock = true;
				halfState = "rightDown";
		}
		
		if (halfState == "rightDown" &
			rightFoot.IsLocked == true) {
				rightAnkle.Enabled = true;
				setAngle(0, rightHip);
				setAngle(-0.7, rightKnee);
				setAngle(-0.7, rightAnkle);
				
				leftFoot.Unlock();
				setAngle(-0.2, leftHip);
				setAngle(1, leftKnee);
				leftAnkle.TargetVelocityRad = Convert.ToSingle(0);
				leftAnkle.LowerLimitRad = Convert.ToSingle(0.4);
				leftAnkle.UpperLimitRad = Convert.ToSingle(1);
				leftAnkle.Enabled = false;
				halfState = "none";
				Runtime.UpdateFrequency = UpdateFrequency.None; // Turn off program
				
			}
		
	
		
	}
	
	if argument = "half2"
		


}

public void setAngle(double angle, IMyMotorStator rotorName) {
		if (rotorName.Angle < (angle - 0.02)) {
			rotorName.UpperLimitRad = Convert.ToSingle(angle);
			//rotorName.LowerLimitRad = Convert.ToSingle(angle);
			rotorName.TargetVelocityRad = Convert.ToSingle(0.2);
		}
		if (rotorName.Angle > (angle + 0.02)) {
			rotorName.LowerLimitRad = Convert.ToSingle(angle);
			//rotorName.UpperLimitRad = Convert.ToSingle(angle);
			rotorName.TargetVelocityRad = Convert.ToSingle(-0.2);
		}
	
	
}

public bool isAngle(double angle, IMyMotorStator rotorName) {
	
	if (rotorName.Angle < (angle - 0.02) |
		rotorName.Angle > (angle + 0.02)) {
			return(false);
		} else {
			return(true);
		}
	
}