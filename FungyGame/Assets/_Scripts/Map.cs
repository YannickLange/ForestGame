using UnityEngine;
using System.Collections;
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
	//Size parameters
	public Material NormalMaterial;
	public Material HighlightedMaterial;
	public Material SurroundingValidMaterial;
	public Material SurroundingInvalidMaterial;
	private Hexagon _prevHexagon = null;
	//Array for the 6 surrounding tiles of the selected hexagon
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

	private Hexagon[] GetSurroundingTiles (Hexagon hexagon)
	{
		int x = hexagon.X;
		int y = hexagon.Y;
		Hexagon[] surroundingHexs = new Hexagon[6];
		#region LEFT SIDE
		//Left upper sider
		if (y % 2 != 0) {
			if (!IsOutOfBounds (x, y - 1)) {
				surroundingHexs [0] = _hexagons [x + (y - 1) * _arrayOffset];
			} else {
				surroundingHexs [0] = null;
			}
		} else if (!IsOutOfBounds (x - 1, y - 1))
			surroundingHexs [0] = _hexagons [(x - 1) + (y - 1) * _arrayOffset];
		else
			surroundingHexs [0] = null;


		//Left tile
		if (!IsOutOfBounds (x - 1, y))
			surroundingHexs [1] = _hexagons [(x - 1) + y * _arrayOffset];
		else
			surroundingHexs [1] = null;
		//Left down tile
		if (y % 2 != 0) {
			if (!IsOutOfBounds (x, y + 1))
				surroundingHexs [2] = _hexagons [x + (y + 1) * _arrayOffset];
			else
				surroundingHexs [2] = null;
		} else if (!IsOutOfBounds (x - 1, y + 1))
			surroundingHexs [2] = _hexagons [(x - 1) + (y + 1) * _arrayOffset];
		else
			surroundingHexs [2] = null;

		#endregion
		#region RIGHT SIDE
		//Right down tile
		if (y % 2 == 0) {
			if (!IsOutOfBounds (x, y + 1))
				surroundingHexs [3] = _hexagons [x + (y + 1) * _arrayOffset];
			else
				surroundingHexs [3] = null;
		} else if (!IsOutOfBounds (x + 1, y + 1))
			surroundingHexs [3] = _hexagons [(x + 1) + (y + 1) * _arrayOffset];
		else
			surroundingHexs [3] = null;
		//Right tile
		if (!IsOutOfBounds (x + 1, y))
			surroundingHexs [4] = _hexagons [(x + 1) + y * _arrayOffset];
		else
			surroundingHexs [4] = null;

		if (y % 2 == 0) {
			if (!IsOutOfBounds (x, y - 1))
				surroundingHexs [5] = _hexagons [x + (y - 1) * _arrayOffset];
			else
				surroundingHexs [5] = null;
		} else if (!IsOutOfBounds (x + 1, y - 1))
			surroundingHexs [5] = _hexagons [(x + 1) + (y - 1) * _arrayOffset];
		else
			surroundingHexs [5] = null;
		#endregion

		return surroundingHexs;
	}

	private void SetSurroundingTilesHighlighted (Hexagon hexagon)
	{
		var surroundingHexagons = GetSurroundingTiles (hexagon);
		for (int i = 0; i < 6; i++) {
			if (surroundingHexagons [i] != null) {
				if (surroundingHexagons [i].HexTree != null)
					surroundingHexagons [i].HexagonRenderer.material = SurroundingValidMaterial;
				else
					surroundingHexagons [i].HexagonRenderer.material = SurroundingInvalidMaterial;
			}
		}
	}

	private bool IsOutOfBounds (int x, int y)
	{
		return (y < 0 || x >= GridManager.instance.gridWidthInHexes || y >= GridManager.instance.gridWidthInHexes);
	}

	private void ResetSurroundingTilesHighlighted (Hexagon hexagon)
	{
		var surroundingHexagons = GetSurroundingTiles (hexagon);
		for (int i = 0; i < 6; i++) {
			if (surroundingHexagons [i] != null)
				surroundingHexagons [i].HexagonRenderer.material = NormalMaterial;
		}
	}

	private void OnHexagonClickedEvent (object sender, EventArgs e, int clickID)
	{
		Hexagon hex = sender as Hexagon;
		switch (clickID) {
		case 0:
			if (_prevHexagon != null)
				_prevHexagon.HexagonRenderer.material = NormalMaterial;

			ResetSurroundingTilesHighlighted (_prevHexagon);
			_prevHexagon = hex;

			hex.HexagonRenderer.material = HighlightedMaterial;
			SetSurroundingTilesHighlighted (_prevHexagon);

			break;
		case 1:
			//Checking if the selected hexgon is in the surroundings
			var surroundingTiles = GetSurroundingTiles (hex);
			foreach (var surroundingTile in surroundingTiles){
				if (surroundingTile == hex){
					Debug.Log ("Perform action on the selected hexagon...[Infect/Expand/Other?]");
					break;
				}
			}
			break;
		}
	}
}
