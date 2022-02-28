


// Declare variables: welder, sorter, piston
public IMyPistonBase piston;
public IMyShipWelder welder;
//public IMyPistonBase sorter;
public IMyInventory welder_inventory;


// Required materials for BP
public int target_steel = 1635;
public int target_construction = 129;
public int target_interior = 225;
public int target_computer = 26;
public int target_motor = 42;
public int target_display = 3;
public int target_smalltube = 26;
public int target_powercell = 2;

// Define all the item types needed
MyItemType comp_steel = new MyItemType("MyObjectBuilder_Component", "SteelPlate");
MyItemType comp_construction = new MyItemType("MyObjectBuilder_Component", "Construction");
MyItemType comp_interior = new MyItemType("MyObjectBuilder_Component", "InteriorPlate");
MyItemType comp_computer = new MyItemType("MyObjectBuilder_Component", "Computer");
MyItemType comp_motor = new MyItemType("MyObjectBuilder_Component", "Motor");
MyItemType comp_display = new MyItemType("MyObjectBuilder_Component", "Display");
MyItemType comp_smalltube = new MyItemType("MyObjectBuilder_Component", "SmallTube");
MyItemType comp_powercell = new MyItemType("MyObjectBuilder_Component", "PowerCell");

// Stages
public string stage;

public void Main(string argument) {


	piston = GridTerminalSystem.GetBlockWithName("Container Constructor Piston") as IMyPistonBase;
	
	/*
	List<MyInventoryItem> itemList = new List<MyInventoryItem>();
		welder_inventory.GetItems(itemList);
	foreach (MyInventoryItem item in itemList) {
		Echo(item.Type.ToString());
	}
	*/
	

		

	
	

	
	// Find all container, assemblers, refineries, welders and connectors on the grid
	var cargo = new List<IMyCargoContainer>();
	GridTerminalSystem.GetBlocksOfType(cargo);
	var assembler = new List<IMyAssembler>();
	GridTerminalSystem.GetBlocksOfType(assembler);
	var refinery = new List<IMyRefinery>();
	GridTerminalSystem.GetBlocksOfType(refinery);
	var connector = new List<IMyShipConnector>();
	GridTerminalSystem.GetBlocksOfType(connector);
	var welder = new List<IMyShipWelder>();
	GridTerminalSystem.GetBlocksOfType(welder);
	
	
	
	// Create a list with all inventories, starting with the assembler then cargo containers

	List<IMyInventory> inventories = new List<IMyInventory>[];
	foreach(var c in assembler) {inventories.Add(c.GetInventory(1));}
	foreach(var c in cargo) {inventories.Add(c.GetInventory());}
	foreach(var c in refinery) {inventories.Add(c.GetInventory(1));}
	foreach(var c in connector) {inventories.Add(c.GetInventory());}
	foreach(var c in welder) {inventories.Add(c.GetInventory());}


	int curr_steel = currItem(comp_steel);
	/*
	bool have_all_components = true;

	// If any component is less than the target amount grab from any connected inventory
	if (curr_steel < target_steel) {
		have_all_components = false;		
	}
	if (curr_construction < target_construction) {
		have_all_components = false;		
	}
	if (curr_interior < target_interior) {		
		have_all_components = false;
	}
	if (curr_computer < target_computer) {	
		have_all_components = false;
	}
	if (curr_motor < target_motor) {		
		have_all_components = false;
	}
	if (curr_display < target_display) {	
		have_all_components = false;
	}
	if (curr_metalgrid < target_metalgrid) {		
		have_all_components = false;
	}
	if (curr_smalltube < target_smalltube) {		
		have_all_components = false;
	}
	if (curr_powercell < target_powercell) {	
		have_all_components = false;
	}
	
	if (have_all_components) {
		stage_collect_components = false;
		stage_retracting_piston = true;
	}
	
	}
	
	
	if (stage_retracting_piston) {
		if (welder.Enabled == false) {
		welder.Enabled = true;
		}
		piston.Velocity = -0.05f;
		piston.Retract();
		if (piston.CurrentPosition < 0.1) {
				welder.Enabled = false;
				stage_retracting_piston = false;
				stage_finished_constructing = true;
		}
	}
	
	
	
	
	
	string display_components = stage_collect_components ? "In progress" : "DONE";
	string display_construction = "Not started";
	if (stage_retracting_piston) {
		display_construction = "In progress";
	} else if (stage_finished_constructing) {
		display_construction = "DONE";
	}
	*/
	
	IMyTextPanel lcd_display = GridTerminalSystem.GetBlockWithName("Welder Display") as IMyTextPanel;
	string screen_output = "CARGO CONTAINER CONSTRUCTION" + "\n" + 
	"Components: "  + "\n" + 
	"    Steel Plates : " + curr_steel + " / " + target_steel;
/*
	+ "\n" +
	"    Construction Comp : " + curr_construction + " / " + target_construction + "\n" +
	"    Interior Plates : " + curr_interior + " / " + target_interior + "\n" +
	"    Computer : " + curr_computer + " / " + target_computer + "\n" +
	"    Motor : " + curr_motor + " / " + target_motor + "\n" +
	"    Display : " + curr_display + " / " + target_display + "\n" +
	"    Metal Grids : " + curr_metalgrid + " / " + target_metalgrid + "\n" +
	"    Small Steel Tubes : " + curr_smalltube + " / " + target_smalltube + "\n" +
	"    Power Cells : " + curr_powercell + " / " + target_powercell + "\n" +
	"Construction: ";
	*/
	lcd_display.WriteText(screen_output);
	
	
	

}

public class invList {
	public string name { get; set; }
	public double volume { get; set; }
}

public int currItem(MyItemType item, ) {
		int curr_item = 0;
	foreach(var inv in inventories) {
			curr_item = curr_item + inv.GetItemAmount(item).ToIntSafe();
		}
	return(curr_item);
}

// Required materials
// steel plate 1635
// interior plate 225
// construction component 129
// computer 26
// display 3
// motor 42
// small steel tube 26
// Power Cell 2


