

string _broadCastTag = "Rover Connector Position";
IMyBroadcastListener _myBroadcastListener;
IMyProgrammableBlock PB;
public IMyPistonBase piston_Up_1;
public Vector3D connector_pos;
public Vector3D connector_forward;

public Program()
{
    Echo("Creator");
    _myBroadcastListener=IGC.RegisterBroadcastListener(_broadCastTag);
    _myBroadcastListener.SetMessageCallback(_broadCastTag); 
}

public void Main(string argument, UpdateType updateSource)
{
	piston_Up_1 = GridTerminalSystem.GetBlockWithName("Alpha" + " Crane Up 1") as IMyPistonBase;
	PB = GridTerminalSystem.GetBlockWithName("CraneSupport Program") as IMyProgrammableBlock;
	IMyTextSurface PB_display = PB.GetSurface(0);
	

	
	
if ((updateSource & UpdateType.IGC) > 0) { // script was run because of incoming IGC message
        while (_myBroadcastListener.HasPendingMessage) {
            var myIGCMessage = _myBroadcastListener.AcceptMessage();
            if (myIGCMessage.Tag == _broadCastTag) { // This is our tag
                if (myIGCMessage.Data is string) {
                    PB.CustomData = myIGCMessage.Data.ToString();
		

					}
				}

        }
    }
	

	
	
	parseMsg();
	
	
	Vector3D reference_position = piston_Up_1.GetPosition();
	Vector3D diff_from_ref = Vector3D.Subtract(connector_pos, reference_position);
	Vector3D diff_from_ref_internal = Vector3D.TransformNormal(diff_from_ref, MatrixD.Transpose(piston_Up_1.WorldMatrix));
	
	Vector3D facing_diff_target = Vector3D.TransformNormal(connector_forward, MatrixD.Transpose(piston_Up_1.WorldMatrix));
	
	
	PB_display.WriteText(PB.CustomData + "\n" + 
	connector_pos.ToString() + "\n" + 
	connector_forward.ToString() + "\n" +
	diff_from_ref_internal.X.ToString() + "\n" + 
	diff_from_ref_internal.Z.ToString() + "\n" + 
	facing_diff_target.X.ToString() + "\n" +
	facing_diff_target.Z.ToString()
	);
	
	
}

public void parseMsg() {
	string[] msg = PB.CustomData.Split(';');
	connector_pos = new Vector3D(Convert.ToDouble(msg[0]),
	Convert.ToDouble(msg[1]),
	Convert.ToDouble(msg[2]));	
	connector_forward = new Vector3D(Convert.ToDouble(msg[3]),
	Convert.ToDouble(msg[4]),
	Convert.ToDouble(msg[5]));
	
}