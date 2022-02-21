public Program()
{
  // Configure this program to run the Main method every 100 update ticks
  Runtime.UpdateFrequency = UpdateFrequency.Update10;
}



public IMyCameraBlock camera_front;
public IMyLandingGear magnetic_plate;
public IMyExtendedPistonBase piston;

public void Main(string argument) {


	camera_front = GridTerminalSystem.GetBlockWithName("Alpha Crane Front Camera") as IMyCameraBlock;
	magnetic_plate = GridTerminalSystem.GetBlockWithName("Alpha Crane Plate") as IMyLandingGear;
	piston = GridTerminalSystem.GetBlockWithName("Alpha Crane Up 1") as IMyExtendedPistonBase;
	
	camera_front.EnableRaycast = true;
	MyDetectedEntityInfo detector_front = camera_front.Raycast(20, 0, 0);	
	
	Vector3D reference_position = piston.GetPosition();
	Vector3D target_position = detector_front.Position;
	Vector3D diff_from_ref = Vector3D.Subtract(target_position, reference_position);
	
	Vector3D diff_from_ref_internal = Vector3D.TransformNormal(diff_from_ref, MatrixD.Transpose(piston.WorldMatrix));
	Echo("Transposed diff: " + diff_from_ref_internal.ToString());
	/*
	X is E/W
	Z is N/S
	Y is up
	*/
	
	Vector3D plate_position = magnetic_plate.GetPosition();
	Vector3D plate_diff_from_ref = Vector3D.Subtract(plate_position, reference_position);
	Vector3D plate_diff_from_ref_internal = Vector3D.TransformNormal(plate_diff_from_ref, MatrixD.Transpose(piston.WorldMatrix));
	Echo("Plate diff: " + plate_diff_from_ref_internal.ToString());
	
	

	
	//Echo(camera_front.AvailableScanRange.ToString());
	
	
	
	//https://stackoverflow.com/questions/5995317/how-to-convert-c-sharp-nullable-int-to-int
	
	//Vector3D hit_front = detector_front.HitPosition ?? default(Vector3D);
	
	
	
	//Echo(detector_front.Name);
	/*
	
	Echo(detector_front.Type.ToString());
	Echo("Position:");
	Echo(detector_front.Position.X.ToString());
	Echo(detector_front.Position.Y.ToString());
	Echo(detector_front.Position.Z.ToString());
	
	
	
	Echo("Crane position:");
	Echo(magnetic_plate.GetPosition().X.ToString());
	Echo(magnetic_plate.GetPosition().Y.ToString());
	Echo(magnetic_plate.GetPosition().Z.ToString());
	*/
	
	
	/*
	Vector3D diff = detector_front.Position.Subtract(detector_front.Position, magnetic_plate.GetPosition());
	Echo("Diff");
	Echo(diff.X.ToString());
	Echo(diff.Y.ToString());
	Echo(diff.Z.ToString());
	*/
	/*
	IMyCubeGrid maingrid = piston.CubeGrid as IMyCubeGrid;
	Vector3I piston_position = piston.Position;
	Echo("Piston position:" + piston_position.ToString());
	Vector3D main_orientation = maingrid.GridIntegerToWorld(piston_position);
	Echo("piston realworld:" + main_orientation.ToString());
	*/
	
	
	/*
	Vector3D diff = VRageMath.Vector3D.Subtract(magnetic_plate.GetPosition(), detector_front.Position);
	Echo("Relative target poisiton:" + diff.ToString());
	
	
	Echo("UnitX" + diff.UnitX.ToString());
	Echo("UnitY" + diff.UnitX.ToString());
	Echo("UnitZ" + diff.UnitX.ToString());
	*/
	
	/*
	Vector3I internal_detector_location = maingrid.WorldToGridInteger(detector_front.Position);
	Echo("target internal position:" + internal_detector_location.ToString());
	*/
	
	
	
	
	/*
	Matrix crane_orientation;
	magnetic_plate.Orientation.GetMatrix(out crane_orientation);
	MatrixD detector_front_orientation = detector_front.Orientation;
	
	Enum crane_forward = magnetic_plate.Orientation.Forward;
	
	Echo("Crane orientation");
	*/
	
	/*Echo(crane_orientation.Col0.ToString());
	Echo(crane_orientation.Col1.ToString());
	Echo(crane_orientation.Col2.ToString());
	//Echo(crane_forward.ToString());
*/
/*
	Vector3I crane_vector = magnetic_plate.Position;
	Echo(crane_vector.ToString());
	
	IMyCubeGrid maingrid = magnetic_plate.CubeGrid as IMyCubeGrid;
	Vector3D main_orientation = maingrid.GridIntegerToWorld(crane_vector);
	Echo(main_orientation.X.ToString());
	Echo(main_orientation.Y.ToString());
	Echo(main_orientation.Z.ToString());
	*/
/*
	Echo("detector_front_orientation orientation");
	Echo(detector_front_orientation.Col0.ToString());
	Echo(detector_front_orientation.Col1.ToString());
	Echo(detector_front_orientation.Col2.ToString());
	*/

}
	
/*

	Echo("Hit position:");
	Echo(hit_front.X.ToString());
	Echo(hit_front.Y.ToString());
	Echo(hit_front.Z.ToString());
	
	*/