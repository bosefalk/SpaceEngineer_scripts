public IMyMotorStator headRotor; //https://github.com/malware-dev/MDK-SE/wiki/Sandbox.ModAPI.Ingame.IMyMotorStator
public IMyMotorStator armRotorRight;
public IMyMotorStator armRotorLeft;


public void Main() 
{

/*
* The two arm rotors for a mech will follow the up/down rotation of the independently moving cockpit
* All rotors need to be mounted with the same orientation, i.e. 0 being straight down when constructed
*/

Runtime.UpdateFrequency = UpdateFrequency.Update10;

headRotor = GridTerminalSystem.GetBlockWithName("Head Rotor") as IMyMotorStator;
armRotorRight = GridTerminalSystem.GetBlockWithName("Arm Rotor Right") as IMyMotorStator;
armRotorLeft = GridTerminalSystem.GetBlockWithName("Arm Rotor Left") as IMyMotorStator;

double head_angle = headRotor.Angle;
double armRight_angle = armRotorRight.Angle;
double armLeft_angle = armRotorLeft.Angle;

if (-armRight_angle < (head_angle - 0.02) |
	armLeft_angle < (head_angle - 0.02) {
	armRotorRight.LowerLimitRad = Convert.ToSingle(-head_angle);
	armRotorRight.TargetVelocityRad = Convert.ToSingle(-1) ;
	armRotorLeft.UpperLimitRad = Convert.ToSingle(head_angle);
	armRotorLeft.TargetVelocityRad = Convert.ToSingle(1) ;
	
}

 
if (-armRight_angle > (head_angle + 0.02) |
	armLeft_angle > (head_angle + 0.02)) {
	armRotorRight.UpperLimitRad = Convert.ToSingle(-head_angle);
	armRotorRight.TargetVelocityRad = Convert.ToSingle(1);
	armRotorLeft.LowerLimitRad = Convert.ToSingle(head_angle);
	armRotorLeft.TargetVelocityRad = Convert.ToSingle(-1) ;
} 
 

}