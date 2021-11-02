

public IMyPistonBase piston_Up_1;
public IMyPistonBase piston_Up_2;
public IMyPistonBase piston_Out_1;
public IMyPistonBase piston_Out_2;
public IMyMotorStator main_Rotor;
// global variables
public double up_dist; // distance in meters above lowest possible position of UP pistons, 0 - 20 max
public double out_dist; // distance in meters to extend OUT pistons, 0 - 20 max
public double rotor_angle; // turning the crane, negatives moves towards "loading dock", positive towards storage, in degrees
public int X;
public int Y;
public bool move_to_grid = false;
public bool move_up_pistons = false;
public bool move_out_pistons = false;
public bool move_rotor = false;


public void Main(string argument) {
	piston_Up_1 = GridTerminalSystem.GetBlockWithName("Crane Up 1") as IMyPistonBase;
	piston_Up_2 = GridTerminalSystem.GetBlockWithName("Crane Up 2") as IMyPistonBase;
	piston_Out_1 = GridTerminalSystem.GetBlockWithName("Crane Out 1") as IMyPistonBase;
	piston_Out_2 = GridTerminalSystem.GetBlockWithName("Crane Out 2") as IMyPistonBase;
	main_Rotor = GridTerminalSystem.GetBlockWithName("Crane Main Rotor") as IMyMotorStator;
	
	setArgs(argument);
	
	if (move_to_grid == true) {
		calculateGridCoordinates(X, Y);
	}
	
	if (move_up_pistons == true) {
		moveUpPistons(up_dist);
	}
	if (move_out_pistons == true) {
		moveOutPistons(out_dist);
	}
	if (move_rotor == true) {
		moveRotor(rotor_angle);
	}
}

public void setArgs(string argument) {
	string[] args = argument.Split(null); // default is to split string by whitespace
	// input arguments should be i.e. "up 10", "angle 50", "angle -125" etc
	// or "grid" followed by the grid position with crane as 0 0, i.e. 6 blocks to the right and 3 up from the crane is "grid 6 3"
	if (args[0] == "grid") {
		X = int.Parse(args[1]);
		Y = int.Parse(args[2]);
		Echo(X.ToString());
		Echo(Y.ToString());
		move_to_grid = true;
		move_out_pistons = true;
		move_rotor = true;
	}
	
	if (args[0] == "up") {
		up_dist = Convert.ToDouble(args[1]);
		move_up_pistons = true;
	}
	
		if (args[0] == "out") {
		out_dist = Convert.ToDouble(args[1]);
		move_out_pistons = true;
	}	
		if (args[0] == "angle") {
		rotor_angle = Convert.ToDouble(args[1]);
		move_rotor = true;
	}	

}

public void calculateGridCoordinates(int X, int Y) {
	// Calculate hypothenuse of triangle given by crane base (0,0) and the target grid position (X, Y)
	double X2 = Math.Pow(X,2);
	double Y2 = Math.Pow(Y,2);
	double hypothenuse = Math.Sqrt(X2 + Y2);
	// This hypothenuse is the distance the Out pistons should be extended, given in number of grids. When they are fully retracted they stick out 10.2m (just over 4 grids),
	// therefore we take -5 so a target grid of 5 means a 0 grid extension, and remove the 0.2 for the two heads.
	out_dist = ((hypothenuse - 5)*2.5) - 0.2;
	// Target angle is given by inverse sine (in radians, need to convert to degrees)
	rotor_angle = Math.Asin(X / hypothenuse);
	rotor_angle = rotor_angle * 180 / 3.1415; // Convert from radians to degrees
	
	if (X > 0 && Y < 0) {
		rotor_angle = 180 - rotor_angle;
	}
	
	if (X < 0 && Y < 0) {
		rotor_angle = -180 - rotor_angle;
	}
	
	
	
	Echo(rotor_angle.ToString());
}

public void moveUpPistons(double targetUp) {
	
   double currentUp = piston_Up_1.CurrentPosition;
   currentUp = currentUp * 2; // Both pistons are moved together so always have the same extension
	
	if (currentUp < targetUp - 0.1) {
		
		piston_Up_1.MaxLimit = Convert.ToSingle(targetUp / 2);
		piston_Up_2.MaxLimit = Convert.ToSingle(targetUp / 2);
		piston_Up_1.MinLimit = 0;
		piston_Up_2.MinLimit = 0;
		piston_Up_1.Extend();
		piston_Up_2.Extend();
	}
	if (currentUp > targetUp + 0.1) {
		
		piston_Up_1.MinLimit = Convert.ToSingle(targetUp / 2);
		piston_Up_2.MinLimit = Convert.ToSingle(targetUp / 2);
		piston_Up_1.MaxLimit = 10;
		piston_Up_2.MaxLimit = 10;
		piston_Up_1.Retract();
		piston_Up_2.Retract();
	}
	//if (currentUp > (targetUp - 0.1) && currentUp < (targetUp + 0.1))
		// END
}
	


public void moveOutPistons(double targetOut) {
	
   double currentOut = piston_Out_1.CurrentPosition;
   currentOut = currentOut * 2; // Both pistons are moved together so always have the same extension
	
	if (currentOut < targetOut - 0.1) {
		
		piston_Out_1.MaxLimit = Convert.ToSingle(targetOut / 2);
		piston_Out_2.MaxLimit = Convert.ToSingle(targetOut / 2);
		piston_Out_1.MinLimit = 0;
		piston_Out_2.MinLimit = 0;
		piston_Out_1.Extend();
		piston_Out_2.Extend();
	}
	if (currentOut > targetOut + 0.1) {
		
		piston_Out_1.MinLimit = Convert.ToSingle(targetOut / 2);
		piston_Out_2.MinLimit = Convert.ToSingle(targetOut / 2);
		piston_Out_1.MaxLimit = 10;
		piston_Out_2.MaxLimit = 10;
		piston_Out_1.Retract();
		piston_Out_2.Retract();
	}
		
	//if (currentOut > (targetOut - 0.1) && currentOut < (targetOut + 0.1))
		// END

}
	


public void moveRotor(double targetAngle) {

	double currentAngle = main_Rotor.Angle;
	currentAngle = currentAngle * 180 / 3.1415; // Convert from radians to degrees
	if (currentAngle > targetAngle - 0.1) {
		
		main_Rotor.LowerLimitDeg = Convert.ToSingle(targetAngle);
		main_Rotor.TargetVelocityRPM = Convert.ToSingle(-1);
	}
	
		if (currentAngle < targetAngle + 0.1) {
		
		main_Rotor.UpperLimitDeg = Convert.ToSingle(targetAngle);
		main_Rotor.TargetVelocityRPM = Convert.ToSingle(1);
	}
}