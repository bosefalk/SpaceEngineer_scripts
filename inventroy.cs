cargoContainer alpha_3_container;

public Program() {
	alpha_3_container = new cargoContainer(this);
}



public void Main(string argument) {


	IMyShipConnector alpha_3 = GridTerminalSystem.GetBlockWithName("Alpha Cargo Connector 3") as IMyShipConnector;
	IMyShipConnector connected_alpha_3 = alpha_3.OtherConnector;
	IMyCubeGrid connected_grid = connected_alpha_3.CubeGrid;
	Echo(connected_grid.CustomName);
	alpha_3_container.setup(connected_grid.CustomName);
	alpha_3_container.getPerc();
	

}

public class invList {
	public string name { get; set; }
	public double volume { get; set; }
}

public class cargoContainer {
	Program _program;

	public string gridName;
	public IMyCargoContainer cont_1;
	public IMyCargoContainer cont_2;
	public IMyCargoContainer cont_3;
	//private string connector;

	public cargoContainer(Program program) {
		_program = program;
		}

	public void setup(string name) {
		gridName = name;
		cont_1 = _program.GridTerminalSystem.GetBlockWithName("Cont_" + gridName + "_1") as IMyCargoContainer;
		cont_2 = _program.GridTerminalSystem.GetBlockWithName("Cont_" + gridName + "_2") as IMyCargoContainer;
		cont_3 = _program.GridTerminalSystem.GetBlockWithName("Cont_" + gridName + "_3") as IMyCargoContainer;
	
	}

	public void getPerc() {
		double max_vol = Convert.ToDouble((cont_1.GetInventory().MaxVolume + cont_2.GetInventory().MaxVolume + cont_3.GetInventory().MaxVolume).ToString());
		List<MyInventoryItem> itemList = new List<MyInventoryItem>();
		cont_1.GetInventory().GetItems(itemList);	
		cont_2.GetInventory().GetItems(itemList);	
		cont_3.GetInventory().GetItems(itemList);

		var joined_list = new List<invList>();
		foreach (MyInventoryItem item in itemList) {
			string tmp_name = item.Type.SubtypeId.ToString();
            var tmp_volume = Convert.ToDouble((item.Amount * item.Type.GetItemInfo().Volume).ToString());
            joined_list.Add(new invList() { name = tmp_name, volume = tmp_volume});
		}
		var sumList = joined_list.GroupBy(p => p.name).Select(g => new {name = g.Key, totalVol = g.Sum(p => p.volume)}).ToList();
		sumList.ForEach(e => {
        	double perc = e.totalVol / max_vol;
        	_program.Echo(e.name + " " + perc.ToString() + "%");
        	});

	}

}