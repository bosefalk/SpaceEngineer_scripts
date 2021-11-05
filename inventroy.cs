cargoContainer alpha_1_container;
cargoContainer alpha_2_container;
cargoContainer alpha_3_container;
cargoContainer alpha_4_container;
cargoContainer alpha_5_container;
cargoContainer alpha_6_container;
cargoContainer alpha_7_container;


public Program() {
	alpha_1_container = new cargoContainer(this);
	alpha_2_container = new cargoContainer(this);
	alpha_3_container = new cargoContainer(this);
	alpha_4_container = new cargoContainer(this);
	alpha_5_container = new cargoContainer(this);
	alpha_6_container = new cargoContainer(this);
	alpha_7_container = new cargoContainer(this);
}



public void Main(string argument) {

	Echo("A1: " + findConnected("1") + " " + printCargoPerc("1"));
	Echo("A2: " + findConnected("2") + " " + printCargoPerc("2"));
	Echo("A3: " + findConnected("3") + " " + printCargoPerc("3"));
	Echo("A4: " + findConnected("4") + " " + printCargoPerc("4"));
	Echo("A5: " + findConnected("5") + " " + printCargoPerc("5"));
	Echo("A6: " + findConnected("6") + " " + printCargoPerc("6"));
	Echo("A7: " + findConnected("7") + " " + printCargoPerc("7"));
}

public string printCargoPerc(string nr) {
	
	string connected_grid = findConnected(nr);
	if (connected_grid == "Not connected") {
	return "";
	}
	
	alpha_2_container.setup(connected_grid);
	var perc_dict = alpha_2_container.getPerc();
	
	string perc_string;
	int  perc_dict_length = perc_dict.Count;
	
	if (perc_dict_length == 0) {
	return "Empty";
	}
	
	if (perc_dict_length > 3) {
		return "Mix todo %";
	}
		
	perc_string = perc_dict.ElementAt(0).Key + " " +Math.Round((perc_dict.ElementAt(0).Value * 100), 0).ToString() + "%";
	if (perc_dict_length > 1) {
	perc_string = perc_string + " " + perc_dict.ElementAt(1).Key + " " + Math.Round((perc_dict.ElementAt(1).Value * 100), 0).ToString() + "%";
	}
	if (perc_dict_length > 2) {
	perc_string = perc_string + " " + perc_dict.ElementAt(2).Key + " " + Math.Round((perc_dict.ElementAt(2).Value * 100), 0).ToString() + "%";	
	}
	return perc_string;
	
	
	
}


public string findConnected(string nr) {
	IMyShipConnector alpha_connector = GridTerminalSystem.GetBlockWithName("Alpha Cargo Connector " + nr) as IMyShipConnector;
	IMyShipConnector connected_alpha = alpha_connector.OtherConnector;
	if (connected_alpha == null) {
	return "Not connected";}
	
	IMyCubeGrid connected_grid = connected_alpha.CubeGrid;
	return connected_grid.CustomName;
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

	public Dictionary<string, double> getPerc() {
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
		/*sumList.ForEach(e => {
        	double perc = e.totalVol / max_vol;
        	_program.Echo(e.name + " " + perc.ToString() + "%");
        	});*/
			
		Dictionary<string, double> percDict = new Dictionary<string, double>();
		sumList.ForEach(e => {
			double perc = e.totalVol / max_vol;
		percDict.Add(e.name, perc);
		});

		return (percDict);

	}

}