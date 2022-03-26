using UnityEngine;

public class HexCell : MonoBehaviour
{
	public HexCoordinates coordinates;
	public Color Color {
		get { return color; }
		set {
			if (color == value) { return; }
			color = value;
			Refresh();
		}
	}
	private Color color;


	[SerializeField] HexCell[] neighbors = {};
	public HexGridChunk? chunk;

	public int Elevation {
		get {
			return elevation;
		}
		set {
			if (elevation == value)
				{ return; }
			elevation = value;
			Vector3 position = transform.localPosition;
			position.y = value * HexMetrics.elevationStep;
			position.y +=
				(HexMetrics.SampleNoise(position).y * 2f - 1f) *
				HexMetrics.elevationPerturbStrength;
			transform.localPosition = position;

			if (uiRect is RectTransform)
			{
				var uiPosition = uiRect.localPosition;
				uiPosition.z = -position.y;
				uiRect.localPosition = uiPosition;
			}

			if (
				hasOutgoingRiver &&
				elevation < GetNeighbor(outgoingRiver).elevation
			){
				RemoveOutgoingRiver();
			}
			if (
				hasIncomingRiver &&
				elevation > GetNeighbor(incomingRiver).elevation
			){
				RemoveIncomingRiver();
			}

			Refresh();
		}
	}
	int elevation = int.MinValue;

	public RectTransform? uiRect;

	public Vector3 Position {
		get {
			return transform.localPosition;
		}
	}



	void Refresh()
	{
		if (chunk is HexGridChunk)
		{
			chunk.Refresh();
			for (int i = 0; i < neighbors.Length; i++)
			{
				HexCell neighbor = neighbors[i];
				if (neighbor != null && neighbor.chunk != chunk)
				{
					neighbor.chunk?.Refresh();
				}
			}
		}
	}

	public HexCell GetNeighbor (HexDirection direction)
	{
		return neighbors[(int)direction];
	}
	public void SetNeighbor (HexDirection direction, HexCell cell)
	{
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public HexEdgeType GetEdgeType(HexDirection direction)
	{
		return HexMetrics.GetEdgeType(
			elevation, neighbors[(int)direction].elevation
		);
	}

	public HexEdgeType GetEdgeType(HexCell otherCell)
	{
		return HexMetrics.GetEdgeType(
			elevation, otherCell.elevation
		);
	}


#region River

	bool hasIncomingRiver, hasOutgoingRiver;
	HexDirection incomingRiver, outgoingRiver;

	public bool HasIncomingRiver {
		get { return hasIncomingRiver; }
	}
	public bool HasOutgoingRiver{
		get { return hasOutgoingRiver; }
	}
	public HexDirection IncomingRiver {
		get { return incomingRiver; }
	}
	public HexDirection OutgoingRiver {
		get { return outgoingRiver; }
	}

	public bool HasRiver {
		get { return hasIncomingRiver || hasOutgoingRiver; }
	}
	public bool HasRiverBeginOrEnd {
		get { return hasIncomingRiver != hasOutgoingRiver; }
	}
	public bool HasRiverThroughEdge (HexDirection direction){
		return
			hasIncomingRiver && incomingRiver == direction ||
			hasOutgoingRiver && outgoingRiver == direction;
	}

	public void RemoveOutgoingRiver()
	{
		if (!hasOutgoingRiver)
		{
			return;
		}
		hasOutgoingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(outgoingRiver);
		neighbor.hasIncomingRiver = false;
		neighbor.RefreshSelfOnly();
	}

	void RefreshSelfOnly()
	{
		chunk?.Refresh();
	}
	public void RemoveIncomingRiver()
	{
		if (!hasIncomingRiver) { return; }

		hasIncomingRiver = false;
		RefreshSelfOnly();

		HexCell neighbor = GetNeighbor(incomingRiver);
		neighbor.hasOutgoingRiver = false;
		neighbor.RefreshSelfOnly();
	}

	public void RemoveRiver()
	{
		RemoveOutgoingRiver();
		RemoveIncomingRiver();
	}

	public void SetOutgoingRiver(HexDirection direction)
	{
		if (hasOutgoingRiver && outgoingRiver == direction) { return; }

		HexCell neighbor = GetNeighbor(direction);
		if (!neighbor || elevation < neighbor.elevation) { return; }

		RemoveOutgoingRiver();
		if (hasIncomingRiver && incomingRiver == direction)
		{
			RemoveIncomingRiver();
		}

		hasOutgoingRiver = true;
		outgoingRiver = direction;
		RefreshSelfOnly();

		neighbor.RemoveIncomingRiver();
		neighbor.hasIncomingRiver = true;
		neighbor.incomingRiver = direction.Opposite();
		neighbor.RefreshSelfOnly();
	}


#endregion


}

public enum HexDirection
{
	NE, E, SE, SW, W, NW
}

public static class HexDirectionExtensions {
	public static HexDirection Opposite (this HexDirection direction)
	{
		return (int)direction < 3 ? (direction + 3) : (direction - 3);
	}
	public static HexDirection Previous (this HexDirection direction)
	{
		return direction == HexDirection.NE ? HexDirection.NW : (direction - 1);
	}
	public static HexDirection Next (this HexDirection direction)
	{
		return direction == HexDirection.NW ? HexDirection.NE : (direction + 1);
	}
}


public enum HexEdgeType
{
	Flat, Slope, Cliff
}
