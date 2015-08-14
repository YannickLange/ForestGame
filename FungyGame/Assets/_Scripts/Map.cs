using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Map : MonoBehaviour
{
	//TODO: CHANGE THIS FUGLY
	public GameObject[] TreeTypes;
	//<<<<<<<<<<<<<<<<<<
	private TreeGenerator treeGenerator;
	//private array of hexagons
	private Hexagon[] _hexagons;
	//Readonly hexagons
	public Hexagon[] Hexagons {
		get {
			return _hexagons;
		}
	}

	private Hexagon _prevHexagon = null;
	private int _arrayOffset = 0;
	//Map singleton
	public static Map instance = null;

	void Awake ()
	{
		//Check if instance already exists
		if (instance == null) {
			//if not, set instance to this
			instance = this;
		}
        //If instance already exists and it's not this:
        else if (instance != this) {
			//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
			Destroy (gameObject);
		}
		DontDestroyOnLoad (gameObject);
	}

	void Start ()
	{
		BuildMap ();
	}

	public void BuildMap ()
	{
		if (GridManager.instance.gridWidthInHexes >= GridManager.instance.gridHeightInHexes)
			_arrayOffset = GridManager.instance.gridWidthInHexes;
		else
			_arrayOffset = GridManager.instance.gridHeightInHexes;
		_hexagons = new Hexagon[GridManager.instance.gridWidthInHexes * GridManager.instance.gridHeightInHexes];
		for (int x = 0; x < GridManager.instance.gridWidthInHexes; x++)
			for (int y = 0; y < GridManager.instance.gridHeightInHexes; y++) {
				_hexagons [x + y * _arrayOffset] = GridManager.instance.CreateHexagonAt (x, y);
				_hexagons [x + y * _arrayOffset].ClickEvent += new HexagonEventHandler (this.OnHexagonClickedEvent);
			}

		treeGenerator = new TreeGenerator (TreeTypes);
	}

	private List<Hexagon> GetSurroundingTiles (Hexagon hexagon)
	{
		if (hexagon == null)
			return new List<Hexagon> ();
		int x = hexagon.X;
		int y = hexagon.Y;
		List<Hexagon> surroundingHexs = new List<Hexagon> ();
		#region LEFT SIDE
		//Left upper sider
		if (y % 2 != 0) {
			if (!IsOutOfBounds (x, y - 1)) {
				surroundingHexs.Add (_hexagons [x + (y - 1) * _arrayOffset]);
			}
		} else if (!IsOutOfBounds (x - 1, y - 1)) {
			surroundingHexs.Add (_hexagons [(x - 1) + (y - 1) * _arrayOffset]);
		}


		//Left tile
		if (!IsOutOfBounds (x - 1, y)) {
			surroundingHexs.Add (_hexagons [(x - 1) + y * _arrayOffset]);
		}
		//Left down tile
		if (y % 2 != 0) {
			if (!IsOutOfBounds (x, y + 1)) {
				surroundingHexs.Add (_hexagons [x + (y + 1) * _arrayOffset]);
			}
		} else if (!IsOutOfBounds (x - 1, y + 1)) {
			surroundingHexs.Add (_hexagons [(x - 1) + (y + 1) * _arrayOffset]);
		}

		#endregion
		#region RIGHT SIDE
		//Right down tile
		if (y % 2 == 0) {
			if (!IsOutOfBounds (x, y + 1))
				surroundingHexs.Add (_hexagons [x + (y + 1) * _arrayOffset]);
		} else if (!IsOutOfBounds (x + 1, y + 1))
			surroundingHexs.Add (_hexagons [(x + 1) + (y + 1) * _arrayOffset]);
		//Right tile
		if (!IsOutOfBounds (x + 1, y))
			surroundingHexs.Add (_hexagons [(x + 1) + y * _arrayOffset]);

		if (y % 2 == 0) {
			if (!IsOutOfBounds (x, y - 1))
				surroundingHexs.Add (_hexagons [x + (y - 1) * _arrayOffset]);
		} else if (!IsOutOfBounds (x + 1, y - 1))
			surroundingHexs.Add (_hexagons [(x + 1) + (y - 1) * _arrayOffset]);
		#endregion

		return surroundingHexs;
	}

	private void SetSurroundingTilesHighlighted (Hexagon hexagon)
	{
		foreach (var surroundingHexagon in GetSurroundingTiles (hexagon)) {
			if (surroundingHexagon.HexTree != null) {
				surroundingHexagon.currentState = Hexagon.State.CanMoveThere;
			} else {
				surroundingHexagon.currentState = Hexagon.State.CannotMoveThere;
			}
		}
	}

	private bool IsOutOfBounds (int x, int y)
	{
		return (y < 0 || x >= GridManager.instance.gridWidthInHexes || y >= GridManager.instance.gridWidthInHexes);
	}

	private void ResetSurroundingTilesHighlighted (Hexagon hexagon)
	{
		foreach (var surroundingHexagon in GetSurroundingTiles (hexagon)) {
			surroundingHexagon.currentState = Hexagon.State.Normal;
		}
	}

	private void OnHexagonClickedEvent (object sender, EventArgs e, int clickID)
	{
		Hexagon hex = sender as Hexagon;
		switch (clickID) {
		case 0:
			if (_prevHexagon != null)
				_prevHexagon.currentState = Hexagon.State.Normal;

			ResetSurroundingTilesHighlighted (_prevHexagon);
			_prevHexagon = hex;
			
			hex.currentState = Hexagon.State.IsSelected;
			SetSurroundingTilesHighlighted (_prevHexagon);

			break;
		case 1:
			//Checking if the selected hexgon is in the surroundings
			var surroundingTiles = GetSurroundingTiles (hex);
			foreach (var surroundingTile in surroundingTiles) {
				if (surroundingTile == hex) {
					Debug.Log ("Perform action on the selected hexagon...[Infect/Expand/Other?]");
					break;
				}
			}
			break;
		}
	}
}
