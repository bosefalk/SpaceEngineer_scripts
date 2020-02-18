public Program() {
	turnOff();
}

public IMyPistonBase piston1;
public IMyPistonBase piston2;
public IMyPistonBase piston3;
public IMyPistonBase piston4;
public IMyPistonBase piston5;
public IMyPistonBase pistonX;
public IMyPistonBase pistonY;
// global variables
public int depth;
public string[] grid_l;
public bool extending_drill = false;
public bool retracting_drill = false;
public bool drill_ready = false;
public bool correct_position = false;
public bool drill_in_place = false;




public void Main(string argument, UpdateType updateType) {

piston1 = GridTerminalSystem.GetBlockWithName("Piston 1 Down") as IMyPistonBase;
piston2 = GridTerminalSystem.GetBlockWithName("Piston 2 Up") as IMyPistonBase; 
piston3 = GridTerminalSystem.GetBlockWithName("Piston 3 Up") as IMyPistonBase; 
piston4 = GridTerminalSystem.GetBlockWithName("Piston 4 Up") as IMyPistonBase; 
piston5 = GridTerminalSystem.GetBlockWithName("Piston 5 Up") as IMyPistonBase;

pistonX = GridTerminalSystem.GetBlockWithName("X axis Piston") as IMyPistonBase;
pistonY = GridTerminalSystem.GetBlockWithName("Y axis Piston") as IMyPistonBase;


	// Run Main() every 100 ticks, until cancelled with UpdateFrequency.None
	Runtime.UpdateFrequency = UpdateFrequency.Update100;
	
	// Frist time program is run (with arguments and not from UpdateFrequency.Update100) - set initial public vars
	// which can then be updated by later functions
	if ((updateType & UpdateType.Update100) == 0 & (argument.Length > 0)) { 
		setArgs(argument); // sets drill_ready to true
	}

	if (drill_in_place == false) {
		moveToGrid(); // moves X and Y axis pistons until they have reached grid coordinates, then sets correct_position to true
	}
	
	if (drill_ready == true && correct_position == true) { // start drilling
		drill_ready = false;
		extending_drill = true;
		extendDrill();
	}
	
	if (extending_drill == true) {
		Echo("Currently drilling");
		extendDrill();
	}
	
	if (piston1.CurrentPosition == 10 && retracting_drill == true) { 
		Echo("Currently retracting");
		retracting_drill = false;
		drill_ready = true;
		if (drill_in_place == true) {
			turnOff();
		} else {
		grid_l = grid_l.Skip(1).ToArray();  // Delete grid array element where we just finished drilling
		}
	}
	
	if (grid_l.Length == 0) { // reset / turn off
		turnOff();
	}
}

public void turnOff() {
	extending_drill = false;
	retracting_drill = false;
	drill_ready = false;
	correct_position = false;
	drill_in_place = false;
	Runtime.UpdateFrequency = UpdateFrequency.None; // Turn off program
}


public void setArgs(string argument) {
	string[] args = argument.Split(null); // default is to split string by whitespace
	depth = Convert.ToInt32(args[0]);
	if (argument.Length == 1) { // passed only depth
		drill_in_place = true;
		correct_position = true;
	}
	if (argument.Length > 1) {
		grid_l = args.Skip(1).ToArray();
	}
	drill_ready = true; // TODO add checks that drill is in fact in ready position, dont just assume it
}


public void moveToGrid() {
	
	
	string grid = grid_l[0];
	string future_grids = string.Join(", ", grid_l.Skip(1));
	Echo($"Current grid is {grid}");
	if (future_grids.Length > 0) {
		Echo($"Future grids are {future_grids}");
	}
	if (grid == "0")
		moveAxisPiston(5, 5);
	if (grid == "1")
		moveAxisPiston(10, 10);
	if (grid == "2")
		moveAxisPiston(6.66, 10);
	if (grid == "3")
		moveAxisPiston(3.33, 10);
	if (grid == "4")
		moveAxisPiston(0, 10);
	if (grid == "5")
		moveAxisPiston(10, 6.66);
	if (grid == "6")
		moveAxisPiston(6.66, 6.66);
	if (grid == "7")
		moveAxisPiston(3.33, 6.66);
	if (grid == "8")
		moveAxisPiston(0, 6.66);
	if (grid == "9")
		moveAxisPiston(10, 3.33);
	if (grid == "10")
		moveAxisPiston(6.66, 3.33);
	if (grid == "11")
		moveAxisPiston(3.33, 3.33);
	if (grid == "12")
		moveAxisPiston(0, 3.33);
	if (grid == "13")
		moveAxisPiston(10, 0);
	if (grid == "14")
		moveAxisPiston(6.66, 0);
	if (grid == "15")
		moveAxisPiston(3.33, 0);
	if (grid == "16")
		moveAxisPiston(0, 0);
}

public void moveAxisPiston(double posX, double posY) {
	
   double currX = pistonX.CurrentPosition;
	double currY = pistonY.CurrentPosition;
	
	if (currX < posX - 0.1) {
		Echo("Currently repositioning");
		correct_position = false;
		pistonX.MaxLimit = Convert.ToSingle(posX);
		pistonX.MinLimit = 0;
		pistonX.Extend();
	}
	if (currX > posX + 0.1) {
		Echo("Currently repositioning");
		correct_position = false;
		pistonX.MaxLimit = 10;
		pistonX.MinLimit = Convert.ToSingle(posX);
		pistonX.Retract();
	}
	
	if (currY < posY - 0.1) {
		Echo("Currently repositioning");
		correct_position = false;
		pistonY.MaxLimit = Convert.ToSingle(posY);
		pistonY.MinLimit = 0;
		pistonY.Extend();
	}
	if (currY > posY + 0.1) {
		Echo("Currently repositioning");
		correct_position = false;
		pistonY.MaxLimit = 10;
		pistonY.MinLimit = Convert.ToSingle(posY);
		pistonY.Retract();
	}
	
	if (currX > (posX - 0.1) && currX < (posX + 0.1) &&
		currY > (posY - 0.1) && currY < (posY + 0.1)) {
		Echo("In correct position");
		correct_position = true;
	}
	
}

public void extendDrill() {
	
	if (piston1.CurrentPosition == 10 ) {
       if (depth > 0) {
           piston1.Velocity = 0.1f;
           piston1.Retract();
        } else {
           retractDrill();
		   
       }
   }      
   if (piston1.CurrentPosition == 0 &&
       piston2.CurrentPosition == 0) {
			if (depth > 1) {
           piston2.Velocity = 0.1f;
           piston2.Extend();
		   } else {
			   retractDrill();
		   }
   }
   if (piston2.CurrentPosition == 10 &&
       piston3.CurrentPosition == 0) {
           if (depth > 2) {
            piston3.Velocity = 0.1f;
           piston3.Extend();    
           } else { // depth 2
               retractDrill();
           }
           
   }
   if (piston3.CurrentPosition == 10 &&
       piston4.CurrentPosition == 0) {
           if (depth > 3) {
               piston4.Velocity = 0.1f;
               piston4.Extend();
           } else { // depth 3
               retractDrill();
           }
   }
   if (piston4.CurrentPosition == 10 &&
       piston5.CurrentPosition == 0) {
           if (depth > 4) {
               piston5.Velocity = 0.1f;
               piston5.Extend();
           } else { // depth 4
               retractDrill();
           }
   }
   if (piston5.CurrentPosition == 10) {
       retractDrill(); // depth 5
   }

	
}


public void retractDrill() {
	extending_drill = false;
	retracting_drill = true;
	
	piston1.Velocity = 0.5f;
       piston1.Extend();
       piston2.Velocity = 0.5f;
       piston2.Retract();
       piston3.Velocity = 0.5f;
       piston3.Retract();
       piston4.Velocity = 0.5f;
       piston4.Retract();
       piston5.Velocity = 0.5f;
       piston5.Retract();
}