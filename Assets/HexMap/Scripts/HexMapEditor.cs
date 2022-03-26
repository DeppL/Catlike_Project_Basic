using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HexMapEditor : MonoBehaviour
{
	public Color[] colors = {};
	public HexGrid? hexGrid;
	private Color activeColor;

	private void Awake()
	{
		SelectColor(0);
	}

	private void Update()
	{
		if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
		{
			HandleInput();
		}
		else
		{
			previousCell = null;
		}

	}

	void HandleInput()
	{
		var inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit) && hexGrid is HexGrid)
		{
			HexCell currentCell = hexGrid.GetCell(hit.point);
			if (previousCell && previousCell != currentCell)
			{
				ValidateDrag(currentCell);
			}
			else
			{
				isDrag = false;
			}

			EditCells(currentCell);
			previousCell = currentCell;
		}
		else
		{
			previousCell = null;
		}
	}
	void ValidateDrag(HexCell currentCell)
	{
		previousCell = previousCell ?? throw new System.Exception();
		for (
			dragDirection = HexDirection.NE;
			dragDirection <= HexDirection.NW;
			dragDirection++
		) {
			if (previousCell.GetNeighbor(dragDirection) == currentCell)
			{
				isDrag = true;
				return;
			}
		}
		isDrag = false;
	}
	int activeElevation;
	public void SetElevation (float elevation)
	{
		activeElevation = (int)elevation;
	}

	bool applyColor;
	public void SelectColor (int index)
	{
		applyColor = index >= 0;
		if (applyColor)
		{
			activeColor = colors[index];
		}
	}
	int brushSize;
	public void SetBrushSize (float size)
	{
		brushSize = (int)size;
	}

	bool applyElevation = true;
	void EditCell(HexCell? cell)
	{
		if (cell != null)
		{
			if (applyColor)
			{
				cell.Color = activeColor;
			}
			if (applyElevation)
			{
				cell.Elevation = activeElevation;
			}
			if (riverMode == OptionalToggle.No)
			{
				cell.RemoveRiver();
			}
			else if (isDrag && riverMode == OptionalToggle.Yes)
			{
				HexCell otherCell = cell.GetNeighbor(dragDirection.Opposite());
				if (otherCell != null)
				{
					otherCell.SetOutgoingRiver(dragDirection);
				}
			}
		}
	}
	void EditCells(HexCell center)
	{
		if (hexGrid == null) { throw new System.Exception(); }
		int centerX = center.coordinates.X;
		int centerZ = center.coordinates.Z;

		for (int r = 0, z = centerZ - brushSize; z <= centerZ; z++, r++)
		{
			for (int x = centerX - r; x <= centerX + brushSize; x++)
			{
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}

		for (int r = 0, z = centerZ + brushSize; z > centerZ; z--, r++)
		{
			for (int x = centerX - brushSize; x <= centerX + r; x++)
			{
				EditCell(hexGrid.GetCell(new HexCoordinates(x, z)));
			}
		}
	}

	public void SetApplyElevation(bool toggle)
	{
		applyElevation = toggle;
	}

	public void ShowUI(bool visible)
	{
		hexGrid?.ShowUI(visible);
	}
#region River
	enum OptionalToggle {
		Ignore, Yes, No
	}
	OptionalToggle riverMode;

	public void SetRiverMode (int mode)
	{
		riverMode = (OptionalToggle)mode;
	}

	bool isDrag;
	HexDirection dragDirection;
	HexCell? previousCell;

#endregion
}
