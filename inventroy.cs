
IMyTextSurfaceProvider seat;

public Program() {

}



public void Main(string argument, UpdateType updateType) {
	Runtime.UpdateFrequency = UpdateFrequency.Update100;
	//alpha_cargo_display = GridTerminalSystem.GetBlockWithName("Alpha Cargo Display") as IMyTextPanel;
	seat = GridTerminalSystem.GetBlockWithName("Crane Control Seat") as IMyTextSurfaceProvider;
	IMyTextSurface seat_large_display = seat.GetSurface(0);



	string screen_output = "CURRENT CONTAINERS CONNECTED:" + "\n" +
	"A1: " + findConnected("1") + " " + printCargoPerc("1") + "\n" + 
	"A2: " + findConnected("2") + " " + printCargoPerc("2") + "\n" +
	"A3: " + findConnected("3") + " " + printCargoPerc("3") + "\n" +
	"A4: " + findConnected("4") + " " + printCargoPerc("4") + "\n" +
	"A5: " + findConnected("5") + " " + printCargoPerc("5") + "\n" +
	"A6: " + findConnected("6") + " " + printCargoPerc("6") + "\n" +
	"A7: " + findConnected("7") + " " + printCargoPerc("7");

	//alpha_cargo_display.WriteText(screen_output);
	seat_large_display.WriteText(screen_output);
	
	

}

public string printCargoPerc(string nr) {
	
	string connected_grid = findConnected(nr);
	if (connected_grid == "Not connected") {
	return "";
	}
	
	Echo(connected_grid);

	var perc_dict = cargoContainer(connected_grid);
	
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

public Dictionary<string, double> cargoContainer(string gridName) {

		
		IMyCargoContainer cont_1 = GridTerminalSystem.GetBlockWithName("Cont_" + gridName + "_1") as IMyCargoContainer;
		IMyCargoContainer cont_2 = GridTerminalSystem.GetBlockWithName("Cont_" + gridName + "_2") as IMyCargoContainer;
		IMyCargoContainer cont_3 = GridTerminalSystem.GetBlockWithName("Cont_" + gridName + "_3") as IMyCargoContainer;
	
	
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

