
string _broadCastTag = "Rover Connector Position";
public IMyShipConnector connector;

public Program()
{

}


public void Main(string argument, UpdateType updateSource)
{
	connector = GridTerminalSystem.GetBlockWithName("Rover Connector") as IMyShipConnector;
	
	Vector3D connector_pos = connector.GetPosition();
	Vector3 connector_forward = connector.WorldMatrix.Forward;
	
	string connector_pos_string = string.Join(";",
	connector_pos.X.ToString(),
	connector_pos.Y.ToString(),
	connector_pos.Z.ToString(),
	connector_forward.X.ToString(),
	connector_forward.Y.ToString(),
	connector_forward.Z.ToString());
	
	
	
    if (
        (updateSource & (UpdateType.Trigger | UpdateType.Terminal)) > 0 // run by a terminal action
        || (updateSource & (UpdateType.Mod)) > 0 // script run by a mod
        || (updateSource & (UpdateType.Script)) > 0 // this pb run by another script (PB)
        )	{ // script was run because of an action

            
            IGC.SendBroadcastMessage(_broadCastTag, connector_pos_string);
            Echo("Sending message:\n" + connector_pos_string);
        
    }

}