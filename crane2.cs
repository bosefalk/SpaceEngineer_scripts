public IMyPistonBase piston_Up_1;
public IMyPistonBase piston_Up_2;
public IMyPistonBase piston_Out_1;
public IMyPistonBase piston_Out_2;
public IMyMotorStator main_Rotor;
public IMyMotorStator fine_Rotor;
// global variables
public double up_dist; // distance in meters above lowest possible position of UP pistons, 0 - 20 max
public double out_dist; // distance in meters to extend OUT pistons, 0 - 20 max
public double rotor_angle; // turning the crane itself
public int X; //Grid coordinate
public int Y; //Grid coordinate
public double fine_rotor_angle; // Finetuning the cargo angle at the front of the crane independently of the crane itself
public string crane_name;
	public string crane; // input args
	public string up;
	public string grid;
	public string fine;

public void Main(string argument) {
	

	parseArgs(argument);
	
	//string crane = "alpha", string up = "curr", string grid = "curr", string fine = "curr"
	
	
	if (crane == "alpha") {
		crane_name = "Alpha";
	}
		
	piston_Up_1 = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Up 1") as IMyPistonBase;
	piston_Up_2 = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Up 2") as IMyPistonBase;
	piston_Out_1 = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Out 1") as IMyPistonBase;
	piston_Out_2 = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Out 2") as IMyPistonBase;
	main_Rotor = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Main Rotor") as IMyMotorStator;
	fine_Rotor = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Fine Rotor") as IMyMotorStator;
	
	
	if (up != "curr") {
		double up_d = Convert.ToSingle(up);
		moveUpPistons(up_d);
	}
	
	if (grid != "curr") {
		convertNWSEtoGrid(grid); // Sets X and Y
		calculateGridCoordinates(X, Y); // sets rotor_angle and out_dist
		moveOutPistons(out_dist);
		moveRotor(rotor_angle);
	}
	
	if (fine != "curr") {
		if (grid != "curr") { // Use target rotor angle set above
			calculateFineAngle(fine, rotor_angle); // Set fine_rotor_angle
			moveFineRotor(fine_rotor_angle);
		}
		if (grid == "curr"){ // Turn in place with current angle
			double currentAngle = main_Rotor.Angle;
			currentAngle = currentAngle * 180 / 3.1415; // Convert from radians to degrees
			calculateFineAngle(fine, currentAngle);
			moveFineRotor(fine_rotor_angle);
		}
	}

}

public void parseArgs(string argument) {
	crane = "alpha"; // TODO allow other crane names as first argument
	
	var args_up = System.Text.RegularExpressions.Regex.Matches(argument, "up\\s(\\d{1,2})");
	if (args_up.Count == 0) {
		up = "curr";
	} else {
		up = args_up[0].Groups[1].Value;
	}
	
	var args_grid = System.Text.RegularExpressions.Regex.Matches(argument, "grid\\s(.{4,6})");
	if (args_grid.Count == 0) {
		grid = "curr";
	} else {
		grid = args_grid[0].Groups[1].Value;
	}
	
	var args_fine = System.Text.RegularExpressions.Regex.Matches(argument, "fine\\s(.{1})");
	if (args_fine.Count == 0) {
		fine = "curr";
	} else {
		fine = args_fine[0].Groups[1].Value;
	}
	
	
}


public void convertNWSEtoGrid(string argument) {
	// Regex to look for N or S followed by one or two numbers, set as Y (negative if S), then look for W or E and set as X coord (negative if W)
	System.Text.RegularExpressions.Regex pattern_NS = new System.Text.RegularExpressions.Regex("([NSns])(\\d{1,2})"); //N, S, n or s (as the first group), followed by one or two numbers as a second group.
	var rx_NS = pattern_NS.Matches(argument);
	Y = Convert.ToInt32(rx_NS[0].Groups[2].Value); // Second group is the grid coordinate
	if (rx_NS[0].Groups[1].Value == "S" | rx_NS[0].Groups[1].Value == "s") {
			Y = -Y; // If South then Y direction coords are negative
	}
	
	Echo("X" + X.ToString());
	
	System.Text.RegularExpressions.Regex pattern_EW = new System.Text.RegularExpressions.Regex("([EWew])(\\d{1,2})"); //Same with East West
	var rx_EW = pattern_EW.Matches(argument);
	X = Convert.ToInt32(rx_EW[0].Groups[2].Value);
	if (rx_EW[0].Groups[1].Value == "W" | rx_EW[0].Groups[1].Value == "w") {
		X = -X; // West is the negative x direcion
	}
	Echo("Y" + Y.ToString());
	
}

public void calculateGridCoordinates(int X, int Y) {
	// Calculate hypothenuse of triangle given by crane base (0,0) and the target grid position (X, Y)
	double X2 = Math.Pow(X,2);
	double Y2 = Math.Pow(Y,2);
	double hypothenuse = Math.Sqrt(X2 + Y2);
	
	// This hypothenuse is the distance the Out pistons should be extended, given in number of grids. When they are fully retracted they stick out 10.2m (just over 4 grids),
	// therefore we take -5 so a target grid of 5 means a 0 grid extension, each block is 2.5m and remove the 0.2 for the two heads.
	out_dist = ((hypothenuse - 5)*2.5) - 0.2;
	
	// Target angle is given by inverse cos (in radians, need to convert to degrees)
	rotor_angle = Math.Acos(X / hypothenuse);
	rotor_angle = rotor_angle * 180 / 3.1415; // Convert from radians to degrees
	
	
	if (Y < 0) { // Turning to the negative side (south)
		rotor_angle = -rotor_angle;
	}
	
	// The main rotor is upside down, so we need to take the negative of the final calculated angle
	rotor_angle = -rotor_angle;

}

public void calculateFineAngle(string direction, double target_angle) {
	// target_angle is the angle of the main crane rotor
	double fine_East = target_angle;
	
	if (direction == "N") {
		fine_rotor_angle = fine_East + 90;
	}
	if (direction == "E") {
		fine_rotor_angle = fine_East;
	}
	if (direction == "S") {
		fine_rotor_angle = fine_East - 90;
	}
	if (direction == "W") {
		fine_rotor_angle = fine_East - 180;
	}
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

public void moveFineRotor(double targetAngle) {

	double currentAngle = fine_Rotor.Angle;
	currentAngle = currentAngle * 180 / 3.1415; // Convert from radians to degrees
	if (currentAngle > targetAngle - 0.1) {
		
		fine_Rotor.LowerLimitDeg = Convert.ToSingle(targetAngle);
		fine_Rotor.TargetVelocityRPM = Convert.ToSingle(-1);
	}
	
		if (currentAngle < targetAngle + 0.1) {
		
		fine_Rotor.UpperLimitDeg = Convert.ToSingle(targetAngle);
		fine_Rotor.TargetVelocityRPM = Convert.ToSingle(1);
	}
}

