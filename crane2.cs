public IMyPistonBase piston_Up_1;
public IMyPistonBase piston_Up_2;
public IMyPistonBase piston_Out_1;
public IMyPistonBase piston_Out_2;
public IMyMotorStator main_Rotor;
public IMyMotorStator fine_Rotor;
public IMyCameraBlock camera_front;
public IMyLandingGear magnetic_plate;
public IMyTextSurfaceProvider seat;
public IMyTextSurface seat_display_3;
public IMyTextSurface seat_display_2;
public IMyTextSurface seat_display_1;
public IMyTextSurface seat_display_4;
public IMyProgrammableBlock CraneSupport;


// global variables
public double up_dist; // distance in meters above lowest possible position of UP pistons, 0 - 20 max
public double out_dist; // distance in meters to extend OUT pistons, 0 - 20 max
public double rotor_angle; // turning the crane itself
public double X; //Grid coordinate
public double Y; //Grid coordinate
public double curr_X; //Grid coordinate
public double curr_Y; //Grid coordinate
public string curr_fineDirection;
public double fine_rotor_angle; // Finetuning the cargo angle at the front of the crane independently of the crane itself
public string crane_name;
	public string crane; // input args
	public string up;
	public string grid;
	public string fine;
	public string adj;
	public bool save_grid;
	public string saved_grid = "";
	public bool pickup;
	public bool to_save;
	public bool to_load;
	public double saved_up_dist;
	public double saved_out_dist;
	public double saved_rotor_angle;
	public double saved_fine_angle;
	public bool rover;
	public bool roverhigh;


public Program() {
	
	saved_grid = Storage;

}



public void Main(string argument) {
	
	parseArgs(argument);

	seat = GridTerminalSystem.GetBlockWithName("Crane Control Seat") as IMyTextSurfaceProvider;
	seat_display_2 = seat.GetSurface(2);
	seat_display_3 = seat.GetSurface(3);
	seat.GetSurface(1).WriteText("1");
	CraneSupport = GridTerminalSystem.GetBlockWithName("CraneSupport Program") as IMyProgrammableBlock;

	

	
	if (crane == "alpha") {
		crane_name = "Alpha";
	}
		
	piston_Up_1 = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Up 1") as IMyPistonBase;
	piston_Up_2 = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Up 2") as IMyPistonBase;
	piston_Out_1 = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Out 1") as IMyPistonBase;
	piston_Out_2 = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Out 2") as IMyPistonBase;
	main_Rotor = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Main Rotor") as IMyMotorStator;
	fine_Rotor = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Fine Rotor") as IMyMotorStator;
	camera_front = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Front Camera") as IMyCameraBlock;
	magnetic_plate = GridTerminalSystem.GetBlockWithName(crane_name + " Crane Plate") as IMyLandingGear;
	
	
	if (pickup == true) {
		pickUpContainerSeenByCamera();
	}
	
	if (rover == true) {
		if (roverhigh == false) {
		positionRoverConnector("rover");
		} else {
			positionRoverConnector("roverhigh");
		}
	}
	
	if (to_save == true) {
		toSave();
	}
	
	if (to_load == true) {
		load();
	}
	
	if (pickup == false) {
	
	if (up != "curr") {
		double up_d = Convert.ToSingle(up);
		moveUpPistons(up_d);
	}
	
	if (grid != "curr") {

		
		if (grid.Any(c => char.IsDigit(c))) {
		
		convertNWSEtoGrid(grid); // Sets X and Y
		calculateGridCoordinates(X, Y); // sets rotor_angle and out_dist
		moveOutPistons(out_dist);
		moveRotor(rotor_angle);
		}
	}

	
	
	if (adj != "curr") {
		adjustPosition(adj);
		calculateFineAngle(fine, rotor_angle); // Set fine_rotor_angle
		moveFineRotor(fine_rotor_angle);
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

	
	seat_display_2.WriteText("2");
	seat.GetSurface(4).WriteText(displaySaved());
}

public void parseArgs(string argument) {
	crane = "alpha"; // TODO allow other crane names as first argument

	if (argument == "camera") {
		pickup = true;
	} else {pickup = false;}
	
	if (argument == "rover" | argument == "roverhigh") {
		rover = true;
		if (argument == "roverhigh") {
			roverhigh = true;
	} else {roverhigh = false;}
	} else {rover = false;}
	
	
	if (argument == "save") {
		to_save = true;
	} else {to_save = false;}
	
	if (argument == "load") {
		to_load = true;
	} else {to_load = false;}
		
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
	
	var args_adj = System.Text.RegularExpressions.Regex.Matches(argument, "adj\\s(.{1,2})");
	if (args_adj.Count == 0) {
		adj = "curr";
	} else {
		adj = args_adj[0].Groups[1].Value;		
	}
	
	
}

public void toSave() {
	

	double currentOut = piston_Out_1.CurrentPosition;
	currentOut = currentOut * 2; // Both pistons are moved together so always have the same extension
	double currentUp = piston_Up_1.CurrentPosition;
	currentUp = currentUp * 2; // Both pistons are moved together so always have the same extension
   	double currentAngleRad = main_Rotor.Angle; 
	double currentAngleDeg = currentAngleRad * 180 / 3.1415; 	
	double currentFineAngleRad = fine_Rotor.Angle;
	double currentFineAngleDeg = currentFineAngleRad * 180 / 3.1415;
	
	Vector3D plate_position = magnetic_plate.GetPosition();
	Vector3D plate_coords = Vector3D.Transform(plate_position, MatrixD.Invert(piston_Up_1.WorldMatrix));
	string approx_saved_X;
	string approx_saved_Z;
		if (plate_coords.X >= 0) {
			approx_saved_X = "E " + Math.Round(plate_coords.X/2.5, 1).ToString();
		} else {
			approx_saved_X = "W " + Math.Round(-plate_coords.X/2.5, 1).ToString();
		}
		if (plate_coords.Z >= 0) {
			approx_saved_Z = "S " + Math.Round(plate_coords.Z/2.5, 1).ToString();
		} else {
			approx_saved_Z = "N " + Math.Round(-plate_coords.Z/2.5, 1).ToString();
		}

	saved_grid = string.Join(";",
        currentUp,
		currentOut,
		currentAngleDeg,
		currentFineAngleDeg,
		approx_saved_X,
		approx_saved_Z
    );
}

public void Save() {

	Storage = saved_grid;

}

public void load() {
	
	
	string[] storedData = saved_grid.Split(';');
	if (storedData.Length == 6) {
		
		saved_up_dist = Convert.ToDouble(storedData[0]);
		saved_out_dist = Convert.ToDouble(storedData[1]); 
		saved_rotor_angle = Convert.ToDouble(storedData[2]);
		saved_fine_angle = Convert.ToDouble(storedData[3]);
		
		moveUpPistons(saved_up_dist);
		moveOutPistons(saved_out_dist);
		moveRotor(saved_rotor_angle);
		moveFineRotor(saved_fine_angle);
		
	}
	
	
	

	
}

public void pickUpContainerSeenByCamera() {
	
	
	camera_front.EnableRaycast = true;
	MyDetectedEntityInfo detector_front = camera_front.Raycast(20, 0, 0);	
	Vector3D detected_position = detector_front.Position;
	Vector3D detected_facing = detector_front.Orientation.Forward;
	
	moveToTargetGrid(detected_position, detected_facing, "container");
}

public void positionRoverConnector(string roverqualifier) {
	
	// Parse position message from rover, saved in PB CustomData
	string[] msg = CraneSupport.CustomData.Split(';');
	Vector3D connector_pos = new Vector3D(Convert.ToDouble(msg[0]),
	Convert.ToDouble(msg[1]),
	Convert.ToDouble(msg[2]));	
	
	Vector3D connector_forward = new Vector3D(Convert.ToDouble(msg[3]),
	Convert.ToDouble(msg[4]),
	Convert.ToDouble(msg[5]));
	if (roverqualifier == "roverhigh") {
	moveToTargetGrid(connector_pos, connector_forward, "roverhigh");
	} else {
		moveToTargetGrid(connector_pos, connector_forward, "rover");
	}
}




public void moveToTargetGrid(Vector3D target_pos, Vector3D target_orientation, string argument) {
	
	/*
	Conversion table between manual N,S,W,E, the X,Y coords used in calculateGridCoordinates and the Z,X coordinates from transformed matrix:
	manual 	|	interal X,Y	| worldMatrix	
	N				Y				-Z
	S				-Y				Z
	E				X				X
	W				-X				-X
	*/
	
	// X and Y location
	Vector3D reference_position = piston_Up_1.GetPosition();
	Vector3D original_target_position = target_pos;
	if (argument == "rover" | argument == "roverhigh") {
		Vector3D offset_from_connector = Vector3D.Multiply(target_orientation, 7.5);
		target_pos = Vector3D.Add(target_pos, offset_from_connector);
	}
	
	Vector3D diff_from_ref = Vector3D.Subtract(target_pos, reference_position);
	Vector3D diff_from_ref_internal = Vector3D.TransformNormal(diff_from_ref, MatrixD.Transpose(piston_Up_1.WorldMatrix));
		
	X = diff_from_ref_internal.X;
	Y = -diff_from_ref_internal.Z;
	
	// START calculateGridCoordinates
	double X2 = Math.Pow(X,2);
	double Y2 = Math.Pow(Y,2);
	double hypothenuse = Math.Sqrt(X2 + Y2);
	
	// This hypothenuse is the distance from the center of the up piston block, to calculate how far the Out pistons should be extended in meters:
	// Remove a block (2.5m) to get to the middle of the magnetic plate, 10m for 2x piston bases, 0.2 for the two heads.
	out_dist = hypothenuse - 2.5 - 0.2 - 10;
	
	// Target angle is given by inverse cos (in radians, need to convert to degrees)
	rotor_angle = Math.Acos(X / hypothenuse);
	rotor_angle = rotor_angle * 180 / 3.1415; // Convert from radians to degrees
	
	
	if (Y < 0) { // Turning to the negative side (south)
		rotor_angle = -rotor_angle;
	}
	
	// The main rotor is upside down, so we need to take the negative of the final calculated angle
	rotor_angle = -rotor_angle;
	// END calculateGridCoordinates
	
	moveOutPistons(out_dist);
	moveRotor(rotor_angle);
	
	// Fine angle rotor matching to target orientation
	
	Vector3D facing_diff_target = Vector3D.TransformNormal(target_orientation, MatrixD.Transpose(piston_Up_1.WorldMatrix));
	
	double X_rot = facing_diff_target.X;
	double Y_rot = -facing_diff_target.Z;
	double X2_rot = Math.Pow(X_rot,2);
	double Y2_rot = Math.Pow(Y_rot,2);
	double hypothenuse_rot = Math.Sqrt(X2_rot + Y2_rot);
	double pickup_target_angle = Math.Acos(X_rot / hypothenuse);
	pickup_target_angle = pickup_target_angle * 180 / 3.1415; // Convert from radians to degrees
	
	Echo(X_rot.ToString());
	Echo(Y_rot.ToString());
	if (Y_rot < 0) { // Turning to the negative side (south)
		pickup_target_angle = -pickup_target_angle;
	}
	if (argument == "rover" | argument == "roverhigh") {
		pickup_target_angle = pickup_target_angle - 90;
	}
	
	// Pickup angle is towards the battery / the direction the connector is facing on the rover, we are thinking of "forward" as the connector on the container
	pickup_target_angle = -pickup_target_angle;
	
	double matching_fine_angle = rotor_angle + pickup_target_angle;
	moveFineRotor(matching_fine_angle);
	
	
	// Up
	Vector3D diff_from_ref_up = Vector3D.Subtract(original_target_position, reference_position);
	double target_height = diff_from_ref_internal.Y;
	double target_up_rot = target_height -0.86;
	if (argument == "rover) {
		target_up_rot = target_height -1;
	}
	if (argument == "roverhigh") {	
		target_up_rot = target_height + 10;
	}
	
	moveUpPistons(Convert.ToSingle(target_up_rot));
	
	
	
}

public void convertNWSEtoGrid(string argument) {
	// Regex to look for N or S followed by one or two numbers, set as Y (negative if S), then look for W or E and set as X coord (negative if W)
	System.Text.RegularExpressions.Regex pattern_NS = new System.Text.RegularExpressions.Regex("([NSns])(\\d{1,2})"); //N, S, n or s (as the first group), followed by one or two numbers as a second group.
	var rx_NS = pattern_NS.Matches(argument);
	Y = Convert.ToInt32(rx_NS[0].Groups[2].Value); // Second group is the grid coordinate
	if (rx_NS[0].Groups[1].Value == "S" | rx_NS[0].Groups[1].Value == "s") {
			Y = -Y; // If South then Y direction coords are negative
	}
	
	
	System.Text.RegularExpressions.Regex pattern_EW = new System.Text.RegularExpressions.Regex("([EWew])(\\d{1,2})"); //Same with East West
	var rx_EW = pattern_EW.Matches(argument);
	X = Convert.ToInt32(rx_EW[0].Groups[2].Value);
	if (rx_EW[0].Groups[1].Value == "W" | rx_EW[0].Groups[1].Value == "w") {
		X = -X; // West is the negative x direcion
	}
	
	// output input coordinates to the control seat screen. .Groups[0] returns the whole match without groups
	string grid_string = rx_NS[0].Groups[0].Value + "\n" + rx_EW[0].Groups[0].Value;
	seat_display_3.WriteText("Target:" + "\n" + grid_string);
	
	if (save_grid == true) {
		//pickup_grid = grid_string;
	}
}

public void calculateGridCoordinates(double X, double Y) {
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

public void getCurrentPos() {
	// Get current X and Y positions by reading the piston extension and angle and using the inverse of the calculation in calculateGridCoordinates to find _X and _Y current coords
	double currentOut = piston_Out_1.CurrentPosition;
	currentOut = currentOut * 2; // Both pistons are moved together so always have the same extension
   	double currentAngleRad = main_Rotor.Angle; 
	double currentAngleDeg = currentAngleRad * 180 / 3.1415; 	
	double currentFineAngleRad = fine_Rotor.Angle;
	double currentFineAngleDeg = currentFineAngleRad * 180 / 3.1415;
	
	double _hypothenuse = (currentOut + 0.2 + 12.5) / 2.5;
	curr_X = Math.Round(Math.Cos(-currentAngleRad) * _hypothenuse, 1);
	curr_Y = Math.Round(Math.Sin(-currentAngleRad) * _hypothenuse, 1);
	

	
	if (closeDeg(-currentAngleDeg - currentFineAngleDeg)) {
	curr_fineDirection = "E"; }
	if (closeDeg((-currentAngleDeg + 90) - currentFineAngleDeg)) {
	curr_fineDirection = "N"; }
	if (closeDeg((-currentAngleDeg - 90) - currentFineAngleDeg)) {
	curr_fineDirection = "S"; }
	if (closeDeg((-currentAngleDeg + 180) - currentFineAngleDeg) | closeDeg((currentAngleDeg - 180) - currentFineAngleDeg)) {
	curr_fineDirection = "W"; }
	
}

public bool closeDeg(double A) {
		if (A > -2 && A < 2) {
			return true;
		} else {return false;}
}
		
public void adjustPosition(string adj) {
	
	getCurrentPos(); //Sets curr_X and curr_Y
	
	if (adj == "N") {
		X = curr_X;
		Y = curr_Y + 0.1;
	}
		if (adj == "NE") {
		X = curr_X + 0.1;
		Y = curr_Y + 0.1;
	}
		if (adj == "E") {
		X = curr_X + 0.1;
		Y = curr_Y;
	}
	if (adj == "SE") {
		X = curr_X + 0.1;
		Y = curr_Y - 0.1;
	}
		if (adj == "S") {
		X = curr_X;
		Y = curr_Y - 0.1;
	}
		if (adj == "SW") {
		X = curr_X - 0.1;
		Y = curr_Y - 0.1;
	}
	if (adj == "W") {
		X = curr_X - 0.1;
		Y = curr_Y;
	}
	if (adj == "NW") {
		X = curr_X - 0.1;
		Y = curr_Y + 0.1;
	}
	calculateGridCoordinates(X, Y);
	moveOutPistons(out_dist);
	moveRotor(rotor_angle);
	Echo(curr_fineDirection);
	calculateFineAngle(curr_fineDirection, rotor_angle); // Set fine_rotor_angle
	moveFineRotor(fine_rotor_angle);
}

public void moveUpPistons(double targetUp) {
	
	
   double currentUp = piston_Up_1.CurrentPosition;
   currentUp = currentUp * 2; // Both pistons are moved together so always have the same extension
	
	if (currentUp < targetUp - 0.01) {
		
		piston_Up_1.MaxLimit = Convert.ToSingle(targetUp / 2);
		piston_Up_2.MaxLimit = Convert.ToSingle(targetUp / 2);
		piston_Up_1.MinLimit = 0;
		piston_Up_2.MinLimit = 0;
		piston_Up_1.Extend();
		piston_Up_2.Extend();
	}
	if (currentUp > targetUp + 0.01) {
		
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

public string displaySaved() {
	
	string[] storedData = saved_grid.Split(';');
	if (storedData.Length != 6) {
		return("Saved:" + "\n" + 
		"No saved grid");
	} else {
		
		saved_up_dist = Convert.ToDouble(storedData[0]);
		
		return("Saved:" + "\n" + 
		storedData[5] + "\n" + 
		storedData[4] + "\n" +
		"Up " + Math.Round(saved_up_dist, 1).ToString());		
	}
	
}
