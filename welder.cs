


// Declare variables: welder, sorter, piston
public IMyPistonBase piston;
public IMyShipWelder welder;
//public IMyPistonBase sorter;
public IMyInventory welder_inventory;
public List<IMyInventory> inventories;


// Stages
public bool stage_collect_components = true;
public bool stage_retracting_piston = false;
public bool stage_finished_constructing = false;

public void Main(string argument) {


	welder = GridTerminalSystem.GetBlockWithName("Welder") as IMyShipWelder;
	welder_inventory = welder.GetInventory();
	piston = GridTerminalSystem.GetBlockWithName("Welder Piston") as IMyPistonBase;
	
	/*
	List<MyInventoryItem> itemList = new List<MyInventoryItem>();
		welder_inventory.GetItems(itemList);
	foreach (MyInventoryItem item in itemList) {
		Echo(item.Type.ToString());
	}
	*/
	
		// Define all the item types needed as well as their current amount and target for construction
	MyItemType comp_steel = new MyItemType("MyObjectBuilder_Component", "SteelPlate");
	MyItemType comp_construction = new MyItemType("MyObjectBuilder_Component", "Construction");
	MyItemType comp_interior = new MyItemType("MyObjectBuilder_Component", "InteriorPlate");
	MyItemType comp_computer = new MyItemType("MyObjectBuilder_Component", "Computer");
	MyItemType comp_motor = new MyItemType("MyObjectBuilder_Component", "Motor");
	MyItemType comp_display = new MyItemType("MyObjectBuilder_Component", "Display");
	MyItemType comp_metalgrid = new MyItemType("MyObjectBuilder_Component", "MetalGrid");
	MyItemType comp_smalltube = new MyItemType("MyObjectBuilder_Component", "SmallTube");
	MyItemType comp_powercell = new MyItemType("MyObjectBuilder_Component", "PowerCell");
		
	int curr_steel = welder_inventory.GetItemAmount(comp_steel).ToIntSafe();
	int target_steel = 1300;
	int curr_construction = welder_inventory.GetItemAmount(comp_construction).ToIntSafe();
	int target_construction = 190;
	int curr_interior = welder_inventory.GetItemAmount(comp_interior).ToIntSafe();
	int target_interior = 120;
	int curr_computer = welder_inventory.GetItemAmount(comp_computer).ToIntSafe();
	int target_computer = 51;
	int curr_motor = welder_inventory.GetItemAmount(comp_motor).ToIntSafe();
	int target_motor = 20;
	int curr_display = welder_inventory.GetItemAmount(comp_display).ToIntSafe();
	int target_display = 3;
	int curr_metalgrid = welder_inventory.GetItemAmount(comp_metalgrid).ToIntSafe();
	int target_metalgrid = 12;
	int curr_smalltube = welder_inventory.GetItemAmount(comp_smalltube).ToIntSafe();
	int target_smalltube = 72;
	int curr_powercell = welder_inventory.GetItemAmount(comp_powercell).ToIntSafe();
	int target_powercell = 80;
	
	if (stage_collect_components) {
	

	
	// Find all container, assemblers, refineriesand connectors on the grid
	var cargo = new List<IMyCargoContainer>();
	GridTerminalSystem.GetBlocksOfType(cargo);
	var assembler = new List<IMyAssembler>();
	GridTerminalSystem.GetBlocksOfType(assembler);
	var refinery = new List<IMyRefinery>();
	GridTerminalSystem.GetBlocksOfType(refinery);
	var connector = new List<IMyShipConnector>();
	GridTerminalSystem.GetBlocksOfType(connector);
	
	// Create a list with all inventories, starting with the assembler then cargo containers
	inventories = new List<IMyInventory>();
	
	foreach(var c in assembler) {inventories.Add(c.GetInventory(1));}
	foreach(var c in cargo) {inventories.Add(c.GetInventory());}
	foreach(var c in refinery) {inventories.Add(c.GetInventory(1));}
	foreach(var c in connector) {inventories.Add(c.GetInventory());}


	bool have_all_components = true;

	// If any component is less than the target amount grab from any connected inventory
	if (curr_steel < target_steel) {
		curr_steel = grabItems(comp_steel, target_steel, curr_steel);
		have_all_components = false;		
	}
	if (curr_construction < target_construction) {
		curr_construction = grabItems(comp_construction, target_construction, curr_construction);
		have_all_components = false;		
	}
	if (curr_interior < target_interior) {
		curr_interior = grabItems(comp_interior, target_interior, curr_interior);		
		have_all_components = false;
	}
	if (curr_computer < target_computer) {
		curr_computer = grabItems(comp_computer, target_computer, curr_computer);		
		have_all_components = false;
	}
	if (curr_motor < target_motor) {
		curr_motor = grabItems(comp_motor, target_motor, curr_motor);		
		have_all_components = false;
	}
	if (curr_display < target_display) {
		curr_display = grabItems(comp_display, target_display, curr_display);		
		have_all_components = false;
	}
	if (curr_metalgrid < target_metalgrid) {
		curr_metalgrid = grabItems(comp_metalgrid, target_metalgrid, curr_metalgrid);		
		have_all_components = false;
	}
	if (curr_smalltube < target_smalltube) {
		curr_smalltube = grabItems(comp_smalltube, target_smalltube, curr_smalltube);		
		have_all_components = false;
	}
	if (curr_powercell < target_powercell) {
		curr_powercell = grabItems(comp_powercell, target_powercell, curr_powercell);		
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
	
	
	
	IMyTextPanel lcd_display = GridTerminalSystem.GetBlockWithName("Welder Display") as IMyTextPanel;
	
	string display_components = stage_collect_components ? "In progress" : "DONE";
	string display_construction = "Not started";
	if (stage_retracting_piston) {
		display_construction = "In progress";
	} else if (stage_finished_constructing) {
		display_construction = "DONE";
	}
	
	
	
	string screen_output = "CARGO CONTAINER CONSTRUCTION" + "\n" + 
	"Components: " + display_components + "\n" + 
	"    Steel Plates : " + curr_steel + " / " + target_steel + "\n" +
	"    Construction Comp : " + curr_construction + " / " + target_construction + "\n" +
	"    Interior Plates : " + curr_interior + " / " + target_interior + "\n" +
	"    Computer : " + curr_computer + " / " + target_computer + "\n" +
	"    Motor : " + curr_motor + " / " + target_motor + "\n" +
	"    Display : " + curr_display + " / " + target_display + "\n" +
	"    Metal Grids : " + curr_metalgrid + " / " + target_metalgrid + "\n" +
	"    Small Steel Tubes : " + curr_smalltube + " / " + target_smalltube + "\n" +
	"    Power Cells : " + curr_powercell + " / " + target_powercell + "\n" +
	"Construction: " + display_construction;
	
	lcd_display.WriteText(screen_output);
	
	
	

}

public class invList {
	public string name { get; set; }
	public double volume { get; set; }
}

public int grabItems(MyItemType item_type, int target_amount, int current_amount) {
		foreach(var inv in inventories) {
			if (inv.GetItemAmount(item_type) > 0) { // && inv.IsConnectedTo(welder_inventory)) {
				MyInventoryItem item_to_transfer = (MyInventoryItem)inv.FindItem(item_type);
				inv.TransferItemTo(welder_inventory, item_to_transfer, target_amount - current_amount);
				current_amount = welder_inventory.GetItemAmount(item_type).ToIntSafe();
				if (current_amount >= target_amount) {

					break;
					}
			}
		}
		
		return(current_amount);
	
	
}

// Define required materials
// armor blocks 40
// small cargo container 3
// connector 1
// battery 1
// letter X 2 <----

// steel plate 230 + max 1000
// interior plate 120
// construction component 190
// computer 51
// display 3
// motor 20
// small steel tube 72
// metal grid 12
// Power Cell 80


// Check materials currently in Welder, calculate what is needed


// Turn on sorter, attempt to bring in materials, turn off sorter


// Check materials, print what is needed and pause


// When all materials are present: bring piston to 10m extension


// Turn on welder and slowly bring piston back


// When piston reaches 0, turn off welder and turn off program